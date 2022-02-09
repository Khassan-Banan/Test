using OfficeOpenXml;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Data
{
    internal static class ReadFromExcel
    {
        public static List<ProgramExcelTemplate> Read(string filename)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"../../../{filename}");
            FileInfo existingFile = new FileInfo(path);
            if (!existingFile.Exists)
            {
                throw new FileNotFoundException("File does not exists");

            }
            var programs = new List<ProgramExcelTemplate>();
            using (ExcelPackage pck = new ExcelPackage(existingFile))
            {
                
                ExcelWorksheet sheet = pck.Workbook.Worksheets[1];

                ExcelWorksheet worksheet = pck.Workbook.Worksheets[1];
                int colCount = worksheet.Dimension.End.Column;  //get Column Count
                int rowCount = worksheet.Dimension.End.Row;     //get row count
                Console.WriteLine(colCount);
                for (int row = 1; row <= rowCount; row++)
                {
                    var p = new ProgramExcelTemplate();

                    //Console.WriteLine(" Row:" + row + " column:" + col + " Value:" + worksheet.Cells[row, col].Value?.ToString().Trim());
                    p.Percentage = worksheet.Cells[row, 1].Value?.ToString().Trim();
                    p.ProgramName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                    p.ProgramCode = worksheet.Cells[row, 3].Value?.ToString().Trim();
                    p.UniversityName = worksheet.Cells[row, 4].Value?.ToString().Trim();
                    p.UniversityType = worksheet.Cells[row, 5].Value?.ToString().Trim();
                    p.UniversityId = worksheet.Cells[row, 6].Value?.ToString().Trim();
                    p.AcademicTrack = worksheet.Cells[row, 7].Value?.ToString().Trim();
                    p.AcademicField = worksheet.Cells[row, 8].Value?.ToString().Trim();
                    p.State = worksheet.Cells[row, 9].Value?.ToString().Trim();
                    p.Gender = worksheet.Cells[row, 10].Value?.ToString().Trim();
                    programs.Add(p);
                }
            }
            return programs;
        }
    }
}
