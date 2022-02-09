using Guide.Shared.Constants;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Data
{
    class ProgramExcelTemplate
    {
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public string Percentage { get; set; }
        public string PriceSDG { get; set; }
        public string PriceUSD { get; set; }
        public string    AcademicTrack{ get; set; }
        public string    AcademicField{ get; set; }

        public string UniversityName { get; set; }
        public string UniversityType { get; set; }
        public string UniversityId { get; set; }
    
        public string Gender { get; set; }
        public string State{ get; set; }

    }
}
