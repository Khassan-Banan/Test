using Guide.Shared.Constants;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Data
{
    internal static class Helpers
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        
        public static Gender GetGender(string name)
        {
            var gender = Gender.All;
            if (name.Contains("طالبات"))
            {
                gender = Gender.Female;
            }
            else if (name.Contains("طلاب"))
            {
                gender = Gender.Male;
            }
            else
            {
                gender = Gender.All;
            }
            return gender;
        }

        public static AcademicTrack GetTrack(AcademicField field)
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
    }
}
