using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    [DataContract]
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
