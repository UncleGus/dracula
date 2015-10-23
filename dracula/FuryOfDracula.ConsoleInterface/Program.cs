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
                    case "reveal": RevealCardInTrailAtPosition(game, commandSet.argument1); break;
                    case "end": EndHunterTurn(game, logic); break;
                    case "state": DisplayState(game); break;
                    case "discard": DiscardCard(game, commandSet.argument1, commandSet.argument2); break;
                    case "draw": DrawCard(game, commandSet.argument1, commandSet.argument2); break;
                    case "save": SaveGameState(game, commandSet.argument1); break;
                    case "load": game = (new Program()).LoadGameState(commandSet.argument1); break;
                    case "move": MoveHunter(game, commandSet.argument1, commandSet.argument2); break;
                    case "exit": Console.WriteLine("Fare well"); break;
                }
            } while (commandSet.command != "exit");
        }

        private static void RevealCardInTrailAtPosition(GameState game, string position)
        {
            int index;
            if (Int32.TryParse(position, out index))
            {
                if (!game.Dracula.RevealCardAtPosition(game, index - 1))
                {
                    Console.WriteLine("Cannot reveal card at position {0}", index);
                }
            } else
            {
                string line = "";
                Console.WriteLine("What position do you want to reveal?");
                bool successful = false;
                do
                {
                    line = Console.ReadLine();
                    successful = Int32.TryParse(line, out index);
                } while (!successful);
                if (!game.Dracula.RevealCardAtPosition(game, index - 1))
                {
                    Console.WriteLine("Cannot reveal card at position {0}", index);
                }
            }
        }

        private static void EndHunterTurn(GameState game, DecisionMaker logic)
        {
            if (game.Map.TypeOfLocation(game.Dracula.CurrentLocation) != LocationType.Sea)
            {
                game.TimeOfDay = game.TimeOfDay + 1 % 6;
                if (game.TimeOfDay == TimeOfDay.Dawn)
                {
                    game.Vampires++;
                    game.Resolve++;
                }
            }
            Power power;
            DraculaCardSlot cardDroppedOffTrail = game.Dracula.MoveTo(logic.ChooseDestinationAndPower(game, out power), power);
            game.Dracula.PlaceEncounterOnCard(logic.ChooseEncounterToPlaceOnDraculaCardSlot(game, game.Dracula.Trail[0]), game.Dracula.Trail[0]);
            if (cardDroppedOffTrail != null)
            {
                cardDroppedOffTrail.DraculaCards[0].IsRevealed = false;
                if (cardDroppedOffTrail.DraculaCards[1] != null)
                {
                    cardDroppedOffTrail.DraculaCards[1].IsRevealed = false;
                }
                if (cardDroppedOffTrail.Encounters[0] != Encounter.None)
                {
                    game.Encounters.GetEncounterDetail(cardDroppedOffTrail.Encounters[0]).IsRevealed = false;
                }
                if (cardDroppedOffTrail.Encounters[1] != Encounter.None)
                {
                    game.Encounters.GetEncounterDetail(cardDroppedOffTrail.Encounters[1]).IsRevealed = false;
                }
            }            
        }

        private static void DrawGameState(GameState game)
        {
            Console.WriteLine("Trail");
            for (int i = 5; i >= 0; i--)
            {
                if (game.Dracula.Trail[i] != null)
                {
                    Console.ForegroundColor = game.Dracula.Trail[i].DraculaCards[0].Color;
                    if (game.Dracula.Trail[i].DraculaCards[0].IsRevealed)
                    {
                        Console.Write(game.Dracula.Trail[i].DraculaCards[0].Abbreviation + " ");
                    } else
                    {
                        Console.Write("### ");
                    }
                } else
                {
                    Console.Write("    ");
                }
            }
            Console.WriteLine("");
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
            Console.WriteLine("");
            for (int i = 5; i >= 0; i--)
            {
                if (game.Dracula.Trail[i] != null && game.Dracula.Trail[i].Encounters[0] != Encounter.None)
                {
                    if (game.Encounters.GetEncounterDetail(game.Dracula.Trail[i].Encounters[0]).IsRevealed)
                    {
                        Console.ResetColor();
                        Console.Write(game.Dracula.Trail[i].Encounters[0].Name().Substring(0, 3).ToUpper() + " ");
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
            Console.WriteLine();
        }

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
            List<Event> possibleEventsToDiscard = game.Events.GetEventsFromString(cardName);
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

        private static void MoveHunter(GameState game, string hunterName, string location)
        {
            Hunter hunterToMove = Hunter.Nobody;
            int index;
            if (Int32.TryParse(hunterName, out index))
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
            if (index == -1)
            {
                Console.WriteLine("Cancelled");
                return;
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
                return;
            }
            Console.Write("{0} moved from {1} to ", hunterToMove.Name(), game.Hunters[(int)hunterToMove].CurrentLocation.Name());
            game.Hunters[(int)hunterToMove].MoveTo(destination);
            Console.WriteLine(destination.Name());
        }

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
