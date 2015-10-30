using FuryOfDracula.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.ArtificialIntelligence
{
    public class PossibleTrail
    {
        public PossibleTrailSlot[] Trail = new PossibleTrailSlot[6];

        public PossibleTrail();

        public PossibleTrail(Location location, Power power)
        {
            Trail[0] = new PossibleTrailSlot(location, power);
        }

        public bool Contains(Location loc)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Trail[i] != null && Trail[i].Location == loc)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(Power power)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Trail[i] != null && Trail[i].Power == power)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
