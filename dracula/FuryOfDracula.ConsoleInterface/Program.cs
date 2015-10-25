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
                try
                {
                    game = new FileLoader().LoadGameState(args[0]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
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
                        destination = Enumerations.GetLocationFromString(line);
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
                        if (game.Dracula.TakeEvent(game.EventDeck))
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
                    case "load": game = new FileLoader().LoadGameState(commandSet.argument1); break;
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
                if (h != null)
                {
                    if (h.ItemCount > 0 && h.BiteCount > 0 && h.ItemShownToDraculaForBeingBitten == null)
                    {
                        Item itemToReveal = Item.None;
                        Console.WriteLine("{0} is bitten and does not have an Item revealed to Dracula. Please name the Item you are revealing", h.Hunter.Name());
                        while (itemToReveal == Item.None)
                        {
                            itemToReveal = Enumerations.GetItemFromString(Console.ReadLine());
                            if (itemToReveal == Item.None)
                            {
                                Console.WriteLine("I couldn't find an Item with that name");
                            }
                        }
                        ItemCard itemCardToReveal = h.ItemsKnownToDracula.Find(card => card.Item == itemToReveal);
                        if (itemCardToReveal != null)
                        {
                            h.ItemShownToDraculaForBeingBitten = itemCardToReveal;
                        }
                        else
                        {
                            itemCardToReveal = game.ItemDeck.Find(card => card.Item == itemToReveal);
                            h.ItemShownToDraculaForBeingBitten = itemCardToReveal;
                            h.ItemsKnownToDracula.Add(itemCardToReveal);
                            game.ItemDeck.Remove(itemCardToReveal);
                        }
                        Console.WriteLine("{0} revealed to Dracula", itemCardToReveal.Item.Name());
                    }
                    if (h.EventCount > 0 && h.BiteCount > 0 && h.EventShownToDraculaForBeingBitten == null)
                    {
                        Event eventToReveal = Event.None;
                        Console.WriteLine("{0} is bitten and does not have an Event revealed to Dracula. Please name the Event you are revealing", h.Hunter.Name());
                        while (eventToReveal == Event.None)
                        {
                            eventToReveal = Enumerations.GetEventFromString(Console.ReadLine());
                            if (eventToReveal == Event.None)
                            {
                                Console.WriteLine("I couldn't find an Item with that name");
                            }
                        }
                        EventCard eventCardToReveal = h.EventsKnownToDracula.Find(card => card.Event == eventToReveal);
                        if (eventCardToReveal != null)
                        {
                            h.EventShownToDraculaForBeingBitten = eventCardToReveal;
                        }
                        else
                        {
                            eventCardToReveal = game.EventDeck.Find(card => card.Event == eventToReveal);
                            h.EventShownToDraculaForBeingBitten = eventCardToReveal;
                            h.EventsKnownToDracula.Add(eventCardToReveal);
                            game.EventDeck.Remove(eventCardToReveal);
                        }
                        Console.WriteLine("{0} revealed to Dracula", eventCardToReveal.Event.Name());
                    }
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
            Hunter hunterMoved = Hunter.Nobody;
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
                if (slotBeingRevealed.DraculaCards.First().Location == game.Dracula.LocationWhereHideWasUsed)
                {
                    int positionRevealed = game.Dracula.RevealHideCard();
                    game.Dracula.RevealEncountersAtPositionInTrail(game, positionRevealed);
                }
                if (slotBeingRevealed.DraculaCards.First().Location == game.Dracula.CurrentLocation)
                {
                    Console.WriteLine("Dracula is here!");
                }
                else
                {
                    Console.WriteLine("Dracula has been here");
                }
                List<EncounterTile> encounterTilesToResolve = new List<EncounterTile>();
                foreach (EncounterTile enc in slotBeingRevealed.EncounterTiles)
                {
                    encounterTilesToResolve.Add(enc);
                }
                bool hunterCanContinueToResolveEncounters = true;
                while (encounterTilesToResolve.Count() > 0 && hunterCanContinueToResolveEncounters)
                {
                    EncounterTile encounterTileBeingResolved = logic.ChooseEncounterToResolveOnSearchingHunter(game, encounterTilesToResolve);
                    hunterCanContinueToResolveEncounters = ResolveEncounterTile(game, encounterTileBeingResolved, game.Hunters[(int)hunterMoved].HuntersInGroup, logic);
                    encounterTilesToResolve.Remove(encounterTileBeingResolved);
                }
                if (slotBeingRevealed.DraculaCards.First().Location == game.Dracula.CurrentLocation && hunterCanContinueToResolveEncounters) {
                    List<HunterPlayer> huntersAttacking = new List<HunterPlayer>();
                    foreach (Hunter h in game.Hunters[(int)hunterMoved].HuntersInGroup)
                    {
                        huntersAttacking.Add(game.Hunters[(int)h]);
                    }
                    ResolveCombat(game, huntersAttacking, Opponent.Dracula, logic);
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
        /// <param name="encounterTileBeingResolved">The Encounter being resolved</param>
        /// <returns></returns>
        private static bool ResolveEncounterTile(GameState game, EncounterTile encounterTileBeingResolved, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            Console.WriteLine("Resolving {0} with {1}{2}", encounterTileBeingResolved.Encounter.Name(), huntersInvolved.First().Name(), huntersInvolved.Count() > 1 ? " and his group" : "");
            switch (encounterTileBeingResolved.Encounter)
            {
                case Encounter.Ambush: return ResolveAmbush(game, huntersInvolved, logic);
                case Encounter.Assassin: return ResolveAssassin(game, huntersInvolved, logic);
                case Encounter.Bats: return ResolveBats(game, huntersInvolved, logic);
                case Encounter.DesecratedSoil: return ResolveDesecratedSoil(game, huntersInvolved, logic);
                case Encounter.Fog: return ResolveFog(game, huntersInvolved, logic);
                case Encounter.MinionWithKnife: return ResolveMinionWithKnife(game, huntersInvolved, logic);
                case Encounter.MinionWithKnifeAndPistol: return ResolveMinionWithKnifeAndPistol(game, huntersInvolved, logic);
                case Encounter.MinionWithKnifeAndRifle: return ResolveMinionWithKnifeAndRifle(game, huntersInvolved, logic);
                case Encounter.Hoax: return ResolveHoax(game, huntersInvolved, logic);
                case Encounter.Lightning: return ResolveLightning(game, huntersInvolved, logic);
                case Encounter.Peasants: return ResolvePeasants(game, huntersInvolved, logic);
                case Encounter.Plague: return ResolvePlague(game, huntersInvolved, logic);
                case Encounter.Rats: return ResolveRats(game, huntersInvolved, logic);
                case Encounter.Saboteur: return ResolveSaboteur(game, huntersInvolved, logic);
                case Encounter.Spy: return ResolveSpy(game, huntersInvolved, logic);
                case Encounter.Thief: return ResolveThief(game, huntersInvolved, logic);
                case Encounter.NewVampire: return ResolveNewVampire(game, huntersInvolved, logic);
                case Encounter.Wolves: return ResolveWolves(game, huntersInvolved, logic);
                default: return false;
            }
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
                        game.Dracula.PlaceEncounterTileOnCard(logic.ChooseEncounterTileToPlaceOnDraculaCardSlot(game, game.Dracula.Trail[0]), game.Dracula.Trail[0]); break;
                    case Power.DoubleBack:
                        EncounterTile encounterTileToDiscard = logic.ChooseEncounterTileToDiscardFromDoubleBackedCatacomnbsLocation(game);
                        game.Dracula.DiscardEncounterTileFromCardSlot(encounterTileToDiscard, game.Dracula.Trail[0], game.EncounterPool); break;
                }
            }
            else
            {
                game.Dracula.Trail[0].DraculaCards.First().IsRevealed = true;
                Console.WriteLine("Dracula attacks!");
                List<HunterPlayer> huntersAttacked = new List<HunterPlayer>();
                foreach (HunterPlayer h in game.Hunters)
                {
                    if (h != null && h.CurrentLocation == game.Dracula.CurrentLocation)
                    {
                        huntersAttacked.Add(h);
                    }
                }
                ResolveCombat(game, huntersAttacked, Opponent.Dracula, logic);
            }
            if (cardDroppedOffTrail != null)
            {
                if (cardDroppedOffTrail.DraculaCards.Count() > 1)
                {
                    cardDroppedOffTrail.DraculaCards.Remove(cardDroppedOffTrail.DraculaCards[1]);
                }
                int index = logic.PutDroppedOffCardInCatacombs(game, cardDroppedOffTrail);
                if (index > -1)
                {
                    game.Dracula.Catacombs[index] = cardDroppedOffTrail;
                    game.Dracula.PlaceEncounterTileOnCard(logic.ChooseEncounterTileToPlaceOnDraculaCardSlot(game, game.Dracula.Catacombs[index]), cardDroppedOffTrail);
                }
                else
                {
                    cardDroppedOffTrail.DraculaCards.First().IsRevealed = false;
                    while (cardDroppedOffTrail.EncounterTiles.Count() > 0)
                    {
                        cardDroppedOffTrail.EncounterTiles.First().IsRevealed = false;
                        game.EncounterPool.Add(cardDroppedOffTrail.EncounterTiles.First());
                        cardDroppedOffTrail.EncounterTiles.Remove(cardDroppedOffTrail.EncounterTiles.First());
                    }
                }
                if (cardDroppedOffTrail.DraculaCards.First().Location == game.Dracula.LocationWhereHideWasUsed && game.Dracula.LocationWhereHideWasUsed != Location.Nowhere)
                {
                    List<EncounterTile> encountersToReturnToEncounterPool = game.Dracula.DiscardHide();
                    foreach (EncounterTile enc in encountersToReturnToEncounterPool)
                    {
                        enc.IsRevealed = false;
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
            for (int i = 0; i < 3; i++)
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
            for (int i = 0; i < 3; i++)
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
            // line 3
            for (int i = 5; i >= 0; i--)
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
            for (int i = 0; i < 3; i++)
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
                    if (h.ItemsKnownToDracula.Count() > 0)
                    {
                        Console.Write("These Items held by {0} are known to Dracula: ", h.Hunter.Name());
                        foreach (ItemCard i in h.ItemsKnownToDracula)
                        {
                            Console.Write("{0}, ", i.Item.Name());
                        }
                        Console.WriteLine("");
                    }
                    if (h.EventsKnownToDracula.Count() > 0)
                    {
                        Console.Write("These Events held by {0} are known to Dracula: ", h.Hunter.Name());
                        foreach (EventCard e in h.EventsKnownToDracula)
                        {
                            Console.Write("{0}, ", e.Event.Name());
                        }
                        Console.WriteLine("");
                    }
                }
            }
            Console.WriteLine("Dracula is in {0} with {1} blood and has {2} Events", game.Dracula.CurrentLocation.Name(), game.Dracula.Blood, game.Dracula.EventHand.Count());
            Console.Write("Dracula is holding these Encounter tiles: ");
            foreach (EncounterTile enc in game.Dracula.EncounterHand)
            {
                Console.Write("{0} ", enc.Encounter.Name());
            }
            if (game.Dracula.EventHand.Count() > 0)
            {
                Console.Write("Dracula is holding these Event cards: ");
                foreach (EventCard e in game.Dracula.EventHand)
                {
                    Console.Write("{0} ", e.Event.Name());
                }
            }
            Console.WriteLine("");
            Console.WriteLine("These Items are in the discard:");
            foreach (ItemCard i in game.ItemDiscard)
            {
                Console.WriteLine(i.Item.Name());
            }
            Console.WriteLine("These Events are in the discard:");
            foreach (EventCard e in game.EventDiscard)
            {
                Console.WriteLine(e.Event.Name());
            }
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
            if (Int32.TryParse(hunterIndex, out index) && index > 0 && index < 5)
            {
                hunterToDiscard = (Hunter)index;
            }
            string line = "";
            while (hunterToDiscard == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is discarding a card? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (Int32.TryParse(line, out index))
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
            Item itemToDiscard = Enumerations.GetItemFromString(cardName);
            Event eventToDiscard = Enumerations.GetEventFromString(cardName);
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
                Console.WriteLine("{0} discarded {1}, down to {2}", hunterToDiscard.Name(), itemToDiscard.Name(), game.Hunters[index].ItemCount);
            }
            else if (eventToDiscard != Event.None)
            {
                game.Hunters[index].DiscardEvent(game, eventToDiscard);
                Console.WriteLine("{0} discarded {1}, down to {2}", hunterToDiscard.Name(), eventToDiscard.Name(), game.Hunters[index].EventCount);
            }
            CheckForCardsRevealedForBeingBitten(game);
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
                    case "item": game.Hunters[index].DrawItemCard(); Console.WriteLine("{0} drew an Item card, up to {1}", hunterToDraw.Name(), game.Hunters[index].ItemCount); return;
                    case "e":
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
        private static int MoveHunter(GameState game, string hunterIndex, string location, out Hunter hunterMoved)
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
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        hunterMoved = Hunter.Nobody;
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
            Location destination = Enumerations.GetLocationFromString(location);
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
            Console.Write("{0} moved from {1} to ", hunterToMove.Name(), game.Hunters[(int)hunterToMove].CurrentLocation.Name());
            foreach (Hunter h in game.Hunters[(int)hunterToMove].HuntersInGroup)
            {
                game.Hunters[(int)h].MoveTo(destination);
            }
            Console.WriteLine(destination.Name() + (game.Hunters[(int)hunterToMove].HuntersInGroup.Count() > 1 ? " with his group" : ""));
            for (int i = 0; i < 6; i++)
            {
                if (game.Dracula.Trail[i] != null && game.Dracula.Trail[i].DraculaCards.First().Location == destination)
                {
                    return i;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null && game.Dracula.Catacombs[i].DraculaCards.First().Location == destination)
                {
                    return i + 6;
                }
            }
            return -1;
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

        private static bool ResolveCombat(GameState game, List<HunterPlayer> huntersInvolved, Opponent opponent, DecisionMaker logic)
        {
            List<ItemCard> basicHunterCombatCards = new List<ItemCard> { new ItemCard(Item.Punch), new ItemCard(Item.Dodge), new ItemCard(Item.Escape) };
            List<EnemyCombatCard> enemyCombatCards = new List<EnemyCombatCard>();
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
                            enemyCombatCards.Add(EnemyCombatCard.Strength); break;
                    }; break;
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
                            enemyCombatCards.Add(EnemyCombatCard.Strength); break;
                    }; break;
            }
            bool firstRound = true;
            bool repelled = false;
            bool continueCombat = true;
            EnemyCombatCard enemyCombatCardChosen = EnemyCombatCard.None;
            Hunter enemyTarget = Hunter.Nobody;
            while (continueCombat)
            {
                foreach (HunterPlayer h in huntersInvolved)
                {
                    do
                    {
                        Console.WriteLine("Which combat card is {0} using this round?", h.Hunter.Name());
                        h.LastCombatCardChosen = Enumerations.GetItemFromString(Console.ReadLine());
                    } while (h.LastCombatCardChosen == Item.None);
                }
                enemyCombatCardChosen = logic.ChooseCombatCardAndTarget(huntersInvolved, enemyCombatCards, firstRound, out enemyTarget, enemyCombatCardChosen, repelled);
                Console.WriteLine("{0} used {1} against {2}", opponent.Name(), enemyCombatCardChosen.Name(), enemyTarget.Name());
                string line = "";
                int index = 0;
                do
                {
                    Console.WriteLine("What was the outcome? 1= Continue 2= Repel 3= Item destroyed 4= End");
                    line = Console.ReadLine();
                    if (Int32.TryParse(line, out index))
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
                    case 2: repelled = true; break;
                    case 3:
                        HunterPlayer hunterToDiscardItem = null;
                        if (huntersInvolved.Count == 1)
                        {
                            hunterToDiscardItem = huntersInvolved.First();
                        }
                        else
                        {
                            int hunterIndex = 0;
                            do
                            {
                                Console.Write("Whose Item was destroyed? ");
                                foreach (HunterPlayer h in huntersInvolved)
                                {
                                    Console.Write("{0}= {1} ", (int)h.Hunter, h.Hunter.Name());
                                }
                                line = Console.ReadLine();
                                if (Int32.TryParse(line, out hunterIndex))
                                {
                                    foreach (HunterPlayer h in huntersInvolved)
                                    {
                                        if ((int)h.Hunter == hunterIndex)
                                        {
                                            hunterToDiscardItem = h;
                                            break;
                                        }
                                    }
                                }
                            } while (hunterToDiscardItem == null);
                        }
                        Item itemDestroyed = Item.None;
                        while (itemDestroyed == Item.None)
                        {
                            Console.WriteLine("What Item was destroyed?");
                            line = Console.ReadLine();
                            itemDestroyed = Enumerations.GetItemFromString(line);
                        }
                        hunterToDiscardItem.DiscardItem(game, itemDestroyed);
                        break;
                    case 4: continueCombat = false; break;
                }
            }
            int health = -1;
            foreach (HunterPlayer h in huntersInvolved)
            {
                Console.WriteLine("How much health does {0} have now?", h.Hunter.Name());
                do
                {
                    if (Int32.TryParse(Console.ReadLine(), out health))
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
                    if (Int32.TryParse(Console.ReadLine(), out health))
                    {
                        health = Math.Max(0, Math.Min(15, health));
                        Console.WriteLine(health);
                    }
                } while (health == -1);
                game.Dracula.AdjustBlood(health - game.Dracula.Blood);
            }
            Console.WriteLine("Did the Hunter{0} win?", huntersInvolved.Count() > 1 ? "s" : "");
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
                return true;
            } else
            {
                return false;
            }
        }
    }
}
