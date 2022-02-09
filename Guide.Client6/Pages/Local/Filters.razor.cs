using Guide.Client6.Services;
using Guide.Shared.Constants;
using Microsoft.AspNetCore.Components;

namespace Guide.Client6.Pages.Local
{
    public partial class Filters
    {
        public string Margin { get; } = "mx-4 my-2";
        [Inject]
        public FilterService FilterService { get; set; }

        public double? Percentage { get; set; }
        public bool PercentageError { get; set; }
        public AcademicTrack Track { get; set; }
        //public Gender Gender { get; set; }

        [Inject]
        private TopBarService TopBarService { get; set; }

        private void ShowResult()
        {
            PercentageError = !Percentage.HasValue;
            if (PercentageError)
            {
                TopBarService.LockRight();
            }
            else
            {
                TopBarService.UnLockRight();
                FilterService.ChangeFilters(f =>
               {
                   f.Track = Track;
                   f.Percentage = Percentage;
                   //f.Gender = Gender;
               });
                TopBarService.OpenLeft = false;
            }
        }
        protected override void OnInitialized()
        {
            TopBarService.OpenLeft = true;
            TopBarService.OpenRight = false;
            TopBarService.LockRight();
            Track = AcademicTrack.Scientific;
            //Gender = Gender.Male;
            base.OnInitialized();
        }


    }
}
