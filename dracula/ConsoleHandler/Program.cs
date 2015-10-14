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
            Logger.ClearLogs(ui);

            g.SetLocationForHunterAt(0, ui.GetHunterStartLocation(g, 0));
            g.SetLocationForHunterAt(1, ui.GetHunterStartLocation(g, 1));
            g.SetLocationForHunterAt(2, ui.GetHunterStartLocation(g, 2));
            g.SetLocationForHunterAt(3, ui.GetHunterStartLocation(g, 3));

            g.PlaceDraculaAtStartLocation();
            g.DrawEncountersUpToHandSize();
            PerformDraculaTurn(g, ui);

            CommandSet commandSet = new CommandSet();

            do
            {
                ui.drawGameState(g);
                commandSet = ui.GetCommandSet();

                switch (commandSet.command.ToLower())
                {
                    //case "s": LocationHelper.ShowLocationDetails(g.GetLocationFromName(commandSet.argument1)); break;
                    //case "r": PerformRevealLocation(g, commandSet.argument1, ui); break;
                    //case "e": PerformRevealEncounter(g, commandSet.argument1, ui); break;
                    //case "f": PerformCombat(g, commandSet.argument1, commandSet.argument2, ui); break;
                    //case "c": PerformTrailClear(g, commandSet.argument1, ui); break;
                    case "m": PerformHunterMove(g, commandSet.argument1, commandSet.argument2, ui); break;
                    case "t": PerformCatchTrain(g, commandSet.argument1, ui); break;
                    case "e": PerformPlayEventCard(g, commandSet.argument1, commandSet.argument2, ui); break;
                    case "d": PerformDraculaDrawCards(g, commandSet.argument1, ui); break;
                    case "v": PerformHunterDrawEvent(g, commandSet.argument1, ui); break;
                    case "i": PerformHunterDrawItem(g, commandSet.argument1, ui); break;
                    case "a": PerformHunterDiscardEvent(g, commandSet.argument1, commandSet.argument2, ui); break;
                    case "c": PerformHunterDiscardItem(g, commandSet.argument1, commandSet.argument2, ui); break;
                    case "z": PerformDraculaTurn(g, ui); break;
                    case "g": SetUpGroups(g, commandSet.argument1, ui); break;
                    case "b": PerformBatsMove(g, commandSet.argument1, ui); break;
                    case "r": PerformTrade(g, commandSet.argument1, commandSet.argument2, ui); break;
                    case "u": PerformUseItem(g, commandSet.argument1, commandSet.argument2, ui); break;
                    case "o": PerformRest(g, commandSet.argument1, ui); break;
                    case "h": PerformHospital(g, commandSet.argument1, ui); break;
                    case "s": PerformResolve(g, commandSet.argument1, commandSet.argument2, ui); break;
                    case "help": ui.ShowHelp(); break;
                    case "exit": break;
                    default: Console.WriteLine("I don't know what you're talking about, 'help' for help"); break;
                }
            } while (commandSet.command != "exit");
        }

        private static void PerformResolve(GameState g, string argument1, string argument2, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfHunterUsingResolve();
            }
            else
            {
                hunterIndex--;
            }
            int resolveType;
            if (!int.TryParse(argument2, out resolveType) || resolveType < 1 || resolveType > 3)
            {
                resolveType= ui.GetTypeOfResolveUsed();
            }
            switch (resolveType)
            {
                case 1:
                    g.PerformNewspaperReportsFromResolve(ui);
                    break;
                case 2:
                    g.PerformSenseOfEmergencyFromResolve(hunterIndex, ui);
                    break;
                case 3:
                    g.PerformInnerStrengthFromResolve(hunterIndex, ui);
                    break;
            }
        }

        private static void PerformHospital(GameState g, string argument1, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfHunterUsingHospital();
            }
            else
            {
                hunterIndex--;
            }
            g.BlessHunterAtHunterIndex(hunterIndex, ui);
        }

        private static void PerformRest(GameState g, string argument1, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfHunterResting();
            }
            else
            {
                hunterIndex--;
            }
            g.RestHunterAtHunterIndex(hunterIndex, ui);           
        }

        private static void PerformUseItem(GameState g, string argument1, string argument2, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfHunterUsingItem();
            }
            else
            {
                hunterIndex--;
            }
            string itemName = g.GetItemByNameFromItemDeck(argument2).name;
            while (itemName == "Unknown item")
            {
                itemName = g.GetItemByNameFromItemDeck(ui.GetNameOfItemUsedByHunter(g.NameOfHunterAtIndex(hunterIndex))).name;
                if (itemName == "Unknown item")
                {
                    ui.TellUser("I can't find that item");
                }
            }
            g.UseItemByHunterAtHunterIndex(itemName, hunterIndex, ui);
        }

        private static void PerformTrade(GameState g, string argument1, string argument2, UserInterface ui)
        {
            int hunterIndexA;
            if (!int.TryParse(argument1, out hunterIndexA) || hunterIndexA < 1 || hunterIndexA > 4)
            {
                hunterIndexA = ui.GetIndexOfHunterEnteringTrade();
            }
            else
            {
                hunterIndexA--;
            }
            int hunterIndexB;
            if (!int.TryParse(argument2, out hunterIndexB) || hunterIndexB < 1 || hunterIndexB > 4 || hunterIndexA == hunterIndexB)
            {
                hunterIndexB = ui.GetIndexOfHunterEnteringTrade();
            }
            else
            {
                hunterIndexB--;
            }
            g.TradeBetweenHunters(hunterIndexA, hunterIndexB, ui);
        }

        private static void PerformBatsMove(GameState g, string argument1, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfHunterBeingMovedByBats();
            }
            else
            {
                hunterIndex--;
            }
            g.PerformBatsMoveForHunter(hunterIndex, ui);
        }

        private static void SetUpGroups(GameState g, string argument1, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 3)
            {
                hunterIndex = ui.GetIndexOfHunterFormingGroup();
            }
            else
            {
                hunterIndex--;
            }
            int hunterToAdd;
            do
            {
                ui.ShowGroupMembersAtHunterIndex(g, hunterIndex);
                hunterToAdd = g.GetHunterToAddToGroup(g, hunterIndex, ui);
                if (hunterToAdd != -2 && hunterToAdd != hunterIndex)
                {
                    if (g.HunterAIsInHunterBGroup(hunterToAdd, hunterIndex))
                    {
                        g.RemoveHunterAFromHunterBGroup(hunterToAdd, hunterIndex);
                    }
                    else
                    {
                        g.AddHunterAToHunterBGroup(hunterToAdd, hunterIndex);
                    }
                }
            } while (hunterToAdd != -2);
            
        }

        private static void PerformCombat(GameState g, string argument1, string argument2, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfHunterEnteringCombat();
            }
            else
            {
                hunterIndex--;
            }
            int enemyInCombat;
            if (!int.TryParse(argument2, out enemyInCombat) || enemyInCombat < 1 || enemyInCombat > 6)
            {
                enemyInCombat = ui.GetTypeOfEnemyEnteringCombat();
            }
            switch (g.ResolveCombat(hunterIndex, enemyInCombat, ui))
            {
                case "Bite":
                    if (g.NumberOfHuntersAtLocation(g.LocationOfHunterAtHunterIndex(hunterIndex)) > 1)
                    {
                        g.ApplyBiteToOneOfMultipleHunters(hunterIndex, ui);
                    }
                    else
                    {
                        g.ApplyBiteToHunter(hunterIndex, ui);
                    }
                    if (g.Time() > 2 && enemyInCombat == 1)
                    {
                        g.HandleDraculaEscape(ui);
                    }
                    break;
                case "Enemy killed": break;
                case "Hunter killed": g.HandlePossibleHunterDeath(ui);
                    break;
                case "End":
                    if (g.Time() > 2 && enemyInCombat == 1)
                    {
                        g.HandleDraculaEscape(ui);
                    }
                    break;
            }
        }

        private static void PerformHunterDiscardEvent(GameState g, string argument1, string argument2, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfHunterDiscardingEvent();
            }
            else
            {
                hunterIndex--;
            }
            string eventName = g.GetEventByNameFromEventDeck(argument2).name;
            while (eventName == "Unknown event")
            {
                eventName = g.GetEventByNameFromEventDeck(ui.GetNameOfEventDiscardedByHunter(g.NameOfHunterAtIndex(hunterIndex))).name;
                if (eventName == "Unknown event")
                {
                    ui.TellUser("I can't find that event");
                }
            }
            g.DiscardEventFromHunterAtIndex(eventName, hunterIndex);
            ui.TellUser(g.NameOfHunterAtIndex(hunterIndex) + " discarded " + eventName + " and has " + g.NumberOfEventCardsAtHunterIndex(hunterIndex) + " events left");
        }

        private static void PerformHunterDiscardItem(GameState g, string argument1, string argument2, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfHunterDiscardingItem();
            }
            else
            {
                hunterIndex--;
            }
            string itemName = g.GetItemByNameFromItemDeck(argument2).name;
            while (itemName == "Unknown item")
            {
                itemName = g.GetItemByNameFromItemDeck(ui.GetNameOfItemDiscardedByHunter(g.NameOfHunterAtIndex(hunterIndex))).name;
                if (itemName == "Unknown item")
                {
                    ui.TellUser("I can't find that item");
                }
            }
            g.DiscardItemFromHunterAtIndex(itemName, hunterIndex);
            ui.TellUser(g.NameOfHunterAtIndex(hunterIndex) + " discarded " + itemName + " and has " + g.NumberOfItemCardsAtHunterIndex(hunterIndex) + " items left");
        }

        private static void PerformHunterDrawItem(GameState g, string argument1, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfHunterDrawingItem();
            }
            else
            {
                hunterIndex--;
            }
            g.AddItemCardToHunterAtIndex(hunterIndex);
            ui.TellUser(g.NameOfHunterAtIndex(hunterIndex) + " drew an item and now has " + g.NumberOfItemCardsAtHunterIndex(hunterIndex));
            switch (g.HunterShouldDiscardAtHunterIndex(hunterIndex))
            {
                case "item": ui.TellUser(g.NameOfHunterAtIndex(hunterIndex) + " needs to discard an item"); break;
                case "item or event": ui.TellUser(g.NameOfHunterAtIndex(hunterIndex) + " needs to discard an item or an event"); break;
            }
        }

        private static void PerformHunterDrawEvent(GameState g, string argument1, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfHunterDrawingEvent();
            }
            else
            {
                hunterIndex--;
            }
            g.AddEventCardToHunterAtIndex(hunterIndex);
            ui.TellUser(g.NameOfHunterAtIndex(hunterIndex) + " drew an event and now has " + g.NumberOfEventCardsAtHunterIndex(hunterIndex));
            switch (g.HunterShouldDiscardAtHunterIndex(hunterIndex))
            {
                case "event": ui.TellUser(g.NameOfHunterAtIndex(hunterIndex) + " needs to discard an event"); break;
                case "item or event": ui.TellUser(g.NameOfHunterAtIndex(hunterIndex) + " needs to discard an item or an event"); break;
            }
        }

        private static void PerformCatchTrain(GameState g, string argument1, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfMovingHunter();
            }
            else
            {
                hunterIndex--;
            }
            Logger.WriteToDebugLog(g.NameOfHunterAtIndex(hunterIndex) + " is attempting to catch a train");
            g.DraculaCancelTrain(hunterIndex, ui);
        }

        private static void PerformHunterMove(GameState g, string argument1, string argument2, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfMovingHunter();
            }
            else
            {
                hunterIndex--;
            }
            if (g.DraculaWillPlayControlStorms(hunterIndex, ui))
            {
                return;
            }
            Location locationToMoveTo = g.GetLocationFromName(argument2);
            while (locationToMoveTo.name == "Unknown location")
            {
                locationToMoveTo = g.GetLocationFromName(ui.GetNameOfLocationWhereHunterIsMoving(g.NameOfHunterAtIndex(hunterIndex)));
            }            
            g.MoveHunterToLocationAtHunterIndex(hunterIndex, locationToMoveTo);
            if (!g.DraculaWillPlayCustomsSearch(hunterIndex, ui))
            {
                g.SearchWithHunterAtIndex(hunterIndex, locationToMoveTo, ui);
            }
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
            int hunterIndex = 0;
            switch (PlayEventCard(g, argument1, argument2, out hunterIndex, ui))
            {
                case "Rufus Smith": g.PlayRufusSmith(); break;
                case "Jonathan Harker": g.PlayJonathanHarker(); break;
                case "Sister Agatha": g.PlaySisterAgatha(); break;
                case "Heroic Leap": g.PlayHeroicLeap(ui); break;
                case "Great Strength": g.PlayGreatStrength(ui); break;
                case "Money Trail": g.PlayMoneyTrail(ui); break;
                case "Sense of Emergency": g.PlaySenseOfEmergency(hunterIndex, ui); break;
                case "Vampire Lair": g.PlayVampireLair(hunterIndex, ui); break;
                case "Long Day": g.PlayLongDay(ui); break;
                case "Mystic Research": g.PlayMysticResearch(ui); break;
                case "Advance Planning": g.PlayAdvancePlanning(ui); break;
                case "Newspaper Reports": g.PlayNewspaperReports(ui); break;
                case "Re-Equip": g.PlayReEquip(ui); break;
                case "Consecrated Ground": g.PlayConsecratedGround(hunterIndex, ui); break;
                case "Telegraph Ahead": g.PlayTelegraphAhead(hunterIndex, ui); break;
                case "Hypnosis": g.PlayHypnosis(ui); break;
                case "Stormy Seas": g.PlayStormySeas(ui); break;
                case "Surprising Return": g.PlaySurprisingReturn(hunterIndex, ui); break;
                case "Good Luck": g.PlayGoodLuck(); break;
                case "Blood Transfusion": g.PlayBloodTransfusion(ui); break;
                case "Secret Weapon": g.PlaySecretWeapon(); break;
                case "Forewarned": g.PlayForewarned(ui); break;
                case "Chartered Carriage": g.PlayCharteredCarriage(ui); break;
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

        public static string PlayEventCard(GameState g, string argument1, string argument2, out int index, UserInterface ui)
        {
            int hunterIndex;
            if (!int.TryParse(argument1, out hunterIndex) || hunterIndex < 1 || hunterIndex > 4)
            {
                hunterIndex = ui.GetIndexOfHunterPlayingEventCard();
            } else
            {
                hunterIndex--;
            }
            index = hunterIndex;
            int eventIndex = g.IndexOfEventCardInEventDeck(argument2);
            while (eventIndex == -1)
            {
                string line = ui.GetEventCardName();
                eventIndex = g.IndexOfEventCardInEventDeck(line);
                if (eventIndex == -1)
                {
                    ui.TellUser("I don't recognise a card starting with " + line);
                }
                else if (g.EventCardIsDraculaCardAtIndex(eventIndex))
                {
                    ui.TellUser(g.NameOfEventCardAtIndex(eventIndex) + " is Dracula's card");
                    eventIndex = -1;
                }
            }
            ui.TellUser(g.NameOfHunterAtIndex(hunterIndex) + " is playing event card " + g.NameOfEventCardAtIndex(eventIndex));
            Logger.WriteToDebugLog(g.NameOfHunterAtIndex(hunterIndex) + " is playing event card " + g.NameOfEventCardAtIndex(eventIndex));
            Logger.WriteToGameLog(g.NameOfHunterAtIndex(hunterIndex) + " is playing event card " + g.NameOfEventCardAtIndex(eventIndex));
            g.DiscardEventFromHunterAtIndex(g.NameOfEventCardAtIndex(eventIndex), hunterIndex);
            return g.NameOfEventCardAtIndex(eventIndex);
        }
        
    }

}
