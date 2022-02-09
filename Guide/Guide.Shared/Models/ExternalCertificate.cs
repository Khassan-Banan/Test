using System.Collections.Generic;
using Guide.Shared.Constants;

namespace Guide.Shared.Models
{
    public class ExternalCertificate
    {
        public ExternalCertificate()
        {
            ExternalCertificateCourses = new List<ExternalCertificateCourse>();
        }

        /// <summary>
        /// The Campus id.
        /// </summary>
        public int Id { get; set; }

        public string Name { get; set; }

        public EquivalenceMethod EquivalenceMethod { get; set; }
        
        public IList<GradeEquivalence> GradeEquivalences { get; set; }

        /// <summary>
        /// The score algorithm ignores courses below this threshold.
        /// </summary>
        public float ScoreThreshold { get; set; }

        #region Navigation

        public IList<ExternalCertificateCourse> ExternalCertificateCourses { get; set; }

        #endregion
    }
}