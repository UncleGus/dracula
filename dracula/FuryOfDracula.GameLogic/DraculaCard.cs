using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    public class DraculaCard
    {
        public string Abbreviation;
        public Location Location;
        public Power Power;

        public DraculaCard(string abbreviation, Location location, Power power)
        {
            Abbreviation = abbreviation;
            Location = location;
            Power = power;
        }
    }
}
