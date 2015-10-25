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
        public void ChooseDestination_HamburgWithPowersUsed_ReturnsLocationConnectedToHamburg()
        {
            game.Dracula.MoveTo(Location.Hamburg, Power.None);
            game.Dracula.Trail[1] = new DraculaCardSlot(new DraculaCard("HID", Location.Nowhere, Power.Hide, ConsoleColor.DarkGreen));
            game.Dracula.Trail[2] = new DraculaCardSlot(new DraculaCard("FEE", Location.Nowhere, Power.Feed, ConsoleColor.DarkGreen));
            game.Dracula.Trail[3] = new DraculaCardSlot(new DraculaCard("DAR", Location.Nowhere, Power.DarkCall, ConsoleColor.DarkGreen));
            game.Dracula.Trail[4] = new DraculaCardSlot(new DraculaCard("DOU", Location.Nowhere, Power.DoubleBack, ConsoleColor.DarkGreen));
            game.Dracula.Trail[5] = new DraculaCardSlot(new DraculaCard("WOL", Location.Nowhere, Power.WolfForm, ConsoleColor.DarkGreen));
            List<Location> validLocations = new List<Location>() { Location.NorthSea, Location.Cologne, Location.Leipzig, Location.Berlin };
            Power power;
            Assert.AreEqual(true, validLocations.Contains(logic.ChooseDestinationAndPower(game, out power)));
        }

        [Test]
        public void ChooseStartLocation_ReturnsSmallCityOrLargeCity()
        {
            Location startLocation = logic.ChooseStartLocation(game);
            Assert.AreEqual(true, game.Map.TypeOfLocation(startLocation) == LocationType.SmallCity || game.Map.TypeOfLocation(startLocation) == LocationType.LargeCity);
        }

        [Test]
        public void ChooseEncounterToPlace_ReturnsAnEncounterInDraculasHand()
        {
            game.Dracula.MoveTo(Location.Toulouse, Power.None);
            game.Dracula.DrawEncounter(game.EncounterPool);
            Assert.AreEqual(game.Dracula.EncounterHand.First(), logic.ChooseEncounterTileToPlaceOnDraculaCardSlot(game, game.Dracula.Trail[0]));
        }

        [Test]
        public void MoveTo_WithFullTrail_ReturnsDraculaCardSlot()
        {
            game.Dracula.MoveTo(Location.Nuremburg, Power.None);
            game.Dracula.MoveTo(Location.Leipzig, Power.None);
            game.Dracula.MoveTo(Location.Frankfurt, Power.None);
            game.Dracula.MoveTo(Location.Cologne, Power.None);
            game.Dracula.MoveTo(Location.Brussels, Power.None);
            game.Dracula.MoveTo(Location.Paris, Power.None);
            DraculaCardSlot cardSlot = game.Dracula.MoveTo(Location.LeHavre, Power.None);
            Assert.AreEqual(Location.Nuremburg, cardSlot.DraculaCards[0].Location);
        }
    }
}
