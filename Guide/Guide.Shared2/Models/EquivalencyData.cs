using System.Collections.Generic;

namespace Guide.Shared.Models
{
    public class EquivalencyData
    {
        public IList<ExternalCertificate> ExternalCertificates { get; set; }
        public IList<ExternalCourse> ExternalCourses { get; set; }
        public IList<ExternalCertificateCourse> ExternalCertificateCourses { get; set; }
    }
}