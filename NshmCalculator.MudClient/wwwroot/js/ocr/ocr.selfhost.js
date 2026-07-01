// =====================================================================
// 自托管模式专用 OCR 封装层（ES module）
//
// 与预设模式的区别：import 改为本地 npm 包（由 tools/bundle.mjs 用 esbuild
// 打成单文件 bundle），并显式把 ONNX Runtime Web 的 .wasm/.mjs 胶水路径
// 指向本地 wwwroot/ocr-ort/。于是「自托管模型」模式下库与模型都不再依赖
// 任何 CDN，可彻底断网运行。
//
// 入口导出同名函数（initialize / recognize / destroy）；C# 端 WebOcrEngine
// import 打包产物 ocr.selfhost.bundle.js。
//
// 重要：所有异常都向上抛出，由 C# 端捕获并显示给用户，绝不静默吞掉。
// =====================================================================

import {
  PaddleOcrService,
  isWebGpuAvailable,
} from "ppu-paddle-ocr/web";
// 注意：导入主入口（而非 /web 子路径）——因为 ppu-paddle-ocr 内部的
// platform.web.js 也是 `import * as ort from "onnxruntime-web"`。两边必须
// 指向同一个模块实例，下面设置的 ort.env.wasm.* 才会被库识别。
import * as ort from "onnxruntime-web";
// 项目自带的 brotli 解码器（Blazor 模板自带，index.html 也用它解压 dotnet 资源）。
// 用它把预压缩的 ort wasm .wasm.br 解压成 ArrayBuffer，绕过 ort 对原始 .wasm
// 的 fetch——从而让自托管发布产物里不含 >25M 的单文件（满足静态托管商体积限制）。
import { BrotliDecode } from "../decode.min.js";

// ONNX Runtime Web 运行时文件名（与 tools/bundle.mjs 复制/压缩的产物一致）。
// 注意：胶水用 .glue.js 而非源文件的 .mjs——很多静态托管商对 .mjs 返回
// application/octet-stream，会被浏览器的严格 MIME 检查拒绝（动态 import 失败）；
// 改成 .js 后会被正确映射为 text/javascript。详见 tools/bundle.mjs 注释。
//
// ORT_DIR 为「相对 wwwroot 根」的目录名。本 bundle 部署在 wwwroot/js/ocr/ 下，
// 故下方用 new URL("../../"+ORT_DIR+..., import.meta.url) 生成绝对 URL。
const ORT_DIR = "ocr-ort/";
const ORT_MJS = "ort-wasm-simd-threaded.jsep.glue.js";
const ORT_WASM_BR = "ort-wasm-simd-threaded.jsep.wasm.br";

let service = null;
let dotNetRef = null;

// 把状态/错误文本推回 .NET（WebOcrEngine.ReportStatus）
function report(msg) {
  console.log("[ocr/selfhost]", msg);
  if (dotNetRef) {
    try { dotNetRef.invokeMethod("ReportStatus", msg); } catch (_) { /* 回调本身失败不再上报 */ }
  }
}

// 把进度百分比推回 .NET（WebOcrEngine.ReportProgress，0~100）
function reportProgress(percent) {
  if (dotNetRef) {
    try { dotNetRef.invokeMethod("ReportProgress", Math.max(0, Math.min(100, Math.round(percent)))); } catch (_) { /* 忽略 */ }
  }
}

// 带进度的 fetch：流式读取 response.body，按已读字节 / Content-Length 回报百分比。
//   onLength : 可选回调，拿到响应头里的 Content-Length 时触发（供外部累加总字节）
//   onRatio  : 可选回调，接收「本文件内部完成比例」(0~1)
// 失败抛异常。
async function fetchWithProgress(url, { onLength, onRatio } = {}) {
  const resp = await fetch(url, { cache: "no-cache" });
  if (!resp.ok) {
    throw new Error(`无法加载 ${url}（HTTP ${resp.status}）。请确认产物已部署。`);
  }
  const total = Number(resp.headers.get("Content-Length")) || 0;
  if (onLength && total > 0) onLength(total);
  const reader = resp.body.getReader();
  const chunks = [];
  let received = 0;
  for (;;) {
    const { done, value } = await reader.read();
    if (done) break;
    chunks.push(value);
    received += value.length;
    if (total > 0 && onRatio) onRatio(received / total);
  }
  if (onRatio && total > 0) onRatio(1);
  // 拼装成单个 ArrayBuffer
  const blob = new Blob(chunks);
  return await blob.arrayBuffer();
}

