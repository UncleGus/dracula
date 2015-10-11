using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationHandler;
using LogHandler;
using EncounterHandler;
using ConsoleHandler;
using EventHandler;

namespace DraculaHandler
{
    public class Dracula
    {
        public Location currentLocation;
        public List<Location> trail = new List<Location>();
        public List<DraculaPower> powers = new List<DraculaPower>();
        List<Location> possibleMoves = new List<Location>();
        List<Location> possibleDoubleBackMoves = new List<Location>();
        List<DraculaPower> possiblePowers = new List<DraculaPower>();
        public int blood;
        bool lostBloodAtSeaOnLatestTurn;
        public int encounterHandSize;
        List<Encounter> encountersToMature = new List<Encounter>();
        public List<Encounter> encounterHand = new List<Encounter>();
        public Location locationWhereHideWasUsed;
        public Location[] catacombs = new Location[3];
        public GameState g;
        public List<Event> eventCardsInHand = new List<Event>();
        public int eventHandSize;

        public Dracula(GameState gameState)
        {
            g = gameState;
            
            powers.Add(new DraculaPower("Dark Call", true));
            powers.Add(new DraculaPower("Double Back", true));
            powers.Add(new DraculaPower("Feed", false));
            powers.Add(new DraculaPower("Hide", true));
            powers.Add(new DraculaPower("Wolf Form", false));
            blood = 15;
            lostBloodAtSeaOnLatestTurn = false;
            encounterHandSize = 5;
            DrawEncounters(encounterHandSize);
            eventHandSize = 4;
        }

        public bool MoveDracula(UserInterface ui)
        {
            Logger.WriteToDebugLog("STARTING MOVEMENT PHASE");

            // build list of possible locations to move to
            DeterminePossibleLocations();

            // build list of possible powers to play
            DeterminePossiblePowers(g.Time());

            // check if there are no legal moves
            Logger.WriteToDebugLog("Checking if there are legal moves");
            if (possibleMoves.Count() + possiblePowers.Count() == 0)
            {
                Logger.WriteToDebugLog("Dracula has no legal moves");
                ui.TellUser("Dracula is cornered by his own trail");
                return false;
            }
            else if (possibleMoves.Count() == 0 && possiblePowers.Count() == 1 && possiblePowers.Contains(powers[1]))
            {
                Logger.WriteToDebugLog("Dracula has no regular moves available");
                DeterminePossibleWolfFormLocations();
                if (possibleMoves.Count() == 0)
                {
                    Logger.WriteToDebugLog("Dracula has no moves available by Wolf Form");
                    ui.TellUser("Dracula is cornered by his own trail");
                    return false;
                }
                DeterminePossibleLocations();
            }

            // choose an action from all possible actions
            int chosenActionIndex = new Random().Next(0, possibleMoves.Count() + possiblePowers.Count());

            // choose a power
            if (chosenActionIndex > possibleMoves.Count() - 1)
            {
                chosenActionIndex -= possibleMoves.Count();
                Logger.WriteToDebugLog("Dracula has chosen power " + possiblePowers[chosenActionIndex].name);
                if (possiblePowers[chosenActionIndex].name != "Double Back" && possiblePowers[chosenActionIndex].name != "Wolf Form")
                {
                    Logger.WriteToGameLog("Dracula used power " + possiblePowers[chosenActionIndex].name);
                }

                if (possiblePowers[chosenActionIndex].name == "Dark Call" || possiblePowers[chosenActionIndex].name == "Feed" || possiblePowers[chosenActionIndex].name == "Hide")
                {
                    AddDummyPowerCardToTrail(chosenActionIndex);
                    if (possiblePowers[chosenActionIndex].name == "Hide")
                    {
                        locationWhereHideWasUsed = currentLocation;
                    }
                }
                else if (possiblePowers[chosenActionIndex].name == "Double Back")
                {
                    DoDoubleBackMove(ui);
                }
                else if (possiblePowers[chosenActionIndex].name == "Wolf Form")
                {
                    DoWolfFormMove(ui);
                }

                // put the power used at the head of the trail
                Logger.WriteToDebugLog("Putting power " + possiblePowers[chosenActionIndex].name + " at the head of the trail");
                possiblePowers[chosenActionIndex].positionInTrail = 0;
            }
            else
            // performing a regular move
            {
                // choose a location
                MoveByRoadOrSea(ui);
                Logger.WriteToGameLog("Dracula moved to " + currentLocation.name);
            }
            Logger.WriteToDebugLog("Dracula has finished moving");
            return true;
        }

