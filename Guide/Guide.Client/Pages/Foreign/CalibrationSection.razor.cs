using Guide.Client.Services;
using Guide.Shared.Constants;
using Guide.Shared.Models;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guide.Client.Pages.Foreign
{
    public partial class CalibrationSection
    {
       
        private int SudaneseCertificateId = 27;
        public string Margin { get; } = "mx-4 my-2";
        public ExternalCertificate ExternalCertificate { get; set; }
        
        [Inject]
        public EquivalencyService EquivalencyService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public FiltersService FiltersService { get; set; }


        public EquivalencyResult Result { get; set; }

        public List<ExternalCertificate> Certificates{ get; set; }

        public List<ExternalCourse> Courses { get; set; }
        public bool Loading { get; set; }

        public ExternalCourseType ExternalCourseType { get; set; }


        [Inject]
        private TopBarService TopBarService { get; set; }
        public List<StudentExternalCourse> StudentExternalCourses{ get; set; }
        protected async override Task OnInitializedAsync()
        {
            Loading = true;
            ExternalCourseType = ExternalCourseType.Scientific;
            TopBarService.OpenLeft = true;
            TopBarService.OpenRight = false;
            TopBarService.LockRight();
       
            await EquivalencyService.Load();
            Certificates = EquivalencyService.Get().ExternalCertificates.ToList();
            ExternalCertificate = Certificates.First(); // id for sudanese certificate
            Courses = ExternalCertificate.ExternalCertificateCourses.Select(c => c.ExternalCourse).ToList();
            StudentExternalCourses = Courses.Select(c => new StudentExternalCourse
            {
                ExternalCourseId = c.Id,
                LetterGrade = string.Empty,
                NumericalGrade = null
            }).ToList();
            Console.WriteLine(Certificates.Count);
            Loading = false;
            base.OnInitialized();
        }
        private void CertificateChanged(ExternalCertificate externalCertificate)
        {
            ExternalCertificate = externalCertificate;
            //StudentExternalCourses = new 
            Courses = ExternalCertificate.ExternalCertificateCourses.Select(c => c.ExternalCourse).ToList();
            StudentExternalCourses = Courses.Select(c => new StudentExternalCourse
            {
                ExternalCourseId = c.Id,
                LetterGrade = string.Empty,
                NumericalGrade = null
            }).ToList();
            StateHasChanged();
        }
        private void Calculate()
        {
            if(Result != null)
            {
                Result = null;
            }
            else
            {
               Result = EquivalencyService.Calculate(ExternalCertificate.Id, StudentExternalCourses);
                Console.WriteLine(Result.TrackResults[AcademicTrack.Scientific].Message);
            }

        }

        private void GoToFilters(AcademicTrack track, float percentage)
        {
            FiltersService.Filters.Track = track;
            FiltersService.Filters.Percentage = percentage;
            TopBarService.OpenRight = true;
            TopBarService.OpenLeft = false;
            TopBarService.UnLockRight();
        }
    }
}
