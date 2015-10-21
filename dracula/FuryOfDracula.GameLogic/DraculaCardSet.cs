using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    public class DraculaCardSet
    {
        private DraculaCard[] draculaCardDeck;

        public DraculaCardSet()
        {
            draculaCardDeck = CreateDraculaCardDeck();
        }

        private DraculaCard[] CreateDraculaCardDeck()
        {
            throw new NotImplementedException();
        }
    }
}
