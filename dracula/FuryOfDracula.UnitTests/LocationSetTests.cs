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
    public class LocationSetTests
    {
        LocationSet map;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            map = new LocationSet();
        }

        [Test]
        public void TypeOfLocation_Milan_ReturnsLargeCity()
        {
            Assert.AreEqual(LocationType.LargeCity, map.TypeOfLocation(Location.Milan));
        }

        [Test]
        public void GetLocationFromString_Bru_ReturnsBrussels()
        {
            Assert.AreEqual(Location.Brussels, map.GetLocationFromString("Bru"));
        }

        [Test]
        public void GetLocationFromString_AmbiguousStringBar_ReturnsNowhere()
        {
            Assert.AreEqual(Location.Nowhere, map.GetLocationFromString("BAR"));
        }

        [Test]
        public void GetLocationFromString_MismatchingString_ReturnsNowhere()
        {
            Assert.AreEqual(Location.Nowhere, map.GetLocationFromString("#"));
        }

        [Test]
        public void IsEastern_Madrid_ReturnsFalse()
        {
            Assert.AreEqual(false, map.IsEastern(Location.Madrid));
        }

        [Test]
        public void LocationsConnectedByRoadOrSeaTo_Bordeaux_ReturnsCorrectList()
        {
            List<Location> locationsConnectedToFrankfurt = map.LocationsConnectedByRoadOrSeaTo(Location.Bordeaux);
            Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.Nantes));
            Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.ClermontFerrand));
            Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.Toulouse));
            Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.Saragossa));
            Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.BayOfBiscay));
            Assert.AreEqual(false, locationsConnectedToFrankfurt.Contains(Location.Paris));
        }

        [Test]
        public void GetAllLocations_ReturnsListOfCount72()
        {
            Assert.AreEqual(72, map.GetAllLocations().Count());
        }
    }
}
