using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    [DataContract]
    public class HunterPlayer
    {
        [DataMember]
        public Hunter Hunter;
        [DataMember]
        public int Health;
        [DataMember]
        public int BiteCount;
        [DataMember]
        public int BitesRequiredToKill;
        [DataMember]
        public int ItemCount;
        [DataMember]
        public int EventCount;
        [DataMember]
        public Location CurrentLocation;
        [DataMember]
        public List<Item> ItemsKnownToDracula;
        [DataMember]
        public List<Event> EventsKnownToDracula;
        [DataMember]
        public Item ItemShownToDraculaForBeingBitten;
        [DataMember]
        public Event EventShownToDraculaForBeingBitten;
        [DataMember]
        public bool HasDogsFaceUp;
        public Item LastCombatCardChosen;
        [DataMember]
        public ConnectionType NextMoveConnectionType;
        [DataMember]
        public Location NextMoveDestination;
        [DataMember]
        public List<Hunter> HuntersInGroup;

        public HunterPlayer(Hunter hunter, int health, int numberOfBites, int bitesRequiredToKill)
        {
            Hunter = hunter;
            Health = health;
            BiteCount = numberOfBites;
            BitesRequiredToKill = bitesRequiredToKill;
            ItemCount = 0;
            EventCount = 0;
            ItemsKnownToDracula = new List<Item>();
            EventsKnownToDracula = new List<Event>();
            HasDogsFaceUp = false;
            HuntersInGroup.Add(Hunter);
        }
    }
}
