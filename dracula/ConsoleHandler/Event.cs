using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHandler
{
    public class Event
    {
        public string name;
        public bool isDraculaCard;

        public Event(string newName, bool newIsDraculaCard)
        {
            name = newName;
            isDraculaCard = newIsDraculaCard;
        }

    }

}
