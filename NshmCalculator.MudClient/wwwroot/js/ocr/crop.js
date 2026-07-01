// =====================================================================
// 图片识别范围框选（ES module）
//
// 在预览图上方覆盖一个 <canvas>，用户拖拽框选矩形选区；选区外半透明
// 遮罩、选区内透明 + 蓝边。识别时由 C# 调用 getCroppedStream，按选区
// 从【原图】裁剪出字节回传（IJSStreamReference）；无选区返回 null，C#
// 端走整图分支。
//
// 关键：框选发生在「显示像素」坐标，裁剪发生在「原图真实像素」坐标，
// 故裁剪前需用比例 scaleX/Y 把显示选区换算回原图选区，保证清晰度无损。
// =====================================================================

// 模块级状态：当前绑定的 canvas、上下文、选区（显示像素，已 clamp 到正值）
let canvas = null;
let ctx = null;
let selection = null;        // { x, y, w, h } 显示像素；null 表示无有效选区
let drawing = false;         // 是否正在拖拽
let startPt = null;          // 拖拽起点 { x, y }

const PRIMARY_COLOR = "#2563eb";   // 与 app.css --primary 一致
const MASK_COLOR = "rgba(0,0,0,0.45)";
const MIN_SELECTION = 10;          // 小于此尺寸视为无效（误点）

let resizeObserver = null;     // 监听画布尺寸变化（图片加载/窗口缩放），自动重绘

// ---- 工具函数 ----

// 把鼠标/触摸事件的客户端坐标转为 canvas 内部坐标（显示像素）
function getPos(e) {
  const rect = canvas.getBoundingClientRect();
  // touch 事件用 touches[0] / changedTouches[0]
  const src = e.touches && e.touches.length ? e.touches[0]
    : e.changedTouches && e.changedTouches.length ? e.changedTouches[0]
    : e;
  return {
    x: src.clientX - rect.left,
    y: src.clientY - rect.top,
  };
}

// 按 canvas 显示尺寸校准其 width/height 属性，保证 1:1 映射不模糊
function syncCanvasSize() {
  if (!canvas) return;
  const rect = canvas.getBoundingClientRect();
  const w = Math.max(1, Math.round(rect.width));
  const h = Math.max(1, Math.round(rect.height));
  // 尺寸变化时才重设（避免清空已画内容）
  if (canvas.width !== w || canvas.height !== h) {
    canvas.width = w;
    canvas.height = h;
  }
}

// 绘制：半透明遮罩盖住整图，再「挖空」选区并描边
function draw() {
  if (!ctx) return;
  syncCanvasSize();
  const w = canvas.width;
  const h = canvas.height;
  ctx.clearRect(0, 0, w, h);

  if (!selection || (selection.w < MIN_SELECTION && selection.h < MIN_SELECTION)) {
    // 无有效选区：不画遮罩，canvas 全透明（预览图清晰可见）
    return;
  }

  // 选区外的四块遮罩（上、下、左、右）
  ctx.fillStyle = MASK_COLOR;
  const { x, y, w: sw, h: sh } = selection;
  ctx.fillRect(0, 0, w, y);                 // 上
  ctx.fillRect(0, y + sh, w, h - y - sh);   // 下
  ctx.fillRect(0, y, x, sh);                // 左
  ctx.fillRect(x + sw, y, w - x - sw, sh);  // 右

  // 选区描边
  ctx.strokeStyle = PRIMARY_COLOR;
  ctx.lineWidth = 2;
  ctx.strokeRect(x + 0.5, y + 0.5, sw - 1, sh - 1);
}

// ---- 框选交互 ----

function onDown(e) {
  e.preventDefault();
  syncCanvasSize();
  drawing = true;
  startPt = getPos(e);
  selection = { x: startPt.x, y: startPt.y, w: 0, h: 0 };
  draw();
}

function onMove(e) {
  if (!drawing) return;
  e.preventDefault();
  const p = getPos(e);
  // clamp 到 canvas 范围
  const x0 = Math.max(0, Math.min(startPt.x, p.x));
  const y0 = Math.max(0, Math.min(startPt.y, p.y));
  const x1 = Math.min(canvas.width, Math.max(startPt.x, p.x));
  const y1 = Math.min(canvas.height, Math.max(startPt.y, p.y));
  selection = { x: x0, y: y0, w: x1 - x0, h: y1 - y0 };
  draw();
}

function onUp(e) {
  if (!drawing) return;
  drawing = false;
  // 选区过小视为无效（误点）→ 清空
  if (!selection || selection.w < MIN_SELECTION || selection.h < MIN_SELECTION) {
    selection = null;
    draw();
  }
}