        public void DrawEventCard(UserInterface ui)
        {
            Event eventCardDrawn;
            do
            {
                eventCardDrawn = g.DrawEventCard();
            } while (!eventCardDrawn.isDraculaCard);
            Logger.WriteToDebugLog("Dracula drew card " + eventCardDrawn.name);
            Logger.WriteToGameLog("Dracula drew card " + eventCardDrawn.name);
            if (eventCardDrawn.type == EventType.Ally)
            {
                PlayAlly(eventCardDrawn, ui);
            }
            else if (eventCardDrawn.type == EventType.PlayImmediately)
            {
                PlayImmediately(eventCardDrawn);
            }
            else
            {
                eventCardsInHand.Add(eventCardDrawn);
                g.RemoveEventFromEventDeck(eventCardDrawn);
            }
        }

        public void PlayImmediately(Event eventCardDrawn)
        {
            throw new NotImplementedException();
        }

        public void PlayAlly(Event allyDrawn, UserInterface ui)
        {
            string allyKept;
            if (!g.DraculaHasAlly())
            {
                Logger.WriteToDebugLog("Dracula has no current Ally, keeping this one");
                g.SetDraculaAlly(allyDrawn);
                g.RemoveEventFromEventDeck(allyDrawn);
                Logger.WriteToDebugLog("Dracula put " + allyDrawn.name + " into his empty Ally slot");
                Logger.WriteToGameLog("Dracula put " + allyDrawn.name + " into his empty Ally slot");
                allyKept = allyDrawn.name;
            } else
            {
                Logger.WriteToDebugLog("Dracula already has an Ally, deciding which one to keep");
                string allyDiscarded;
                if (new Random().Next(0, 2) > 0)
                {
                    Logger.WriteToDebugLog("Keeping the new Ally");
                    allyDiscarded = g.NameOfDraculaAlly();
                    g.AddEventToEventDiscard(g.GetEventFromEventDeck(allyDiscarded));
                    g.SetDraculaAlly(allyDrawn);
                    allyKept = allyDrawn.name;
                    Logger.WriteToDebugLog("Dracula put " + allyDrawn.name + " into his Ally slot, replacing " + allyDiscarded);
                    Logger.WriteToGameLog("Dracula put " + allyDrawn.name + " into his Ally slot, replacing " + allyDiscarded);
                }
                else
                {
                    Logger.WriteToDebugLog("Keeping the existing Ally");
                    allyDiscarded = allyDrawn.name;
                    g.AddEventToEventDiscard(g.GetEventFromEventDeck(allyDiscarded));
                    allyKept = g.NameOfDraculaAlly();
                    Logger.WriteToDebugLog("Dracula kept " + g.NameOfDraculaAlly() + " in his Ally slot, discarding " + allyDiscarded);
                    Logger.WriteToGameLog("Dracula kept " + g.NameOfDraculaAlly() + " in his Ally slot, discarding " + allyDiscarded);
                }
                switch (allyDiscarded)
                {
                    case "Immanuel Hildesheim":
                        {
                            Logger.WriteToDebugLog("Discarded Immanuel Hildesheim, discarding events down to 4");
                            eventHandSize = 4;
                            DiscardEventsDownTo(eventHandSize, ui);
                            break;
                        }
                    case "Dracula's Brides":
                        {
                            Logger.WriteToDebugLog("Discarding Dracula's Brides, discarding encounters down to 5");
                            encounterHandSize = 5;
                            DiscardEncountersDownTo(encounterHandSize);
                            break;
                        }
                }
            }
            switch (allyKept)
            {
                case "Dracula's Brides":
                    {
                        Logger.WriteToDebugLog("Dracula's Brides is in play, encounter hand size is 7");
                        encounterHandSize = 7;
                        break;
                    }
                case "Immanuel Hildesheim":
                    {
                        Logger.WriteToDebugLog("Immanueal Hildesheim is in play, event hand size is 6");
                        eventHandSize = 6;
                        break;
                    }
            }

        }

        public void DiscardEncountersDownTo(int encountersToKeep)
        {
            while (encounterHand.Count() > encountersToKeep)
            {
                Encounter encounterToDiscard = encounterHand[new Random().Next(0, encounterHand.Count())];
                encounterHand.Remove(encounterToDiscard);
                g.AddEncounterToEncounterPool(encounterToDiscard);
                Logger.WriteToDebugLog("Dracula discarded " + encounterToDiscard.name);
                Logger.WriteToGameLog("Dracula discarded " + encounterToDiscard.name);
            }

        }

