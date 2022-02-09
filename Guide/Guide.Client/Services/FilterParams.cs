using Guide.Shared.Constants;

namespace Guide.Client.Services
{
    public class FilterParams
    {
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
