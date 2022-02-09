using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Guide.Shared.Models;
using HtmlAgilityPack;
using Guide.Shared.Constants;

namespace Client.Data
{
    public static class WebScrapping
    {
        private static HttpClient _httpClient;
        static WebScrapping()
        {
            _httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
        }
        public static async Task<string> CallUrl(string fullUrl)
        {
            var response = _httpClient.GetStringAsync(fullUrl);
            return await response;
        }
        public static async Task<List<University>> ReadPublicUniversities()
        {
            var url = "http://daleel.admission.gov.sd/org_compet/gen_copet.aspx";
            var response = await CallUrl(url);
            var universities = GetPublicUniversities(response);


            foreach (var university in universities)
            {
                university.Programs = await GetPrograms(university);
                Console.WriteLine($"finished univ : {university.Id}");
            }

            foreach (var item in universities)
            {
          
                await AssignPricesToPrograms(item);
           
            }
            foreach (var item in universities)
            {
                Console.WriteLine("university program without names" + item.Programs.Count);
          
                await AssignCodesToPrograms(item);
            }
            var (states, fields) = await GetAcademicFieldsAndStates();
            foreach (var state in states)
            {

                foreach (var field in fields)
                {
                    await AssignFieldAndStatesToPrograms(field, state, universities);
                    var (value, name) = field;
                    Console.WriteLine($"Finished field {value}");
                }
            }
            foreach (var program in universities.SelectMany(p => p.Programs))
            {
                var gender = Gender.All;
                if (program.Name.Contains("طالبات"))
                {
                    gender = Gender.Female;
                }
                else if (program.Name.Contains("طلاب"))
                {
                    gender = Gender.Male;
                }
                else
                {
                    gender = Gender.All;
                }
                program.Gender = gender;
            }
            return universities;
        }
        public static List<University> GetPublicUniversities(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.DocumentNode.Descendants("option").Where(i => int.Parse(i.Attributes["value"].Value) > 0).Select(node => new University
            {
                Name = node.InnerText,
                Id = int.Parse(node.Attributes["value"].Value),
                Type = UniversityType.Public
            }).ToList();
        }

        public static async Task<string> GetProgramsRequest(University university)
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
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<List<AcademicProgram>> GetPrograms(University university)
        {
            var programStr = await GetProgramsRequest(university);
            var doc = new HtmlDocument();
            doc.LoadHtml(programStr);
            int idCount = 0;
            var programsTable = doc.DocumentNode.Descendants("table").First(t => t.Id == "ctl00_ContentPlaceHolder1_GridView1");

            return programsTable.Descendants("tr").Skip(1).Select(node =>
            {
                //Console.WriteLine(node.Descendants("td").First().InnerText);
                var p = new AcademicProgram
                {
                    Name = node.Descendants("td").First().InnerText,
                    Percentage = double.Parse(node.Descendants("td").Last().InnerText),
                    Id = idCount,
                    UniversityId = university.Id

                };
                idCount++;
                return p;

            }).ToList();

        }