        public void DiscardEventsDownTo(int numberOfCards, UserInterface ui)
        {
            while (eventCardsInHand.Count() > numberOfCards)
            {
                Event cardToDiscard = eventCardsInHand[new Random().Next(0, eventCardsInHand.Count())];
                eventCardsInHand.Remove(cardToDiscard);
                g.AddEventToEventDiscard(cardToDiscard);
                Logger.WriteToDebugLog("Dracula discarded " + cardToDiscard.name);
                Logger.WriteToGameLog("Dracula discarded " + cardToDiscard.name);
                ui.TellUser("Dracula discarded " + cardToDiscard.name);
            }
        }

        public void ShowLocation(UserInterface ui)
        {
            ui.TellUser("Dracula is currently in: " + currentLocation.name);
        }

        public void ShowTrail(UserInterface ui)
        {
            for (int i = 0; i < trail.Count(); i++)
            {
                string powerName = "";
                for (int j = 0; j < powers.Count(); j++)
                {
                    if (powers[j].positionInTrail == i)
                    {
                        if (trail[i].name != powers[j].name)
                        {
                            powerName = powerName + " with " + powers[j].name;
                        }
                    }
                }
                ui.TellUser(trail[i].name + powerName);
            }
        }

        public void DeterminePossibleLocations()
        {
            Logger.WriteToDebugLog("Dracula is determining possible locations to move to");
            Logger.WriteToDebugLog("Clearing the old list of possible locations");
            possibleMoves.Clear();
            Logger.WriteToDebugLog("Clearing the old list of possible locations that can be moved to with Double Back");
            possibleDoubleBackMoves.Clear();
            for (int i = 0; i < currentLocation.byRoad.Count(); i++)
            {
                if (currentLocation.byRoad[i].type != LocationType.Hospital)
                {
                    Logger.WriteToDebugLog("Adding location " + currentLocation.byRoad[i].name);
                    possibleMoves.Add(currentLocation.byRoad[i]);
                }
                else
                {
                    Logger.WriteToDebugLog("Not adding location " + currentLocation.byRoad[i].name);
                }

            }

            for (int i = 0; i < currentLocation.bySea.Count(); i++)
            {
                Logger.WriteToDebugLog("Adding location " + currentLocation.bySea[i].name);
                possibleMoves.Add(currentLocation.bySea[i]);
            }

            Logger.WriteToDebugLog("Checking trail locations against possible moves");
            for (int i = 0; i < trail.Count(); i++)
            {
                if (trail[i].name != "Dark Call" && trail[i].name != "Hide" && trail[i].name != "Feed")
                {
                    if (possibleMoves.Contains(trail[i]))
                    {
                        Logger.WriteToDebugLog("Moving location " + trail[i].name + " from possible moves to Double Back locations because it is in Dracula's trail");
                        possibleDoubleBackMoves.Add(trail[i]);
                        possibleMoves.Remove(trail[i]);
                    }
                }
            }
            Logger.WriteToDebugLog("Checking Catacombs locations against possible moves");
            for (int i = 0; i < 3; i++)
            {
                if (catacombs[i] != null && possibleMoves.Contains(catacombs[i]))
                {
                    Logger.WriteToDebugLog("Moving " + catacombs[i].name + " from possible moves to possible Double Back moves because it is in the Catacombs");
                    possibleDoubleBackMoves.Add(catacombs[i]);
                    possibleMoves.Remove(catacombs[i]);
                }
            }
        }

