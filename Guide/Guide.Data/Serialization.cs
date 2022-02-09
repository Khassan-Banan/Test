using Guide.Shared.Constants;
using Guide.Shared.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using static Client.Data.Helpers;
namespace Client.Data
{
    internal static class Serialization
    {
        static JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };
        public static T ReadJson<T>(string relativePath)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"..\\..\\..\\{relativePath}");

            var json = File.ReadAllText(path);

            var values = JsonSerializer.Deserialize<T>(json, options);
            return values;
        }
        public static DaleelData Convert(List<ProgramExcelTemplate> templates)
        {
            int programIdCount = 0;
            var universities = templates.GroupBy(p => p.UniversityId).Select(g =>
            {
                var university = new University();
                university.Id = int.Parse(g.Key);
                university.Name = g.First().UniversityName;
                university.Type = g.First().UniversityType.ToEnum<UniversityType>();
                university.Programs = g.Select(p =>
                {
                    decimal priceSDG;
                    decimal priceUSD;
                    bool convertedSDG = decimal.TryParse(p.PriceSDG, out priceSDG);
                    bool convertedUSD = decimal.TryParse(p.PriceUSD, out priceUSD);

                    var program = new AcademicProgram();
                    program.Id = programIdCount;
                    program.Gender = p.Gender.ToEnum<Gender>();
                    program.AcademicTrack = p.AcademicTrack.ToEnum<AcademicTrack>();
                    program.Code = p.ProgramCode;
                    program.Name = p.ProgramName;
                    program.Percentage = double.Parse(p.Percentage);
                    program.PriceSDG = convertedSDG ? priceSDG : null;
                    program.PriceUSD = convertedUSD ? priceUSD : null;
                    program.State = p.State.ToEnum<State>();
                    program.UniversityId = university.Id;
                    programIdCount++;
                    return program;
                }

                ).ToList();
                return university;
            }).ToList();

            return new DaleelData
            {
                Universities = universities
            }
            ;
        }

        public static void Serialize(DaleelData data, string filename)
        {
            //string fileName = "universites.json";  
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"..\\..\\..\\{filename}");
            string jsonString = JsonSerializer.Serialize(data, options);
            File.WriteAllText(path, jsonString);
        }

        public static void WriteToCsv(List<University> universities, string filename)
        {
            var templates = universities.SelectMany(u => u.Programs, (u, p) =>
            {
                return new ProgramExcelTemplate
                {
                    Percentage = p.Percentage + "",
                    UniversityId = u.Id + "",
                    ProgramName = p.Name,
                    UniversityName = u.Name,
                    UniversityType = u.Type + ""

                };
            }).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (ProgramExcelTemplate template in templates)
            {


                sb.AppendLine(template.ProgramCode);
                sb.AppendLine(template.ProgramName);
                sb.AppendLine(template.Percentage);
                sb.AppendLine(template.PriceSDG);
                sb.AppendLine(template.PriceUSD);
                sb.AppendLine(template.AcademicTrack);
                sb.AppendLine(template.AcademicField);
                sb.AppendLine(template.State);
                sb.AppendLine(template.Gender);
                sb.AppendLine(template.UniversityName);
                sb.AppendLine(template.UniversityType);
                sb.AppendLine(template.UniversityId);

            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"..\\..\\..\\{filename}");
            System.IO.File.WriteAllText(path, sb.ToString());
        }
    }
}
