using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    [DataContract]
    public class EventCard
    {
        [DataMember]
        public Event Event { get; private set; }
        [DataMember]
        public bool IsDraculaCard { get; private set; }
        [DataMember]
        public EventType EventType { get; private set; }

        public EventCard(Event e, bool isDraculaCard, EventType eventType)
        {
            Event = e;
            IsDraculaCard = isDraculaCard;
            EventType = eventType;
        }
    }
}
