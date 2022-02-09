using Guide.Shared.Constants;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Shared.Models
{
    public class University
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UniversityType Type{ get; set; }
        public List<AcademicProgram> Programs { get; set; }
        //public List<Faculty> Faculties { get; set; }
    }
}