        public void DeterminePossibleWolfFormLocations()
        {
            Logger.WriteToDebugLog("Dracula is determining which locations can be moved to using Wolf Form, starting with locations connected by one road");
            Logger.WriteToDebugLog("Clearing the old list of possible locations");
            possibleMoves.Clear();
            if (currentLocation.type == LocationType.Sea)
            {
                return;
            }
            for (int i = 0; i < currentLocation.byRoad.Count(); i++)
            {
                if (currentLocation.byRoad[i].type != LocationType.Hospital)
                {
                    Logger.WriteToDebugLog("Adding location " + currentLocation.byRoad[i].name);
                    possibleMoves.Add(currentLocation.byRoad[i]);
                }
                else
                {
                    Logger.WriteToDebugLog("Not adding location " + currentLocation.byRoad[i].name);
                }
            }

            // extend every remaining location by road
            Logger.WriteToDebugLog("Dracula is now adding locations that are connected by two roads");
            List<Location> extendedLocations = new List<Location>();
            for (int i = 0; i < possibleMoves.Count(); i++)
            {
                for (int j = 0; j < possibleMoves[i].byRoad.Count(); j++)
                {
                    if (!possibleMoves.Contains(possibleMoves[i].byRoad[j]) && !extendedLocations.Contains(possibleMoves[i].byRoad[j]))
                    {
                        Logger.WriteToDebugLog("Adding location " + possibleMoves[i].byRoad[j].name);
                        extendedLocations.Add(possibleMoves[i].byRoad[j]);
                    }
                }

            }
            possibleMoves.AddRange(extendedLocations);
            Logger.WriteToDebugLog("Removing current location, " + currentLocation.name + " from the list of possible moves");
            possibleMoves.Remove(currentLocation);
            for (int i = 0; i < trail.Count(); i++)
            {
                Logger.WriteToDebugLog("Removing " + trail[i].name + " from the list, as it is in Dracula's trail");
                possibleMoves.Remove(trail[i]);
            }

        }

        public void DeterminePossiblePowers(int timeOfDay)
        {
            Logger.WriteToDebugLog("Determining possible powers to use");
            Logger.WriteToDebugLog("Clearing the old list of possible powers to use");
            possiblePowers.Clear();
            if (currentLocation.type == LocationType.Sea)
            {
                Logger.WriteToDebugLog("Dracula cannot use any powers at sea");
                return;
            }
            for (int i = 0; i < powers.Count(); i++)
            {
                if (powers[i].positionInTrail > 5 && (timeOfDay > 2 || powers[i].canBeUsedDuringDaylight))
                {
                    if (powers[i].name != "Double Back" || possibleDoubleBackMoves.Count() > 0)
                    {
                        if (powers[i].name != "Hide" || (currentLocation.type != LocationType.Sea && currentLocation.type != LocationType.Castle))
                        {
                            if (powers[i].name == "Wolf Form")
                            {
                                DeterminePossibleWolfFormLocations();
                                if (possibleMoves.Count() > 0)
                                {
                                    Logger.WriteToDebugLog("Adding power " + powers[i].name + " to the list");
                                    possiblePowers.Add(powers[i]);
                                }
                                else
                                {
                                    Logger.WriteToDebugLog("Not adding Wolf Form to the list as it cannot be used validly from this location");
                                }
                                DeterminePossibleLocations();
                            }
                            else
                            {
                                Logger.WriteToDebugLog("Adding power " + powers[i].name + " to the list");
                                possiblePowers.Add(powers[i]);
                            }
                        }
                        else
                        {
                            Logger.WriteToDebugLog("Not adding Hide since Dracula is not at a location in which he can use it");
                        }
                    }
                    else
                    {
                        Logger.WriteToDebugLog("Not adding Double Back since there are no legal moves with it");
                    }
                }
            }
        }

        public void MoveByRoadOrSea(UserInterface ui)
        {
            Logger.WriteToDebugLog("Moving Dracula to a new location");
            Logger.WriteToDebugLog("Remembering that Dracula is moving from a location of type " + currentLocation.type);
            LocationType previousLocationType = currentLocation.type;
            if (possibleMoves.Count() == 0)
            {
                Logger.WriteToDebugLog("SOMEHOW DRACULA IS TRYING TO MOVE WHEN HE HAS NO POSSIBLE MOVES");
            }
            try
            {
                int newIndex = new Random().Next(0, possibleMoves.Count());
                Logger.WriteToDebugLog("Dracula is moving from " + currentLocation.name + " to " + possibleMoves[newIndex].name);
                currentLocation = possibleMoves[newIndex];
            }
            catch (ArgumentOutOfRangeException)
            {
                ui.TellUser("Dracula tried to do something illegal");
                Logger.WriteToDebugLog("DRACULA TRIED TO DO SOMETHING ILLEGAL");
            }

            Logger.WriteToDebugLog("Adding " + currentLocation.name + " to the head of the trail");
            trail.Insert(0, currentLocation);
            MovePowersAlongTrail();
            CheckBloodLossAtSea(previousLocationType);
            Logger.WriteToDebugLog("Checking if Dracula is in Castle Dracula");
            if (currentLocation.type == LocationType.Castle)
            {
                Logger.WriteToDebugLog("Revealing Castle Dracula");
                currentLocation.isRevealed = true;
                if (locationWhereHideWasUsed != null)
                {
                    if (locationWhereHideWasUsed.type == LocationType.Castle)
                    {
                        Logger.WriteToDebugLog("Also revealing Hide as it was used at Castle Dracula");
                        RevealHide(ui);
                    }
                }
            }

        }

