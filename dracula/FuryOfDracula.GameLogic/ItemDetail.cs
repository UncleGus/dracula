using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    public class ItemDetail
    {
        Item Item;

        public ItemDetail(Item item)
        {
            Item = item;
        }
    }

    public enum Item
    {
        [Description("No item")]
        None,
        [Description("Crucifix")]
        Crucifix1,
        [Description("Crucifix")]
        Crucifix2,
        [Description("Crucifix")]
        Crucifix3,
        [Description("Dogs")]
        Dogs1,
        [Description("Dogs")]
        Dogs2,
        [Description("Fast Horse")]
        FastHorse1,
        [Description("Fast Horse")]
        FastHorse2,
        [Description("Fast Horse")]
        FastHorse3,
        [Description("Garlic")]
        Garlic1,
        [Description("Garlic")]
        Garlic2,
        [Description("Garlic")]
        Garlic3,
        [Description("Garlic")]
        Garlic4,
        [Description("Heavenly Host")]
        HeavenlyHost1,
        [Description("Heavenly Host")]
        HeavenlyHost2,
        [Description("Holy Water")]
        HolyWater1,
        [Description("Holy Water")]
        HolyWater2,
        [Description("Holy Water")]
        HolyWater3,
        [Description("Knife")]
        Knife1,
        [Description("Knife")]
        Knife2,
        [Description("Knife")]
        Knife3,
        [Description("Knife")]
        Knife4,
        [Description("Knife")]
        Knife5,
        [Description("Local Rumors")]
        LocalRumors1,
        [Description("Local Rumors")]
        LocalRumors2,
        [Description("Pistol")]
        Pistol1,
        [Description("Pistol")]
        Pistol2,
        [Description("Pistol")]
        Pistol3,
        [Description("Pistol")]
        Pistol4,
        [Description("Pistol")]
        Pistol5,
        [Description("Rifle")]
        Rifle1,
        [Description("Rifle")]
        Rifle2,
        [Description("Rifle")]
        Rifle3,
        [Description("Rifle")]
        Rifle4,
        [Description("Sacred Bullets")]
        SacredBullets1,
        [Description("Sacred Bullets")]
        SacredBullets2,
        [Description("Sacred Bullets")]
        SacredBullets3,
        [Description("Stake")]
        Stake1,
        [Description("Stake")]
        Stake2,
        [Description("Stake")]
        Stake3,
        [Description("Stake")]
        Stake4
    }
}
