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
    public class HunterPlayerTests
    {
        public HunterPlayer vanHelsing;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            vanHelsing = new HunterPlayer(Hunter.VanHelsing, 10, 0, 2);
        }

        [Test]
        public void MoveTo_Hamburg_CurrentLocationReturnsHamburg()
        {
            vanHelsing.MoveTo(Location.Hamburg);
            Assert.AreEqual(Location.Hamburg, vanHelsing.CurrentLocation);
        }

        [Test]
        public void DrawItemCard_ItemCountReturns1()
        {
            vanHelsing.DrawItemCard();
            Assert.AreEqual(1, vanHelsing.ItemCount);
        }

        [Test]
        public void DrawEventCard_EventCountReturns1()
        {
            vanHelsing.DrawEventCard();
            Assert.AreEqual(1, vanHelsing.EventCount);
        }

        [Test]
        public void DiscardItem_ItemRemovedFromItemsKnownToDraculaAndItemCountDecreased()
        {
            vanHelsing.ItemsKnownToDracula.Add(new ItemCard(Item.Knife));
            vanHelsing.DrawItemCard();
            int cardCountBefore = vanHelsing.ItemCount;
            List<ItemCard> itemDiscard = new List<ItemCard>();
            vanHelsing.DiscardItem(Item.Knife, itemDiscard);
            Assert.AreEqual(cardCountBefore - 1, vanHelsing.ItemCount);
            Assert.AreEqual(null, vanHelsing.ItemsKnownToDracula.Find(card => card.Item == Item.Knife));
        }

        [Test]
        public void DiscardEvent_EventRemovedFromEventsKnownToDraculaAndEventCountDecreased()
        {
            vanHelsing.EventsKnownToDracula.Add(new EventCard(Event.GoodLuck, false, EventType.Keep));
            vanHelsing.DrawEventCard();
            int cardCountBefore = vanHelsing.EventCount;
            List<EventCard> eventDiscard = new List<EventCard>();
            vanHelsing.DiscardEvent(Event.GoodLuck, eventDiscard);
            Assert.AreEqual(cardCountBefore - 1, vanHelsing.EventCount);
            Assert.AreEqual(null, vanHelsing.EventsKnownToDracula.Find(card => card.Event == Event.GoodLuck));
        }

        [Test]
        public void AdjustHealth_Minus1_HealthReturns9()
        {
            vanHelsing.AdjustHealth(-1);
            Assert.AreEqual(9, vanHelsing.Health);
        }

        [Test]
        public void AdjustHealth_Minus20_HealthReturns0()
        {
            vanHelsing.AdjustHealth(-20);
            Assert.AreEqual(0, vanHelsing.Health);
        }

        [Test]
        public void AdjustHealth_Plus20_HealthReturns10()
        {
            vanHelsing.AdjustHealth(20);
            Assert.AreEqual(10, vanHelsing.Health);
        }

    }
}