        public void TrimTrail(int length)
        {
            Logger.WriteToDebugLog("Trimming Dracula's trail to length " + length);

            while (trail.Count() > length)
            {
                for (int i = 0; i < powers.Count(); i++)
                {
                    if (powers[i].positionInTrail > length - 1)
                    {
                        powers[i].positionInTrail = 6;
                    }
                }
                if (trail.Last().name == "Hide")
                {
                    locationWhereHideWasUsed = null;
                }
                trail.Last().isRevealed = false;
                if (trail.Last() != currentLocation)
                {
                    while (trail.Last().encounters.Count() > 0)
                    {
                        Logger.WriteToDebugLog("Moving encounter " + trail.Last().encounters[0].name + " back to the encounter pool");
                        g.AddEncounterToEncounterPool(trail.Last().encounters[0]);
                        trail.Last().encounters[0].isRevealed = false;
                        trail.Last().encounters.Remove(trail.Last().encounters[0]);
                    }
                }
                else
                {
                    Logger.WriteToDebugLog("Not moving this location's encounters back to the encounter pool because it is Dracula's current location");
                }
                Logger.WriteToDebugLog("Removing " + trail.Last().name + " from the trail");
                trail.Remove(trail.Last());
            }
            if (!trail.Contains(currentLocation))
            {
                Logger.WriteToDebugLog("Dracula's current location was removed from the trail, adding it back at what will be the end of the trail");
                trail.Insert(length - 1, currentLocation);
                Logger.WriteToDebugLog("Removing " + trail.Last().name);
                trail.Remove(trail.Last());
            }
        }

        public void MovePowersAlongTrail()
        {
            for (int i = 0; i < powers.Count(); i++)
            {
                Logger.WriteToDebugLog("Moving power " + powers[i].name + " from position " + powers[i].positionInTrail + " to " + (powers[i].positionInTrail + 1));
                powers[i].positionInTrail++;
            }
        }

        public void RevealHide(int hideIndex, UserInterface ui)
        {
            // move Hide's encounter to the actual location card if applicable
            // ACTUALLY, THIS DOESN'T HAPPEN
            //if (moveEncounterToHideLocation)
            //{
            //    Logger.WriteToDebugLog("Moving the encounter on the Hide card to the actual location card");
            //    for (int i = 0; i < trail[hideIndex].encounters.Count(); i++)
            //    {
            //        Logger.WriteToDebugLog("Moving encounter " + trail[hideIndex].encounters[i].name + " to location " + locationWhereHideWasUsed.name);
            //        locationWhereHideWasUsed.encounters.Add(trail[hideIndex].encounters[i]);
            //    }
            //}
            Logger.WriteToDebugLog("Clearing the encounters from the Hide card");
            trail[hideIndex].encounters.Clear();


            // remove Hide from the trail
            Logger.WriteToDebugLog("Removing Hide from the trail");
            trail.Remove(trail[hideIndex]);
            locationWhereHideWasUsed = null;
            // move all power cards beyond that point up one position
            for (int i = 0; i < powers.Count(); i++)
            {
                if (powers[i].name == "Hide")
                {
                    Logger.WriteToDebugLog("Putting the Hide power into position 6 (out of the trail)");
                    powers[i].positionInTrail = 6;
                }
                else if (powers[i].positionInTrail > hideIndex && powers[i].positionInTrail < 6)
                {
                    Logger.WriteToDebugLog("Moving power " + powers[i].name + " further up the trail");
                    powers[i].positionInTrail--;
                }
            }

            ui.TellUser("Dracula was hiding");
        }

        public void RevealHide(UserInterface ui)
        {
            RevealHide(trail.FindIndex(location => location.name == "Hide"), ui);
        }

        public void CheckBloodLossAtSea(LocationType locationType)
        {
            Logger.WriteToDebugLog("Checking if Dracula should lose blood due to moving at sea");
            if (currentLocation.type == LocationType.Sea && locationType != LocationType.Sea)
            {
                Logger.WriteToDebugLog("Dracula has embarked this turn, losing one blood");
                blood--;
                lostBloodAtSeaOnLatestTurn = true;
            }
            else if (currentLocation.type == LocationType.Sea && !lostBloodAtSeaOnLatestTurn)
            {
                Logger.WriteToDebugLog("Dracula is at sea and did not lose blood last turn, losing one blood");
                blood--;
                lostBloodAtSeaOnLatestTurn = true;
            }
            else if (currentLocation.type == LocationType.Sea)
            {
                Logger.WriteToDebugLog("Dracula is at sea but lost blood last turn");
                lostBloodAtSeaOnLatestTurn = false;
            }
            else
            {
                Logger.WriteToDebugLog("Dracula is not at sea");
                lostBloodAtSeaOnLatestTurn = false;
            }
        }

