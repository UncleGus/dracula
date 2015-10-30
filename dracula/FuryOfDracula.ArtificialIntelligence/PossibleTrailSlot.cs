using FuryOfDracula.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.ArtificialIntelligence
{
    public class PossibleTrailSlot
    {
        public Location Location;
        public Power Power;

        public PossibleTrailSlot(Location location, Power power)
        {
            Location = location;
            Power = power;
        }
    }
}
