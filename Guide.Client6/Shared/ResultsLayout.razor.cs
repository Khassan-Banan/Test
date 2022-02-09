using Guide.Client6.Services;
using Guide.Shared.Models;

using Microsoft.AspNetCore.Components;

namespace Guide.Client6.Shared
{
    public partial class ResultsLayout
    {
        //[Parameter]
        //public RenderFragment Body { get; set; }

        [CascadingParameter]
        public bool Rtl { get; set; }

        [Parameter]
        public RenderFragment Top { get; set; }
        [Inject]
        private ProgramService _programService { get; set; }

        [Inject]
        private FilterService FilterService { get; set; }
        public List<AcademicProgram> Programs { get; set; }
        //public bool Loading { get; set; }
        //public FilterParams Filters { get; set; }
        //private async Task ChangePrograms()
        //{

        //    Console.WriteLine("filters changed");
        //    Loading = true;
        //    StateHasChanged();
        //    Programs = await _programService.Get(FilterService.Filters);
        //    Loading = false;
        //    StateHasChanged();
        //    Console.WriteLine("filters finished");
        //}
        //private void OnFiltersChanged()
        //{
          
        //    ChangePrograms();
        //}

        //protected override void OnInitialized()
        //{
        //    Programs = new();
        //    FilterService.OnChange += OnFiltersChanged;


        //}
    }
}
