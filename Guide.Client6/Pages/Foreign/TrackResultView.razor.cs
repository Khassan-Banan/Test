using Guide.Client6.Services;
using Guide.Shared.Constants;

using Microsoft.AspNetCore.Components;

namespace Guide.Client6.Pages.Foreign
{
    public partial class TrackResultView
    {
        [Parameter]
        public TrackResult TrackResult { get; set; }
        [Parameter]
        public AcademicTrack AcademicTrack { get; set; }
        [Inject]
        public FilterService FilterService { get; set; }
        [Inject]
        private TopBarService TopBarService { get; set; }
        protected   override void OnInitialized()
        {
            Console.WriteLine("Track Result View On INit" + TrackResult.Message);
        }
        private void GoToFilters(AcademicTrack track, float percentage)
        {

            FilterService.ChangeFilters(filters =>
            {
                filters.Track = track;
                filters.Percentage = percentage;
            });
            TopBarService.OpenLeft = false;
            TopBarService.UnLockRight();
            TopBarService.OpenRight = true;
        }
    }
}
