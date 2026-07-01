using System.Diagnostics;
using Microsoft.JSInterop;
using NshmCalculator.MudClient.Models;

namespace NshmCalculator.MudClient.Services;

/// <summary>
/// 浏览器端 OCR 引擎的 C# 封装。
/// 通过 JS Interop 调用 wwwroot/js/ocr/ocr.selfhost.bundle.js（封装了 ppu-paddle-ocr）。
/// 推理全程在浏览器内（WebAssembly / WebGPU）完成，不依赖任何后端服务。
/// 仅支持自托管模式：库与模型均来自本地 wwwroot/ocr-models 与 wwwroot/ocr-ort，彻底断网可用。
/// </summary>
public class WebOcrEngine : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private readonly DotNetObjectReference<WebOcrEngine> _dotRef;
    private IJSObjectReference? _module;
    private bool _initialized;

    /// <summary>用于把状态文本推给 UI（调用方订阅）。</summary>
    public event Action<string>? OnStatus;

    /// <summary>下载进度：0~100。由 JS 端在 fetch 模型/wasm 时按已读字节回报。</summary>
    public event Action<int>? OnProgress;

    /// <summary>引擎是否已加载就绪。</summary>
    public bool IsReady => _initialized;

    public WebOcrEngine(IJSRuntime js)
    {
        _js = js;
        _dotRef = DotNetObjectReference.Create(this);
    }

    /// <summary>
    /// 检测自托管模式的模型/ort 文件是否已被 Service Worker 缓存齐全。
    /// 仅用于 UI 提示（不阻塞加载）。返回 null 表示检测失败（如 JS interop 异常）。
    /// </summary>
    public async Task<CacheStatus?> CheckCacheAsync()
    {
        try
        {
            var mod = await _js.InvokeAsync<IJSObjectReference>("import", "./js/ocr/cache-check.js");
            return await mod.InvokeAsync<CacheStatus>("checkCacheComplete");
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 加载 OCR 引擎并下载模型。仅在用户点击「加载引擎」按钮后调用一次。
    /// 自托管模式：库与 wasm 胶水都已本地化（ocr.selfhost.bundle.js），模型来自 wwwroot/ocr-models。
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_initialized) return;

        const string modulePath = "./js/ocr/ocr.selfhost.bundle.js";
        _module ??= await _js.InvokeAsync<IJSObjectReference>("import", modulePath);

        OnStatus?.Invoke("正在下载 OCR 引擎与模型，首次约 21 MB …");
        var sw = Stopwatch.StartNew();

        // 调用 JS initialize，传入 .NET 回调引用（用于状态/进度回报）
        await _module.InvokeVoidAsync("initialize", _dotRef);

        _initialized = true;
        sw.Stop();
        OnStatus?.Invoke($"引擎就绪（加载耗时 {sw.Elapsed.TotalSeconds:F1}s）。");
    }

    /// <summary>
    /// 识别一张图片。图片字节会被传到 JS 端转为 ArrayBuffer 后送入引擎。
    /// </summary>
    public async Task<OcrResult?> RecognizeAsync(byte[] imageBytes)
    {
        if (!_initialized || _module is null)
        {
            OnStatus?.Invoke("引擎未加载，请先点击「加载引擎」。");
            return null;
        }
        OnStatus?.Invoke("识别中…");
        var sw = Stopwatch.StartNew();
        try
        {
            var result = await _module.InvokeAsync<OcrResult>("recognize", imageBytes);
            sw.Stop();
            OnStatus?.Invoke($"识别完成，耗时 {sw.Elapsed.TotalMilliseconds:F0} ms。");
            return result;
        }
        catch (Exception ex)
        {
            OnStatus?.Invoke($"识别失败：{ex.Message}");
            return null;
        }
    }

    /// <summary>JS 端通过 [JSInvokable] 调用，把进度/状态文本推回 .NET。</summary>
    [JSInvokable]
    public void ReportStatus(string message) => OnStatus?.Invoke(message);

    /// <summary>JS 端通过 [JSInvokable] 调用，回报下载进度百分比（0~100）。</summary>
    [JSInvokable]
    public void ReportProgress(int percent) => OnProgress?.Invoke(Math.Clamp(percent, 0, 100));

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_module is not null)
                await _module.InvokeVoidAsync("destroy");
            if (_module is not null)
                await _module.DisposeAsync();
        }
        catch { /* 释放阶段忽略 */ }
        _dotRef.Dispose();
    }
}
