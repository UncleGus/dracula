using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    [DataContract]
    public class Dracula
    {
        [DataMember]
        public int Blood;
        [DataMember]
        public Location CurrentLocation;
        [DataMember]
        public List<Encounter> EncounterHand;
        [DataMember]
        public List<Event> EventHand;
        [DataMember]
        public int EncounterHandSize;
        [DataMember]
        public int EventHandSize;

        public Dracula()
        {
            EventHandSize = 4;
            EncounterHandSize = 5;
        }
    }
}