        public static async Task<(List<(string, string)>, List<(string, string)>)> GetAcademicFieldsAndStates()
        {
            var fieldsStr = await CallUrl("http://daleel.admission.gov.sd/state/st_mj.aspx");
            var doc = new HtmlDocument();
            doc.LoadHtml(fieldsStr);

            var fields = doc.DocumentNode.Descendants("select").First(d => d.Id == "ctl00_ContentPlaceHolder1_DropDownList2")
                .Descendants("option").Skip(1).Select(node => (node.Attributes["value"].Value, node.InnerText)).ToList();
            var states = doc.DocumentNode.Descendants("select").First(d => d.Id == "ctl00_ContentPlaceHolder1_DropDownList1")
                .Descendants("option").Skip(1).Select(node => (node.Attributes["value"].Value, node.InnerText)).ToList();
            return (states, fields);
        }
        public static async Task AssignFieldAndStatesToPrograms((string, string) field, (string, string) state, List<University> universities)
        {
            (string fieldValue, string fieldName) = field;
            (string stateValue, string stateName) = state;
            Console.WriteLine(fieldValue);
            Console.WriteLine(stateValue);
            var values = new Dictionary<string, string>
                                {
{ "ctl00$ContentPlaceHolder1$DropDownList1" ,$"{stateValue}" },
{ "ctl00$ContentPlaceHolder1$DropDownList2" ,$"{fieldValue}" },
{ "ctl00$ContentPlaceHolder1$Button1" ,"عرض" },
{"__EVENTTARGET" , @"ctl00$ContentPlaceHolder1$DropDownList1"},
{"__VIEWSTATE", @"/wEPDwUKLTgyOTUyNDEyNg9kFgJmD2QWAgIDD2QWAgIBD2QWBAIHDzwrAA0BAA8WAh4HVmlzaWJsZWhkZAIJDw9kDxAWAmYCARYCFgIeDlBhcmFtZXRlclZhbHVlBQEwFgIfAQUBMBYCZmZkZBgBBSNjdGwwMCRDb250ZW50UGxhY2VIb2xkZXIxJEdyaWRWaWV3MQ9nZE48+IoWwI/g6LFH4zFcPmmeupIi" },
{ "__EVENTVALIDATION", @"/wEWEwKCj47WCQKltsSeCAKttsSeCAK6tsSeCAK7tsSeCAK4tsSeCAK5tsSeCAK+tsSeCAK/tsSeCAK8tsSeCAKKn/q1BgKVn/q1BgKUn/q1BgKXn/q1BgKRn/q1BgKQn/q1BgKTn/q1BgKCn/q1BgKA4sljRuAHGzTFBmYhPtsOofqoAPZu9IY="}
                                };

            var content = new FormUrlEncodedContent(values);
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync("http://daleel.admission.gov.sd/state/st_mj.aspx", content);
            var responseStr = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(responseStr);
            var programTable = doc.DocumentNode.Descendants("table").FirstOrDefault(t => t.Id == "ctl00_ContentPlaceHolder1_GridView1");
            if (programTable != null)
            {

                var rows = programTable.Descendants("tr").Skip(1);
                foreach (var program in rows)
                {
                    var descendants = program.Descendants("td").ToArray();
                    var universityName = descendants[1].InnerText;
                    if (universities.FirstOrDefault(u => u.Name == universityName) != null)
                    {

                        var programName = descendants[0].InnerText;
                        if (universities.First(u => u.Name == universityName).Programs.FirstOrDefault(p => p.Name == programName) != null)
                        {
                            universities.First(u => u.Name == universityName).Programs.First(p => p.Name == programName).State = (State)int.Parse(stateValue);
                            universities.First(u => u.Name == universityName).Programs.First(p => p.Name == programName).AcademicField = (AcademicField)int.Parse(fieldValue);
                            universities.First(u => u.Name == universityName).Programs.First(p => p.Name == programName).AcademicTrack = GetTrack((AcademicField)int.Parse(fieldValue));
                        }
                    }
                }
            }
        }
        private static AcademicTrack GetTrack(AcademicField field)
        {
            if (field == AcademicField.Computer || field == AcademicField.Engineering || field == AcademicField.BasicSience || field == AcademicField.Health)
            {
                return AcademicTrack.Scientific;
            }
            else
            {
                return AcademicTrack.NonScientific;
            }

        }
        public static async Task AssignPricesToPrograms(University university)
        {
            var values = new Dictionary<string, string>
                                {
{ "ctl00$ContentPlaceHolder1$DropDownList1" ,$"{university.Id}" },
{ "__VIEWSTATEENCRYPTED" ,$"" },
{"__EVENTTARGET" , @"ctl00$ContentPlaceHolder1$DropDownList1"},
{"__VIEWSTATE", @"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" },
{ "__EVENTVALIDATION", @"rx6t6q3ufB5dcsAz0o8QXNz6DuhpUFQTkGkR7ylV3yds0TKOW+4HG5C7Qf9Mj8Rs5XBUy0Y3YaV0k2spB6VI0v2QJ6FK20aFi3E5mzgqT9fj+0FMHcv8hlUbjDv5jzcURYKChUMyrekLnX227bopWW4X26OdsiJTXdoQbBbwZcUQYdG3q9xq+a/7A/bUT2dMvNHuaGZJNkCiPbAiZb1BExROMflx0ks5gQ6vdzE6ZVnhB8/W8Hs/9qz5kSWmAwkrN3T7jRWo3LeQzKhHhJdGOrXWCVtUjxDeb7i8H4GQwKodA7xhI8gP6SrhMjGM8y0xtUypYfewY1uByXv80kNO4NJ6p10psC4/dKoPoYFz2JrouHHV"}
                                };

            var content = new FormUrlEncodedContent(values);
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync("http://daleel.admission.gov.sd/private_gov_un2020/Default_private.aspx", content);
            var str = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();

            doc.LoadHtml(str);
            var progamTable = doc.DocumentNode.Descendants("table").FirstOrDefault(t => t.Id == "ctl00_ContentPlaceHolder1_GridView1");
            if (progamTable != null)
            {
                var progams = progamTable.Descendants("tr").Skip(1);
                if (progams.Any())
                {
                    foreach (var progam in progams)
                    {
                        var cols = progam.Descendants("td").ToList();
                        var name = cols[0].InnerText;
                        var priceSDG = cols[1].InnerText;
                        var priceUSD = cols[2].InnerText;
                     
                        if (university.Programs.FirstOrDefault(p => p.Name == name) != null)
                        {

                            //university.Programs.FirstOrDefault(p => p.Name == name).Code = code;
                            university.Programs.First(p => p.Name == name).PriceUSD = decimal.Parse(priceUSD);
                            university.Programs.First(p => p.Name == name).PriceSDG = decimal.Parse(priceSDG);
                        }

                    }
                }
            }
        }
        public static async Task AssignCodesToPrograms(University university)
        {
            var values = new Dictionary<string, string>
                                {
{ "ctl00$ContentPlaceHolder1$DropDownList1" ,$"{university.Id}" },
{ "__VIEWSTATEENCRYPTED" ,$"" },
{"__EVENTTARGET" , @"ctl00$ContentPlaceHolder1$DropDownList1"},
{"__VIEWSTATE", @"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" },
{ "__EVENTVALIDATION", @"cK+LdEGl4p1RZeY48PDE2GN+wlMgo5wmbZgZ6Ys9rpnZKlqYPbof2sEnri0Ie9Cs5GnwUxWX4b1yN3kbgMAP5teQwLaRfi6tG80qttEMkKz9V2b0leKn8axqxfQ6Cdml4dKpgL09DILOTKtt6pFC9OdmWei3vdA3dHdfjqeCdFfGHmOors98pihaApcq1Xs0X2++DuLsjrPH9LzHUDpDoqYmb+BC9kITM8FpgLiKNtGxta8tT+/cp0qZTBLb0iXjpTvUUGr6wHfwag8xDeUt7eJ2rDpbg2bd6q6PhdwkioDB+stbCKaqQPj4sTcQza5crXUjDKInYgrA4Gu8JT1wP86M5j3FO5E/+krRDZvY8pqiDfkO"}
                                };

            var content = new FormUrlEncodedContent(values);
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync("http://daleel.admission.gov.sd/org_code/gov_org_code.aspx", content);
            var str = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();

            doc.LoadHtml(str);

            var progamTable = doc.DocumentNode.Descendants("table").FirstOrDefault(t => t.Id == "ctl00_ContentPlaceHolder1_GridView1");
            if(progamTable == null)
            {
                return;
            }
            foreach(var progamRow in progamTable.Descendants("tr").Skip(1))
            {
                var cols = progamRow.Descendants("td").ToArray();
                var name = cols[0].InnerText;
                var code = cols[1].InnerText;
        
                if (university.Programs.FirstOrDefault(p => p.Name == name) != null)
                {

                    university.Programs.FirstOrDefault(p => p.Name == name).Code = code;
                    //university.Programs.First(p => p.Name == name).PriceUSD = decimal.Parse(priceUSD);
                    //university.Programs.First(p => p.Name == name).PriceSDG = decimal.Parse(priceSDG);
                }
            }
        }
        private static async Task<List<AcademicProgram>> GetFieldUniversities(string field)
        {
            var values = new Dictionary<string, string>
                                {
{ "ctl00$ContentPlaceHolder1$DropDownList1" ,$"{field}" },
{ "__VIEWSTATEENCRYPTED" ,$"" },
{ "__EVENTARGUMENT" ,$"" },
{"__EVENTTARGET" , @"ctl00$ContentPlaceHolder1$DropDownList1"},
{"__VIEWSTATE", @"/wEPDwUKLTE2NjA4Mjk1NA9kFgJmD2QWAgIDD2QWAgIBD2QWBAIDDzwrAA0BAA8WBB4LXyFEYXRhQm91bmRnHgtfIUl0ZW1Db3VudGZkZAIFDw9kDxAWAWYWARYCHg5QYXJhbWV0ZXJWYWx1ZQUBMBYBZmRkGAEFI2N0bDAwJENvbnRlbnRQbGFjZUhvbGRlcjEkR3JpZFZpZXcxDzwrAAoBCGZkL4A3R1X77azs3xy9tdZVRpLu4Y0=" },
{ "__EVENTVALIDATION", @"/wEWCgKmz+HXDwK12e7wBAKltsSeCAK6tsSeCAK7tsSeCAK4tsSeCAK+tsSeCAK/tsSeCAK8tsSeCAKttsSeCExCtqikZmsb96qJW4dq86vTzUaO"}
                                };

            var content = new FormUrlEncodedContent(values);
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync("http://daleel.admission.gov.sd/majalat/majalat_ahli.aspx", content);
            var str = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();

            doc.LoadHtml(str);
            var programTable = doc.DocumentNode.Descendants("table").First(n => n.Id == "ctl00_ContentPlaceHolder1_GridView1");
            var rows = programTable.Descendants("tr").Skip(1);
            var programs = new List<AcademicProgram>();
            foreach (var row in rows)
            {
                var cols = row.Descendants("td").ToArray();
                var name = cols[0].InnerText;
                var universityName = cols[1].InnerText;
                var priceSDG = decimal.Parse(cols[2].InnerText);
                var priceUSD = decimal.Parse(cols[3].InnerText);
                var program = new AcademicProgram
                {
                    Name = name
                };
                programs.Add(program);

            }
            return programs;
        }
        private static async Task<List<University>> GetPrivateUniversities()
        {
            var html = await CallUrl("http://daleel.admission.gov.sd/org_code/ahli_org_code.aspx");
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.DocumentNode.Descendants("select").First(n => n.Id == "ctl00_ContentPlaceHolder1_DropDownList1")
                .Descendants("option").Skip(1).Select(node => new University
                {

                    Name = node.InnerText,
                    Id = int.Parse(node.Attributes["value"].Value),
                    Type = UniversityType.Private
                }).ToList();
        }

