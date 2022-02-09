using Guide.Shared.Constants;
using Guide.Shared.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
namespace Guide.Client.Services
{
    public class ProgramService
    {
        private readonly HttpClient _httpClient;

        private List<University> _universities = new();

        public ProgramService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }
        public async Task LoadUniversities()
        {
            _universities = await _httpClient.GetFromJsonAsync<List<University>>("/data/daleel.json");

            _universities = _universities.Where(u=> u.Programs != null).Select(u =>
           {
               
               u.Programs = u.Programs.Select(p =>
               {
                   p.University = u;
                   return p;

               }).ToList();
               return u;
           }).ToList();
        }
        public List<University> GetUniversities()
        {
            return _universities.Select(u => new University
            {
                Name = u.Name,
                Id = u.Id,
                Type = u.Type
            }).ToList();
        }
        public List<AcademicProgram> Get(FilterParams filters)
        {

            var programs = _universities.ConditionalWhere(filters.UniversityType != UniversityType.All, u => u.Type == filters.UniversityType)
                        .SelectMany(f => f.Programs)
                        .ConditionalWhere(filters.State != State.None, p => p.State == filters.State)
                        .ConditionalWhere(filters.Track != AcademicTrack.All, p => p.AcademicTrack == filters.Track)
                        .ConditionalWhere(filters.AcademicField != AcademicField.None, p => p.AcademicField == filters.AcademicField)
                        .Where(p => p.Gender == filters.Gender)
                        ;


            //var publicPrograms = programs;
            if (filters.AdmissionType == AdmissionType.Public)
            {
                programs = programs.Where(p => p.University.Type == UniversityType.Public)
                         .ConditionalWhere(filters.Percentage.HasValue, p => filters.Percentage.Value >= p.Percentage)
                         .OrderByDescending(p => p.Percentage);

            }
            else
            {


                Func<AcademicProgram, decimal?> priceSelector = filters.Currency == Currency.USD ? p => p.PriceUSD : p => p.PriceSDG;
                programs = programs
                    .ConditionalWhere(filters.LowestPrice.HasValue, p => priceSelector(p) >= filters.LowestPrice.Value)
                    .ConditionalWhere(filters.HighestPrice.HasValue, p => priceSelector(p) >= filters.HighestPrice.Value)
                    .ConditionalWhere(filters.Percentage.HasValue, p =>
                          {
                              if (p.University.Type == UniversityType.Public)
                              {
                                  return (filters.Percentage + 10) >= (p.Percentage);// accorading to ministery 
                              }
                              else
                              {
                                  return true;
                              }
                          })
                    .OrderByDescending(p => p.Percentage);

            }
            return programs.ToList();

        }
    }
}
