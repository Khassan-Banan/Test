using Guide.Shared.Constants;

namespace Guide.Client6.Services
{
    public class CourseResult
    {
        public CourseResult(float score)
        {
            Score = score;
        }

        public CourseResult(string message)
        {
            IsFailed = true;
            Message = message;
        }

        /// <summary>
        /// True if the score is below the ScoreThreshold of the external certificate.
        /// The equivalency algorithm ignores all failed courses, and a student needs to pass all foundational courses.
        /// </summary>
        public bool IsFailed { get; set; }

        /// <summary>
        /// Has a Value if !IsFailed.
        /// </summary>
        public float? Score { get; }
        
        /// <summary>
        /// Explanation in case Score is null.
        /// </summary>
        /// <remarks>
        /// This happens when the student grade (numerical or letter) lies out of bounds.
        /// </remarks>
        public string Message { get; }

        /// <summary>
        /// Copied from the external course to simplify the algorithm. 
        /// </summary>
        public ExternalCourseType CourseType { get; set; }
    }
}