        private static async Task LoadPrivatePrograms(University university)
        {
            if (university.Id == 185)
            {
                return;
            }
            Console.WriteLine(university.Id);
            var values = new Dictionary<string, string>
                                {
{ "ctl00$ContentPlaceHolder1$DropDownList1" ,$"{university.Id}" },
{ "__VIEWSTATEENCRYPTED" ,$"" },
{ "__LASTFOCUS" ,$"" },
{ "__EVENTARGUMENT" ,$"" },
{"__EVENTTARGET" , @"ctl00$ContentPlaceHolder1$DropDownList1"},
{"__VIEWSTATE", @"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" },
{ "__EVENTVALIDATION", @"n7ZZL/yLcJWhMzD6+xp6SyZm3ggrLI2qVxUoNNB+CSgQa21Vix3pYcWNUn9Yq2QwOY5wh2ArnKWOpSrlkjXu/EYh90ewzeqteIRIaAYhy09xnrnvHRxDBHdyjctDzTqYPx4lCUKYqVRBkYYScDaAMJvNSh2Hox4r6k6XtbJxQONkMq1AS4fvviVulGTtsvHQZdHKV8noNzlgi/Ab6Mq78bx++mSR5OJbGDYAWUPPJHxpAe6YbLpMcsROveSRkvjgklo+lUcQ+nKKp6DjPXNYO9if8Z7qTkl1wqBWlsUAU7dI0zACinUD4j65/f+dBB/s1O5Ahc54RvSqz3VxraXZL2rgtuRByBJzR7ZRVgXbOI/jdnsIGJG1juythOtGop5r4PHepHi8lJBJIOOTSdQO+phkXwZ1tFYewq5aF95jkvE3Naf71PVdlfkdD9ufhA9kZxkMc7lunfhZ1gf/ln1wVvVX5tZ3jfemeRbXWDOnD4lSvah+Z+5iyaum3MVJca86Qs8IV4vJmknWGPfqfrK/hptcnrSx0XYSIyHKqQbf5d95/Y2rklln5W9uItDue8aG9iuI0DHR+U7yMrmCIIXinV1nL0HiBl83e3Q9PDdBYdVr+41nu29OvJKscQeZAvx382BDKkfarh8J9R73eLtssycDPrpXEpPyQMwOtZg48JERcTDOnGbdp7lDnb12p2unAdaVYVjGI1giKquedz8IITVYRQi38XqsVav87T05SyNL9HugSOx0FkkV64GoLFM4eItoCHJJD02/XYgTM6iL53MZ7Ol4I4u35Eg+fwrBygee6Y3NkQ/9G7X3nqNOh7cs70UK8NuQP8vgL/MlVWyDtnuyiFn7u3gwAfDGFQuKARQiwrKhgNniWsyVtkqNMIVCA+EIivLyVXWRf0Q1uJ4SoRtwZ3PVBL9jQrrP3/ldwLn3gxnv0sljCowGhic0+Fu1XR0ye+VXG4HMthhfOHfvuKB7ZMXhroeaZZgllhKFoNEHypOr"}
                                };

            var content = new FormUrlEncodedContent(values);
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync("http://daleel.admission.gov.sd/org_code/ahli_org_code.aspx", content);
            var str = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();

            doc.LoadHtml(str);

            var table = doc.DocumentNode.Descendants("table").FirstOrDefault(t => t.Id == "ctl00_ContentPlaceHolder1_GridView1");
            if (table == null)
            {
                return;
            }
            var cnt = 0;
            foreach (var node in table.Descendants("tr").Skip(1))
            {

                var cols = node.Descendants("td").ToArray();
                var name = cols[0].InnerText;
                var code = cols[1].InnerText;
                university.Programs.FirstOrDefault(p => p.Name == name).Code = code;
             
                //program.Code = code;
                cnt++;
            }

            Console.WriteLine($"num of progams in the uni: {university.Name} is {cnt}");
            //university.Programs = .Select(node =>
            //    {
            //        return new AcademicProgram
            //        {
            //            Name = cols[0].InnerText,
            //            Code =
            //        };

            //    }).ToList();

        }
        public static async Task PopulateProgramCodes(List<University> universities)
        {
            foreach (var university in universities)
            {
                await LoadPrivatePrograms(university);
            }
        }
        public static async Task<List<int>> GetFields()
        {
            var response = await CallUrl("http://daleel.admission.gov.sd/majalat/majalat_ahli.aspx");
            var doc = new HtmlDocument();
            doc.LoadHtml(response);
            return doc.DocumentNode.Descendants("select").First(s => s.Id == "ctl00_ContentPlaceHolder1_DropDownList1")
                .Descendants("option").Skip(1).Select(node => int.Parse(node.Attributes["value"].Value)).ToList();

        }