        public void AddDummyPowerCardToTrail(int index)
        {
            // add the power card to the location trail by using a dummy "location"
            Location powerCard = new Location();
            powerCard.name = possiblePowers[index].name;
            powerCard.abbreviation = possiblePowers[index].name.Substring(0, 3).ToUpper();
            if (powerCard.name != "Hide")
            {
                Logger.WriteToDebugLog("Power card is not Hide, so it is added to the trail revealed");
                powerCard.isRevealed = true;
                powerCard.type = LocationType.Power;
            }
            Logger.WriteToDebugLog("Adding the power card " + powerCard.name + " to the head of trail");
            trail.Insert(0, powerCard);
            MovePowersAlongTrail();
        }

        public void DoDoubleBackMove(UserInterface ui)
        {
            Logger.WriteToDebugLog("Remembering that Dracula is moving from a location of type " + currentLocation.type);
            LocationType previousLocationType = currentLocation.type;


            // choose a location from the possible double back moves
            int doubleBackLocation = new Random().Next(0, possibleDoubleBackMoves.Count());
            bool doublingBackToCatacombs = false;

            int doubleBackIndex = trail.FindIndex(location => location == possibleDoubleBackMoves[doubleBackLocation]);
            if (doubleBackIndex > -1)
            {
                Logger.WriteToDebugLog("Doubling back to a location in the trail");
                if (doubleBackIndex > 0)
                {
                    if (trail[doubleBackIndex - 1].name == "Hide")
                    {
                        Logger.WriteToDebugLog("Dracula Doubled Back to " + possibleDoubleBackMoves[doubleBackLocation].name + " and Hide is the next card in the trail");
                        ui.TellUser("Dracula Doubled Back to a location where he previously used Hide (position " + (doubleBackIndex - 1) + ")");
                        RevealHide(doubleBackIndex - 1, ui);
                    }
                    else if (doubleBackIndex > 1)
                    {
                        if (trail[doubleBackIndex - 1].type == LocationType.Power && trail[doubleBackIndex - 2].name == "Hide")
                        {
                            Logger.WriteToDebugLog("Dracula Doubled Back to " + possibleDoubleBackMoves[doubleBackLocation].name + " and Hide is the next card in the trail, after " + trail[doubleBackIndex - 1].name);
                            ui.TellUser("Dracula Doubled Back to a location where he previously used Hide (position " + (doubleBackIndex - 1) + ")");
                            RevealHide(doubleBackIndex - 2, ui);
                        }
                        else if (doubleBackIndex > 2)
                        {
                            if (trail[doubleBackIndex - 1].type == LocationType.Power && trail[doubleBackIndex - 2].type == LocationType.Power && trail[doubleBackIndex - 3].name == "Hide")
                            {
                                Logger.WriteToDebugLog("Dracula Doubled Back to " + possibleDoubleBackMoves[doubleBackLocation].name + " and Hide is the next card in the trail, after " + trail[doubleBackIndex - 1].name + " and " + trail[doubleBackIndex - 2].name);
                                ui.TellUser("Dracula Doubled Back to a location where he previously used Hide (position " + (doubleBackIndex - 1) + ")");
                                RevealHide(doubleBackIndex - 3, ui);
                            }
                        }
                    }
                }
                else
                {
                    Logger.WriteToDebugLog("TRIED TO DOUBLE BACK TO A LOCATION WHEN THERE ARE NONE IN THE LIST");
                }

            }
            else
            {
                Logger.WriteToDebugLog("Doubling back to a location in the Catacombs");
                doublingBackToCatacombs = true;
            }


            // move location to the front of the trail
            Logger.WriteToDebugLog("Temporarily holding " + possibleDoubleBackMoves[doubleBackLocation].name);
            Location tempLocation = possibleDoubleBackMoves[doubleBackLocation];
            if (doublingBackToCatacombs)
            {
                Logger.WriteToDebugLog("Removing " + tempLocation.name + " from the Catacombs");
                catacombs[Array.IndexOf(catacombs, tempLocation)] = null;
            }
            else
            {
                Logger.WriteToDebugLog("Removing " + tempLocation.name + " from the trail");
                trail.Remove(tempLocation);
            }
            Logger.WriteToDebugLog("Putting " + tempLocation.name + " back at the head of the trail");
            trail.Insert(0, tempLocation);
            Logger.WriteToDebugLog("Dracula's current location is now " + currentLocation.name);
            currentLocation = trail[0];

            Logger.WriteToGameLog("Dracula Doubled Back to " + currentLocation.name + " from " + (doublingBackToCatacombs ? " the Catacombs" : " his trail"));

            // move the power cards that are in the trail
            for (int i = 0; i < powers.Count(); i++)
            {
                if (powers[i].positionInTrail == doubleBackLocation)
                {
                    Logger.WriteToDebugLog("Moving Double Back power position to the head of the trail");
                    powers[i].positionInTrail = 0;
                }
                else if (powers[i].positionInTrail < doubleBackLocation)
                {
                    Logger.WriteToDebugLog("Moving power " + powers[i].name + " position further along the trail");
                    powers[i].positionInTrail++;
                }
            }
            CheckBloodLossAtSea(previousLocationType);
        }

