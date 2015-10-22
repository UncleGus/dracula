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
    public class GameStateTests
    {
        public GameState game;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            game = new GameState();
        }

        [Test]
        public void GameState_MapInitialisation()
        {
            Assert.AreEqual(Location.Paris, game.Map.GetLocationFromString("Par"));
        }

        [Test]
        public void GameState_ItemsInitialisation()
        {
            Assert.AreEqual(3, game.Items.GetItemsFromString("Holy").Count());
        }

        [Test]
        public void GameState_EventsInitialisation()
        {
            Assert.AreEqual(2, game.Events.GetEventsFromString("Mystic").Count());
        }

        [Test]
        public void GameState_EncountersInitialisation()
        {
            Assert.AreEqual(3, game.Encounters.GetEncountersFromString("Dese").Count());
        }

        [Test]
        public void GameState_HuntersInitialisation()
        {
            Assert.AreEqual(3, game.Hunters[(int)Hunter.VanHelsing].BitesRequiredToKill);
        }

        [Test]
        public void GameState_DraculaInitialisation()
        {
            Assert.AreEqual(4, game.Dracula.EventHandSize);
        }

        [Test]
        public void GetHunterFromString_Van_ReturnsVanHelsing()
        {
            Assert.AreEqual(Hunter.VanHelsing, game.GetHunterFromString("van"));
        }

        [Test]
        public void GetHunterFromString_Bob_ReturnsNobody()
        {
            Assert.AreEqual(Hunter.Nobody, game.GetHunterFromString("Bob"));
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

    }
}
