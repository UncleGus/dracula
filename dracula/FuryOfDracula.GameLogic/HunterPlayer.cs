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
        private int _biteCount;
        [DataMember]
        public int BiteCount
        {
            get
            {
                return _biteCount;
            }
            private set
            {
                if (value < 0)
                {
                    _biteCount = 0;
                }
                else
                {
                    _biteCount = value;
                }
            }
        }
        [DataMember]
        public int BitesRequiredToKill { get; private set; }
        [DataMember]
        public int ItemCount { get; private set; }
        [DataMember]
        public int EventCount { get; private set; }
        [DataMember]
        public Location CurrentLocation { get; private set; }
        [DataMember]
        public List<ItemCard> ItemsKnownToDracula { get; set; }
        [DataMember]
        public List<EventCard> EventsKnownToDracula { get; set; }
        [DataMember]
        public ItemCard ItemShownToDraculaForBeingBitten { get; set; }
        [DataMember]
        public EventCard EventShownToDraculaForBeingBitten { get; set; }
        [DataMember]
        public bool HasDogsFaceUp { get; private set; }
        public Item LastCombatCardChosen { get; set; }
        [DataMember]
        public ConnectionType NextMoveConnectionType { get; private set; }
        [DataMember]
        public Location NextMoveDestination { get; private set; }
        [DataMember]
        public List<Hunter> HuntersInGroup { get; private set; }
        [DataMember]
        public List<EncounterTile> EncountersInFrontOfPlayer { get; set; }

        public HunterPlayer(Hunter hunter, int health, int numberOfBites, int bitesRequiredToKill)
        {
            Hunter = hunter;
            MaxHealth = health;
            Health = health;
            BiteCount = numberOfBites;
            BitesRequiredToKill = bitesRequiredToKill;
            ItemCount = 0;
            EventCount = 0;
            ItemsKnownToDracula = new List<ItemCard>();
            EventsKnownToDracula = new List<EventCard>();
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

        public void DiscardItem(GameState game, Item itemToDiscard)
        {
            ItemCount--;
            ItemCard ItemCardToDiscard = ItemsKnownToDracula.Find(card => card.Item == itemToDiscard);
            if (ItemCardToDiscard != null)
            {
                ItemsKnownToDracula.Remove(ItemCardToDiscard);
            }
            else
            {
                ItemCardToDiscard = game.ItemDeck.Find(card => card.Item == itemToDiscard);
                game.ItemDeck.Remove(ItemCardToDiscard);
            }
            game.ItemDiscard.Add(ItemCardToDiscard);
            if (ItemShownToDraculaForBeingBitten != null && ItemShownToDraculaForBeingBitten.Item == itemToDiscard)
            {
                ItemShownToDraculaForBeingBitten = null;
            }
            if (itemToDiscard == Item.Dogs)
            {
                HasDogsFaceUp = false;
            }

        }

        public void DiscardEvent(GameState game, Event eventToDiscard)
        {
            EventCount--;
            EventCard eventCardToDiscard = EventsKnownToDracula.Find(card => card.Event == eventToDiscard);
            if (eventCardToDiscard != null)
            {
                EventsKnownToDracula.Remove(eventCardToDiscard);
            }
            else
            {
                eventCardToDiscard = game.EventDeck.Find(card => card.Event == eventToDiscard);
                game.EventDeck.Remove(eventCardToDiscard);
            }
            game.EventDiscard.Add(eventCardToDiscard);
            if (EventShownToDraculaForBeingBitten != null && EventShownToDraculaForBeingBitten.Event == eventToDiscard)
            {
                EventShownToDraculaForBeingBitten = null;
            }
        }

        public void AdjustHealth(int amount)
        {
            Health += amount;
        }

        public void AdjustBites(int amount)
        {
            BiteCount += amount;
        }

        public void SetNextMoveDestination(Location destination)
        {
            NextMoveDestination = destination;
        }

        public void SetNextMoveConnectionType(ConnectionType methodOfTravel)
        {
            NextMoveConnectionType = methodOfTravel;
        }

        public void SetDogsFaceUp(bool v)
        {
            HasDogsFaceUp = v;
        }
    }
}
