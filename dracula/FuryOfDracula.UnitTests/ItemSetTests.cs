using FuryOfDracula.GameLogic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.UnitTests
{
    [TestFixture]
    public class ItemSetTests
    {
        public ItemSet itemDeck;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            itemDeck = new ItemSet();
        }

        [Test]
        public void GetItemsFromString_Knife_ReturnsAllKnifeItems()
        {
            List<Item> knifeItems = itemDeck.GetItemsFromString("knife");
            Assert.AreEqual(true, knifeItems.Contains(Item.Knife1));
            Assert.AreEqual(true, knifeItems.Contains(Item.Knife2));
            Assert.AreEqual(true, knifeItems.Contains(Item.Knife3));
            Assert.AreEqual(true, knifeItems.Contains(Item.Knife4));
            Assert.AreEqual(true, knifeItems.Contains(Item.Knife5));
            Assert.AreEqual(false, knifeItems.Contains(Item.HeavenlyHost1));
        }

        [Test]
        public void GetItemsFromString_MismatchingString_ReturnsEmptyList()
        {
            Assert.AreEqual(0, itemDeck.GetItemsFromString("B").Count());            
        }

        [Test]
        public void GetItemsFromString_AmbiguousString_ReturnsEmptyList()
        {
            Assert.AreEqual(0, itemDeck.GetItemsFromString("H").Count());
        }
    }
}
