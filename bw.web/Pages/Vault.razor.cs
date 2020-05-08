using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace bw.web.Pages
{
    public partial class Vault
    {
        [Parameter] public string Target { get; set; }
        [Parameter] public string Parameter { get; set; }

        [Inject] NavigationManager Nav { get; set; }
        [Inject] AppState App { get; set; }

        bool _isAuthenticated;
        bool _isLocked;

        protected override async Task OnParametersSetAsync()
        {
            await CheckUnlocked();
            await base.OnParametersSetAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            Console.WriteLine("AfterRender");
            return base.OnAfterRenderAsync(firstRender);
        }

        async Task Lock()
        {
            await App.BW.Lock();
            _isAuthenticated = await App.BW.IsAuthenticated();
            _isLocked = await App.BW.IsLocked();
            await CheckUnlocked();
        }

        async Task CheckUnlocked()
        {
            if (!(_isAuthenticated = await App.BW.IsAuthenticated()))
            {
                Nav.NavigateTo("signin");
            }

            if (_isLocked = await App.BW.IsLocked())
            {
                Nav.NavigateTo("unlock");
            }
        }
    }
}
