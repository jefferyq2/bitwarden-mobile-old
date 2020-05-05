using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bit.Core.Abstractions;
using bw.web.Services;
using Microsoft.JSInterop;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazored.Toast;
using Blazored.Modal;

namespace bw.web
{
    public class Program
    {
        public static BWClient BW;

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddBlazoredToast();
            builder.Services.AddBlazoredModal();
            builder.Services.AddScoped<BrowserStorageService>();
            builder.Services.AddScoped<AppState>();

            var wasmHost = builder.Build();

            var js = wasmHost.Services.GetRequiredService<IJSRuntime>();

            //BW = new BWClient();
            //BW.CreateServiceResolver = (t, n) =>
            //{
            //    if (t == typeof(ICryptoPrimitiveService))
            //    {
            //        return new JSInteropCryptoPrimitiveService(js);
            //    }
            //    return null;
            //};
            //await BW.InitAsync();

            await wasmHost.RunAsync();
        }
    }
}
