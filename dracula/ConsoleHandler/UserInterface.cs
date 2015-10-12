﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationHandler;

namespace ConsoleHandler
{
    public class UserInterface
    {
        internal void TellUser(string v)
        {
            Console.WriteLine(v);
        }

        internal Location GetHunterStartLocation(GameState g, int v)
        {
            string line;
            Location hunterStartLocation;
            do
            {
                TellUser("Where is " + g.NameOfHunterAtIndex(v) + "?");
                line = AskUser();
                hunterStartLocation = g.GetLocationFromName(line);
                TellUser(hunterStartLocation.name);
            } while (hunterStartLocation.name == "Unknown location");
            return hunterStartLocation;
        }

        internal void drawGameState(GameState g)
        {
            // top line, trail headers, time header, Dracula blood and Vampire track header, Catacombs header, Dracula cards header
            Console.WriteLine("6th 5th 4th 3rd 2nd 1st   Time        Blood    Vampires  Catacombs    Events");
            // second line, trail cards, time, Dracula blood, Vampires, Catacombs cards
            // trail cards
            for (int i = 5; i >= 0; i--)
            {
                if (i + 1 > g.TrailLength())
                {
                    Console.Write("    ");
                }
                else
                {
                    g.DrawLocationAtTrailIndex(i);
                }
            }

            string timeOfDay = g.TimeOfDay();
            switch (timeOfDay)
            {
                case "Dawn": Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case "Noon": Console.ForegroundColor = ConsoleColor.Yellow; break;
                case "Dusk": Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case "Twilight": Console.ForegroundColor = ConsoleColor.Cyan; break;
                case "Midnight": Console.ForegroundColor = ConsoleColor.Blue; break;
                case "Small Hours": Console.ForegroundColor = ConsoleColor.Cyan; break;
            }
            // time of day
            Console.Write("  " + timeOfDay);
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < (12 - timeOfDay.Length); i++)
            {
                Console.Write(" ");
            }
            // Dracula blood
            Console.Write(g.DraculaBloodLevel());
            Console.ResetColor();
            for (int i = 0; i < (9 - g.DraculaBloodLevel().ToString().Length); i++)
            {
                Console.Write(" ");
            }
            // Vampire tracker
            Console.Write(Math.Max(0, g.VampireTracker()));
            for (int i = 0; i < (10 - g.VampireTracker().ToString().Length); i++)
            {
                Console.Write(" ");
            }
            Console.ForegroundColor = ConsoleColor.Red;
            // Catacombs cards
            for (int i = 0; i < 3; i++)
            {
                if (!g.LocationIsEmptyAtCatacombIndex(i))
                {
                    if (g.LocationIsRevealedAtCatacombIndex(i))
                    {
                        Console.Write(g.LocationAbbreviationAtCatacombIndex(i) + " ");
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
            Console.ResetColor();
            Console.WriteLine("  " + g.NumberOfEventCardsInDraculaHand());
            // third line power cards, 
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            string tempString;
            // power cards
            for (int counter = 5; counter > -1; counter--)
            {
                tempString = "    ";
                for (int i = 0; i < g.NumberOfDraculaPowers(); i++)
                {
                    if (g.DraculaPowerAtPowerIndexIsAtLocationIndex(i, counter) && g.DraculaPowerNameAtPowerIndex(i) != "Hide" && g.DraculaPowerNameAtPowerIndex(i) != "Dark Call" && g.DraculaPowerNameAtPowerIndex(i) != "Feed")
                    {
                        tempString = g.DraculaPowerNameAtPowerIndex(i).Substring(0, 3).ToUpper() + " ";
                    }
                }
                Console.Write(tempString);
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("                                 ");
            // first Catacombs encounters
            for (int i = 0; i < 3; i++)
            {
                if (!g.LocationIsEmptyAtCatacombIndex(i))
                {
                    if (g.NumberOfEncountersAtLocationAtCatacombIndex(i) > 0)
                    {
                        g.DrawEncounterAtCatacombIndex(i);
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }

            Console.WriteLine("");
            // fourth line trail encounters, ally headers, second Catacomb encounters
            // trail encounters
            for (int i = 5; i > -1; i--)
            {
                if (i + 1 > g.TrailLength())
                {
                    Console.Write("    ");
                }
                else
                {
                    g.DrawEncounterAtTrailIndex(i);
                }
            }
            Console.ResetColor();
            // ally headers
            Console.Write("  Dracula's Ally    Hunters' Ally");

            // second Catacomb encounters
            for (int i = 0; i < 3; i++)
            {
                if (!g.LocationIsEmptyAtCatacombIndex(i))
                {
                    if (g.NumberOfEncountersAtLocationAtCatacombIndex(i) > 0)
                    {
                        g.DrawEncounterAtCatacombIndex(i, true);
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }
            Console.WriteLine("");
            // fifth line, ally names
            Console.Write("                          ");
            Console.ResetColor();
            if (g.DraculaHasAlly())
            {
                Console.Write(g.NameOfDraculaAlly().Substring(0, 3).ToUpper());
            }
            else
            {
                Console.Write("   ");
            }
            Console.Write("               ");
            if (g.HuntersHaveAlly())
            {
                Console.Write(g.NameOfHunterAlly().Substring(0, 3).ToUpper());
            }
            else
            {
                Console.Write("   ");
            }
            Console.ResetColor();
            Console.WriteLine("");
            // sixth line, nothing
            Console.WriteLine("");
            // seventh line, resolve header
            Console.WriteLine("                          Resolve");
            // eighth line, resolve value
            Console.WriteLine("                          " + g.ResolveTracker());
        }

        internal string GetNameOfLocationWhereHunterIsMoving(string v)
        {
            TellUser("Where is " + v + " moving? (partial name will suffice)");
            return AskUser();
        }

        internal int GetIndexOfMovingHunter()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is moving? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal CommandSet GetCommandSet()
        {
            string line;
            string command;
            string argument1;
            string argument2 = "no argument";

            line = AskUser();
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

            return new CommandSet(command, argument1, argument2);
        }

        internal string GetFirstLocationNameForHiredScouts()
        {
            TellUser("Name the first city");
            return AskUser();
        }

        internal string GetEventCardName()
        {
            TellUser("What is the event card name? (partial name will suffice)");
            return AskUser();
        }

        internal string GetSecondLocationNameForHiredScouts()
        {
            TellUser("Name the first city");
            return AskUser();
        }

        internal int GetHunterHolyItems(string name)
        {
            string line;
            int answer;
            do
            {
                TellUser(name + " has 0) Nothing 1) Crucifix 2) Heavenly Host");
                line = AskUser();
            }
            while (!int.TryParse(line, out answer) || answer < 0 || answer > 2);
            return answer;
        }

        internal int GetHunterHealthLostFromRats(string name)
        {
            string line;
            int loss;
            do
            {
                TellUser("How much health did " + name + " lose?");
                line = AskUser();
            } while (!int.TryParse(line, out loss) || loss < 0);
            return loss;
        }

        private string AskUser()
        {
            return Console.ReadLine();
        }

        internal int GetHunterEquipmentForWolves(string name)
        {
            string line;
            int answer;
            do
            {
                TellUser(name + " has 0) Nothing 1) Pistol 2) Rifle 3) Both");
                line = AskUser();
            }
            while (!int.TryParse(line, out answer) || answer < 0 || answer > 3);
            return answer;
        }

        internal int GetIndexOfHunterDrawingEvent()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is drawing an event card? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal int GetIndexOfHunterDrawingItem()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is drawing an item card? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal int GetIndexOfHunterDiscardingEvent()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is discarding an event card? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal string GetNameOfEventDiscardedByHunter(string p)
        {
            TellUser("What is the name of the event being discarded?");
            return AskUser();
        }

        internal int GetIndexOfHunterDiscardingItem()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is discarding an item card? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal string GetNameOfItemDiscardedByHunter(string p)
        {
            TellUser("What is the name of the item being discarded?");
            return AskUser();
        }
    }
}
