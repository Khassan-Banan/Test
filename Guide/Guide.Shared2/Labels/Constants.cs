using Guide.Shared.Constants;

using System.Collections.Generic;

namespace Guide.Shared.Labels
{
    public static class Constants
    {
        public static Dictionary<AcademicTrack, string> AcademicTracks = new Dictionary<AcademicTrack, string>
        {
            [AcademicTrack.Scientific] = "علمي",
            [AcademicTrack.NonScientific] = "أدبي",
            [AcademicTrack.All] = "الكل"

        };

        public static Dictionary<Gender, string> Genders = new Dictionary<Gender, string>
        {
            [Gender.Male] = "طلاب فقط",
            [Gender.Female] = "طالبات فقط",
            [Gender.All] = "الكل"
        };

        public static Dictionary<Currency, string> Currencies = new Dictionary<Currency, string>
        {
            [Currency.SDG] = "جنيه",
            [Currency.USD] = "دولار",

        };

        public static Dictionary<UniversityType, string> UniversityTypes = new Dictionary<UniversityType, string>
        {
            [UniversityType.Public] = "عام",
            [UniversityType.Private] = "أهلي",
            [UniversityType.All] = "الكل"

        }; public static Dictionary<AdmissionType, string> AdmissionTypes = new Dictionary<AdmissionType, string>
        {
            [AdmissionType.Public] = "عام",
            [AdmissionType.Private] = "خاص",
           

        };

        public static Dictionary<AcademicField, string> AcademicFields = new Dictionary<AcademicField, string>
        {
            [AcademicField.Agriculture] = "العلوم الزراعية",
            [AcademicField.BasicSience] = "العلوم الأساسية",
            [AcademicField.Computer] = "علوم الحاسوب",
            [AcademicField.EconomicAndSocial] = "العلوم الاجتماعية و الاقتصادية",
            [AcademicField.Education] = "العلوم التربوية",
            [AcademicField.Engineering] = "العلوم الهندسية",
            [AcademicField.Health] = "العلوم الصحية",
            [AcademicField.Human] = "العلوم الانسانية",
            [AcademicField.None] = "الكل"

        };

        public static Dictionary<State, string> States = new Dictionary<State, string>
        {
            [State.None] = "الكل",
            [State.Khartoum] = "ولاية الخرطوم",
            [State.Kassala] = "ولاية كسلا",

            [State.AlJazirah] = "ولاية الجزيرة",
            [State.AlQadarif] = "ولاية القضارف",

            [State.NorthDarfur] = "ولاية شمال دارفور",
            [State.Northern] = "الولاية الشمالية",
            [State.NorthKordofan] = "ولاية شمل كردفان",

            [State.WestDarfur] = "ولاية غرب دارفور",
            [State.WestKordofan] = "ولاية غرب كردفان",
            [State.WhiteNile] = "ولاية النيل الأبيض",

            [State.RiverNile] = "ولاية نهر النيل",
            [State.RedSea] = "ولاية البحر الأحمر",

            [State.Sennar] = "ولاية سنار",
            [State.SouthDarfur] = "ولاية جنوب دارفور",
            [State.SouthKordofan] = "ولاية جنوب كردفان",

            [State.CentralDarfur] = "ولاية وسط دارفور",
            [State.EastDarfur] = "ولاية شرق دارفور",
            [State.BlueNile] = "ولاية النيل الأزرق",
        };
    }
}
