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
            UserInterface ui = new UserInterface();
            GameState g = new GameState();

            g.SetLocationForHunterAt(0, ui.GetHunterStartLocation(g, 0));
            g.SetLocationForHunterAt(1, ui.GetHunterStartLocation(g, 1));
            g.SetLocationForHunterAt(2, ui.GetHunterStartLocation(g, 2));
            g.SetLocationForHunterAt(3, ui.GetHunterStartLocation(g, 3));

            g.PlaceDraculaAtStartLocation();
            g.DrawEncountersUpToHandSize();

            Logger.ClearLogs(ui);

            Logger.WriteToDebugLog("Game start");

            Logger.WriteToDebugLog("Dracula started in " + g.DraculaCurrentLocationName());
            Logger.WriteToGameLog("Dracula started in " + g.DraculaCurrentLocationName());

            CommandSet commandSet = new CommandSet();

            do
            {
                ui.drawTrail(g);
                commandSet = ui.GetCommandSet();

                switch (commandSet.command)
                {
                    case "s": LocationHelper.ShowLocationDetails(g.GetLocationFromName(commandSet.argument1)); break;
                    case "m": g.PerformDraculaTurn(ui); break;
                    case "r": PerformRevealLocation(g, commandSet.argument1, ui); break;
                    case "e": PerformRevealEncounter(g, commandSet.argument1, ui); break;
                    case "c": PerformTrailClear(g, commandSet.argument1, ui); break;
                    case "v": PerformPlayEventCard(g, commandSet.argument1, commandSet.argument2, ui); break;
                    case "h": PerformDraculaDrawCards(g, commandSet.argument1, ui); break;
                    case "l": PerformHunterMove(g, commandSet.argument1, commandSet.argument2, ui); break;
                    case "exit": break;
                    default: Console.WriteLine("I don't know what you're talking about"); break;
                }
            } while (commandSet.command != "exit");
        }

        private static void PerformHunterMove(GameState g, string argument1, string argument2, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfMovingHunter();
            }
            Location locationToMoveTo = g.GetLocationFromName(argument2);
            while (locationToMoveTo.name == "Unknown location")
            {
                locationToMoveTo = g.GetLocationFromName(ui.GetNameOfLocationWhereHunterIsMoving(g.NameOfHunterAtIndex(hunterIndex)));
            }            
            g.MoveHunterToLocationAtHunterIndex(hunterIndex, locationToMoveTo);
        }

        private static void PerformDraculaDrawCards(GameState g, string argument, UserInterface ui)
        {
            int numberOfCards;
            if (int.TryParse(argument, out numberOfCards))
            {
                do
                {
                    g.DrawEventCardForDracula(ui);
                    numberOfCards--;
                } while (numberOfCards > 0);
                g.DiscardDraculaCardsDownToHandSize(ui);
            }
            else
            {
                ui.TellUser("Dracula cannot draw " + argument + " cards");
            }
        }

        private static void PerformPlayEventCard(GameState g, string argument1, string argument2, UserInterface ui)
        {
            switch (PlayEventCard(g, argument1, argument2, ui))
            {
                case "Rufus Smith": g.PlayRufusSmith(); break;
                case "Jonathan Harker": g.PlayJonathanHarker(); break;
                case "Sister Agatha": g.PlaySisterAgatha(); break;
                case "Heroic Leap": g.PlayHeroicLeap(ui); break;
                case "Great Strength": g.PlayGreatStrength(ui); break;
                case "Money Trail": g.PlayMoneyTrail(ui); break;
                case "Sense of Emergency": g.PlaySenseOfEmergency(); break;
                case "Vampiric Lair": g.PlayVampiricLair(); break;
                case "Long Day": g.PlayLongDay(ui); break;
                case "Mystic Research": g.PlayMysticResearch(); break;
                case "Advance Planning": g.PlayAdvancePlanning(ui); break;
                case "Newspaper Reports": g.PlayNewspaperReports(ui); break;
                case "Re-Equip": g.PlayReEquip(); break;
                case "Consecrated Ground": g.PlayConsecratedGround(); break;
                case "Telegraph Ahead": g.PlayTelegraphAhead(); break;
                case "Hypnosis": g.PlayHypnosis(); break;
                case "Stormy Seas": g.PlayStormySeas(); break;
                case "Surprising Return": g.PlaySurprisingReturn(); break;
                case "Good Luck": g.PlayGoodLuck(); break;
                case "Blood Transfusion": g.PlayBloodTransfusion(); break;
                case "Secret Weapon": g.PlaySecretWeapon(); break;
                case "Forewarned": g.PlayForewarned(ui); break;
                case "Chartered Carriage": g.PlayCharteredCarriage(); break;
                case "Excellent Weather": g.PlayExcellentWeather(ui); break;
                case "Escape Route": g.PlayEscapeRoute(ui); break;
                case "Hired Scouts": g.PlayHiredScouts(ui); break;
            }
        }

        private static void PerformTrailClear(GameState g, string argument, UserInterface ui)
        {
            int trailLength;
            if (int.TryParse(argument, out trailLength))
            {
                g.TrimDraculaTrail(trailLength);
            }
            else
            {
                ui.TellUser("Unable to clear Dracula's trail to length " + argument);
            }
        }

        private static void PerformRevealEncounter(GameState g, string argument, UserInterface ui)
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
                    ui.TellUser("Unable to reveal encounter " + argument);
                }
            }
            else
            {
                ui.TellUser("Unable to reveal encounter " + argument);
            }
        }

        private static void PerformRevealLocation(GameState g, string argument, UserInterface ui)
        {
            int trailIndex;
            if (int.TryParse(argument, out trailIndex))
            {
                try
                {
                    g.RevealLocationAtTrailIndex(trailIndex - 1, ui);
                }
                catch (ArgumentOutOfRangeException)
                {
                    ui.TellUser("Unable to reveal card " + argument);
                }
            }
            else
            {
                ui.TellUser("Unable to reveal card " + argument);
            }
        }

        private static void PerformDraculaTurn(GameState g, UserInterface ui)
        {
            Logger.WriteToDebugLog("STARTING DRACULA'S TURN =====================================================");
            g.PerformDraculaTurn(ui);
        }

        public static string PlayEventCard(GameState g, string argument1, string argument2, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfMovingHunter();
            }
            int eventIndex = g.IndexOfEventCardInEventDeck(argument2);
            while (eventIndex == -1)
            {
                string line = ui.GetEventCardName();
                eventIndex = g.IndexOfEventCardInEventDeck(line);
                if (eventIndex == -1)
                {
                    ui.TellUser("I don't recognise a card starting with " + line + ". Is it in the discard pile?");
                }
                else if (g.EventCardIsDraculaCardAtIndex(eventIndex))
                {
                    ui.TellUser(g.NameOfEventCardAtIndex(eventIndex) + " is Dracula's card");
                    eventIndex = -1;
                }
            }
            ui.TellUser(g.NameOfHunterAtIndex(hunterIndex - 1) + " is playing event card " + g.NameOfEventCardAtIndex(eventIndex));
            Logger.WriteToDebugLog(g.NameOfHunterAtIndex(hunterIndex - 1) + " is playing event card " + g.NameOfEventCardAtIndex(eventIndex));
            Logger.WriteToGameLog(g.NameOfHunterAtIndex(hunterIndex - 1) + " is playing event card " + g.NameOfEventCardAtIndex(eventIndex));
            return g.NameOfEventCardAtIndex(eventIndex);
        }
        
    }

}
