using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DraculaSimulator
{
    public class ItemDetail
    {
        public string Name { get; set; }

        public ItemDetail(string newName)
        {
            Name = newName;
        }
    }

    public enum Item
    {
        None,
        Crucifix1,
        Crucifix2,
        Crucifix3,
        Dogs1,
        Dogs2,
        FastHorse1,
        FastHorse2,
        FastHorse3,
        Garlic1,
        Garlic2,
        Garlic3,
        Garlic4,
        HeavenlyHost1,
        HeavenlyHost2,
        HolyWater1,
        HolyWater2,
        HolyWater3,
        Knife1,
        Knife2,
        Knife3,
        Knife4,
        Knife5,
        LocalRumors1,
        LocalRumors2,
        Pistol1,
        Pistol2,
        Pistol3,
        Pistol4,
        Pistol5,
        Rifle1,
        Rifle2,
        Rifle3,
        Rifle4,
        SacredBullets1,
        SacredBullets2,
        SacredBullets3,
        Stake1,
        Stake2,
        Stake3,
        Stake4
    }
}
