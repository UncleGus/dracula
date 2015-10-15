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
using HunterHandler;

namespace DraculaHandler
{
    public class Dracula
    {
        public Location currentLocation;
        public List<Location> trail = new List<Location>();
        public List<DraculaPower> powers = new List<DraculaPower>();
        public List<Location> possibleMoves = new List<Location>();
        public List<Location> possibleDoubleBackMoves = new List<Location>();
        public List<DraculaPower> possiblePowers = new List<DraculaPower>();
        public int blood;
        public bool lostBloodAtSeaOnLatestTurn;
        public int encounterHandSize;
        public List<Encounter> encountersToMature = new List<Encounter>();
        public List<Encounter> encounterHand = new List<Encounter>();
        public Location locationWhereHideWasUsed;
        public Location[] catacombs = new Location[3];
        public List<Event> eventCardsInHand = new List<Event>();
        public int eventHandSize;
        public DecisionMaker logic = new DecisionMaker();
        public string advanceMovePower = null;
        public Location advanceMoveDestination = null;

        public Dracula()
        {
            powers.Add(new DraculaPower("Dark Call", true));
            powers.Add(new DraculaPower("Double Back", true));
            powers.Add(new DraculaPower("Feed", false));
            powers.Add(new DraculaPower("Hide", true));
            powers.Add(new DraculaPower("Wolf Form", false));
            blood = 15;
            lostBloodAtSeaOnLatestTurn = false;
            encounterHandSize = 5;
            eventHandSize = 4;
        }

        public bool DraculaMovementPhase(GameState g, UserInterface ui)
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


            string powerUsed;
            Location destination;
            logic.DecideMove(g, this, out powerUsed, out destination);

            // choose a power
            if (powerUsed != "no power")
            {
                Logger.WriteToDebugLog("Dracula has chosen power " + powerUsed);
                if (powerUsed != "Double Back" && powerUsed != "Wolf Form")
                {
                    Logger.WriteToGameLog("Dracula used power " + powerUsed);
                }

                if (powerUsed == "Dark Call" || powerUsed == "Feed" || powerUsed == "Hide")
                {
                    AddDummyPowerCardToTrail(powerUsed);
                    if (powerUsed == "Hide")
                    {
                        locationWhereHideWasUsed = currentLocation;
                    }
                }
                else if (powerUsed == "Double Back")
                {
                    DoDoubleBackMove(g, destination, ui);
                }
                else if (powerUsed == "Wolf Form")
                {
                    DoWolfFormMove(g, destination, ui);
                }

                // put the power used at the head of the trail
                Logger.WriteToDebugLog("Putting power " + powerUsed + " at the head of the trail");
                possiblePowers[possiblePowers.FindIndex(power => power.name == powerUsed)].positionInTrail = 0;
            }
            else
            // performing a regular move
            {
                // choose a location
                MoveByRoadOrSea(g, destination, ui);
                Logger.WriteToGameLog("Dracula moved to " + currentLocation.name);
            }
            Logger.WriteToDebugLog("Dracula has finished moving");
            return true;
        }

