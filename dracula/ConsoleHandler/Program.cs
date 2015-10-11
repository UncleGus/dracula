using EventHandler;
using LocationHandler;
using LogHandler;
using System;
using System.Linq;

namespace ConsoleHandler
{
    class Program
    {

        static void Main(string[] args)
        {
            GameState g = new GameState();
            DecisionMaker logic = new DecisionMaker();
            UserInterface ui = new UserInterface();

            g.SetLocationForHunterAt(0, ui.GetHunterStartLocation(g, 0));
            g.SetLocationForHunterAt(1, ui.GetHunterStartLocation(g, 1));
            g.SetLocationForHunterAt(2, ui.GetHunterStartLocation(g, 2));
            g.SetLocationForHunterAt(3, ui.GetHunterStartLocation(g, 3));

            g.PlaceDraculaAtStartLocation(logic.DecideDraculaStartLocation(g));

            try
            {
                System.IO.File.Delete(@"debuglog.txt");
            }
            catch (System.IO.IOException)
            {
                ui.TellUser("Couldn't delete the old debug log file");
            }

            try
            {
                System.IO.File.Delete(@"gamelog.txt");
            }
            catch (System.IO.IOException)
            {
                ui.TellUser("Couldn't delete the old game log file");
            }

            Logger.WriteToDebugLog("Game start");

            Logger.WriteToDebugLog("Dracula started in " + g.DraculaCurrentLocationName());
            Logger.WriteToGameLog("Dracula started in " + g.DraculaCurrentLocationName());

            string line;
            string command;
            string argument1;
            string argument2 = "no argument";
            do
            {
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
                    try {
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

                switch (command)
                {
                    case "s": LocationHelper.ShowLocationDetails(g.GetLocationFromName(argument1)); break;
                    case "m": PerformDraculaTurn(g); break;
                    case "r": PerformRevealLocation(g, argument1); break;
                    case "e": PerformRevealEncounter(g, argument1); break;
                    case "c": PerformTrailClear(g, argument1); break;
                    case "v": PerformPlayEventCard(g, argument1, argument2); break;
                    case "h": PerformDraculaDrawCards(g, argument1); break;
                    case "l": PerformHunterMove(g, argument1, argument2); break;
                    case "exit": break;
                    default: Console.WriteLine("I don't know what you're talking about"); break;
                }
            } while (command != "exit");
        }

        private static void PerformHunterMove(GameState g, string argument1, string argument2)
        {
            string line = "";
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                do
                {
                    Console.WriteLine("Who is moving? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                    line = Console.ReadLine();
                } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            }
            Location locationToMoveTo = g.GetLocationFromName(argument2);
            while (locationToMoveTo.name == "Unknown location")
            {
                Console.WriteLine("Where is " + g.NameOfHunterAtIndex(hunterIndex) + " moving? (partial name will suffice)");
                line = Console.ReadLine();
                locationToMoveTo = g.GetLocationFromName(line);
                Console.WriteLine(locationToMoveTo.name);
            }
            g.MoveHunterToLocationAtHunterIndex(hunterIndex, locationToMoveTo);
        }

        private static void PerformDraculaDrawCards(GameState g, string argument)
        {
            int numberOfCards;
            if (int.TryParse(argument, out numberOfCards))
            {
                do
                {
                    g.DrawEventCardForDracula();
                    numberOfCards--;
                } while (numberOfCards > 0);
                g.DiscardDraculaCardsDownToHandSize();
            }
            else
            {
                Console.WriteLine("Dracula cannot draw " + argument + " cards");
            }
        }

        private static void PerformPlayEventCard(GameState g, string argument1, string argument2)
        {
            switch (PlayEventCard(g, argument1, argument2))
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
        }

        private static void PerformTrailClear(GameState g, string argument)
        {
            int trailLength;
            if (int.TryParse(argument, out trailLength))
            {
                g.TrimDraculaTrail(trailLength);
            }
            else
            {
                Console.WriteLine("Unable to clear Dracula's trail to length " + argument);
            }
        }

        private static void PerformRevealEncounter(GameState g, string argument)
        {
            int trailIndex;
            if (int.TryParse(argument, out trailIndex))
            {
                try
                {
                    g.RevealEncounterInTrail(trailIndex - 1);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Unable to reveal encounter " + argument);
                }
            }
            else
            {
                Console.WriteLine("Unable to reveal encounter " + argument);
            }
        }

        private static void PerformRevealLocation(GameState g, string argument)
        {
            int trailIndex;
            if (int.TryParse(argument, out trailIndex))
            {
                try
                {
                    g.RevealLocationAtTrailIndex(trailIndex - 1);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Unable to reveal card " + argument);
                }
            }
            else
            {
                Console.WriteLine("Unable to reveal card " + argument);
            }
        }

        private static void PerformDraculaTurn(GameState g)
        {
            Logger.WriteToDebugLog("STARTING DRACULA'S TURN =====================================================");
            g.PerformDraculaTurn();
        }

        public static string PlayEventCard(GameState g, string argument1, string argument2)
        {
            string line = "";
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                do
                {
                    Console.WriteLine("Who is playing the card? 1 = Lord Godalming; 2 = Van Helsing; 3 = Dr. Seward; 4 = Mina Harker");
                    line = Console.ReadLine();
                } while (!int.TryParse(line, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4);
            }
            int eventIndex = g.IndexOfEventCardInEventDeck(argument2);
            while (eventIndex == -1)
            {
                Console.WriteLine("What is the event card name? (partial name will suffice)");
                line = Console.ReadLine();
                eventIndex = g.IndexOfEventCardInEventDeck(line);
                if (eventIndex == -1)
                {
                    Console.WriteLine("I don't recognise a card starting with " + line + ". Is it in the discard pile?");
                }
                else if (g.EventCardIsDraculaCardAtIndex(eventIndex))
                {
                    Console.WriteLine(g.NameOfEventCardAtIndex(eventIndex) + " is Dracula's card");
                    eventIndex = -1;
                }
            }
            Console.WriteLine(g.NameOfHunterAtIndex(hunterIndex - 1) + " is playing event card " + g.NameOfEventCardAtIndex(eventIndex));
            Logger.WriteToDebugLog(g.NameOfHunterAtIndex(hunterIndex - 1) + " is playing event card " + g.NameOfEventCardAtIndex(eventIndex));
            Logger.WriteToGameLog(g.NameOfHunterAtIndex(hunterIndex - 1) + " is playing event card " + g.NameOfEventCardAtIndex(eventIndex));
            return g.NameOfEventCardAtIndex(eventIndex);
        }

        private static void PlayHiredScouts(GameState g)
        {
            string line = "";
            Location locationToReveal;
            do
            {
                Console.WriteLine("Name the first city");
                line = Console.ReadLine();
                locationToReveal = g.GetLocationFromName(line);
                Console.WriteLine(locationToReveal.name);
            } while (locationToReveal.name == "Unknown location");
            if (g.LocationIsInTrail(locationToReveal))
            {
                locationToReveal.isRevealed = true;
                Console.Write("Revealing " + locationToReveal.name);
                for (int i = 0; i < locationToReveal.encounters.Count(); i++)
                {
                    locationToReveal.encounters[i].isRevealed = true;
                    Console.Write(" and " + locationToReveal.encounters[i].name);
                }
                Console.WriteLine("");
                drawTrail(g);
            }
            else
            {
                Console.Write(locationToReveal.name + " is not in Dracula's trail");
            }
            do
            {
                Console.WriteLine("Name the second city");
                line = Console.ReadLine();
                locationToReveal = g.GetLocationFromName(line);
                Console.WriteLine(locationToReveal.name);
            } while (locationToReveal.name == "Unknown location");
            if (g.LocationIsInTrail(locationToReveal))
            {
                locationToReveal.isRevealed = true;
                Console.Write("Revealing " + locationToReveal.name);
                for (int i = 0; i < locationToReveal.encounters.Count(); i++)
                {
                    locationToReveal.encounters[i].isRevealed = true;
                    Console.Write(" and " + locationToReveal.encounters[i].name);
                }
                Console.WriteLine("");
            }
            else
            {
                Console.WriteLine(locationToReveal.name + " is not in Dracula's trail");
            }
            DiscardEventCard(g, "Hired Scouts");

        }

        private static void PlayEscapeRoute(GameState g)
        {
            Console.WriteLine("Forewarned is supposed to be played at the start of combat");
        }

        private static void PlayExcellentWeather(GameState g)
        {
            Console.WriteLine("You may move up to four sea moves this turn");
            DiscardEventCard(g, "Excellent Weather");
        }

        private static void PlayCharteredCarriage(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayForewarned(GameState g)
        {
            Console.WriteLine("Forewarned is supposed to be played when Dracula reveals an encounter at your location");
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
            int checkingLocationIndex = g.TrailLength(); 
            do
            {
                checkingLocationIndex--;
            } while ((g.TypeOfLocationAtTrailIndex(checkingLocationIndex) != LocationType.Castle && g.TypeOfLocationAtTrailIndex(checkingLocationIndex) != LocationType.City && g.TypeOfLocationAtTrailIndex(checkingLocationIndex) != LocationType.Sea && g.TypeOfLocationAtTrailIndex(checkingLocationIndex) != LocationType.Town) || g.LocationIsRevealedAtTrailIndex(checkingLocationIndex));

            if (g.DraculaCurrentLocationIsAtTrailIndex(checkingLocationIndex))
            {
                Console.WriteLine("The oldest unrevealed location in Dracula's trail is his current location");
                if (g.LocationWhereHideWasUsedIsDraculaCurrentLocation())
                {
                    Console.WriteLine("Here's the Hide card to prove it");
                    g.RevealHide();
                }
            }
            else
            {
                g.RevealLocationAtTrailIndex(checkingLocationIndex);
                Console.WriteLine("Revealing " + g.NameOfLocationAtTrailIndex(checkingLocationIndex));
            }
            DiscardEventCard(g, "Newspaper Reports");
        }

        private static void PlayAdvancePlanning(GameState g)
        {
            Console.WriteLine("Advance Planning is supposed to be played at the start of a combat");
        }

        private static void PlayMysticResearch(GameState g)
        {
            throw new NotImplementedException();
        }

        private static void PlayLongDay(GameState g)
        {
            if (g.Time() < 1)
            {
                Console.WriteLine("You cannot play Long Day during Dawn");
            }
            else
            {
                g.AdjustTime(-1);
                DiscardEventCard(g, "Long Day");
            }
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
            for (int i = 0; i < g.TrailLength(); i++)
            {
                if (g.TypeOfLocationAtTrailIndex(i) == LocationType.Sea)
                {
                    g.RevealLocationAtTrailIndex(i);
                    LocationHelper.RevealLocation(g, i);
                }
            }
            DiscardEventCard(g, "Money Trail");
        }

        private static void PlayGreatStrength(GameState g)
        {
            Console.WriteLine("Great Strength is supposed to be played when a Hunter receives damage or a bite");
        }

        private static void PlayHeroicLeap(GameState g)
        {
            Console.WriteLine("Heroic Leap is supposed to be played at the start of a combat");
        }

        private static void PlaySisterAgatha(GameState g)
        {
            if (g.HuntersHaveAlly())
            {
                g.AddEventToEventDiscard(g.GetEventFromEventDeck(g.NameOfHunterAlly()));
            }
            g.SetHunterAlly("Sister Agatha");
            g.RemoveEventFromEventDeck(g.GetEventFromEventDeck(g.NameOfHunterAlly()));
        }

        private static void PlayJonathanHarker(GameState g)
        {
            if (g.HuntersHaveAlly())
            {
                g.AddEventToEventDiscard(g.GetEventFromEventDeck(g.NameOfHunterAlly()));
            }
            g.SetHunterAlly("Jonathan Harker");
            g.RemoveEventFromEventDeck(g.GetEventFromEventDeck(g.NameOfHunterAlly()));
        }

        private static void PlayRufusSmith(GameState g)
        {
            if (g.HuntersHaveAlly())
            {
                g.AddEventToEventDiscard(g.GetEventFromEventDeck(g.NameOfHunterAlly()));
            }
            g.SetHunterAlly("Rufus Smith");
            g.RemoveEventFromEventDeck(g.GetEventFromEventDeck(g.NameOfHunterAlly()));
        }

        private static void DiscardEventCard(GameState g, string cardName)
        {
            Event playedCard = g.GetEventFromEventDeck(cardName);
            g.AddEventToEventDiscard(playedCard);
            g.RemoveEventFromEventDeck(playedCard);
        }

        public static void drawTrail(GameState g)
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

    }

}
