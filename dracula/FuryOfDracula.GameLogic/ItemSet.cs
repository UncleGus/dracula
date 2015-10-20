using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    public class ItemSet
    {
        private ItemDetail[] itemDeck;

        public ItemSet()
        {
            itemDeck = CreateItemDeck();
        }

        private ItemDetail[] CreateItemDeck()
        {
            ItemDetail[] tempItemDeck = new ItemDetail[41];
            tempItemDeck[(int)Item.None] = new ItemDetail((int)Item.None);
            tempItemDeck[(int)Item.Crucifix1] = new ItemDetail(Item.Crucifix1);
            tempItemDeck[(int)Item.Crucifix2] = new ItemDetail(Item.Crucifix2);
            tempItemDeck[(int)Item.Crucifix3] = new ItemDetail(Item.Crucifix3);
            tempItemDeck[(int)Item.Dogs1] = new ItemDetail(Item.Dogs1);
            tempItemDeck[(int)Item.Dogs2] = new ItemDetail(Item.Dogs2);
            tempItemDeck[(int)Item.FastHorse1] = new ItemDetail(Item.FastHorse1);
            tempItemDeck[(int)Item.FastHorse2] = new ItemDetail(Item.FastHorse2);
            tempItemDeck[(int)Item.FastHorse3] = new ItemDetail(Item.FastHorse3);
            tempItemDeck[(int)Item.Garlic1] = new ItemDetail(Item.Garlic1);
            tempItemDeck[(int)Item.Garlic2] = new ItemDetail(Item.Garlic2);
            tempItemDeck[(int)Item.Garlic3] = new ItemDetail(Item.Garlic3);
            tempItemDeck[(int)Item.Garlic4] = new ItemDetail(Item.Garlic4);
            tempItemDeck[(int)Item.HeavenlyHost1] = new ItemDetail(Item.HeavenlyHost1);
            tempItemDeck[(int)Item.HeavenlyHost2] = new ItemDetail(Item.HeavenlyHost2);
            tempItemDeck[(int)Item.HolyWater1] = new ItemDetail(Item.HolyWater1);
            tempItemDeck[(int)Item.HolyWater2] = new ItemDetail(Item.HolyWater2);
            tempItemDeck[(int)Item.HolyWater3] = new ItemDetail(Item.HolyWater3);
            tempItemDeck[(int)Item.Knife1] = new ItemDetail(Item.Knife1);
            tempItemDeck[(int)Item.Knife2] = new ItemDetail(Item.Knife2);
            tempItemDeck[(int)Item.Knife3] = new ItemDetail(Item.Knife3);
            tempItemDeck[(int)Item.Knife4] = new ItemDetail(Item.Knife4);
            tempItemDeck[(int)Item.Knife5] = new ItemDetail(Item.Knife5);
            tempItemDeck[(int)Item.LocalRumors1] = new ItemDetail(Item.LocalRumors1);
            tempItemDeck[(int)Item.LocalRumors2] = new ItemDetail(Item.LocalRumors2);
            tempItemDeck[(int)Item.Pistol1] = new ItemDetail(Item.Pistol1);
            tempItemDeck[(int)Item.Pistol2] = new ItemDetail(Item.Pistol2);
            tempItemDeck[(int)Item.Pistol3] = new ItemDetail(Item.Pistol3);
            tempItemDeck[(int)Item.Pistol4] = new ItemDetail(Item.Pistol4);
            tempItemDeck[(int)Item.Pistol5] = new ItemDetail(Item.Pistol5);
            tempItemDeck[(int)Item.Rifle1] = new ItemDetail(Item.Rifle1);
            tempItemDeck[(int)Item.Rifle2] = new ItemDetail(Item.Rifle2);
            tempItemDeck[(int)Item.Rifle3] = new ItemDetail(Item.Rifle3);
            tempItemDeck[(int)Item.Rifle4] = new ItemDetail(Item.Rifle4);
            tempItemDeck[(int)Item.SacredBullets1] = new ItemDetail(Item.SacredBullets1);
            tempItemDeck[(int)Item.SacredBullets2] = new ItemDetail(Item.SacredBullets2);
            tempItemDeck[(int)Item.SacredBullets3] = new ItemDetail(Item.SacredBullets3);
            tempItemDeck[(int)Item.Stake1] = new ItemDetail(Item.Stake1);
            tempItemDeck[(int)Item.Stake2] = new ItemDetail(Item.Stake2);
            tempItemDeck[(int)Item.Stake3] = new ItemDetail(Item.Stake3);
            tempItemDeck[(int)Item.Stake4] = new ItemDetail(Item.Stake4);

            return tempItemDeck;
        }
    }
}
