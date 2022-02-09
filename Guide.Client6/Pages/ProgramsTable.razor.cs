using Guide.Client6.Services;
using Guide.Shared.Models;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using System.Net.Http;

namespace Guide.Client6.Pages
{
    public partial class ProgramsTable
    {
        //[Parameter]
        //public List<AcademicProgram> Programs { get; set; }
        private bool Loading = false;
        public List<AcademicProgram> Programs { get; set; }
        [Inject]
        public FilterService FilterService { get; set; }
        [Inject]
        public ProgramService ProgramService { get; set; }

        public int TotalItems { get; set; }
        protected override void OnInitialized()
        {
            FilterService.OnChange += () => table.ReloadServerData();

            base.OnInitialized();
        }
        private MudTable<AcademicProgram> table;


        private async Task<TableData<AcademicProgram>> FetchPrograms(TableState state)
        {
            Loading = true;
            StateHasChanged();
            Console.WriteLine("Fetch Programs");
            Func<AcademicProgram, object> sortFunc;
            switch (state.SortLabel)
            {
                case "name":
                    sortFunc = (p) => p.Name;
                    break;
                case "percentage":
                    sortFunc = p => p.Percentage;
                    break;
                case "code":
                    sortFunc = p => p.Code;
                    break;
                case "university-name":
                    sortFunc = p => p.University.Name;
                    break;
                default:
                    sortFunc = null;
                    break;

            }
            Console.WriteLine(state.SortLabel);
            Console.WriteLine(state.SortDirection);
            (TotalItems, Programs) = await ProgramService.Get(FilterService.Filters, state.Page, state.PageSize, state.SortDirection, sortFunc);
            Loading = false;
            StateHasChanged();
            return new TableData<AcademicProgram>() { TotalItems = TotalItems, Items = Programs };
        }
    }
}
