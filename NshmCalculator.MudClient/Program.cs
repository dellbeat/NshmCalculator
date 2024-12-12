using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using NshmCalculator.MudClient;
using NshmCalculator.MudClient.Utilities;

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

if (errorCount == 3)
{
    Console.WriteLine("»ñÈ¡»ù´¡ÅäÖÃÊ§°Ü£¬Çë¼ì²éÍøÂç");
    throw new Exception("»ñÈ¡»ù´¡ÅäÖÃÊ§°Ü£¬Çë¼ì²éÍøÂç");
}

#endregion

await builder.Build().RunAsync();