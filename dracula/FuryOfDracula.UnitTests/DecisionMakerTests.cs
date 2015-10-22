using FuryOfDracula.ArtificialIntelligence;
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
    public class DecisionMakerTests
    {
        GameState game;
        DecisionMaker logic;
        [TestFixtureSetUp]
        public void BeforeAll()
        {
            game = new GameState();
            logic = new DecisionMaker();
        }

        [Test]
        public void ChooseDestination_Hamburg_ReturnsLocationConnectedToHamburg()
        {
            game.Dracula.MoveTo(Location.Hamburg);
            List<Location> validLocations = new List<Location>() { Location.NorthSea, Location.Cologne, Location.Leipzig, Location.Berlin };
            Assert.AreEqual(true, validLocations.Contains(logic.ChooseDestination(game)));
        }

        [Test]
        public void ChooseStartLocation_ReturnsSmallCityOrLargeCity()
        {
            Location startLocation = logic.ChooseStartLocation(game);
            Assert.AreEqual(true, game.Map.TypeOfLocation(startLocation) == LocationType.SmallCity || game.Map.TypeOfLocation(startLocation) == LocationType.LargeCity);
        }
    }
}
