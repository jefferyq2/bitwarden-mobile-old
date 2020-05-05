using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bit.Core.Abstractions;
using Bit.Core.Exceptions;
using Blazored.Modal.Services;
using Blazored.SessionStorage;
using Blazored.Toast.Services;
using bw.web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace bw.web.Pages
{
    public partial class Index
    {
        [Inject] IJSRuntime JS { get; set;}
        [Inject] AppState App { get; set; }
        [Inject] BrowserStorageService Storage { get; set; }
        [Inject] IModalService Modal { get; set; }
        [Inject] IToastService Toast { get; set; }

        public async Task Login()
        {
            if (App.BW == null)
            {
                await InitBW();
            }

            try
            {
                await App.BW.Login(Username, Password);
            }
            catch (ApiException ex)
            {
                Console.Error.WriteLine("Login Failed:");
                if (ex.Error != null)
                {
                    Toast.ShowError(ex.Error.GetSingleMessage(), "Login Error");
                    Console.Error.WriteLine("Error Details:");
                    Console.Error.WriteLine(ex.Error.GetSingleMessage());
                    Console.Error.WriteLine(JsonSerializer.Serialize(ex.Error.ValidationErrors));
                }
                else
                {
                    Toast.ShowError(ex.Message, "Login Error");
                }
            }
            catch (Exception ex)
            {
                Toast.ShowError(ex.Message, "Unexpected Login Error");
            }
        }

        public async Task ShowPreferences()
        {
            var result = await Modal.Show<PreferencesModal>().Result;
            if (result.Cancelled)
                Console.WriteLine("Cancelled");
            else
                Console.WriteLine("Saved");
        }

        public async Task InitBW()
        {
            App.BW = new BWClient();
            App.BW.CreateServiceResolver = (t, n) =>
            {
                if (t == typeof(ICryptoPrimitiveService))
                {
                    return new JSInteropCryptoPrimitiveService(JS);
                }
                if (t == typeof(IStorageService))
                {
                    if (n == "storageService")
                    {
                        return Storage;
                    }
                    else if (n == "secureStorageService")
                    {
                        return Storage;
                    }
                    else
                    {
                        throw new Exception("Unexpected storage service name: " + n);
                    }
                }
                return null;
            };
            await App.BW.InitAsync();
        }
    }
}
