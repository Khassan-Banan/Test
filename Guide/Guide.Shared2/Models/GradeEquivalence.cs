namespace Guide.Shared.Models
{
    public class GradeEquivalence
    {
        /// <summary>
        /// Either NumericalGrade or LetterGrade is used, depending on the
        /// EquivalenceMethod of the related external certificate.
        /// </summary>
        public float? NumericalGrade { get; set; }
        public string LetterGrade { get; set; }

        public float EquivalentScore { get; set; }
    }
}