using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace FuryOfDracula.GameLogic
{
    [DataContract(IsReference = true)]
    public class HunterPlayer
    {
        private int _biteCount;
        private int _health;

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
            EncountersInFrontOfPlayer = new List<EncounterTile>();
        }

        [DataMember]
        public Hunter Hunter { get; private set; }

        [DataMember]
        public int MaxHealth { get; private set; }

        [DataMember]
        public int Health
        {
            get { return _health; }
            private set
            {
                if (value > MaxHealth && MaxHealth > 0)
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

        [DataMember]
        public int BiteCount
        {
            get { return _biteCount; }
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
            var ItemCardToDiscard = ItemsKnownToDracula.Find(card => card.Item == itemToDiscard);
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
            var eventCardToDiscard = EventsKnownToDracula.Find(card => card.Event == eventToDiscard);
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

        public int NumberOfKnownItemsOfType(Item item)
        {
            int count = 0;
            ItemCard desiredCard = ItemsKnownToDracula.Find(card => card.Item == item);
            List<ItemCard> itemsTemporarilyRemovedFromHand = new List<ItemCard>();
            while (desiredCard != null)
            {
                count++;
                itemsTemporarilyRemovedFromHand.Add(desiredCard);
                ItemsKnownToDracula.Remove(desiredCard);
                desiredCard = ItemsKnownToDracula.Find(card => card.Item == item);
            }
            ItemsKnownToDracula.AddRange(itemsTemporarilyRemovedFromHand);
            return count;
        }

        public int NumberOfKnownEventsOfType(Event ev)
        {
            int count = 0;
            EventCard desiredCard = EventsKnownToDracula.Find(card => card.Event == ev);
            List<EventCard> eventsTemporarilyRemovedFromHand = new List<EventCard>();
            while (desiredCard != null)
            {
                count++;
                eventsTemporarilyRemovedFromHand.Add(desiredCard);
                EventsKnownToDracula.Remove(desiredCard);
                desiredCard = EventsKnownToDracula.Find(card => card.Event == ev);
            }
            EventsKnownToDracula.AddRange(eventsTemporarilyRemovedFromHand);
            return count;
        }

        public float LikelihoodOfHavingItemOfType(GameState game, Item item)
        {
            if (ItemsKnownToDracula.Find(card => card.Item == item) != null)
            {
                return NumberOfKnownItemsOfType(item);
            }
            int numberOfUnknownCards = ItemCount - ItemsKnownToDracula.Count();
            if (numberOfUnknownCards == 0)
            {
                return 0F;
            }
            int numberOfItemsUnaccountedFor = game.NumberOfItemsOfType(item) - game.NumberOfRevealedItemsOfType(item);
            return (float)numberOfItemsUnaccountedFor / game.ItemDeck.Count();
        }

        public float LikelihoodOfHavingEventOfType(GameState game, Event ev)
        {
            if (EventsKnownToDracula.Find(card => card.Event == ev) != null)
            {
                return NumberOfKnownEventsOfType(ev);
            }
            int numberOfUnknownCards = EventCount - EventsKnownToDracula.Count();
            if (numberOfUnknownCards == 0)
            {
                return 0F;
            }
            int numberOfEventsUnaccountedFor = game.NumberOfEventsOfType(ev) - game.NumberOfRevealedEventsOfType(ev);
            return (float)numberOfEventsUnaccountedFor / game.EventDeck.Count();
        }
    }
}