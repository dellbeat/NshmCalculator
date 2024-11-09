using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using NshmCalculator.MudClient.Utilities;
using NshmCalculator.Shared.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using NshmCalculator.MudClient;
using NshmCalculator.Shared.Models.BaseModel;
using NshmCalculator.Shared.Models.CalculatorModel.KI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var client = new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
};
client.DefaultRequestHeaders.Add("Clear-Site-Data", "cache");
client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
{
    NoCache = true
};
builder.Services.AddScoped(sp => client);
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 300;
    config.SnackbarConfiguration.ShowTransitionDuration = 300;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});
builder.Services.AddBlazoredLocalStorage();

#region InitConfig

UpdateLog[] updateLogs = new UpdateLog[] { };
Dictionary<string, string> tipsDictionary = new Dictionary<string, string>();
GameData gameData = new GameData();

long timeTicks = DateTime.Now.Ticks;

var gameConfigJson = await client.GetStringAsync(ConstText.GameConfigPath + $"?t={timeTicks}");
if (!string.IsNullOrEmpty(gameConfigJson))
{
    gameData = JsonSerializer.Deserialize<GameData>(gameConfigJson);
}

var newJson = await client.GetStringAsync(ConstText.UpdateLogPath + $"?t={timeTicks}"); //需要处理缓存未更新的情况
if (!string.IsNullOrEmpty(newJson))
{
    var logs = JsonSerializer.Deserialize<UpdateLog[]>(newJson);
    if (logs is { Length: > 0 })
    {
        updateLogs = logs;
    }
}

var tipsJson = await client.GetStringAsync(ConstText.TipsJsonPath + $"?t={timeTicks}");
if (!string.IsNullOrEmpty(tipsJson))
{
    var dic = JsonSerializer.Deserialize<Dictionary<string, string>>(tipsJson);
    if (dic != null)
    {
        tipsDictionary = dic;
    }
}


builder.Services.AddSingleton(updateLogs);
builder.Services.AddSingleton(tipsDictionary);
builder.Services.AddSingleton(gameData);
/*后面如果动态配置项多了考虑直接做一个大类*/

#endregion

await builder.Build().RunAsync();