using Guide.Shared.Constants;

namespace Guide.Client6.Services
{
    public class EquivalencyResult
    {
        public IDictionary<int, CourseResult> CourseResults { get; set; }

        public IDictionary<AcademicTrack, TrackResult> TrackResults { get; set; }
    }
}