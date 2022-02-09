using Guide.Client.Services;
using Guide.Shared.Constants;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static Guide.Shared.Labels.Constants;

namespace Guide.Client.Pages
{
    public partial class FiltersSection
    {

        public string Margin { get; } = "mx-4 my-2";

        

        public FilterParams Filters
        {
            get
            {
                return FiltersService.Filters;
            }
           
        }


        [Inject]
        private FiltersService FiltersService { get; set; }
       
        private void show()
        {
            System.Console.WriteLine(Filters.AdmissionType);
            //System.Console.WriteLine(Filters.Currency);
            //System.Console.WriteLine(Filters.Track);
            //System.Console.WriteLine(Filters.State);
            //System.Console.WriteLine(Filters.Percentage);
            //System.Console.WriteLine(Filters.LowestPrice);
            //System.Console.WriteLine(Filters.HighestPrice);
            //System.Console.WriteLine(Filters.Gender);
            //System.Console.WriteLine(Filters.UniversityType);
        }

        private Task<IEnumerable<AcademicField>> SearchAcademicFields(string value)
        {
            List<AcademicField> result = new();
            foreach (var field in Enum.GetValues(typeof(AcademicField)))
            {
                var enumField = (AcademicField)field;

                if (AcademicFields[enumField].Contains(value) || value == AcademicFields[AcademicField.None])
                {
                    result.Add(enumField);
                }

            }

            return Task.FromResult((IEnumerable<AcademicField>)result);

        }
        private Task<IEnumerable<State>> SearchStates(string value)
        {
            List<State> result = new();
            foreach (var field in Enum.GetValues(typeof(AcademicField)))
            {
                var enumField = (State)field;

                if (States[enumField].Contains(value) || value == States[State.None])
                {
                    result.Add(enumField);
                }

            }

            return Task.FromResult((IEnumerable<State>)result);

        }

        #region LifeCycle
        protected override void OnInitialized()
        {
            FiltersService.OnChange += StateHasChanged;
            base.OnInitialized();
        }
        #endregion
    }
}
