using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bit.Core.Exceptions;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace bw.web.Pages
{
    public partial class SignIn
    {
        public const string EmailId = "email";

        private const string Keys_RememberedEmail = "rememberedEmail";
        private const string Keys_RememberEmail = "rememberEmail";

        [Parameter] public string ReturnUrl { get; set; }

        [Inject] IJSRuntime JS { get; set; }
        [Inject] NavigationManager Nav { get; set; }
        [Inject] IMatToaster Toaster { get; set; }
        [Inject] AppState App { get; set; }

        // View Models - inline instead of dedicated model classes

        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        [EmailAddress]
        public string HintEmail { get; set; }

        bool _forgotPasswordVisible = false;
        string _passwordInputType = "password";
        string _submitIcon = "verified_user"; // Other possibilities:  verified_user hourglass_empty timer sync
        string _submitIconClass = "";
        string _submitText = "Sign In";
        bool _submitDisabled = false;


        protected override async Task OnParametersSetAsync()
        {
            if (await App.BW.IsAuthenticated())
            {
                if (await App.BW.IsLocked())
                    Nav.NavigateTo("unlock");
                else
                    Nav.NavigateTo("vault");
            }

            await base.OnParametersSetAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("bw_web.focusById", EmailId);

            }

            await base.OnAfterRenderAsync(firstRender);
        }

        void NavToRegister()
        {
            Nav.NavigateTo("register");
        }

        void ShowPreferences()
        {
            //navigationManager.NavigateTo(navigationManager.BaseUri + "api/externalauth/challenge/google", true);
        }

        async Task SubmitSignIn()
        {
            _submitIcon = "hourglass_empty";
            _submitIconClass = "icon-spinning";
            _submitText = "Signing In...";
            _submitDisabled = true;
            StateHasChanged();

            try
            {
                await App.BW.SignIn(Email, Password);
                Toaster.Add("Success!", MatToastType.Success, "Sign In");
                Nav.NavigateTo("vault");
            }
            catch (ApiException ex)
            {
                ShowApiError(ex);
            }
            catch (Exception ex)
            {
                ShowUnexpectedError(ex);
            }
            finally
            {
                _submitIcon = "verified_user";
                _submitIconClass = "";
                _submitText = "Sign In";
                _submitDisabled = false;
                StateHasChanged();
            }

            //try
            //{
            //    var response = await ((IdentityAuthenticationStateProvider)authStateProvider).Login(loginParameters);
            //    if (response.StatusCode == Status200OK)
            //    {
            //        // On successful Login the response.Message is the Last Page Visited from User Profile
            //        // We can't navigate yet as the setup is proceeding asynchronously
            //        if (!string.IsNullOrEmpty(response.Message))
            //        {
            //            navigateTo = response.Message;
            //        }
            //        else
            //        {
            //            navigateTo = "/dashboard";
            //        }
            //    }
            //    else
            //    {
            //        matToaster.Add(response.Message, MatToastType.Danger, "Login Attempt Failed");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    matToaster.Add(ex.Message, MatToastType.Danger, "Login Attempt Failed");
            //}
        }

        async Task SubmitPasswordHint()
        {
            try
            {
                await App.BW.SendHint(HintEmail);
                Toaster.Add("You should receive an email with your Master Password hint.",
                    MatToastType.Success, "Sign In");
            }
            catch (ApiException ex)
            {
                ShowApiError(ex);
            }
            catch (Exception ex)
            {
                ShowUnexpectedError(ex);
            }

            //try
            //{
            //    await ((IdentityAuthenticationStateProvider)authStateProvider).ForgotPassword(forgotPasswordParameters);
            //    matToaster.Add("Forgot Password Email Sent", MatToastType.Success);
            //    forgotPasswordParameters.Email = "";
            //    forgotPasswordToggle = false;
            //}
            //catch (Exception ex)
            //{
            //    matToaster.Add(ex.Message, MatToastType.Danger, "Reset Password Attempt Failed");
            //}
        }

        void ShowApiError(ApiException ex)
        {
            Console.WriteLine("Login Failed:");
            if (ex.Error != null)
            {
                //Toast.ShowError(ex.Error.GetSingleMessage(), "Login Error");
                Toaster.Add(ex.Error.GetSingleMessage(), MatToastType.Danger, "Sign In - Error");
                Console.WriteLine("Error Details:");
                Console.WriteLine(ex.Error.GetSingleMessage());
                Console.WriteLine(JsonSerializer.Serialize(ex.Error.ValidationErrors));
            }
            else
            {
                //Toast.ShowError(ex.Message, "Login Error");
                Toaster.Add(ex.Message, MatToastType.Danger, "Sign In - Error");
                Console.WriteLine("Login Error");
                Console.WriteLine(ex);
            }
        }
        void ShowUnexpectedError(Exception ex)
        {
            //Toast.ShowError(ex.Message, "Unexpected Login Error");
            Toaster.Add(ex.Message, MatToastType.Danger, "Sign In - Unexpected Error");
            Console.WriteLine("Unexpected Login Error");
            Console.WriteLine(ex);
        }
    }
}
