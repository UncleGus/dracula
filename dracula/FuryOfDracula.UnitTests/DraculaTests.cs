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
            dracula.RevealCardAtPosition(new GameState(), 0);
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
            Assert.AreEqual(false, dracula.DraculaCardDeck.GetDraculaCardForLocation(Location.Athens).IsRevealed);
        }

        [Test]
        public void DrawEncounter_ReducesListBy1AndIncreasedEncounterHandBy1()
        {
            List<Encounter> encounterPool = new EncounterSet().GetAllEncounters();
            dracula.DrawEncounter(encounterPool);
            Assert.AreEqual(44, encounterPool.Count());
            Assert.AreEqual(1, dracula.EncounterHand.Count());
        }

        [Test]
        public void PlaceEncounterOnCard_Peasants_RemovesPeasantsFromEncounterHandAndAddsToTrail()
        {
            dracula.MoveTo(Location.Nuremburg, Power.None);
            dracula.DrawEncounter(new List<Encounter> { Encounter.Peasants1 });
            int countBefore = dracula.EncounterHand.Count();
            dracula.PlaceEncounterOnCard(Encounter.Peasants1, dracula.Trail[0]);
            Assert.AreEqual(countBefore - 1, dracula.EncounterHand.Count());
            Assert.AreEqual(Encounter.Peasants1, dracula.Trail[0].Encounters[0]);
        }

        [Test]
        public void PlaceEncounterOnCard_PerformedTwice_PutsEncounterInSecondEncounterSlot()
        {
            dracula.MoveTo(Location.Edinburgh, Power.None);
            List<Encounter> encounters = new List<Encounter> { Encounter.Rats1, Encounter.Rats2 };
            dracula.DrawEncounter(encounters);
            dracula.DrawEncounter(encounters);
            dracula.PlaceEncounterOnCard(Encounter.Rats1, dracula.Trail[0]);
            dracula.PlaceEncounterOnCard(Encounter.Rats2, dracula.Trail[0]);
            Assert.AreEqual(Encounter.Rats2, dracula.Trail[0].Encounters[1]);
        }

        [Test]
        public void TakeEvent_ListOfEvents_ListGetsShorterAndEventHandGetsLonger()
        {
            List<Event> cards = new List<Event> { Event.DevilishPower1 };
            int cardCountBefore = dracula.EventHand.Count();
            dracula.TakeEvent(cards, new EventSet());
            Assert.AreEqual(false, cards.Contains(Event.DevilishPower1));
            Assert.AreEqual(true, dracula.EventHand.Contains(Event.DevilishPower1));
            Assert.AreEqual(cardCountBefore + 1, dracula.EventHand.Count());
        }

        [Test]
        public void DiscardEvent_EventCard_EventHandGetsShorterAndDiscardGetsLonger()
        {
            List<Event> cards = new List<Event> { Event.DevilishPower2 };
            List<Event> discard = new List<Event>();
            dracula.TakeEvent(cards, new EventSet());
            int cardCountBefore = dracula.EventHand.Count();
            dracula.DiscardEvent(Event.DevilishPower2, discard);
            Assert.AreEqual(false, dracula.EventHand.Contains(Event.DevilishPower2));
            Assert.AreEqual(true, discard.Contains(Event.DevilishPower2));
            Assert.AreEqual(cardCountBefore - 1, dracula.EventHand.Count());
        }

        [Test]
        public void DiscardEncounterFromCardSlot_EncounterInSlot1_EncounterIsMovedToEncounterPool()
        {
            List<Encounter> encounterPool = new List<Encounter>();
            dracula.Trail[0] = new DraculaCardSlot();
            dracula.Trail[0].Encounters[0] = Encounter.Bats1;
            dracula.DiscardEncounterFromCardSlot(Encounter.Bats1, dracula.Trail[0], encounterPool);
            Assert.AreEqual(Encounter.None, dracula.Trail[0].Encounters[0]);
            Assert.AreEqual(true, encounterPool.Contains(Encounter.Bats1));
        }

        [Test]
        public void DiscardEncounterFromCardSlot_EncounterInSlot2_EncounterIsMovedToEncounterPool()
        {
            List<Encounter> encounterPool = new List<Encounter>();
            dracula.Trail[0] = new DraculaCardSlot();
            dracula.Trail[0].Encounters[1] = Encounter.Bats2;
            dracula.DiscardEncounterFromCardSlot(Encounter.Bats2, dracula.Trail[0], encounterPool);
            Assert.AreEqual(Encounter.None, dracula.Trail[0].Encounters[1]);
            Assert.AreEqual(true, encounterPool.Contains(Encounter.Bats2));
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
            dracula.Trail[0].DraculaCards[0] = dracula.DraculaCardDeck.GetDraculaCardForLocation(Location.Bari);
            dracula.Trail[1].DraculaCards[0] = dracula.DraculaCardDeck.GetDraculaCardForLocation(Location.Naples);
            dracula.Trail[2].DraculaCards[0] = dracula.DraculaCardDeck.GetDraculaCardForLocation(Location.Rome);
            dracula.Trail[3].DraculaCards[0] = dracula.DraculaCardDeck.GetDraculaCardForPower(Power.Hide);
            dracula.Trail[3].Encounters[0] = Encounter.Hoax1;
            dracula.Trail[4].DraculaCards[0] = dracula.DraculaCardDeck.GetDraculaCardForLocation(Location.Florence);
            dracula.Trail[5].DraculaCards[0] = dracula.DraculaCardDeck.GetDraculaCardForLocation(Location.Milan);
            List<Encounter> discardedEncounters = dracula.DiscardHide();
            Assert.AreEqual(dracula.DraculaCardDeck.GetDraculaCardForLocation(Location.Bari), dracula.Trail[0].DraculaCards[0]);
            Assert.AreEqual(dracula.DraculaCardDeck.GetDraculaCardForLocation(Location.Naples), dracula.Trail[1].DraculaCards[0]);
            Assert.AreEqual(dracula.DraculaCardDeck.GetDraculaCardForLocation(Location.Rome), dracula.Trail[2].DraculaCards[0]);
            Assert.AreEqual(dracula.DraculaCardDeck.GetDraculaCardForLocation(Location.Florence), dracula.Trail[3].DraculaCards[0]);
            Assert.AreEqual(dracula.DraculaCardDeck.GetDraculaCardForLocation(Location.Milan), dracula.Trail[4].DraculaCards[0]);
            Assert.AreEqual(null, dracula.Trail[5]);
            Assert.AreEqual(1, discardedEncounters.Count());
            Assert.AreEqual(true, discardedEncounters.Contains(Encounter.Hoax1));
        }
    }
}
