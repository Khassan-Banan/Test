using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Shared.Models
{
    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AcademicProgram> Programs { get; set; }
        public int UniversityId { get; set; }
        public University University { get; set; } // it is loaded in c#, does not exists in the JSON
    }
}
