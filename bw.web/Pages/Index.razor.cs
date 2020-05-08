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

        protected void ShowApiError(ApiException ex)
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
                Console.WriteLine("Login Error");
                Console.WriteLine(ex);
            }
        }

        protected void ShowUnexpectedError(Exception ex)
        {
            Toast.ShowError(ex.Message, "Unexpected Login Error");
            Console.WriteLine("Unexpected Login Error");
            Console.WriteLine(ex);
        }

        public async Task SendHint()
        {
            try
            {
                var bw = App.BW;
                await bw.SendHint(Username);
                Toast.ShowSuccess("You should receive an email with your Master Password hint.");
            }
            catch (ApiException ex)
            {
                ShowApiError(ex);
            }
            catch (Exception ex)
            {
                ShowUnexpectedError(ex);
            }
        }

        public async Task Login()
        {
            try
            {
                var bw = App.BW;
                await bw.SignIn(Username, Password);
            }
            catch (ApiException ex)
            {
                ShowApiError(ex);
            }
            catch (Exception ex)
            {
                ShowUnexpectedError(ex);
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

    }
}