// 拉取本地预压缩的 ort wasm（.wasm.br）并用 BrotliDecode 解压成 ArrayBuffer。
//   rangeStart/rangeEnd : 该文件在整体进度条中所占的百分比区间
async function loadWasmBinary(rangeStart, rangeEnd) {
  report(`下载并解压OCR运行时环境…`);
  const buf = await fetchWithProgress(ORT_DIR + ORT_WASM_BR, {
    onRatio: (r) => reportProgress(rangeStart + (rangeEnd - rangeStart) * r),
  });
  // BrotliDecode: Int8Array(压缩) -> Int8Array(解压)
  const decompressed = BrotliDecode(new Int8Array(buf));
  return decompressed.buffer;
}

// ---------------------------------------------------------------------
// 初始化引擎。由 C# WebOcrEngine.InitializeAsync 调用。
//   dotRef : DotNetObjectReference，用于回报状态/错误
// ---------------------------------------------------------------------
export async function initialize(dotRef) {
  dotNetRef = dotRef || null;
  report("开始加载 OCR 引擎（自托管模式：库与模型均来自本地）…");

  // 探测 WebGPU，仅用于状态提示；引擎内部会自动选择后端
  try {
    if (await isWebGpuAvailable()) {
      report("检测到 WebGPU 支持，将优先使用 GPU 加速。");
    } else {
      report("当前浏览器不支持 WebGPU，将使用 WASM 后端（速度较慢）。");
    }
  } catch (_) { /* 探测失败不影响后续，引擎自会回退 */ }

  // 自托管模型：放于 wwwroot/ocr-models/，文件名见 cache-check.js。
  // 关键：库的 _loadResource 支持直接传入 ArrayBuffer（见 PaddleOcrService 源码，
  //   if (source instanceof ArrayBuffer) return source），所以我们自己带进度地
  //   fetch 这三个文件，再以 ArrayBuffer 形式喂给库，从而拿到精确下载进度。
  report("使用自托管模型（wwwroot/ocr-models）。");
  const MODEL_FILES = {
    detection: "ocr-models/det.onnx",
    recognition: "ocr-models/rec.onnx",
    charactersDictionary: "ocr-models/dict.txt",
  };

  // 阶段②：并行下载 3 个模型文件（占整体进度 15%→98%）。
  // 按各自字节占比加权聚合进度，rec.onnx 最大、贡献最多，符合真实感受。
  // 不预先发 HEAD（部分静态托管不支持 HEAD）；改为从每个 GET 响应的
  // Content-Length 动态累加总字节，边下边修正权重。
  report("下载识别模型…");
  const sizes = {};      // 每个文件的总字节（从 GET 响应 Content-Length 取）
  const got = {};        // 每个文件已读字节
  Object.keys(MODEL_FILES).forEach((k) => { sizes[k] = 0; got[k] = 0; });
  let totalBytes = 0;

  const RANGE_START = 15, RANGE_END = 98;
  const buffers = {};
  await Promise.all(Object.keys(MODEL_FILES).map(async (key) => {
    const url = MODEL_FILES[key];
    buffers[key] = await fetchWithProgress(url, {
      onLength: (len) => {
        // 首次拿到该文件长度时累加进总量
        if (sizes[key] === 0 && len > 0) {
          sizes[key] = len;
          totalBytes += len;
        }
      },
      onRatio: (r) => {
        got[key] = sizes[key] * r;
        if (totalBytes > 0) {
          const sumGot = Object.keys(got).reduce((a, k) => a + got[k], 0);
          reportProgress(RANGE_START + (RANGE_END - RANGE_START) * (sumGot / totalBytes));
        }
      },
    });
  }));

  // model 用 ArrayBuffer 形式（库会直接用，不再自行 fetch）
  const model = {
    detection: buffers.detection,
    recognition: buffers.recognition,
    charactersDictionary: buffers.charactersDictionary,
  };

  // 指定 ort 运行时胶水（.mjs）的本地路径；wasm 二进制通过 wasmBinary
  // 直接喂给 ort（见下），故 wasmPaths 只需指向 .mjs。
  //
  // 关键：ort 通过动态 import() 加载 .mjs，浏览器只接受「相对/绝对 URL」，
  // 不接受裸模块名。这里必须给出绝对 URL，否则报
  // "Failed to resolve module specifier 'ort/...mjs'"。
  // 用 new URL(.., import.meta.url) 生成绝对 URL：打包后 import.meta.url 指向
  // bundle 所在的 wwwroot/js/ocr/ 目录，故相对路径写 ../../ort/... 才能回到 wwwroot 根。
  // （这种写法部署在根路径或子路径都正确。）
  ort.env.wasm.wasmPaths = {
    mjs: new URL("../../" + ORT_DIR + ORT_MJS, import.meta.url).href,
  };

  // 阶段①：下载并解压 ort wasm（占整体进度 0%→15%）。赋给 wasmBinary 后
  // ort 会走 `locateFile=$=>$` 路径，完全跳过对 .wasm 的 fetch（含 pthread worker）。
  ort.env.wasm.wasmBinary = await loadWasmBinary(0, 15);

  // 构造服务。executionProviders 让引擎自动处理 WebGPU→WASM 回退。
  service = new PaddleOcrService({
    model,
    // 处理后端：canvas-native 无需 OpenCV.js，依赖更轻、加载更快
    processing: { engine: "canvas-native" },
  });

  report("创建推理会话中…");
  // initialize() 此时模型已是 ArrayBuffer，无需网络；这里主要是 ONNX 会话编译。
  // 编译进度无法精确，固定停在 98% 直至完成。
  await service.initialize();
  reportProgress(100);
  report("OCR 引擎初始化完成（自托管模式）。");
}

