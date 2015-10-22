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

        public void MoveTo(Location destination)
        {
            CurrentLocation = destination;
            if (Trail[5] != null && Trail[5].DraculaCards[0] != null)
            {
                Trail[5].DraculaCards[0].IsRevealed = false;
            }
            for (int i = 5; i > 0; i--)
            {
                Trail[i] = Trail[i - 1];
            }
            Trail[0] = new DraculaCardSlot(DraculaCardDeck.GetDraculaCardForLocation(destination));
        }

        public bool RevealCardAtPosition(int position)
        {
            if (position < 0 || position > 5)
            {
                return false;
            }
            if (Trail[position] != null)
            {
                Trail[position].DraculaCards[0].IsRevealed = true;
                return true;
            }
            return false;
        }
    }
}
