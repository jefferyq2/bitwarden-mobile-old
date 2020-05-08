using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace bw.web.Pages
{
    public partial class Unlock
    {
        [Inject] IJSRuntime JS { get; set; }
        [Inject] NavigationManager Nav { get; set; }
        [Inject] IMatToaster Toaster { get; set; }
        [Inject] AppState App { get; set; }

        // View Models - inline instead of dedicated model classes

        public string Password { get; set; }

        string _currentEmail;
        string _passwordInputType = "password";
        string _submitIcon = "lock_open"; // Other possibilities:  verified_user hourglass_empty timer sync
        string _submitIconClass = "";
        string _submitText = "Unlock";
        bool _submitDisabled = false;

        protected override async Task OnParametersSetAsync()
        {
            await CheckUnlocked();
            await base.OnParametersSetAsync();
        }

        async Task SubmitUnlock()
        {
            _submitIcon = "hourglass_empty";
            _submitIconClass = "icon-spinning";
            _submitText = "Unlocking...";
            _submitDisabled = true;
            StateHasChanged();

            try
            {
                await App.BW.Unlock(Password);
                Toaster.Add("Unlocked!", MatToastType.Success, "Unlock");
                Nav.NavigateTo("vault");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                _submitIcon = "lock_open";
                _submitIconClass = "";
                _submitText = "Sign In";
                _submitDisabled = false;
                StateHasChanged();

            }
        }

        async Task SignOut()
        {
            await App.BW.SignOut();
            await CheckUnlocked();
        }

        void ShowError(Exception ex)
        {
            Toaster.Add(ex.Message, MatToastType.Danger, "Unlock - Error");
            Console.WriteLine(ex);
        }

        async Task CheckUnlocked()
        {
            if (!await App.BW.IsAuthenticated())
                Nav.NavigateTo("signin");

            if (!await App.BW.IsLocked())
                Nav.NavigateTo("vault");
        }
    }
}
