using Guide.Client6.Services;
using Guide.Shared.Models;

using Microsoft.AspNetCore.Components;

namespace Guide.Client6.Pages
{
    public partial class ResultsPage
    {

        [Inject]
        private ProgramService _programService { get; set; }

        [Inject]
        private FilterService FilterService { get; set; }
        public List<AcademicProgram> Programs { get; set; }
        public bool Loading { get; set; }
        public FilterParams Filters { get; set; }
        //private async Task ChangePrograms()
        //{
        //    Loading = true;
        //    Programs = await _programService.Get(FilterService.Filters);
        //    StateHasChanged();
        //    Loading = false;
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
