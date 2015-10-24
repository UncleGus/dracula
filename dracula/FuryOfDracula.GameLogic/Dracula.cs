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
        public int Blood { get; private set; }
        [DataMember]
        public Location CurrentLocation { get; private set; }
        [DataMember]
        public List<Encounter> EncounterHand { get; private set; }
        [DataMember]
        public List<EventCard> EventHand { get; private set; }
        [DataMember]
        public int EncounterHandSize { get; set; }
        [DataMember]
        public int EventHandSize { get; set; }
        [DataMember]
        public DraculaCardSlot[] Trail { get; private set; }
        [DataMember]
        public DraculaCardSlot[] Catacombs { get; private set; }
        change this so that its just a list of DraculaCard 
        public DraculaCardSet DraculaCardDeck { get; private set; }
        [DataMember]
        public Location LocationWhereHideWasUsed { get; private set; }

        public Dracula()
        {
            EventHandSize = 4;
            EncounterHandSize = 5;
            EventHand = new List<EventCard>();
            EncounterHand = new List<Encounter>();
            Blood = 15;
            Trail = new DraculaCardSlot[6];
            Catacombs = new DraculaCardSlot[3];
            DraculaCardDeck = new DraculaCardSet();
        }

        public DraculaCardSlot MoveTo(Location destination, Power power)
        {
            DraculaCardSlot doubleBackedCard = null;
            int catacombsSlot = 0;
            if (power == Power.DoubleBack)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (Trail[i] != null && Trail[i].DraculaCards[0].Location == destination)
                    {
                        doubleBackedCard = Trail[i];
                        for (int j = i; j > 0; j--)
                        {
                            Trail[j] = Trail[j - 1];
                        }
                        Trail[0] = doubleBackedCard;
                        if (Trail[0].DraculaCards[0] == null)
                        {
                            Trail[0].DraculaCards[0] = DraculaCardDeck.GetDraculaCardForPower(Power.DoubleBack);
                            Trail[0].DraculaCards[0].IsRevealed = true;
                        }
                        else
                        {
                            Trail[0].DraculaCards[1] = DraculaCardDeck.GetDraculaCardForPower(Power.DoubleBack);
                            Trail[0].DraculaCards[1].IsRevealed = true;
                        }
                        CurrentLocation = destination;
                        return null;
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    if (Catacombs[i] != null && Catacombs[i].DraculaCards[0].Location == destination)
                    {
                        doubleBackedCard = Catacombs[i];
                        catacombsSlot = i;
                    }
                }
            }
            DraculaCardSlot cardSlotDroppedOffTrail = Trail[5];
            for (int i = 5; i > 0; i--)
            {
                Trail[i] = Trail[i - 1];
            }
            if (destination != Location.Nowhere)
            {
                CurrentLocation = destination;
                if (doubleBackedCard == null)
                {
                    Trail[0] = new DraculaCardSlot(DraculaCardDeck.GetDraculaCardForLocation(destination));
                    if (destination == Location.CastleDracula)
                    {
                        Trail[0].DraculaCards[0].IsRevealed = true;
                    }
                } else
                {
                    Trail[0] = doubleBackedCard;
                    Catacombs[catacombsSlot] = null;
                }
            }
            else
            {
                Trail[0] = new DraculaCardSlot();
            }
            if (power != Power.None)
            {
                if (Trail[0].DraculaCards[0] == null)
                {
                    Trail[0].DraculaCards[0] = DraculaCardDeck.GetDraculaCardForPower(power);
                    if (power != Power.Hide || CurrentLocation == Location.CastleDracula)
                    {
                        Trail[0].DraculaCards[0].IsRevealed = true;
                    }
                }
                else
                {
                    Trail[0].DraculaCards[1] = DraculaCardDeck.GetDraculaCardForPower(power);
                    if (power != Power.Hide)
                    {
                        Trail[0].DraculaCards[1].IsRevealed = true;
                    }
                }
            }
            if (power == Power.Hide)
            {
                LocationWhereHideWasUsed = CurrentLocation;
            }
            return cardSlotDroppedOffTrail;
        }

        public void RevealEncountersAtPositionInTrail(GameState game, int positionRevealed)
        {
            if (positionRevealed < 6)
            {
                if (Trail[positionRevealed].Encounters[0] != Encounter.None)
                {
                    game.Encounters.GetEncounterDetail(Trail[positionRevealed].Encounters[0]).IsRevealed = true;
                }
                if (Trail[positionRevealed].Encounters[1] != Encounter.None)
                {
                    game.Encounters.GetEncounterDetail(Trail[positionRevealed].Encounters[1]).IsRevealed = true;
                }
            } else
            {
                positionRevealed -= 6;
                if (Catacombs[positionRevealed].Encounters[0] != Encounter.None)
                {
                    game.Encounters.GetEncounterDetail(Catacombs[positionRevealed].Encounters[0]).IsRevealed = true;
                }
                if (Catacombs[positionRevealed].Encounters[1] != Encounter.None)
                {
                    game.Encounters.GetEncounterDetail(Catacombs[positionRevealed].Encounters[1]).IsRevealed = true;
                }
            }
        }

        public int RevealCardInTrailWithLocation(Location location)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Trail[i].DraculaCards[0].Location == location)
                {
                    Trail[i].DraculaCards[0].IsRevealed = true;
                    return i;
                }
            }
            return -1;
        }

        public void DiscardEvent(EventCard eventToDiscard, List<EventCard> discardPile)
        {
            EventHand.Remove(eventToDiscard);
            discardPile.Add(eventToDiscard);
        }

        public void DiscardCatacombsCards(GameState game, List<int> list)
        {
            List<Encounter> encountersDiscarded = new List<Encounter>();
            foreach (int i in list)
            {
                if (Catacombs[i].Encounters[0] != Encounter.None)
                {
                    encountersDiscarded.Add(Catacombs[i].Encounters[0]);
                }
                if (Catacombs[i].Encounters[1] != Encounter.None)
                {
                    encountersDiscarded.Add(Catacombs[i].Encounters[1]);
                }
                Catacombs[i] = null;
            }
            foreach (Encounter enc in encountersDiscarded)
            {
                game.Encounters.GetEncounterDetail(enc).IsRevealed = false;
                game.EncounterPool.Add(enc);
            }
        }

        public bool RevealCardAtPosition(int position)
        {
            if (position < 0 || position > 8)
            {
                return false;
            }
            if (position < 6)
            {
                if (Trail[position] != null)
                {
                    Trail[position].DraculaCards[0].IsRevealed = true;
                    if (Trail[position].DraculaCards[1] != null)
                    {
                        Trail[position].DraculaCards[1].IsRevealed = true;
                    }
                    return true;
                }
            } else if (Catacombs[position - 6] != null)
            {
                Catacombs[position - 6].DraculaCards[0].IsRevealed = true;
                if (Catacombs[position - 6].DraculaCards[1] != null)
                {
                    Catacombs[position - 6].DraculaCards[1].IsRevealed = true;
                }
                return true;
            }
            return false;
        }

        public void DiscardEncounterFromCardSlot(Encounter encounterToDiscard, DraculaCardSlot draculaCardSlot, List<Encounter> encounterPool)
        {
            if (draculaCardSlot.Encounters[0] == encounterToDiscard)
            {
                draculaCardSlot.Encounters[0] = draculaCardSlot.Encounters[1];
            }
            draculaCardSlot.Encounters[1] = Encounter.None;
            encounterPool.Add(encounterToDiscard);
        }

        public void DrawEncounter(List<Encounter> encounterPool)
        {
            Encounter encounterDrawn = encounterPool[new Random().Next(0, encounterPool.Count())];
            EncounterHand.Add(encounterDrawn);
            encounterPool.Remove(encounterDrawn);
        }

        public void PlaceEncounterOnCard(Encounter encounterToPlace, DraculaCardSlot card)
        {
            if (card.Encounters[0] == Encounter.None)
            {
                card.Encounters[0] = encounterToPlace;
            }
            else
            {
                card.Encounters[1] = encounterToPlace;
            }
            EncounterHand.Remove(encounterToPlace);
        }

        public List<Encounter> DiscardHide()
        {
            List<Encounter> encountersDiscarded = new List<Encounter>();
            for (int i = 0; i < 6; i++)
            {
                if (Trail[i].DraculaCards[0].Power == Power.Hide)
                {
                    LocationWhereHideWasUsed = Location.Nowhere;
                    if (Trail[i].Encounters[0] != Encounter.None)
                    {
                        encountersDiscarded.Add(Trail[i].Encounters[0]);
                    }
                    if (Trail[i].Encounters[1] != Encounter.None)
                    {
                        encountersDiscarded.Add(Trail[i].Encounters[1]);
                    }
                    for (int j = i; j < 5; j++)
                    {
                        Trail[j] = Trail[j + 1];
                    }
                    Trail[5] = null;
                    return encountersDiscarded;
                }
            }
            return encountersDiscarded;
        }

        public bool TakeEvent(List<EventCard> eventDeck, EventSet eventCards)
        {
            EventCard cardDrawn;
            do
            {
                cardDrawn = eventDeck[new Random().Next(0, eventDeck.Count())];
            } while (!eventCards.EventDetail(cardDrawn).IsDraculaCard);
            EventHand.Add(cardDrawn);
            eventDeck.Remove(cardDrawn);
            if (EventHand.Count() > EventHandSize)
            {
                return true;
            }
            return false;
        }

        public int RevealHideCard()
        {
            for (int i = 0; i < 6; i++) {
                if (Trail[i].DraculaCards[0].Power == Power.Hide)
                {
                    Trail[i].DraculaCards[0].IsRevealed = true;
                    return i;
                }
            }
            return -1;
        }
    }
}
