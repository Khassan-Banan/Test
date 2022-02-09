using Guide.Client.Services;
using Guide.Shared.Constants;
using Guide.Shared.Models;
using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guide.Client.Shared
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
        private FiltersService FiltersService { get; set; }
        public List<AcademicProgram> Programs { get; set; }

        public FilterParams Filters { get; set; }
        private void OnFiltersChanged()
        {
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
