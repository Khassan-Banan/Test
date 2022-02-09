using Guide.Shared.Models;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Client.Data
{
    internal class ExportToExcel
    {

        public static void ListToExcel(List<University> universities, string filename)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("programs");

                //get our column headings

                var Headings = typeof(ProgramExcelTemplate).GetProperties();
                for (int i = 0; i < Headings.Count(); i++)
                {

                    ws.Cells[1, i + 1].Value = Headings[i].Name;
                }

                //populate our Data

                var programs = universities.Where(u=> u.Programs != null).SelectMany(u => u.Programs, (u, p) =>
                 {
                     return new ProgramExcelTemplate
                     {
                         AcademicField = p.AcademicField.ToString(),
                         AcademicTrack = p.AcademicTrack.ToString(),
                         Gender = p.Gender.ToString(),
                         Percentage = p.Percentage.ToString(),
                         PriceSDG = p.PriceSDG.ToString(),
                         PriceUSD = p.PriceUSD.ToString(),
                         ProgramCode = p.Code,
                         ProgramName = p.Name,
                         State = p.State.ToString(),
                         UniversityId = p.UniversityId.ToString(),
                         UniversityName = u.Name,
                         UniversityType = u.Type.ToString()

                     };
                 });

                if (programs.Count() > 0)
                {
                    ws.Cells["A2"].LoadFromCollection(programs);
                }

                //Format the header
                using (ExcelRange rng = ws.Cells["A1:BZ1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(Color.White);
                }

                //Write it in the excel file.
                var path = Path.Combine(Directory.GetCurrentDirectory(), $"../../../{filename}");

                File.WriteAllBytes(path, pck.GetAsByteArray());

            }
            }
        public static void ListToExcel(University University, string filename)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("programs");

                //get our column headings

                var Headings = typeof(ProgramExcelTemplate).GetProperties();
                for (int i = 0; i < Headings.Count(); i++)
                {

                    ws.Cells[1, i + 1].Value = Headings[i].Name;
                }

                //populate our Data

                var programs = University.Programs.Select(p =>
               {
                   return new ProgramExcelTemplate
                   {
                       AcademicField = p.AcademicField.ToString(),
                       AcademicTrack = p.AcademicTrack.ToString(),
                       Gender = p.Gender.ToString(),
                       Percentage = p.Percentage.ToString(),
                       PriceSDG = p.PriceSDG.ToString(),
                       PriceUSD = p.PriceUSD.ToString(),
                       ProgramCode = p.Code,
                       ProgramName = p.Name,
                       State = p.State.ToString(),
                       UniversityId = p.UniversityId.ToString(),
                       UniversityName = University.Name,
                       UniversityType = University.Type.ToString()

                   };
               });

                if (programs.Count() > 0)
                {
                    ws.Cells["A2"].LoadFromCollection(programs);
                }

                //Format the header
                using (ExcelRange rng = ws.Cells["A1:BZ1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(Color.White);
                }

                //Write it in the excel file.
                var path = Path.Combine(Directory.GetCurrentDirectory(), $"../../../{filename}");

                File.WriteAllBytes(path, pck.GetAsByteArray());

            }
        }
    }
}
