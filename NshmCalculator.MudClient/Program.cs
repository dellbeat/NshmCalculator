using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using NshmCalculator.MudClient;
using NshmCalculator.MudClient.Utilities;
using NshmCalculator.Shared.Models;
using System.Net.Http;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var client = new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
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
var newJson = await client.GetStringAsync(TipsText.UpdateLogPath);//需要处理缓存未更新的情况
if (!string.IsNullOrEmpty(newJson))
{
    var logs = JsonSerializer.Deserialize<UpdateLog[]>(newJson);
    if (logs is { Length: > 0 })
    {
        updateLogs = logs;
    }
}

builder.Services.AddSingleton(updateLogs);

#endregion

await builder.Build().RunAsync();
