using System.Runtime.Serialization;

namespace FuryOfDracula.GameLogic
{
    [DataContract]
    public class EventCard
    {
        public EventCard(Event e, bool isDraculaCard, EventType eventType)
        {
            Event = e;
            IsDraculaCard = isDraculaCard;
            EventType = eventType;
        }

        [DataMember]
        public Event Event { get; private set; }

        [DataMember]
        public bool IsDraculaCard { get; private set; }

        [DataMember]
        public EventType EventType { get; private set; }
    }
}