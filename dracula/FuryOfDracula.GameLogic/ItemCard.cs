using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    [DataContract]
    public class ItemCard
    {
        [DataMember]
        public Item Item { get; private set; }

        public ItemCard(Item item)
        {
            Item = item;
        }
    }
}