        public void DrawEventCard(GameState g, UserInterface ui)
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
                PlayAlly(g, eventCardDrawn, ui);
            }
            else if (eventCardDrawn.type == EventType.PlayImmediately)
            {
                PlayImmediately(g, eventCardDrawn, ui);
                g.RemoveEventFromEventDeck(eventCardDrawn);
            }
            else
            {
                eventCardsInHand.Add(eventCardDrawn);
                g.RemoveEventFromEventDeck(eventCardDrawn);
            }
        }

        public void PlayImmediately(GameState g, Event eventCardDrawn, UserInterface ui)
        {
            switch (eventCardDrawn.name)
            {
                case "Evasion":
                    int hunterPlayingGoodluckC = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                    if (hunterPlayingGoodluckC > 0)
                    {
                        g.DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluckC);
                        g.DiscardEventFromDracula("Night Visit");
                    }
                    else
                    {

                        PlayEvasion(g, ui); break;
                    }
                    break;
                case "Night Visit":
                    int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                    if (hunterPlayingGoodluck > 0)
                    {
                        g.DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck);
                        g.DiscardEventFromDracula("Night Visit");
                    }
                    else
                    {
                        PlayNightVisit(g, ui); break;
                    }
                    break;
                case "Vampiric Influence":
                    int hunterPlayingGoodluckB = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                    if (hunterPlayingGoodluckB > 0)
                    {
                        g.DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluckB);
                        g.DiscardEventFromDracula("Vampiric Influence");
                    }
                    else
                    {

                        PlayVampiricInfluence(g, ui); break;
                    }
                    break;
            }
        }

        private void PlayVampiricInfluence(GameState g, UserInterface ui)
        {
            Hunter hunterToInfluence = logic.DecideWhoToInfluence(g);
            ui.TellUser(hunterToInfluence.name + " was affected by Dracula's Vampiric Influence and must reveal all cards and tell Dracula their next move");
            ui.TellUser("Of course, none of this means anything right now because I don't have a brain, so don't bother doing anything");
        }

        private void PlayNightVisit(GameState g, UserInterface ui)
        {
            Hunter hunterToVisit = logic.DecideWhoToNightVisit(g);
            ui.TellUser(hunterToVisit.name + " was visited by Dracula in the night and loses 2 health");
            hunterToVisit.health -= 2;
            g.HandlePossibleHunterDeath(ui);
        }

        private void PlayEvasion(GameState g, UserInterface ui)
        {
            ui.TellUser("Dracula drew Evasion");
            Location locationToMoveTo = logic.DecideWhereToEvadeTo(g);
            MoveByRoadOrSea(g, locationToMoveTo, ui);
            PlaceEncounterIfLegal(g, locationToMoveTo);
        }

        public void PlayAlly(GameState g, Event allyDrawn, UserInterface ui)
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
            }
            else
            {
                Logger.WriteToDebugLog("Dracula already has an Ally, deciding which one to keep");
                string allyToKeep = logic.DecideWhichAllyToKeep(g.NameOfDraculaAlly(), allyDrawn.name);
                string allyDiscarded = (allyToKeep == allyDrawn.name ? g.NameOfDraculaAlly() : allyDrawn.name);
                Logger.WriteToDebugLog("Keeping " + allyToKeep);
                g.AddEventToEventDiscard(g.GetEventByNameFromEventDeck(allyDiscarded));
                g.SetDraculaAlly(g.GetEventByNameFromEventDeck(allyToKeep));
                switch (allyDiscarded)
                {
                    case "Immanuel Hildesheim":
                        {
                            Logger.WriteToDebugLog("Discarded Immanuel Hildesheim, discarding events down to 4");
                            eventHandSize = 4;
                            DiscardEventsDownTo(g, eventHandSize, ui);
                            break;
                        }
                    case "Dracula's Brides":
                        {
                            Logger.WriteToDebugLog("Discarding Dracula's Brides, discarding encounters down to 5");
                            encounterHandSize = 5;
                            DiscardEncountersDownTo(g, encounterHandSize);
                            break;
                        }
                }
                allyKept = allyToKeep;
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

        public void DiscardEncountersDownTo(GameState g, int encountersToKeep)
        {
            while (encounterHand.Count() > encountersToKeep)
            {
                Encounter encounterToDiscard = logic.DecideWhichEncounterToDiscard(g, this);
                encounterHand.Remove(encounterToDiscard);
                g.AddEncounterToEncounterPool(encounterToDiscard);
                Logger.WriteToDebugLog("Dracula discarded " + encounterToDiscard.name);
                Logger.WriteToGameLog("Dracula discarded " + encounterToDiscard.name);
            }

        }

        public void DiscardEventsDownTo(GameState g, int numberOfCards, UserInterface ui)
        {
            while (eventCardsInHand.Count() > numberOfCards)
            {
                Event cardToDiscard = logic.DecideWhichEventToDiscard(g, this);
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

        internal void OrderEncounters(Hunter hunter, Location location)
        {
            logic.DecideOrderOfEncountersAtLocation(hunter, location);
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
                if (currentLocation.bySea[i].turnsUntilStormSubsides < 1)
                {
                    Logger.WriteToDebugLog("Adding location " + currentLocation.bySea[i].name);
                    possibleMoves.Add(currentLocation.bySea[i]);
                }
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
            Logger.WriteToDebugLog("Checking possible move locations for Heavenly Hosts and Consecrated Ground");
            List<Location> movesToRemove = new List<Location>();
            foreach (Location loc in possibleMoves)
            {
                if (loc.hasHost || loc.isConsecrated)
                {
                    movesToRemove.Add(loc);
                }
            }
            foreach (Location mov in movesToRemove)
            {
                possibleMoves.Remove(mov);
            }
            Logger.WriteToDebugLog("Checking possible double back locations for Heavenly Hosts and Consecrated Ground");
            movesToRemove.Clear();
            foreach (Location loc in possibleDoubleBackMoves)
            {
                if (loc.hasHost || loc.isConsecrated)
                {
                    movesToRemove.Add(loc);
                }
            }
            foreach (Location mov in movesToRemove)
            {
                possibleDoubleBackMoves.Remove(mov);
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
                if (currentLocation.byRoad[i].type != LocationType.Hospital && !currentLocation.byRoad[i].hasHost && !currentLocation.byRoad[i].isConsecrated)
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
                    if (!possibleMoves.Contains(possibleMoves[i].byRoad[j]) && !extendedLocations.Contains(possibleMoves[i].byRoad[j]) && possibleMoves[i].byRoad[j].type != LocationType.Hospital && !possibleMoves[i].byRoad[j].hasHost && !possibleMoves[i].byRoad[j].isConsecrated)
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
                                // Wolf Form
                                DeterminePossibleWolfFormLocations();
                                if (possibleMoves.Count() > 0)
                                {
                                    Logger.WriteToDebugLog("Adding Wolf Form to the list");
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
                                // Hide, Feed, Dark Call, Double Back
                                if (currentLocation.isConsecrated || currentLocation.hasHost)
                                {
                                    Logger.WriteToDebugLog("Not adding power " + powers[i].name + " to the list as it would cause Dracula to stay in the same location as a Host/Consecrated Ground");
                                }
                                else
                                {
                                    Logger.WriteToDebugLog("Adding power " + powers[i].name + " to the list");
                                    possiblePowers.Add(powers[i]);
                                }
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

        public void MoveByRoadOrSea(GameState g, Location goingTo, UserInterface ui)
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
                Logger.WriteToDebugLog("Dracula is moving from " + currentLocation.name + " to " + goingTo.name);
                currentLocation = goingTo;
            }
            catch (ArgumentOutOfRangeException)
            {
                ui.TellUser("Dracula tried to do something illegal");
                Logger.WriteToDebugLog("DRACULA TRIED TO DO SOMETHING ILLEGAL");
            }

            Logger.WriteToDebugLog("Adding " + currentLocation.name + " to the head of the trail");
            trail.Insert(0, currentLocation);
            MovePowersAlongTrail();
            CheckBloodLossAtSea(previousLocationType, g.NameOfHunterAlly());
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

        internal Event WillPlayCardToCancelCharteredCarriage(GameState g)
        {
            return logic.DecideWhetherToCancelCharteredCarriage(g, this);
        }

        public void TrimTrail(GameState g, int length)
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

        public void CheckBloodLossAtSea(LocationType locationType, string hunterAllyName)
        {
            Logger.WriteToDebugLog("Checking if Dracula should lose blood due to moving at sea");
            if (currentLocation.type == LocationType.Sea && locationType != LocationType.Sea)
            {
                Logger.WriteToDebugLog("Dracula has embarked this turn, losing one blood");
                blood--;
                lostBloodAtSeaOnLatestTurn = true;
            }
            else if (currentLocation.type == LocationType.Sea && hunterAllyName == "Rufus Smith")
            {
                Logger.WriteToDebugLog("Dracula is at sea and Rufus Smith is in play, losing one blood");
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

        public void AddDummyPowerCardToTrail(string powerName)
        {
            // add the power card to the location trail by using a dummy "location"
            Location powerCard = new Location();
            powerCard.name = powerName;
            powerCard.abbreviation = powerName.Substring(0, 3).ToUpper();
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

        public void DoDoubleBackMove(GameState g, Location goingTo, UserInterface ui)
        {
            Logger.WriteToDebugLog("Remembering that Dracula is moving from a location of type " + currentLocation.type);
            LocationType previousLocationType = currentLocation.type;

            bool doublingBackToCatacombs = false;

            int doubleBackIndex = trail.FindIndex(location => location == goingTo);
            if (doubleBackIndex > -1)
            {
                Logger.WriteToDebugLog("Doubling back to a location in the trail");
                if (doubleBackIndex > 0)
                {
                    if (trail[doubleBackIndex - 1].name == "Hide")
                    {
                        Logger.WriteToDebugLog("Dracula Doubled Back to " + goingTo + " and Hide is the next card in the trail");
                        ui.TellUser("Dracula Doubled Back to a location where he previously used Hide (position " + (doubleBackIndex - 1) + ")");
                        RevealHide(doubleBackIndex - 1, ui);
                    }
                    else if (doubleBackIndex > 1)
                    {
                        if (trail[doubleBackIndex - 1].type == LocationType.Power && trail[doubleBackIndex - 2].name == "Hide")
                        {
                            Logger.WriteToDebugLog("Dracula Doubled Back to " + goingTo + " and Hide is the next card in the trail, after " + trail[doubleBackIndex - 1].name);
                            ui.TellUser("Dracula Doubled Back to a location where he previously used Hide (position " + (doubleBackIndex - 1) + ")");
                            RevealHide(doubleBackIndex - 2, ui);
                        }
                        else if (doubleBackIndex > 2)
                        {
                            if (trail[doubleBackIndex - 1].type == LocationType.Power && trail[doubleBackIndex - 2].type == LocationType.Power && trail[doubleBackIndex - 3].name == "Hide")
                            {
                                Logger.WriteToDebugLog("Dracula Doubled Back to " + goingTo + " and Hide is the next card in the trail, after " + trail[doubleBackIndex - 1].name + " and " + trail[doubleBackIndex - 2].name);
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

            int doubleBackLocation = trail.FindIndex(loc => loc == goingTo);
            // move location to the front of the trail
            Logger.WriteToDebugLog("Temporarily holding " + goingTo.name);
            if (doublingBackToCatacombs)
            {
                Logger.WriteToDebugLog("Removing " + goingTo.name + " from the Catacombs");
                catacombs[Array.IndexOf(catacombs, goingTo)] = null;
                Encounter encounterToDiscard = logic.DecideWhichCatacombsEncounterToDiscard(g, goingTo, ui);
                goingTo.encounters.Remove(encounterToDiscard);
                g.AddEncounterToEncounterPool(encounterToDiscard);
            }
            else
            {
                Logger.WriteToDebugLog("Removing " + goingTo.name + " from the trail");
                trail.Remove(goingTo);
            }
            Logger.WriteToDebugLog("Putting " + goingTo.name + " back at the head of the trail");
            trail.Insert(0, goingTo);
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
            CheckBloodLossAtSea(previousLocationType, g.NameOfHunterAlly());
        }

        public void DoWolfFormMove(GameState g, Location destination, UserInterface ui)
        {
            // determine possible locations for wolf form (road only, up to two moves away)
            DeterminePossibleWolfFormLocations();
            // carry out move
            MoveByRoadOrSea(g, destination, ui);
            Logger.WriteToGameLog("Dracula used Wolf Form to move to " + currentLocation.name);
        }

        public void DrawEncounters(GameState g, int totalNumberOfEncounters)
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

        public void PlaceEncounterIfLegal(GameState g, Location location)
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
            Encounter chosenEncounter = logic.DecideWhichEncounterToPlace(g, this);
            Logger.WriteToDebugLog("Placing encounter " + chosenEncounter.name);
            Logger.WriteToGameLog("Dracula placed encounter " + chosenEncounter.name);
            location.encounters.Add(chosenEncounter);
            encounterHand.Remove(chosenEncounter);
        }

        internal Event WillPlayDevilishPower(GameState g, UserInterface ui)
        {
            Event eventToReturn = logic.DecideWhetherToPlayDevilishPower(g, this);
            if (eventToReturn != null)
            {
                int hunterIndex = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                if (hunterIndex > 0)
                {
                    g.DiscardEventFromHunterAtIndex("Good Luck", hunterIndex);
                    eventCardsInHand.Remove(eventCardsInHand.Find(c => c.name == "Devilish Power"));
                    eventToReturn = null;
                }
            }
            return eventToReturn;
        }

        public void HandleDroppedOffLocations(GameState g, UserInterface ui)
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
                    TrimTrail(g, trail.Count() - 1);
                    return;
                }
                if (trail.Last().type == LocationType.Sea)
                {
                    Logger.WriteToDebugLog(trail.Last().name + " is a sea location, so it gets discarded");
                    TrimTrail(g, trail.Count() - 1);
                    return;
                }
                int emptyIndex = Array.IndexOf(catacombs, null);
                if (emptyIndex > -1)
                {
                    Logger.WriteToDebugLog("There is a space in the Catacombs");
                    if (logic.DecideToPutLocationInCatacombs(g, this))
                    {
                        Logger.WriteToDebugLog("Putting " + trail.Last().name + " into the Catacombs");
                        catacombs[emptyIndex] = trail.Last();
                        trail.Remove(trail.Last());
                        PlaceEncounterIfLegal(g, catacombs[emptyIndex]);
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
                        TrimTrail(g, trail.Count() - 1);
                    }
                }
                else
                {
                    Logger.WriteToDebugLog("There is no space in the Catacombs, discarding " + trail.Last().name);
                    TrimTrail(g, trail.Count() - 1);
                }
            }
        }

        public void TakeStartOfTurnActions(GameState g, UserInterface ui)
        {
            Logger.WriteToDebugLog("Deciding what to do with Catacombs locations");
            for (int i = 0; i < 3; i++)
            {
                if (catacombs[i] != null)
                {
                    Logger.WriteToDebugLog("Deciding what to do with location " + catacombs[i].name);
                    if (logic.DecideToDiscardCatacombLocation(g, this))
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

        public void DoActionPhase(GameState g, UserInterface ui)
        {
            Logger.WriteToDebugLog("PERFORMING ACTION PHASE");
            if (g.IndexOfHunterAtLocation(currentLocation) > -1)
            {
                g.ResolveCombat(g.IndexOfHunterAtLocation(currentLocation), 1, ui);
            }
            else
            {
                Logger.WriteToDebugLog("Placing an encounter");
                PlaceEncounterIfLegal(g, trail[0]);
            }
        }

        public void MatureEncounters(GameState g, UserInterface ui)
        {
            Logger.WriteToDebugLog("MATURING ENCOUNTERS");
            while (encountersToMature.Count > 0)
            {
                Logger.WriteToDebugLog("Maturing encounter " + encountersToMature.First().name);
                Logger.WriteToGameLog(encountersToMature.First().name + " matured");
                g.MatureEncounter(encountersToMature.First().name, ui);
                g.AddEncounterToEncounterPool(encountersToMature.First());
                encountersToMature.Remove(encountersToMature.First());
            }
        }

        internal void DiscardHunterCard(GameState g, HunterHandler.Hunter hunter, UserInterface ui)
        {
            string cardToDiscard = logic.DecideToDiscardEventOrItem(g, this, hunter);
            if (cardToDiscard != "no cards")
            {
                ui.TellUser(hunter.name + " must discard an " + cardToDiscard);
                ui.TellUser("Don't forget to tell me what was discarded");
            }
        }

        internal bool WillCancelTrain(GameState g, HunterHandler.Hunter hunter)
        {
            return logic.DecideToCancelHunterTrain(g, this, hunter);
        }

        internal void DiscardEventFromHand(GameState g, string p)
        {
            g.DiscardEventCard(p);
            eventCardsInHand.Remove(GetEventCardFromHand(p));
        }

        private Event GetEventCardFromHand(string p)
        {
            return eventCardsInHand[eventCardsInHand.FindIndex(card => card.name == p)];
        }

        internal Item ChooseCombatCardAndTarget(Hunter hunter, List<Item> combatCards, CombatRoundResult result, out string name)
        {
            name = logic.DecideHunterToAttack(hunter, combatCards, result);
            return logic.DecideWhichCombatCardToPlay(hunter, combatCards, result);
        }

        internal Location DecideWhereToSendHunterWithBats(GameState gameState, Hunter hunter, UserInterface ui)
        {
            List<Location> batsMoves = new List<Location>();
            foreach (Location loc in hunter.currentLocation.byRoad)
            {
                batsMoves.Add(loc);
            }
            return batsMoves[new Random().Next(0, batsMoves.Count())];
        }

        internal void DoEscapeAsBatsMove(GameState g, UserInterface ui)
        {
            DeterminePossibleWolfFormLocations();
            foreach (Location loc in possibleMoves)
            {
                if (g.NumberOfHuntersAtLocation(loc.name) > 0)
                {
                    possibleMoves.Remove(loc);
                }
            }
            possiblePowers.Clear();
            string throwAway;
            Location locationToMoveTo;
            logic.DecideMove(g, this, out throwAway, out locationToMoveTo);
            MoveByRoadOrSea(g, locationToMoveTo, ui);
        }

        internal Event PlayEventCardAtStartOfHunterMovement(GameState g)
        {
            return logic.DecideEventCardToPlayAtStartOfHunterMovement(g, this);
        }

        internal Event PlayEventCardAtStartOfDraculaTurn(GameState g)
        {
            return logic.DecideEventCardToPlayAtStartOfDraculaTurn(g, this);
        }

        internal Event PlaySeductionCard(GameState g)
        {
            return logic.DecideToPlaySeductionDuringVampireEncounter(g, this);
        }

        internal Event PlayEventCardAtStartOfCombat(GameState g, bool trapPlayed)
        {
            return logic.DecideToPlayCardAtStartOfCombat(g, this, trapPlayed);
        }

        internal Location DecideWhereToSendHunterWithControlStorms(int hunterIndex, List<Location> possiblePorts, GameState g)
        {
            return logic.DecideLocationToSendHunterWithControlStorms(g, hunterIndex, possiblePorts);
        }

        internal Event WillPlayCustomsSearch(GameState g, Hunter hunter)
        {
            return logic.DecideToPlayCustomsSearch(g, this, hunter);
        }

        internal Hunter ChooseHunterToRage(List<Hunter> huntersInCombat)
        {
            return logic.DecideWhichHunterToRage(huntersInCombat);
        }

        internal Item ChooseItemCardToDiscard(List<Item> items)
        {
            return logic.DecideWhichItemToDiscard(items);
        }

        internal Location ChooseLocationToSendHuntersToWithWildHorses(GameState g, List<Hunter> hunters)
        {
            return logic.DecideWhereToSendHuntersWithWildHorses(g, hunters);
        }

        internal Event WillPlayRelentlessMinion(GameState g, List<Hunter> huntersEncountered, string enemyType)
        {
            return logic.DecideWhetherToPlayRelentlessMinion(g, huntersEncountered, enemyType, this);
        }

        internal void PlaceRoadBlockCounter(GameState g, Roadblock roadblockCounter)
        {
            logic.DecideWhereToPutRoadblock(g, roadblockCounter);
        }

        internal Event WillPlaySensationalistPress(GameState g, int trailIndex)
        {
            return logic.DecideWhetherToPlaySensationalistPress(g, trailIndex, this);
        }

        internal Location DecideWhichPortToGoToAfterStormySeas(GameState g, Location locationStormed)
        {
            return logic.DecideWhichPortToGoToAfterStormySeas(g, locationStormed);
        }

        internal void PlayDevilishPowerToRemoveHeavenlyHostOrHunterAlly(GameState g, UserInterface ui)
        {
            List<Location> map = g.GetMap();
            if (map.FindIndex(l => l.hasHost == true) > -1)
            {
                if (g.HuntersHaveAlly())
                {
                }
                else
                {
                    Location locationToRemoveHost = logic.DecideWhichLocationToRemoveHeavenlyHostFrom(g);
                    ui.TellUser("Dracula played Devilish Power to discard a Heavenly Host from " + locationToRemoveHost.name);
                    int hunterIndex = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                    if (hunterIndex > 0)
                    {
                        g.DiscardEventFromHunterAtIndex("Good Luck", hunterIndex);
                    }
                    else
                    {
                        locationToRemoveHost.hasHost = false;
                    }
                }

            }
            else
            {
                ui.TellUser("Dracula played Devilish Power to discard the Hunters' Ally from play");
                int hunterIndex = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                if (hunterIndex > 0)
                {
                    g.DiscardEventFromHunterAtIndex("Good Luck", hunterIndex);
                }
                else
                {
                    g.RemoveHunterAlly();
                }
            }
        }

        internal Hunter ChooseHunterToAmbush(Hunter[] hunters)
        {
            return logic.DecideWhichHunterToAmbush(hunters);
        }

        internal Encounter ChooseEncounterToAmbushHunter()
        {
            return logic.DecideWhichEncounterToAmbushHunterWith(encounterHand);
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
