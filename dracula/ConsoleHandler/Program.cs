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
            List<Location> map = new List<Location>();
            SetupMap(map);

            List<Encounter> encounterDeck = new List<Encounter>();
            SetupEncounters(encounterDeck);

            List<Event> eventDeck = new List<Event>();
            SetupEvents(eventDeck);

            Hunter[] hunters = new Hunter[4];
            hunters[0] = new Hunter();
            hunters[1] = new Hunter();
            hunters[2] = new Hunter();
            hunters[3] = new Hunter();

            hunters[0].name = "Lord Godalming";
            hunters[1].name = "Van Helsing";
            hunters[2].name = "Dr. Seward";
            hunters[3].name = "Mina Harker";

            List<Event> hunterAlly = new List<Event>();
            List<Event> draculaAlly = new List<Event>();

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

            int startLocation;
            do
            {
                startLocation = new Random().Next(0, map.Count());
            } while (map[startLocation].type == LocationType.Hospital);

            Dracula dracula = new Dracula(map[startLocation], encounterDeck);

            Logger.WriteToDebugLog("Dracula started in " + dracula.currentLocation.name);
            Logger.WriteToGameLog("Dracula started in " + dracula.currentLocation.name);

            int time = -1;
            string[] timesOfDay = new string[6] { "Dawn", "Noon", "Dusk", "Twilight", "Midnight", "Small Hours" };

            string line;
            string command;
            string argument1;
            do
            {
                drawTrail(dracula, timesOfDay[Math.Max(0, time)], draculaAlly, hunterAlly);
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
                    case "s": LocationHelper.ShowLocationDetails(GetLocationFromName(argument1, map)); break;
                    case "d": dracula.ShowLocation(); break;
                    case "m":
                        {
                            Logger.WriteToDebugLog("STARTING DRACULA'S TURN =====================================================");
                            if (dracula.currentLocation.type != LocationType.Sea)
                            {
                                time = (time + 1) % 6;
                                Logger.WriteToDebugLog("Time is now " + timesOfDay[time]);
                                if (time == 0)
                                {
                                    dracula.vampireTracker++;
                                    Logger.WriteToDebugLog("Increasing vampire track to " + dracula.vampireTracker);
                                    if (dracula.vampireTracker > 0)
                                    {
                                        Logger.WriteToGameLog("Dracula earned a point, up to " + dracula.vampireTracker);
                                    }
                                }
                            }
                            else
                            {
                                Logger.WriteToDebugLog("Dracula is at sea, skipping Timekeeping phase so time remains " + timesOfDay[Math.Max(0, time)]);
                            }
                            dracula.TakeStartOfTurnActions();
                            dracula.MoveDracula(time);
                            dracula.HandleDroppedOffLocations();
                            dracula.DoActionPhase();
                            dracula.MatureEncounters();
                            dracula.DrawEncounters(dracula.encounterHandSize);
                            break;
                        }
                    case "t": dracula.ShowTrail(); break;
                    case "r":
                        {
                            int trailIndex;
                            if (int.TryParse(argument1, out trailIndex))
                            {
                                try
                                {
                                    if (dracula.trail[trailIndex - 1].name == "Hide")
                                    {
                                        dracula.RevealHide(trailIndex - 1);
                                    }
                                    else
                                    {
                                        LocationHelper.RevealLocation(dracula.trail, trailIndex - 1);
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
                                    LocationHelper.RevealEncounter(dracula.trail, trailIndex - 1);
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
                                dracula.TrimTrail(Math.Max(1, trailLength));
                            }
                            else
                            {
                                Console.WriteLine("Unable to clear Dracula's trail to length " + argument1);
                            }
                            break;
                        }
                    case "v":
                        {
                            switch (PlayEventCard(eventDeck, hunters))
                            {
                                case "Rufus Smith": PlayRufusSmith(eventDeck, hunterAlly); break;
                                case "Jonathan Harker": PlayJonathanHarker(eventDeck); break;
                                case "Sister Agatha": PlaySisterAgatha(eventDeck); break;
                                case "Heroic Leap": PlayHeroicLeap(eventDeck); break;
                                case "Great Strength": PlayGreatStrength(eventDeck); break;
                                case "Money Trail": PlayMoneyTrail(eventDeck); break;
                                case "Sense of Emergency": PlaySenseOfEmergency(eventDeck); break;
                                case "Vampiric Lair": PlayVampiricLair(eventDeck); break;
                                case "Long Day": PlayLongDay(eventDeck); break;
                                case "Mystic Research": PlayMysticResearch(eventDeck); break;
                                case "Advance Planning": PlayAdvancePlanning(eventDeck); break;
                                case "Newspaper Reports": PlayNewspaperReports(eventDeck); break;
                                case "Re-Equip": PlayReEquip(eventDeck); break;
                                case "Consecrated Ground": PlayConsecratedGround(eventDeck); break;
                                case "Telegraph Ahead": PlayTelegraphAhead(eventDeck); break;
                                case "Hypnosis": PlayHypnosis(eventDeck); break;
                                case "Stormy Seas": PlayStormySeas(eventDeck); break;
                                case "Surprising Return": PlaySurprisingReturn(eventDeck); break;
                                case "Good Luck": PlayGoodLuck(eventDeck); break;
                                case "Blood Transfusion": PlayBloodTransfusion(eventDeck); break;
                                case "Secret Weapon": PlaySecretWeapon(eventDeck); break;
                                case "Forewarned": PlayForewarned(eventDeck); break;
                                case "Chartered Carriage": PlayCharteredCarriage(eventDeck); break;
                                case "Excellent Weather": PlayExcellentWeather(eventDeck); break;
                                case "Escape Route": PlayEscapeRoute(eventDeck); break;
                                case "Hired Scouts": PlayHiredScouts(eventDeck); break;
                            }
                            break;

                        }
                    case "exit": break;
                    default: Console.WriteLine("I don't know what you're talking about"); break;
                }
            } while (command != "exit");
        }

        public static string PlayEventCard(List<Event> events, Hunter[] hunters)
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
                eventIndex = events.FindIndex(card => card.name.ToUpper().StartsWith(line.ToUpper()));
                if (eventIndex == -1) {
                    Console.WriteLine("I don't recognise a card starting with " + line);
                } else if (events[eventIndex].isDraculaCard)
                {
                    Console.WriteLine(events[eventIndex].name + " is Dracula's card");
                    eventIndex = -1;
                }
            } while (eventIndex == -1);
            Console.WriteLine(hunters[hunterIndex - 1].name + " is playing event card " + events[eventIndex].name);
            return events[eventIndex].name;
        }

        private static void PlayHiredScouts(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayEscapeRoute(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayExcellentWeather(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayCharteredCarriage(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayForewarned(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlaySecretWeapon(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayBloodTransfusion(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayGoodLuck(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlaySurprisingReturn(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayStormySeas(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayHypnosis(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayTelegraphAhead(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayConsecratedGround(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayReEquip(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayNewspaperReports(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayAdvancePlanning(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayMysticResearch(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayLongDay(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayVampiricLair(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlaySenseOfEmergency(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayMoneyTrail(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayGreatStrength(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayHeroicLeap(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlaySisterAgatha(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayJonathanHarker(List<Event> events)
        {
            throw new NotImplementedException();
        }

        private static void PlayRufusSmith(List<Event> events, List<Event> allySlot)
        {
            allySlot.Clear();
            allySlot.Add(events[events.FindIndex(card => card.name == "Rufus Smith")]);
            events.Remove(allySlot.First());
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

        public static void drawTrail(Dracula dracula, string timeOfDay, List<Event> draculaAlly, List<Event> hunterAlly)
        {
            Console.WriteLine("6th 5th 4th 3rd 2nd 1st   Time        Dracula  Vampires  Catacombs");
            for (int i = 5; i >= 0; i--)
            {
                if (i + 1 > dracula.trail.Count())
                {
                    Console.Write("    ");
                }
                else
                {
                    dracula.trail[i].DrawLocation();
                }
            }

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
            Console.Write(dracula.blood);
            Console.ResetColor();
            for (int i = 0; i < (9 - dracula.blood.ToString().Length); i++)
            {
                Console.Write(" ");
            }
            Console.Write(Math.Max(0, dracula.vampireTracker));
            for (int i = 0; i < (10 - dracula.vampireTracker.ToString().Length); i++)
            {
                Console.Write(" ");
            }
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < 3; i++)
            {
                if (dracula.catacombs[i] != null)
                {
                    if (dracula.catacombs[i].isRevealed)
                    {
                        Console.Write(dracula.catacombs[i].abbreviation + " ");
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
                for (int i = 0; i < dracula.powers.Count(); i++)
                {
                    if (dracula.powers[i].positionInTrail == counter && dracula.powers[i].name != "Hide" && dracula.powers[i].name != "Dark Call" && dracula.powers[i].name != "Feed")
                    {
                        tempString = dracula.powers[i].name.Substring(0, 3).ToUpper() + " ";
                    }
                }
                Console.Write(tempString);
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("                                 ");

            for (int i = 0; i < 3; i++)
            {
                if (dracula.catacombs[i] != null)
                {
                    if (dracula.catacombs[i].encounters.Count > 0)
                    {
                        dracula.catacombs[i].DrawEncounter();
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
                if (i + 1 > dracula.trail.Count())
                {
                    Console.Write("    ");
                }
                else
                {
                    dracula.trail[i].DrawEncounter();
                }
            }
            Console.ResetColor();
            Console.Write("  Dracula's Ally    Hunters' Ally");

            Console.WriteLine("");
            for (int i = 5; i > -1; i--)
            {
                if (i + 1 > dracula.trail.Count())
                {
                    Console.Write("    ");
                }
                else
                {
                    dracula.trail[i].DrawEncounter(true);
                }
            }
            Console.ResetColor();
            Console.Write("  ");
            if (draculaAlly.Count() > 0)
            {
                Console.Write(draculaAlly.First().name.Substring(0, 3).ToUpper());
            } else
            {
                Console.Write("   ");
            }
            Console.Write("               ");
            if (hunterAlly.Count > 0)
            {
                Console.Write(hunterAlly.First().name.Substring(0, 3).ToUpper());
            }
            else
            {
                Console.Write("   ");
            }

            Console.Write("          ");
            for (int i = 0; i < 3; i++)
            {
                if (dracula.catacombs[i] != null)
                {
                    if (dracula.catacombs[i].encounters.Count > 0)
                    {
                        dracula.catacombs[i].DrawEncounter(true);
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

        public static void SetupMap(List<Location> map)
        {
            Location galway = new Location();
            Location dublin = new Location();
            Location liverpool = new Location();
            Location edinburgh = new Location();
            Location manchester = new Location();
            Location swansea = new Location();
            Location plymouth = new Location();
            Location nantes = new Location();
            Location lehavre = new Location();
            Location london = new Location();
            Location paris = new Location();
            Location brussels = new Location();
            Location amsterdam = new Location();
            Location strasbourg = new Location();
            Location cologne = new Location();
            Location hamburg = new Location();
            Location frankfurt = new Location();
            Location nuremburg = new Location();
            Location leipzig = new Location();
            Location berlin = new Location();
            Location prague = new Location();
            Location castledracula = new Location();
            Location santander = new Location();
            Location saragossa = new Location();
            Location bordeaux = new Location();
            Location toulouse = new Location();
            Location barcelona = new Location();
            Location clermontferrand = new Location();
            Location marseilles = new Location();
            Location geneva = new Location();
            Location genoa = new Location();
            Location milan = new Location();
            Location zurich = new Location();
            Location florence = new Location();
            Location venice = new Location();
            Location munich = new Location();
            Location zagreb = new Location();
            Location vienna = new Location();
            Location stjosephandstmary = new Location();
            Location sarajevo = new Location();
            Location szeged = new Location();
            Location budapest = new Location();
            Location belgrade = new Location();
            Location klausenburg = new Location();
            Location sofia = new Location();
            Location bucharest = new Location();
            Location galatz = new Location();
            Location varna = new Location();
            Location constanta = new Location();
            Location lisbon = new Location();
            Location cadiz = new Location();
            Location madrid = new Location();
            Location granada = new Location();
            Location alicante = new Location();
            Location cagliari = new Location();
            Location rome = new Location();
            Location naples = new Location();
            Location bari = new Location();
            Location valona = new Location();
            Location salonica = new Location();
            Location athens = new Location();
            Location atlanticocean = new Location();
            Location irishsea = new Location();
            Location englishchannel = new Location();
            Location northsea = new Location();
            Location bayofbiscay = new Location();
            Location mediterraneansea = new Location();
            Location tyrrheniansea = new Location();
            Location adriaticsea = new Location();
            Location ioniansea = new Location();
            Location blacksea = new Location();

            galway.name = "Galway";
            galway.abbreviation = "GAW";
            galway.type = LocationType.Town;
            galway.isEastern = false;
            galway.byRoad.Add(dublin);
            galway.bySea.Add(atlanticocean);
            map.Add(galway);

            dublin.name = "Dublin";
            dublin.abbreviation = "DUB";
            dublin.type = LocationType.Town;
            dublin.isEastern = false;
            dublin.byRoad.Add(galway);
            dublin.bySea.Add(irishsea);
            map.Add(dublin);

            liverpool.name = "Liverpool";
            liverpool.abbreviation = "LIV";
            liverpool.type = LocationType.City;
            liverpool.isEastern = false;
            liverpool.byRoad.Add(manchester);
            liverpool.byRoad.Add(swansea);
            liverpool.byTrain.Add(manchester);
            liverpool.bySea.Add(irishsea);
            map.Add(liverpool);

            edinburgh.name = "Edinburgh";
            edinburgh.abbreviation = "EDI";
            edinburgh.type = LocationType.City;
            edinburgh.isEastern = false;
            edinburgh.byRoad.Add(manchester);
            edinburgh.byTrain.Add(manchester);
            edinburgh.bySea.Add(northsea);
            map.Add(edinburgh);

            manchester.name = "Manchester";
            manchester.abbreviation = "MAN";
            manchester.type = LocationType.City;
            manchester.isEastern = false;
            manchester.byRoad.Add(edinburgh);
            manchester.byRoad.Add(liverpool);
            manchester.byRoad.Add(london);
            manchester.byTrain.Add(edinburgh);
            manchester.byTrain.Add(liverpool);
            manchester.byTrain.Add(london);
            map.Add(manchester);

            swansea.name = "Swansea";
            swansea.abbreviation = "SWA";
            swansea.type = LocationType.Town;
            swansea.isEastern = false;
            swansea.byRoad.Add(liverpool);
            swansea.byRoad.Add(london);
            swansea.byTrain.Add(london);
            swansea.bySea.Add(irishsea);
            map.Add(swansea);

            plymouth.name = "Plymouth";
            plymouth.abbreviation = "PLY";
            plymouth.type = LocationType.Town;
            plymouth.isEastern = false;
            plymouth.byRoad.Add(london);
            plymouth.bySea.Add(englishchannel);
            map.Add(plymouth);

            nantes.name = "Nantes";
            nantes.abbreviation = "NAN";
            nantes.type = LocationType.City;
            nantes.isEastern = false;
            nantes.byRoad.Add(lehavre);
            nantes.byRoad.Add(paris);
            nantes.byRoad.Add(clermontferrand);
            nantes.byRoad.Add(bordeaux);
            nantes.bySea.Add(bayofbiscay);
            map.Add(nantes);

            lehavre.name = "Le Havre";
            lehavre.abbreviation = "LEH";
            lehavre.type = LocationType.Town;
            lehavre.isEastern = false;
            lehavre.byRoad.Add(nantes);
            lehavre.byRoad.Add(paris);
            lehavre.byRoad.Add(brussels);
            lehavre.byTrain.Add(paris);
            lehavre.bySea.Add(englishchannel);
            map.Add(lehavre);

            london.name = "London";
            london.abbreviation = "LON";
            london.type = LocationType.City;
            london.isEastern = false;
            london.byRoad.Add(manchester);
            london.byRoad.Add(swansea);
            london.byRoad.Add(plymouth);
            london.byTrain.Add(manchester);
            london.byTrain.Add(swansea);
            london.bySea.Add(englishchannel);
            map.Add(london);

            paris.name = "Paris";
            paris.abbreviation = "PAR";
            paris.type = LocationType.City;
            paris.isEastern = false;
            paris.byRoad.Add(nantes);
            paris.byRoad.Add(lehavre);
            paris.byRoad.Add(brussels);
            paris.byRoad.Add(strasbourg);
            paris.byRoad.Add(geneva);
            paris.byRoad.Add(clermontferrand);
            paris.byTrain.Add(lehavre);
            paris.byTrain.Add(brussels);
            paris.byTrain.Add(marseilles);
            paris.byTrain.Add(bordeaux);
            map.Add(paris);

            brussels.name = "Brussels";
            brussels.abbreviation = "BRU";
            brussels.type = LocationType.City;
            brussels.isEastern = false;
            brussels.byRoad.Add(lehavre);
            brussels.byRoad.Add(amsterdam);
            brussels.byRoad.Add(cologne);
            brussels.byRoad.Add(strasbourg);
            brussels.byRoad.Add(paris);
            brussels.byTrain.Add(cologne);
            brussels.byTrain.Add(paris);
            map.Add(brussels);

            amsterdam.name = "Amsterdam";
            amsterdam.abbreviation = "AMS";
            amsterdam.type = LocationType.City;
            amsterdam.isEastern = false;
            amsterdam.byRoad.Add(brussels);
            amsterdam.byRoad.Add(cologne);
            amsterdam.bySea.Add(northsea);
            map.Add(amsterdam);

            strasbourg.name = "Strasbourg";
            strasbourg.abbreviation = "STR";
            strasbourg.type = LocationType.Town;
            strasbourg.isEastern = false;
            strasbourg.byRoad.Add(paris);
            strasbourg.byRoad.Add(brussels);
            strasbourg.byRoad.Add(cologne);
            strasbourg.byRoad.Add(frankfurt);
            strasbourg.byRoad.Add(nuremburg);
            strasbourg.byRoad.Add(munich);
            strasbourg.byRoad.Add(zurich);
            strasbourg.byRoad.Add(geneva);
            strasbourg.byTrain.Add(frankfurt);
            strasbourg.byTrain.Add(zurich);
            map.Add(strasbourg);

            cologne.name = "Cologne";
            cologne.abbreviation = "COL";
            cologne.type = LocationType.City;
            cologne.isEastern = false;
            cologne.byRoad.Add(brussels);
            cologne.byRoad.Add(amsterdam);
            cologne.byRoad.Add(hamburg);
            cologne.byRoad.Add(leipzig);
            cologne.byRoad.Add(frankfurt);
            cologne.byRoad.Add(strasbourg);
            cologne.byTrain.Add(brussels);
            cologne.byTrain.Add(frankfurt);
            map.Add(cologne);

            hamburg.name = "Hamburg";
            hamburg.abbreviation = "HAM";
            hamburg.type = LocationType.City;
            hamburg.isEastern = false;
            hamburg.byRoad.Add(cologne);
            hamburg.byRoad.Add(berlin);
            hamburg.byRoad.Add(leipzig);
            hamburg.byTrain.Add(berlin);
            hamburg.bySea.Add(northsea);
            map.Add(hamburg);

            frankfurt.name = "Frankfurt";
            frankfurt.abbreviation = "FRA";
            frankfurt.type = LocationType.Town;
            frankfurt.isEastern = false;
            frankfurt.byRoad.Add(strasbourg);
            frankfurt.byRoad.Add(cologne);
            frankfurt.byRoad.Add(leipzig);
            frankfurt.byRoad.Add(nuremburg);
            frankfurt.byTrain.Add(strasbourg);
            frankfurt.byTrain.Add(cologne);
            frankfurt.byTrain.Add(leipzig);
            map.Add(frankfurt);

            nuremburg.name = "Nuremburg";
            nuremburg.abbreviation = "NUR";
            nuremburg.type = LocationType.Town;
            nuremburg.isEastern = false;
            nuremburg.byRoad.Add(strasbourg);
            nuremburg.byRoad.Add(frankfurt);
            nuremburg.byRoad.Add(leipzig);
            nuremburg.byRoad.Add(prague);
            nuremburg.byRoad.Add(munich);
            nuremburg.byTrain.Add(leipzig);
            nuremburg.byTrain.Add(munich);
            map.Add(nuremburg);

            leipzig.name = "Leipzig";
            leipzig.abbreviation = "LEI";
            leipzig.type = LocationType.City;
            leipzig.isEastern = false;
            leipzig.byRoad.Add(cologne);
            leipzig.byRoad.Add(hamburg);
            leipzig.byRoad.Add(berlin);
            leipzig.byRoad.Add(nuremburg);
            leipzig.byRoad.Add(frankfurt);
            leipzig.byTrain.Add(frankfurt);
            leipzig.byTrain.Add(berlin);
            leipzig.byTrain.Add(nuremburg);
            map.Add(leipzig);

            berlin.name = "Berlin";
            berlin.abbreviation = "BER";
            berlin.type = LocationType.City;
            berlin.isEastern = false;
            berlin.byRoad.Add(hamburg);
            berlin.byRoad.Add(prague);
            berlin.byRoad.Add(leipzig);
            berlin.byTrain.Add(hamburg);
            berlin.byTrain.Add(leipzig);
            berlin.byTrain.Add(prague);
            map.Add(berlin);

            prague.name = "Prague";
            prague.abbreviation = "PRA";
            prague.type = LocationType.City;
            prague.isEastern = true;
            prague.byRoad.Add(berlin);
            prague.byRoad.Add(vienna);
            prague.byRoad.Add(nuremburg);
            prague.byTrain.Add(berlin);
            prague.byTrain.Add(vienna);
            map.Add(prague);

            castledracula.name = "Castle Dracula";
            castledracula.abbreviation = "CAS";
            castledracula.type = LocationType.Castle;
            castledracula.isEastern = true;
            castledracula.byRoad.Add(klausenburg);
            castledracula.byRoad.Add(galatz);
            map.Add(castledracula);

            santander.name = "Santander";
            santander.abbreviation = "SAN";
            santander.type = LocationType.Town;
            santander.isEastern = false;
            santander.byRoad.Add(lisbon);
            santander.byRoad.Add(madrid);
            santander.byRoad.Add(saragossa);
            santander.byTrain.Add(madrid);
            santander.bySea.Add(bayofbiscay);
            map.Add(santander);

            saragossa.name = "Saragossa";
            saragossa.abbreviation = "SAG";
            saragossa.type = LocationType.Town;
            saragossa.isEastern = false;
            saragossa.byRoad.Add(madrid);
            saragossa.byRoad.Add(santander);
            saragossa.byRoad.Add(bordeaux);
            saragossa.byRoad.Add(toulouse);
            saragossa.byRoad.Add(barcelona);
            saragossa.byRoad.Add(alicante);
            saragossa.byTrain.Add(madrid);
            saragossa.byTrain.Add(bordeaux);
            saragossa.byTrain.Add(barcelona);
            map.Add(saragossa);

            bordeaux.name = "Bordeaux";
            bordeaux.abbreviation = "BOR";
            bordeaux.type = LocationType.City;
            bordeaux.isEastern = false;
            bordeaux.byRoad.Add(saragossa);
            bordeaux.byRoad.Add(nantes);
            bordeaux.byRoad.Add(clermontferrand);
            bordeaux.byRoad.Add(toulouse);
            bordeaux.byTrain.Add(paris);
            bordeaux.byTrain.Add(saragossa);
            bordeaux.bySea.Add(bayofbiscay);
            map.Add(bordeaux);

            toulouse.name = "Toulouse";
            toulouse.abbreviation = "TOU";
            toulouse.type = LocationType.Town;
            toulouse.isEastern = false;
            toulouse.byRoad.Add(saragossa);
            toulouse.byRoad.Add(bordeaux);
            toulouse.byRoad.Add(clermontferrand);
            toulouse.byRoad.Add(marseilles);
            toulouse.byRoad.Add(barcelona);
            map.Add(toulouse);

            barcelona.name = "Barcelona";
            barcelona.abbreviation = "BAC";
            barcelona.type = LocationType.City;
            barcelona.isEastern = false;
            barcelona.byRoad.Add(saragossa);
            barcelona.byRoad.Add(toulouse);
            barcelona.byTrain.Add(saragossa);
            barcelona.byTrain.Add(alicante);
            barcelona.bySea.Add(mediterraneansea);
            map.Add(barcelona);

            clermontferrand.name = "Clermont Ferrand";
            clermontferrand.abbreviation = "CLE";
            clermontferrand.type = LocationType.Town;
            clermontferrand.isEastern = false;
            clermontferrand.byRoad.Add(bordeaux);
            clermontferrand.byRoad.Add(nantes);
            clermontferrand.byRoad.Add(paris);
            clermontferrand.byRoad.Add(geneva);
            clermontferrand.byRoad.Add(marseilles);
            clermontferrand.byRoad.Add(toulouse);
            map.Add(clermontferrand);

            marseilles.name = "Marseilles";
            marseilles.abbreviation = "MAR";
            marseilles.type = LocationType.City;
            marseilles.isEastern = false;
            marseilles.byRoad.Add(toulouse);
            marseilles.byRoad.Add(clermontferrand);
            marseilles.byRoad.Add(geneva);
            marseilles.byRoad.Add(zurich);
            marseilles.byRoad.Add(milan);
            marseilles.byRoad.Add(genoa);
            marseilles.byTrain.Add(paris);
            marseilles.bySea.Add(mediterraneansea);
            map.Add(marseilles);

            geneva.name = "Geneva";
            geneva.abbreviation = "GEV";
            geneva.type = LocationType.Town;
            geneva.isEastern = false;
            geneva.byRoad.Add(marseilles);
            geneva.byRoad.Add(clermontferrand);
            geneva.byRoad.Add(paris);
            geneva.byRoad.Add(strasbourg);
            geneva.byRoad.Add(zurich);
            geneva.byTrain.Add(milan);
            map.Add(geneva);

            genoa.name = "Genoa";
            genoa.abbreviation = "GEO";
            genoa.type = LocationType.City;
            genoa.isEastern = true;
            genoa.byRoad.Add(marseilles);
            genoa.byRoad.Add(milan);
            genoa.byRoad.Add(venice);
            genoa.byRoad.Add(florence);
            genoa.byTrain.Add(milan);
            genoa.bySea.Add(tyrrheniansea);
            map.Add(genoa);

            milan.name = "Milan";
            milan.abbreviation = "MIL";
            milan.type = LocationType.City;
            milan.isEastern = true;
            milan.byRoad.Add(marseilles);
            milan.byRoad.Add(zurich);
            milan.byRoad.Add(munich);
            milan.byRoad.Add(venice);
            milan.byRoad.Add(genoa);
            milan.byTrain.Add(geneva);
            milan.byTrain.Add(zurich);
            milan.byTrain.Add(florence);
            milan.byTrain.Add(genoa);
            map.Add(milan);

            zurich.name = "Zurich";
            zurich.abbreviation = "ZUR";
            zurich.type = LocationType.Town;
            zurich.isEastern = false;
            zurich.byRoad.Add(marseilles);
            zurich.byRoad.Add(geneva);
            zurich.byRoad.Add(strasbourg);
            zurich.byRoad.Add(munich);
            zurich.byRoad.Add(milan);
            zurich.byTrain.Add(strasbourg);
            zurich.byTrain.Add(milan);
            map.Add(zurich);

            florence.name = "Florence";
            florence.abbreviation = "FLO";
            florence.type = LocationType.Town;
            florence.isEastern = true;
            florence.byRoad.Add(genoa);
            florence.byRoad.Add(venice);
            florence.byRoad.Add(rome);
            florence.byTrain.Add(milan);
            florence.byTrain.Add(rome);
            map.Add(florence);

            venice.name = "Venice";
            venice.abbreviation = "VEN";
            venice.type = LocationType.Town;
            venice.isEastern = true;
            venice.byRoad.Add(florence);
            venice.byRoad.Add(genoa);
            venice.byRoad.Add(milan);
            venice.byRoad.Add(munich);
            venice.byTrain.Add(vienna);
            venice.bySea.Add(adriaticsea);
            map.Add(venice);

            munich.name = "Munich";
            munich.abbreviation = "MUN";
            munich.type = LocationType.City;
            munich.isEastern = false;
            munich.byRoad.Add(milan);
            munich.byRoad.Add(zurich);
            munich.byRoad.Add(strasbourg);
            munich.byRoad.Add(nuremburg);
            munich.byRoad.Add(vienna);
            munich.byRoad.Add(zagreb);
            munich.byRoad.Add(venice);
            munich.byTrain.Add(nuremburg);
            map.Add(munich);

            zagreb.name = "Zagreb";
            zagreb.abbreviation = "ZAG";
            zagreb.type = LocationType.Town;
            zagreb.isEastern = true;
            zagreb.byRoad.Add(munich);
            zagreb.byRoad.Add(vienna);
            zagreb.byRoad.Add(budapest);
            zagreb.byRoad.Add(szeged);
            zagreb.byRoad.Add(stjosephandstmary);
            zagreb.byRoad.Add(sarajevo);
            map.Add(zagreb);

            vienna.name = "Vienna";
            vienna.abbreviation = "VIE";
            vienna.type = LocationType.City;
            vienna.isEastern = true;
            vienna.byRoad.Add(munich);
            vienna.byRoad.Add(prague);
            vienna.byRoad.Add(budapest);
            vienna.byRoad.Add(zagreb);
            vienna.byTrain.Add(venice);
            vienna.byTrain.Add(prague);
            vienna.byTrain.Add(budapest);
            map.Add(vienna);

            stjosephandstmary.name = "St. Joseph & St. Mary";
            stjosephandstmary.abbreviation = "STJ";
            stjosephandstmary.type = LocationType.Hospital;
            stjosephandstmary.isEastern = true;
            stjosephandstmary.byRoad.Add(zagreb);
            stjosephandstmary.byRoad.Add(szeged);
            stjosephandstmary.byRoad.Add(belgrade);
            stjosephandstmary.byRoad.Add(sarajevo);
            map.Add(stjosephandstmary);

            sarajevo.name = "Sarajevo";
            sarajevo.abbreviation = "SAJ";
            sarajevo.type = LocationType.Town;
            sarajevo.isEastern = true;
            sarajevo.byRoad.Add(zagreb);
            sarajevo.byRoad.Add(stjosephandstmary);
            sarajevo.byRoad.Add(belgrade);
            sarajevo.byRoad.Add(sofia);
            sarajevo.byRoad.Add(valona);
            map.Add(sarajevo);

            szeged.name = "Szeged";
            szeged.abbreviation = "SZE";
            szeged.type = LocationType.Town;
            szeged.isEastern = true;
            szeged.byRoad.Add(zagreb);
            szeged.byRoad.Add(budapest);
            szeged.byRoad.Add(klausenburg);
            szeged.byRoad.Add(belgrade);
            szeged.byRoad.Add(stjosephandstmary);
            szeged.byTrain.Add(budapest);
            szeged.byTrain.Add(bucharest);
            szeged.byTrain.Add(belgrade);
            map.Add(szeged);

            budapest.name = "Budapest";
            budapest.abbreviation = "BUD";
            budapest.type = LocationType.City;
            budapest.isEastern = true;
            budapest.byRoad.Add(vienna);
            budapest.byRoad.Add(klausenburg);
            budapest.byRoad.Add(szeged);
            budapest.byRoad.Add(zagreb);
            budapest.byTrain.Add(vienna);
            budapest.byTrain.Add(szeged);
            map.Add(budapest);

            belgrade.name = "Belgrade";
            belgrade.abbreviation = "BEL";
            belgrade.type = LocationType.Town;
            belgrade.isEastern = true;
            belgrade.byRoad.Add(stjosephandstmary);
            belgrade.byRoad.Add(szeged);
            belgrade.byRoad.Add(klausenburg);
            belgrade.byRoad.Add(bucharest);
            belgrade.byRoad.Add(sofia);
            belgrade.byRoad.Add(sarajevo);
            belgrade.byTrain.Add(szeged);
            belgrade.byTrain.Add(sofia);
            map.Add(belgrade);

            klausenburg.name = "Klausenburg";
            klausenburg.abbreviation = "KLA";
            klausenburg.type = LocationType.Town;
            klausenburg.isEastern = true;
            klausenburg.byRoad.Add(budapest);
            klausenburg.byRoad.Add(castledracula);
            klausenburg.byRoad.Add(galatz);
            klausenburg.byRoad.Add(bucharest);
            klausenburg.byRoad.Add(belgrade);
            klausenburg.byRoad.Add(szeged);
            map.Add(klausenburg);

            sofia.name = "Sofia";
            sofia.abbreviation = "SOF";
            sofia.type = LocationType.Town;
            sofia.isEastern = true;
            sofia.byRoad.Add(sarajevo);
            sofia.byRoad.Add(belgrade);
            sofia.byRoad.Add(bucharest);
            sofia.byRoad.Add(varna);
            sofia.byRoad.Add(salonica);
            sofia.byRoad.Add(valona);
            sofia.byTrain.Add(belgrade);
            sofia.byTrain.Add(salonica);
            map.Add(sofia);

            bucharest.name = "Bucharest";
            bucharest.abbreviation = "BUC";
            bucharest.type = LocationType.City;
            bucharest.isEastern = true;
            bucharest.byRoad.Add(belgrade);
            bucharest.byRoad.Add(klausenburg);
            bucharest.byRoad.Add(galatz);
            bucharest.byRoad.Add(constanta);
            bucharest.byRoad.Add(sofia);
            bucharest.byTrain.Add(szeged);
            bucharest.byTrain.Add(galatz);
            bucharest.byTrain.Add(constanta);
            map.Add(bucharest);

            galatz.name = "Galatz";
            galatz.abbreviation = "GAT";
            galatz.type = LocationType.Town;
            galatz.isEastern = true;
            galatz.byRoad.Add(klausenburg);
            galatz.byRoad.Add(castledracula);
            galatz.byRoad.Add(constanta);
            galatz.byRoad.Add(bucharest);
            galatz.byTrain.Add(bucharest);
            map.Add(galatz);

            varna.name = "Varna";
            varna.abbreviation = "VAR";
            varna.type = LocationType.City;
            varna.isEastern = true;
            varna.byRoad.Add(sofia);
            varna.byRoad.Add(constanta);
            varna.byTrain.Add(sofia);
            varna.bySea.Add(blacksea);
            map.Add(varna);

            constanta.name = "Constanta";
            constanta.abbreviation = "CON";
            constanta.type = LocationType.City;
            constanta.isEastern = true;
            constanta.byRoad.Add(galatz);
            constanta.byRoad.Add(varna);
            constanta.byRoad.Add(bucharest);
            constanta.byTrain.Add(bucharest);
            constanta.bySea.Add(blacksea);
            map.Add(constanta);

            lisbon.name = "Lisbon";
            lisbon.abbreviation = "LIS";
            lisbon.type = LocationType.City;
            lisbon.isEastern = false;
            lisbon.byRoad.Add(santander);
            lisbon.byRoad.Add(madrid);
            lisbon.byRoad.Add(cadiz);
            lisbon.byTrain.Add(madrid);
            lisbon.bySea.Add(atlanticocean);
            map.Add(lisbon);

            cadiz.name = "Cadiz";
            cadiz.abbreviation = "CAD";
            cadiz.type = LocationType.City;
            cadiz.isEastern = false;
            cadiz.byRoad.Add(lisbon);
            cadiz.byRoad.Add(madrid);
            cadiz.byRoad.Add(granada);
            cadiz.bySea.Add(atlanticocean);
            map.Add(cadiz);

            madrid.name = "Madrid";
            madrid.abbreviation = "MAD";
            madrid.type = LocationType.City;
            madrid.isEastern = false;
            madrid.byRoad.Add(lisbon);
            madrid.byRoad.Add(santander);
            madrid.byRoad.Add(saragossa);
            madrid.byRoad.Add(alicante);
            madrid.byRoad.Add(granada);
            madrid.byRoad.Add(cadiz);
            madrid.byTrain.Add(lisbon);
            madrid.byTrain.Add(santander);
            madrid.byTrain.Add(saragossa);
            madrid.byTrain.Add(alicante);
            map.Add(madrid);

            granada.name = "Granada";
            granada.abbreviation = "GRA";
            granada.type = LocationType.Town;
            granada.isEastern = false;
            granada.byRoad.Add(cadiz);
            granada.byRoad.Add(madrid);
            granada.byRoad.Add(alicante);
            map.Add(granada);

            alicante.name = "Alicante";
            alicante.abbreviation = "ALI";
            alicante.type = LocationType.Town;
            alicante.isEastern = false;
            alicante.byRoad.Add(granada);
            alicante.byRoad.Add(madrid);
            alicante.byRoad.Add(saragossa);
            alicante.byTrain.Add(madrid);
            alicante.byTrain.Add(barcelona);
            alicante.bySea.Add(mediterraneansea);
            map.Add(alicante);

            cagliari.name = "Cagliari";
            cagliari.abbreviation = "CAG";
            cagliari.type = LocationType.Town;
            cagliari.isEastern = true;
            cagliari.bySea.Add(mediterraneansea);
            cagliari.bySea.Add(tyrrheniansea);
            map.Add(cagliari);

            rome.name = "Rome";
            rome.abbreviation = "ROM";
            rome.type = LocationType.City;
            rome.isEastern = true;
            rome.byRoad.Add(florence);
            rome.byRoad.Add(bari);
            rome.byRoad.Add(naples);
            rome.byTrain.Add(florence);
            rome.byTrain.Add(naples);
            rome.bySea.Add(tyrrheniansea);
            map.Add(rome);

            naples.name = "Naples";
            naples.abbreviation = "NAP";
            naples.type = LocationType.City;
            naples.isEastern = true;
            naples.byRoad.Add(rome);
            naples.byRoad.Add(bari);
            naples.byTrain.Add(rome);
            naples.byTrain.Add(bari);
            naples.bySea.Add(tyrrheniansea);
            map.Add(naples);

            bari.name = "Bari";
            bari.abbreviation = "BAI";
            bari.type = LocationType.Town;
            bari.isEastern = true;
            bari.byRoad.Add(naples);
            bari.byRoad.Add(rome);
            bari.byTrain.Add(naples);
            bari.bySea.Add(adriaticsea);
            map.Add(bari);

            valona.name = "Valona";
            valona.abbreviation = "VAL";
            valona.type = LocationType.Town;
            valona.isEastern = true;
            valona.byRoad.Add(sarajevo);
            valona.byRoad.Add(sofia);
            valona.byRoad.Add(salonica);
            valona.byRoad.Add(athens);
            valona.bySea.Add(ioniansea);
            map.Add(valona);

            salonica.name = "Salonica";
            salonica.abbreviation = "SAL";
            salonica.type = LocationType.Town;
            salonica.isEastern = true;
            salonica.byRoad.Add(valona);
            salonica.byRoad.Add(sofia);
            salonica.byTrain.Add(sofia);
            salonica.bySea.Add(ioniansea);
            map.Add(salonica);

            athens.name = "Athens";
            athens.abbreviation = "ATH";
            athens.type = LocationType.City;
            athens.isEastern = true;
            athens.byRoad.Add(valona);
            athens.bySea.Add(ioniansea);
            map.Add(athens);

            atlanticocean.name = "Atlantic Ocean";
            atlanticocean.abbreviation = "ATL";
            atlanticocean.type = LocationType.Sea;
            atlanticocean.isEastern = false;
            atlanticocean.bySea.Add(northsea);
            atlanticocean.bySea.Add(irishsea);
            atlanticocean.bySea.Add(englishchannel);
            atlanticocean.bySea.Add(bayofbiscay);
            atlanticocean.bySea.Add(mediterraneansea);
            atlanticocean.bySea.Add(galway);
            atlanticocean.bySea.Add(lisbon);
            atlanticocean.bySea.Add(cadiz);
            map.Add(atlanticocean);

            irishsea.name = "Irish Sea";
            irishsea.abbreviation = "IRI";
            irishsea.type = LocationType.Sea;
            irishsea.isEastern = false;
            irishsea.bySea.Add(atlanticocean);
            irishsea.bySea.Add(dublin);
            irishsea.bySea.Add(liverpool);
            irishsea.bySea.Add(swansea);
            map.Add(irishsea);

            englishchannel.name = "English Channel";
            englishchannel.abbreviation = "ENG";
            englishchannel.type = LocationType.Sea;
            englishchannel.isEastern = false;
            englishchannel.bySea.Add(atlanticocean);
            englishchannel.bySea.Add(northsea);
            englishchannel.bySea.Add(plymouth);
            englishchannel.bySea.Add(london);
            englishchannel.bySea.Add(lehavre);
            map.Add(englishchannel);

            northsea.name = "North Sea";
            northsea.abbreviation = "NOR";
            northsea.type = LocationType.Sea;
            northsea.isEastern = false;
            northsea.bySea.Add(atlanticocean);
            northsea.bySea.Add(englishchannel);
            northsea.bySea.Add(edinburgh);
            northsea.bySea.Add(amsterdam);
            northsea.bySea.Add(hamburg);
            map.Add(northsea);

            bayofbiscay.name = "Bay of Biscay";
            bayofbiscay.abbreviation = "BAY";
            bayofbiscay.type = LocationType.Sea;
            bayofbiscay.isEastern = false;
            bayofbiscay.bySea.Add(atlanticocean);
            bayofbiscay.bySea.Add(nantes);
            bayofbiscay.bySea.Add(bordeaux);
            bayofbiscay.bySea.Add(santander);
            map.Add(bayofbiscay);

            mediterraneansea.name = "Mediterranean Sea";
            mediterraneansea.abbreviation = "MED";
            mediterraneansea.type = LocationType.Sea;
            mediterraneansea.isEastern = true;
            mediterraneansea.bySea.Add(atlanticocean);
            mediterraneansea.bySea.Add(tyrrheniansea);
            mediterraneansea.bySea.Add(alicante);
            mediterraneansea.bySea.Add(barcelona);
            mediterraneansea.bySea.Add(marseilles);
            mediterraneansea.bySea.Add(cagliari);
            map.Add(mediterraneansea);

            tyrrheniansea.name = "Tyrrhenian Sea";
            tyrrheniansea.abbreviation = "TYR";
            tyrrheniansea.type = LocationType.Sea;
            tyrrheniansea.isEastern = false;
            tyrrheniansea.bySea.Add(mediterraneansea);
            tyrrheniansea.bySea.Add(ioniansea);
            tyrrheniansea.bySea.Add(cagliari);
            tyrrheniansea.bySea.Add(genoa);
            tyrrheniansea.bySea.Add(rome);
            tyrrheniansea.bySea.Add(naples);
            map.Add(tyrrheniansea);

            adriaticsea.name = "Adriatic Sea";
            adriaticsea.abbreviation = "ADR";
            adriaticsea.type = LocationType.Sea;
            adriaticsea.isEastern = false;
            adriaticsea.bySea.Add(ioniansea);
            adriaticsea.bySea.Add(bari);
            adriaticsea.bySea.Add(venice);
            map.Add(adriaticsea);

            ioniansea.name = "Ionian Sea";
            ioniansea.abbreviation = "ION";
            ioniansea.type = LocationType.Sea;
            ioniansea.isEastern = false;
            ioniansea.bySea.Add(mediterraneansea);
            ioniansea.bySea.Add(adriaticsea);
            ioniansea.bySea.Add(blacksea);
            ioniansea.bySea.Add(valona);
            ioniansea.bySea.Add(athens);
            ioniansea.bySea.Add(salonica);
            map.Add(ioniansea);

            blacksea.name = "Black Sea";
            blacksea.abbreviation = "BLA";
            blacksea.type = LocationType.Sea;
            blacksea.isEastern = false;
            blacksea.bySea.Add(ioniansea);
            blacksea.bySea.Add(varna);
            blacksea.bySea.Add(constanta);
            map.Add(blacksea);
        }

        public static void SetupEncounters(List<Encounter> encounters)
        {
            encounters.Add(new Encounter("Ambush", "AMB"));
            encounters.Add(new Encounter("Ambush", "AMB"));
            encounters.Add(new Encounter("Ambush", "AMB"));
            encounters.Add(new Encounter("Assasin", "ASS"));
            encounters.Add(new Encounter("Bats", "BAT"));
            encounters.Add(new Encounter("Bats", "BAT"));
            encounters.Add(new Encounter("Bats", "BAT"));
            encounters.Add(new Encounter("Desecrated Soil", "DES"));
            encounters.Add(new Encounter("Desecrated Soil", "DES"));
            encounters.Add(new Encounter("Desecrated Soil", "DES"));
            encounters.Add(new Encounter("Fog", "FOG"));
            encounters.Add(new Encounter("Fog", "FOG"));
            encounters.Add(new Encounter("Fog", "FOG"));
            encounters.Add(new Encounter("Fog", "FOG"));
            encounters.Add(new Encounter("Minion with Knife", "MIK"));
            encounters.Add(new Encounter("Minion with Knife", "MIK"));
            encounters.Add(new Encounter("Minion with Knife", "MIK"));
            encounters.Add(new Encounter("Minion with Knife and Pistol", "MIP"));
            encounters.Add(new Encounter("Minion with Knife and Pistol", "MIP"));
            encounters.Add(new Encounter("Minion with Knife and Rifle", "MIR"));
            encounters.Add(new Encounter("Minion with Knife and Rifle", "MIR"));
            encounters.Add(new Encounter("Hoax", "HOA"));
            encounters.Add(new Encounter("Hoax", "HOA"));
            encounters.Add(new Encounter("Lightning", "LIG"));
            encounters.Add(new Encounter("Lightning", "LIG"));
            encounters.Add(new Encounter("Peasants", "PEA"));
            encounters.Add(new Encounter("Peasants", "PEA"));
            encounters.Add(new Encounter("Plague", "PLA"));
            encounters.Add(new Encounter("Rats", "RAT"));
            encounters.Add(new Encounter("Rats", "RAT"));
            encounters.Add(new Encounter("Saboteur", "SAB"));
            encounters.Add(new Encounter("Saboteur", "SAB"));
            encounters.Add(new Encounter("Spy", "SPY"));
            encounters.Add(new Encounter("Spy", "SPY"));
            encounters.Add(new Encounter("Thief", "THI"));
            encounters.Add(new Encounter("Thief", "THI"));
            encounters.Add(new Encounter("New Vampire", "VAM"));
            encounters.Add(new Encounter("New Vampire", "VAM"));
            encounters.Add(new Encounter("New Vampire", "VAM"));
            encounters.Add(new Encounter("New Vampire", "VAM"));
            encounters.Add(new Encounter("New Vampire", "VAM"));
            encounters.Add(new Encounter("New Vampire", "VAM"));
            encounters.Add(new Encounter("Wolves", "WOL"));
            encounters.Add(new Encounter("Wolves", "WOL"));
            encounters.Add(new Encounter("Wolves", "WOL"));
        }

        public static void SetupEvents(List<Event> events)
        {
            events.Add(new Event("Rufus Smith", false));
            events.Add(new Event("Jonathan Harker", false));
            events.Add(new Event("Sister Agatha", false));
            events.Add(new Event("Heroic Leap", false));
            events.Add(new Event("Great Strength", false));
            events.Add(new Event("Money Trail", false));
            events.Add(new Event("Sense of Emergency", false));
            events.Add(new Event("Sense of Emergency", false));
            events.Add(new Event("Vampiric Lair", false));
            events.Add(new Event("Long Day", false));
            events.Add(new Event("Long Day", false));
            events.Add(new Event("Mystic Research", false));
            events.Add(new Event("Mystic Research", false));
            events.Add(new Event("Advance Planning", false));
            events.Add(new Event("Advance Planning", false));
            events.Add(new Event("Advance Planning", false));
            events.Add(new Event("Newspaper Reports", false));
            events.Add(new Event("Newspaper Reports", false));
            events.Add(new Event("Newspaper Reports", false));
            events.Add(new Event("Newspaper Reports", false));
            events.Add(new Event("Newspaper Reports", false));
            events.Add(new Event("Re-Equip", false));
            events.Add(new Event("Re-Equip", false));
            events.Add(new Event("Re-Equip", false));
            events.Add(new Event("Consecrated Ground", false));
            events.Add(new Event("Telegraph Ahead", false));
            events.Add(new Event("Telegraph Ahead", false));
            events.Add(new Event("Hypnosis", false));
            events.Add(new Event("Hypnosis", false));
            events.Add(new Event("Stormy Seas", false));
            events.Add(new Event("Surprising Return", false));
            events.Add(new Event("Surprising Return", false));
            events.Add(new Event("Good Luck", false));
            events.Add(new Event("Good Luck", false));
            events.Add(new Event("Blood Transfusion", false));
            events.Add(new Event("Secret Weapon", false));
            events.Add(new Event("Secret Weapon", false));
            events.Add(new Event("Forewarned", false));
            events.Add(new Event("Forewarned", false));
            events.Add(new Event("Forewarned", false));
            events.Add(new Event("Chartered Carriage", false));
            events.Add(new Event("Chartered Carriage", false));
            events.Add(new Event("Chartered Carriage", false));
            events.Add(new Event("Excellent Weather", false));
            events.Add(new Event("Excellent Weather", false));
            events.Add(new Event("Escape Route", false));
            events.Add(new Event("Escape Route", false));
            events.Add(new Event("Hired Scouts", false));
            events.Add(new Event("Hired Scouts", false));
            events.Add(new Event("Hired Scouts", false));
            events.Add(new Event("Dracula's Brides", true));
            events.Add(new Event("Immanuel Hildesheim", true));
            events.Add(new Event("Quincey P. Morris", true));
            events.Add(new Event("Roadblock", true));
            events.Add(new Event("Unearthly Swiftness", true));
            events.Add(new Event("Time Runs Short", true));
            events.Add(new Event("Customs Search", true));
            events.Add(new Event("Devilish Power", true));
            events.Add(new Event("Devilish Power", true));
            events.Add(new Event("Vampiric Influence", true));
            events.Add(new Event("Vampiric Influence", true));
            events.Add(new Event("Night Visit", true));
            events.Add(new Event("Evasion", true));
            events.Add(new Event("Wild Horses", true));
            events.Add(new Event("False Tip-off", true));
            events.Add(new Event("False Tip-off", true));
            events.Add(new Event("Sensationalist Press", true));
            events.Add(new Event("Rage", true));
            events.Add(new Event("Seduction", true));
            events.Add(new Event("Control Storms", true));
            events.Add(new Event("Relentless Minion", true));
            events.Add(new Event("Relentless Minion", true));
            events.Add(new Event("Trap", true));
            events.Add(new Event("Trap", true));
            events.Add(new Event("Trap", true));
        }
    }

}
