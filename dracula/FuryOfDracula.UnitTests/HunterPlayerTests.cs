﻿using System.Collections.Generic;
using FuryOfDracula.GameLogic;
using NUnit.Framework;
using System.Linq;

namespace FuryOfDracula.UnitTests
{
    [TestFixture]
    public class HunterPlayerTests
    {
        public HunterPlayer vanHelsing;

        [SetUp]
        public void BeforeAll()
        {
            vanHelsing = new HunterPlayer(Hunter.VanHelsing, 10, 0, 2);
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

        [Test]
        public void DiscardEvent_EventRemovedFromEventsKnownToDraculaAndEventCountDecreased()
        {
            vanHelsing.EventsKnownToDracula.Add(new EventCard(Event.GoodLuck, false, EventType.Keep));
            vanHelsing.DrawEventCard();
            var cardCountBefore = vanHelsing.EventCount;
            var eventDiscard = new List<EventCard>();
            vanHelsing.DiscardEvent(new GameState(), Event.GoodLuck);
            Assert.AreEqual(cardCountBefore - 1, vanHelsing.EventCount);
            Assert.AreEqual(null, vanHelsing.EventsKnownToDracula.Find(card => card.Event == Event.GoodLuck));
        }

        [Test]
        public void DiscardItem_ItemRemovedFromItemsKnownToDraculaAndItemCountDecreased()
        {
            vanHelsing.ItemsKnownToDracula.Add(new ItemCard(Item.Knife));
            vanHelsing.DrawItemCard();
            var cardCountBefore = vanHelsing.ItemCount;
            var itemDiscard = new List<ItemCard>();
            vanHelsing.DiscardItem(new GameState(), Item.Knife);
            Assert.AreEqual(cardCountBefore - 1, vanHelsing.ItemCount);
            Assert.AreEqual(null, vanHelsing.ItemsKnownToDracula.Find(card => card.Item == Item.Knife));
        }

        [Test]
        public void DrawEventCard_EventCountReturns1()
        {
            vanHelsing.DrawEventCard();
            Assert.AreEqual(1, vanHelsing.EventCount);
        }

        [Test]
        public void DrawItemCard_ItemCountReturns1()
        {
            vanHelsing.DrawItemCard();
            Assert.AreEqual(1, vanHelsing.ItemCount);
        }

        [Test]
        public void MoveTo_Hamburg_CurrentLocationReturnsHamburg()
        {
            vanHelsing.MoveTo(Location.Hamburg);
            Assert.AreEqual(Location.Hamburg, vanHelsing.CurrentLocation);
        }

        [Test]
        public void NumberOfKnownItemsOfType_CrucifixThree_Returns3()
        {
            vanHelsing.ItemsKnownToDracula.Add(new ItemCard(Item.Crucifix));
            vanHelsing.ItemsKnownToDracula.Add(new ItemCard(Item.Crucifix));
            vanHelsing.ItemsKnownToDracula.Add(new ItemCard(Item.Crucifix));
            Assert.AreEqual(3, vanHelsing.NumberOfKnownItemsOfType(Item.Crucifix));
            Assert.AreEqual(3, vanHelsing.ItemsKnownToDracula.Count());
        }

        [Test]
        public void NumberOfKnownItemsOfType_CrucifixZero_Returns0()
        {
            vanHelsing.ItemsKnownToDracula.Clear();
            Assert.AreEqual(0, vanHelsing.NumberOfKnownItemsOfType(Item.Crucifix));
        }

        [Test]
        public void LikelihoodOfHavingItemOfType_NoItems_Returns0()
        {
            GameState game = new GameState();
            Assert.AreEqual(0, vanHelsing.LikelihoodOfHavingItemOfType(game, Item.Crucifix));
        }

        [Test]
        public void LikelihoodOfHavingItemOfType_1ItemKnown_Returns1()
        {
            GameState game = new GameState();
            ItemCard knife = game.ItemDeck.Find(card => card.Item == Item.Knife);
            vanHelsing.DrawItemCard();
            vanHelsing.ItemsKnownToDracula.Add(knife);
            game.ItemDeck.Remove(knife);
            Assert.AreEqual(1, vanHelsing.LikelihoodOfHavingItemOfType(game, Item.Knife));
        }

        [Test]
        public void LikelihoodOfHavingItemOfType_1ItemKnown1Unknown_Returns1()
        {
            GameState game = new GameState();
            ItemCard knife = game.ItemDeck.Find(card => card.Item == Item.Knife);
            vanHelsing.ItemsKnownToDracula.Add(knife);
            vanHelsing.DrawItemCard();
            vanHelsing.DrawItemCard();
            game.ItemDeck.Remove(knife);
            Assert.AreEqual(1, vanHelsing.LikelihoodOfHavingItemOfType(game, Item.Knife));
        }

        [Test]
        public void LikelihoodOfHavingItemOfType_1UnknownItem_Returns5OutOf40()
        {
            GameState game = new GameState();
            vanHelsing.DrawItemCard();
            Assert.AreEqual( 5F / 40F, vanHelsing.LikelihoodOfHavingItemOfType(game, Item.Knife));
        }

        [Test]
        public void LikelihoodOfHavingItemOfType_1UnknownItemIKnownItemDifferentType_Returns3OutOf39()
        {
            GameState game = new GameState();
            vanHelsing.DrawItemCard();
            vanHelsing.DrawItemCard();
            ItemCard knife = game.ItemDeck.Find(card => card.Item == Item.Knife);
            vanHelsing.ItemsKnownToDracula.Add(knife);
            Assert.AreEqual(3F / 40F, vanHelsing.LikelihoodOfHavingItemOfType(game, Item.Crucifix));
        }
    }
}