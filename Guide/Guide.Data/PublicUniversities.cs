using Guide.Shared.Constants;
using Guide.Shared.Models;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using static Client.Data.Helpers;
namespace Client.Data
{
    internal static class PublicUniversities
    {
        private static List<University> _publicUniversities;

        private static HttpClient _httpClient;
        static PublicUniversities()
        {
            _httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _publicUniversities = new List<University>();
        }
        public static async Task<List<University>> GetUniveristies()
        {
            // دليل المنافسة للقبول العام 
            await GetUniversityNames();
            await GetUniversitiesFromCodesPage(); // some universities are a in the codes page but not in the first page "دليل المنافسة"
            Console.WriteLine("all universities names Loaded");
            foreach (var university in _publicUniversities)
            {
                await university.LoadProgramsAndPercentages();
            }
            FixDuplicateUniversities();
            Console.WriteLine("Fix duplication");
            Console.WriteLine("all programs and percentages added");
            // اسماء الكليات للمؤسسات الحكومية و رموزها
            foreach (var university in _publicUniversities)
            {
                await university.LoadPogramCodes();
            }
            Console.WriteLine("all programs codes added");
            // add Ids to programs 
            foreach (var university in _publicUniversities)
            {
                await university.LoadPrices();
            }
            Console.WriteLine("all programs prices added");
            int idCount = 1;
            foreach (var program in _publicUniversities.SelectMany(u => u.Programs))
            {
                program.Id = idCount;
                idCount++;
            }
            Console.WriteLine("all programs Ids added");

            return _publicUniversities;
        }
        private static void FixDuplicateUniversities()
        {
            // fix Omdurman university split issue.
            var omUni = _publicUniversities.FirstOrDefault(a => a.Id == 113);
            var omUniFemale = _publicUniversities.FirstOrDefault(a => a.Id == 114);
            omUniFemale.Programs.ForEach(p =>
            {
                p.Gender = Gender.Female;
                p.Name = p.Name + "";
            });
            omUni.Programs.AddRange(omUniFemale.Programs);
            _publicUniversities.Remove(omUniFemale);
            // fix Quran university split issue.
            var quUni = _publicUniversities.FirstOrDefault(a => a.Id == 123);
            var quUniFemale = _publicUniversities.FirstOrDefault(a => a.Id == 124);
            quUniFemale.Programs.ForEach(p =>
            {
                p.Gender = Gender.Female;
                p.Name = p.Name + "";
            });
            quUni.Programs.AddRange(quUniFemale.Programs);
            _publicUniversities.Remove(quUniFemale);
            // fix Quran university 2 split issue.
            var qu2Uni = _publicUniversities.FirstOrDefault(a => a.Id == 121);
            var qu2UniFemale = _publicUniversities.FirstOrDefault(a => a.Id == 122);
            qu2UniFemale.Programs.ForEach(p =>
            {
                p.Gender = Gender.Female;
                p.Name = p.Name + "";
            });
            qu2Uni.Programs.AddRange(qu2UniFemale.Programs);
            _publicUniversities.Remove(qu2UniFemale);
        }
        private async static Task GetUniversityNames()
        {
            var response = await _httpClient.GetStringAsync("http://daleel.admission.gov.sd/org_compet/gen_copet.aspx");
            var doc = new HtmlDocument();
            doc.LoadHtml(response);
            var selectElement = doc.DocumentNode.Descendants("select").First(el => el.Id == "ctl00_ContentPlaceHolder1_DropDownList1");
            if (selectElement == null)
            {
                throw new Exception("Cannot find select element ");
            }
            var names = selectElement.Descendants("option").Where(el => el.Attributes["value"].Value != "0");
            if (!names.Any())
            {

                throw new Exception("Cannot find option elements");
            }
            _publicUniversities = names.Select(node => new University
            {
                Name = node.InnerText,
                Id = int.Parse(node.Attributes["value"].Value),
                Type = UniversityType.Public
            }).ToList();

        }


