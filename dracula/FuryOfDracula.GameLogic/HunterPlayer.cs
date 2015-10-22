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
        public Hunter Hunter { get; private set; }
        private int _health;
        [DataMember]
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
                else
                {
                    _health = value;
                }
            }
        }
        public int MaxHealth { get; private set; }
        [DataMember]
        public int BiteCount { get; private set; }
        [DataMember]
        public int BitesRequiredToKill { get; private set; }
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
            MaxHealth = health;
            Health = health;
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

        public void DiscardItem(List<Item> possibleItems, List<Item> itemDiscard)
        {
            ItemCount--;
            foreach (Item i in possibleItems)
            {
                if (ItemsKnownToDracula.Contains(i))
                {
                    ItemsKnownToDracula.Remove(i);
                    itemDiscard.Add(i);
                    return;
                }
            }            
            itemDiscard.Add(possibleItems.First());
        }

        public void DiscardEvent(List<Event> possibleEvents, List<Event> eventDiscard)
        {
            EventCount--;
            foreach (Event e in possibleEvents)
            {
                if (EventsKnownToDracula.Contains(e))
                {
                    EventsKnownToDracula.Remove(e);
                    eventDiscard.Add(e);
                    return;
                }
            }
            eventDiscard.Add(possibleEvents.First());
        }

        public void AdjustHealth(int amount)
        {
            Health += amount;
        }
    }
}
