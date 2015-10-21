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
    public class DraculaCardSetTests
    {
        public DraculaCardSet draculaCardDeck;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            draculaCardDeck = new DraculaCardSet();
        }

        [Test]
        public void GetDraculaCardForLocation_Milan_ReturnsMilan()
        {
            Assert.AreEqual("MIL", draculaCardDeck.GetDraculaCardForLocation(Location.Milan).Abbreviation);
        }

        [Test]
        public void GetDraculaCardForPower_WolfForm_ReturnsWolfForm()
        {
            Assert.AreEqual("WOL", draculaCardDeck.GetDraculaCardForPower(Power.WolfForm).Abbreviation);
        }

        [Test]
        public void GetDraculaCardForLocation_Hospital_ReturnsNull()
        {
            Assert.AreEqual(null, draculaCardDeck.GetDraculaCardForLocation(Location.StJosephAndStMary));
        }
    }
}
