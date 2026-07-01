// =====================================================================
// 模型/ort 缓存完整性检测（ES module）
//
// 自托管模式下，「加载引擎」所需的文件会由 Service Worker 缓存到
// Cache Storage。本模块在页面加载时检测它们是否齐全：
//   齐全 → 提示「已缓存，可离线加载」
//   缺失 → 提示「未缓存/不完整，首次加载需联网下载」
//
// 用 caches.match(url) 查询。SW 缓存时 key 是绝对 URL（fetch 的 Request
// 自动解析），故这里也用 new URL(rel, location.href) 构造绝对 URL 匹配。
// =====================================================================

// 自托管模式加载引擎所需的全部文件（与 ocr.selfhost.js / service-worker 一致）
const REQUIRED_FILES = [
  "ocr-models/det.onnx",
  "ocr-models/rec.onnx",
  "ocr-models/dict.txt",
  "ocr-ort/ort-wasm-simd-threaded.jsep.wasm.br",
  "ocr-ort/ort-wasm-simd-threaded.jsep.glue.js",
];

// ---------------------------------------------------------------------
// 检测缓存完整性。
// 返回 { supported: boolean, complete: boolean, missing: string[], total: number }
//   supported : 浏览器是否支持 Cache API / SW（不支持则无法检测，complete=false）
//   complete  : 文件是否都在缓存里
//   missing   : 缺失的文件名列表
//   total     : 应有文件数
// ---------------------------------------------------------------------
export async function checkCacheComplete() {
  // 不支持 Cache API（如隐私模式）→ 无法检测，按「未缓存」处理
  if (typeof caches === "undefined" || !caches.match) {
    return { supported: false, complete: false, missing: REQUIRED_FILES.slice(), total: REQUIRED_FILES.length };
  }

  const missing = [];
  for (const rel of REQUIRED_FILES) {
    // 用绝对 URL 查询，与 SW 缓存时的 key 形式一致。
    //
    // ignoreVary（关键）：公网静态托管（如 EdgeOne Pages）对所有文件统一回传
    // `Vary: Origin, Access-Control-Request-Headers, Access-Control-Request-Method`。
    // Cache API 默认把 Vary 纳入匹配判定 —— 而这些文件是「加载引擎」时由 SW 用
    // event.request 写入缓存的，写入时的 Origin 等头与本检测用裸 URL 合成请求时的
    // 头不一定一致，于是 Vary 校验失败、明明已缓存却判为缺失。统一忽略 Vary，避免误报。
    const abs = new URL(rel, location.href).href;
    const hit = await caches.match(abs, { ignoreVary: true });
    if (!hit) missing.push(rel);
  }

  // 缓存不完整时，把缺失的文件打印到控制台，方便排查。
  if (missing.length > 0) {
    console.warn(
      `[OCR] ⚠ 引擎缓存不完整：缺 ${missing.length}/${REQUIRED_FILES.length} 个文件，首次加载需联网下载：`
    );
    missing.forEach((rel, i) => {
      console.warn(
        `[OCR]   ${i + 1}. ${rel}\n            → ${new URL(rel, location.href).href}`
      );
    });
  }

  return {
    supported: true,
    complete: missing.length === 0,
    missing,
    total: REQUIRED_FILES.length,
  };
}
