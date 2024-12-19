using System.Text.Json;
using Blazored.LocalStorage;
using NshmCalculator.Shared.Models.BaseModel;

namespace NshmCalculator.MudClient.Utilities;

/// <summary>
/// 配置文件帮助类
/// </summary>
public static class ConfigHelper
{
    private static AppVersionInfo _info;

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
            Console.WriteLine("无可用版本配置，正在初始化中");
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

        Console.WriteLine($"获取配置完成-{code}");

        return (configJson, needUpdate);
    }
}