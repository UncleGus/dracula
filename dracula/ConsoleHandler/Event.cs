using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EventHandler
{
    [DataContract]
    public class Event
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public bool isDraculaCard { get; set; }
        [DataMember]
        public EventType type { get; set; }

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
