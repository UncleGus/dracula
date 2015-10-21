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
        public Hunter Hunter { get; }
        [DataMember]
        private int _health;
        public int Health
        {
            get
            {
                return _health;
            }
            private set
            {
                if (value > MaxHealth)
                {
                    _health = MaxHealth;
                }
                else if (value < 0)
                {
                    _health = 0;
                }
            }
        }
        public int MaxHealth { get; }
        [DataMember]
        public int BiteCount { get; private set; }
        [DataMember]
        public int BitesRequiredToKill { get; }
        [DataMember]
        public int ItemCount { get; private set; }
        [DataMember]
        public int EventCount { get; private set; }
        [DataMember]
        public Location CurrentLocation { get; private set; }
        [DataMember]
        public List<Item> ItemsKnownToDracula { get; set; }
        [DataMember]
        public List<Event> EventsKnownToDracula { get; set; }
        [DataMember]
        public Item ItemShownToDraculaForBeingBitten { get; private set; }
        [DataMember]
        public Event EventShownToDraculaForBeingBitten { get; private set; }
        [DataMember]
        public bool HasDogsFaceUp { get; private set; }
        public Item LastCombatCardChosen { get; private set; }
        [DataMember]
        public ConnectionType NextMoveConnectionType { get; private set; }
        [DataMember]
        public Location NextMoveDestination { get; private set; }
        [DataMember]
        public List<Hunter> HuntersInGroup { get; private set; }
        [DataMember]
        public List<Encounter> EncountersInFrontOfPlayer { get; set; }

        public HunterPlayer(Hunter hunter, int health, int numberOfBites, int bitesRequiredToKill)
        {
            Hunter = hunter;
            Health = health;
            MaxHealth = health;
            BiteCount = numberOfBites;
            BitesRequiredToKill = bitesRequiredToKill;
            ItemCount = 0;
            EventCount = 0;
            ItemsKnownToDracula = new List<Item>();
            EventsKnownToDracula = new List<Event>();
            HasDogsFaceUp = false;
            HuntersInGroup = new List<Hunter>();
            HuntersInGroup.Add(Hunter);
        }

        public void MoveTo(Location location)
        {
            CurrentLocation = location;
        }

        public void DrawItemCard()
        {
            ItemCount++;
        }

        public void DrawEventCard()
        {
            EventCount++;
        }

        public void DiscardItem(Item item, List<Item> itemDiscard)
        {
            ItemsKnownToDracula.Remove(item);
            itemDiscard.Add(item);
            ItemCount--;
        }

        public void DiscardEvent(Event e, List<Event> eventDiscard)
        {
            EventsKnownToDracula.Remove(e);
            eventDiscard.Add(e);
            EventCount--;
        }
    }
}
