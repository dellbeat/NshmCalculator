using ApexCharts;
using BlazorDownloadFile;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using NshmCalculator.MudClient;
using NshmCalculator.MudClient.Utilities;
using NshmCalculator.MudClient.Utilities.Interface;
using Tewr.Blazor.FileReader;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var client = new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
};
builder.Services.AddFileReaderService(options => options.UseWasmSharedBuffer = true);
builder.Services.AddSingleton(client);
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
builder.Services.AddBlazorDownloadFile();
builder.Services.AddSingleton<IStateContainer, StateContainer>();
builder.Services.AddApexCharts(option =>
{
    option.GlobalOptions = new ApexChartBaseOptions()
    {
        
    };
});

#region InitConfig

int errorCount = 0;

while (errorCount < 3)
{
    if (await ConfigHelper.InitAppVersion(client))
    {
        break;
    }
    errorCount++;
}

ConfigHelper.InitClient(client);

if (errorCount == 3)
{
    throw new Exception("获取基础配置失败，请检查网络");
}

#endregion

await builder.Build().RunAsync();