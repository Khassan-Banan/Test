using Guide.Shared.Constants;
using Guide.Shared.Models;

namespace Guide.Client6.Services
{
    public class ScoreCalculator
    {
        private readonly ExternalCertificate _externalCertificate;
        private readonly ICollection<GradeEquivalence> _orderedGradeEquivalences;

        public ScoreCalculator(ExternalCertificate externalCertificate)
        {
            _externalCertificate = externalCertificate;

            if (externalCertificate.EquivalenceMethod == EquivalenceMethod.Interpolation)
            {
                _orderedGradeEquivalences = externalCertificate.GradeEquivalences
                    .Where(ge => ge.NumericalGrade.HasValue)
                    .OrderBy(ge => ge.NumericalGrade)
                    .ToList();
            }
        }

        public CourseResult Calculate(float? numericalGrade, string letterGrade)
        {
            switch (_externalCertificate.EquivalenceMethod)
            {
                case EquivalenceMethod.Interpolation:
                    return CalculateByInterpolation(numericalGrade);

                case EquivalenceMethod.LetterGrade:
                    return CalculateByLetterGrade(letterGrade);

                default:
                    throw new NotSupportedException($"Unsupported equivalence method {_externalCertificate.EquivalenceMethod}.");
            }
        }

        private CourseResult CalculateByLetterGrade(string letterGrade)
        {
            if (string.IsNullOrWhiteSpace(letterGrade))
            {
                return new CourseResult($"Missing letter grade.");
            }

            var hasMultipleGrades = _externalCertificate.GradeEquivalences.Select(ge => ge.LetterGrade).Distinct().Count() < _externalCertificate.GradeEquivalences.Count;

            if (hasMultipleGrades)
            {
                
            }

            var gradeEquivalence = _externalCertificate.GradeEquivalences.SingleOrDefault(ge => ge.LetterGrade == letterGrade);
            if (gradeEquivalence == null)
            {
                return new CourseResult($"The letter grade {letterGrade} is not valid.");
            }

            return new CourseResult(gradeEquivalence.EquivalentScore);
        }

        private CourseResult CalculateByInterpolation(float? numericalGrade)
        {
            if (!numericalGrade.HasValue)
            {
                return new CourseResult($"Missing numerical grade.");
            }

            if (_orderedGradeEquivalences.Count < 2)
            {
                return new CourseResult($"Less than 2 interpolation points.");
            }

            // Zip ya!
            var applicableRange = _orderedGradeEquivalences
                .Zip(_orderedGradeEquivalences.Skip(1))
                .FirstOrDefault(p => p.First.NumericalGrade.Value <= numericalGrade.Value && numericalGrade.Value <= p.Second.NumericalGrade.Value);

            if (applicableRange == default)
            {
                return new CourseResult($"The numerical grade {numericalGrade.Value} lies outside the interpolation range.");
            }

            var score = Interpolate(numericalGrade.Value, applicableRange.First, applicableRange.Second);

            if (score<0 || score>100)
            {
                return new CourseResult($"The interpolated score {score} is outside the 0 to 100 range.");
            }

            return new CourseResult(score);
        }

        private static float Interpolate(float numericalGrade, GradeEquivalence from, GradeEquivalence to)
        {
            var x0 = from.NumericalGrade.Value;
            var x1 = to.NumericalGrade.Value;

            var y0 = from.EquivalentScore;
            var y1 = to.EquivalentScore;

            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (numericalGrade - x0) * (y1 - y0) / (x1 - x0);
        }
    }
}