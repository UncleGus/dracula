using FuryOfDracula.GameLogic;
using NUnit.Framework;
using System.Collections.Generic;

namespace FuryOfDracula.UnitTests
{
    [TestFixture]
    public class GameStateTests
    {
        public GameState game;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            game = new GameState();
        }

        [Test]
        public void GameState_DraculaInitialisation()
        {
            Assert.AreEqual(4, game.Dracula.EventHandSize);
        }

        [Test]
        public void GameState_HuntersInitialisation()
        {
            Assert.AreEqual(3, game.Hunters[(int) Hunter.VanHelsing].BitesRequiredToKill);
        }

        [Test]
        public void GetHunterFromInt_1_ReturnsLordGodalming()
        {
            Assert.AreEqual(Hunter.LordGodalming, game.GetHunterFromInt(1));
        }

        [Test]
        public void GetHunterFromInt_5_ReturnsNobody()
        {
            Assert.AreEqual(Hunter.Nobody, game.GetHunterFromInt(5));
        }

        [Test]
        public void GetHunterFromString_Bob_ReturnsNobody()
        {
            Assert.AreEqual(Hunter.Nobody, game.GetHunterFromString("Bob"));
        }

        [Test]
        public void GetHunterFromString_Van_ReturnsVanHelsing()
        {
            Assert.AreEqual(Hunter.VanHelsing, game.GetHunterFromString("van"));
        }

        [Test]
        public void DistanceByRoadOrSeaBetween_MadridMadrid_Returns0()
        {
            Assert.AreEqual(0, game.DistanceByRoadOrSeaBetween(Location.Madrid, Location.Madrid, true));
        }

        [Test]
        public void DistanceByRoadOrSeaBetween_MadridSantander_Returns1()
        {
            Assert.AreEqual(1, game.DistanceByRoadOrSeaBetween(Location.Madrid, Location.Santander, true));
        }

        [Test]
        public void DistanceByRoadOrSeaBetween_MadridBerlin_Returns5()
        {
            Assert.AreEqual(5, game.DistanceByRoadOrSeaBetween(Location.Madrid, Location.Berlin, true));
        }

        [Test]
        public void DistanceByRoadBetween_MadridBerlin_Returns8()
        {
            Assert.AreEqual(8, game.DistanceByRoadBetween(Location.Madrid, Location.Berlin, true));
        }

        [Test]
        public void DistanceByRoadBetween_MunichDublin_Returns99()
        {
            Assert.AreEqual(99, game.DistanceByRoadBetween(Location.Munich, Location.Dublin, true));
        }

        [Test]
        public void DistanceByTrainBetween_MunichDublin_Returns99()
        {
            Assert.AreEqual(99, game.DistanceByTrainBetween(Location.Munich, Location.Dublin, true));
        }

        [Test]
        public void DistanceByTrainBetween_LisbonSalonica_Returns16()
        {
            Assert.AreEqual(16, game.DistanceByTrainBetween(Location.Lisbon, Location.Salonica, true));
        }
    }
}