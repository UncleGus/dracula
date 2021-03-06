﻿using System;
using System.Collections.Generic;
using System.Linq;
using FuryOfDracula.ArtificialIntelligence;
using FuryOfDracula.GameLogic;
using NUnit.Framework;

namespace FuryOfDracula.UnitTests
{
    [TestFixture]
    public class DecisionMakerTests
    {
        private GameState game;
        private DecisionMaker logic;

        [SetUp]
        public void BeforeEach()
        {
            game = new GameState();
            logic = new DecisionMaker();
        }

        [Test]
        public void ChooseDestination_HamburgWithPowersUsed_ReturnsLocationConnectedToHamburg()
        {
            int rubbish = 0;
            game.Dracula.MoveTo(Location.Hamburg, Power.None, out rubbish);
            game.Dracula.Trail[1] =
                new DraculaCardSlot(new DraculaCard("HID", Location.Nowhere, Power.Hide, ConsoleColor.DarkGreen));
            game.Dracula.Trail[2] =
                new DraculaCardSlot(new DraculaCard("FEE", Location.Nowhere, Power.Feed, ConsoleColor.DarkGreen));
            game.Dracula.Trail[3] =
                new DraculaCardSlot(new DraculaCard("DAR", Location.Nowhere, Power.DarkCall, ConsoleColor.DarkGreen));
            game.Dracula.Trail[4] =
                new DraculaCardSlot(new DraculaCard("DOU", Location.Nowhere, Power.DoubleBack, ConsoleColor.DarkGreen));
            game.Dracula.Trail[5] =
                new DraculaCardSlot(new DraculaCard("WOL", Location.Nowhere, Power.WolfForm, ConsoleColor.DarkGreen));
            var validLocations = new List<Location>
            {
                Location.NorthSea,
                Location.Cologne,
                Location.Leipzig,
                Location.Berlin
            };
            Power power;
            Assert.AreEqual(true, validLocations.Contains(logic.ChooseDestinationAndPower(game, out power)));
        }

        [Test]
        public void ChooseEncounterToPlace_ReturnsAnEncounterInDraculasHand()
        {
            int rubbish = 0;
            game.Dracula.MoveTo(Location.Toulouse, Power.None, out rubbish);
            game.Dracula.DrawEncounter(game.EncounterPool);
            Assert.AreEqual(game.Dracula.EncounterHand.First(),
                logic.ChooseEncounterTileToPlaceOnDraculaCardSlot(game, game.Dracula.Trail[0]));
        }

        [Test]
        public void ChooseStartLocation_ReturnsSmallCityOrLargeCity()
        {
            var startLocation = logic.ChooseStartLocation(game);
            Assert.AreEqual(true,
                game.Map.TypeOfLocation(startLocation) == LocationType.SmallCity ||
                game.Map.TypeOfLocation(startLocation) == LocationType.LargeCity);
        }

        [Test]
        public void MoveTo_WithFullTrail_ReturnsDraculaCardSlot()
        {
            int rubbish = 0;
            game.Dracula.MoveTo(Location.Nuremburg, Power.None, out rubbish);
            game.Dracula.MoveTo(Location.Leipzig, Power.None, out rubbish);
            game.Dracula.MoveTo(Location.Frankfurt, Power.None, out rubbish);
            game.Dracula.MoveTo(Location.Cologne, Power.None, out rubbish);
            game.Dracula.MoveTo(Location.Brussels, Power.None, out rubbish);
            game.Dracula.MoveTo(Location.Paris, Power.None, out rubbish);
            var cardSlot = game.Dracula.MoveTo(Location.LeHavre, Power.None, out rubbish);
            Assert.AreEqual(Location.Nuremburg, cardSlot.DraculaCards[0].Location);
        }

