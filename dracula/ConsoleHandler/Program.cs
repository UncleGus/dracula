using DraculaHandler;
using EventHandler;
using LocationHandler;
using LogHandler;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace DraculaSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            UserInterface ui = new UserInterface();
            Map map = new Map();
            ItemSet itemDeck = new ItemSet();
            EventSet eventDeck = new EventSet();
            EncounterSet encounterDeck = new EncounterSet();
            GameState g = new GameState();
            Logger.ClearLogs(ui);

            //            PerformLoad(g, "lol", ui);

            g.SetHunterStartLocations(map, ui);
            g.Dracula.ChooseStartLocation(map, g.Hunters);
            g.Dracula.DrawEncounters(g, g.Dracula.EncounterHandSize, encounterDeck);
            PerformDraculaTurn(g, ui);

            PerformSave(g, "lol", ui);

            CommandSet commandSet = new CommandSet();

            do
            {
                ui.drawGameState(g);
                commandSet = ui.GetCommandSet();

                switch (commandSet.command.ToLower())
                {
#if DEBUG
                    case "rl": PerformRevealLocation(g, commandSet.argument1, ui); break;
                    case "re": PerformRevealEncounter(g, commandSet.argument1, ui); break;
                    case "clear": PerformTrailClear(g, commandSet.argument1, ui); break;
                    case "known": g.TellMeWhatYouKnow(ui); break;
#endif

                    case "m": PerformHunterMove(g, commandSet.argument1, commandSet.argument2, ui); break;
                    case "t": PerformCatchTrain(g, commandSet.argument1, ui); break;
                    case "e": PerformPlayEventCard(g, commandSet.argument1, commandSet.argument2, ui); break;
                    case "d": PerformDraculaDrawCards(g, ui); break;
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
                    case "save": PerformSave(g, commandSet.argument1, ui); break;
                    case "state": ShowKnownState(g, ui); break;
                    case "load": PerformLoad(g, commandSet.argument1, ui); break;
                    case "exit": break;
                    default: Console.WriteLine("I don't know what you're talking about, 'help' for help"); break;
                }
            } while (commandSet.command != "exit");
        }

        private static void PerformSave(GameState g, string fileName, UserInterface ui)
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

                fileWriter.WriteObject(writeStream, g);
                writeStream.Close();
                ui.TellUser(fileName + " saved");
            }
            catch (Exception e)
            {
                ui.TellUser("File could not be saved because " + e.Message);
            }

        }

        private static void PerformLoad(GameState g, string fileName, UserInterface ui)
        {
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

                g = (GameState)fileReader.ReadObject(readStream);
                readStream.Close();
                ui.TellUser(fileName + " loaded");
            }
            catch (Exception e)
            {
                ui.TellUser("File could not be loaded because " + e.Message);
            }

        }

        private static void ShowKnownState(GameState g, UserInterface ui)
        {
            g.ShowStateOfGame(ui);
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
                resolveType = ui.GetTypeOfResolveUsed();
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
            string itemName = g.GetItemByNameFromItemDeck(argument2).Name;
            while (itemName == "Unknown item")
            {
                string line = ui.GetNameOfItemUsedByHunter(g.NameOfHunterAtIndex(hunterIndex));
                itemName = g.GetItemByNameFromItemDeck(line).Name;
                if (itemName == "Unknown item")
                {
                    if (line.ToLower() == "cancel")
                    {
                        return;
                    }
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

                hunterToAdd = ui.GetHunterToAddToGroup(g.Hunters[hunterIndex].Name);
                if (hunterToAdd != -2 && hunterToAdd < hunterIndex)
                {
                    ui.TellUser(g.Hunters[hunterToAdd].Name + " cannot join " + g.Hunters[hunterIndex].Name + "'s group, instead " + g.Hunters[hunterIndex].Name + " should be added to " + g.Hunters[hunterToAdd].Name + "'s group");
                    hunterToAdd = -2;
                }
                if (hunterToAdd != -2 && hunterToAdd != hunterIndex)
                {
                    if (g.Hunters[hunterIndex].HuntersInGroup.Contains(hunterToAdd))
                    {
                        g.Hunters[hunterIndex].HuntersInGroup.Remove(hunterToAdd);
                    }
                    else
                    {
                        g.Hunters[hunterIndex].HuntersInGroup.Add(hunterToAdd);
                    }
                }
            } while (hunterToAdd != -2);

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
                string line = ui.GetNameOfEventDiscardedByHunter(g.NameOfHunterAtIndex(hunterIndex));
                eventName = g.GetEventByNameFromEventDeck(line).name;
                if (eventName == "Unknown event")
                {
                    if (line.ToLower() == "cancel")
                    {
                        return;
                    }
                    ui.TellUser("I can't find that event");
                }
            }
            g.DiscardEventFromHunterAtIndex(eventName, hunterIndex, ui);
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
            string itemName = g.GetItemByNameFromItemDeck(argument2).Name;
            while (itemName == "Unknown item")
            {
                itemName = g.GetItemByNameFromItemDeck(ui.GetNameOfItemDiscardedByHunter(g.NameOfHunterAtIndex(hunterIndex))).Name;
                if (itemName == "Unknown item")
                {
                    ui.TellUser("I can't find that item");
                }
            }
            g.DiscardItemFromHunterAtIndex(itemName, hunterIndex, ui);
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
            g.AddItemCardToHunterAtIndex(hunterIndex, ui);
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
            g.AddEventCardToHunterAtIndex(hunterIndex, ui);
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
            if (!g.DraculaCancelTrain(hunterIndex, ui))
            {
                ui.TellUser(g.NameOfHunterAtIndex(hunterIndex) + " can catch a train");
            }
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
            LocationDetail locationToMoveTo = g.GetLocationFromName(argument2);
            while (locationToMoveTo.Name == "Unknown location" || locationToMoveTo.Name == "Multiple locations")
            {
                locationToMoveTo = g.GetLocationFromName(ui.GetNameOfLocationWhereHunterIsMoving(g.NameOfHunterAtIndex(hunterIndex)));
                ui.TellUser(locationToMoveTo.Name);
            }
            g.MoveHunterToLocationAtHunterIndex(hunterIndex, locationToMoveTo, ui);
            if (!g.DraculaWillPlayCustomsSearch(hunterIndex, ui))
            {
                g.SearchWithHunterAtIndex(hunterIndex, locationToMoveTo, ui);
            }
        }

        private static void PerformDraculaDrawCards(GameState g, UserInterface ui)
        {
            g.DrawEventCardForDracula(ui);
            g.DiscardDraculaCardsDownToHandSize(ui);
        }

        private static void PerformPlayEventCard(GameState g, string argument1, string argument2, UserInterface ui)
        {
            int hunterIndex = 0;
            string cardName = PlayEventCard(g, argument1, argument2, out hunterIndex, ui);
            switch (cardName)
            {
                case "Rufus Smith":
                    g.PlayRufusSmith();
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Jonathan Harker":
                    g.PlayJonathanHarker();
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Sister Agatha":
                    g.PlaySisterAgatha();
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Heroic Leap":
                    g.PlayHeroicLeap(ui);
                    break;
                case "Great Strength":
                    g.PlayGreatStrength(ui);
                    break;
                case "Money Trail":
                    g.PlayMoneyTrail(ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Sense of Emergency":
                    g.PlaySenseOfEmergency(hunterIndex, ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Vampire Lair":
                    g.PlayVampireLair(hunterIndex, ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Long Day":
                    if (g.Time() < 1)
                    {
                        ui.TellUser("You cannot play Long Day during Dawn");
                    }
                    else
                    {
                        g.PlayLongDay(ui);
                        g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    }
                    break;
                case "Mystic Research":
                    g.PlayMysticResearch(ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Advance Planning":
                    g.PlayAdvancePlanning(ui);
                    break;
                case "Newspaper Reports":
                    g.PlayNewspaperReports(ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Re-Equip":
                    g.PlayReEquip(ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Consecrated Ground":
                    g.PlayConsecratedGround(hunterIndex, ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Telegraph Ahead":
                    g.PlayTelegraphAhead(hunterIndex, ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Hypnosis":
                    g.PlayHypnosis(ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Stormy Seas":
                    g.PlayStormySeas(ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Surprising Return":
                    g.PlaySurprisingReturn(hunterIndex, ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Good Luck":
                    g.PlayGoodLuck(hunterIndex, ui); // discard handled within function
                    break;
                case "Blood Transfusion":
                    g.PlayBloodTransfusion(ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Secret Weapon":
                    g.PlaySecretWeapon(ui);
                    break;
                case "Forewarned":
                    g.PlayForewarned(ui);
                    break;
                case "Chartered Carriage":
                    g.PlayCharteredCarriage(ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Excellent Weather":
                    g.PlayExcellentWeather(ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "Escape Route":
                    g.PlayEscapeRoute(ui);
                    break;
                case "Hired Scouts":
                    g.PlayHiredScouts(ui);
                    g.DiscardEventFromHunterAtIndex(cardName, hunterIndex, ui);
                    break;
                case "cancel":
                    ui.TellUser("Action canclled");
                    break;
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
            }
            else
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
                    if (line.ToLower() == "cancel")
                    {
                        return "cancel";
                    }
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
            return g.NameOfEventCardAtIndex(eventIndex);
        }

    }

}
