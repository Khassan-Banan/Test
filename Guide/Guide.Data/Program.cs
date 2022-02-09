
using Guide.Shared.Constants;
using Guide.Shared.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Client.Data.Serialization;
using static Client.Data.WebScrapping;
using static Client.Data.ExportToExcel;
using static Client.Data.ReadFromExcel;
using System.Text;

namespace Client.Data
{
    class Program
    {


        static async Task Main(string[] args)
        {


            //var u = await PublicUniversities.GetUniveristies();

            //Console.WriteLine(u.Count);
            //foreach (var university in u)
            //{
            //ListToExcel(university, $"Samples/{university.Name}.xlsx");

            //}
            //ListToExcel(u, "Samples/public.xlsx");
            /****** Convert Json to Excel *********/
            //var programTemplates = ReadJson<List<ProgramExcelTemplate>>("Samples/programs.json");
            //var universities = Convert(programTemplates);
            //Serialize(universities, "Samples/daleel.json");
            //Console.WriteLine("Finished");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var programs = Read("Scraped_Data/tech-university.xlsx");
            foreach (var item in programs)
            {
                Console.WriteLine(item.ProgramName);
            }
        }
    }
}
