using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationHandler;

namespace ConsoleHandler
{
    class UserInterface
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
                Console.WriteLine("Where is " + g.NameOfHunterAtIndex(v) + "?");
                line = Console.ReadLine();
                hunterStartLocation = g.GetLocationFromName(line);
                Console.WriteLine(hunterStartLocation.name);
            } while (hunterStartLocation.name == "Unknown location");
            return hunterStartLocation;
        }

        internal void drawTrail(GameState g)
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
        }

        internal string GetNameOfLocationWhereHunterIsMoving(string v)
        {
            Console.WriteLine("Where is " + v + " moving? (partial name will suffice)");
            return Console.ReadLine();
        }

        internal int GetIndexOfMovingHunter()
        {
            string line = "";
            int hunterIndex;
            do
            {
                Console.WriteLine("Who is moving? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = Console.ReadLine();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex;
        }

        internal CommandSet GetCommandSet()
        {
            string line;
            string command;
            string argument1;
            string argument2 = "no argument";

            line = Console.ReadLine();
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

        internal string GetEventCardName()
        {
            Console.WriteLine("What is the event card name? (partial name will suffice)");
            return Console.ReadLine();
        }
    }
}
