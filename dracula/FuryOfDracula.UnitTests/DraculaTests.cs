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
            dracula.MoveTo(Location.Szeged);
            Assert.AreEqual(Location.Szeged, dracula.CurrentLocation);
        }

        [Test]
        public void MoveTo_Vienna_Trail0LocationReturnsVienna()
        {
            dracula.MoveTo(Location.Vienna);
            Assert.AreEqual(Location.Vienna, dracula.Trail[0].DraculaCards[0].Location);
        }

        [Test]
        public void RevealCardAtPosition0_CardIsRevealed()
        {
            dracula.MoveTo(Location.Belgrade);
            dracula.RevealCardAtPosition(0);
            Assert.AreEqual(true, dracula.Trail[0].DraculaCards[0].IsRevealed);
        }

        [Test]
        public void MoveTo_SixTimes_CardDroppedOffTrailIsUnrevealed()
        {
            dracula.MoveTo(Location.Athens);
            dracula.MoveTo(Location.Barcelona);
            dracula.MoveTo(Location.Berlin);
            dracula.MoveTo(Location.Bordeaux);
            dracula.MoveTo(Location.Cadiz);
            dracula.MoveTo(Location.Dublin);
            dracula.MoveTo(Location.Frankfurt);
            Assert.AreEqual(false, dracula.DraculaCardDeck.GetDraculaCardForLocation(Location.Athens).IsRevealed);
        }
    }
}
