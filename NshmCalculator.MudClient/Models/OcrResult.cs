using System.Text.Json.Serialization;

namespace NshmCalculator.MudClient.Models;

/// <summary>
/// 文字框（ppu-paddle-ocr 6.x 的 box 结构：左上角坐标 + 宽高）。
/// </summary>
public class OcrBox
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}

/// <summary>
/// 单个文字块的识别结果。字段名与 JS 端 ocr.selfhost.js 归一化后的结构一致。
/// </summary>
public class OcrItem
{
    /// <summary>识别出的文字</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>置信度 0~1（引擎字段名为 confidence，用 JsonPropertyName 映射）</summary>
    [JsonPropertyName("confidence")]
    public double Score { get; set; }

    /// <summary>文字框，可能为 null</summary>
    public OcrBox? Box { get; set; }
}

/// <summary>
/// 整张图的识别结果，对应 JS 端 recognize() 的返回。
/// </summary>
public class OcrResult
{
    /// <summary>全文（引擎已按行用换行拼接）</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>平均置信度</summary>
    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    /// <summary>所有文字块</summary>
    public List<OcrItem> Items { get; set; } = new();

    /// <summary>耗时(ms)</summary>
    public double Elapsed { get; set; }
}

/// <summary>
/// 自托管模式模型/ort 缓存完整性状态。
/// 字段名与 wwwroot/js/ocr/cache-check.js 的 checkCacheComplete() 返回一致。
/// </summary>
public class CacheStatus
{
    /// <summary>浏览器是否支持 Cache API（不支持则无法检测）</summary>
    public bool Supported { get; set; }

    /// <summary>必需文件是否都在缓存里</summary>
    public bool Complete { get; set; }

    /// <summary>缺失的文件名列表</summary>
    public List<string> Missing { get; set; } = new();

    /// <summary>应有文件数</summary>
    public int Total { get; set; }
}
