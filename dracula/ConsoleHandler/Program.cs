using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationHandler;
using DraculaHandler;
using LogHandler;
using EncounterHandler;
using EventHandler;
using HunterHandler;

namespace ConsoleHandler
{
    class Program
    {

        static void Main(string[] args)
        {
            //List<Location> map = new List<Location>();
            //SetupMap(map);

            //List<Encounter> encounterDeck = new List<Encounter>();
            //SetupEncounters(encounterDeck);

            //List<Event> eventDeck = new List<Event>();
            //SetupEvents(eventDeck);

            //Hunter[] hunters = new Hunter[4];
            //hunters[0] = new Hunter();
            //hunters[1] = new Hunter();
            //hunters[2] = new Hunter();
            //hunters[3] = new Hunter();

            //hunters[0].name = "Lord Godalming";
            //hunters[1].name = "Van Helsing";
            //hunters[2].name = "Dr. Seward";
            //hunters[3].name = "Mina Harker";

            //List<Event> hunterAlly = new List<Event>();
            //List<Event> draculaAlly = new List<Event>();

            GameState g = new GameState();

            try
            {
                System.IO.File.Delete(@"debuglog.txt");
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("Couldn't delete the old debug log file");
            }

            try
            {
                System.IO.File.Delete(@"gamelog.txt");
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("Couldn't delete the old game log file");
            }

            Logger.WriteToDebugLog("Game start");

            //int startLocation;
            //do
            //{
            //    startLocation = new Random().Next(0, map.Count());
            //} while (map[startLocation].type == LocationType.Hospital);

            //Dracula dracula = new Dracula(map[startLocation], encounterDeck);

            Logger.WriteToDebugLog("Dracula started in " + g.dracula.currentLocation.name);
            Logger.WriteToGameLog("Dracula started in " + g.dracula.currentLocation.name);

            //int time = -1;
            //string[] timesOfDay = new string[6] { "Dawn", "Noon", "Dusk", "Twilight", "Midnight", "Small Hours" };

            string line;
            string command;
            string argument1;
            do
            {
                //drawTrail(dracula, timesOfDay[Math.Max(0, time)], draculaAlly, hunterAlly);
                drawTrail(g);
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
                }
                catch (ArgumentOutOfRangeException)
                {
                    argument1 = "no argument";
                }

                switch (command)
                {
                    case "s": LocationHelper.ShowLocationDetails(GetLocationFromName(argument1, g.map)); break;
                    case "d": g.dracula.ShowLocation(); break;
                    case "m":
                        {
                            Logger.WriteToDebugLog("STARTING DRACULA'S TURN =====================================================");
                            if (g.dracula.currentLocation.type != LocationType.Sea)
                            {
                                g.time = (g.time + 1) % 6;
                                Logger.WriteToDebugLog("Time is now " + g.timesOfDay[g.time]);
                                if (g.time == 0)
                                {
                                    g.dracula.vampireTracker++;
                                    Logger.WriteToDebugLog("Increasing vampire track to " + g.dracula.vampireTracker);
                                    if (g.dracula.vampireTracker > 0)
                                    {
                                        Logger.WriteToGameLog("Dracula earned a point, up to " + g.dracula.vampireTracker);
                                    }
                                }
                            }
                            else
                            {
                                Logger.WriteToDebugLog("Dracula is at sea, skipping Timekeeping phase so time remains " + g.timesOfDay[Math.Max(0, g.time)]);
                            }
                            g.dracula.TakeStartOfTurnActions();
                            g.dracula.MoveDracula(g.time);
                            g.dracula.HandleDroppedOffLocations();
                            g.dracula.DoActionPhase();
                            g.dracula.MatureEncounters();
                            g.dracula.DrawEncounters(g.dracula.encounterHandSize);
                            break;
                        }
                    case "t": g.dracula.ShowTrail(); break;
                    case "r":
                        {
                            int trailIndex;
                            if (int.TryParse(argument1, out trailIndex))
                            {
                                try
                                {
                                    if (g.dracula.trail[trailIndex - 1].name == "Hide")
                                    {
                                        g.dracula.RevealHide(trailIndex - 1);
                                    }
                                    else
                                    {
                                        LocationHelper.RevealLocation(g, trailIndex - 1);
                                    }
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    Console.WriteLine("Unable to reveal card " + argument1);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Unable to reveal card " + argument1);
                            }
                            break;
                        }
                    case "e":
                        {
                            int trailIndex;
                            if (int.TryParse(argument1, out trailIndex))
                            {
                                try
                                {
                                    LocationHelper.RevealEncounter(g.dracula.trail, trailIndex - 1);
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    Console.WriteLine("Unable to reveal encounter " + argument1);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Unable to reveal encounter " + argument1);
                            }
                            break;
                        }
                    case "c":
                        {
                            int trailLength;
                            if (int.TryParse(argument1, out trailLength))
                            {
                                g.dracula.TrimTrail(Math.Max(1, trailLength));
                            }
                            else
                            {
                                Console.WriteLine("Unable to clear Dracula's trail to length " + argument1);
                            }
                            break;
                        }
                    case "v":
                        {
                            switch (PlayEventCard(g))
                            {
                                case "Rufus Smith": PlayRufusSmith(g); break;
                                case "Jonathan Harker": PlayJonathanHarker(g); break;
                                case "Sister Agatha": PlaySisterAgatha(g); break;
                                case "Heroic Leap": PlayHeroicLeap(g); break;
                                case "Great Strength": PlayGreatStrength(g); break;
                                case "Money Trail": PlayMoneyTrail(g); break;
                                case "Sense of Emergency": PlaySenseOfEmergency(g); break;
                                case "Vampiric Lair": PlayVampiricLair(g); break;
                                case "Long Day": PlayLongDay(g); break;
                                case "Mystic Research": PlayMysticResearch(g); break;
                                case "Advance Planning": PlayAdvancePlanning(g); break;
                                case "Newspaper Reports": PlayNewspaperReports(g); break;
                                case "Re-Equip": PlayReEquip(g); break;
                                case "Consecrated Ground": PlayConsecratedGround(g); break;
                                case "Telegraph Ahead": PlayTelegraphAhead(g); break;
                                case "Hypnosis": PlayHypnosis(g); break;
                                case "Stormy Seas": PlayStormySeas(g); break;
                                case "Surprising Return": PlaySurprisingReturn(g); break;
                                case "Good Luck": PlayGoodLuck(g); break;
                                case "Blood Transfusion": PlayBloodTransfusion(g); break;
                                case "Secret Weapon": PlaySecretWeapon(g); break;
                                case "Forewarned": PlayForewarned(g); break;
                                case "Chartered Carriage": PlayCharteredCarriage(g); break;
                                case "Excellent Weather": PlayExcellentWeather(g); break;
                                case "Escape Route": PlayEscapeRoute(g); break;
                                case "Hired Scouts": PlayHiredScouts(g); break;
                            }
                            break;

                        }
                    case "exit": break;
                    default: Console.WriteLine("I don't know what you're talking about"); break;
                }
            } while (command != "exit");
        }

        public static string PlayEventCard(GameState g)
        {
            string line = "";
            int hunterIndex = -1;
            do {
                Console.WriteLine("Who is playing the card? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                line = Console.ReadLine();
            } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
                int eventIndex = -1;
            do
            {
                Console.WriteLine("What is the event card name? (partial name will suffice)");
                line = Console.ReadLine();
                eventIndex = g.eventDeck.FindIndex(card => card.name.ToUpper().StartsWith(line.ToUpper()));
                if (eventIndex == -1) {
                    Console.WriteLine("I don't recognise a card starting with " + line + ". Is it in the discard pile?");
                } else if (g.eventDeck[eventIndex].isDraculaCard)
                {
                    Console.WriteLine(g.eventDeck[eventIndex].name + " is Dracula's card");
                    eventIndex = -1;
                }
            } while (eventIndex == -1);
            Console.WriteLine(g.hunters[hunterIndex - 1].name + " is playing event card " + g.eventDeck[eventIndex].name);
            return g.eventDeck[eventIndex].name;
        }

        private static void PlayHiredScouts(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayEscapeRoute(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayExcellentWeather(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayCharteredCarriage(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayForewarned(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlaySecretWeapon(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayBloodTransfusion(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayGoodLuck(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlaySurprisingReturn(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayStormySeas(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayHypnosis(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayTelegraphAhead(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayConsecratedGround(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayReEquip(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayNewspaperReports(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayAdvancePlanning(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayMysticResearch(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayLongDay(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayVampiricLair(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlaySenseOfEmergency(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayMoneyTrail(GameState g)
        {
            Console.WriteLine("Revealing all sea locations in Dracula's trail");
            for (int i = 0; i < g.dracula.trail.Count(); i++) {
                if (g.dracula.trail[i].type == LocationType.Sea) {
                    LocationHelper.RevealLocation(g, i);
                }
            }

        }

        private static void PlayGreatStrength(GameState g)
        {
            Console.WriteLine("This card is supposed to be played when a Hunter receives damage or a bite");
        }

        private static void PlayHeroicLeap(GameState g)
        {
            Console.WriteLine("This card is supposed to be played at the start of a combat");
        }

        private static void PlaySisterAgatha(GameState g)
        {
            if (g.hunterAlly != null)
            {
                g.eventDiscard.Add(g.hunterAlly);
            }
            g.hunterAlly = g.eventDeck[g.eventDeck.FindIndex(card => card.name == "Sister Agatha")];
            g.eventDeck.Remove(g.hunterAlly);
        }

        private static void PlayJonathanHarker(GameState g)
        {
            if (g.hunterAlly != null)
            {
                g.eventDiscard.Add(g.hunterAlly);
            }
            g.hunterAlly = g.eventDeck[g.eventDeck.FindIndex(card => card.name == "Jonathan Harker")];
            g.eventDeck.Remove(g.hunterAlly);
        }

        private static void PlayRufusSmith(GameState g)
        {
            if (g.hunterAlly != null)
            {
                g.eventDiscard.Add(g.hunterAlly);
            }
            g.hunterAlly = g.eventDeck[g.eventDeck.FindIndex(card => card.name == "Rufus Smith")];
            g.eventDeck.Remove(g.hunterAlly);
        }

        public static Location GetLocationFromName(string locationName, List<Location> locationList)
        {
            for (int i = 0; i < locationList.Count(); i++)
            {
                if ((locationList[i].name.ToLower() == locationName.ToLower()) || (locationList[i].abbreviation.ToLower() == locationName.ToLower()))
                {
                    return locationList[i];
                }
            }
            Location unknownLocation = new Location();
            unknownLocation.name = "Unknown location";
            return unknownLocation;
        }

        public static void drawTrail(GameState g)
        {
            Console.WriteLine("6th 5th 4th 3rd 2nd 1st   Time        Dracula  Vampires  Catacombs");
            for (int i = 5; i >= 0; i--)
            {
                if (i + 1 > g.dracula.trail.Count())
                {
                    Console.Write("    ");
                }
                else
                {
                    g.dracula.trail[i].DrawLocation();
                }
            }

            string timeOfDay = g.timesOfDay[Math.Max(0, g.time)];
            switch (timeOfDay)
            {
                case "Dawn": Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case "Noon": Console.ForegroundColor = ConsoleColor.Yellow; break;
                case "Dusk": Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case "Twilight": Console.ForegroundColor = ConsoleColor.Cyan; break;
                case "Midnight": Console.ForegroundColor = ConsoleColor.Blue; break;
                case "Small Hours": Console.ForegroundColor = ConsoleColor.Cyan; break;
            }
            Console.Write("  " + timeOfDay);
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < (12 - timeOfDay.Length); i++)
            {
                Console.Write(" ");
            }
            Console.Write(g.dracula.blood);
            Console.ResetColor();
            for (int i = 0; i < (9 - g.dracula.blood.ToString().Length); i++)
            {
                Console.Write(" ");
            }
            Console.Write(Math.Max(0, g.dracula.vampireTracker));
            for (int i = 0; i < (10 - g.dracula.vampireTracker.ToString().Length); i++)
            {
                Console.Write(" ");
            }
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < 3; i++)
            {
                if (g.dracula.catacombs[i] != null)
                {
                    if (g.dracula.catacombs[i].isRevealed)
                    {
                        Console.Write(g.dracula.catacombs[i].abbreviation + " ");
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
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            string tempString;
            for (int counter = 5; counter > -1; counter--)
            {
                tempString = "    ";
                for (int i = 0; i < g.dracula.powers.Count(); i++)
                {
                    if (g.dracula.powers[i].positionInTrail == counter && g.dracula.powers[i].name != "Hide" && g.dracula.powers[i].name != "Dark Call" && g.dracula.powers[i].name != "Feed")
                    {
                        tempString = g.dracula.powers[i].name.Substring(0, 3).ToUpper() + " ";
                    }
                }
                Console.Write(tempString);
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("                                 ");

            for (int i = 0; i < 3; i++)
            {
                if (g.dracula.catacombs[i] != null)
                {
                    if (g.dracula.catacombs[i].encounters.Count > 0)
                    {
                        g.dracula.catacombs[i].DrawEncounter();
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }

            Console.WriteLine("");
            for (int i = 5; i > -1; i--)
            {
                if (i + 1 > g.dracula.trail.Count())
                {
                    Console.Write("    ");
                }
                else
                {
                    g.dracula.trail[i].DrawEncounter();
                }
            }
            Console.ResetColor();
            Console.Write("  Dracula's Ally    Hunters' Ally");

            Console.WriteLine("");
            for (int i = 5; i > -1; i--)
            {
                if (i + 1 > g.dracula.trail.Count())
                {
                    Console.Write("    ");
                }
                else
                {
                    g.dracula.trail[i].DrawEncounter(true);
                }
            }
            Console.ResetColor();
            Console.Write("  ");
            if (g.draculaAlly != null)
            {
                Console.Write(g.draculaAlly.name.Substring(0, 3).ToUpper());
            } else
            {
                Console.Write("   ");
            }
            Console.Write("               ");
            if (g.hunterAlly != null)
            {
                Console.Write(g.hunterAlly.name.Substring(0, 3).ToUpper());
            }
            else
            {
                Console.Write("   ");
            }

            Console.Write("          ");
            for (int i = 0; i < 3; i++)
            {
                if (g.dracula.catacombs[i] != null)
                {
                    if (g.dracula.catacombs[i].encounters.Count > 0)
                    {
                        g.dracula.catacombs[i].DrawEncounter(true);
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }


            Console.ResetColor();
            Console.WriteLine("");
        }

    }

}
