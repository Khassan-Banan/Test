using System.Net.Http.Json;

using Guide.Client6.Json;
using Guide.Shared.Constants;
using Guide.Shared.Models;

using MudBlazor;

namespace Guide.Client6.Services
{
    public class ProgramService
    {
        private readonly HttpClient _httpClient;

        //private List<University> _universities = new();

        public ProgramService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }
        public async Task<List<University>> LoadUniversities()
        {
            var daleelData = await _httpClient.GetFromJsonAsync("data/daleel.json", GuideJsonSerializerContext.Default.DaleelData);

            var universities = daleelData.Universities.Where(u => u.Programs != null).Select(u =>
            {

                u.Programs = u.Programs.Select(p =>
                {
                    p.University = u;
                    return p;

                }).ToList();
                return u;
            }).ToList();
            return universities;
        }
        public async Task<List<University>> GetUniversities()
        {
            return (await LoadUniversities()).Select(u => new University
            {
                Name = u.Name,
                Id = u.Id,
                Type = u.Type
            }).ToList();
        }
        public async Task<(int, List<AcademicProgram>)> Get<TSortOut>(FilterParams filters, int page, int pageSize, SortDirection sortDirection, Func<AcademicProgram, TSortOut> sortFunc)
        {
            int totalItems;
            var programs = (await LoadUniversities()).ConditionalWhere(filters.UniversityType != UniversityType.All, u => u.Type == filters.UniversityType)
                        .SelectMany(u => u.Programs);

            programs = programs.ConditionalWhere(filters.SearchString != string.Empty, p => (p.Name + p.University.Name).Trim().Contains(filters.SearchString.Trim()))
                       .ConditionalWhere(filters.State != State.None, p => p.State == filters.State)
                       .ConditionalWhere(filters.Track != AcademicTrack.All, p => p.AcademicTrack == filters.Track)
                       .ConditionalWhere(filters.AcademicField != AcademicField.None, p => p.AcademicField == filters.AcademicField)
                       .Where(p => p.Gender == filters.Gender);


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
                                  return (filters.Percentage + 10) >= (p.Percentage);// according to ministry 
                              }
                              else
                              {
                                  return true;
                              }
                          })
                    .OrderByDescending(p => p.Percentage);

            }
            if (sortFunc != null)
            {
                programs = programs.OrderByDirection(sortDirection, sortFunc);
            }
            totalItems = programs.Count();
            programs = programs.Skip(page * pageSize).Take(pageSize);
            return (totalItems, programs.ToList());

        }
    }
}