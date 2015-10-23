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
        public List<Event> EventHand { get; private set; }
        [DataMember]
        public int EncounterHandSize { get; set; }
        [DataMember]
        public int EventHandSize { get; set; }
        [DataMember]
        public DraculaCardSlot[] Trail { get; private set; }
        [DataMember]
        public DraculaCardSlot[] Catacombs { get; private set; }
        public DraculaCardSet DraculaCardDeck { get; private set; }

        public Dracula()
        {
            EventHandSize = 4;
            EncounterHandSize = 5;
            EventHand = new List<Event>();
            EncounterHand = new List<Encounter>();
            Blood = 15;
            Trail = new DraculaCardSlot[6];
            Catacombs = new DraculaCardSlot[3];
            DraculaCardDeck = new DraculaCardSet();
        }

        public DraculaCardSlot MoveTo(Location destination, Power power)
        {
            if (power == Power.DoubleBack)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (Trail[i].DraculaCards[0].Location == destination)
                    {
                        DraculaCardSlot doubleBackedCard = Trail[i];
                        for (int j = i; j > 0; j--)
                        {
                            Trail[j] = Trail[j - 1];
                        }
                        Trail[0] = doubleBackedCard;
                        if (Trail[0].DraculaCards[1] == null)
                        {
                            Trail[0].DraculaCards[1] = DraculaCardDeck.GetDraculaCardForPower(Power.DoubleBack);
                        }
                        else
                        {
                            Trail[0].DraculaCards[2] = DraculaCardDeck.GetDraculaCardForPower(Power.DoubleBack);
                        }
                        return null;
                    }
                }
                return null;
            }
            else
            {
                DraculaCardSlot cardSlotDroppedOffTrail = Trail[5];
                for (int i = 5; i > 0; i--)
                {
                    Trail[i] = Trail[i - 1];
                }
                if (destination != Location.Nowhere)
                {
                    CurrentLocation = destination;
                    Trail[0] = new DraculaCardSlot(DraculaCardDeck.GetDraculaCardForLocation(destination));
                }
                if (power != Power.None)
                {
                    if (Trail[0] == null)
                    {
                        Trail[0] = new DraculaCardSlot(DraculaCardDeck.GetDraculaCardForPower(power));
                    }
                    else
                    {
                        Trail[0].DraculaCards[1] = DraculaCardDeck.GetDraculaCardForPower(power);
                    }
                }
                return cardSlotDroppedOffTrail;
            }
        }

        public bool RevealCardAtPosition(GameState game, int position)
        {
            if (position < 0 || position > 5)
            {
                return false;
            }
            if (Trail[position] != null)
            {
                Trail[position].DraculaCards[0].IsRevealed = true;
                if (Trail[position].DraculaCards[1] != null)
                {
                    Trail[position].DraculaCards[1].IsRevealed = true;
                }
                if (Trail[position].Encounters[0] != null)
                {
                    game.Encounters.GetEncounterDetail(Trail[position].Encounters[0]).IsRevealed = true;
                }
                if (Trail[position].Encounters[1] != null)
                {
                    game.Encounters.GetEncounterDetail(Trail[position].Encounters[1]).IsRevealed = true;
                }
                return true;
            }
            return false;
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
    }
}
