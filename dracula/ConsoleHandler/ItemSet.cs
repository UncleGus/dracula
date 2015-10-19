using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraculaSimulator
{
    public class ItemSet
    {
        private ItemDetail[] ItemDeck;

        public ItemSet()
        {
            ItemDeck = CreateItemDeck();
        }

        private ItemDetail[] CreateItemDeck()
        {
            ItemDetail[] tempItemDeck = new ItemDetail[41];
            tempItemDeck[(int)Item.None] = new ItemDetail("None");
            tempItemDeck[(int)Item.Crucifix1] = new ItemDetail("Crucifix");
            tempItemDeck[(int)Item.Crucifix2] = new ItemDetail("Crucifix");
            tempItemDeck[(int)Item.Crucifix3] = new ItemDetail("Crucifix");
            tempItemDeck[(int)Item.Dogs1] = new ItemDetail("Dogs");
            tempItemDeck[(int)Item.Dogs2] = new ItemDetail("Dogs");
            tempItemDeck[(int)Item.FastHorse1] = new ItemDetail("Fast Horse");
            tempItemDeck[(int)Item.FastHorse2] = new ItemDetail("Fast Horse");
            tempItemDeck[(int)Item.FastHorse3] = new ItemDetail("Fast Horse");
            tempItemDeck[(int)Item.Garlic1] = new ItemDetail("Garlic");
            tempItemDeck[(int)Item.Garlic2] = new ItemDetail("Garlic");
            tempItemDeck[(int)Item.Garlic3] = new ItemDetail("Garlic");
            tempItemDeck[(int)Item.Garlic4] = new ItemDetail("Garlic");
            tempItemDeck[(int)Item.HeavenlyHost1] = new ItemDetail("Heavenly Host");
            tempItemDeck[(int)Item.HeavenlyHost2] = new ItemDetail("Heavenly Host");
            tempItemDeck[(int)Item.HolyWater1] = new ItemDetail("Holy Water");
            tempItemDeck[(int)Item.HolyWater2] = new ItemDetail("Holy Water");
            tempItemDeck[(int)Item.HolyWater3] = new ItemDetail("Holy Water");
            tempItemDeck[(int)Item.Knife1] = new ItemDetail("Knife");
            tempItemDeck[(int)Item.Knife2] = new ItemDetail("Knife");
            tempItemDeck[(int)Item.Knife3] = new ItemDetail("Knife");
            tempItemDeck[(int)Item.Knife4] = new ItemDetail("Knife");
            tempItemDeck[(int)Item.Knife5] = new ItemDetail("Knife");
            tempItemDeck[(int)Item.LocalRumors1] = new ItemDetail("Local Rumors");
            tempItemDeck[(int)Item.LocalRumors2] = new ItemDetail("Local Rumors");
            tempItemDeck[(int)Item.Pistol1] = new ItemDetail("Pistol");
            tempItemDeck[(int)Item.Pistol2] = new ItemDetail("Pistol");
            tempItemDeck[(int)Item.Pistol3] = new ItemDetail("Pistol");
            tempItemDeck[(int)Item.Pistol4] = new ItemDetail("Pistol");
            tempItemDeck[(int)Item.Pistol5] = new ItemDetail("Pistol");
            tempItemDeck[(int)Item.Rifle1] = new ItemDetail("Rifle");
            tempItemDeck[(int)Item.Rifle2] = new ItemDetail("Rifle");
            tempItemDeck[(int)Item.Rifle3] = new ItemDetail("Rifle");
            tempItemDeck[(int)Item.Rifle4] = new ItemDetail("Rifle");
            tempItemDeck[(int)Item.SacredBullets1] = new ItemDetail("Sacred Bullets");
            tempItemDeck[(int)Item.SacredBullets2] = new ItemDetail("Sacred Bullets");
            tempItemDeck[(int)Item.SacredBullets3] = new ItemDetail("Sacred Bullets");
            tempItemDeck[(int)Item.Stake1] = new ItemDetail("Stake");
            tempItemDeck[(int)Item.Stake2] = new ItemDetail("Stake");
            tempItemDeck[(int)Item.Stake3] = new ItemDetail("Stake");
            tempItemDeck[(int)Item.Stake4] = new ItemDetail("Stake");
            return tempItemDeck;
        }

        public string ItemName(Item item)
        {
            return ItemDeck[(int)item].Name;
        }
    }


}
