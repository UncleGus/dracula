﻿using FuryOfDracula.GameLogic;
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
            vanHelsing.ItemsKnownToDracula.Add(Item.Knife1);
            vanHelsing.DrawItemCard();
            int cardCountBefore = vanHelsing.ItemCount;
            List<Item> itemDiscard = new List<Item>();
            vanHelsing.DiscardItem(Item.Knife1, itemDiscard);
            Assert.AreEqual(cardCountBefore - 1, vanHelsing.ItemCount);
            Assert.AreEqual(false, vanHelsing.ItemsKnownToDracula.Contains(Item.Knife1));
        }

        [Test]
        public void DiscardEvent_EventRemovedFromEventsKnownToDraculaAndEventCountDecreased()
        {
            vanHelsing.EventsKnownToDracula.Add(Event.GoodLuck1);
            vanHelsing.DrawEventCard();
            int cardCountBefore = vanHelsing.EventCount;
            List<Event> eventDiscard = new List<Event>();
            vanHelsing.DiscardEvent(Event.GoodLuck1, eventDiscard);
            Assert.AreEqual(cardCountBefore - 1, vanHelsing.EventCount);
            Assert.AreEqual(false, vanHelsing.EventsKnownToDracula.Contains(Event.GoodLuck1));
        }

    }
}