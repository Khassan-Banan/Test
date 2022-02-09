using System.Collections.Generic;
using Guide.Shared.Constants;

namespace Guide.Client.Services
{
    public class EquivalencyResult
    {
        public IDictionary<int, CourseResult> CourseResults { get; set; }

        public IDictionary<AcademicTrack, TrackResult> TrackResults { get; set; }
    }
}