        private static async Task GetFieldPrograms(List<University> universities, int field)
        {
            var fieldEnum = (AcademicField)field;
            var values = new Dictionary<string, string>
                                {
{ "ctl00$ContentPlaceHolder1$DropDownList1" ,$"{field}" },
{"__EVENTTARGET" , @"ctl00$ContentPlaceHolder1$DropDownList1"},
{"__VIEWSTATE", @"/wEPDwUKLTE2NjA4Mjk1NA9kFgJmD2QWAgIDD2QWAgIBD2QWBAIDDzwrAA0BAA8WBB4LXyFEYXRhQm91bmRnHgtfIUl0ZW1Db3VudGZkZAIFDw9kDxAWAWYWARYCHg5QYXJhbWV0ZXJWYWx1ZQUBMBYBZmRkGAEFI2N0bDAwJENvbnRlbnRQbGFjZUhvbGRlcjEkR3JpZFZpZXcxDzwrAAoBCGZkL4A3R1X77azs3xy9tdZVRpLu4Y0=" },
{ "__EVENTVALIDATION", @"/wEWCgKmz+HXDwK12e7wBAKltsSeCAK6tsSeCAK7tsSeCAK4tsSeCAK+tsSeCAK/tsSeCAK8tsSeCAKttsSeCExCtqikZmsb96qJW4dq86vTzUaO"}
                                };

            var content = new FormUrlEncodedContent(values);
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync("http://daleel.admission.gov.sd/majalat/majalat_ahli.aspx", content);
            var str = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();

            doc.LoadHtml(str);
            foreach (var node in doc.DocumentNode.Descendants("table").First(t => t.Id == "ctl00_ContentPlaceHolder1_GridView1")
               .Descendants("tr").Skip(1))
            {
                var cols = node.Descendants("td").ToArray();
                var university = universities.FirstOrDefault(u => u.Name == cols[1].InnerText);
                if (university == null)
                {
                    continue;
                }

                decimal priceUSD;
                decimal priceSDG;
                var convertedUSD = decimal.TryParse(cols[3].InnerText, out priceUSD);
                var convertedSDG = decimal.TryParse(cols[2].InnerText, out priceSDG);
                if (!convertedSDG)
                {
                    //throw new Exception("Cannot convert to price SDG" + cols[2].InnerText);
                    continue;
                }

                var gender = Gender.All;
                if (cols[0].InnerText.Contains("طالبات"))
                {
                    gender = Gender.Female;
                }
                else if (cols[0].InnerText.Contains("طلاب"))
                {
                    gender = Gender.Male;
                }
                else
                {
                    gender = Gender.All;
                }


                if (university.Programs == null)
                {
                    university.Programs = new List<AcademicProgram>();
                }
                var p = new AcademicProgram
                {
                    Name = cols[0].InnerText,
                    //Code = cols[1].InnerText,
                    PriceSDG = decimal.Parse(cols[2].InnerText),
                    PriceUSD = convertedUSD ? priceUSD : null,
                    UniversityId = university.Id,
                    Gender = gender,
                    AcademicField = fieldEnum,
                    AcademicTrack = GetTrack(fieldEnum)
                };
            university.Programs.Add(p);


        }
    }

    public static async Task<List<University>> ReadPrivateUniversities()
    {

        var universities = await GetPrivateUniversities();

        Console.WriteLine("University Loading");
        var fields = await GetFields();
        Console.WriteLine("Fields");
        foreach (var field in fields)
        {
            await GetFieldPrograms(universities, field);
            Console.WriteLine("Field - " + field);

        }
        universities = universities.Where(u => u.Programs != null).ToList();
        
        Console.WriteLine("Codes" + universities.Count);
        await PopulateProgramCodes(universities);

        return universities;
    }

}


}
