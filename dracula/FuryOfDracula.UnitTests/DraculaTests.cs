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
    }
}
