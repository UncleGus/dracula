using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FuryOfDracula.GameLogic
{
    [DataContract(IsReference = true)]
    public class DraculaCardSlot
    {
        [DataMember]
        public List<DraculaCard> DraculaCards = new List<DraculaCard>();

        [DataMember]
        public List<EncounterTile> EncounterTiles = new List<EncounterTile>();

        public DraculaCardSlot(DraculaCard card)
        {
            DraculaCards.Add(card);
        }

        public DraculaCardSlot()
        {
        }
    }
}