        private async static Task LoadProgramsAndPercentages(this University university)
        {
            var values = new Dictionary<string, string>
                                {
{ "ctl00$ContentPlaceHolder1$DropDownList1" ,$"{university.Id}" },
{"__EVENTTARGET" , @"ctl00$ContentPlaceHolder1$DropDownList1"},
{"__VIEWSTATE", @"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" },
{ "__EVENTVALIDATION", @"/wEWKAKag7TzAQK12e7wBAKltsSeCALxzsm/AQLPsIyJDQKgiu5/AoXl8NQKAtPXtL4GAtmU1lUC8c7NvwEC6tmvlAsCz7CwiQ0CoIqSfgKF5fTUCgLT17i+BgLZlNpVAvHO8b8BAs+wtIkNAoXl+NQKAtmU3lUC8c71vwECz7C4iQ0CheX81AoC09egvgYC2ZTCVQLxzvm/AQLPsLyJDQKF5eDUCgLT16S+BgLZlMZVAvHO/b8BAs+woIkNAoXl5NQKAtPXqL4GAtmUylUC8c7hvwECz7CkiQ0CheXo1AoC09esvgYC2ZTOVQ4rADbnhTQQzAGKz5y8WaF8tMxM"}
                                };

            var content = new FormUrlEncodedContent(values);
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync("http://daleel.admission.gov.sd/org_compet/gen_copet.aspx", content);
            var html = await response.Content.ReadAsStringAsync();


            var doc = new HtmlDocument();
            doc.LoadHtml(html);


            var programsTable = doc.DocumentNode.Descendants("table").FirstOrDefault(t => t.Id == "ctl00_ContentPlaceHolder1_GridView1");
            if (programsTable == null)
            {
                //throw new Exception("Cannot find program table");
                return;
            }
            var rows = programsTable.Descendants("tr").Skip(1);
            if (!rows.Any())
            {
                throw new Exception("Cannot find program for this university  with id : " + university.Id);
            }
            university.Programs = rows.Select(node => new AcademicProgram
            {
                Name = node.Descendants("td").First().InnerText,
                Percentage = double.Parse(node.Descendants("td").Last().InnerText),
                UniversityId = university.Id,
                Gender = GetGender(node.Descendants("td").First().InnerText)
            }
            ).ToList();

        }
        private static async Task GetUniversitiesFromCodesPage()
        {
            var publicUniversitiesIds = _publicUniversities.Select(u => u.Id).ToList();
            var response = await _httpClient.GetStringAsync("http://daleel.admission.gov.sd/org_code/gov_org_code.aspx");
            var doc = new HtmlDocument();
            doc.LoadHtml(response);
            var selectElement = doc.DocumentNode.Descendants("select").First(el => el.Id == "ctl00_ContentPlaceHolder1_DropDownList1");
            if (selectElement == null)
            {
                throw new Exception("Cannot find select element ");
            }
            var names = selectElement.Descendants("option").Where(el => el.Attributes["value"].Value != "0");
            if (!names.Any())
            {

                throw new Exception("Cannot find option elements");
            }
            _publicUniversities.AddRange(names.Select(node => new University
            {
                Name = node.InnerText,
                Id = int.Parse(node.Attributes["value"].Value),
                Type = UniversityType.Public
            }).Where(u => !publicUniversitiesIds.Contains(u.Id)).ToList());

        }
        private async static Task LoadPogramCodes(this University university)
        {


            if (university.Programs == null)
            {
                university.Programs = new List<AcademicProgram>();
            }
            var values = new Dictionary<string, string>
                                {
{ "ctl00$ContentPlaceHolder1$DropDownList1" ,$"{university.Id}" },
{ "__VIEWSTATEENCRYPTED" ,$"" },
{"__EVENTTARGET" , @"ctl00$ContentPlaceHolder1$DropDownList1"},
{"__VIEWSTATE", @"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" },
{ "__EVENTVALIDATION", @"cK+LdEGl4p1RZeY48PDE2GN+wlMgo5wmbZgZ6Ys9rpnZKlqYPbof2sEnri0Ie9Cs5GnwUxWX4b1yN3kbgMAP5teQwLaRfi6tG80qttEMkKz9V2b0leKn8axqxfQ6Cdml4dKpgL09DILOTKtt6pFC9OdmWei3vdA3dHdfjqeCdFfGHmOors98pihaApcq1Xs0X2++DuLsjrPH9LzHUDpDoqYmb+BC9kITM8FpgLiKNtGxta8tT+/cp0qZTBLb0iXjpTvUUGr6wHfwag8xDeUt7eJ2rDpbg2bd6q6PhdwkioDB+stbCKaqQPj4sTcQza5crXUjDKInYgrA4Gu8JT1wP86M5j3FO5E/+krRDZvY8pqiDfkO"}
                                };

            var content = new FormUrlEncodedContent(values);

            var response = await _httpClient.PostAsync("http://daleel.admission.gov.sd/org_code/gov_org_code.aspx", content);
            var str = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();

            doc.LoadHtml(str);

            var programsTable = doc.DocumentNode.Descendants("table").FirstOrDefault(t => t.Id == "ctl00_ContentPlaceHolder1_GridView1");
            if (programsTable == null)
            {
                Console.WriteLine("Cannot find program table university id = " + university.Id);
                return;
            }
            var rows = programsTable.Descendants("tr").Skip(1);
            if (!rows.Any())
            {
                throw new Exception("Cannot find program for this university  with id : " + university.Id);
            }
            var idx = 0;
            foreach (var progamRow in rows)
            {
                var cols = progamRow.Descendants("td").ToArray();
                var name = cols[0].InnerText;
                var code = cols[1].InnerText;
                var program = university.Programs.FirstOrDefault(p => p.Name.Trim().Contains(name.Trim()));

                if (code == "")
                {
                    throw new Exception(" no code !");
                }
                if (program != null)
                {
                    program.Code = code;
                }
                else
                {
                    program = new AcademicProgram
                    {
                        Name = name,
                        Code = code
                    };
                    university.Programs.Add(program);
                }
                idx++;
            }
        }

