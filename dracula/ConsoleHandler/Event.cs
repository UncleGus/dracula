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
        public EventType type;

        public Event(string newName, bool newIsDraculaCard, EventType eventType)
        {
            name = newName;
            isDraculaCard = newIsDraculaCard;
            type = eventType;
        }

    }

    public enum EventType
    {
        Ally,
        Keep,
        PlayImmediately
    }

}
