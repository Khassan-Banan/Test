namespace Guide.Shared.Models
{
    public class ExternalCertificateCourse
    {
        public int ExternalCertificateId { get; set; }
        public int ExternalCourseId { get; set; }

        // For future support for alternative courses.
        // public int GroupKey { get; set; }

        #region Navigation

        public ExternalCertificate ExternalCertificate { get; set; }

        public ExternalCourse ExternalCourse { get; set; }

        #endregion
    }
}