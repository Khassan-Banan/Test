using Guide.Shared.Constants;

namespace Guide.Client6.Services
{
    public class FilterParams
    {
        public string SearchString { get; set; } = string.Empty;
        public double? Percentage { get; set; }
        public AcademicTrack Track { get; set; }
        public AdmissionType AdmissionType { get; set; }
        public AcademicField AcademicField { get; set; }
        public Gender Gender{ get; set; }
        public UniversityType UniversityType { get; set; }
        public State State { get; set; }
        public decimal? LowestPrice { get; set; }
        public decimal? HighestPrice { get; set; }
        public Currency Currency { get; set; }
    }
}
