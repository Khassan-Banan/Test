using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor;
using Guide.Client.Services;

namespace Guide.Client.Shared
{
    public partial class TopBar
    {
        [Parameter]
        public RenderFragment LeftDrawerBody { get; set; }

        [Parameter]
        public string LeftDrawerTitle { get; set; }
        [Parameter]
        public string LeftDrawerIcon { get; set; }
        [CascadingParameter]
        public bool Rtl { get; set; }
        #region Drawers
        [Inject]
        private TopBarService TopBarService { get; set; }

        void ToggleFiltersDrawer()
        {
            
            TopBarService.OpenRight = !TopBarService.OpenRight;
        }

        void ToggleLeftDrawer()
        {
            TopBarService.OpenLeft = !TopBarService.OpenLeft;
        }

        #endregion
        protected override void OnInitialized()
        {
            TopBarService.OnChange += StateHasChanged;
            base.OnInitialized();
        }
    }
}
