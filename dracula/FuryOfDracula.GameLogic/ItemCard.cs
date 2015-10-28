using System.Runtime.Serialization;

namespace FuryOfDracula.GameLogic
{
    [DataContract(IsReference = true)]
    public class ItemCard
    {
        public ItemCard(Item item)
        {
            Item = item;
        }

        [DataMember]
        public Item Item { get; private set; }
    }
}