        private static async Task LoadPrices(this University university)
        {
            var values = new Dictionary<string, string>
                                {
{ "ctl00$ContentPlaceHolder1$DropDownList1" ,$"{university.Id}" },
{ "__VIEWSTATEENCRYPTED" ,$"" },
{"__EVENTTARGET" , @"ctl00$ContentPlaceHolder1$DropDownList1"},
{"__VIEWSTATE", @"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" },
{ "__EVENTVALIDATION", @"e9NfnTZaVgwMuUZv3SNauB0LEbUuZ4K4FJF5JxSjhoUpD6OjKKTPEI9aBkJ2KsWfXrmP7D091lLBF1iUKVVBirV3g2iNix+h1PpkLcUe7/2irwmLXa4sOSw0hNwo9T7f3gdIqlEk/1iNjVGqASp6RaRwUzC22EOvTEF7nE5Lp6TIeecfswhIxYK9sRBe5VP2D0jP+FtKzthUzEdUloe0lh6B2SM7Ej/hVpW3JBtLTrUVTLiZznnsOg7K0vPYKnC3IAtCQVkwSNQD+VGGTxzLeBci75Cc168LYeJ+wruEWeF2w3J0h46YlWc03y+qtUUT+d8jmOEL+dYu0SDVT/jsTmMemxN4YS1JpYHu42794lgFhgx+"}
                                };

            var content = new FormUrlEncodedContent(values);
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync("http://daleel.admission.gov.sd/private_gov_un2020/Default_private.aspx", content);
            var str = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(str);
            var programsTable = doc.DocumentNode.Descendants("table").FirstOrDefault(t => t.Id == "ctl00_ContentPlaceHolder1_GridView1");
            if (programsTable == null)
            {
                Console.WriteLine("Cannot find program table university id = " + university.Id);
                return;
            }
            var rows = programsTable.Descendants("tr").Skip(1);
            if (!university.Programs.Any())
            {

                Console.WriteLine("university has no programs id= "+ university.Id);
                return;
            }
            if (!rows.Any())
            {
                throw new Exception("Cannot find program for this university  with id : " + university.Id);
            }
            foreach (var progamRow in rows)
            {
                var cols = progamRow.Descendants("td").ToArray();
                var name = cols[0].InnerText;
                var code = cols[3].InnerText;
                var priceSDGTxt = cols[1].InnerText;
                var priceUSDTxt = cols[2].InnerText;
                decimal priceSDG = 0;
                decimal priceUSD = 0;

                var convertedUSD = decimal.TryParse(priceUSDTxt, out priceUSD);
                var convertedSDG = decimal.TryParse(priceSDGTxt, out priceSDG);

                var program = university.Programs.FirstOrDefault(p => p.Name.Contains(name.Trim()) );
                if (code == "")
                {
                    throw new Exception(" no code !");
                }
                if (program != null)
                {
                    program.Code = program.Code == string.Empty ? code : program.Code;
                    program.PriceUSD = convertedUSD ? priceUSD : null;
                    program.PriceSDG = convertedSDG ? priceSDG : null;
                }
                //else
                //{
                //    program = new AcademicProgram
                //    {
                //        Code =  code , 
                //        PriceUSD = convertedUSD ? priceUSD : null,
                //        PriceSDG = convertedSDG ? priceSDG : null,

                //    };
                //    university.Programs.Add(program);
                //}

            }
        }
    }
}