// ---------------------------------------------------------------------
// 在指定 canvas 上初始化框选交互。由 C# OnAfterRenderAsync 调用。
//   canvasEl : 覆盖在预览图上的 <canvas> 元素
// ---------------------------------------------------------------------
export function initCrop(canvasEl) {
  // 解绑旧的（重新选图时复用同一 canvas）
  teardown();
  canvas = canvasEl;
  if (!canvas) return;
  ctx = canvas.getContext("2d");
  selection = null;

  canvas.addEventListener("mousedown", onDown);
  window.addEventListener("mousemove", onMove);
  window.addEventListener("mouseup", onUp);
  canvas.addEventListener("touchstart", onDown, { passive: false });
  canvas.addEventListener("touchmove", onMove, { passive: false });
  canvas.addEventListener("touchend", onUp);

  // 图片加载完成或窗口缩放时，canvas 显示尺寸会变，需重设像素属性并重绘。
  // 用 ResizeObserver 监听 canvas（其尺寸随底层 <img> 变化）。
  resizeObserver = new ResizeObserver(() => draw());
  resizeObserver.observe(canvas);

  // 初始绘制（无选区，透明）；draw 内部会 syncCanvasSize
  draw();
}

// 解绑事件、清状态
function teardown() {
  if (resizeObserver) {
    resizeObserver.disconnect();
    resizeObserver = null;
  }
  if (canvas) {
    canvas.removeEventListener("mousedown", onDown);
    canvas.removeEventListener("touchstart", onDown);
    canvas.removeEventListener("touchmove", onMove);
    canvas.removeEventListener("touchend", onUp);
  }
  window.removeEventListener("mousemove", onMove);
  window.removeEventListener("mouseup", onUp);
}

// ---------------------------------------------------------------------
// 查询当前是否有有效选区
// ---------------------------------------------------------------------
export function hasSelection() {
  return !!(selection && selection.w >= MIN_SELECTION && selection.h >= MIN_SELECTION);
}

// ---------------------------------------------------------------------
// 清除选区（重置）。由「重置选区」按钮调用。
// ---------------------------------------------------------------------
export function resetSelection() {
  selection = null;
  draw();
}

// ---------------------------------------------------------------------
// 关键：若有选区，按选区从【原图】裁剪出图片，返回 base64 字符串（不含
// data: 前缀，纯 base64）。无选区返回空字符串 ""。
//   说明：不返回 Blob/IJSStreamReference——Blazor 对裸 Blob 的流引用反序列化
//   会读 .buffer 失败（"Cannot read properties of null (reading 'buffer')"）。
//   改用 base64 字符串回传最稳，C# 端 Convert.FromBase64String 解码即可。
//   imageSrcUrl : 当前预览图的 data URL（即 C# 端 _imagePreviewUrl）
// ---------------------------------------------------------------------
export async function getCroppedBase64(imageSrcUrl) {
  if (!hasSelection()) return "";
  if (!imageSrcUrl) return "";

  // 1. 取原图（含真实尺寸）
  const blob = await (await fetch(imageSrcUrl)).blob();
  const bitmap = await createImageBitmap(blob);

  // 2. 显示像素 → 原图像素 比例
  // canvas 的 CSS 显示宽高对应 bitmap 的真实宽高
  const dispW = canvas.clientWidth || canvas.width;
  const dispH = canvas.clientHeight || canvas.height;
  const scaleX = bitmap.width / dispW;
  const scaleY = bitmap.height / dispH;

  // 3. 还原到原图坐标（取整、clamp 到原图范围）
  let sx = Math.round(selection.x * scaleX);
  let sy = Math.round(selection.y * scaleY);
  let sw = Math.round(selection.w * scaleX);
  let sh = Math.round(selection.h * scaleY);
  sx = Math.max(0, Math.min(sx, bitmap.width));
  sy = Math.max(0, Math.min(sy, bitmap.height));
  sw = Math.max(1, Math.min(sw, bitmap.width - sx));
  sh = Math.max(1, Math.min(sh, bitmap.height - sy));

  // 4. 离屏 canvas 按真实选区尺寸裁剪
  const off = document.createElement("canvas");
  off.width = sw;
  off.height = sh;
  const offCtx = off.getContext("2d");
  offCtx.drawImage(bitmap, sx, sy, sw, sh, 0, 0, sw, sh);
  if (bitmap.close) bitmap.close();

  // 5. 转 base64 字符串返回
  const dataUrl = off.toDataURL("image/png");
  // toDataURL 返回 "data:image/png;base64,xxxx"，去掉前缀只留 base64 部分
  const commaIdx = dataUrl.indexOf(",");
  return commaIdx >= 0 ? dataUrl.slice(commaIdx + 1) : "";
}
