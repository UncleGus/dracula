using DraculaSimulator;
using EventHandler;
using LocationHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HunterHandler
{
    [DataContract]
    public class Hunter
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Health { get; set; }
        [DataMember]
        public Location CurrentLocation { get; set; }
        [DataMember]
        public int NumberOfEvents { get; set; }
        [DataMember]
        public int NumberOfItems { get; set; }
        [DataMember]
        public int NumberOfBites { get; set; }
        [DataMember]
        public int NumberOfBitesRequiredToKill { get; set; }
        [DataMember]
        public bool HasDogsFaceUp { get; set; }
        [DataMember]
        public List<int> HuntersInGroup { get; set; }
        [DataMember]
        public string LastItemUsedInCombat { get; set; }
        [DataMember]
        public List<EventDetail> EventsKnownToDracula { get; set; }
        [DataMember]
        public List<ItemDetail> ItemsKnownToDracula { get; set; }
        [DataMember]
        public string TravelType { get; set; }
        [DataMember]
        public LocationDetail Destination { get; set; }
        [DataMember]
        public ItemDetail ItemShownToDraculaForBeingBitten { get; set; }
        [DataMember]
        public EventDetail EventShownToDraculaForBeingBitten { get; set; }
        [DataMember]
        public int Index { get; set; }

        public Hunter(int newIndex, string newName, int newHealth, int newNumberOfBites, int newBitesRequiredToKill)
        {
            Index = newIndex;
            Name = newName;
            Health = newHealth;
            NumberOfEvents = 0;
            NumberOfItems = 0;
            NumberOfBites = newNumberOfBites;
            NumberOfBitesRequiredToKill = newBitesRequiredToKill;
            HasDogsFaceUp = false;
            HuntersInGroup = new List<int>();
            HuntersInGroup.Add(Index);
            LastItemUsedInCombat = null;
            EventsKnownToDracula = new List<EventDetail>();
            ItemsKnownToDracula = new List<ItemDetail>();
            TravelType = null;
            Destination = null;
            ItemShownToDraculaForBeingBitten = null;
            EventShownToDraculaForBeingBitten = null;
        }
    }

    public enum HunterName
    {
        LordGodalming,
        DrSeward,
        VanHelsing,
        MinaHarker
    }

}
