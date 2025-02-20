using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using Blazored.LocalStorage;
using NshmCalculator.Shared.Models.BaseModel;

namespace NshmCalculator.MudClient.Utilities;

/// <summary>
/// 配置文件帮助类
/// </summary>
public static class ConfigHelper
{
    private static AppVersionInfo _info;
    private const string UrlPrefix = "https://textdb.online/";
    private const string ApiUrlPrefix = "https://api.textdb.online/";
    private const string TempCodeStr = "tmpCode";
    public const string CodeTimeStr = "tmpCodeGenerateTimeStamp";

    /// <summary>
    /// 获取基础配置文件
    /// </summary>
    /// <param name="client">调用时拥有的<see cref="HttpClient"/>对象</param>
    /// <returns></returns>
    public static async Task<bool> InitAppVersion(HttpClient client)
    {
        long timeTicks = DateTime.Now.Ticks;
        var versionJson = await client.GetStringAsync(ConstText.VersionPath + $"?t={timeTicks}");
        if (!string.IsNullOrEmpty(versionJson))
        {
            _info = JsonSerializer.Deserialize<AppVersionInfo>(versionJson);
        }

        return !string.IsNullOrEmpty(versionJson);
    }

    /// <summary>
    /// 获取指定的配置文件，并根据版本状况决定是否强制刷新
    /// </summary>
    /// <param name="client">调用时拥有的<see cref="HttpClient"/>对象</param>
    /// <param name="code">配置文件代号</param>
    /// <param name="service">调用时的<see cref="LocalStorageService">本地存储</see>实例</param>
    /// <returns></returns>
    public static async Task<(string, bool)> GetConfigJson(HttpClient client, string code, ISyncLocalStorageService service)
    {
        if (_info == null)
        {
            InitAppVersion(client);
        }

        if (!_info.ConfigVersionInfo.ContainsKey(code))
        {
            return (ConstText.NotExistText, false);
        }

        long versionNumber = long.MinValue;
        if (service.ContainKey($"config_{code}"))
        {
            versionNumber = service.GetItem<long>($"config_{code}");
        }

        bool needUpdate = versionNumber < _info.ConfigVersionInfo[code];

        long timeTicks = DateTime.Now.Ticks;
        var configJson = await client.GetStringAsync(_info.ConfigPathInfo[code] + $"?v={_info.ConfigVersionInfo[code]}");

        if (needUpdate)
        {
            service.SetItem($"config_{code}", _info.ConfigVersionInfo[code]);
        }

        return (configJson, needUpdate);
    }

    /// <summary>
    /// 读取在线服务内的文本
    /// </summary>
    /// <param name="client">调用时拥有的<see cref="HttpClient"/>对象</param>
    /// <param name="code">在导出至在线服务时使用的代号</param>
    /// <returns></returns>
    public static async Task<string> LoadOnlineConfig(HttpClient client, string code)
    {
        long timeTicks = DateTime.Now.Ticks / 1000;
        string readUrl = $"{UrlPrefix}{code}?t={timeTicks}";
        string resultJson = string.Empty;

        try
        {
            resultJson = await client.GetStringAsync(readUrl);
        }
        catch (Exception)
        {
        }

        return resultJson;
    }

    /// <summary>
    /// 上传配置文本至在线服务
    /// </summary>
    /// <param name="client">调用时拥有的<see cref="HttpClient"/>对象</param>
    /// <param name="code">在上传至在线服务时使用的代号</param>
    /// <param name="jsonStr">需要上传的内容</param>
    /// <returns></returns>
    public static async Task<bool> UpdateConfigOnline(HttpClient client, string code, string jsonStr)
    {
        string updateUrl = $"{ApiUrlPrefix}update";
        string postData = $"key={code}&value={HttpUtility.UrlEncode(jsonStr, Encoding.UTF8)}";
        var responseMessage = await client.PostAsync(updateUrl, new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded"));
        if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return false;
        }

        string resultJson = await responseMessage.Content.ReadAsStringAsync();
        var entity = JsonSerializer.Deserialize<UploadStatus>(resultJson);
        return entity is { status: 1 };
    }


    /// <summary>
    /// 初始化httpclient
    /// </summary>
    /// <param name="client">调用时拥有的<see cref="HttpClient"/>对象</param>
    public static async Task InitClient(HttpClient client)
    {
        client.Timeout = TimeSpan.FromSeconds(1);
        await client.GetAsync(UrlPrefix);
        client.Timeout = TimeSpan.FromSeconds(15);
    }

    /// <summary>
    /// 根据本地存储获取临时交换代码
    /// </summary>
    /// <param name="service">本地存储服务对象</param>
    /// <param name="readMode">只读模式，在希望仅获取而不修改状态时传入，默认为<c>false</c></param>
    /// <returns></returns>
    public static (string, int) GenerateTempCode(ISyncLocalStorageService service, bool readMode = false)
    {
        DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0);
        string randomCode = string.Empty;
        int totalSeconds = -1;
        if (service.ContainKey(TempCodeStr))
        {
            int timestamp = service.GetItem<int>(CodeTimeStr);

            if (DateTime.Now.Subtract(baseTime.AddSeconds(timestamp)).TotalMinutes <= 30)
            {
                randomCode = service.GetItemAsString(TempCodeStr);
                totalSeconds = timestamp;
            }
        }

        if (string.IsNullOrEmpty(randomCode) && !readMode)
        {
            totalSeconds = Convert.ToInt32(DateTime.Now.Subtract(baseTime).TotalSeconds);
            randomCode = $"nshmCalculator_{Guid.NewGuid():N}";
            service.SetItemAsString(TempCodeStr, randomCode);
            service.SetItem(CodeTimeStr, totalSeconds);
        }

        return (randomCode, totalSeconds);
    }
}