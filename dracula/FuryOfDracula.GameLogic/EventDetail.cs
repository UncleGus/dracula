using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    public class EventDetail
    {
        public Event Event;
        public bool IsDraculaCard;
        public EventType EventType;

        public EventDetail(Event e, bool isDraculaCard, EventType eventType)
        {
            Event = e;
            IsDraculaCard = isDraculaCard;
            EventType = eventType;
        }
    }
}
