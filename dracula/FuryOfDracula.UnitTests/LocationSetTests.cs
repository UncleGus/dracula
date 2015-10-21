using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using NUnit.Core;
using FuryOfDracula.GameLogic;

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
            NUnitFramework.Assert.AreEqual(LocationType.LargeCity, map.TypeOfLocation(Location.Milan));
        }

        [Test]
        public void GetLocationFromString_Bru_ReturnsBrussels()
        {
            NUnitFramework.Assert.AreEqual(Location.Brussels, map.GetLocationFromString("Bru"));
        }

        [Test]
        public void GetLocationFromString_AmbiguousStringBar_ReturnsNowhere()
        {
            NUnitFramework.Assert.AreEqual(Location.Nowhere, map.GetLocationFromString("BAR"));
        }

        [Test]
        public void GetLocationFromString_MismatchingString_ReturnsNowhere()
        {
            NUnitFramework.Assert.AreEqual(Location.Nowhere, map.GetLocationFromString("#"));
        }

        [Test]
        public void LocationAbbreviation_Toulouse_ReturnsTOU()
        {
            NUnitFramework.Assert.AreEqual("TOU", map.LocationAbbreviation(Location.Toulouse));
        }

        [Test]
        public void IsEastern_Madrid_ReturnsFalse()
        {
            NUnitFramework.Assert.AreEqual(false, map.IsEastern(Location.Madrid));
        }

        [Test]
        public void LocationsConnectedByRoadOrSeaTo_Bordeaux_ReturnsCorrectList()
        {
            List<Location> locationsConnectedToFrankfurt = map.LocationsConnectedByRoadOrSeaTo(Location.Bordeaux);
            NUnitFramework.Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.Nantes));
            NUnitFramework.Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.ClermontFerrand));
            NUnitFramework.Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.Toulouse));
            NUnitFramework.Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.Saragossa));
            NUnitFramework.Assert.AreEqual(true, locationsConnectedToFrankfurt.Contains(Location.BayOfBiscay));
            NUnitFramework.Assert.AreEqual(false, locationsConnectedToFrankfurt.Contains(Location.Paris));
        }
    }
}
