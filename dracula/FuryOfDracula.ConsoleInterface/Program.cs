using FuryOfDracula.ArtificialIntelligence;
using FuryOfDracula.GameLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.ConsoleInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            GameState game = new GameState();
            if (args.Length > 0)
            {
                game = (new Program()).LoadGameState(args[0]);
            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    Console.WriteLine("Where is {0} starting?", game.Hunters[i].Hunter.Name());
                    Location destination = Location.Nowhere;
                    string line = "";
                    while (destination == Location.Nowhere || (game.Map.TypeOfLocation(destination) != LocationType.SmallCity && game.Map.TypeOfLocation(destination) != LocationType.LargeCity))
                    {
                        line = Console.ReadLine();
                        destination = game.Map.GetLocationFromString(line);
                        Console.WriteLine(destination.Name());
                        if (game.Map.TypeOfLocation(destination) != LocationType.SmallCity && game.Map.TypeOfLocation(destination) != LocationType.LargeCity)
                        {
                            Console.WriteLine("You must start in a city");
                        }
                    }
                    game.Hunters[i].MoveTo(destination);
                }
            }

            DecisionMaker logic = new DecisionMaker();
            game.Dracula.MoveTo(logic.ChooseStartLocation(game), Power.None);
            while (game.Dracula.EncounterHand.Count() < game.Dracula.EncounterHandSize)
            {
                game.Dracula.DrawEncounter(game.EncounterPool);
            }
            EndHunterTurn(game, logic);

            CommandSet commandSet = new CommandSet();

            do
            {
                DrawGameState(game);
                commandSet = GetCommandSet();

                switch (commandSet.command)
                {
                    case "g":
                    case "group": SetupGroup(game, commandSet.argument1); break;
                    case "t":
                    case "take":
                        if (game.Dracula.TakeEvent(game.EventDeck, game.Events))
                        {
                            game.Dracula.DiscardEvent(logic.ChooseEventToDiscard(game), game.EventDiscard);
                        }; break;
                    case "r":
                    case "reveal": RevealCardInTrailAtPosition(game, commandSet.argument1); break;
                    case "z":
                    case "end": EndHunterTurn(game, logic); break;
                    case "state": DisplayState(game); break;
                    case "discard": DiscardCard(game, commandSet.argument1, commandSet.argument2); break;
                    case "d":
                    case "draw": DrawCard(game, commandSet.argument1, commandSet.argument2); CheckForDiscardRequired(game); CheckForCardsRevealedForBeingBitten(game); break;
                    case "save": SaveGameState(game, commandSet.argument1); break;
                    case "load": game = (new Program()).LoadGameState(commandSet.argument1); break;
                    case "m":
                    case "move": HandleMoveOperation(game, commandSet.argument1, commandSet.argument2, logic); break;
                    case "exit": Console.WriteLine("Fare well"); break;
                    default: Console.WriteLine("I didn't understand that"); break;
                }
            } while (commandSet.command != "exit");
        }

        /// <summary>
        /// Checks all hunters' cards and bites to see if they should be revealing a card to Dracula
        /// </summary>
        /// <param name="game">The GameState</param>
        private static void CheckForCardsRevealedForBeingBitten(GameState game)
        {
            foreach (HunterPlayer h in game.Hunters)
            {
                if (h.ItemCount > 0 && h.BiteCount > 0 && h.ItemShownToDraculaForBeingBitten == Item.None)
                {
                    Console.WriteLine("{0} is bitten and does not have an Item revealed to Dracula. Please name the Item you are revealing");
                    string line = "";
                }
            }
        }

        /// <summary>
        /// Handles the entirety of a hunter's move, including search, encounters, combat etc.
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterIndex">The index of the Hunter (and group) to move</param>
        /// <param name="destination">The destination of the move</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void HandleMoveOperation(GameState game, string hunterIndex, string destination, DecisionMaker logic)
        {
            int hunterMoved = -1;
            int trailIndex = MoveHunter(game, hunterIndex, destination, out hunterMoved);
            if (trailIndex > -1)
            {
                game.Dracula.RevealCardAtPosition(trailIndex);
                game.Dracula.RevealEncountersAtPositionInTrail(game, trailIndex);
                DraculaCardSlot slotBeingRevealed;
                if (trailIndex < 6)
                {
                    slotBeingRevealed = game.Dracula.Trail[trailIndex];
                }
                else
                {
                    slotBeingRevealed = game.Dracula.Catacombs[trailIndex - 6];
                }
                if (slotBeingRevealed.DraculaCards[0].Location == game.Dracula.LocationWhereHideWasUsed)
                {
                    int positionRevealed = game.Dracula.RevealHideCard();
                    game.Dracula.RevealEncountersAtPositionInTrail(game, positionRevealed);
                }
                if (slotBeingRevealed.DraculaCards[0].Location == game.Dracula.CurrentLocation)
                {
                    Console.WriteLine("Dracula is here!");
                }
                else
                {
                    Console.WriteLine("Dracula has been here");
                }
                List<Encounter> encountersToResolve = new List<Encounter>();
                if (slotBeingRevealed.Encounters[0] != Encounter.None)
                {
                    encountersToResolve.Add(slotBeingRevealed.Encounters[0]);
                }
                if (slotBeingRevealed.Encounters[1] != Encounter.None)
                {
                    encountersToResolve.Add(slotBeingRevealed.Encounters[1]);
                }
                bool hunterCanContinueToResolveEncounters = true;
                while (encountersToResolve.Count() > 0 && hunterCanContinueToResolveEncounters)
                {
                    Encounter encounterBeingResolved = logic.ChooseEncounterToResolveOnSearchingHunter(game, encountersToResolve);
                    hunterCanContinueToResolveEncounters = ResolveEncounter(game, encounterBeingResolved, game.Hunters[hunterMoved].HuntersInGroup, logic);
                    encountersToResolve.Remove(encounterBeingResolved);
                }
            }
        }

        /// <summary>
        /// Adds and removes people from the given hunter's group
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterIndex">The index of the hunter with whom to set up a group</param>
        private static void SetupGroup(GameState game, string hunterIndex)
        {
            Hunter hunterFormingGroup = Hunter.Nobody;
            int index;
            if (Int32.TryParse(hunterIndex, out index))
            {
                hunterFormingGroup = game.GetHunterFromInt(index);
            }
            string line = "";
            while (hunterFormingGroup == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is forming a group? {0}= {1}, {2}= {3}, {4}= {5} (Mina Harker cannot lead a group, -1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name());
                line = Console.ReadLine();
                if (Int32.TryParse(line, out index))
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
                foreach (Hunter h in game.Hunters[(int)hunterFormingGroup].HuntersInGroup)
                {
                    if (h != hunterFormingGroup)
                    {
                        Console.WriteLine(h.Name());
                    }
                }
                Hunter hunterToAddOrRemove = Hunter.Nobody;
                while (hunterToAddOrRemove == Hunter.Nobody && index != -1)
                {
                    Console.WriteLine("Who is joining or leaving {0}'s group? {1}= {2}, {3}= {4}, {5}= {6} (Lord Godalming must lead any group he is in, -1 to cancel)", hunterFormingGroup.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                    line = Console.ReadLine();
                    if (Int32.TryParse(line, out index))
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
                    Console.WriteLine("{0} cannot join {1}'s group, instead add {1} to {0}'s group", hunterToAddOrRemove.Name(), hunterFormingGroup.Name());
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
        /// Resolves an encounter on a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="encounterBeingResolved">The Encounter being resolved</param>
        /// <returns></returns>
        private static bool ResolveEncounter(GameState game, Encounter encounterBeingResolved, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            Console.WriteLine("Resolving {0} with {1}{2}", encounterBeingResolved.Name(), huntersInvolved.First().Name(), huntersInvolved.Count() > 1 ? " and his group" : "");
            switch (encounterBeingResolved.Name())
            {
                case "Ambush": return ResolveAmbush(game, huntersInvolved, logic);
                case "Assassin": return ResolveAssassin(game, huntersInvolved, logic);
                case "Bats": return ResolveBats(game, huntersInvolved, logic);
                case "Desecrated Soil": return ResolveDesecratedSoil(game, huntersInvolved, logic);
                case "Fog": return ResolveFog(game, huntersInvolved, logic);
                case "Minion With Knife": return ResolveMinionWithKnife(game, huntersInvolved, logic);
                case "Minion With Knife And Pistol": return ResolveMinionWithKnifeAndPistol(game, huntersInvolved, logic);
                case "Minion With Knife And Rifle": return ResolveMinionWithKnifeAndRifle(game, huntersInvolved, logic);
                case "Hoax": return ResolveHoax(game, huntersInvolved, logic);
                case "Lightning": return ResolveLightning(game, huntersInvolved, logic);
                case "Peasants": return ResolvePeasants(game, huntersInvolved, logic);
                case "Plague": return ResolvePlague(game, huntersInvolved, logic);
                case "Rats": return ResolveRats(game, huntersInvolved, logic);
                case "Saboteur": return ResolveSaboteur(game, huntersInvolved, logic);
                case "Spy": return ResolveSpy(game, huntersInvolved, logic);
                case "Thief": return ResolveThief(game, huntersInvolved, logic);
                case "New Vampire": return ResolveNewVampire(game, huntersInvolved, logic);
                case "Wolves": return ResolveWolves(game, huntersInvolved, logic);
                default: return false;
            }
            game.Encounters.GetEncounterDetail(encounterBeingResolved).IsRevealed = false;
            game.EncounterPool.Add(encounterBeingResolved);
        }

        private static bool ResolveWolves(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveNewVampire(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveThief(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveSpy(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveSaboteur(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveRats(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolvePlague(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolvePeasants(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveLightning(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveHoax(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveMinionWithKnifeAndRifle(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveMinionWithKnifeAndPistol(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveMinionWithKnife(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveFog(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveDesecratedSoil(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveBats(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveAssassin(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        private static bool ResolveAmbush(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            return true;
        }

        /// <summary>
        /// Checks if any of the hunters should discard a card
        /// </summary>
        /// <param name="game">The GameState</param>
        private static void CheckForDiscardRequired(GameState game)
        {
            for (int i = 1; i < 5; i++)
            {
                if (game.Hunters[i].Hunter == Hunter.DrSeward)
                {
                    if (game.Hunters[i].ItemCount > 4 && game.Hunters[i].EventCount > 4)
                    {
                        Console.WriteLine("{0} must discard an Item or an Event", game.Hunters[i].Hunter.Name());
                    }
                    else
                    {

                        if (game.Hunters[i].ItemCount > 5)
                        {
                            Console.WriteLine("{0} must discard an Item", game.Hunters[i].Hunter.Name());
                        }
                        if (game.Hunters[i].EventCount > 5)
                        {
                            Console.WriteLine("{0} must discard an Event", game.Hunters[i].Hunter.Name());
                        }
                    }
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
        }

        /// <summary>
        /// Reveals a card and its encounters at a given position, for debugging
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="position">1-6 for positions in the trail, 7-9 for the three catacombs positions</param>
        private static void RevealCardInTrailAtPosition(GameState game, string position)
        {
            int index;
            if (!Int32.TryParse(position, out index))
            {
                string line = "";
                Console.WriteLine("What position do you want to reveal?");
                bool successful = false;
                do
                {
                    line = Console.ReadLine();
                    successful = Int32.TryParse(line, out index);
                } while (!successful);
            }
            if (!game.Dracula.RevealCardAtPosition(index - 1))
            {
                Console.WriteLine("Cannot reveal card at position {0}", index);
            }
            else
            {
                game.Dracula.RevealEncountersAtPositionInTrail(game, index - 1);
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

            List<int> catacombsSlotsCleared = logic.ChooseWhichCatacombsCardsToDiscard(game);
            if (catacombsSlotsCleared.Count() > 0)
            {
                Console.Write("Dracula discarded cards from Catacombs positions: ");
                foreach (int i in catacombsSlotsCleared)
                {
                    Console.Write("{0} ", i + 1);
                }
                Console.WriteLine("");
                game.Dracula.DiscardCatacombsCards(game, catacombsSlotsCleared);
            }
            Power power;
            DraculaCardSlot cardDroppedOffTrail = game.Dracula.MoveTo(logic.ChooseDestinationAndPower(game, out power), power);
            if (game.Dracula.CurrentLocation != game.Hunters[(int)Hunter.LordGodalming].CurrentLocation && game.Dracula.CurrentLocation != game.Hunters[(int)Hunter.DrSeward].CurrentLocation && game.Dracula.CurrentLocation != game.Hunters[(int)Hunter.VanHelsing].CurrentLocation && game.Dracula.CurrentLocation != game.Hunters[(int)Hunter.MinaHarker].CurrentLocation)
            {
                switch (power)
                {
                    case Power.Hide:
                    case Power.None:
                    case Power.WolfForm:
                        game.Dracula.PlaceEncounterOnCard(logic.ChooseEncounterToPlaceOnDraculaCardSlot(game, game.Dracula.Trail[0]), game.Dracula.Trail[0]); break;
                    case Power.DoubleBack: Encounter encounterToDiscard = logic.ChooseEncounterToDiscardFromDoubleBackedCatacomnbsLocation(game);
                        game.Dracula.DiscardEncounterFromCardSlot(encounterToDiscard, game.Dracula.Trail[0], game.EncounterPool); break;
                }
            }
            else
            {
                game.Dracula.Trail[0].DraculaCards[0].IsRevealed = true;
                Console.WriteLine("Dracula attacks!");
            }
            if (cardDroppedOffTrail != null)
            {
                cardDroppedOffTrail.DraculaCards[1] = null;
                int index = logic.PutDroppedOffCardInCatacombs(game, cardDroppedOffTrail);
                if (index > -1)
                {
                    game.Dracula.Catacombs[index] = cardDroppedOffTrail;
                    game.Dracula.PlaceEncounterOnCard(logic.ChooseEncounterToPlaceOnDraculaCardSlot(game, game.Dracula.Catacombs[index]), cardDroppedOffTrail);
                }
                else
                {
                    cardDroppedOffTrail.DraculaCards[0].IsRevealed = false;
                    if (cardDroppedOffTrail.Encounters[0] != Encounter.None)
                    {
                        game.Encounters.GetEncounterDetail(cardDroppedOffTrail.Encounters[0]).IsRevealed = false;
                        game.EncounterPool.Add(cardDroppedOffTrail.Encounters[0]);
                        cardDroppedOffTrail.Encounters[0] = Encounter.None;
                    }
                    if (cardDroppedOffTrail.Encounters[1] != Encounter.None)
                    {
                        game.Encounters.GetEncounterDetail(cardDroppedOffTrail.Encounters[1]).IsRevealed = false;
                        game.EncounterPool.Add(cardDroppedOffTrail.Encounters[1]);
                        cardDroppedOffTrail.Encounters[1] = Encounter.None;
                    }
                }
                if (cardDroppedOffTrail.DraculaCards[0].Location == game.Dracula.LocationWhereHideWasUsed && game.Dracula.LocationWhereHideWasUsed != Location.Nowhere)
                {
                    List<Encounter> encountersToReturnToEncounterPool = game.Dracula.DiscardHide();
                    foreach (Encounter enc in encountersToReturnToEncounterPool)
                    {
                        game.Encounters.GetEncounterDetail(enc).IsRevealed = false;
                        game.EncounterPool.Add(enc);
                    }
                    Console.WriteLine("The location where Dracula used Hide dropped off the trail, so the Hide card is also removed from the trail");
                }
                if (game.Dracula.CurrentLocation == game.Dracula.LocationWhereHideWasUsed && power == Power.DoubleBack && game.Dracula.LocationWhereHideWasUsed != Location.Nowhere)
                {
                    game.Dracula.RevealHideCard();
                    Console.WriteLine("Dracula used Double Back to return to the location where he previously used Hide");
                }
            }
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
            Console.WriteLine("Trail                       Catacombs       Time          Vampires   Resolve");
            for (int i = 5; i >= 0; i--)
            {
                if (game.Dracula.Trail[i] != null)
                {
                    Console.ForegroundColor = game.Dracula.Trail[i].DraculaCards[0].Color;
                    if (game.Dracula.Trail[i].DraculaCards[0].IsRevealed)
                    {
                        Console.Write(game.Dracula.Trail[i].DraculaCards[0].Abbreviation + " ");
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
            for (int i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null)
                {
                    Console.ForegroundColor = game.Dracula.Catacombs[i].DraculaCards[0].Color;
                    if (game.Dracula.Catacombs[i].DraculaCards[0].IsRevealed)
                    {
                        Console.Write(game.Dracula.Catacombs[i].DraculaCards[0].Abbreviation + " ");
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
                case TimeOfDay.Dusk: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case TimeOfDay.Noon: Console.ForegroundColor = ConsoleColor.Yellow; break;
                case TimeOfDay.Midnight: Console.ForegroundColor = ConsoleColor.Blue; break;
                case TimeOfDay.Twilight:
                case TimeOfDay.SmallHours: Console.ForegroundColor = ConsoleColor.DarkBlue; break;
            }
            Console.Write("    {0}", game.TimeOfDay.Name());
            Console.ResetColor();
            for (int i = game.TimeOfDay.Name().Length; i < 14; i++)
            {
                Console.Write(" ");
            }
            Console.Write(game.Vampires);
            for (int i = game.Vampires.ToString().Length; i < 11; i++)
            {
                Console.Write(" ");
            }
            Console.Write(game.Resolve);
            Console.WriteLine("");
            // line 2
            for (int i = 5; i >= 0; i--)
            {
                if (game.Dracula.Trail[i] != null && game.Dracula.Trail[i].DraculaCards[1] != null)
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
            for (int i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null && game.Dracula.Catacombs[i].Encounters[0] != Encounter.None)
                {
                    if (game.Encounters.GetEncounterDetail(game.Dracula.Catacombs[i].Encounters[0]).IsRevealed)
                    {
                        Console.ResetColor();
                        Console.Write(game.Encounters.GetEncounterDetail(game.Dracula.Catacombs[i].Encounters[0]).Abbreviation + " ");
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
            // line 3
            for (int i = 5; i >= 0; i--)
            {
                if (game.Dracula.Trail[i] != null && game.Dracula.Trail[i].Encounters[0] != Encounter.None)
                {
                    if (game.Encounters.GetEncounterDetail(game.Dracula.Trail[i].Encounters[0]).IsRevealed)
                    {
                        Console.ResetColor();
                        Console.Write(game.Encounters.GetEncounterDetail(game.Dracula.Trail[i].Encounters[0]).Abbreviation + " ");
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
            for (int i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null && game.Dracula.Catacombs[i].Encounters[1] != Encounter.None)
                {
                    if (game.Encounters.GetEncounterDetail(game.Dracula.Catacombs[i].Encounters[1]).IsRevealed)
                    {
                        Console.ResetColor();
                        Console.Write(game.Encounters.GetEncounterDetail(game.Dracula.Catacombs[i].Encounters[1]).Abbreviation + " ");
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
            Console.WriteLine();
            Console.ResetColor();
        }

        /// <summary>
        /// Provides details about various aspects of the gamestate that are not otherwise displayed on the screen, for debugging
        /// </summary>
        /// <param name="game">The GameState</param>
        private static void DisplayState(GameState game)
        {
            Console.WriteLine("The state of the game:");
            foreach (HunterPlayer h in game.Hunters)
            {
                if (h != null)
                {
                    Console.WriteLine("{0} is in {1} with {2} Items and {3} Events, on {4} health with {5} bites", h.Hunter.Name(), h.CurrentLocation.Name(), h.ItemCount, h.EventCount, h.Health, h.BiteCount);
                }
            }
            Console.WriteLine("Dracula is in {0} with {1} blood and has {2} Events", game.Dracula.CurrentLocation.Name(), game.Dracula.Blood, game.Dracula.EventHand.Count());
        }

        /// <summary>
        /// Discards a card of a given name from the given hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="cardName">The name of the card (Item or Event) to discard</param>
        /// <param name="hunterIndex">The number of the hunter (1-4)</param>
        private static void DiscardCard(GameState game, string cardName, string hunterIndex)
        {
            int index = 0;
            Hunter hunterToDiscard = Hunter.Nobody;
            if (Int32.TryParse(hunterIndex, out index))
            {
                hunterToDiscard = game.GetHunterFromInt(index);
            }
            string line = "";
            while (hunterToDiscard == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is discarding a card? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (Int32.TryParse(line, out index))
                {
                    hunterToDiscard = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterToDiscard.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
                if (index == -1)
                {
                    Console.WriteLine("Cancelled");
                    return;
                }
            }
            List<Item> possibleItemsToDiscard = game.Items.GetItemsFromString(cardName);
            List<EventCard> possibleEventsToDiscard = game.Events.GetEventsFromString(cardName);
            while ((possibleEventsToDiscard.Count() == 0 && possibleItemsToDiscard.Count() == 0) || (possibleEventsToDiscard.Count() > 0 && possibleItemsToDiscard.Count() > 0) && line.ToLower() != "cancel")
            {
                Console.WriteLine("What is the name of the card you are discarding? (type cancel to cancel)");
                line = Console.ReadLine().ToLower();
                possibleItemsToDiscard = game.Items.GetItemsFromString(cardName);
                possibleEventsToDiscard = game.Events.GetEventsFromString(cardName);
                if (possibleEventsToDiscard.Count() == 0 && possibleItemsToDiscard.Count() == 0)
                {
                    Console.WriteLine("I couldn't find any card with that name");
                }
                else if (possibleEventsToDiscard.Count() > 0 && possibleItemsToDiscard.Count() > 0)
                {
                    Console.WriteLine("I couldn't find a unique card with that name");
                }
            }
            if (possibleItemsToDiscard.Count() > 0)
            {
                game.Hunters[index].DiscardItem(possibleItemsToDiscard, game.ItemDiscard);
                Console.WriteLine("{0} discarded {1}, down to {2}", hunterToDiscard.Name(), possibleItemsToDiscard.First().Name(), game.Hunters[index].ItemCount);
            }
            else if (possibleEventsToDiscard.Count() > 0)
            {
                game.Hunters[index].DiscardEvent(possibleEventsToDiscard, game.EventDiscard);
                Console.WriteLine("{0} discarded {1}, down to {2}", hunterToDiscard.Name(), possibleEventsToDiscard.First().Name(), game.Hunters[index].EventCount);
            }
        }

        /// <summary>
        /// Adds 1 to the count of cards of the given type to the given hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="cardType">Item or Event</param>
        /// <param name="hunterIndex">The number of the hunter (1-4)</param>
        private static void DrawCard(GameState game, string cardType, string hunterIndex)
        {
            int index = 0;
            Hunter hunterToDraw = Hunter.Nobody;
            if (Int32.TryParse(hunterIndex, out index))
            {
                hunterToDraw = game.GetHunterFromInt(index);
            }
            string line = "";
            while (hunterToDraw == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is drawing a card? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (Int32.TryParse(line, out index))
                {
                    hunterToDraw = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterToDraw.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
                if (index == -1)
                {
                    Console.WriteLine("Cancelled");
                    return;
                }
            }
            do
            {
                switch (cardType)
                {
                    case "item": game.Hunters[index].DrawItemCard(); Console.WriteLine("{0} drew an Item card, up to {1}", hunterToDraw.Name(), game.Hunters[index].ItemCount); return;
                    case "event": game.Hunters[index].DrawEventCard(); Console.WriteLine("{0} drew an Event card, up to {1}", hunterToDraw.Name(), game.Hunters[index].EventCount); return;
                    default:
                        Console.WriteLine("What type of card is {0} drawing?", hunterToDraw.Name());
                        cardType = Console.ReadLine().ToLower();
                        break;
                }
            } while (cardType != "cancel");
            Console.WriteLine("Cancelled");
        }

        /// <summary>
        /// Moves the given hunter to the given location, along with all hunters in the given hunter's group
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterIndex">The number of the hunter (1-4)</param>
        /// <param name="location">The name of the location to move to</param>        
        /// <returns>The position of the card in Dracula's trail (0-5) or catacombs (6-8) that corresponds to the given location, or -1 if not in the trail/catacombs</returns>
        private static int MoveHunter(GameState game, string hunterIndex, string location, out int hunterMoved)
        {
            Hunter hunterToMove = Hunter.Nobody;
            int index;
            if (Int32.TryParse(hunterIndex, out index))
            {
                hunterToMove = game.GetHunterFromInt(index);
            }
            string line = "";
            while (hunterToMove == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is moving? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (Int32.TryParse(line, out index))
                {
                    hunterToMove = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterToMove.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            hunterMoved = index;
            if (index == -1)
            {
                Console.WriteLine("Cancelled");
                return -1;
            }
            Location destination = game.Map.GetLocationFromString(location);
            while (destination == Location.Nowhere && line.ToLower() != "cancel")
            {
                Console.WriteLine("Where is {0} moving? (Type cancel to cancel)", hunterToMove.Name());
                line = Console.ReadLine();
                destination = game.Map.GetLocationFromString(line);
                Console.WriteLine(destination.Name());
            }
            if (line.ToLower() == "cancel")
            {
                Console.WriteLine("Cancelled");
                return -1;
            }
            Console.Write("{0} moved from {1} to ", hunterToMove.Name(), game.Hunters[(int)hunterToMove].CurrentLocation.Name());
            foreach (Hunter h in game.Hunters[(int)hunterToMove].HuntersInGroup)
            {
                game.Hunters[(int)h].MoveTo(destination);
            }
            Console.WriteLine(destination.Name() + (game.Hunters[(int)hunterToMove].HuntersInGroup.Count() > 1 ? " with his group" : ""));
            for (int i = 0; i < 6; i++)
            {
                if (game.Dracula.Trail[i] != null && game.Dracula.Trail[i].DraculaCards[0].Location == destination)
                {
                    return i;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null && game.Dracula.Catacombs[i].DraculaCards[0].Location == destination)
                {
                    return i + 6;
                }
            }
            return -1;
        }

        /// <summary>
        /// Loads a previously saved gamestate from a file with the given fileName
        /// </summary>
        /// <param name="fileName">The name of the file, without extension</param>
        /// <returns>A deserialised GameState object from the file</returns>
        private GameState LoadGameState(string fileName)
        {
            GameState tempGame = null;
            try
            {
                string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                foreach (char c in invalid)
                {
                    fileName = fileName.Replace(c.ToString(), "");
                }
                fileName = fileName + ".sav";
                DataContractSerializer fileReader = new DataContractSerializer(typeof(GameState));
                FileStream readStream = File.OpenRead(fileName);

                tempGame = (GameState)fileReader.ReadObject(readStream);
                readStream.Close();
                Console.WriteLine(fileName + " loaded");
            }
            catch (Exception e)
            {
                Console.WriteLine("File {0} could not be loaded because {1}", fileName, e.Message);

            }
            return tempGame;
        }

        /// <summary>
        /// Serialises and saves a gamestate to a file with the given filename
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="fileName">The name of the file, without extension</param>
        private static void SaveGameState(GameState game, string fileName)
        {
            try
            {
                string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                foreach (char c in invalid)
                {
                    fileName = fileName.Replace(c.ToString(), "");
                }
                fileName = fileName + ".sav";
                DataContractSerializer fileWriter = new DataContractSerializer(typeof(GameState));
                FileStream writeStream = File.OpenWrite(fileName);

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
            string argument1;
            string argument2 = "no argument";

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
                    argument2 = "no argument";
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                argument1 = "no argument";
            }

            return new CommandSet(command.ToLower(), argument1.ToLower(), argument2.ToLower());
        }

    }
}
