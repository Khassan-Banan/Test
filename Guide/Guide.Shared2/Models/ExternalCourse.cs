using System.Collections.Generic;
using Guide.Shared.Constants;

namespace Guide.Shared.Models
{
    public class ExternalCourse
    {
        public ExternalCourse()
        {
            ExternalCertificateCourses = new List<ExternalCertificateCourse>();
        }

        /// <summary>
        /// The Campus id.
        /// </summary>
        public int Id { get; set; }

        public string Name { get; set; }

        public ExternalCourseType CourseType { get; set; }

        public int Cardinality { get; set; }

        #region Navigation

        public IList<ExternalCertificateCourse> ExternalCertificateCourses { get; set; }

        #endregion
    }
}