// ---------------------------------------------------------------------
// 识别一张图片。由 C# WebOcrEngine.RecognizeAsync 调用。
//   imageBytes : 图片的字节数组（Blazor 传入）
// 返回归一化结构 { text, confidence, items:[{text, confidence, box}], elapsed }
// ---------------------------------------------------------------------
export async function recognize(imageBytes) {
  if (!service) throw new Error("OCR 引擎未初始化，请先点击「加载引擎」。");

  // 引擎 recognize 直接接受 ArrayBuffer
  const u8 = imageBytes instanceof Uint8Array ? imageBytes : new Uint8Array(imageBytes);
  // Uint8Array 的 .buffer 可能不是独立 ArrayBuffer（带 byteOffset），复制一份保证整段可用
  const buffer = u8.byteOffset === 0 && u8.byteLength === u8.buffer.byteLength
    ? u8.buffer
    : u8.slice().buffer;

  const t0 = performance.now();
  // flatten:true → 返回 { text, results:[{text,box,confidence}], confidence }
  const raw = await service.recognize(buffer, { flatten: true });
  const elapsed = performance.now() - t0;

  // 归一化字段，兼容两种返回形态（保险起见）
  const items = (raw?.results || raw?.lines?.flat() || []).map((it) => ({
    text: it.text ?? "",
    confidence: typeof it.confidence === "number" ? it.confidence : 0,
    // box: {x,y,width,height}
    box: it.box || null,
  }));

  return {
    text: raw?.text ?? items.map((i) => i.text).join("\n"),
    confidence: raw?.confidence ?? 0,
    items,
    elapsed,
  };
}

// ---------------------------------------------------------------------
// 释放引擎资源
// ---------------------------------------------------------------------
export async function destroy() {
  try {
    if (service && typeof service.destroy === "function") {
      await service.destroy();
    }
  } catch (_) { /* 释放阶段忽略 */ }
  service = null;
  dotNetRef = null;
}

// 兜底：页面卸载时尽量释放
if (typeof window !== "undefined") {
  window.addEventListener("beforeunload", () => { if (service) service.destroy(); });
}
