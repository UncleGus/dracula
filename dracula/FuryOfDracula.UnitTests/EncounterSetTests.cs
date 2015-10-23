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
    public class EncounterSetTests
    {
        public EncounterSet encounterDeck;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            encounterDeck = new EncounterSet();
        }

        [Test]
        public void GetEncountersFromString_NewVampire_ReturnsAllNewVampireEncounters()
        {
            List<Encounter> newVampireEncounters = encounterDeck.GetEncountersFromString("New");
            Assert.AreEqual(true, newVampireEncounters.Contains(Encounter.NewVampire1));
            Assert.AreEqual(true, newVampireEncounters.Contains(Encounter.NewVampire2));
            Assert.AreEqual(true, newVampireEncounters.Contains(Encounter.NewVampire3));
            Assert.AreEqual(true, newVampireEncounters.Contains(Encounter.NewVampire4));
            Assert.AreEqual(true, newVampireEncounters.Contains(Encounter.NewVampire5));
            Assert.AreEqual(true, newVampireEncounters.Contains(Encounter.NewVampire6));
            Assert.AreEqual(false, newVampireEncounters.Contains(Encounter.Hoax1));
        }

        [Test]
        public void GetEncountersFromString_MismatchingString_ReturnsEmptyList()
        {
            Assert.AreEqual(0, encounterDeck.GetEncountersFromString("ZZZZZ").Count());
        }

        [Test]
        public void GetEncountersFromString_AmbiguousString_ReturnsEmptyList()
        {
            Assert.AreEqual(0, encounterDeck.GetEncountersFromString("Minion").Count());
        }

        [Test]
        public void GetAllEncounters_ReturnsListOfCount45()
        {
            Assert.AreEqual(45, encounterDeck.GetAllEncounters().Count());
        }
    }
}
