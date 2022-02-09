using Guide.Shared.Constants;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Shared.Models
{
    public class AcademicProgram
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double Percentage { get; set; }
        public decimal? PriceSDG { get; set; }
        public decimal? PriceUSD { get; set; }

        public State State { get; set; }
        public AcademicTrack AcademicTrack { get; set; }
        public Gender Gender { get; set; }

        public AcademicField AcademicField { get; set; }

        public int UniversityId { get; set; }
        //public int FacultyId { get; set; }
        //public Faculty Faculty { get; set; } // it is loaded in c#, does not exists in the JSON
        public University University { get; set; }

    }
}
