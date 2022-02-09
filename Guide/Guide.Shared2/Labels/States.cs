using Guide.Shared.Constants;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Shared.Labels
{
    public static class States
    {
        public static Dictionary<State, string> Labels = new Dictionary<State, string>
        {
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
