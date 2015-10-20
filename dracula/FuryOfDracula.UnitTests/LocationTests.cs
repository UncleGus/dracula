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
    public class LocationTests
    {
        LocationSet map;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            map = new LocationSet();
        }

        [TestFixtureTearDown]
        public void AfterAll()
        {

        }

        [SetUp]
        public void BeforeEach()
        {

        }

        [TearDown]
        public void AfterEach()
        {

        }

        [Test]
        public void GetParisName()
        {
            NUnitFramework.Assert.AreEqual("Paris", Location.Paris.Name());
        }

        [Test]
        public void GetMilanType()
        {
            NUnitFramework.Assert.AreEqual(LocationType.LargeCity, map.TypeOfLocation(Location.Milan));
        }

        [Test]
        public void GetBrusselsFromString()
        {
            NUnitFramework.Assert.AreEqual(Location.Brussels, map.GetLocationFromString("Bru"));
        }

        [Test]
        public void GetNowhereFromAmbiguousString()
        {
            NUnitFramework.Assert.AreEqual(Location.Nowhere, map.GetLocationFromString("BAR"));
        }

        [Test]
        public void GetNowhereFromMismatchingString()
        {
            NUnitFramework.Assert.AreEqual(Location.Nowhere, map.GetLocationFromString("#"));
        }

        [Test]
        public void GetToulouseAbbreviation()
        {
            NUnitFramework.Assert.AreEqual("TOU", map.LocationAbbreviation(Location.Toulouse));
        }

        [Test]
        public void GetWesternFromMadrid()
        {
            NUnitFramework.Assert.AreEqual(false, map.IsEastern(Location.Madrid));
        }

        [Test]
        public void GetLocationsConnectedByRoadOrSeaToBordeaux()
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
