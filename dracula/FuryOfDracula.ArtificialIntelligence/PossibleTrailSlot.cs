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
        public TimeOfDay TimeOfDay;
        public bool IsRevealed;
        public CardBack CardBack;

        public PossibleTrailSlot(Location location, Power power, TimeOfDay timeOfDay, CardBack cardBack)
        {
            Location = location;
            Power = power;
            TimeOfDay = timeOfDay;
            CardBack = cardBack;
            if (power == Power.Feed || power == Power.DarkCall)
            {
                IsRevealed = true;
            }
            else
            {
                IsRevealed = false;
            }
        }

        public PossibleTrailSlot(Location location, Power power, TimeOfDay timeOfDay)
        {
            Location = location;
            Power = power;
            if (power == Power.Feed || power == Power.DarkCall)
            {
                IsRevealed = true;
            }
            else
            {
                IsRevealed = false;
            }
            TimeOfDay = timeOfDay;
        }

        public PossibleTrailSlot(Location location, Power power)
        {
            Location = location;
            Power = power;
            if (power == Power.Feed || power == Power.DarkCall)
            {
                IsRevealed = true;
            }
            else
            {
                IsRevealed = false;
            }
            TimeOfDay = TimeOfDay.None;
            CardBack = CardBack.None;
        }
    }
}