        public void DoWolfFormMove(UserInterface ui)
        {
            // determine possible locations for wolf form (road only, up to two moves away)
            DeterminePossibleWolfFormLocations();
            // carry out move
            MoveByRoadOrSea(ui);
            Logger.WriteToGameLog("Dracula used Wolf Form to move to " + currentLocation.name);
        }

        public void DrawEncounters(int totalNumberOfEncounters)
        {
            Logger.WriteToDebugLog("DRAWING ENCOUNTERS");
            Logger.WriteToDebugLog("Dracula is drawing up to " + totalNumberOfEncounters + " encounters");
            while (encounterHand.Count() < totalNumberOfEncounters)
            {
                Encounter tempEncounter = g.DrawEncounterFromEncounterPool();
                Logger.WriteToDebugLog("Dracula drew encounter " + tempEncounter.name);
                Logger.WriteToGameLog("Dracula drew encounter " + tempEncounter.name);
                encounterHand.Add(tempEncounter);
                g.RemoveEncounterFromEncounterPool(tempEncounter);
            }
        }

        public void PlaceEncounterIfLegal(Location location)
        {
            if (location.type == LocationType.Castle)
            {
                Logger.WriteToDebugLog("Not placing an encounter here as this is Castle Dracula");
                return;
            }
            if (location.type == LocationType.Sea)
            {
                Logger.WriteToDebugLog("Not placing an encounter here as it is a sea location");
                return;
            }
            Logger.WriteToDebugLog("Checking if Dracula used a power at this location");
            int locationIndex = trail.FindIndex(loc => loc == location);
            if (locationIndex > -1)
            {
                Logger.WriteToDebugLog("This location is in the trail, checking if a power was used here");
                if (trail[locationIndex].name == "Dark Call")
                {
                    Logger.WriteToDebugLog("Not placing an encounter here as Dracula used Dark Call");
                    return;
                }
                if (trail[locationIndex].name == "Feed")
                {
                    Logger.WriteToDebugLog("Not placing an encounter here as Dracula used Feed");
                    return;
                }
                for (int i = 0; i < powers.Count(); i++)
                {
                    if (powers[i].name == "Double Back" && powers[i].positionInTrail == locationIndex)
                    {
                        Logger.WriteToDebugLog("Not placing an encounter here as Dracula used Double Back");
                        return;
                    }
                }
                Logger.WriteToDebugLog("Checking if this location already has an encounter");
                if (trail[locationIndex].encounters.Count() > 0)
                {
                    Logger.WriteToDebugLog("THIS LOCATION ALREADY HAS AN ENCOUNTER. THIS SHOULD NOT BE POSSIBLE. INVESTIGATE.");
                    return;
                }
            }
            else
            {
                Logger.WriteToDebugLog("This location isn't in the trail, not checking any further");
            }
            int chosenEncounter = new Random().Next(0, encounterHand.Count());
            Logger.WriteToDebugLog("Placing encounter " + encounterHand[chosenEncounter].name);
            Logger.WriteToGameLog("Dracula placed encounter " + encounterHand[chosenEncounter].name);
            location.encounters.Add(encounterHand[chosenEncounter]);
            encounterHand.Remove(encounterHand[chosenEncounter]);
        }

