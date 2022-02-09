namespace Guide.Client6.Services
{
    public class StudentExternalCourse
    {
        public int ExternalCourseId { get; set; }

        /// <summary>
        /// Either NumericalGrade or LetterGrade is used, depending on the
        /// EquivalenceMethod of the related external certificate.
        /// </summary>
        public float? NumericalGrade { get; set; }
        public string LetterGrade { get; set; }
    }
}