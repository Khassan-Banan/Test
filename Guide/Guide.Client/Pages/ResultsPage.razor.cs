using Guide.Client.Services;
using Guide.Shared.Constants;
using Guide.Shared.Models;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using System.Collections.Generic;

namespace Guide.Client.Pages
{
    public partial class ResultsPage
    {

        [Inject]
        private ProgramService _programService { get; set; }

        [Inject]
        private FiltersService FiltersService { get; set; }
        public List<AcademicProgram> Programs { get; set; }

        public FilterParams Filters { get; set; }
        private void OnFiltersChanged()
        {
          
            System.Console.WriteLine("Filters Changed");
            Programs = _programService.Get(FiltersService.Filters);
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            Programs = new();
            FiltersService.OnChange += OnFiltersChanged; 


        }
    }
}