        public void HandleDroppedOffLocations(UserInterface ui)
        {
            Logger.WriteToDebugLog("DEALING WITH DROPPED OFF LOCATIONS");
            while (trail.Count > 6)
            {
                if (trail.Last() == locationWhereHideWasUsed)
                {
                    Logger.WriteToDebugLog("Hide location has left the trail, revealing the Hide card");
                    RevealHide(trail.FindIndex(location => location.name == "Hide"), ui);
                }
                Logger.WriteToDebugLog("Deciding what to do with " + trail.Last().name);
                if (trail.Last().type == LocationType.Power)
                {
                    Logger.WriteToDebugLog(trail.Last().name + " is a power card, so it gets discarded");
                    TrimTrail(trail.Count() - 1);
                    return;
                }
                if (trail.Last().type == LocationType.Sea)
                {
                    Logger.WriteToDebugLog(trail.Last().name + " is a sea location, so it gets discarded");
                    TrimTrail(trail.Count() - 1);
                    return;
                }
                int emptyIndex = Array.IndexOf(catacombs, null);
                if (emptyIndex > -1)
                {
                    Logger.WriteToDebugLog("There is a space in the Catacombs");
                    if (new Random().Next(0, 6) > 4)
                    {
                        Logger.WriteToDebugLog("Putting " + trail.Last().name + " into the Catacombs");
                        catacombs[emptyIndex] = trail.Last();
                        trail.Remove(trail.Last());
                        PlaceEncounterIfLegal(catacombs[emptyIndex]);
                        Logger.WriteToGameLog("Dracula put " + catacombs[emptyIndex].name + " into the Catacombs with encounters");
                    }
                    else
                    {
                        Logger.WriteToDebugLog("Discarding " + trail.Last().name);
                        while (trail.Last().encounters.Count() > 0)
                        {
                            Logger.WriteToDebugLog("Hanging on to " + trail.Last().encounters.First().name + " to mature it later");
                            encountersToMature.Add(trail.Last().encounters.First());
                            trail.Last().encounters.Remove(trail.Last().encounters.First());
                        }
                        Logger.WriteToDebugLog("Discarding " + trail.Last().name);
                        TrimTrail(trail.Count() - 1);
                    }
                }
                else
                {
                    Logger.WriteToDebugLog("There is no space in the Catacombs, discarding " + trail.Last().name);
                    TrimTrail(trail.Count() - 1);
                }
            }
        }

        public void TakeStartOfTurnActions(UserInterface ui)
        {
            Logger.WriteToDebugLog("Deciding what to do with Catacombs locations");
            for (int i = 0; i < 3; i++)
            {
                if (catacombs[i] != null)
                {
                    Logger.WriteToDebugLog("Deciding what to do with location " + catacombs[i].name);
                    if (new Random().Next(0, 5) > 3)
                    {
                        Logger.WriteToDebugLog("Discarding " + catacombs[i].name);
                        while (catacombs[i].encounters.Count() > 0)
                        {
                            Logger.WriteToDebugLog("Putting encounter " + catacombs[i].encounters.First().name + " back into the encounter pool");
                            g.AddEncounterToEncounterPool(catacombs[i].encounters.First());
                            catacombs[i].encounters.Remove(catacombs[i].encounters.First());
                        }
                        Logger.WriteToDebugLog("Emptying " + catacombs[i].name + " from Catacombs");
                        catacombs[i] = null;
                    }
                }
            }
        }

        public void DoActionPhase(UserInterface ui)
        {
            Logger.WriteToDebugLog("PERFORMING ACTION PHASE");
            Logger.WriteToDebugLog("Placing an encounter");
            PlaceEncounterIfLegal(trail[0]);
        }

        public void MatureEncounters(UserInterface ui)
        {
            Logger.WriteToDebugLog("MATURING ENCOUNTERS");
            while (encountersToMature.Count > 0)
            {
                Logger.WriteToDebugLog("Maturing encounter " + encountersToMature.First().name);
                g.MatureEncounter(encountersToMature.First().name, ui);
                g.AddEncounterToEncounterPool(encountersToMature.First());
                encountersToMature.Remove(encountersToMature.First());
            }
        }
    }

    public class DraculaPower
    {
        public string name;
        public bool canBeUsedDuringDaylight;
        public int positionInTrail;

        public DraculaPower(string newName, bool daylightPower)
        {
            name = newName;
            canBeUsedDuringDaylight = daylightPower;
            positionInTrail = 6;
        }
    }

}
