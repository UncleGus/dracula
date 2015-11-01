using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using FuryOfDracula.ArtificialIntelligence;
using FuryOfDracula.GameLogic;

namespace FuryOfDracula.ConsoleInterface
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var game = new GameState();
            var logic = new DecisionMaker();

            if (args.Length > 0)
            {
                try
                {
                    game = new FileLoader().LoadGameState(args[0]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Environment.Exit(10);
                }
            }
            else
            {
                for (var i = 1; i < 5; i++)
                {
                    Console.WriteLine("Where is {0} starting?", game.Hunters[i].Hunter.Name());
                    var destination = Location.Nowhere;
                    var line = "";
                    while (destination == Location.Nowhere ||
                           (game.Map.TypeOfLocation(destination) != LocationType.SmallCity &&
                            game.Map.TypeOfLocation(destination) != LocationType.LargeCity))
                    {
                        line = Console.ReadLine();
                        destination = Enumerations.GetLocationFromString(line);
                        Console.WriteLine(destination.Name());
                        if (game.Map.TypeOfLocation(destination) != LocationType.SmallCity &&
                            game.Map.TypeOfLocation(destination) != LocationType.LargeCity)
                        {
                            Console.WriteLine("You must start in a city");
                        }
                    }
                    game.Hunters[i].MoveTo(destination);
                }
                int rubbish = -1;
                game.Dracula.MoveTo(logic.ChooseStartLocation(game), Power.None, out rubbish);
                while (game.Dracula.EncounterHand.Count() < game.Dracula.EncounterHandSize)
                {
                    game.Dracula.DrawEncounter(game.EncounterPool);
                }
                EndHunterTurn(game, logic);
            }

            var commandSet = new CommandSet();

            do
            {
                DrawGameState(game);
                commandSet = GetCommandSet();

                switch (commandSet.command)
                {
                    case "w":
                    case "water": UseHolyWaterAtHospital(game, commandSet.argument1); break;
                    case "h":
                    case "help":
                        DisplayHelp(); break;
                    case "f":
                    case "front":
                        HandleEncountersInFrontOfHunter(game, commandSet.argument1, logic); break;
                    case "y":
                    case "cycle":
                        CycleDeck(game, commandSet.argument1); break;
                    case "s":
                    case "resolve":
                        SpendResolve(game, commandSet.argument1, commandSet.argument2, logic); break;
                    case "c":
                    case "catch":
                        CatchTrain(game, commandSet.argument1, logic); break;
                    case "e":
                    case "event":
                        PlayEvent(game, commandSet.argument1, commandSet.argument2, logic); break;
                    case "u":
                    case "use":
                        UseItem(game, commandSet.argument1, commandSet.argument2); break;
                    case "g":
                    case "group":
                        SetupGroup(game, commandSet.argument1); break;
                    case "t":
                    case "take":
                        var eventPlayedByDracula = game.Dracula.TakeEvent(game.EventDeck, game.EventDiscard);
                        PlayImmediatelyDraculaCard(game, eventPlayedByDracula, logic);
                        while (game.Dracula.EventHand.Count() > game.Dracula.EventHandSize)
                        {
                            Console.WriteLine("Dracula discarded {0}", game.Dracula.DiscardEvent(logic.ChooseEventToDiscard(game), game.EventDiscard).Name());
                        }; break;
                    case "reveal":
                        RevealCardInTrailAtPosition(game, commandSet.argument1); break;
                    case "z":
                    case "end":
                        EndHunterTurn(game, logic); break;
                    case "state":
                        DisplayState(game, commandSet.argument1); break;
                    case "a":
                    case "discard":
                        DiscardCard(game, commandSet.argument1, commandSet.argument2); break;
                    case "d":
                    case "draw":
                        DrawCard(game, commandSet.argument1, commandSet.argument2);
                        CheckForDiscardRequired(game);
                        CheckForCardsRevealedForBeingBitten(game); break;
                    case "save":
                        SaveGameState(game, commandSet.argument1); break;
                    case "load":
                        try
                        {
                            game = new FileLoader().LoadGameState(commandSet.argument1);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Environment.Exit(10);
                        } break;
                    case "m":
                    case "move":
                        HandleMoveOperation(game, commandSet.argument1, commandSet.argument2, logic); break;
                    case "exit":
                        Console.WriteLine("Fare well"); break;
                    default:
                        Console.WriteLine("I didn't understand that"); break;
                }
            } while (commandSet.command != "exit");
        }

        private static void UseHolyWaterAtHospital(GameState game, string hunterIndex)
        {
            var hunterUsingHolyWater = Hunter.Nobody;
            int index = -2;
            if (int.TryParse(hunterIndex, out index))
            {
                hunterUsingHolyWater = game.GetHunterFromInt(index);
            }
            var line = "";
            while (hunterUsingHolyWater == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is using the Holy Water font? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)",
                    (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward,
                    Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker,
                    Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (int.TryParse(line, out index))
                {
                    if (index < -1 || index > 4)
                    {
                        index = -2;
                    }
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    hunterUsingHolyWater = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterUsingHolyWater.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            UseHolyWaterOnHunter(game, hunterUsingHolyWater);
        }

        /// <summary>
        /// Displays help text to the user
        /// </summary>
        private static void DisplayHelp()
        {
            Console.WriteLine("Commands can be written as the full word or the letter in brackets:");
            Console.WriteLine("Catch: (C)atch a train");
            Console.WriteLine("Move: (M)ove Hunter");
            Console.WriteLine("Draw: (D)raw a card for a Hunter");
            Console.WriteLine("Take: Let Dracula (T)ake an Event card");
            Console.WriteLine("Discard: Disc(A)rd a card from a Hunter");
            Console.WriteLine("Event: Play an (E)vent card");
            Console.WriteLine("Use: (U)se an Item from a Hunter");
            Console.WriteLine("Group: Set up a Hunter (G)roup");
            Console.WriteLine("Resolve: (S)pend resolve");
            Console.WriteLine("Front: Resolve an Encounter in (F)ront of a Hunter");
            Console.WriteLine("Water: Use Holy (W)ater font at the Hospital");
            Console.WriteLine("Cycle: C(Y)cle a card discard back into a new deck");
            Console.WriteLine("State: Show the state of the game");
            Console.WriteLine("Save: Save the game to a file");
            Console.WriteLine("Load: Load the game from a file");
            Console.WriteLine("Help: Display this (H)elp text");
            Console.WriteLine("");
            Console.WriteLine("Commands that ask additional questions can also be answered in advance");
            Console.WriteLine("e.g. \"e news 2\" play Newspaper Reports from Dr. Seward");
            Console.WriteLine("When you need a space in an argument, use an underscore instead");
            Console.WriteLine("e.g. \"m le_ha 4\" to move Mina Harker to Le Havre");
        }

        /// <summary>
        /// Resolves EncounterTiles in front of a Hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterIndex">The string to be converted to a Hunter</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void HandleEncountersInFrontOfHunter(GameState game, string hunterIndex, DecisionMaker logic)
        {
            var hunterWithEncounter = Hunter.Nobody;
            int index = -2;
            if (int.TryParse(hunterIndex, out index))
            {
                hunterWithEncounter = game.GetHunterFromInt(index);
            }
            var line = "";
            while (hunterWithEncounter == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is resolving an Encounter in front of them? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)",
                    (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward,
                    Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker,
                    Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (int.TryParse(line, out index))
                {
                    if (index < -1 || index > 4)
                    {
                        index = -2;
                    }
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    hunterWithEncounter = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterWithEncounter.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            while (game.Hunters[(int)hunterWithEncounter].EncountersInFrontOfPlayer.Any())
            {
                switch (game.Hunters[(int)hunterWithEncounter].EncountersInFrontOfPlayer.First().Encounter)
                {
                    case Encounter.Fog:
                        game.EncounterPool.Add(game.Hunters[(int)hunterWithEncounter].EncountersInFrontOfPlayer.First());
                        game.Hunters[(int)hunterWithEncounter].EncountersInFrontOfPlayer.Remove(game.Hunters[(int)hunterWithEncounter].EncountersInFrontOfPlayer.First());
                        Console.WriteLine("Fog tile returned to the Encounter pool");
                        break;
                    case Encounter.Bats:
                        game.EncounterPool.Add(game.Hunters[(int)hunterWithEncounter].EncountersInFrontOfPlayer.First());
                        game.Hunters[(int)hunterWithEncounter].EncountersInFrontOfPlayer.Remove(game.Hunters[(int)hunterWithEncounter].EncountersInFrontOfPlayer.First());
                        Console.WriteLine("Dracula is controlling {0}'s movement", hunterWithEncounter.Name());
                        Location destination = logic.ChooseBatsDestination(game, hunterWithEncounter);
                        HandleMoveOperation(game, destination.Name(), ((int)hunterWithEncounter).ToString(), logic);
                        break;
                }
            }
        }

        /// <summary>
        /// Cycles a discard pile into a new deck
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="deckType">The string to be converted to the type of deck being cycled</param>
        private static void CycleDeck(GameState game, string deckType)
        {
            while (deckType.ToLower() != "cancel")
            {
                switch (deckType.ToLower())
                {
                    case "i":
                    case "item":
                        game.RecycleItemDiscard();
                        return;
                    case "e":
                    case "event":
                        game.RecycleEventDiscard();
                        return;
                    default:
                        Console.WriteLine("Which deck are you recycling? Item or Event? (cancel to cancel)");
                        deckType = Console.ReadLine();
                        break;
                }
            }
            Console.WriteLine("Cancelled");
        }

        /// <summary>
        /// For spending a Resolve point by a Hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="resolveName">A string to be converted to the ResolveAbility being used</param>
        /// <param name="hunterIndex">A string to be converted to the Hunter spending Resolve</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void SpendResolve(GameState game, string resolveName, string hunterIndex, DecisionMaker logic)
        {
            var hunterSpendingResolve = Hunter.Nobody;
            int index = -2;
            if (int.TryParse(hunterIndex, out index))
            {
                hunterSpendingResolve = game.GetHunterFromInt(index);
            }
            var line = "";
            while (hunterSpendingResolve == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is spending resolve? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)",
                    (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward,
                    Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker,
                    Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (int.TryParse(line, out index))
                {
                    if (index < -1 || index > 4)
                    {
                        index = -2;
                    }
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    hunterSpendingResolve = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterSpendingResolve.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            var ability = Enumerations.GetResolveAbilityFromString(resolveName);
            while (ability == ResolveAbility.None && line.ToLower() != "cancel")
            {
                Console.WriteLine("What resolve ability is {0} using?", hunterSpendingResolve.Name());
                line = Console.ReadLine();
                if (line.ToLower() == "cancel")
                {
                    Console.WriteLine("Cancelled");
                    return;
                }
                ability = Enumerations.GetResolveAbilityFromString(line);
            }
            switch (ability)
            {
                case ResolveAbility.NewspaperReports:
                    PlayNewsPaperReports(game, hunterSpendingResolve, logic);
                    break;
                case ResolveAbility.InnerStrength:
                    PlayInnerStrength(game, hunterSpendingResolve);
                    break;
                case ResolveAbility.SenseOfEmergency:
                    PlaySenseOfEmergency(game, hunterSpendingResolve, logic);
                    break;
            }
            game.AdjustResolve(-1);
        }

        /// <summary>
        /// Spends a point of Resolve using the Inner Strength ability
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterSpendingResolve">The Hunter spending the resolve and being healed</param>
        private static void PlayInnerStrength(GameState game, Hunter hunterSpendingResolve)
        {
            game.Hunters[(int)hunterSpendingResolve].AdjustHealth(4);
            Console.WriteLine("{0} now has {1} health", hunterSpendingResolve.Name(), game.Hunters[(int)hunterSpendingResolve].Health);
        }

        /// <summary>
        /// Checks if a card drawn by Dracula can be played immediately and resolves it if so
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="eventPlayedByDracula">The Event to assess</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayImmediatelyDraculaCard(GameState game, Event eventPlayedByDracula, DecisionMaker logic)
        {
            switch (eventPlayedByDracula)
            {
                case Event.NightVisit:
                    PlayNightVisit(game, logic); break;
                case Event.VampiricInfluence:
                    PlayVampiricInfluence(game, logic); break;
                case Event.Evasion:
                    PlayEvasion(game, logic); break;
                case Event.QuinceyPMorris:
                case Event.ImmanuelHildesheim:
                case Event.DraculasBrides:
                    PlayDraculaAlly(game, eventPlayedByDracula, logic); break;
                default:
                    Console.WriteLine("Dracula drew an Event card into his hand"); break;
            }
        }

        /// <summary>
        /// Resolves the Evasion Event card
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayEvasion(GameState game, DecisionMaker logic)
        {
            Console.WriteLine("Dracula played Evasion");
            if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.Evasion, Event.Evasion, logic) > 0)
            {
                Console.WriteLine("Evasion cancelled");
                return;
            }
            var destination = logic.ChooseWhereToEvadeTo(game);
            int doubleBackSlot = -1;
            var cardDroppedOffTrail = game.Dracula.MoveTo(destination, Power.None, out doubleBackSlot);
            if (doubleBackSlot > -1)
            {
                Console.WriteLine("Dracula Doubled Back to the location in slot {0}", doubleBackSlot);
            }
            if (cardDroppedOffTrail != null)
            {
                DealWithDroppedOffCardSlots(game, new List<DraculaCardSlot> { cardDroppedOffTrail }, logic);
            }
            if (game.Dracula.Trail[0].EncounterTiles.Count == 0)
            {
                game.Dracula.PlaceEncounterTileOnCard(
                    logic.ChooseEncounterTileToPlaceOnDraculaCardSlot(game, game.Dracula.Trail[0]),
                    game.Dracula.Trail[0]);
            }
        }

        /// <summary>
        /// Resolves the Vampiric Influence Event card
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayVampiricInfluence(GameState game, DecisionMaker logic)
        {
            Console.WriteLine("Dracula played Vampiric Influence");
            var bittenHunters = new List<HunterPlayer>();
            if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.VampiricInfluence, Event.VampiricInfluence, logic) >
                0)
            {
                Console.WriteLine("Vampiric Influence cancelled");
                return;
            }
            foreach (var h in game.Hunters)
            {
                if (h != null && h.BiteCount > 0)
                {
                    bittenHunters.Add(h);
                }
            }
            var victim = logic.ChooseVampiricInfluenceVictim(bittenHunters);
            Console.WriteLine(
                "Dracula is influencing {0}, who must show Dracula all of {1} cards and declare {1} next move", victim.Hunter.Name(),
                victim.Hunter == Hunter.MinaHarker ? "her" : "his");
            DraculaLooksAtAllHuntersItems(game, victim);
            DraculaLooksAtAllHuntersEvents(game, victim);
            HunterDeclaresNextMove(game, victim);
        }

        /// <summary>
        /// Handles the situation when a Hunter must reveal all Event cards to Dracula
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="victim">The Hunter revealing Event cards</param>
        private static void DraculaLooksAtAllHuntersEvents(GameState game, HunterPlayer victim)
        {
            game.EventDeck.AddRange(victim.EventsKnownToDracula);
            victim.EventsKnownToDracula.Clear();
            for (var i = 0; i < victim.EventCount; i++)
            {
                Console.WriteLine("What is the name of the Event being revealed?");
                var eventRevealed = Event.None;
                while (eventRevealed == Event.None)
                {
                    eventRevealed = Enumerations.GetEventFromString(Console.ReadLine());
                }
                AddEventCardToDraculaKnownCards(game, victim, eventRevealed);
            }
        }

        /// <summary>
        /// Adds an Event card to the list of cards known to Dracula
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="victim">The Hunter who has revealed an Event</param>
        /// <param name="eventRevealed">The Event revealed by the Hunter</param>
        private static void AddEventCardToDraculaKnownCards(GameState game, HunterPlayer victim, Event eventRevealed)
        {
            var eventCardRevealed = game.EventDeck.Find(card => card.Event == eventRevealed);
            victim.EventsKnownToDracula.Add(eventCardRevealed);
            game.EventDeck.Remove(eventCardRevealed);
        }

        /// <summary>
        /// Resolves the situation when Dracula plays an Ally card
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="eventPlayedByDracula">The Ally Event</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayDraculaAlly(GameState game, Event eventPlayedByDracula, DecisionMaker logic)
        {
            Console.WriteLine("Dracula drew ally {0}", eventPlayedByDracula.Name());
            Event allyToKeep;
            if (game.DraculaAlly == null)
            {
                allyToKeep = eventPlayedByDracula;
            }
            else
            {
                game.EventDiscard.Add(game.DraculaAlly);
                allyToKeep = logic.ChooseAllyToKeep(game.DraculaAlly.Event, eventPlayedByDracula);
            }
            game.DraculaAlly = game.EventDiscard.Find(card => card.Event == allyToKeep);
            game.EventDiscard.Remove(game.DraculaAlly);
            Console.WriteLine("{0} kept", allyToKeep);
            game.Dracula.EncounterHandSize = game.DraculaAlly.Event == Event.DraculasBrides ? 7 : 5;
            game.Dracula.EventHandSize = game.DraculaAlly.Event == Event.ImmanuelHildesheim ? 6 : 4;
        }

        /// <summary>
        /// Resolves the Night Visit Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayNightVisit(GameState game, DecisionMaker logic)
        {
            Console.WriteLine("Dracula is playing Night Visit");
            if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.NightVisit, Event.NightVisit, logic) > 0)
            {
                Console.WriteLine("Night Visit cancelled");
                return;
            }
            var bittenHunters = new List<HunterPlayer>();
            foreach (var h in game.Hunters)
            {
                if (h != null && h.BiteCount > 0)
                {
                    bittenHunters.Add(h);
                }
            }
            var victim = logic.ChooseNightVisitVictim(bittenHunters);
            Console.WriteLine("{0} is visited in the night and loses 2 health", victim.Hunter.Name());
            victim.AdjustHealth(-2);
            CheckForHunterDeath(game);
        }

        /// <summary>
        /// Checks if Dracula will play False Tip-off when a Hunter attempts to catch a train
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterIndex">A String to be converted into the Hunter catching the train</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void CatchTrain(GameState game, string hunterIndex, DecisionMaker logic)
        {
            var hunterCatchingTrain = Hunter.Nobody;
            int index;
            if (int.TryParse(hunterIndex, out index))
            {
                hunterCatchingTrain = game.GetHunterFromInt(index);
            }
            var line = "";
            while (hunterCatchingTrain == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is catching the train? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)",
                    (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward,
                    Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker,
                    Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (int.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    hunterCatchingTrain = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterCatchingTrain.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            if (DraculaIsPlayingFalseTipoffToDelayHunter(game, logic))
            {
                Console.WriteLine("{0} is delayed over papers", hunterCatchingTrain.Name());
            }
            else
            {
                Console.WriteLine("{0} may catch a train", hunterCatchingTrain.Name());
            }
        }

        /// <summary>
        /// Checks to see if Dracula is playing False Tip-off when a Hunter attempts to catch a train by rolling the die
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if Dracula successfully plays False Tip-off</returns>
        private static bool DraculaIsPlayingFalseTipoffToDelayHunter(GameState game, DecisionMaker logic)
        {
            if (logic.ChooseToDelayHunterWithFalseTipoff(game))
            {
                Console.WriteLine("Dracula is playing False Tip-off to delay the train");
                game.Dracula.DiscardEvent(Event.FalseTipoff, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.FalseTipoff, Event.FalseTipoff, logic) > 0 ||
                    HunterPlayingCharteredCarriageToCancelFalseTipOff(game, Event.FalseTipoff, Event.FalseTipoff, logic) > 0)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a Hunter is playing Chartered Carriage to cancel Dracula's False Tip-off
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="eventBeingPlayed">The current Event being played</param>
        /// <param name="eventInitiallyPlayed">The initial Event played at the start of this exchange</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>The int corresponding to the Hunter playing Chartered Carriage, 0 if none</returns>
        private static int HunterPlayingCharteredCarriageToCancelFalseTipOff(GameState game, Event eventBeingPlayed, Event eventInitiallyPlayed, DecisionMaker logic)
        {
            Console.WriteLine("Will anyone play Chartered Carriage to cancel False Tip-off? 0= Nobody, {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7}",
    (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(),
    (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(),
    (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
            var answer = -1;
            while (answer == -1)
            {
                if (int.TryParse(Console.ReadLine(), out answer))
                {
                    if (answer < 0 || answer > 4)
                    {
                        answer = -1;
                    }
                }
            }
            if (answer > 0)
            {
                game.Hunters[answer].DiscardEvent(game, Event.CharteredCarriage);
                if (DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.CharteredCarriage, eventInitiallyPlayed, logic))
                {
                    game.Dracula.DiscardEvent(Event.DevilishPower, game.EventDiscard);
                    return 0;
                }
            }
            return answer;
        }

        /// <summary>
        /// Resolves an Event card played by a Hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="eventName">A string to be converted to the Event being played</param>
        /// <param name="hunterIndex">A string to be converted to the Hunter playing the Event</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayEvent(GameState game, string eventName, string hunterIndex, DecisionMaker logic)
        {
            var index = 0;
            var hunterPlayingEvent = Hunter.Nobody;
            if (int.TryParse(hunterIndex, out index))
            {
                hunterPlayingEvent = game.GetHunterFromInt(index);
            }
            var line = "";
            while (hunterPlayingEvent == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is playing an Event? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)",
                    (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward,
                    Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker,
                    Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (int.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    hunterPlayingEvent = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterPlayingEvent.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            var eventBeingPlayed = Enumerations.GetEventFromString(eventName);
            while (eventBeingPlayed == Event.None && line.ToLower() != "cancel")
            {
                Console.WriteLine("What is the name of the Event being played?");
                line = Console.ReadLine();
                eventBeingPlayed = Enumerations.GetEventFromString(line);
                Console.WriteLine(eventBeingPlayed.Name());
            }
            if (line.ToLower() == "cancel")
            {
                Console.WriteLine("Cancelled");
                return;
            }
            game.Hunters[(int)hunterPlayingEvent].DiscardEvent(game, eventBeingPlayed);
            if (DraculaIsPlayingDevilishPowerToCancelEvent(game, eventBeingPlayed, eventBeingPlayed, logic))
            {
                Console.WriteLine("{0} cancelled", eventBeingPlayed.Name());
                return;
            }
            switch (eventBeingPlayed)
            {
                case Event.JonathanHarker:
                case Event.RufusSmith:
                case Event.SisterAgatha:
                    PlayHunterAlly(game, hunterPlayingEvent, eventBeingPlayed); break;
                case Event.BloodTransfusion:
                    PlayBloodTransfusion(game, hunterPlayingEvent); break;
                case Event.CharteredCarriage:
                    PlayCharteredCarriage(game, hunterPlayingEvent, logic); break;
                case Event.ConsecratedGround:
                    PlayConsecratedGround(game, hunterPlayingEvent); break;
                case Event.ExcellentWeather:
                    PlayExcellentWeather(game, hunterPlayingEvent); break;
                case Event.GoodLuck:
                    PlayGoodLuck(game, hunterPlayingEvent); break;
                case Event.HiredScouts:
                    PlayHiredScouts(game, hunterPlayingEvent, logic); break;
                case Event.Hypnosis:
                    PlayHypnosis(game, hunterPlayingEvent, logic); break;
                case Event.LongDay:
                    PlayLongDay(game, hunterPlayingEvent); break;
                case Event.MoneyTrail:
                    PlayMoneyTrail(game, hunterPlayingEvent, logic); break;
                case Event.MysticResearch:
                    PlayMysticResearch(game, hunterPlayingEvent); break;
                case Event.NewspaperReports:
                    PlayNewsPaperReports(game, hunterPlayingEvent, logic); break;
                case Event.ReEquip:
                    PlayReEquip(game, hunterPlayingEvent); break;
                case Event.SenseofEmergency:
                    PlaySenseOfEmergency(game, hunterPlayingEvent, logic); break;
                case Event.StormySeas:
                    PlayStormySeas(game, hunterPlayingEvent, logic); break;
                case Event.SurprisingReturn:
                    PlaySurprisingReturn(game, hunterPlayingEvent); break;
                case Event.TelegraphAhead:
                    PlayTelegraphAhead(game, hunterPlayingEvent, logic); break;
                case Event.VampireLair:
                    PlayVampireLair(game, hunterPlayingEvent, logic); break;
                default:
                    Console.WriteLine("It is not appropriate to play {0} at this time", eventBeingPlayed.Name()); break;
            }
        }

        /// <summary>
        /// Resolves the Vampire Lair Event card
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayVampireLair(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            if (game.Vampires < 1)
            {
                Console.WriteLine("No Vampires to battle");
                return;
            }
            if (ResolveCombat(game, new List<HunterPlayer> { game.Hunters[(int)hunterPlayingEvent] }, Opponent.NewVampire,
                logic))
            {
                game.AdjustVampires(-1);
            }
        }

        /// <summary>
        /// Resolves the Telegraph Ahead Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayTelegraphAhead(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            foreach (var loc in game.Map.LocationsConnectedByRoadTo(game.Hunters[(int)hunterPlayingEvent].CurrentLocation))
            {
                if (DraculaIsPlayingSensationalistPressToPreventRevealingLocation(game, loc, logic))
                {
                    Console.WriteLine("Dracula prevented a location from being revealed");
                }
                else
                {
                    game.Dracula.RevealCardInTrailWithLocation(loc);
                }
            }
        }

        /// <summary>
        /// Checks if Dracula is playing Sensationalist Press to prevent a Location card from being revealed
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="location">The Location being revealed</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if Dracula successfully plays Sensationalist Press</returns>
        private static bool DraculaIsPlayingSensationalistPressToPreventRevealingLocation(GameState game,
            Location location, DecisionMaker logic)
        {
            if (logic.ChooseToPlaySensationalistPress(game, location))
            {
                Console.WriteLine("Dracula is playing Sensationalist Press");
                game.Dracula.DiscardEvent(Event.SensationalistPress, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.SensationalistPress, Event.SensationalistPress, logic) > 0)
                {
                    Console.WriteLine("Sensationalist Press cancelled");
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resolves the Surprising Return Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        private static void PlaySurprisingReturn(GameState game, Hunter hunterPlayingEvent)
        {
            if (game.EventDiscard.Count == 0)
            {
                return;
            }
            Console.WriteLine("What is the name of the card you are taking from the Event discard pile?");
            var eventRetrieved = Event.None;
            string line = "";
            while (eventRetrieved == Event.None && line.ToLower() != "nothing")
            {
                eventRetrieved = Enumerations.GetEventFromString(Console.ReadLine());
            }
            if (line.ToLower() == "nothing")
            {
                Console.WriteLine("Event discarded");
                return;
            }
            game.Hunters[(int)hunterPlayingEvent].DrawEventCard();
            var eventCardRetrieved = game.EventDiscard.Find(card => card.Event == eventRetrieved);
            game.Hunters[(int)hunterPlayingEvent].EventsKnownToDracula.Add(eventCardRetrieved);
            game.EventDiscard.Remove(eventCardRetrieved);
        }

        /// <summary>
        /// Resolves the Stormy Seas Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayStormySeas(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            Console.WriteLine("Where is {0} playing Stormy Seas?", hunterPlayingEvent.Name());
            var stormySeasLocation = Location.Nowhere;
            while (stormySeasLocation == Location.Nowhere ||
                   game.Map.TypeOfLocation(stormySeasLocation) != LocationType.Sea)
            {
                stormySeasLocation = Enumerations.GetLocationFromString(Console.ReadLine());
            }
            game.StormySeasLocation = stormySeasLocation;
            game.StormySeasRounds = 2;
            if (stormySeasLocation == game.Dracula.CurrentLocation)
            {
                Console.WriteLine("Dracula was in {0}", stormySeasLocation.Name());
                game.Dracula.RevealCardInTrailWithLocation(stormySeasLocation);
                var destination = logic.ChoosePortToGoToAfterStormySeas(game);
                if (destination == Location.Nowhere)
                {
                    Console.WriteLine("Dracula cannot make a legal move");
                    game.Dracula.TakePunishmentForCheating(game);
                    return;
                }
                int doubleBackSlot;
                var cardDroppedOffTrail = game.Dracula.MoveTo(destination, Power.None, out doubleBackSlot);
                if (doubleBackSlot > -1)
                {
                    Console.WriteLine("Dracula Doubled Back to the location in slot {0}", doubleBackSlot);
                }
                var cardsDroppedOffTrail = new List<DraculaCardSlot>();
                if (cardDroppedOffTrail != null)
                {
                    cardsDroppedOffTrail.Add(cardDroppedOffTrail);
                }
                if (DraculaIsPlayingSensationalistPressToPreventRevealingLocation(game, destination, logic))
                {
                    Console.WriteLine("Dracula prevented a location from being revealed");
                }
                else
                {
                    game.Dracula.RevealCardInTrailWithLocation(destination);
                }
                DealWithDroppedOffCardSlots(game, cardsDroppedOffTrail, logic);
                CheckForJonathanHarker(game);
            }
        }

        /// <summary>Checks if the sixth card in Dracula's Trail should be revealed by Jonathan Harker</summary>
        /// <param name="game">The GameState</param>
        private static void CheckForJonathanHarker(GameState game)
        {
            if (game.HunterAlly != null && game.HunterAlly.Event == Event.JonathanHarker && game.Dracula.Trail[5] != null)
            {
                game.Dracula.RevealCardAtPosition(5);
            }
        }

        /// <summary>
        /// Handles cards dropped off the end of Dracula's Trails
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="cardsDroppedOffTrail">A List of cards that have dropped off the end of the Trail</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void DealWithDroppedOffCardSlots(GameState game, List<DraculaCardSlot> cardsDroppedOffTrail,
            DecisionMaker logic)
        {
            foreach (var cardDroppedOffTrail in cardsDroppedOffTrail)
            {
                if (cardDroppedOffTrail.DraculaCards.Count() > 1)
                {
                    cardDroppedOffTrail.DraculaCards.Remove(cardDroppedOffTrail.DraculaCards[1]);
                }
                var index = logic.ChooseToPutDroppedOffCardInCatacombs(game, cardDroppedOffTrail);
                if (index > -1)
                {
                    game.Dracula.Catacombs[index] = cardDroppedOffTrail;
                    game.Dracula.PlaceEncounterTileOnCard(
                        logic.ChooseEncounterTileToPlaceOnDraculaCardSlot(game, game.Dracula.Catacombs[index]),
                        cardDroppedOffTrail);
                }
                else
                {
                    cardDroppedOffTrail.DraculaCards.First().IsRevealed = false;
                    while (cardDroppedOffTrail.EncounterTiles.Count() > 0)
                    {
                        cardDroppedOffTrail.EncounterTiles.First().IsRevealed = false;
                        MatureEncounter(game, cardDroppedOffTrail.EncounterTiles.First(), logic);
                        cardDroppedOffTrail.EncounterTiles.Remove(cardDroppedOffTrail.EncounterTiles.First());
                    }
                }
                if (cardDroppedOffTrail.DraculaCards.First().Location == game.Dracula.LocationWhereHideWasUsed &&
                    game.Dracula.LocationWhereHideWasUsed != Location.Nowhere)
                {
                    var encountersToReturnToEncounterPool = game.Dracula.DiscardHide();
                    foreach (var enc in encountersToReturnToEncounterPool)
                    {
                        enc.IsRevealed = false;
                        game.EncounterPool.Add(enc);
                    }
                    Console.WriteLine("The location where Dracula used Hide dropped off the trail, so the Hide card is also removed from the trail");
                }
            }
        }

        /// <summary>
        /// Handles the Sense of Emergency card or Resolve ability
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlaySenseOfEmergency(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            if (game.Hunters[(int)hunterPlayingEvent].Health <= 6 - game.Vampires)
            {
                Console.WriteLine("{0} does not have enough health", hunterPlayingEvent.Name());
                return;
            }
            Console.WriteLine("{0} loses {1} health", hunterPlayingEvent.Name(), 6 - game.Vampires);
            game.Hunters[(int)hunterPlayingEvent].AdjustHealth(game.Vampires - 6);
            Console.WriteLine("Where is {0} moving?");
            var destination = Location.Nowhere;
            while (destination == Location.Nowhere)
            {
                destination = Enumerations.GetLocationFromString(Console.ReadLine());
            }
            HandleMoveOperation(game, ((int)hunterPlayingEvent).ToString(), destination.Name(), logic);
        }

        /// <summary>
        /// Resolves the Re-Equip Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        private static void PlayReEquip(GameState game, Hunter hunterPlayingEvent)
        {
            if (DiscardUnknownItemFromHunter(game, game.Hunters[(int)hunterPlayingEvent]))
            {
                game.Hunters[(int)hunterPlayingEvent].DrawItemCard();
                Console.WriteLine("Now draw another card from the Item deck and shuffle it. You now have {0} Items", game.Hunters[(int)hunterPlayingEvent].ItemCount);
            }
        }

        /// <summary>
        /// Resolves the Newspaper Reports Event or Resolve ability
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayNewsPaperReports(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            for (var i = 5; i > 0; i--)
            {
                if (game.Dracula.Trail[i] != null &&
                    game.Dracula.Trail[i].DraculaCards.First().Location != Location.Nowhere &&
                    game.Dracula.Trail[i].DraculaCards.First().Location != game.Dracula.CurrentLocation &&
                    !game.Dracula.Trail[i].DraculaCards.First().IsRevealed)
                {
                    if (DraculaIsPlayingSensationalistPressToPreventRevealingLocation(game,
                        game.Dracula.Trail[i].DraculaCards.First().Location, logic))
                    {
                        Console.WriteLine("Dracula prevented a location from being revealed");
                    }
                    else
                    {
                        game.Dracula.RevealCardAtPosition(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Resolves the Mystic Research Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        private static void PlayMysticResearch(GameState game, Hunter hunterPlayingEvent)
        {
            if (!game.Dracula.EventHand.Any())
            {
                Console.WriteLine("Dracula has no Events to reveal");
                return;
            }
            Console.WriteLine("These are Dracula's Event cards: ");
            foreach (var e in game.Dracula.EventHand)
            {
                Console.Write(" {0},", e.Event.Name());
            }
            Console.WriteLine("");
        }

        /// <summary>
        /// Resolves the Money Trail Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayMoneyTrail(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            for (var i = 5; i > 0; i--)
            {
                if (game.Dracula.Trail[i] != null &&
                    game.Map.TypeOfLocation(game.Dracula.Trail[i].DraculaCards.First().Location) == LocationType.Sea)
                {
                    if (DraculaIsPlayingSensationalistPressToPreventRevealingLocation(game,
                        game.Dracula.Trail[i].DraculaCards.First().Location, logic))
                    {
                        Console.WriteLine("Dracula prevented a location from being revealed");
                    }
                    else
                    {
                        game.Dracula.RevealCardAtPosition(i);
                    }
                }
            }
        }

        /// <summary>
        /// Resolves the Long Day Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        private static void PlayLongDay(GameState game, Hunter hunterPlayingEvent)
        {
            if (game.TimeOfDay == TimeOfDay.Dawn)
            {
                Console.WriteLine("You cannot play this card during Dawn. Take the card back.");
                var longDayCard = game.EventDiscard.Find(card => card.Event == Event.LongDay);
                game.Hunters[(int)hunterPlayingEvent].EventsKnownToDracula.Add(longDayCard);
                game.EventDiscard.Remove(longDayCard);
                game.Hunters[(int)hunterPlayingEvent].DrawEventCard();
                return;
            }
            game.RegressTimeTracker();
        }

        /// <summary>
        /// Resolves the Hypnosis Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayHypnosis(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            Console.WriteLine("Roll a die and enter the result");
            var dieRoll = 0;
            while (dieRoll == 0)
            {
                if (int.TryParse(Console.ReadLine(), out dieRoll))
                {
                    if (dieRoll < 1 || dieRoll > 7)
                    {
                        dieRoll = 0;
                    }
                }
            }
            switch (dieRoll)
            {
                case 1:
                case 2:
                    Console.WriteLine("No effect"); break;
                case 3:
                case 4:
                case 5:
                case 6:
                    if (DraculaIsPlayingSensationalistPressToPreventRevealingLocation(game, game.Dracula.CurrentLocation, logic))
                    {
                        Console.WriteLine("Dracula prevented a location from being revealed");
                    }
                    else
                    {
                        game.Dracula.RevealCardInTrailWithLocation(game.Dracula.CurrentLocation);
                    }
                    game.Dracula.RevealAllVampires();
                    var power = Power.None;
                    var advanceMoveLocation = logic.ChooseDestinationAndPower(game, out power);
                    if (advanceMoveLocation == Location.Nowhere && power == Power.None)
                    {
                        Console.WriteLine("Dracula will have no legal moves next turn");
                    }
                    else
                    {
                        game.Dracula.AdvanceMoveLocation = advanceMoveLocation;
                        game.Dracula.AdvanceMovePower = power;
                        Console.WriteLine("Dracula's next move will be to {0}{1}", game.Dracula.AdvanceMoveLocation,
                            power == Power.None ? "" : " using " + game.Dracula.AdvanceMovePower.Name());
                    } break;
            }
        }

        /// <summary>
        /// Resolves the Hired Scouts Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayHiredScouts(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            Console.WriteLine("Name the first city");
            var firstLocationToReveal = Location.Nowhere;
            while (firstLocationToReveal == Location.Nowhere || (game.Map.TypeOfLocation(firstLocationToReveal) != LocationType.SmallCity && game.Map.TypeOfLocation(firstLocationToReveal) != LocationType.LargeCity))
            {
                firstLocationToReveal = Enumerations.GetLocationFromString(Console.ReadLine());
            }
            var secondLocationToReveal = Location.Nowhere;
            Console.WriteLine("Name the second city");
            secondLocationToReveal = Location.Nowhere;
            while (secondLocationToReveal == Location.Nowhere || (game.Map.TypeOfLocation(secondLocationToReveal) != LocationType.SmallCity && game.Map.TypeOfLocation(secondLocationToReveal) != LocationType.LargeCity))
            {
                secondLocationToReveal = Enumerations.GetLocationFromString(Console.ReadLine());
            }
            if (DraculaIsPlayingSensationalistPressToPreventRevealingLocation(game, firstLocationToReveal, logic))
            {
                Console.WriteLine("Dracula prevented a location from being revealed");
            }
            else
            {
                var trailIndex = game.Dracula.RevealCardInTrailWithLocation(firstLocationToReveal);
                if (trailIndex == -1)
                {
                    trailIndex = game.Dracula.RevealCardInCatacombsWithLocation(firstLocationToReveal);
                }
                if (trailIndex == -1)
                {
                    Console.WriteLine("{0} is not in Dracula's trail or Catacombs", firstLocationToReveal.Name());
                }
                else
                {
                    Console.WriteLine("{0} revealed", firstLocationToReveal.Name());
                    game.Dracula.RevealEncountersAtPositionInTrail(trailIndex);
                    DrawGameState(game);
                }
            }
            if (DraculaIsPlayingSensationalistPressToPreventRevealingLocation(game, secondLocationToReveal, logic))
            {
                Console.WriteLine("Dracula prevented a location from being revealed");
            }
            else
            {
                var trailIndex = game.Dracula.RevealCardInTrailWithLocation(secondLocationToReveal);
                if (trailIndex == -1)
                {
                    trailIndex = game.Dracula.RevealCardInCatacombsWithLocation(secondLocationToReveal);
                }
                if (trailIndex == -1)
                {
                    Console.WriteLine("{0} is not in Dracula's trail or Catacombs", secondLocationToReveal.Name());
                }
                else
                {
                    Console.WriteLine("{0} revealed", secondLocationToReveal.Name());
                    game.Dracula.RevealEncountersAtPositionInTrail(trailIndex);
                    DrawGameState(game);
                }
            }
        }

        /// <summary>
        /// Resolves the Good Luck Event played at the start of a Hunter's turn
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        private static void PlayGoodLuck(GameState game, Hunter hunterPlayingEvent)
        {
            Console.WriteLine("What would you like to discard? 0= Nothing (cancel), 1= Roadblock, 2= Dracula's Ally");
            var answer = -1;
            while (answer == -1)
            {
                if (int.TryParse(Console.ReadLine(), out answer))
                {
                    if (answer < 0 || answer > 2)
                    {
                        answer = -1;
                    }
                }
                switch (answer)
                {
                    case 0:
                        Console.WriteLine("Take the card back");
                        var goodLuckCard = game.EventDiscard.Find(card => card.Event == Event.GoodLuck);
                        game.EventDiscard.Remove(goodLuckCard);
                        game.Hunters[(int)hunterPlayingEvent].EventsKnownToDracula.Add(goodLuckCard);
                        game.Hunters[(int)hunterPlayingEvent].DrawEventCard(); break;
                    case 1:
                        game.RoadBlockLocation1 = Location.Nowhere;
                        game.RoadBlockLocation2 = Location.Nowhere;
                        game.RoadBlockConnectionType = ConnectionType.None; break;
                    case 2:
                        if (game.DraculaAlly != null)
                        {
                            game.EventDiscard.Add(game.DraculaAlly);
                            game.DraculaAlly = null;
                        } break;
                }
            }
        }

        /// <summary>
        /// Resolves the Excellent Weather Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        private static void PlayExcellentWeather(GameState game, Hunter hunterPlayingEvent)
        {
            Console.WriteLine("{0} may make up to four sea moves this turng", hunterPlayingEvent.Name());
        }

        /// <summary>
        /// Resolves the Consecrated Ground Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        private static void PlayConsecratedGround(GameState game, Hunter hunterPlayingEvent)
        {
            Console.WriteLine("Where would you like to place the Consecrated Ground marker?");
            var ground = Location.Nowhere;
            while (ground == Location.Nowhere || ground == Location.CastleDracula || ground == Location.Galatz ||
                   ground == Location.Klausenburg ||
                   (game.Map.TypeOfLocation(ground) != LocationType.SmallCity &&
                    game.Map.TypeOfLocation(ground) != LocationType.LargeCity))
            {
                ground = Enumerations.GetLocationFromString(Console.ReadLine());
            }
            game.ConsecratedGroundLocation = ground;
            if (ground == game.Dracula.CurrentLocation)
            {
                var index = game.Dracula.RevealCardInTrailWithLocation(ground);
                game.Dracula.RevealEncountersAtPositionInTrail(index);
            }
        }

        /// <summary>
        /// Resolves the Chartered Carriage Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void PlayCharteredCarriage(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            if (DraculaIsPlayingFalseTipoffToCancelCharteredCarriage(game, logic))
            {
                Console.WriteLine("Chartered Carriage cancelled");
                return;
            }
            Console.WriteLine("{0} catches a fast train this turn", hunterPlayingEvent.Name());
        }

        /// <summary>
        /// Resolves the Blood Transfusion Event
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Event</param>
        private static void PlayBloodTransfusion(GameState game, Hunter hunterPlayingEvent)
        {
            Console.WriteLine(
                "Who is receiving the transfusion? { 0}= { 1}, { 2}= { 3}, { 4}= { 5}, { 6}= { 7} (-1 to cancel)",
                (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(),
                (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
            var answer = 0;
            while (answer == 0)
            {
                if (int.TryParse(Console.ReadLine(), out answer))
                {
                    if (answer < 1 || answer > 4)
                    {
                        answer = 0;
                    }
                    if (answer == -1)
                    {
                        Console.WriteLine("Cancelled, card is still discarded");
                        return;
                    }
                }
            }
            if ((Hunter)answer == hunterPlayingEvent)
            {
                Console.WriteLine(
                    "Who is giving the transfusion? { 0}= { 1}, { 2}= { 3}, { 4}= { 5}, { 6}= { 7} (-1 to cancel)",
                    (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward,
                    Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker,
                    Hunter.MinaHarker.Name());
                answer = 0;
                while (answer == 0)
                {
                    if (int.TryParse(Console.ReadLine(), out answer))
                    {
                        if (answer < 1 || answer > 4)
                        {
                            answer = 0;
                        }
                    }
                }
                game.Hunters[(int)hunterPlayingEvent].AdjustBites(-1);
                game.Hunters[answer].AdjustHealth(-1);
                CheckForHunterDeath(game);
            }
            else
            {
                game.Hunters[(int)hunterPlayingEvent].AdjustHealth(-1);
                game.Hunters[answer].AdjustBites(-1);
                CheckForHunterDeath(game);
            }
        }

        /// <summary>
        /// Resolves the situation when a Hunter plays an Ally
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterPlayingEvent">The Hunter playing the Ally Event</param>
        /// <param name="allyBeingPlayed">The Ally Event being played</param>
        private static void PlayHunterAlly(GameState game, Hunter hunterPlayingEvent, Event allyBeingPlayed)
        {
            if (game.HunterAlly != null)
            {
                game.EventDiscard.Add(game.HunterAlly);
            }
            game.HunterAlly = game.EventDiscard.Find(card => card.Event == allyBeingPlayed);
            game.EventDiscard.Remove(game.HunterAlly);
        }

        /// <summary>
        /// Determines if Dracula is playing False Tip-off to cancel Chartered Carriage
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if Dracula successfully plays False Tip-off</returns>
        private static bool DraculaIsPlayingFalseTipoffToCancelCharteredCarriage(GameState game, DecisionMaker logic)
        {
            if (logic.ChooseToCancelCharteredCarriageWithFalseTipoff(game))
            {
                Console.WriteLine("Dracula is playing False Tip-off to cancel Chartered Carriage");
                game.Dracula.DiscardEvent(Event.FalseTipoff, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.FalseTipoff, Event.CharteredCarriage, logic) > 0 ||
                    HunterPlayingCharteredCarriageToCancelFalseTipOff(game, Event.FalseTipoff, Event.CharteredCarriage, logic) > 0)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if Dracula is playing Devilish Power to cancel an Event played by a Hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="eventBeingPlayedNow">The Event being played now</param>
        /// <param name="eventInitiallyPlayed">The Event initially played at the beginning of this exchange</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if Dracula successfully plays Devilish Power</returns>
        private static bool DraculaIsPlayingDevilishPowerToCancelEvent(GameState game, Event eventBeingPlayedNow,
            Event eventInitiallyPlayed, DecisionMaker logic)
        {
            if (logic.ChooseToCancelEventWithDevilishPower(game, eventBeingPlayedNow, eventInitiallyPlayed))
            {
                Console.WriteLine("Dracula is playing Devilish Power to cancel {0}", eventBeingPlayedNow.Name());
                game.Dracula.DiscardEvent(Event.DevilishPower, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.DevilishPower, eventInitiallyPlayed, logic) > 0)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if a Hunter is playing Good Luck to cancel an Event played by Dracula
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="draculaEventBeingPlayed">The Event being played now</param>
        /// <param name="eventInitiallyPlayed">The Event initially played at the beginning of this exchange</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>An int corresponding to the Hunter playing Good Luck, 0 if none</returns>
        private static int HunterPlayingGoodLuckToCancelDraculaEvent(GameState game, Event draculaEventBeingPlayed, Event eventInitiallyPlayed, DecisionMaker logic)
        {
            Console.WriteLine(
                "Will anyone play Good Luck to cancel {0}? 0= Nobody, {1}= {2}, {3}= {4}, {5}= {6}, {7}= {8}",
                draculaEventBeingPlayed.Name(), (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(),
                (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(),
                (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
            var answer = -1;
            while (answer == -1)
            {
                if (int.TryParse(Console.ReadLine(), out answer))
                {
                    if (answer < 0 || answer > 4)
                    {
                        answer = -1;
                    }
                }
            }
            if (answer > 0)
            {
                game.Hunters[answer].DiscardEvent(game, Event.GoodLuck);
                if (DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.GoodLuck, eventInitiallyPlayed, logic))
                {
                    game.Dracula.DiscardEvent(Event.DevilishPower, game.EventDiscard);
                    return 0;
                }
            }
            return answer;
        }

        /// <summary>
        /// Resolves a Hunter using an Item outside of combat
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="itemName">The string to be converted to the Item being used</param>
        /// <param name="hunterIndex">The string to be converted to the Hunter using the Item</param>
        private static void UseItem(GameState game, string itemName, string hunterIndex)
        {
            var index = 0;
            var hunterUsingItem = Hunter.Nobody;
            if (int.TryParse(hunterIndex, out index))
            {
                hunterUsingItem = game.GetHunterFromInt(index);
            }
            var line = "";
            while (hunterUsingItem == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is using an Item? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)",
                    (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward,
                    Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker,
                    Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (int.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    hunterUsingItem = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterUsingItem.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            var itemBeingUsed = Enumerations.GetItemFromString(itemName);
            while (itemBeingUsed == Item.None && line.ToLower() != "cancel")
            {
                Console.WriteLine("What is the name of the Item being used?");
                line = Console.ReadLine();
                itemBeingUsed = Enumerations.GetItemFromString(line);
            }
            if (line.ToLower() == "cancel")
            {
                Console.WriteLine("Cancelled");
                return;
            }
            switch (itemBeingUsed)
            {
                case Item.Dogs:
                    Console.WriteLine("Dogs is now face up in front of {0}", hunterUsingItem.Name());
                    game.Hunters[(int)hunterUsingItem].SetDogsFaceUp(true);
                    AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)hunterUsingItem], Item.Dogs); break;
                case Item.LocalRumors:
                    var trailIndex = -1;
                    Console.WriteLine(
                        "In which trail position (1-6) or Catacombs position (7-9) would you like to reveal an Encounter?");
                    while (trailIndex == -1)
                    {
                        if (int.TryParse(Console.ReadLine(), out trailIndex))
                        {
                            if (trailIndex < 1 || trailIndex > 9)
                            {
                                trailIndex = -1;
                            }
                        }
                    }
                    if (trailIndex < 7)
                    {
                        game.Dracula.RevealEncountersAtPositionInTrail(trailIndex - 1);
                    }
                    else
                    {
                        if (game.Dracula.Catacombs[trailIndex - 7] != null &&
                            game.Dracula.Catacombs[trailIndex - 7].EncounterTiles.Count() > 1)
                        {
                            var encounterIndex = -1;
                            while (encounterIndex == -1)
                            {
                                Console.WriteLine("Which Encounter would you like to reveal? 1 or 2");
                                if (int.TryParse(Console.ReadLine(), out encounterIndex))
                                {
                                    if (encounterIndex < 1 || encounterIndex > 2)
                                    {
                                        encounterIndex = -1;
                                    }
                                }
                            }
                            game.Dracula.RevealEncounterAtPositionInTrail(game, trailIndex - 1, encounterIndex - 1);
                        }
                        else
                        {
                            game.Dracula.RevealEncountersAtPositionInTrail(trailIndex - 1);
                        }
                    }
                    game.Hunters[(int)hunterUsingItem].DiscardItem(game, Item.LocalRumors);
                    DrawGameState(game); break;
                case Item.HolyWater:
                    UseHolyWaterOnHunter(game, hunterUsingItem);
                    game.Hunters[(int)hunterUsingItem].DiscardItem(game, Item.HolyWater); break;
                case Item.FastHorse:
                    Console.WriteLine("{0} may travel two roads this turn", hunterUsingItem.Name());
                    game.Hunters[(int)hunterUsingItem].DiscardItem(game, Item.FastHorse); break;
                case Item.HeavenlyHost:
                    Console.WriteLine("Heavenly Host placed in {0}",
                        game.Hunters[(int)hunterUsingItem].CurrentLocation.Name());
                    PlaceHeavenlyHostIn(game, game.Hunters[(int)hunterUsingItem].CurrentLocation);
                    game.Hunters[(int)hunterUsingItem].DiscardItem(game, Item.HeavenlyHost); break;
                default:
                    Console.WriteLine("It is not appropriate to use {0} at this time", itemBeingUsed.Name()); break;
            }
            CheckForCardsRevealedForBeingBitten(game);
        }

        /// <summary>
        /// Resolves a Hunter using Heavenly Host outside of combat
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="location">The Location where the Heavenly Host is being placed</param>
        private static void PlaceHeavenlyHostIn(GameState game, Location location)
        {
            if (game.HeavenlyHostLocation1 == Location.Nowhere)
            {
                game.HeavenlyHostLocation1 = location;
            }
            else if (game.HeavenlyHostLocation2 == Location.Nowhere)
            {
                game.HeavenlyHostLocation2 = location;
            }
            else
            {
                Console.WriteLine("Where are you moving a Heavenly Host token from? 1= {0}, 2= {1}",
                    game.HeavenlyHostLocation1.Name(), game.HeavenlyHostLocation2.Name());
                var answer = 0;
                while (answer == 0)
                {
                    if (int.TryParse(Console.ReadLine(), out answer))
                    {
                        if (answer < 1 || answer > 2)
                        {
                            answer = 0;
                        }
                    }
                }
                switch (answer)
                {
                    case 1:
                        game.HeavenlyHostLocation1 = location; break;
                    case 2:
                        game.HeavenlyHostLocation2 = location; break;
                }
            }
        }

        /// <summary>
        /// Resolves a Hunter using the Holy Water Item or the Holy Water font at St. Joseph & St. Mary
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterReceivingHolyWater">The Hunter receiving the Holy Water effect</param>
        private static void UseHolyWaterOnHunter(GameState game, Hunter hunterReceivingHolyWater)
        {
            if (hunterReceivingHolyWater == Hunter.MinaHarker)
            {
                Console.WriteLine("Mina's bite cannot be cured");
                return;
            }
            Console.WriteLine("Roll a die and enter the result");
            var dieRoll = 0;
            while (dieRoll == 0)
            {
                if (int.TryParse(Console.ReadLine(), out dieRoll))
                {
                    if (dieRoll < 1 || dieRoll > 7)
                    {
                        dieRoll = 0;
                    }
                }
            }
            switch (dieRoll)
            {
                case 1:
                    Console.WriteLine("{0} loses 2 health", hunterReceivingHolyWater.Name());
                    game.Hunters[(int)hunterReceivingHolyWater].AdjustHealth(-2);
                    CheckForHunterDeath(game); break;
                case 2:
                case 3:
                case 4:
                    Console.WriteLine("Nothing happens"); break;
                case 5:
                case 6:
                    Console.WriteLine("{0} is cured of a Bite", hunterReceivingHolyWater.Name());
                    game.Hunters[(int)hunterReceivingHolyWater].AdjustBites(-1); break;
            }
        }

        /// <summary>
        /// Checks if a given Item card is already known by Dracula to be in a given Hunter's hand and adds it if not
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterRevealingCard">The Hunter revealing the card</param>
        /// <param name="itemBeingRevealed">The Item being revealed</param>
        /// <returns>The card corresponding to the Item revealed</returns>
        private static ItemCard AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(GameState game,
            HunterPlayer hunterRevealingCard, Item itemBeingRevealed)
        {
            var itemCardBeingRevealed =
                hunterRevealingCard.ItemsKnownToDracula.Find(card => card.Item == itemBeingRevealed);
            if (itemCardBeingRevealed == null)
            {
                itemCardBeingRevealed = game.ItemDeck.Find(card => card.Item == itemBeingRevealed);
                game.ItemDeck.Remove(itemCardBeingRevealed);
                hunterRevealingCard.ItemsKnownToDracula.Add(itemCardBeingRevealed);
            }
            return itemCardBeingRevealed;
        }

        /// <summary>
        /// Checks if a given Event card is already known by Dracula to be in a given Hunter's hand and adds it if not
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterRevealingCard">The Hunter revealing the card</param>
        /// <param name="eventBeingRevealed">The Event being revealed</param>
        /// <returns>The card corresponding to the Event revealed</returns>
        private static EventCard AddEventCardToDraculaKnownCardsIfNotAlreadyKnown(GameState game,
            HunterPlayer hunterRevealingCard, Event eventBeingRevealed)
        {
            var eventCardBeingRevealed =
                hunterRevealingCard.EventsKnownToDracula.Find(card => card.Event == eventBeingRevealed);
            if (eventCardBeingRevealed == null)
            {
                eventCardBeingRevealed = game.EventDeck.Find(card => card.Event == eventBeingRevealed);
                game.EventDeck.Remove(eventCardBeingRevealed);
                hunterRevealingCard.EventsKnownToDracula.Add(eventCardBeingRevealed);
            }
            return eventCardBeingRevealed;
        }

        /// <summary>
        /// Checks all Hunters to see if they are dead and adjusts them accordingly if they are
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <returns>True if a Hunter has died</returns>
        private static bool CheckForHunterDeath(GameState game)
        {
            var hunterDied = false;
            foreach (var h in game.Hunters)
            {
                if (h != null && (h.Health == 0 || h.BiteCount == h.BitesRequiredToKill))
                {
                    Console.WriteLine(
                        "{0} has been defeated. All Items and Events discarded. Hunter moved to St. Joseph and St. Mary and must skip the next turn", h.Hunter.Name());
                    while (h.ItemCount > 0)
                    {
                        DiscardUnknownItemFromHunter(game, h);
                    }
                    while (h.EventCount > 0)
                    {
                        DiscardUnknownEventFromHunter(game, h);
                    }
                    h.MoveTo(Location.StJosephAndStMary);
                    h.AdjustHealth(h.MaxHealth);
                    h.AdjustBites(-3);
                    if (h.Hunter == Hunter.MinaHarker)
                    {
                        h.AdjustBites(1);
                    }
                    hunterDied = true;
                }
            }
            return hunterDied;
        }

        /// <summary>
        ///     Asks the user to name an Item and then discards it from that Hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunter">The Hunter discarding an Item</param>
        private static bool DiscardUnknownItemFromHunter(GameState game, HunterPlayer hunter)
        {
            Console.WriteLine("Name the Item being discarded (cancel to cancel)");
            var itemBeingDiscarded = Item.None;
            var line = "";
            while (itemBeingDiscarded == Item.None && line.ToLower() != "cancel")
            {
                line = Console.ReadLine();
                itemBeingDiscarded = Enumerations.GetItemFromString(line);
            }
            if (line.ToLower() != "cancel")
            {
                hunter.DiscardItem(game, itemBeingDiscarded);
                return true;
            }
            else
            {
                Console.WriteLine("No Item discarded");
                return false;
            }
        }

        /// <summary>
        /// Asks the user to name an Event and then discards it from that Hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunter">The Hunter discarding an Event</param>
        private static void DiscardUnknownEventFromHunter(GameState game, HunterPlayer hunter)
        {
            Console.WriteLine("Name the Event being discarded");
            var eventBeingDiscarded = Event.None;
            while (eventBeingDiscarded == Event.None)
            {
                eventBeingDiscarded = Enumerations.GetEventFromString(Console.ReadLine());
            }
            hunter.DiscardEvent(game, eventBeingDiscarded);
        }

        /// <summary>
        /// Checks all Hunters' cards and bites to see if they should be revealing a card to Dracula
        /// </summary>
        /// <param name="game">The GameState</param>
        private static void CheckForCardsRevealedForBeingBitten(GameState game)
        {
            foreach (var h in game.Hunters)
            {
                if (h != null)
                {
                    if (h.ItemCount > 0 && h.BiteCount > 0 && h.ItemShownToDraculaForBeingBitten == null)
                    {
                        var itemToReveal = Item.None;
                        Console.WriteLine(
                            "{0} is bitten and does not have an Item revealed to Dracula. Please name the Item you are revealing",
                            h.Hunter.Name());
                        while (itemToReveal == Item.None)
                        {
                            itemToReveal = Enumerations.GetItemFromString(Console.ReadLine());
                            if (itemToReveal == Item.None)
                            {
                                Console.WriteLine("I couldn't find an Item with that name");
                            }
                        }
                        var itemCardToReveal = AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, h, itemToReveal);
                        h.ItemShownToDraculaForBeingBitten = itemCardToReveal;
                        Console.WriteLine("{0} revealed to Dracula", itemCardToReveal.Item.Name());
                    }
                    if (h.EventCount > 0 && h.BiteCount > 0 && h.EventShownToDraculaForBeingBitten == null)
                    {
                        var eventToReveal = Event.None;
                        Console.WriteLine(
                            "{0} is bitten and does not have an Event revealed to Dracula. Please name the Event you are revealing",
                            h.Hunter.Name());
                        while (eventToReveal == Event.None)
                        {
                            eventToReveal = Enumerations.GetEventFromString(Console.ReadLine());
                            if (eventToReveal == Event.None)
                            {
                                Console.WriteLine("I couldn't find an Item with that name");
                            }
                        }
                        var eventCardToReveal = AddEventCardToDraculaKnownCardsIfNotAlreadyKnown(game, h, eventToReveal);
                        h.EventShownToDraculaForBeingBitten = eventCardToReveal;
                        Console.WriteLine("{0} revealed to Dracula", eventCardToReveal.Event.Name());
                    }
                }
            }
        }

        /// <summary>
        /// Handles the entirety of a Hunter's move, including search, Encounters, combat etc.
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterIndex">The index of the Hunter (and group) to move</param>
        /// <param name="destination">The destination of the move</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void HandleMoveOperation(GameState game, string destination, string hunterIndex,
            DecisionMaker logic)
        {
            var hunterMoved = Hunter.Nobody;
            Location originatingLocation;
            var trailIndex = MoveHunter(game, hunterIndex, destination, out hunterMoved, out originatingLocation, logic);
            if (DraculaIsPlayingCustomsSearch(game, hunterMoved, originatingLocation, logic))
            {
                foreach (var h in game.Hunters[(int)hunterMoved].HuntersInGroup)
                {
                    Console.WriteLine("{0} must discard all Items", h.Name());
                    while (game.Hunters[(int)h].ItemCount > 0)
                    {
                        DiscardUnknownItemFromHunter(game, game.Hunters[(int)h]);
                    }
                }
            }
            if (trailIndex > -1 && game.Map.TypeOfLocation(game.Dracula.Trail[trailIndex].DraculaCards.First().Location) != LocationType.Sea)
            {
                game.Dracula.RevealCardAtPosition(trailIndex);
                game.Dracula.RevealEncountersAtPositionInTrail(trailIndex);
                DraculaCardSlot slotBeingRevealed;
                if (trailIndex < 6)
                {
                    slotBeingRevealed = game.Dracula.Trail[trailIndex];
                }
                else
                {
                    slotBeingRevealed = game.Dracula.Catacombs[trailIndex - 6];
                }
                if (slotBeingRevealed.DraculaCards.First().Location == game.Dracula.LocationWhereHideWasUsed)
                {
                    var positionRevealed = game.Dracula.RevealHideCard();
                    game.Dracula.RevealEncountersAtPositionInTrail(positionRevealed);
                }
                if (slotBeingRevealed.DraculaCards.First().Location == game.Dracula.CurrentLocation)
                {
                    Console.WriteLine("Dracula is here!");
                }
                else
                {
                    Console.WriteLine("Dracula has been here");
                }
                DrawGameState(game);
                var encounterTilesToResolve = new List<EncounterTile>();
                foreach (var enc in slotBeingRevealed.EncounterTiles)
                {
                    encounterTilesToResolve.Add(enc);
                }
                var hunterCanContinueToResolveEncounters = true;
                while (encounterTilesToResolve.Count() > 0 && hunterCanContinueToResolveEncounters)
                {
                    var encounterTileBeingResolved = logic.ChooseEncounterToResolveOnSearchingHunter(game,
                        encounterTilesToResolve);
                    hunterCanContinueToResolveEncounters = ResolveEncounterTile(game, encounterTileBeingResolved,
                        game.Hunters[(int)hunterMoved].HuntersInGroup, logic, trailIndex);
                    encounterTilesToResolve.Remove(encounterTileBeingResolved);
                }
                if (game.Hunters[(int)hunterMoved].CurrentLocation == game.Dracula.LocationWhereHideWasUsed)
                {
                    foreach (var enc in game.Dracula.slotWhereHideCardIs().EncounterTiles)
                    {
                        encounterTilesToResolve.Add(enc);
                    }
                }
                while (encounterTilesToResolve.Count() > 0 && hunterCanContinueToResolveEncounters)
                {
                    var encounterTileBeingResolved = logic.ChooseEncounterToResolveOnSearchingHunter(game,
                        encounterTilesToResolve);
                    hunterCanContinueToResolveEncounters = ResolveEncounterTile(game, encounterTileBeingResolved,
                        game.Hunters[(int)hunterMoved].HuntersInGroup, logic, game.Dracula.PositionWhereHideCardIs());
                    encounterTilesToResolve.Remove(encounterTileBeingResolved);
                }
                if (slotBeingRevealed.DraculaCards.First().Location == game.Dracula.CurrentLocation &&
                    hunterCanContinueToResolveEncounters)
                {
                    Console.WriteLine("Entering combat with Dracula");
                    if (DraculaIsPlayingWildHorses(game, logic))
                    {
                        Console.WriteLine("The Wild Horses carry you away");
                        HandleMoveOperation(game, ((int)hunterMoved).ToString(),
                            logic.ChooseDestinationForWildHorses(game).Name(), logic);
                    }
                    else
                    {
                        var huntersAttacking = new List<HunterPlayer>();
                        foreach (var h in game.Hunters[(int)hunterMoved].HuntersInGroup)
                        {
                            huntersAttacking.Add(game.Hunters[(int)h]);
                        }
                        ResolveCombat(game, huntersAttacking, Opponent.Dracula, logic);
                        CheckForHunterDeath(game);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if Dracula is playing Wild Horses at the start of a combat
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if Dracula successfully plays Wild Horses</returns>
        private static bool DraculaIsPlayingWildHorses(GameState game, DecisionMaker logic)
        {
            if (logic.ChooseToPlayWildHorses(game))
            {
                Console.WriteLine("Dracula is playing Wild Horses to control your movement");
                game.Dracula.DiscardEvent(Event.WildHorses, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.WildHorses, Event.WildHorses, logic) > 0)
                {
                    Console.WriteLine("Wild Horses cancelled");
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if Dracula is playing Customs Search on a Hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterMoved">The Hunter who moved</param>
        /// <param name="origin">The original Location</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if Dracula successfully plays Customs Search</returns>
        private static bool DraculaIsPlayingCustomsSearch(GameState game, Hunter hunterMoved, Location origin,
            DecisionMaker logic)
        {
            if (logic.ChooseToPlayCustomsSearch(game, hunterMoved, origin))
            {
                Console.WriteLine("Dracula is playing Customs Search");
                game.Dracula.DiscardEvent(Event.CustomsSearch, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.CustomsSearch, Event.CustomsSearch, logic) > 0)
                {
                    Console.WriteLine("Customs Search cancelled");
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds and removes people from the given Hunter's group
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterIndex">A string to be converted to the Hunter with whom to set up a group</param>
        private static void SetupGroup(GameState game, string hunterIndex)
        {
            var hunterFormingGroup = Hunter.Nobody;
            int index;
            if (int.TryParse(hunterIndex, out index))
            {
                hunterFormingGroup = game.GetHunterFromInt(index);
            }
            var line = "";
            while (hunterFormingGroup == Hunter.Nobody && index != -1)
            {
                Console.WriteLine(
                    "Who is forming a group? {0}= {1}, {2}= {3}, {4}= {5} (Mina Harker cannot lead a group, -1 to cancel)",
                    (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward,
                    Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name());
                line = Console.ReadLine();
                if (int.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    if (index == 4)
                    {
                        Console.WriteLine("Mina Harker cannot lead a group, add her to someone else's group instead");
                    }
                    else
                    {
                        hunterFormingGroup = game.GetHunterFromInt(index);
                        Console.WriteLine(hunterFormingGroup.Name());
                    }
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            while (index != -1)
            {
                Console.WriteLine("These are the people in {0}'s group:", hunterFormingGroup.Name());
                foreach (var h in game.Hunters[(int)hunterFormingGroup].HuntersInGroup)
                {
                    if (h != hunterFormingGroup)
                    {
                        Console.WriteLine(h.Name());
                    }
                }
                var hunterToAddOrRemove = Hunter.Nobody;
                while (hunterToAddOrRemove == Hunter.Nobody && index != -1)
                {
                    Console.WriteLine(
                        "Who is joining or leaving {0}'s group? {1}= {2}, {3}= {4}, {5}= {6} (Lord Godalming must lead any group he is in, -1 to cancel)",
                        hunterFormingGroup.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(),
                        (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker,
                        Hunter.MinaHarker.Name());
                    line = Console.ReadLine();
                    if (int.TryParse(line, out index))
                    {
                        if (index == -1)
                        {
                            Console.WriteLine("Cancelled");
                            return;
                        }
                        if (index == 1)
                        {
                            Console.WriteLine("Lord Godalming must lead any group he is in");
                        }
                        else
                        {
                            hunterToAddOrRemove = game.GetHunterFromInt(index);
                            Console.WriteLine(hunterFormingGroup.Name());
                        }
                    }
                    else
                    {
                        Console.WriteLine("I didn't understand that");
                    }
                }
                if ((int)hunterToAddOrRemove < (int)hunterFormingGroup)
                {
                    Console.WriteLine("{0} cannot join {1}'s group, instead add {1} to {0}'s group",
                        hunterToAddOrRemove.Name(), hunterFormingGroup.Name());
                }
                else if (hunterToAddOrRemove == hunterFormingGroup)
                {
                    Console.WriteLine("{0} is already in his own group, of course!", hunterFormingGroup.Name());
                }
                else if (game.Hunters[(int)hunterFormingGroup].HuntersInGroup.Contains(hunterToAddOrRemove))
                {
                    game.Hunters[(int)hunterFormingGroup].HuntersInGroup.Remove(hunterToAddOrRemove);
                }
                else
                {
                    game.Hunters[(int)hunterFormingGroup].HuntersInGroup.Add(hunterToAddOrRemove);
                }
            }
        }

        /// <summary>
        /// Matures an Encounter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="encounterTileBeingMatured">The EncounterTile being matured</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void MatureEncounter(GameState game, EncounterTile encounterTileBeingMatured, DecisionMaker logic)
        {
            switch (encounterTileBeingMatured.Encounter)
            {
                case Encounter.Ambush:
                    Console.WriteLine("Dracula matured Ambush");
                    var hunterToAmbush = logic.ChooseHunterToAmbush(game);
                    if (hunterToAmbush != Hunter.Nobody)
                    {
                        var encounterToAmbushWith = logic.ChooseEncounterTileToAmbushHunterWith(game, hunterToAmbush);
                        ResolveEncounterTile(game, encounterToAmbushWith, new List<Hunter> { hunterToAmbush }, logic, -1);
                        game.Dracula.DiscardEncounterTile(game, encounterToAmbushWith);
                        game.EncounterPool.Add(encounterToAmbushWith);
                    } break;
                case Encounter.DesecratedSoil:
                    Console.WriteLine("Dracula matured Desecrated Soil");
                    game.Dracula.ClearTrailDownTo(game, 3);
                    for (var i = 0; i < 2; i++)
                    {
                        Console.WriteLine(
                            "Draw an Event card. If it is a Hunter card, name it, otherwise type \"Dracula\"");
                        var line = "";
                        var eventDrawn = Event.None;
                        do
                        {
                            line = Console.ReadLine();
                            eventDrawn = Enumerations.GetEventFromString(line);
                        } while ((eventDrawn == Event.None ||
                                  !game.EventDeck.Find(card => card.Event == eventDrawn).IsDraculaCard) &&
                                 !"dracula".StartsWith(line.ToLower()));
                        if (eventDrawn == Event.None)
                        {
                            var eventPlayedByDracula = game.Dracula.TakeEvent(game.EventDeck, game.EventDiscard);
                            PlayImmediatelyDraculaCard(game, eventPlayedByDracula, logic);
                            if (game.Dracula.EventHand.Count() > game.Dracula.EventHandSize)
                            {
                                Console.WriteLine("Dracula discarded {0}", game.Dracula.DiscardEvent(logic.ChooseEventToDiscard(game), game.EventDiscard).Name());
                            }
                        }
                        else
                        {
                            var eventCardDiscarded = game.EventDeck.Find(card => card.Event == eventDrawn);
                            game.EventDeck.Remove(eventCardDiscarded);
                            game.EventDiscard.Add(eventCardDiscarded);
                        }
                    } break;
                case Encounter.NewVampire:
                    Console.WriteLine("Dracula matured a New Vampire");
                    game.AdjustVampires(2);
                    game.Dracula.ClearTrailDownTo(game, 1); break;
            }
            game.EncounterPool.Add(encounterTileBeingMatured);
        }

        /// <summary>
        /// Resolves an Encounter on a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="encounterTileBeingResolved">The Encounter being resolved</param>
        /// <param name="trailIndex">The Index of the Trail slot (0-5), or Catacombs slot (6-8) where the Encounter is being resolved, or -1 if being resolved from Dracula's hand</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveEncounterTile(GameState game, EncounterTile encounterTileBeingResolved,
            List<Hunter> huntersInvolved, DecisionMaker logic, int trailIndex)
        {
            Console.WriteLine("Resolving {0} with {1}{2}", encounterTileBeingResolved.Encounter.Name(),
                huntersInvolved.First().Name(), huntersInvolved.Count() > 1 ? " and his group" : "");
            var continueEncounters = true;
            var discardEncounterTile = true;
            foreach (var h in huntersInvolved)
            {
                Console.WriteLine("Will {0} play Secret Weapon?", h.Name());
                var line = "";
                do
                {
                    line = Console.ReadLine();
                } while (!"yes".StartsWith(line.ToLower()) && !"no".StartsWith(line.ToLower()));
                if ("yes".StartsWith(line.ToLower()))
                {
                    game.Hunters[(int)h].DiscardEvent(game, Event.SecretWeapon);
                    if (DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.SecretWeapon, Event.SecretWeapon, logic))
                    {
                        Console.WriteLine("Secret Weapon cancelled");
                    }
                    else
                    {
                        DiscardUnknownItemFromHunter(game, game.Hunters[(int)h]);
                        Console.WriteLine("What is the name of the Item being retrieved from the discard pile?");
                        var itemRetrieved = Item.None;
                        while (itemRetrieved == Item.None)
                        {
                            itemRetrieved = Enumerations.GetItemFromString(Console.ReadLine());
                        }
                        var itemCardRetrieved = game.ItemDiscard.Find(card => card.Item == itemRetrieved);
                        game.ItemDiscard.Remove(itemCardRetrieved);
                        game.Hunters[(int)h].ItemsKnownToDracula.Add(itemCardRetrieved);
                        game.Hunters[(int)h].DrawItemCard();
                    }
                }
            }

            foreach (var h in huntersInvolved)
            {
                Console.WriteLine("Will {0} play Forewarned?", h.Name());
                var line = "";
                do
                {
                    line = Console.ReadLine();
                } while (!"yes".StartsWith(line.ToLower()) && !"no".StartsWith(line.ToLower()));
                if ("yes".StartsWith(line.ToLower()))
                {
                    game.Hunters[(int)h].DiscardEvent(game, Event.Forewarned);
                    if (DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.SecretWeapon, Event.SecretWeapon, logic))
                    {
                        Console.WriteLine("Forewarned cancelled");
                    }
                    else
                    {
                        Console.WriteLine("Encounter cancelled");
                        game.EncounterPool.Add(encounterTileBeingResolved);
                        if (trailIndex > -1)
                        {
                            game.Dracula.Trail[trailIndex].EncounterTiles.Remove(encounterTileBeingResolved);
                        }
                        return true;
                    }
                }
            }

            switch (encounterTileBeingResolved.Encounter)
            {
                case Encounter.Ambush:
                    continueEncounters = ResolveAmbush(game, huntersInvolved, logic); break;
                case Encounter.Assassin:
                    continueEncounters = ResolveAssassin(game, huntersInvolved, logic); break;
                case Encounter.Bats:
                    discardEncounterTile = false;
                    game.Hunters[(int)huntersInvolved.First()].EncountersInFrontOfPlayer.Add(encounterTileBeingResolved);
                    if (trailIndex > -1)
                    {
                        game.Dracula.Trail[trailIndex].EncounterTiles.Remove(encounterTileBeingResolved);
                    }
                    else
                    {
                        game.Dracula.EncounterHand.Remove(encounterTileBeingResolved);
                    }
                    return false;
                case Encounter.DesecratedSoil:
                    continueEncounters = ResolveDesecratedSoil(game, huntersInvolved, logic); break;
                case Encounter.Fog:
                    discardEncounterTile = false;
                    game.Hunters[(int)huntersInvolved.First()].EncountersInFrontOfPlayer.Add(encounterTileBeingResolved);
                    if (trailIndex > -1)
                    {
                        game.Dracula.Trail[trailIndex].EncounterTiles.Remove(encounterTileBeingResolved);
                    }
                    else
                    {
                        game.Dracula.EncounterHand.Remove(encounterTileBeingResolved);
                    }
                    return false;
                case Encounter.MinionWithKnife:
                    continueEncounters = ResolveMinionWithKnife(game, huntersInvolved, logic); break;
                case Encounter.MinionWithKnifeAndPistol:
                    continueEncounters = ResolveMinionWithKnifeAndPistol(game, huntersInvolved, logic); break;
                case Encounter.MinionWithKnifeAndRifle:
                    continueEncounters = ResolveMinionWithKnifeAndRifle(game, huntersInvolved, logic); break;
                case Encounter.Hoax:
                    continueEncounters = ResolveHoax(game, huntersInvolved); break;
                case Encounter.Lightning:
                    continueEncounters = ResolveLightning(game, huntersInvolved); break;
                case Encounter.Peasants:
                    continueEncounters = ResolvePeasants(game, huntersInvolved); break;
                case Encounter.Plague:
                    continueEncounters = ResolvePlague(game, huntersInvolved); break;
                case Encounter.Rats:
                    continueEncounters = ResolveRats(game, huntersInvolved); break;
                case Encounter.Saboteur:
                    continueEncounters = ResolveSaboteur(game, huntersInvolved); break;
                case Encounter.Spy:
                    continueEncounters = ResolveSpy(game, huntersInvolved); break;
                case Encounter.Thief:
                    continueEncounters = ResolveThief(game, huntersInvolved, logic); break;
                case Encounter.NewVampire:
                    var seductionPlayed = false;
                    continueEncounters = ResolveNewVampire(game, huntersInvolved, logic, out discardEncounterTile,
                        out seductionPlayed);
                    if (seductionPlayed)
                    {
                        game.Dracula.EncounterHand.Add(encounterTileBeingResolved);
                        while (game.Dracula.EncounterHand.Count() > game.Dracula.EncounterHandSize)
                        {
                            game.Dracula.DiscardEncounterTile(game,
                                logic.ChooseEncounterTileToDiscardFromEncounterHand(game));
                        }
                        if (trailIndex > -1)
                        {
                            game.Dracula.Trail[trailIndex].EncounterTiles.Remove(encounterTileBeingResolved);
                        }
                        else
                        {
                            game.Dracula.EncounterHand.Remove(encounterTileBeingResolved);
                        }
                    } break;
                case Encounter.Wolves:
                    continueEncounters = ResolveWolves(game, huntersInvolved); break;
                default:
                    return false;
            }
            if (discardEncounterTile)
            {
                game.EncounterPool.Add(encounterTileBeingResolved);
                if (trailIndex > -1)
                {
                    game.Dracula.Trail[trailIndex].EncounterTiles.Remove(encounterTileBeingResolved);
                }
                else
                {
                    game.Dracula.EncounterHand.Remove(encounterTileBeingResolved);
                }
            }
            return continueEncounters;
        }

        /// <summary>
        /// Resolves an Encounter of Wolves for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the Encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveWolves(GameState game, List<Hunter> huntersInvolved)
        {
            var groupHasPistol = false;
            var groupHasRifle = false;
            foreach (var h in huntersInvolved)
            {
                Console.WriteLine("What Items does {0} have? 0= nothing, 1= Pistol, 2=Rifle, 3=Both", h.Name());
                var answer = -1;
                while (answer == -1)
                {
                    if (!(int.TryParse(Console.ReadLine(), out answer) && answer > -1 && answer < 4))
                    {
                        answer = -1;
                    }
                }
                switch (answer)
                {
                    case 1:
                        groupHasPistol = true;
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], Item.Pistol); break;
                    case 2:
                        groupHasRifle = true;
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], Item.Rifle); break;
                    case 3:
                        groupHasPistol = true;
                        groupHasRifle = true;
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], Item.Pistol);
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], Item.Rifle); break;
                }
                if (groupHasPistol && groupHasRifle)
                {
                    break;
                }
            }
            if (groupHasPistol && groupHasRifle)
            {
                Console.WriteLine("Encounter neutralised");
            }
            else
            {
                foreach (var h in huntersInvolved)
                {
                    Console.WriteLine("{0} loses {1} health", h.Name(),
                        ((groupHasPistol ? 0 : 1) + (groupHasRifle ? 0 : 1)));
                }
                if (CheckForHunterDeath(game))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Resolves an Encounter of New Vampire for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the Encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveNewVampire(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic,
            out bool discardNewVampire, out bool seductionPlayed)
        {
            seductionPlayed = false;
            switch (game.TimeOfDay)
            {
                case TimeOfDay.Dawn:
                case TimeOfDay.Noon:
                case TimeOfDay.Dusk:
                    Console.WriteLine("New Vampire encountered during the day and disposed of");
                    discardNewVampire = true;
                    return true;
            }
            if (DraculaIsPlayingSeduction(game, logic))
            {
                seductionPlayed = true;
                discardNewVampire = false;
                foreach (var h in huntersInvolved)
                {
                    Console.WriteLine("{0} is bitten!");
                    if (!HunterPlayingGreatStrengthToCancelBite(game, h, logic))
                    {
                        game.Hunters[(int)h].AdjustBites(1);
                    }
                }
                if (CheckForHunterDeath(game))
                {
                    return false;
                }
                return true;
            }
            Console.WriteLine("Roll a die and enter the result");
            var dieRoll = 0;
            while (dieRoll == 0)
            {
                if (!(int.TryParse(Console.ReadLine(), out dieRoll) && dieRoll > 0 && dieRoll < 7))
                {
                    dieRoll = 0;
                }
            }
            switch (dieRoll)
            {
                case 1:
                case 2:
                case 3:
                    Console.WriteLine("The Vampire attempts to bite you");
                    var groupHasHolyItem = false;
                    foreach (var h in huntersInvolved)
                    {
                        Console.WriteLine("What Items does {0} have? 0= nothing, 1= Crucifix, 2= Heavenly Host",
                            h.Name());
                        var holyItem = Item.None;
                        var answer = -1;
                        while (answer == -1)
                        {
                            if (int.TryParse(Console.ReadLine(), out answer))
                            {
                                if (answer > -1 && answer < 3)
                                {
                                    switch (answer)
                                    {
                                        case 0:
                                            holyItem = Item.None;
                                            break;
                                        case 1:
                                            holyItem = Item.Crucifix;
                                            break;
                                        case 2:
                                            holyItem = Item.HeavenlyHost;
                                            break;
                                    }
                                }
                                else
                                {
                                    answer = -1;
                                }
                            }
                        }
                        if (holyItem != Item.None)
                        {
                            groupHasHolyItem = true;
                            AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], holyItem);
                            break;
                        }
                    }
                    if (groupHasHolyItem)
                    {
                        Console.WriteLine("Bite attempted negated by holy item");
                        discardNewVampire = true;
                        return true;
                    }
                    discardNewVampire = true;
                    foreach (var h in huntersInvolved)
                    {
                        Console.WriteLine("{0} is bitten!", h.Name());
                        if (!HunterPlayingGreatStrengthToCancelBite(game, h, logic))
                        {
                            game.Hunters[(int)h].AdjustBites(1);
                        }
                    }
                    if (CheckForHunterDeath(game))
                    {
                        return false;
                    }
                    break;
                case 4:
                case 5:
                case 6:
                    Console.WriteLine("The Vampire attempts to escape");
                    var groupHasSharpItem = false;
                    foreach (var h in huntersInvolved)
                    {
                        Console.WriteLine("What Items does {0} have? 0= nothing, 1= Knife, 2= Stake", h.Name());
                        var sharpItem = Item.None;
                        var answer = -1;
                        while (answer == -1)
                        {
                            if (int.TryParse(Console.ReadLine(), out answer))
                            {
                                if (answer > -1 && answer < 3)
                                {
                                    switch (answer)
                                    {
                                        case 0:
                                            sharpItem = Item.None;
                                            break;
                                        case 1:
                                            sharpItem = Item.Knife;
                                            break;
                                        case 2:
                                            sharpItem = Item.Stake;
                                            break;
                                    }
                                }
                                else
                                {
                                    answer = -1;
                                }
                            }
                        }
                        if (sharpItem != Item.None)
                        {
                            groupHasSharpItem = true;
                            game.Hunters[(int)h].DiscardItem(game, sharpItem);
                            break;
                        }
                    }
                    if (groupHasSharpItem)
                    {
                        Console.WriteLine("Escape attempted negated by sharp weapon");
                        discardNewVampire = true;
                        return true;
                    }
                    Console.WriteLine("The Vampire escapes and remains in the city");
                    discardNewVampire = false;
                    return true;
            }
            discardNewVampire = true;
            return true;
        }

        /// <summary>
        /// Determines if Dracula is playing Seduction at the start of an Encounter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if Dracula successfully plays Seduction</returns>
        private static bool DraculaIsPlayingSeduction(GameState game, DecisionMaker logic)
        {
            if (logic.ChooseToPlaySeduction(game))
            {
                Console.WriteLine("Dracula is playing Seduction");
                game.Dracula.DiscardEvent(Event.Seduction, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.Seduction, Event.Seduction, logic) > 0)
                {
                    Console.WriteLine("Seduction cancelled");
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resolves an Encounter of Thief for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the Encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveThief(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            foreach (var h in huntersInvolved)
            {
                if (game.Hunters[(int)h].HasDogsFaceUp)
                {
                    return true;
                }
            }
            foreach (var h in huntersInvolved)
            {
                var typeOfCardToDiscard = logic.ChooseToDiscardItemFromHunterInsteadOfEvent(game.Hunters[(int)h]);
                if (typeOfCardToDiscard != CardType.None)
                {
                    Console.WriteLine("Dracula has decided to discard an {0} from {1}", typeOfCardToDiscard, h.Name());
                }
            }
            Console.WriteLine("Use the standard discard command to tell me what was discarded");
            return true;
        }

        /// <summary>
        /// Resolves an Encounter of Spy for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the Encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveSpy(GameState game, List<Hunter> huntersInvolved)
        {
            foreach (var h in huntersInvolved)
            {
                if (game.Hunters[(int)h].HasDogsFaceUp)
                {
                    return true;
                }
            }
            foreach (var h in huntersInvolved)
            {
                Console.WriteLine("{0} must reveal all of {1} cards and declare {1} next move", h.Name(), h == Hunter.MinaHarker ? "her" : "his");
                DraculaLooksAtAllHuntersItems(game, game.Hunters[(int)h]);
                DraculaLooksAtAllHuntersEvents(game, game.Hunters[(int)h]);
                HunterDeclaresNextMove(game, game.Hunters[(int)h]);
            }
            return true;
        }

        /// <summary>
        /// Handles the situation when a Hunter must reveal their next move
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="victim">The Hunter revealing their move</param>
        private static void HunterDeclaresNextMove(GameState game, HunterPlayer victim)
        {
            Console.WriteLine("What will be {0}'s destination next turn?", victim.Hunter.Name());
            var destination = Location.Nowhere;
            while (destination == Location.Nowhere)
            {
                destination = Enumerations.GetLocationFromString(Console.ReadLine());
            }
            Console.WriteLine("What will be {0}'s means of travel next turn?", victim.Hunter.Name());
            var methodOfTravel = ConnectionType.None;
            while (methodOfTravel == ConnectionType.None)
            {
                methodOfTravel = Enumerations.GetConnectionTypeFromString(Console.ReadLine());
            }
            victim.SetNextMoveDestination(destination);
            victim.SetNextMoveConnectionType(methodOfTravel);
        }

        /// <summary>
        /// Resolves an Encounter of Saboteur for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the Encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveSaboteur(GameState game, List<Hunter> huntersInvolved)
        {
            foreach (var h in huntersInvolved)
            {
                if (game.Hunters[(int)h].HasDogsFaceUp)
                {
                    return true;
                }
            }
            foreach (var h in huntersInvolved)
            {
                var itemDiscarded = Item.None;
                var eventDiscarded = Event.None;
                var line = "";
                if (game.Hunters[(int)h].ItemCount > 0)
                {
                    if (game.Hunters[(int)h].EventCount > 0)
                    {
                        Console.WriteLine(
                            "{0} must discard 1 Item or Event. What is the name of the card being discarded?", h.Name());
                        while (itemDiscarded == Item.None && eventDiscarded == Event.None)
                        {
                            line = Console.ReadLine();
                            itemDiscarded = Enumerations.GetItemFromString(line);
                            eventDiscarded = Enumerations.GetEventFromString(line);
                        }
                        if (itemDiscarded != Item.None)
                        {
                            game.Hunters[(int)h].DiscardItem(game, itemDiscarded);
                        }
                        else
                        {
                            game.Hunters[(int)h].DiscardEvent(game, eventDiscarded);
                        }
                    }
                    else
                    {
                        Console.WriteLine("{0} must discard 1 Item. What is the name of the card being discarded?",
                            h.Name());
                        while (itemDiscarded == Item.None)
                        {
                            line = Console.ReadLine();
                            itemDiscarded = Enumerations.GetItemFromString(line);
                        }
                        game.Hunters[(int)h].DiscardItem(game, itemDiscarded);
                    }
                }
                else if (game.Hunters[(int)h].EventCount > 0)
                {
                    Console.WriteLine("{0} must discard 1 Event. What is the name of the card being discarded?",
                        h.Name());
                    while (eventDiscarded == Event.None)
                    {
                        line = Console.ReadLine();
                        eventDiscarded = Enumerations.GetEventFromString(line);
                    }
                    game.Hunters[(int)h].DiscardEvent(game, eventDiscarded);
                }
            }
            return false;
        }

        /// <summary>
        /// Resolves an Encounter of Rats for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the Encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveRats(GameState game, List<Hunter> huntersInvolved)
        {
            foreach (var h in huntersInvolved)
            {
                if (game.Hunters[(int)h].HasDogsFaceUp)
                {
                    return true;
                }
                Console.WriteLine("How much health does {0} now have?");
                var amount = -1;
                while (amount == -1)
                {
                    if (int.TryParse(Console.ReadLine(), out amount))
                    {
                        game.Hunters[(int)h].AdjustHealth(amount - game.Hunters[(int)h].Health);
                        break;
                    }
                }
            }
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Resolves an Encounter of Plague for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the Encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolvePlague(GameState game, List<Hunter> huntersInvolved)
        {
            foreach (var h in huntersInvolved)
            {
                game.Hunters[(int)h].AdjustHealth(-2);
            }
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Resolves an Encounter of Peasants for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the Encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolvePeasants(GameState game, List<Hunter> huntersInvolved)
        {
            if (!game.Map.IsEastern(game.Hunters[(int)huntersInvolved.First()].CurrentLocation))
            {
                foreach (var h in huntersInvolved)
                {
                    if (game.Hunters[(int)h].ItemCount > 0)
                    {
                        Console.WriteLine("What Item is {0} discarding?", h.Name());
                        var itemDiscarded = Item.None;
                        while (itemDiscarded == Item.None)
                        {
                            itemDiscarded = Enumerations.GetItemFromString(Console.ReadLine());
                        }
                        game.Hunters[(int)h].DiscardItem(game, itemDiscarded);
                        Console.WriteLine("{0} discarded. Draw another Item card.", itemDiscarded.Name());
                        game.Hunters[(int)h].DrawItemCard();
                        CheckForCardsRevealedForBeingBitten(game);
                    }
                }
            }
            else
            {
                foreach (var h in huntersInvolved)
                {
                    int itemCountBefore = game.Hunters[(int)h].ItemCount;
                    while (game.Hunters[(int)h].ItemCount > 0)
                    {
                        Console.WriteLine("What Item is {0} discarding?", h.Name());
                        var itemDiscarded = Item.None;
                        while (itemDiscarded == Item.None)
                        {
                            itemDiscarded = Enumerations.GetItemFromString(Console.ReadLine());
                        }
                        game.Hunters[(int)h].DiscardItem(game, itemDiscarded);
                        Console.WriteLine("{0} discarded.", itemDiscarded.Name());
                    }
                    Console.WriteLine("Redraw {0} Item cards from the Item deck when this encounter is over.", itemCountBefore);
                    game.Hunters[(int)h].DrawItemCard();
                    CheckForCardsRevealedForBeingBitten(game);
                }
            }
            return true;
        }

        /// <summary>
        /// Resolves an Encounter of Lightning for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the Encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveLightning(GameState game, List<Hunter> huntersInvolved)
        {
            var answer = 0;
            foreach (var h in huntersInvolved)
            {
                Console.WriteLine("What Item does {0} have? 0= Nothing, 1= Crucifix, 2= Heavenly Host", h.Name());
                if (int.TryParse(Console.ReadLine(), out answer) && answer > 0 && answer < 3)
                {
                    if (answer == 1)
                    {
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], Item.Crucifix);
                        break;
                    }
                    if (answer == 2)
                    {
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], Item.HeavenlyHost);
                        break;
                    }
                }
            }
            if (answer == 0)
            {
                foreach (var h in huntersInvolved)
                {
                    game.Hunters[(int)h].AdjustHealth(-2);
                    if (game.Hunters[(int)h].ItemCount > 0)
                    {
                        Console.WriteLine("What Item is {0} discarding?", h.Name());
                        var itemDiscarded = Item.None;
                        while (itemDiscarded == Item.None)
                        {
                            itemDiscarded = Enumerations.GetItemFromString(Console.ReadLine());
                        }
                        game.Hunters[(int)h].DiscardItem(game, itemDiscarded);
                        Console.WriteLine("{0} discarded", itemDiscarded.Name());
                        CheckForCardsRevealedForBeingBitten(game);
                    }
                }
            }
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Resolves an encounter of a Hoax for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the Encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving Encounters</returns>
        private static bool ResolveHoax(GameState game, List<Hunter> huntersInvolved)
        {
            foreach (var h in huntersInvolved)
            {
                if (game.Map.IsEastern(game.Hunters[(int)h].CurrentLocation) && game.Hunters[(int)h].EventCount > 0)
                {
                    Console.WriteLine("{0} must discard an Event. What Event is being discarded?", h.Name());
                    var line = "";
                    var eventToDiscard = Event.None;
                    while (eventToDiscard == Event.None)
                    {
                        line = Console.ReadLine();
                        eventToDiscard = Enumerations.GetEventFromString(line);
                    }
                    game.Hunters[(int)h].DiscardEvent(game, eventToDiscard);
                }
                else
                {
                    Console.WriteLine("{0} must discard all Events.", h.Name());
                    while (game.Hunters[(int)h].EventCount > 0)
                    {
                        Console.WriteLine("What Event is being discarded?");
                        var line = "";
                        var eventToDiscard = Event.None;
                        while (eventToDiscard == Event.None)
                        {
                            line = Console.ReadLine();
                            eventToDiscard = Enumerations.GetEventFromString(line);
                        }
                        game.Hunters[(int)h].DiscardEvent(game, eventToDiscard);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Resolves an encounter of a Minion With Knife And Rifle for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the combat</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveMinionWithKnifeAndRifle(GameState game, List<Hunter> huntersInvolved,
            DecisionMaker logic)
        {
            var huntersInCombat = new List<HunterPlayer>();
            foreach (var h in huntersInvolved)
            {
                huntersInCombat.Add(game.Hunters[(int)h]);
            }
            var continueEncounters = ResolveCombat(game, huntersInCombat, Opponent.MinionWithKnifeAndRifle, logic);
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            if (continueEncounters &&
                DraculaIsPlayingRelentlessMinion(game, huntersInvolved, Opponent.MinionWithKnifeAndRifle, logic))
            {
                return ResolveMinionWithKnifeAndRifle(game, huntersInvolved, logic);
            }
            return continueEncounters;
        }

        /// <summary>
        /// Determines if Dracula is playing Relentless Minion
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">The List of Hunters involved in the Encounter</param>
        /// <param name="opponent">The Opponent type</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if Dracula successfully plays Relentless Minion</returns>
        private static bool DraculaIsPlayingRelentlessMinion(GameState game, List<Hunter> huntersInvolved,
            Opponent opponent, DecisionMaker logic)
        {
            if (logic.ChooseToPlayRelentlessMinion(game, huntersInvolved, opponent))
            {
                Console.WriteLine("Dracula is playing Relentless Minion");
                game.Dracula.DiscardEvent(Event.RelentlessMinion, game.EventDiscard);
                if (
                    HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.RelentlessMinion, Event.RelentlessMinion,
                        logic) > 0)
                {
                    Console.WriteLine("Relentless Minion cancelled");
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resolves an Encounter of a Minion With Knife And Pistol for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the combat</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveMinionWithKnifeAndPistol(GameState game, List<Hunter> huntersInvolved,
            DecisionMaker logic)
        {
            var huntersInCombat = new List<HunterPlayer>();
            foreach (var h in huntersInvolved)
            {
                huntersInCombat.Add(game.Hunters[(int)h]);
            }
            var continueEncounters = ResolveCombat(game, huntersInCombat, Opponent.MinionWithKnifeAndPistol, logic);
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            if (continueEncounters &&
                DraculaIsPlayingRelentlessMinion(game, huntersInvolved, Opponent.MinionWithKnifeAndPistol, logic))
            {
                return ResolveMinionWithKnifeAndPistol(game, huntersInvolved, logic);
            }
            return continueEncounters;
        }

        /// <summary>
        /// Resolves an Encounter of a Minion With Knife for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the combat</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveMinionWithKnife(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            var huntersInCombat = new List<HunterPlayer>();
            foreach (var h in huntersInvolved)
            {
                huntersInCombat.Add(game.Hunters[(int)h]);
            }
            var continueEncounters = ResolveCombat(game, huntersInCombat, Opponent.MinionWithKnife, logic);
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            if (continueEncounters &&
                DraculaIsPlayingRelentlessMinion(game, huntersInvolved, Opponent.MinionWithKnife, logic))
            {
                return ResolveMinionWithKnife(game, huntersInvolved, logic);
            }
            return continueEncounters;
        }

        /// <summary>
        /// Resolves an Encounter of Desecrated Soil for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the Encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveDesecratedSoil(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            Console.WriteLine("Draw an Event card. If it is a Hunter card, name it, otherwise type \"Dracula\"");
            var line = "";
            var eventDrawn = Event.None;
            do
            {
                line = Console.ReadLine();
                eventDrawn = Enumerations.GetEventFromString(line);
            } while (eventDrawn == Event.None && !"dracula".StartsWith(line.ToLower()));
            if (line.ToLower() == "dracula")
            {
                var eventPlayedByDracula = game.Dracula.TakeEvent(game.EventDeck, game.EventDiscard);
                PlayImmediatelyDraculaCard(game, eventPlayedByDracula, logic);
                if (game.Dracula.EventHand.Count() > game.Dracula.EventHandSize)
                {
                    Console.WriteLine("Dracula disacarded {0}", game.Dracula.DiscardEvent(logic.ChooseEventToDiscard(game), game.EventDiscard).Name());
                }
            }
            else
            {
                var eventCardDiscarded = game.EventDeck.Find(card => card.Event == eventDrawn);
                game.EventDeck.Remove(eventCardDiscarded);
                game.EventDiscard.Add(eventCardDiscarded);
            }
            return true;
        }

        /// <summary>
        /// Resolves an Encounter of Assassin for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the combat</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveAssassin(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            var huntersInCombat = new List<HunterPlayer>();
            foreach (var h in huntersInvolved)
            {
                huntersInCombat.Add(game.Hunters[(int)h]);
            }
            var continueEncounters = ResolveCombat(game, huntersInCombat, Opponent.Assassin, logic);
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            return continueEncounters;
        }

        /// <summary>
        /// Resolves an Encounter of Ambush for a group of Hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the Encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveAmbush(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            game.Dracula.DrawEncounter(game.EncounterPool);
            game.Dracula.DiscardEncounterTile(game, logic.ChooseEncounterTileToDiscardFromEncounterHand(game));
            return true;
        }

        /// <summary>
        /// Checks if any of the Hunters should discard a card
        /// </summary>
        /// <param name="game">The GameState</param>
        private static void CheckForDiscardRequired(GameState game)
        {
            for (var i = 1; i < 5; i++)
            {
                if (game.Hunters[i].Hunter == Hunter.DrSeward)
                {
                    if (game.Hunters[i].ItemCount > 3 && game.Hunters[i].EventCount > 3)
                    {
                        Console.WriteLine("{0} must discard an Item or an Event", game.Hunters[i].Hunter.Name());
                    }
                    else
                    {
                        if (game.Hunters[i].ItemCount > 4)
                        {
                            Console.WriteLine("{0} must discard an Item", game.Hunters[i].Hunter.Name());
                        }
                        if (game.Hunters[i].EventCount > 4)
                        {
                            Console.WriteLine("{0} must discard an Event", game.Hunters[i].Hunter.Name());
                        }
                    }
                }
                else
                {
                    if (game.Hunters[i].ItemCount > 3)
                    {
                        Console.WriteLine("{0} must discard an Item", game.Hunters[i].Hunter.Name());
                    }
                    if (game.Hunters[i].EventCount > 3)
                    {
                        Console.WriteLine("{0} must discard an Event", game.Hunters[i].Hunter.Name());
                    }
                }
            }
        }

        /// <summary>
        /// Reveals a card and its Encounters at a given position, for debugging
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="position">1-6 for positions in the trail, 7-9 for the three catacombs positions</param>
        private static void RevealCardInTrailAtPosition(GameState game, string position)
        {
            int index;
            if (!int.TryParse(position, out index))
            {
                var line = "";
                Console.WriteLine("What position do you want to reveal?");
                var successful = false;
                do
                {
                    line = Console.ReadLine();
                    successful = int.TryParse(line, out index);
                } while (!successful);
            }
            if (!game.Dracula.RevealCardAtPosition(index - 1))
            {
                Console.WriteLine("Cannot reveal card at position {0}", index);
            }
            else
            {
                game.Dracula.RevealEncountersAtPositionInTrail(index - 1);
            }
        }

        /// <summary>
        /// Allows Dracula to take his turn
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void EndHunterTurn(GameState game, DecisionMaker logic)
        {
            game.AdvanceTimeTracker();

            var catacombsSlotsCleared = logic.ChooseWhichCatacombsCardsToDiscard(game);
            if (catacombsSlotsCleared.Count() > 0)
            {
                Console.Write("Dracula discarded cards from Catacombs positions: ");
                foreach (var i in catacombsSlotsCleared)
                {
                    Console.Write("{0} ", i + 1);
                }
                Console.WriteLine("");
                game.Dracula.DiscardCatacombsCards(game, catacombsSlotsCleared);
            }
            if (game.DraculaAlly != null && game.DraculaAlly.Event == Event.QuinceyPMorris)
            {
                var victim = logic.ChooseVictimForQuinceyPMorris(game);
                Console.WriteLine("Dracula is targetting {0} with Quincey P. Morris", victim.Name());
                Console.WriteLine("What Item does {0} have? 0= Nothing, 1= Crucifix, 2= Heavenly Host", victim.Name());
                var answer = -1;
                while (answer == -1)
                {
                    if (int.TryParse(Console.ReadLine(), out answer))
                    {
                        if (answer < 0 || answer > 2)
                        {
                            answer = -1;
                        }
                    }
                }
                switch (answer)
                {
                    case 0:
                        Console.WriteLine("{0} loses 1 health");
                        game.Hunters[(int)victim].AdjustHealth(-1);
                        CheckForHunterDeath(game); break;
                    case 1:
                        Console.WriteLine("Health loss cancelled");
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)victim], Item.Crucifix); break;
                    case 2:
                        Console.WriteLine("Health loss cancelled");
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)victim], Item.HeavenlyHost); break;
                }
            }
            var eventPlayed = Event.None;
            var numberOfMoves = 1;
            do
            {
                var devilishPowerTarget = DevilishPowerTarget.None;
                var roadBlock1 = Location.Nowhere;
                var roadBlock2 = Location.Nowhere;
                var roadBlockType = ConnectionType.None;
                eventPlayed = logic.ChooseEventCardToPlayAtStartOfDraculaTurn(game, out devilishPowerTarget,
                    out roadBlock1, out roadBlock2, out roadBlockType);
                if (eventPlayed != Event.None)
                {
                    Console.WriteLine("Dracula played {0}", eventPlayed.Name());
                    game.Dracula.DiscardEvent(eventPlayed, game.EventDiscard);
                    if (HunterPlayingGoodLuckToCancelDraculaEvent(game, eventPlayed, eventPlayed, logic) > 0)
                    {
                        Console.WriteLine("{0} cancelled", eventPlayed.Name());
                    }
                    else
                    {
                        switch (eventPlayed)
                        {
                            case Event.DevilishPower:
                                switch (devilishPowerTarget)
                                {
                                    case DevilishPowerTarget.HeavenlyHost1:
                                        Console.WriteLine("Heavenly Host discarded from {0}", game.HeavenlyHostLocation1);
                                        game.HeavenlyHostLocation1 = Location.Nowhere; break;
                                    case DevilishPowerTarget.HeavenlyHost2:
                                        Console.WriteLine("Heavenly Host discarded from {0}", game.HeavenlyHostLocation2);
                                        game.HeavenlyHostLocation2 = Location.Nowhere; break;
                                    case DevilishPowerTarget.HunterAlly:
                                        Console.WriteLine("{0} discarded from play", game.HunterAlly.Event.Name());
                                        game.EventDiscard.Add(game.HunterAlly);
                                        game.HunterAlly = null; break;
                                } break;
                            case Event.UnearthlySwiftness:
                                numberOfMoves = 2; break;
                            case Event.TimeRunsShort:
                                game.RegressTimeTracker(); break;
                            case Event.Roadblock:
                                game.RoadBlockLocation1 = roadBlock1;
                                game.RoadBlockLocation2 = roadBlock2;
                                game.RoadBlockConnectionType = roadBlockType; break;
                        }
                    }
                }
            } while (eventPlayed != Event.None);

            var power = Power.None;
            Location destination;
            var cardsDroppedOffTrail = new List<DraculaCardSlot>();
            for (var i = 0; i < numberOfMoves; i++)
            {
                destination = logic.ChooseDestinationAndPower(game, out power);
                if (destination == Location.Nowhere && power == Power.None)
                {
                    Console.WriteLine("Dracula is cornered by his own trail and has no valid moves");
                    game.Dracula.TakePunishmentForCheating(game);
                    return;
                }
                int doubleBackSlot = -1;
                var cardDroppedOffTrail = game.Dracula.MoveTo(destination, power, out doubleBackSlot);
                if (doubleBackSlot > -1)
                {
                    Console.WriteLine("Dracula Doubled Back to the location in slot {0}", doubleBackSlot);
                }
                if (cardDroppedOffTrail != null)
                {
                    cardsDroppedOffTrail.Add(cardDroppedOffTrail);
                }
                if (game.Map.TypeOfLocation(destination) == LocationType.Sea)
                {
                    if ((!game.Dracula.LostBloodFromSeaMovementLastTurn || game.HunterAlly != null && game.HunterAlly.Event == Event.RufusSmith))
                    {
                        game.Dracula.AdjustBlood(-1);
                        game.Dracula.LostBloodFromSeaMovementLastTurn = true;
                    }
                    else
                    {
                        game.Dracula.LostBloodFromSeaMovementLastTurn = false;
                    }
                }
            }
            if (game.Map.TypeOfLocation(game.Dracula.CurrentLocation) == LocationType.Sea || (game.Dracula.CurrentLocation != game.Hunters[(int)Hunter.LordGodalming].CurrentLocation &&
                game.Dracula.CurrentLocation != game.Hunters[(int)Hunter.DrSeward].CurrentLocation &&
                game.Dracula.CurrentLocation != game.Hunters[(int)Hunter.VanHelsing].CurrentLocation &&
                game.Dracula.CurrentLocation != game.Hunters[(int)Hunter.MinaHarker].CurrentLocation))
            {
                switch (power)
                {
                    case Power.Hide:
                    case Power.None:
                    case Power.WolfForm:
                        game.Dracula.PlaceEncounterTileOnCard(logic.ChooseEncounterTileToPlaceOnDraculaCardSlot(game, game.Dracula.Trail[0]), game.Dracula.Trail[0]); break;
                    case Power.DoubleBack:
                        var encounterTileToDiscard = logic.ChooseEncounterTileToDiscardFromDoubleBackedCatacombsLocation(game);
                        game.Dracula.DiscardEncounterTileFromCardSlot(encounterTileToDiscard, game.Dracula.Trail[0], game.EncounterPool); break;
                    case Power.Feed:
                        game.Dracula.AdjustBlood(1); break;
                    case Power.DarkCall:
                        game.Dracula.AdjustBlood(-2);
                        for (int i = 0; i < 10; i++)
                        {
                            game.Dracula.DrawEncounter(game.EncounterPool);
                        }
                        while (game.Dracula.EncounterHand.Count() > game.Dracula.EncounterHandSize)
                        {
                            game.Dracula.DiscardEncounterTile(game, logic.ChooseEncounterTileToDiscardFromEncounterHand(game));
                        } break;
                }
            }
            else
            {
                game.Dracula.Trail[0].DraculaCards.First().IsRevealed = true;
                DrawGameState(game);
                var huntersAttacked = new List<HunterPlayer>();
                foreach (var h in game.Hunters)
                {
                    if (h != null && h.CurrentLocation == game.Dracula.CurrentLocation)
                    {
                        huntersAttacked.Add(h);
                    }
                }
                Console.WriteLine("Dracula attacks {0}{1}!", huntersAttacked.First().Hunter.Name(), huntersAttacked.Count > 1 ? " et al" : "");
                ResolveCombat(game, huntersAttacked, Opponent.Dracula, logic);
            }
            if (game.Dracula.CurrentLocation == game.Dracula.LocationWhereHideWasUsed && power == Power.DoubleBack &&
                game.Dracula.LocationWhereHideWasUsed != Location.Nowhere)
            {
                game.Dracula.RevealHideCard();
                Console.WriteLine("Dracula used Double Back to return to the location where he previously used Hide");
            }
            DealWithDroppedOffCardSlots(game, cardsDroppedOffTrail, logic);
            while (game.Dracula.EncounterHand.Count() < game.Dracula.EncounterHandSize)
            {
                game.Dracula.DrawEncounter(game.EncounterPool);
            }
        }

        /// <summary>
        /// Draws the gamestate on the screen
        /// </summary>
        /// <param name="game">The GameState</param>
        private static void DrawGameState(GameState game)
        {
            // line 1
            Console.WriteLine("Trail                       Catacombs       Time          Vampires   Resolve   Dracula Ally   Hunter Ally");
            // line 2
            for (var i = 5; i >= 0; i--)
            {
                if (game.Dracula.Trail[i] != null)
                {
                    Console.ForegroundColor = game.Dracula.Trail[i].DraculaCards.First().Color;
                    if (game.Dracula.Trail[i].DraculaCards.First().IsRevealed)
                    {
                        Console.Write(game.Dracula.Trail[i].DraculaCards.First().Abbreviation + " ");
                    }
                    else
                    {
                        Console.Write("### ");
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }
            Console.Write("    ");
            for (var i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null)
                {
                    Console.ForegroundColor = game.Dracula.Catacombs[i].DraculaCards.First().Color;
                    if (game.Dracula.Catacombs[i].DraculaCards.First().IsRevealed)
                    {
                        Console.Write(game.Dracula.Catacombs[i].DraculaCards.First().Abbreviation + " ");
                    }
                    else
                    {
                        Console.Write("### ");
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }
            switch (game.TimeOfDay)
            {
                case TimeOfDay.Dawn:
                case TimeOfDay.Dusk:
                    Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case TimeOfDay.Noon:
                    Console.ForegroundColor = ConsoleColor.Yellow; break;
                case TimeOfDay.Midnight:
                    Console.ForegroundColor = ConsoleColor.Blue; break;
                case TimeOfDay.Twilight:
                case TimeOfDay.SmallHours:
                    Console.ForegroundColor = ConsoleColor.DarkBlue; break;
            }
            Console.Write("    {0}", game.TimeOfDay.Name());
            Console.ResetColor();
            for (var i = game.TimeOfDay.Name().Length; i < 14; i++)
            {
                Console.Write(" ");
            }
            Console.Write(game.Vampires);
            for (var i = game.Vampires.ToString().Length; i < 11; i++)
            {
                Console.Write(" ");
            }
            Console.Write(game.Resolve);
            for (var i = game.Resolve.ToString().Length; i < 11; i++)
            {
                Console.Write(" ");
            }
            if (game.DraculaAlly != null)
            {
                Console.Write(game.DraculaAlly.Event.Name().Substring(0, 3).ToUpper());
            }
            else
            {
                Console.Write("   ");
            }
            Console.Write("           ");
            if (game.HunterAlly != null)
            {
                Console.Write(game.HunterAlly.Event.Name().Substring(0, 3).ToUpper());
            }
            Console.WriteLine("");
            // line 3
            for (var i = 5; i >= 0; i--)
            {
                if (game.Dracula.Trail[i] != null && game.Dracula.Trail[i].DraculaCards.Count() > 1)
                {
                    Console.ForegroundColor = game.Dracula.Trail[i].DraculaCards[1].Color;
                    if (game.Dracula.Trail[i].DraculaCards[1].IsRevealed)
                    {
                        Console.Write(game.Dracula.Trail[i].DraculaCards[1].Abbreviation + " ");
                    }
                    else
                    {
                        Console.Write("### ");
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }
            Console.Write("    ");
            for (var i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null && game.Dracula.Catacombs[i].EncounterTiles.Count() > 0)
                {
                    if (game.Dracula.Catacombs[i].EncounterTiles.First().IsRevealed)
                    {
                        Console.ResetColor();
                        Console.Write(game.Dracula.Catacombs[i].EncounterTiles.First().Abbreviation + " ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(" ■  ");
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }
            Console.WriteLine("");
            // line 4
            for (var i = 5; i >= 0; i--)
            {
                if (game.Dracula.Trail[i] != null && game.Dracula.Trail[i].EncounterTiles.Count() > 0)
                {
                    if (game.Dracula.Trail[i].EncounterTiles.First().IsRevealed)
                    {
                        Console.ResetColor();
                        Console.Write(game.Dracula.Trail[i].EncounterTiles.First().Abbreviation + " ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(" ■  ");
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }
            Console.Write("    ");
            for (var i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null && game.Dracula.Catacombs[i].EncounterTiles.Count() > 1)
                {
                    if (game.Dracula.Catacombs[i].EncounterTiles[1].IsRevealed)
                    {
                        Console.ResetColor();
                        Console.Write(game.Dracula.Catacombs[i].EncounterTiles[1].Abbreviation + " ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(" ■  ");
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }
            Console.ResetColor();
            Console.Write("    Dracula Blood   Dracula Events");
            Console.WriteLine();
            // line 5
            Console.Write("                                            {0}              {1}", game.Dracula.Blood, game.Dracula.Blood > 9 ? game.Dracula.EventHand.Count().ToString() : (" " + game.Dracula.EventHand.Count()));
            Console.WriteLine();
            Console.ResetColor();
        }

        /// <summary>
        /// Provides details about various aspects of the gamestate that are not otherwise displayed on the screen
        /// </summary>
        /// <param name="game">The GameState</param>
        private static void DisplayState(GameState game, string argument1)
        {
            Console.WriteLine("The state of the game:");
            foreach (var h in game.Hunters)
            {
                if (h != null)
                {
                    Console.WriteLine("{0} is in {1} with {2} Items and {3} Events, on {4} health with {5} bites{6}",
                        h.Hunter.Name(), h.CurrentLocation.Name(), h.ItemCount, h.EventCount, h.Health, h.BiteCount, h.HasDogsFaceUp ? " and has Dogs face up" : "");
                    if (h.EncountersInFrontOfPlayer.Any())
                    {
                        Console.Write("These Encounter tiles are in front of {0}: ", h.Hunter.Name());
                        foreach (EncounterTile enc in h.EncountersInFrontOfPlayer)
                        {
                            Console.Write("{0}, ", enc.Encounter.Name());
                        }
                        Console.WriteLine("");
                    }
                    if (h.ItemsKnownToDracula.Any() && argument1 == "debug")
                    {
                        Console.Write("These Items held by {0} are known to Dracula: ", h.Hunter.Name());
                        foreach (var i in h.ItemsKnownToDracula)
                        {
                            Console.Write("{0}, ", i.Item.Name());
                        }
                        Console.WriteLine("");
                    }
                    if (h.EventsKnownToDracula.Any() && argument1 == "debug")
                    {
                        Console.Write("These Events held by {0} are known to Dracula: ", h.Hunter.Name());
                        foreach (var e in h.EventsKnownToDracula)
                        {
                            Console.Write("{0}, ", e.Event.Name());
                        }
                        Console.WriteLine("");
                    }
                }
            }
            if (argument1 == "debug")
            {
                Console.WriteLine("Dracula is in {0} with {1} blood and has {2} Events", game.Dracula.CurrentLocation.Name(),
                    game.Dracula.Blood, game.Dracula.EventHand.Count());

                Console.Write("Dracula is holding these Encounter tiles: ");
                foreach (var enc in game.Dracula.EncounterHand)
                {
                    Console.Write("{0} ", enc.Encounter.Name());
                }
                if (game.Dracula.EventHand.Count() > 0)
                {
                    Console.Write("Dracula is holding these Event cards: ");
                    foreach (var e in game.Dracula.EventHand)
                    {
                        Console.Write("{0} ", e.Event.Name());
                    }
                }
            }
            Console.WriteLine("");
            Console.WriteLine("These Items are in the discard:");
            foreach (var i in game.ItemDiscard)
            {
                Console.WriteLine(i.Item.Name());
            }
            Console.WriteLine("These Events are in the discard:");
            foreach (var e in game.EventDiscard)
            {
                Console.WriteLine(e.Event.Name());
            }
        }

        /// <summary>
        /// Discards a card of a given name from the given Hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="cardName">The name of the card (Item or Event) to discard</param>
        /// <param name="hunterIndex">A string to be converted to the Hunter discarding the card</param>
        private static void DiscardCard(GameState game, string cardName, string hunterIndex)
        {
            var index = 0;
            var hunterToDiscard = Hunter.Nobody;
            if (int.TryParse(hunterIndex, out index) && index > 0 && index < 5)
            {
                hunterToDiscard = (Hunter)index;
            }
            var line = "";
            while (hunterToDiscard == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is discarding a card? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)",
                    (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward,
                    Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker,
                    Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (int.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    hunterToDiscard = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterToDiscard.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            var itemToDiscard = Enumerations.GetItemFromString(cardName);
            var eventToDiscard = Enumerations.GetEventFromString(cardName);
            while (itemToDiscard == Item.None && eventToDiscard == Event.None && line.ToLower() != "cancel")
            {
                Console.WriteLine("What is the name of the card you are discarding? (type cancel to cancel)");
                line = Console.ReadLine().ToLower();
                if (line.ToLower() == "cancel")
                {
                    Console.WriteLine("Cancelled");
                    return;
                }

                itemToDiscard = Enumerations.GetItemFromString(line);
                eventToDiscard = Enumerations.GetEventFromString(line);
                if (itemToDiscard == Item.None && eventToDiscard == Event.None)
                {
                    Console.WriteLine("I couldn't find any card with that name");
                }
                else if (itemToDiscard != Item.None && eventToDiscard != Event.None)
                {
                    Console.WriteLine("I couldn't tell if you meant an Item or an Event card");
                }
            }
            if (itemToDiscard != Item.None)
            {
                game.Hunters[index].DiscardItem(game, itemToDiscard);
                Console.WriteLine("{0} discarded {1}, down to {2}", hunterToDiscard.Name(), itemToDiscard.Name(),
                    game.Hunters[index].ItemCount);
            }
            else if (eventToDiscard != Event.None)
            {
                game.Hunters[index].DiscardEvent(game, eventToDiscard);
                Console.WriteLine("{0} discarded {1}, down to {2}", hunterToDiscard.Name(), eventToDiscard.Name(),
                    game.Hunters[index].EventCount);
            }
            CheckForCardsRevealedForBeingBitten(game);
        }

        /// <summary>
        /// Adds 1 to the count of cards of the given type to the given Hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="cardType">Item or Event</param>
        /// <param name="hunterIndex">A string to be converted to the Hunter drawing the card</param>
        private static void DrawCard(GameState game, string cardType, string hunterIndex)
        {
            var index = 0;
            var hunterToDraw = Hunter.Nobody;
            if (int.TryParse(hunterIndex, out index))
            {
                hunterToDraw = game.GetHunterFromInt(index);
            }
            var line = "";
            while (hunterToDraw == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is drawing a card? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)",
                    (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward,
                    Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker,
                    Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (int.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    hunterToDraw = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterToDraw.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            do
            {
                switch (cardType)
                {
                    case "i":
                    case "item":
                        game.Hunters[index].DrawItemCard();
                        Console.WriteLine("{0} drew an Item card, up to {1}", hunterToDraw.Name(), game.Hunters[index].ItemCount); return;
                    case "e":
                    case "event":
                        game.Hunters[index].DrawEventCard();
                        Console.WriteLine("{0} drew an Event card, up to {1}", hunterToDraw.Name(), game.Hunters[index].EventCount); return;
                    default:
                        Console.WriteLine("What type of card is {0} drawing?", hunterToDraw.Name());
                        cardType = Console.ReadLine().ToLower(); break;
                }
            } while (cardType != "cancel");
            Console.WriteLine("Cancelled");
        }

        /// <summary>
        /// Moves the given Hunter to the given Location, along with all Hunters in the given Hunter's group
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterIndex">A string to be converted to the Hunter discarding the card</param>
        /// <param name="location">The name of the Location to move to</param>
        /// <returns>The position of the card in Dracula's trail (0-5) or catacombs (6-8) that corresponds to the given location, or -1 if not in the trail/catacombs</returns>
        private static int MoveHunter(GameState game, string hunterIndex, string location, out Hunter hunterMoved,
            out Location originatingLocation, DecisionMaker logic)
        {
            var hunterToMove = Hunter.Nobody;
            int index;
            if (int.TryParse(hunterIndex, out index))
            {
                hunterToMove = game.GetHunterFromInt(index);
            }
            var line = "";
            while (hunterToMove == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is moving? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)",
                    (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward,
                    Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker,
                    Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (int.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        hunterMoved = Hunter.Nobody;
                        originatingLocation = Location.Nowhere;
                        return -1;
                    }
                    hunterToMove = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterToMove.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            hunterMoved = (Hunter)index;
            originatingLocation = game.Hunters[(int)hunterMoved].CurrentLocation;
            Location destination;
            if (DraculaIsPlayingControlStorms(game, hunterMoved, logic))
            {
                Console.WriteLine("Dracula is controlling the ship's movement");
                destination = logic.ChoosePortToMoveHuntersToWithControlStorms(game, hunterMoved);
            }
            else
            {
                destination = Enumerations.GetLocationFromString(location);
                while (destination == Location.Nowhere && line.ToLower() != "cancel")
                {
                    Console.WriteLine("Where is {0} moving? (Type cancel to cancel)", hunterToMove.Name());
                    line = Console.ReadLine();
                    destination = Enumerations.GetLocationFromString(line);
                    Console.WriteLine(destination.Name());
                }
                if (line.ToLower() == "cancel")
                {
                    Console.WriteLine("Cancelled");
                    return -1;
                }
            }
            Console.Write("{0} moved from {1} to ", hunterToMove.Name(),
                game.Hunters[(int)hunterToMove].CurrentLocation.Name());
            foreach (var h in game.Hunters[(int)hunterToMove].HuntersInGroup)
            {
                game.Hunters[(int)h].MoveTo(destination);
            }
            Console.WriteLine(destination.Name() +
                              (game.Hunters[(int)hunterToMove].HuntersInGroup.Count() > 1 ? " with his group" : ""));
            for (var i = 0; i < 6; i++)
            {
                if (game.Dracula.Trail[i] != null && game.Dracula.Trail[i].DraculaCards.First().Location == destination)
                {
                    return i;
                }
            }
            for (var i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null &&
                    game.Dracula.Catacombs[i].DraculaCards.First().Location == destination)
                {
                    return i + 6;
                }
            }
            return -1;
        }

        /// <summary>
        /// Determines if Dracula is playing Control Storms at the start of a Hunter's movement
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterMoved">The Hunter moving</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if Dracula successfully played Control Storms</returns>
        private static bool DraculaIsPlayingControlStorms(GameState game, Hunter hunterMoved, DecisionMaker logic)
        {
            if (logic.ChooseToPlayControlStorms(game, hunterMoved))
            {
                Console.WriteLine("Dracula is playing Control Storms to control your movement");
                game.Dracula.DiscardEvent(Event.ControlStorms, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.ControlStorms, Event.ControlStorms, logic) >
                    0)
                {
                    Console.WriteLine("Control Storms cancelled");
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Serialises and saves a GameState to a file with the given filename
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="fileName">The name of the file, without extension</param>
        private static void SaveGameState(GameState game, string fileName)
        {
            try
            {
                var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                foreach (var c in invalid)
                {
                    fileName = fileName.Replace(c.ToString(), "");
                }
                fileName = fileName + ".sav";
                var fileWriter = new DataContractSerializer(typeof(GameState));
                var writeStream = File.OpenWrite(fileName);

                fileWriter.WriteObject(writeStream, game);
                writeStream.Close();
                Console.WriteLine(fileName + " saved");
            }
            catch (Exception e)
            {
                Console.WriteLine("File could not be saved because " + e.Message);
            }
        }

        /// <summary>
        /// Used to capture multiple strings from a single user console input
        /// </summary>
        /// <returns>A CommandSet</returns>
        internal static CommandSet GetCommandSet()
        {
            string line;
            string command;
            string argument1 = "####";
            var argument2 = "####";

            line = Console.ReadLine().ToLower();
            try
            {
                command = line.Substring(0, line.IndexOf(' '));
            }
            catch (ArgumentOutOfRangeException)
            {
                command = line;
            }

            try
            {
                argument1 = line.Substring(line.IndexOf(' ') + 1);
                try
                {
                    argument2 = argument1.Substring(argument1.IndexOf(' ') + 1);
                    argument1 = argument1.Substring(0, argument1.IndexOf(' '));
                }
                catch (ArgumentOutOfRangeException)
                {
                    argument2 = "####";
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                argument1 = "####";
            }
            line = line.Replace('_', ' ');
            argument1 = argument1.Replace('_', ' ');
            argument2 = argument2.Replace('_', ' ');

            return new CommandSet(command.ToLower(), argument1.ToLower(), argument2.ToLower());
        }

        /// <summary>
        /// Resolves a combat between a group of Hunters and an Opponent
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of HunterPlayers involved in the combat</param>
        /// <param name="opponent">The Opponent type</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunters can continue resolving Encounters</returns>
        private static bool ResolveCombat(GameState game, List<HunterPlayer> huntersInvolved, Opponent opponent,
            DecisionMaker logic)
        {
            var roundsWithoutEscaping = 0;
            var answer = -1;
            do
            {
                Console.WriteLine(
                    "Is anyone playing an Event at the start of this combat? 0= Nobody, {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7}",
                    (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward,
                    Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker,
                    Hunter.MinaHarker.Name());
                while (answer == -1)
                {
                    if (int.TryParse(Console.ReadLine(), out answer))
                    {
                        if (answer < 0 || answer > 4)
                        {
                            answer = -1;
                        }
                    }
                }
                if (answer > 0)
                {
                    answer = -1;
                    var line = "";
                    Console.WriteLine("What Event is {0} playing? (cancel to cancel)", ((Hunter)answer).Name());
                    var eventBeingPlayed = Event.None;
                    while (eventBeingPlayed != Event.AdvancePlanning && eventBeingPlayed != Event.EscapeRoute &&
                           eventBeingPlayed != Event.HeroicLeap && line.ToLower() != "cancel")
                    {
                        line = Console.ReadLine();
                        eventBeingPlayed = Enumerations.GetEventFromString(line);
                    }
                    if (line.ToLower() == "cancel")
                    {
                        answer = -1;
                    }
                    else
                    {
                        switch (eventBeingPlayed)
                        {
                            case Event.AdvancePlanning:
                                game.Hunters[answer].DiscardEvent(game, Event.AdvancePlanning);
                                if (!DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.AdvancePlanning, Event.AdvancePlanning, logic))
                                {
                                    Console.WriteLine("One participant of your choice has combat rolls at +1 for this combat");
                                }
                                else
                                {
                                    Console.WriteLine("Advance Planning cancelled");
                                } break;
                            case Event.EscapeRoute:
                                game.Hunters[answer].DiscardEvent(game, Event.EscapeRoute);
                                if (!DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.EscapeRoute, Event.EscapeRoute, logic))
                                {
                                    Console.WriteLine("Combat cancelled", ((Hunter)answer).Name());
                                    return false;
                                }
                                Console.WriteLine("Escape Route cancelled"); break;
                            case Event.HeroicLeap:
                                game.Hunters[answer].DiscardEvent(game, Event.HeroicLeap);
                                if (!DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.HeroicLeap, Event.HeroicLeap, logic))
                                {
                                    Console.WriteLine("Roll a die and enter the result");
                                    var dieRoll = 0;
                                    while (dieRoll == 0)
                                    {
                                        if (int.TryParse(Console.ReadLine(), out dieRoll))
                                        {
                                            if (dieRoll < 1 || dieRoll > 7)
                                            {
                                                dieRoll = 0;
                                            }
                                        }
                                    }
                                    game.Hunters[answer].AdjustHealth(-dieRoll);
                                    game.Dracula.AdjustBlood(-dieRoll);
                                    CheckForHunterDeath(game);
                                    return false;
                                }
                                Console.WriteLine("Escape Route cancelled"); break;
                        }
                    }
                }
            } while (answer != 0);
            answer = -1;
            if (opponent == Opponent.Dracula || opponent == Opponent.NewVampire)
            {
                Console.WriteLine("Is anyone playing Garlic at the start of this combat? 0= Nobody, {0}{1}{2}{3}", huntersInvolved.Find(h => h.Hunter == Hunter.LordGodalming) == null ? "" : "1= Lord Godalming", huntersInvolved.Find(h => h.Hunter == Hunter.DrSeward) == null ? "" : "2= Dr. Seward", huntersInvolved.Find(h => h.Hunter == Hunter.VanHelsing) == null ? "" : "3= Van Helsing", huntersInvolved.Find(h => h.Hunter == Hunter.MinaHarker) == null ? "" : "4= Mina Harker");

                while (answer == -1)
                {
                    if (int.TryParse(Console.ReadLine(), out answer))
                    {
                        if (answer < 0 || answer > 4)
                        {
                            answer = -1;
                        }
                        else if (answer != 0 && huntersInvolved.Find(h => (int)h.Hunter == answer) == null)
                        {
                            answer = -1;
                        }
                    }
                    else
                    {
                        answer = -1;
                    }
                }
                if (answer > 0)
                {
                    game.Hunters[answer].DiscardItem(game, Item.Garlic);
                    roundsWithoutEscaping = 3;
                }
            }
            if (DraculaIsPlayingTrap(game, huntersInvolved, opponent, logic))
            {
                Console.WriteLine("Dracula played Trap. All of his combat rolls are at +1 for this combat");
            }
            var rageTarget = DraculaIsPlayingRageAgainstHunter(game, huntersInvolved, logic);
            if (rageTarget != Hunter.Nobody)
            {
                Console.WriteLine("Dracula played Rage against {0} and must reveal all Items", rageTarget.Name());
                DraculaLooksAtAllHuntersItems(game, game.Hunters[(int)rageTarget]);
                var itemDiscarded = logic.ChooseItemToDiscardWithRage(game, rageTarget);
                game.Hunters[(int)rageTarget].DiscardItem(game, itemDiscarded);
                Console.WriteLine("Dracula discarded {0}", itemDiscarded.Name());
                roundsWithoutEscaping = 3;
            }
            var basicHunterCombatCards = new List<ItemCard>
            {
                new ItemCard(Item.Punch),
                new ItemCard(Item.Dodge),
                new ItemCard(Item.Escape)
            };
            var enemyCombatCards = new List<EnemyCombatCard>();
            switch (opponent)
            {
                case Opponent.MinionWithKnife:
                    enemyCombatCards.Add(EnemyCombatCard.Punch);
                    enemyCombatCards.Add(EnemyCombatCard.Dodge);
                    enemyCombatCards.Add(EnemyCombatCard.Knife); break;
                case Opponent.MinionWithKnifeAndPistol:
                    enemyCombatCards.Add(EnemyCombatCard.Punch);
                    enemyCombatCards.Add(EnemyCombatCard.Dodge);
                    enemyCombatCards.Add(EnemyCombatCard.Knife);
                    enemyCombatCards.Add(EnemyCombatCard.Pistol); break;
                case Opponent.MinionWithKnifeAndRifle:
                    enemyCombatCards.Add(EnemyCombatCard.Punch);
                    enemyCombatCards.Add(EnemyCombatCard.Dodge);
                    enemyCombatCards.Add(EnemyCombatCard.Knife);
                    enemyCombatCards.Add(EnemyCombatCard.Rifle); break;
                case Opponent.Assassin:
                    enemyCombatCards.Add(EnemyCombatCard.Punch);
                    enemyCombatCards.Add(EnemyCombatCard.Dodge);
                    enemyCombatCards.Add(EnemyCombatCard.Knife);
                    enemyCombatCards.Add(EnemyCombatCard.Pistol);
                    enemyCombatCards.Add(EnemyCombatCard.Rifle); break;
                case Opponent.Dracula:
                    enemyCombatCards.Add(EnemyCombatCard.Claws);
                    enemyCombatCards.Add(EnemyCombatCard.Dodge);
                    enemyCombatCards.Add(EnemyCombatCard.EscapeMan);
                    switch (game.TimeOfDay)
                    {
                        case TimeOfDay.Twilight:
                        case TimeOfDay.Midnight:
                        case TimeOfDay.SmallHours:
                            enemyCombatCards.Add(EnemyCombatCard.EscapeBat);
                            enemyCombatCards.Add(EnemyCombatCard.EscapeMist);
                            enemyCombatCards.Add(EnemyCombatCard.Fangs);
                            enemyCombatCards.Add(EnemyCombatCard.Mesmerize);
                            enemyCombatCards.Add(EnemyCombatCard.Strength);
                            break;
                    } break;
                case Opponent.NewVampire:
                    enemyCombatCards.Add(EnemyCombatCard.Claws);
                    enemyCombatCards.Add(EnemyCombatCard.Dodge);
                    switch (game.TimeOfDay)
                    {
                        case TimeOfDay.Twilight:
                        case TimeOfDay.Midnight:
                        case TimeOfDay.SmallHours:
                            enemyCombatCards.Add(EnemyCombatCard.Fangs);
                            enemyCombatCards.Add(EnemyCombatCard.Mesmerize);
                            enemyCombatCards.Add(EnemyCombatCard.Strength);
                            break;
                    } break;
            }
            var firstRound = true;
            var repelled = false;
            var continueCombat = true;
            var sisterAgathaInEffect = (opponent == Opponent.Dracula && game.HunterAlly != null && game.HunterAlly.Event == Event.SisterAgatha);
            var enemyCombatCardChosen = EnemyCombatCard.None;
            var enemyTarget = Hunter.Nobody;
            while (continueCombat)
            {
                foreach (var h in huntersInvolved)
                {
                    var itemUsedByHunterLastRound = h.LastCombatCardChosen;
                    do
                    {
                        Console.WriteLine("Which combat card is {0} using this round?", h.Hunter.Name());
                        h.LastCombatCardChosen = Enumerations.GetItemFromString(Console.ReadLine());
                    } while (h.LastCombatCardChosen == Item.None);

                    if (basicHunterCombatCards.Find(card => card.Item == h.LastCombatCardChosen) == null &&
                        h.ItemsKnownToDracula.Find(item => item.Item == h.LastCombatCardChosen) == null)
                    {
                        var itemDraculaNowKnowsAbout = game.ItemDeck.Find(card => card.Item == h.LastCombatCardChosen);
                        h.ItemsKnownToDracula.Add(itemDraculaNowKnowsAbout);
                        game.ItemDeck.Remove(itemDraculaNowKnowsAbout);
                    }
                    else if (itemUsedByHunterLastRound == h.LastCombatCardChosen && basicHunterCombatCards.Find(card => card.Item == h.LastCombatCardChosen) == null)
                    {
                        var itemDraculaAlreadyKnewAbout =
                            h.ItemsKnownToDracula.Find(card => card.Item == itemUsedByHunterLastRound);
                        if (itemDraculaAlreadyKnewAbout != null)
                        {
                            h.ItemsKnownToDracula.Remove(itemDraculaAlreadyKnewAbout);
                        }
                        if (h.ItemsKnownToDracula.Find(card => card.Item == itemUsedByHunterLastRound) == null)
                        {
                            var itemDraculaNowKnowsAbout =
                                game.ItemDeck.Find(card => card.Item == h.LastCombatCardChosen);
                            h.ItemsKnownToDracula.Add(itemDraculaNowKnowsAbout);
                            game.ItemDeck.Remove(itemDraculaNowKnowsAbout);
                        }
                        h.ItemsKnownToDracula.Add(itemDraculaAlreadyKnewAbout);
                    }
                }
                enemyCombatCardChosen = logic.ChooseCombatCardAndTarget(game, huntersInvolved, enemyCombatCards,
                    firstRound, out enemyTarget, enemyCombatCardChosen, repelled, sisterAgathaInEffect,
                    roundsWithoutEscaping);
                roundsWithoutEscaping--;
                var bloodCost = false;
                if (sisterAgathaInEffect &&
                    (enemyCombatCardChosen == EnemyCombatCard.Fangs ||
                     enemyCombatCardChosen == EnemyCombatCard.EscapeBat ||
                     enemyCombatCardChosen == EnemyCombatCard.EscapeMan ||
                     enemyCombatCardChosen == EnemyCombatCard.EscapeMist))
                {
                    game.Dracula.AdjustBlood(-2);
                    bloodCost = true;
                }
                Console.WriteLine("{0} used {1} against {2}{3}", opponent.Name(), enemyCombatCardChosen.Name(),
                    enemyTarget.Name(), bloodCost ? " at a cost of 2 blood" : "");
                var line = "";
                var index = 0;
                do
                {
                    Console.WriteLine("What was the outcome? 1= Continue 2= Repel 3= Item destroyed or Event discarded 4= End");
                    line = Console.ReadLine();
                    if (int.TryParse(line, out index))
                    {
                        if (index < 1 || index > 4)
                        {
                            index = 0;
                        }
                    }
                } while (index == 0);
                switch (index)
                {
                    case 1: break;
                    case 2:
                        repelled = true; break;
                    case 3:
                        HunterPlayer hunterToDiscardCard = null;
                        if (huntersInvolved.Count == 1)
                        {
                            hunterToDiscardCard = huntersInvolved.First();
                        }
                        else
                        {
                            var hunterIndex = 0;
                            do
                            {
                                Console.Write("Whose card is being discarded? ");
                                foreach (var h in huntersInvolved)
                                {
                                    Console.Write("{0}= {1} ", (int)h.Hunter, h.Hunter.Name());
                                }
                                line = Console.ReadLine();
                                if (int.TryParse(line, out hunterIndex))
                                {
                                    foreach (var h in huntersInvolved)
                                    {
                                        if ((int)h.Hunter == hunterIndex)
                                        {
                                            hunterToDiscardCard = h;
                                            break;
                                        }
                                    }
                                }
                            } while (hunterToDiscardCard == null);
                        }
                        var itemDestroyed = Item.None;
                        var eventDiscarded = Event.None;
                        while (itemDestroyed == Item.None && eventDiscarded == Event.None)
                        {
                            Console.WriteLine("What card was discarded?");
                            line = Console.ReadLine();
                            itemDestroyed = Enumerations.GetItemFromString(line);
                            eventDiscarded = Enumerations.GetEventFromString(line);
                        }
                        if (itemDestroyed != Item.None)
                        {
                            hunterToDiscardCard.DiscardItem(game, itemDestroyed);
                        }
                        else if (eventDiscarded != Event.None)
                        {
                            hunterToDiscardCard.DiscardEvent(game, eventDiscarded);
                        } break;
                    case 4:
                        continueCombat = false; break;
                }
                firstRound = false;
            }
            var health = -1;
            foreach (var h in huntersInvolved)
            {
                Console.WriteLine("How much health does {0} have now?", h.Hunter.Name());
                do
                {
                    if (int.TryParse(Console.ReadLine(), out health))
                    {
                        health = Math.Max(0, Math.Min(h.MaxHealth, health));
                        Console.WriteLine(health);
                    }
                } while (health == -1);
                h.AdjustHealth(health - h.Health);
            }
            if (opponent == Opponent.Dracula)
            {
                Console.WriteLine("How much blood does Dracula have now?");
                do
                {
                    if (int.TryParse(Console.ReadLine(), out health))
                    {
                        health = Math.Max(0, Math.Min(15, health));
                        Console.WriteLine(health);
                    }
                } while (health == -1);
                game.Dracula.AdjustBlood(health - game.Dracula.Blood);
            }
            Console.WriteLine("Did {0} win? (An end result is a no)", huntersInvolved.Count() > 1 ? "the Hunters" : huntersInvolved.First().Hunter.Name());
            string input;
            do
            {
                input = Console.ReadLine();
            } while (!"yes".StartsWith(input.ToLower()) && !"no".StartsWith(input.ToLower()));
            if ("yes".StartsWith(input.ToLower()))
            {
                if (opponent == Opponent.NewVampire)
                {
                    game.AdjustVampires(-1);
                }
                Console.WriteLine("Don't forget to register any cards discarded, such as Great Strength used to prevent health loss, Events due to enemy Knife wounds, Items consumed, etc.");
                return true;
            }
            if (opponent == Opponent.Dracula)
            {
                switch (enemyCombatCardChosen)
                {
                    case EnemyCombatCard.Mesmerize:
                        Console.WriteLine("{0} is bitten and must discard all Items!", enemyTarget.Name());
                        if (!HunterPlayingGreatStrengthToCancelBite(game, enemyTarget, logic))
                        {
                            game.Hunters[(int)enemyTarget].AdjustBites(1);
                        }
                        while (game.Hunters[(int)enemyTarget].ItemCount > 0)
                        {
                            var line = "";
                            var itemDiscarded = Item.None;
                            do
                            {
                                Console.WriteLine("What is the name of the Item being discarded?");
                                line = Console.ReadLine();
                                itemDiscarded = Enumerations.GetItemFromString(line);
                            } while (itemDiscarded == Item.None);
                            game.Hunters[(int)enemyTarget].DiscardItem(game, itemDiscarded);
                        }
                        CheckForHunterDeath(game); break;
                    case EnemyCombatCard.Fangs:
                        Console.WriteLine("{0} is bitten!", enemyTarget.Name());
                        if (!HunterPlayingGreatStrengthToCancelBite(game, enemyTarget, logic))
                        {
                            game.Hunters[(int)enemyTarget].AdjustBites(1);
                        }
                        CheckForHunterDeath(game);
                        goto case EnemyCombatCard.EscapeBat;
                    case EnemyCombatCard.EscapeBat:
                        Console.WriteLine("Dracula escaped in the form of a bat");
                        game.Dracula.EscapeAsBat(game, logic.ChooseEscapeAsBatDestination(game)); break;
                }
            }
            Console.WriteLine("Don't forget to register any cards discarded, such as Great Strength used to prevent health loss or Items due to enemy Knife wounds");
            foreach (HunterPlayer h in huntersInvolved)
            {
                h.LastCombatCardChosen = Item.None;
            }
            return false;
        }

        /// <summary>
        /// Handles the situation when a Hunter must reveal all Items to Dracula
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="victim">The Hunter revealing Items</param>
        private static void DraculaLooksAtAllHuntersItems(GameState game, HunterPlayer victim)
        {
            game.ItemDeck.AddRange(victim.ItemsKnownToDracula);
            victim.ItemsKnownToDracula.Clear();
            for (var i = 0; i < victim.ItemCount; i++)
            {
                Console.WriteLine("What is the name of the Item being revealed?");
                var itemRevealed = Item.None;
                while (itemRevealed == Item.None)
                {
                    itemRevealed = Enumerations.GetItemFromString(Console.ReadLine());
                }
                AddItemCardToDraculaKnownCards(game, victim, itemRevealed);
            }
        }

        /// <summary>
        /// Adds an Item to a Hunter's list of Items known to Dracula
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="victim">The Hunter revealing an Item</param>
        /// <param name="itemRevealed">The Item revealed</param>
        private static void AddItemCardToDraculaKnownCards(GameState game, HunterPlayer victim, Item itemRevealed)
        {
            var itemCardRevealed = game.ItemDeck.Find(card => card.Item == itemRevealed);
            victim.ItemsKnownToDracula.Add(itemCardRevealed);
            game.ItemDeck.Remove(itemCardRevealed);
        }

        /// <summary>
        /// Determines if Dracula is playing Rage against a Hunter at the start of a combat
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">The list of Hunters involved in the combat</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if Dracula successfully plays Rage</returns>
        private static Hunter DraculaIsPlayingRageAgainstHunter(GameState game, List<HunterPlayer> huntersInvolved,
            DecisionMaker logic)
        {
            var rageTarget = logic.ChooseToPlayRage(game, huntersInvolved);
            if (rageTarget != Hunter.Nobody)
            {
                Console.WriteLine("Dracula is playing Rage against {0}", rageTarget.Name());
                game.Dracula.DiscardEvent(Event.Rage, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.Rage, Event.Rage, logic) > 0)
                {
                    Console.WriteLine("Rage cancelled");
                    return Hunter.Nobody;
                }
                return rageTarget;
            }
            return Hunter.Nobody;
        }

        /// <summary>
        /// Determines if Dracula is playing Trap against a Hunter at the start of a combat
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">The list of Hunters involved in the combat</param>
        /// <param name="opponent">The Opponent</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if Dracula successfully plays Trap</returns>
        private static bool DraculaIsPlayingTrap(GameState game, List<HunterPlayer> huntersInvolved, Opponent opponent,
            DecisionMaker logic)
        {
            if (logic.ChooseToPlayTrap(game, huntersInvolved, opponent))
            {
                Console.WriteLine("Dracula is playing Trap");
                game.Dracula.DiscardEvent(Event.Trap, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.Trap, Event.Trap, logic) > 0)
                {
                    Console.WriteLine("Trap cancelled");
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if a Hunter is playing Great Strength to cancel a Bite
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="bittenHunter">The Hunter receiving a bite</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>True if the Hunter successfully plays Great Strength</returns>
        private static bool HunterPlayingGreatStrengthToCancelBite(GameState game, Hunter bittenHunter,
            DecisionMaker logic)
        {
            Console.WriteLine("Will {0} play a Great Strength to cancel the Bite?", bittenHunter.Name());
            var input = "";
            do
            {
                input = Console.ReadLine();
            } while (!"yes".StartsWith(input.ToLower()) && !"no".StartsWith(input.ToLower()));
            if ("yes".StartsWith(input.ToLower()))
            {
                game.Hunters[(int)bittenHunter].DiscardEvent(game, Event.GreatStrength);
                if (!DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.GreatStrength, Event.GreatStrength, logic))
                {
                    Console.WriteLine("Bite effect cancelled");
                    return true;
                }
                Console.WriteLine("Great Strength cancelled");
                return false;
            }
            return false;
        }
    }
}