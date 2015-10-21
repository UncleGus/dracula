using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    public class GameState
    {
        public LocationSet Map;
        public ItemSet Items;
        public EventSet Events;
        public EncounterSet Encounters;

        public GameState()
        {
            Map = new LocationSet();
            Items = new ItemSet();
            Events = new EventSet();
            Encounters = new EncounterSet();
        }
    }
}
