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
    public class DraculaTests
    {
        Dracula dracula;
        [TestFixtureSetUp]
        public void BeforeAll()
        {
            dracula = new Dracula();
        }

        [Test]
        public void MoveTo_Szeged_CurrentLocationReturnsSzeged()
        {
            dracula.MoveTo(Location.Szeged, Power.None);
            Assert.AreEqual(Location.Szeged, dracula.CurrentLocation);
        }

        [Test]
        public void MoveTo_Vienna_Trail0LocationReturnsVienna()
        {
            dracula.MoveTo(Location.Vienna, Power.None);
            Assert.AreEqual(Location.Vienna, dracula.Trail[0].DraculaCards[0].Location);
        }

        [Test]
        public void RevealCardAtPosition0_CardIsRevealed()
        {
            dracula.MoveTo(Location.Belgrade, Power.None);
            dracula.RevealCardAtPosition(0);
            Assert.AreEqual(true, dracula.Trail[0].DraculaCards[0].IsRevealed);
        }

        [Test]
        public void MoveTo_SixTimes_CardDroppedOffTrailIsUnrevealed()
        {
            dracula.MoveTo(Location.Athens, Power.None);
            dracula.MoveTo(Location.Barcelona, Power.None);
            dracula.MoveTo(Location.Berlin, Power.None);
            dracula.MoveTo(Location.Bordeaux, Power.None);
            dracula.MoveTo(Location.Cadiz, Power.None);
            dracula.MoveTo(Location.Dublin, Power.None);
            dracula.MoveTo(Location.Frankfurt, Power.None);
            Assert.AreEqual(false, dracula.DraculaCardDeck.Find(card => card.Location == Location.Athens).IsRevealed);
        }

        [Test]
        public void DrawEncounter_ReducesListBy1AndIncreasedEncounterHandBy1()
        {
            List<EncounterTile> encounterPool = new List<EncounterTile> { new EncounterTile(Encounter.DesecratedSoil), new EncounterTile(Encounter.NewVampire) };
            dracula.DrawEncounter(encounterPool);
            Assert.AreEqual(1, encounterPool.Count());
            Assert.AreEqual(1, dracula.EncounterHand.Count());
        }

        [Test]
        public void PlaceEncounterTileOnCard_Peasants_RemovesPeasantsFromEncounterHandAndAddsToTrail()
        {
            dracula.MoveTo(Location.Nuremburg, Power.None);
            dracula.EncounterHand.Clear();
            dracula.DrawEncounter(new List<EncounterTile> { new EncounterTile(Encounter.Peasants) });
            int countBefore = dracula.EncounterHand.Count();
            dracula.PlaceEncounterTileOnCard(dracula.EncounterHand.First(), dracula.Trail[0]);
            Assert.AreEqual(countBefore - 1, dracula.EncounterHand.Count());
            Assert.AreEqual(Encounter.Peasants, dracula.Trail[0].EncounterTiles.First().Encounter);
        }

        [Test]
        public void PlaceEncounterTileOnCard_PerformedTwice_PutsEncounterInSecondEncounterSlot()
        {
            dracula.MoveTo(Location.Edinburgh, Power.None);
            List<EncounterTile> encounterTiles = new List<EncounterTile> { new EncounterTile(Encounter.Rats), new EncounterTile(Encounter.Peasants) };
            dracula.EncounterHand.Clear();
            dracula.DrawEncounter(encounterTiles);
            dracula.DrawEncounter(encounterTiles);
            dracula.PlaceEncounterTileOnCard(dracula.EncounterHand.Find(tile => tile.Encounter == Encounter.Rats), dracula.Trail[0]);
            dracula.PlaceEncounterTileOnCard(dracula.EncounterHand.Find(tile => tile.Encounter == Encounter.Peasants), dracula.Trail[0]);
            Assert.AreEqual(Encounter.Peasants, dracula.Trail[0].EncounterTiles[1].Encounter);
        }

        [Test]
        public void TakeEvent_ListOfEvents_ListGetsShorterAndEventHandGetsLonger()
        {
            List<EventCard> cards = new List<EventCard> { new EventCard(Event.DevilishPower, true, EventType.Keep) };
            int cardCountBefore = dracula.EventHand.Count();
            dracula.TakeEvent(cards);
            Assert.AreEqual(null, cards.Find(card => card.Event == Event.DevilishPower));
            Assert.AreNotEqual(null, dracula.EventHand.Find(card => card.Event == Event.DevilishPower));
            Assert.AreEqual(cardCountBefore + 1, dracula.EventHand.Count());
        }

        [Test]
        public void DiscardEvent_EventCard_EventHandGetsShorterAndDiscardGetsLonger()
        {
            List<EventCard> cards = new List<EventCard> { new EventCard(Event.DevilishPower, true, EventType.Keep) };
            List<EventCard> discard = new List<EventCard>();
            dracula.TakeEvent(cards);
            int cardCountBefore = dracula.EventHand.Count();
            dracula.DiscardEvent(Event.DevilishPower, discard);
            Assert.AreEqual(null, dracula.EventHand.Find(card => card.Event == Event.DevilishPower));
            Assert.AreNotEqual(null, discard.Find(card => card.Event == Event.DevilishPower));
            Assert.AreEqual(cardCountBefore - 1, dracula.EventHand.Count());
        }

        [Test]
        public void DiscardEncounterFromCardSlot_EncounterInList_EncounterIsMovedToEncounterPool()
        {
            List<EncounterTile> encounterPool = new List<EncounterTile>();
            dracula.Trail[0] = new DraculaCardSlot();
            EncounterTile batsTile = new EncounterTile(Encounter.Bats);
            dracula.Trail[0].EncounterTiles.Add(batsTile);
            dracula.DiscardEncounterTileFromCardSlot(batsTile, dracula.Trail[0], encounterPool);
            Assert.AreEqual(false, dracula.Trail[0].EncounterTiles.Contains(batsTile));
            Assert.AreEqual(true, encounterPool.Contains(batsTile));
        }

        [Test]
        public void DiscardHide_TrailHasHideInMiddle_TrailIsShortenedAndCompactedAndEncountersReturned()
        {
            dracula.Trail[0] = new DraculaCardSlot();
            dracula.Trail[1] = new DraculaCardSlot();
            dracula.Trail[2] = new DraculaCardSlot();
            dracula.Trail[3] = new DraculaCardSlot();
            dracula.Trail[4] = new DraculaCardSlot();
            dracula.Trail[5] = new DraculaCardSlot();
            dracula.Trail[0].DraculaCards.Add(dracula.DraculaCardDeck.Find(card => card.Location == Location.Bari));
            dracula.Trail[1].DraculaCards.Add(dracula.DraculaCardDeck.Find(card => card.Location == Location.Naples));
            dracula.Trail[2].DraculaCards.Add(dracula.DraculaCardDeck.Find(card => card.Location == Location.Rome));
            dracula.Trail[3].DraculaCards.Add(dracula.DraculaCardDeck.Find(card => card.Power == Power.Hide));
            dracula.Trail[3].EncounterTiles.Add(new EncounterTile(Encounter.Hoax));
            dracula.Trail[4].DraculaCards.Add(dracula.DraculaCardDeck.Find(card => card.Location == Location.Florence));
            dracula.Trail[5].DraculaCards.Add(dracula.DraculaCardDeck.Find(card => card.Location == Location.Milan));
            List<EncounterTile> discardedEncounters = dracula.DiscardHide();
            Assert.AreEqual(Location.Bari, dracula.Trail[0].DraculaCards[0].Location);
            Assert.AreEqual(Location.Naples, dracula.Trail[1].DraculaCards[0].Location);
            Assert.AreEqual(Location.Rome, dracula.Trail[2].DraculaCards[0].Location);
            Assert.AreEqual(Location.Florence, dracula.Trail[3].DraculaCards[0].Location);
            Assert.AreEqual(Location.Milan, dracula.Trail[4].DraculaCards[0].Location);
            Assert.AreEqual(null, dracula.Trail[5]);
            Assert.AreEqual(1, discardedEncounters.Count());
            Assert.AreNotEqual(null, discardedEncounters.Find(tile => tile.Encounter == Encounter.Hoax));
        }

        [Test]
        public void RevealCardInTrailWithLocation_TrailWithLocationInIt_RevealsCardAndReturnsCorrectInt()
        {
            dracula.Trail[0] = new DraculaCardSlot();
            dracula.Trail[1] = new DraculaCardSlot();
            dracula.Trail[2] = new DraculaCardSlot();
            dracula.Trail[0].DraculaCards.Add(dracula.DraculaCardDeck.Find(card => card.Location == Location.Saragossa));
            dracula.Trail[1].DraculaCards.Add(dracula.DraculaCardDeck.Find(card => card.Location == Location.Madrid));
            dracula.Trail[2].DraculaCards.Add(dracula.DraculaCardDeck.Find(card => card.Location == Location.Santander));
            dracula.Trail[1].EncounterTiles.Add(new EncounterTile(Encounter.MinionWithKnife));
            int output = dracula.RevealCardInTrailWithLocation(Location.Madrid);
            Assert.AreEqual(1, output);
            Assert.AreEqual(true, dracula.Trail[1].DraculaCards.First().IsRevealed);
        }
    }
}