        [Test]
        public void InitialisePossibilityTree_MadridHamburgParisZagreb_ListOfCorrectPossibleTrails()
        {
            game.Hunters[(int)Hunter.LordGodalming].MoveTo(Location.Madrid);
            game.Hunters[(int)Hunter.DrSeward].MoveTo(Location.Hamburg);
            game.Hunters[(int)Hunter.VanHelsing].MoveTo(Location.Paris);
            game.Hunters[(int)Hunter.MinaHarker].MoveTo(Location.Zagreb);
            logic.InitialisePossibilityTree(game);
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Madrid));
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Hamburg));
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Paris));
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Zagreb));
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.AtlanticOcean));
            Assert.AreNotEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Klausenburg));
        }

        [Test]
        public void AddOrangeBackedCardToAllPossibleTrails_InstantiatedTree_TreeWithNewLocationsAdd()
        {
            int rubbish = 0;
            game.Hunters[(int)Hunter.LordGodalming].MoveTo(Location.Madrid);
            game.Hunters[(int)Hunter.DrSeward].MoveTo(Location.Hamburg);
            game.Hunters[(int)Hunter.VanHelsing].MoveTo(Location.Paris);
            game.Hunters[(int)Hunter.MinaHarker].MoveTo(Location.Zagreb);
            game.Dracula.MoveTo(Location.Belgrade, Power.None, out rubbish);
            logic.InitialisePossibilityTree(game);
            game.Dracula.MoveTo(Location.Szeged, Power.None, out rubbish);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            Assert.AreNotEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Szeged && trail[1].Location == Location.Belgrade));
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Marseilles && trail[1].Location == Location.Belgrade));
        }

        [Test]
        public void AddBlueBackedCardToAllPossibleTrails_InstantiatedTree_TreeWithNewLocationsAdd()
        {
            game.Hunters[(int)Hunter.LordGodalming].MoveTo(Location.Madrid);
            game.Hunters[(int)Hunter.DrSeward].MoveTo(Location.Hamburg);
            game.Hunters[(int)Hunter.VanHelsing].MoveTo(Location.Paris);
            game.Hunters[(int)Hunter.MinaHarker].MoveTo(Location.Zagreb);
            int rubbish = 0;
            game.Dracula.MoveTo(Location.Belgrade, Power.None, out rubbish);
            logic.InitialisePossibilityTree(game);
            game.Dracula.MoveTo(Location.Szeged, Power.None, out rubbish);
            logic.AddBlueBackedCardToAllPossibleTrails(game);
            Assert.AreNotEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.MediterraneanSea && trail[1].Location == Location.Alicante));
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.AtlanticOcean && trail[1].Location == Location.Belgrade));
        }

        [Test]
        public void AddPowerCardToAllPossibleTrails_InstantiatedTree_TreeWithPowerAdded()
        {
            game.Hunters[(int)Hunter.LordGodalming].MoveTo(Location.Madrid);
            game.Hunters[(int)Hunter.DrSeward].MoveTo(Location.Hamburg);
            game.Hunters[(int)Hunter.VanHelsing].MoveTo(Location.Paris);
            game.Hunters[(int)Hunter.MinaHarker].MoveTo(Location.Zagreb);
            int rubbish = 0;
            game.Dracula.MoveTo(Location.Belgrade, Power.None, out rubbish);
            logic.InitialisePossibilityTree(game);
            game.Dracula.MoveTo(Location.Nowhere, Power.DarkCall, out rubbish);
            logic.AddPowerCardToAllPossibleTrails(game, Power.DarkCall);
            Assert.AreNotEqual(null, logic.PossibilityTree.Find(trail => trail[0].Power == Power.DarkCall && trail[1].Location == Location.Munich));
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[0].Power == Power.Feed && trail[1].Location == Location.Milan));
        }

        [Test]
        public void AddDoubleBackToAllPossibleTrails_TrailsThreeLong_ValidDoubleBackTrails()
        {
            game.Hunters[(int)Hunter.LordGodalming].MoveTo(Location.Madrid);
            game.Hunters[(int)Hunter.DrSeward].MoveTo(Location.Hamburg);
            game.Hunters[(int)Hunter.VanHelsing].MoveTo(Location.Paris);
            game.Hunters[(int)Hunter.MinaHarker].MoveTo(Location.Zagreb);
            int doubleBackSlot = 0;
            game.Dracula.MoveTo(Location.Belgrade, Power.None, out doubleBackSlot);
            logic.InitialisePossibilityTree(game);
            game.Dracula.MoveTo(Location.Szeged, Power.None, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            game.Dracula.MoveTo(Location.Klausenburg, Power.None, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            Assert.AreNotEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Klausenburg && trail[1].Location == Location.Szeged && trail[2].Location == Location.Belgrade));
            game.Dracula.MoveTo(Location.Belgrade, Power.DoubleBack, out doubleBackSlot);
            logic.AddDoubleBackToAllPossibleTrails(game, doubleBackSlot);
            Assert.AreNotEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Belgrade && trail[0].Power == Power.DoubleBack && trail[1].Location == Location.Klausenburg && trail[2].Location == Location.Szeged));
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[0].Power == Power.Feed));
        }

        [Test]
        public void AddOrangeBackedCardToAllPossibleTrails_CadizIsBlocked_NoAlicanteToCadiz()
        {
            game.Hunters[(int)Hunter.LordGodalming].MoveTo(Location.Madrid);
            game.Hunters[(int)Hunter.DrSeward].MoveTo(Location.Hamburg);
            game.Hunters[(int)Hunter.VanHelsing].MoveTo(Location.Paris);
            game.Hunters[(int)Hunter.MinaHarker].MoveTo(Location.Zagreb);
            int doubleBackSlot = 0;
            game.Dracula.MoveTo(Location.Alicante, Power.None, out doubleBackSlot);
            logic.InitialisePossibilityTree(game);
            game.HeavenlyHostLocation1 = Location.Cadiz;
            game.Dracula.MoveTo(Location.Madrid, Power.None, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Cadiz));
        }

        [Test]
        public void AddWolfFormToAllPossibleTrails_CadizIsBlocked_NoWolfFormOnCadiz()
        {
            game.Hunters[(int)Hunter.LordGodalming].MoveTo(Location.Madrid);
            game.Hunters[(int)Hunter.DrSeward].MoveTo(Location.Hamburg);
            game.Hunters[(int)Hunter.VanHelsing].MoveTo(Location.Paris);
            game.Hunters[(int)Hunter.MinaHarker].MoveTo(Location.Zagreb);
            int doubleBackSlot = 0;
            game.Dracula.MoveTo(Location.Alicante, Power.None, out doubleBackSlot);
            logic.InitialisePossibilityTree(game);
            game.HeavenlyHostLocation1 = Location.Cadiz;
            game.Dracula.MoveTo(Location.Santander, Power.WolfForm, out doubleBackSlot);
            logic.AddWolfFormToAllPossibleTrails(game);
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Cadiz));
            Assert.AreNotEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Plymouth && trail[0].Power == Power.WolfForm && trail[1].Location == Location.Manchester));
        }

        [Test]
        public void TrimAllPossibleTrails_TrailsOfLength3TrimmedTo2_NoLength1or3Trails()
        {
            game.Hunters[(int)Hunter.LordGodalming].MoveTo(Location.Madrid);
            game.Hunters[(int)Hunter.DrSeward].MoveTo(Location.Hamburg);
            game.Hunters[(int)Hunter.VanHelsing].MoveTo(Location.Paris);
            game.Hunters[(int)Hunter.MinaHarker].MoveTo(Location.Zagreb);
            int doubleBackSlot = 0;
            game.Dracula.MoveTo(Location.Alicante, Power.None, out doubleBackSlot);
            logic.InitialisePossibilityTree(game);
            game.Dracula.MoveTo(Location.Cadiz, Power.None, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            game.Dracula.MoveTo(Location.Granada, Power.None, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            game.Dracula.ClearTrailDownTo(game, 2);
            logic.TrimAllPossibleTrails(2);
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[2] != null));
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[1] == null));
        }

        [Test]
        public void EliminateTrailsThatContainLocation_TrailLength2LocationNaples_NoTrailsContainingNaples()
        {
            game.Hunters[(int)Hunter.LordGodalming].MoveTo(Location.Madrid);
            game.Hunters[(int)Hunter.DrSeward].MoveTo(Location.Hamburg);
            game.Hunters[(int)Hunter.VanHelsing].MoveTo(Location.Paris);
            game.Hunters[(int)Hunter.MinaHarker].MoveTo(Location.Zagreb);
            int doubleBackSlot = 0;
            game.Dracula.MoveTo(Location.Berlin, Power.None, out doubleBackSlot);
            logic.InitialisePossibilityTree(game);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            logic.EliminateTrailsThatContainLocation(game, Location.Naples);
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Naples));
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[1].Location == Location.Naples));
        }

        [Test]
        public void EliminateTrailsThatDoNotContainLocationAtPosition_TrailLength2LocationRomePosition2_OnlyTrailsWithRomeInPosition2()
        {
            game.Hunters[(int)Hunter.LordGodalming].MoveTo(Location.Madrid);
            game.Hunters[(int)Hunter.DrSeward].MoveTo(Location.Hamburg);
            game.Hunters[(int)Hunter.VanHelsing].MoveTo(Location.Paris);
            game.Hunters[(int)Hunter.MinaHarker].MoveTo(Location.Zagreb);
            int doubleBackSlot = 0;
            game.Dracula.MoveTo(Location.Rome, Power.None, out doubleBackSlot);
            logic.InitialisePossibilityTree(game);
            game.Dracula.MoveTo(Location.Naples, Power.None, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            logic.EliminateTrailsThatDoNotContainLocationAtPosition(game, Location.Rome, 1);
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[1].Location != Location.Rome));
        }

        [Test]
        public void EliminateTrailsThatContainLocationAtPosition_Milan0_NoTrailsHaveZagrebAtPosition0()
        {
            game.Hunters[(int)Hunter.LordGodalming].MoveTo(Location.Madrid);
            game.Hunters[(int)Hunter.DrSeward].MoveTo(Location.Hamburg);
            game.Hunters[(int)Hunter.VanHelsing].MoveTo(Location.Paris);
            game.Hunters[(int)Hunter.MinaHarker].MoveTo(Location.Zagreb);
            int doubleBackSlot = 0;
            game.Dracula.MoveTo(Location.Rome, Power.None, out doubleBackSlot);
            logic.InitialisePossibilityTree(game);
            Assert.AreNotEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Milan));
            int possibleLocationsBefore = logic.NumberOfPossibleCurrentLocations;
            logic.EliminateTrailsThatContainLocationAtPosition(game, Location.Milan, 0);
            Assert.AreEqual(null, logic.PossibilityTree.Find(trail => trail[0].Location == Location.Milan));
            Assert.AreEqual(possibleLocationsBefore - 1, logic.NumberOfPossibleCurrentLocations);
        }

        [Test]
        public void GetActualTrail_TrailFiveCardsLong_CorrectTrail()
        {
            int doubleBackSlot;
            game.Dracula.MoveTo(Location.Genoa, Power.None, out doubleBackSlot);
            game.Dracula.MoveTo(Location.Milan, Power.None, out doubleBackSlot);
            game.Dracula.MoveTo(Location.Nowhere, Power.Hide, out doubleBackSlot);
            game.Dracula.MoveTo(Location.Zurich, Power.None, out doubleBackSlot);
            game.Dracula.MoveTo(Location.Strasbourg, Power.None, out doubleBackSlot);
            game.Dracula.Trail[5] = null;
            PossibleTrailSlot[] actualTrail = logic.GetActualTrail(game);
            Assert.AreEqual(Location.Strasbourg, actualTrail[0].Location);
            Assert.AreEqual(Location.Zurich, actualTrail[1].Location);
            Assert.AreEqual(Power.Hide, actualTrail[2].Power);
            Assert.AreEqual(Location.Milan, actualTrail[3].Location);
            Assert.AreEqual(Location.Genoa, actualTrail[4].Location);
            Assert.AreEqual(null, actualTrail[5]);
        }

        //[Test]
        //public void DetermineOrderedMoves_Marseilles_Returns9Moves()
        //{
        //    int doubleBackSlot;
        //    game.Dracula.Trail[5] = game.Dracula.Trail[4] = game.Dracula.Trail[3] = game.Dracula.Trail[2] = game.Dracula.Trail[1] = game.Dracula.Trail[1] = game.Dracula.Trail[0] = null;
        //    game.Dracula.MoveTo(Location.Marseilles, Power.None, out doubleBackSlot);
        //    PossibleTrailSlot[] actualTrail = logic.GetActualTrail(game);
        //    List<PossibleTrailSlot[]> possibilityTree = new List<PossibleTrailSlot[]>();
        //    possibilityTree.Add(actualTrail);
        //    List<int> numberOfPossibilities;
        //    List<PossibleTrailSlot> orderedMoves = logic.DetermineUniqueMovesOrderedByFreedomOfMovement(game, possibilityTree, 6, 0, out numberOfPossibilities);
        //    Assert.AreEqual(9, orderedMoves.Count());
        //}

        [Test]
        public void AddEscapeAsBatCardToAllTrails_TrailLength1_ReturnsCorrectCount()
        {
            int doubleBackSlot;
            game.Dracula.MoveTo(Location.Marseilles, Power.None, out doubleBackSlot);
            logic.InitialisePossibilityTree(game);
            logic.EliminateTrailsThatDoNotContainLocationAtPosition(game, Location.Marseilles, 0);
            PossibleTrailSlot[] actualTrail = logic.GetActualTrail(game);
            logic.AddEscapeAsBatCardToAllTrails(game);
            Assert.AreEqual(15, logic.PossibilityTree.Count());
        }

        [Test]
        public void AddDisembarkedCardToAllPossibleTrails_ThreeTrails_ReturnsCorrectCount()
        {
            int doubleBackSlot;
            game.Dracula.MoveTo(Location.Marseilles, Power.None, out doubleBackSlot);
            logic.InitialisePossibilityTree(game);
            logic.EliminateTrailsThatDoNotContainLocationAtPosition(game, Location.Marseilles, 0);
            logic.AddBlueBackedCardToAllPossibleTrails(game);
            logic.AddBlueBackedCardToAllPossibleTrails(game);
            logic.AddDisembarkedCardToAllPossibleTrails(game);
            Assert.AreEqual(7, logic.PossibilityTree.Count());
        }

        [Test]
        public void GetPossibleMovesFromTrail_MadridSaragossaToulouseClermontFerrandParis_ReturnsNantesLeHavreBrusselsStrasbourgGenevaHideFeedDarkCallWolfFormDoubleBack()
        {
            int doubleBackSlot = -1;
            game.Dracula.MoveTo(Location.Madrid, Power.None, out doubleBackSlot);
            game.Dracula.MoveTo(Location.Saragossa, Power.None, out doubleBackSlot);
            game.Dracula.MoveTo(Location.Toulouse, Power.None, out doubleBackSlot);
            game.Dracula.MoveTo(Location.ClermontFerrand, Power.None, out doubleBackSlot);
            game.Dracula.MoveTo(Location.Paris, Power.None, out doubleBackSlot);
            while (game.TimeOfDay != TimeOfDay.Midnight)
            {
                game.AdvanceTimeTracker();
            }
            var possibleMoves = logic.GetPossibleMovesFromTrail(game, logic.GetActualTrail(game));
            Assert.AreEqual(22, possibleMoves.Count());
        }

        [Test]
        public void ChooseDestinationAndPower_TestingEvaluationOfCurrentLocationsWithPossibleMoves()
        {
            int doubleBackSlot = -1;
            game.AdvanceTimeTracker();
            game.Dracula.MoveTo(Location.Madrid, Power.None, out doubleBackSlot);
            logic.InitialisePossibilityTree(game);
            game.AdvanceTimeTracker();
            game.Dracula.MoveTo(Location.Saragossa, Power.None, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            game.AdvanceTimeTracker();
            game.Dracula.MoveTo(Location.Toulouse, Power.None, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            game.AdvanceTimeTracker();
            game.Dracula.MoveTo(Location.ClermontFerrand, Power.None, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            logic.EliminateTrailsThatDoNotContainLocationAtPosition(game, Location.ClermontFerrand, 0);
            game.AdvanceTimeTracker();
            game.Dracula.MoveTo(Location.Paris, Power.None, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            var possibleMoves = logic.GetPossibleMovesFromTrail(game, logic.GetActualTrail(game));
            var possibleCurrentOrangeBackedLocations = new List<Location>();
            foreach (var trail in logic.PossibilityTree)
            {
                possibleCurrentOrangeBackedLocations.AddRange(logic.GetPossibleCurrentLocationsFromPossibilityTree(logic.AddOrangeBackedCardToTrail(game, trail)));
            }
            var possibleCurrentBlueBackedLocations = new List<Location>();
            foreach (var trail in logic.PossibilityTree)
            {
                possibleCurrentBlueBackedLocations.AddRange(logic.GetPossibleCurrentLocationsFromPossibilityTree(logic.AddBlueBackedCardToTrail(game, trail)));
            }
            var uniquePossibleCurrentOrangeBackedLocations = new List<Location>();
            var uniquePossibleCurrentBlueBackedLocations = new List<Location>();
            foreach (var location in possibleCurrentOrangeBackedLocations)
            {
                if (!uniquePossibleCurrentOrangeBackedLocations.Contains(location))
                {
                    uniquePossibleCurrentOrangeBackedLocations.Add(location);
                }
            }
            foreach (var location in possibleCurrentBlueBackedLocations)
            {
                if (!uniquePossibleCurrentBlueBackedLocations.Contains(location))
                {
                    uniquePossibleCurrentBlueBackedLocations.Add(location);
                }
            }
            Assert.AreEqual(14, uniquePossibleCurrentOrangeBackedLocations.Count());
            Assert.AreEqual(2, uniquePossibleCurrentBlueBackedLocations.Count());
        }

        [Test]
        public void GetPossibleMovesThatLeadToDeadEnds_ReturnsUsefulList()
        {
            int doubleBackSlot = -1;
            game.AdvanceTimeTracker();
            game.Dracula.MoveTo(Location.MediterraneanSea, Power.None, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            game.AdvanceTimeTracker();
            game.Dracula.MoveTo(Location.AtlanticOcean, Power.None, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            game.AdvanceTimeTracker();
            game.Dracula.MoveTo(Location.MediterraneanSea, Power.DoubleBack, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            game.AdvanceTimeTracker();
            game.Dracula.MoveTo(Location.TyrrhenianSea, Power.None, out doubleBackSlot);
            logic.AddOrangeBackedCardToAllPossibleTrails(game);
            var actualTrail = logic.GetActualTrail(game);
            var possibleMoves = logic.GetPossibleMovesFromTrail(game, logic.GetActualTrail(game));
            var numberOfMovesBeforeDeadEnd = new List<int>();
            Assert.AreEqual(1, logic.GetPossibleMovesThatLeadToDeadEnds(game, actualTrail, possibleMoves, numberOfMovesBeforeDeadEnd).Count(move => move.Location == Location.Cagliari));
        }
    }
}