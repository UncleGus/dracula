using System;
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
            // second line, trail cards, time, Dracula blood, Vampires, Catacombs cards, Events
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
            // Events
            Console.WriteLine(" " + g.NumberOfEventCardsInDraculaHand());
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
            Console.WriteLine("                          " + Math.Max(0, g.ResolveTracker()));
        }

        internal int GetHunterToAddToGroup(string name)
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who do you want to add to or remove from " + name + "'s group? 2) Van Helsing 3) Dr. Seward 4) Mina Harker -1) Cancel");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < -1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal void ShowGroupMembersAtHunterIndex(GameState g, int hunterIndex)
        {
            TellUser("Members of " + g.NameOfHunterAtIndex(hunterIndex) + "'s group, other than himself:");
            string[] names = new string[4];
            g.GetHunterGroupMemberNamesAtHunterIndex(hunterIndex, names);
            foreach (string name in names)
            {
                if (name != g.NameOfHunterAtIndex(hunterIndex))
                {
                    TellUser(name);
                }
            }
        }

        internal int GetIndexOfHunterFormingGroup()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Whose group would you like to set up? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; (Mina Harker can't lead a group)");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 3);
            return hunterIndex - 1;
        }

        internal string GetNameOfLocationWhereHunterIsMoving(string v)
        {
            TellUser("Where is " + v + " moving? (partial name will suffice)");
            return AskUser();
        }

        internal string AskHunterToRevealItemByVampiricInfluence(string name)
        {
            TellUser("What is the name of the item being revealed by " + name + "?");
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

        internal string AskHunterToRevealEventByVampiricInfluence(string name)
        {
            TellUser("What is the name of the event being revealed by " + name + "?");
            return AskUser();
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
            TellUser("Name the second city");
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

        internal int GetHunterHealthLost(string name)
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

        internal int GetIndexOfHunterEnteringCombat()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is entering combat? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal int GetTypeOfEnemyEnteringCombat()
        {
            string line = "";
            int enemyType;
            do
            {
                TellUser("Who is the enemy? 1) Dracula 2) Minion with Knife 3) Minion with Knife and Pistol 4) Minion with Knife and Rifle 5) Assassin 6) Vampire");
                line = AskUser();
            } while (!int.TryParse(line, out enemyType) || enemyType < 1 || enemyType > 6);
            return enemyType;
        }

        internal string GetNameOfItemRetrievedFromDiscardByHunter(string name)
        {
            TellUser("What is the name of the item being retrieved from the discard pile?");
            return AskUser();
        }

        internal string GetCombatCardFromHunter(string name)
        {
            TellUser("What combat card did " + name + " use?");
            return AskUser();
        }

        internal string GetCombatRoundOutcome()
        {
            string line = "";
            int userAnswer;
            do
            {
                TellUser("What was the outcome of this round of combat? 1) Enemy wounded 2) Hunter wounded 3) Continue 4) Repel 5) Bite 6) Enemy killed 7) Hunter killed 8) End");
                line = AskUser();
            } while (!int.TryParse(line, out userAnswer) || userAnswer < 1 || userAnswer > 8);

            switch (userAnswer)
            {
                case 1: line = "Enemy wounded"; break;
                case 2: line = "Hunter wounded"; break;
                case 3: line = "Continue"; break;
                case 4: line = "Repel"; break;
                case 5: line = "Bite"; break;
                case 6: line = "Enemy killed"; break;
                case 7: line = "Hunter killed"; break;
                case 8: line = "End"; break;
            }
            return line;
        }

        internal int GetDraculaBloodLost()
        {
            string line;
            int loss;
            do
            {
                TellUser("How much health did Dracula lose?");
                line = AskUser();
            } while (!int.TryParse(line, out loss) || loss < 0);
            return loss;
        }

        internal int GetDieRoll()
        {
            string line = "";
            int userAnswer;
            do
            {
                TellUser("Roll a die and enter the result");
                line = AskUser();
            } while (!int.TryParse(line, out userAnswer) || userAnswer < 1 || userAnswer > 6);
            return userAnswer;
        }

        internal int GetHunterSharpItems(string name)
        {
            string line;
            int answer;
            do
            {
                TellUser(name + " has 0) Nothing 1) Knife 2) Stake");
                line = AskUser();
            }
            while (!int.TryParse(line, out answer) || answer < 0 || answer > 2);
            return answer;
        }

        internal bool GetCombatOutcome()
        {
            string line;
            do
            {
                TellUser("Did you defeat the enemy?");
                line = AskUser();
            }
            while (!"yes".Contains(line.ToLower()) && !"no".Contains(line.ToLower()));
            return "yes".Contains(line.ToLower()) ? true : false;
        }

        internal int GetIndexOfHunterBeingMovedByBats()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is being moved by Bats? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal int GetIndexOfHunterEnteringTrade()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is entering trade? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal int GetNumberOfCardsGivenByHunter(string name)
        {
            string line;
            int given;
            do
            {
                TellUser("How many items did " + name + " give?");
                line = AskUser();
            } while (!int.TryParse(line, out given) || given < 0);
            return given;
        }

        internal int GetIndexOfHunterUsingItem()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is using an item? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal string GetNameOfItemUsedByHunter(string name)
        {
            TellUser("What item did " + name + " use?");
            return AskUser();
        }

        internal int GetLocationIndexOfEncounterToReveal()
        {
            string line;
            int index;
            do
            {
                TellUser("Which position in the trail would you like to reveal? Trail = 1-6 Catacombs = 7-9");
                line = AskUser();
            } while (!int.TryParse(line, out index) || index < 1 || index > 9);
            return index - 1;
        }

        internal int GetIndexOfEncounterToReveal()
        {
            string line;
            int index;
            do
            {
                TellUser("Which encounter would you like to reveal? 1 or 2");
                line = AskUser();
            } while (!int.TryParse(line, out index) || index < 1 || index > 9);
            return index - 1;
        }

        internal bool GetHunterHeal(string name)
        {
            string line;
            do
            {
                TellUser("Heal " + name + "?");
                line = AskUser();
            }
            while (!"yes".Contains(line.ToLower()) && !"no".Contains(line.ToLower()));
            return "yes".Contains(line.ToLower()) ? true : false;
        }

        internal int GetIndexOfHunterPlayingEventCard()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is playing the event card? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal int GetHolyWaterResult()
        {
            string line;
            int result;
            do
            {
                TellUser("What was the outcome? 1) Lost 2 health 2) Nothing 3) Bite removed");
                line = AskUser();
            } while (!int.TryParse(line, out result) || result < 1 || result > 3);
            return result;
        }

        internal int GetIndexOfHunterReceivingBloodTransfusion()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is donating blood? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward (Mina Harker cannot have her bite healed)");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 3);
            return hunterIndex - 1;
        }

        internal int GetIndexOfHunterGivingBloodTransfusion()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is donating blood? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal string GetNameOfLocationWhereStormySeasIsBeingPlayed()
        {
            TellUser("Where is Stormy Seas being played?");
            return AskUser();
        }

        internal int GetIndexOfHunterBitten()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who was bitten? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal bool GetDidDraculaEscape()
        {
            string line;
            do
            {
                TellUser("Did Dracula escape?");
                line = AskUser();
            }
            while (!"yes".Contains(line.ToLower()) && !"no".Contains(line.ToLower()));
            return "yes".Contains(line.ToLower()) ? true : false;
        }

        public int GetDraculaEscapeForm()
        {
            string line;
            int result;
            do
            {
                TellUser("In what form? 1) Man 2) Mist 3) Bats");
                line = AskUser();
            } while (!int.TryParse(line, out result) || result < 1 || result > 3);
            return result;
        }

        internal int GetIndexOfHunterResting()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is resting? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal int GetEventDrawnFor()
        {
            string line;
            int result;
            do
            {
                TellUser("Who is the drawn event for? 1) Hunters 2) Dracula");
                line = AskUser();
            } while (!int.TryParse(line, out result) || result < 1 || result > 2);
            return result;
        }

        internal int GetIndexOfHunterUsingHospital()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is using the Holy Water font? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward (Mina Harker's bite cannot be healed)");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 3);
            return hunterIndex - 1;
        }

        internal int GetIndexOfHunterUsingResolve()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who is using Resolve? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal int GetTypeOfResolveUsed()
        {
            string line = "";
            int type;
            do
            {
                TellUser("How are you using the Resolve? 1) Newspaper Reports 2) Sense of Emergency 3) Inner Strength");
                line = AskUser();
            } while (!int.TryParse(line, out type) || type < 1 || type > 3);
            return type;
        }

        internal bool GetHunterPlayingCard(string name)
        {
            string line;
            do
            {
                TellUser("Is " + name + " using an item or event at the start of this combat?");
                line = AskUser();
            }
            while (!"yes".Contains(line.ToLower()) && !"no".Contains(line.ToLower()));
            return "yes".Contains(line.ToLower()) ? true : false;
        }

        internal string GetNameOfHunterCardPlayedAtStartOfCombat(string name)
        {
            TellUser("What card is " + name + " playing? (type cancel to cancel playing a card)");
            return AskUser();
        }

        internal bool GetUserCancelEncounter(string name)
        {
            string line;
            do
            {
                TellUser("The encounter is " + name + ", do you want to cancel it?");
                line = AskUser();
            }
            while (!"yes".Contains(line.ToLower()) && !"no".Contains(line.ToLower()));
            return "yes".Contains(line.ToLower()) ? true : false;
        }

        internal int GetHunterPlayingForewarned(string name)
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("The encounter is " + name + ", is anyone playing Forewarned? 0 = Nobody; 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 0 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal void ShowHelp()
        {
            TellUser("(M)ove hunter, Catch (T)rain, Play (E)vent, Give (D)racula event card, Give Hunter e(V)ent card, Give Hunter (I)tem, Disc(A)rd Hunter event, Dis(C)ard Hunter item, Setup (G)roups, Perform (B)ats move, T(R)ade items, (U)se item, Get s(O)me rest, Use (H)ospital font, (S)pend resolve, Sleep (Z)zzzz and let Dracula have his turn, View (STATE) of the game, (EXIT)");
        }

        internal string GetNameOfLocationToConsecrate()
        {
            TellUser("What location do you want to consecrate?");
            return AskUser();
        }

        internal string GetEventCardNameBeingReturned()
        {
            TellUser("What is the name of the card you are retrieving? (none) if not taking one");
            return AskUser();
        }

        internal int GetNameOfHunterKilled()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Who was killed? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal string GetNameOfCardInRagedHunterHand()
        {
            TellUser("What is the name of the item card you have? (none) if have no more");
            return AskUser();
        }

        internal bool AskIfHunterIsUsingGreatStrengthToCancelBite(string name)
        {
            string line;
            do
            {
                TellUser("Will " + name + " cancel the bite with a Great Strength?");
                line = AskUser();
            }
            while (!"yes".Contains(line.ToLower()) && !"no".Contains(line.ToLower()));
            return "yes".Contains(line.ToLower()) ? true : false;
        }

        internal bool AskIfHunterIsUsingGreatStrengthToCancelDamage(string name)
        {
            string line;
            do
            {
                TellUser("Will " + name + " cancel the damage with a Great Strength?");
                line = AskUser();
            }
            while (!"yes".Contains(line.ToLower()) && !"no".Contains(line.ToLower()));
            return "yes".Contains(line.ToLower()) ? true : false;
        }

        internal int AskWhichHunterIsUsingGoodLuckToCancelEvent()
        {
            string line = "";
            int hunterIndex;
            do
            {
                TellUser("Will anyone play Good Luck to cancel this event card? 0 = Nobody; 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = AskUser();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 0 || hunterIndex > 4);
            return hunterIndex - 1;
        }

        internal int AskHunterDiscardAllyOrRoadblock()
        {
            string line = "";
            int response;
            do
            {
                TellUser("What are you discarding? 0 = Nothing; 1 = Dracula's Ally; 2 = Roadblock counter");
                line = AskUser();
            } while (!int.TryParse(line, out response) || response < 0 || response > 2);
            return response;
            
        }

        internal string GetNameOfItemInHandFromHunter(string p)
        {
            TellUser("What is the name of the item in " + p + "'s hand?");
            return AskUser();
        }

        internal string GetNameOfEventInHandFromHunter(string p)
        {
            TellUser("What is the name of the event in " + p + "'s hand?");
            return AskUser();
        }

        internal string AskHunterWhatTravelTypeForSpy(string name)
        {
            string line = "";
            int response;
            do
            {
                TellUser("How will " + name + " be travelling next turn? 1) Road 2) Rail 3) Sea 4) Sense Of Emergency");
                line = AskUser();
            } while (!int.TryParse(line, out response) || response < 1 || response > 4);
            switch (response)
            {
                case 1: return "Road";
                case 2: return "Rail";
                case 3: return "Sea";
                case 4: return "Sense Of Emergency";
            }
            return "Hovercraft";
        }

        internal string AskHunterWhichLocationTheyAreMovingToNextTurn(string name)
        {
            TellUser("To which location will " + name + " be travelling next turn?");
            return AskUser();
        }

        internal string AskHunterToRevealItemForBeingBitten(string p)
        {
            TellUser(p + " is bitten and has an unrevealed item, please tell me what it is");
            return AskUser();
        }

        internal string AskHunterToRevealEvent(string p)
        {
            TellUser(p + " is bitten and has an unrevealed event, please tell me what it is");
            return AskUser();
        }

        internal bool AskIfHunterIsPlayingSecretWeapon(string name)
        {
            string line;
            do
            {
                TellUser("Is " + name + " playing Secret Weapon?");
                line = AskUser();
            }
            while (!"yes".Contains(line.ToLower()) && !"no".Contains(line.ToLower()));
            return "yes".Contains(line.ToLower()) ? true : false;
        }
    }
}
