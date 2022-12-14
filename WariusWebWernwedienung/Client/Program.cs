using Blazored.Modal;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PeptidesTools.Shared.Services;
using WariusWebWernwedienung.Client;
using WariusWebWernwedienung.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddTransient<IErrorLogger, JsInterop>();
builder.Services.AddTransient<IRemoteControlService, RemoteControlService>();
builder.Services.AddTransient<IRestInteropFactory, RestInteropFactory>();
builder.Services.AddTransient<ILocalStorageInterop, LocalStorageInterop>();
builder.Services.AddSingleton<IModalHelper, ModalHelper>();
builder.Services.AddBlazoredModal();

await builder.Build().RunAsync();
