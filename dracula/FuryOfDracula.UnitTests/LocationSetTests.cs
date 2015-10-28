using FuryOfDracula.GameLogic;
using NUnit.Framework;

namespace FuryOfDracula.UnitTests
{
    [TestFixture]
    public class LocationSetTests
    {
        private LocationSet map;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            map = new LocationSet();
        }

        [Test]
        public void IsEastern_Madrid_ReturnsFalse()
        {
            Assert.AreEqual(false, map.IsEastern(Location.Madrid));
        }

        [Test]
        public void LocationsConnectedByRoadOrSeaTo_Bordeaux_ReturnsCorrectList()
        {
            var locationsConnectedToFrankfurt = map.LocationsConnectedByRoadOrSeaTo(Location.Bordeaux);
            Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.Nantes));
            Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.ClermontFerrand));
            Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.Toulouse));
            Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.Saragossa));
            Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.BayOfBiscay));
            Assert.AreEqual(false, locationsConnectedToFrankfurt.Contains(Location.Paris));
        }

        [Test]
        public void TypeOfLocation_Milan_ReturnsLargeCity()
        {
            Assert.AreEqual(LocationType.LargeCity, map.TypeOfLocation(Location.Milan));
        }
    }
}