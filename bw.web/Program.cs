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
using Toolbelt.Blazor.Extensions.DependencyInjection;
using MatBlazor;

namespace bw.web
{
    public class Program
    {
        private static BWClient _bw;

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddBlazoredToast();
            builder.Services.AddBlazoredModal();
            builder.Services.AddScoped<MemoryStorageService>();
            builder.Services.AddScoped<BrowserStorageService>();
            builder.Services.AddScoped<BrowserSecureStorageService>();
            builder.Services.AddScoped((s) => new AppState(_bw));

            builder.Services.AddLoadingBar();
            builder.Services.AddMatToaster(config =>
            {
                config.Position = MatToastPosition.TopRight;
                config.PreventDuplicates = true;
                config.NewestOnTop = true;
                config.ShowCloseButton = true;
                config.MaximumOpacity = 95;
                config.VisibleStateDuration = 3000;
            });

            var wasmHost = builder.Build();
            wasmHost.UseLoadingBar();
            await InitBW(wasmHost.Services);
            await wasmHost.RunAsync();
        }

        static async Task InitBW(IServiceProvider services)
        {
            var js = services.GetRequiredService<IJSRuntime>();
            var ms = services.GetRequiredService<MemoryStorageService>();
            var ss = services.GetRequiredService<BrowserStorageService>();
            var sc = services.GetRequiredService<BrowserSecureStorageService>();

            _bw = new BWClient();
            _bw.CreateServiceResolver = (t, n) =>
            {
                if (t == typeof(ICryptoPrimitiveService))
                {
                    return new JSInteropCryptoPrimitiveService(js);
                }
                if (t == typeof(IStorageService))
                {
                    if (n == "storageService")
                    {
                        return ss;
                    }
                    else if (n == "secureStorageService")
                    {
                        //return ms;
                        return sc;
                    }
                    else
                    {
                        throw new Exception("Unexpected storage service name: " + n);
                    }
                }
                return null;
            };
            await _bw.InitAsync();
        }
    }
}
