using System.Net.Http.Json;
using Guide.Client6.Json;
using Guide.Shared.Constants;
using Guide.Shared.Models;

namespace Guide.Client6.Services
{
    public class EquivalencyService
    {
        private readonly HttpClient _httpClient;

        private EquivalencyData _equivalencyData;
        private IDictionary<int, ExternalCertificate> _certificateById;
        private IDictionary<int, ExternalCourse> _courseById;

        private CourseSelectionStep _foundationalCoursesStep;
        private CourseSelectionStep _scientificCoursesStep;
        private CourseSelectionStep _nonScientificCoursesStep;
        private CourseSelectionStep _nonFoundationalCoursesStep;

        private const int RequiredCoursesCount = 7;

        private const int FoundationalMinCount = 2;
        private const int FoundationalMaxCount = 4;

        private const int ScientificMinCountForScientificPrograms = 2;


        public EquivalencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            CreateSteps();
        }

        private void CreateSteps()
        {
            _foundationalCoursesStep = new CourseSelectionStep
            {
                ExternalCourseTypes = new[] { ExternalCourseType.Foundational },
                MinCount = FoundationalMinCount,
                MaxCount = FoundationalMaxCount,
                //MinCountExceptionMessage = $"Less than {FoundationalMinCount} foundational courses"
                MinCountExceptionMessage = $"أقل من {FoundationalMinCount} مواد"
            };
            _scientificCoursesStep = new CourseSelectionStep
            {
                ExternalCourseTypes = new[] { ExternalCourseType.Scientific },
                MinCount = ScientificMinCountForScientificPrograms,
                MaxCount = int.MaxValue,
                MinCountExceptionMessage = $"أقل من {ScientificMinCountForScientificPrograms} مواد"
            };
            _nonScientificCoursesStep = new CourseSelectionStep
            {
                ExternalCourseTypes = new[] { ExternalCourseType.NonScientific },
                MinCount = 0,
                MaxCount = int.MaxValue
            };
            _nonFoundationalCoursesStep = new CourseSelectionStep
            {
                ExternalCourseTypes = new[] { ExternalCourseType.Scientific, ExternalCourseType.NonScientific },
                MinCount = 0,
                MaxCount = int.MaxValue
            };
        }

        public async Task Load()
        {
            _equivalencyData = await _httpClient.GetFromJsonAsync("data/equivalencydata.json", GuideJsonSerializerContext.Default.EquivalencyData);

            _certificateById = _equivalencyData.ExternalCertificates.ToDictionary(ec => ec.Id);
            _courseById = _equivalencyData.ExternalCourses.ToDictionary(ec => ec.Id);

            // Links the model.
            foreach (var externalCertificateCourse in _equivalencyData.ExternalCertificateCourses)
            {
                externalCertificateCourse.ExternalCertificate = _certificateById[externalCertificateCourse.ExternalCertificateId];
                externalCertificateCourse.ExternalCourse = _courseById[externalCertificateCourse.ExternalCourseId];

                externalCertificateCourse.ExternalCertificate.ExternalCertificateCourses.Add(externalCertificateCourse);
                externalCertificateCourse.ExternalCourse.ExternalCertificateCourses.Add(externalCertificateCourse);
            }
         
        }

        public EquivalencyData Get()
        {
            return _equivalencyData;
        }

        public EquivalencyResult Calculate(int externalCertificateId, IList<StudentExternalCourse> studentExternalCourses)
        {
            var result = new EquivalencyResult();

            result.CourseResults = CalculateCourseResults(externalCertificateId, studentExternalCourses);
            result.TrackResults = CalculateTrackResults(result.CourseResults);

            return result;
        }

        private IDictionary<int, CourseResult> CalculateCourseResults(int externalCertificateId, IList<StudentExternalCourse> studentExternalCourses)
        {
            var result = new Dictionary<int, CourseResult>();

            var externalCertificate = _certificateById[externalCertificateId];
            var calculator = new ScoreCalculator(externalCertificate);

            foreach (var course in studentExternalCourses)
            {
                var courseResult = calculator.Calculate(course.NumericalGrade, course.LetterGrade);

                if (courseResult.Score.HasValue)
                {
                    courseResult.IsFailed = courseResult.Score.Value < externalCertificate.ScoreThreshold;
                }

                var externalCourse = _courseById[course.ExternalCourseId];
                courseResult.CourseType = externalCourse.CourseType;

                result[course.ExternalCourseId] = courseResult;
            }

            return result;
        }

        private IDictionary<AcademicTrack, TrackResult> CalculateTrackResults(IDictionary<int, CourseResult> resultCourseResults)
        {
            var result = new Dictionary<AcademicTrack, TrackResult>();

            result[AcademicTrack.Scientific] = CalculateTrackResult(AcademicTrack.Scientific, resultCourseResults);
            result[AcademicTrack.NonScientific] = CalculateTrackResult(AcademicTrack.NonScientific, resultCourseResults);

            return result;
        }

        private TrackResult CalculateTrackResult(AcademicTrack academicTrack, IDictionary<int, CourseResult> courseResults)
        {
            var hasFailedFoundationalCourses = courseResults
                .Where(c => c.Value.CourseType == ExternalCourseType.Foundational)
                .Any(c => c.Value.IsFailed);
            if (hasFailedFoundationalCourses)
            {
                return new TrackResult("لا يوجد نجاح في المواد الأساسية");
            }

            var steps = new List<CourseSelectionStep>();
            switch (academicTrack)
            {
                case AcademicTrack.Scientific:
                    steps.Add(_foundationalCoursesStep);
                    steps.Add(_scientificCoursesStep);
                    steps.Add(_nonScientificCoursesStep);
                    break;

                case AcademicTrack.NonScientific:
                    steps.Add(_foundationalCoursesStep);
                    steps.Add(_nonFoundationalCoursesStep);
                    break;
            }

            var eligibleCourses = courseResults
                .SelectMany(c => Enumerable.Repeat(c.Value, _courseById[c.Key].Cardinality))
                .Where(c => !c.IsFailed)
                .OrderByDescending(c => c.Score)
                .ToList();

            var allSelectedCourses = new List<CourseResult>();
            foreach (var step in steps)
            {
                var selectedCourses = eligibleCourses
                    .Where(c => step.ExternalCourseTypes.Contains(c.CourseType))
                    .Take(step.MaxCount)
                    .ToList();

                if (selectedCourses.Count < step.MinCount)
                {
                    return new TrackResult(step.MinCountExceptionMessage);
                }

                allSelectedCourses.AddRange(selectedCourses);
            }

            if (allSelectedCourses.Count < RequiredCoursesCount)
            {
                return new TrackResult($"عدد المواد غير كافي");
            }

            var grade = allSelectedCourses
                .Take(RequiredCoursesCount)
                .Sum(c => c.Score.Value) / allSelectedCourses.Count;

            return new TrackResult((float)Math.Round(grade, 1));
        }

        private class CourseSelectionStep
        {
            public IList<ExternalCourseType> ExternalCourseTypes { get; set; }
            public int MinCount { get; set; }
            public int MaxCount { get; set; }

            public string MinCountExceptionMessage { get; set; }
        }

    }
}