using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationHandler;
using LogHandler;
using EncounterHandler;
using DraculaSimulator;
using EventHandler;
using HunterHandler;
using System.Runtime.Serialization;

namespace DraculaHandler
{
    [DataContract]
    public class Dracula
    {
        [DataMember]
        public Location CurrentLocation { get; set; }
        [DataMember]
        public List<Location> Trail { get; set; }
        [DataMember]
        public List<DraculaPower> Powers { get; set; }
        [DataMember]
        public List<Location> PossibleMoves { get; set; }
        [DataMember]
        public List<Location> PossibleDoubleBackMoves { get; set; }
        [DataMember]
        public List<DraculaPower> PossiblePowers { get; set; }
        [DataMember]
        public int Blood { get; set; }
        [DataMember]
        public bool LostBloodAtSeaOnLatestTurn { get; set; }
        [DataMember]
        public int EncounterHandSize { get; set; }
        [DataMember]
        public List<Encounter> EncountersToMature { get; set; }
        [DataMember]
        public List<Encounter> EncounterHand { get; set; }
        [DataMember]
        public Location LocationWhereHideWasUsed { get; set; }
        [DataMember]
        public Location[] Catacombs { get; set; }
        [DataMember]
        public List<Event> EventCardsInHand { get; set; }
        [DataMember]
        public int EventHandSize { get; set; }
        [DataMember]
        public DecisionMaker logic { get; set; }
        [DataMember]
        public string AdvanceMovePower { get; set; }
        [DataMember]
        public Location AdvanceMoveDestination { get; set; }

        public Dracula()
        {
            Trail = new List<Location>();
            Powers = new List<DraculaPower>();
            PossibleMoves = new List<Location>();
            PossibleDoubleBackMoves = new List<Location>();
            PossiblePowers = new List<DraculaPower>();
            EncountersToMature = new List<Encounter>();
            EncounterHand = new List<Encounter>();
            LocationWhereHideWasUsed = null;
            Catacombs = new Location[3];
            EventCardsInHand = new List<Event>();
            Powers.Add(new DraculaPower("Dark Call", true));
            Powers.Add(new DraculaPower("Double Back", true));
            Powers.Add(new DraculaPower("Feed", false));
            Powers.Add(new DraculaPower("Hide", true));
            Powers.Add(new DraculaPower("Wolf Form", false));
            Blood = 15;
            LostBloodAtSeaOnLatestTurn = false;
            EncounterHandSize = 5;
            EventHandSize = 4;
            logic = new DecisionMaker();
            AdvanceMovePower = null;
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
            if (PossibleMoves.Count() + PossiblePowers.Count() == 0)
            {
                Logger.WriteToDebugLog("Dracula has no legal moves");
                ui.TellUser("Dracula is cornered by his own trail");
                return false;
            }
            else if (PossibleMoves.Count() == 0 && PossiblePowers.Count() == 1 && PossiblePowers.Contains(Powers[1]))
            {
                Logger.WriteToDebugLog("Dracula has no regular moves available");
                DeterminePossibleWolfFormLocations();
                if (PossibleMoves.Count() == 0)
                {
                    Logger.WriteToDebugLog("Dracula has no moves available by Wolf Form");
                    ui.TellUser("Dracula is cornered by his own trail");
                    return false;
                }
                DeterminePossibleLocations();
            }


            string powerUsed;
            LocationDetail destination = logic.DecideMove(g, this, out powerUsed);

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
                        LocationWhereHideWasUsed = CurrentLocation;
                    }
                    if (powerUsed == "Dark Call")
                    {
                        Blood -= 2;
                    }
                    if (powerUsed == "Feed" && Blood < 15)
                    {
                        Blood++;
                    }
                }
                else if (powerUsed == "Double Back")
                {
                    DoDoubleBackMove(g, destination, ui);
                }
                else if (powerUsed == "Wolf Form")
                {
                    DoWolfFormMove(g, destination, ui);
                    Blood--;
                }

                // put the power used at the head of the trail
                Logger.WriteToDebugLog("Putting power " + powerUsed + " at the head of the trail");
                PossiblePowers[PossiblePowers.FindIndex(power => power.name == powerUsed)].positionInTrail = 0;
            }
            else
            // performing a regular move
            {
                // choose a location
                MoveByRoadOrSea(g, destination, ui);
                Logger.WriteToGameLog("Dracula moved to " + CurrentLocation.Name);
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
                ui.TellUser("Dracula drew " + eventCardDrawn.name);
                PlayAlly(g, eventCardDrawn, ui);
            }
            else if (eventCardDrawn.type == EventType.PlayImmediately)
            {
                ui.TellUser("Dracula drew " + eventCardDrawn.name);
                PlayImmediately(g, eventCardDrawn, ui);
                g.RemoveEventFromEventDeck(eventCardDrawn);
            }
            else
            {
                EventCardsInHand.Add(eventCardDrawn);
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
                        g.DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluckC, ui);
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
                        g.DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
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
                        g.DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluckB, ui);
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
            Logger.WriteToDebugLog("Playing Vampiric Influence on " + hunterToInfluence.Name);
            ui.TellUser(hunterToInfluence.Name + " was affected by Dracula's Vampiric Influence and must reveal all cards and tell Dracula their next move");
            Logger.WriteToDebugLog("Resetting everything known about " + hunterToInfluence.Name + "'s cards");
            foreach (Item item in hunterToInfluence.ItemsKnownToDracula)
            {
                g.MoveItemFromKnownItemsToItemDeck(hunterToInfluence, item);
            }
            foreach (Event e in hunterToInfluence.EventsKnownToDracula)
            {
                g.MoveEventFromKnownEventsToEventDeck(hunterToInfluence, e);
            }
            for (int i = 0; i < hunterToInfluence.NumberOfItems; i++)
            {
                string line;
                do
                {
                    line = ui.AskHunterToRevealItemByVampiricInfluence(hunterToInfluence.Name);
                    ui.TellUser(g.GetItemByNameFromItemDeck(line).name);
                } while (g.GetItemByNameFromItemDeck(line).name == "Unknown item");
                g.MoveItemFromItemDeckToKnownItems(hunterToInfluence, g.GetItemByNameFromItemDeck(line));
            }
            for (int i = 0; i < hunterToInfluence.NumberOfEvents; i++)
            {
                string line;
                do
                {
                    line = ui.AskHunterToRevealEventByVampiricInfluence(hunterToInfluence.Name);
                    ui.TellUser(g.GetEventByNameFromEventDeck(line).name);
                } while (g.GetEventByNameFromEventDeck(line).name == "Unknown event");
                g.MoveEventFromEventDeckToKnownEvents(hunterToInfluence, g.GetEventByNameFromEventDeck(line));
            }
            string lineA;
            do
            {
                lineA = ui.AskHunterWhichLocationTheyAreMovingToNextTurn(hunterToInfluence.Name);
                ui.TellUser(g.GetLocationFromName(lineA).Name);
            } while (g.GetLocationFromName(lineA).Name == "Unknown location" || g.GetLocationFromName(lineA).Name == "Multiple locations");
            hunterToInfluence.Destination = g.GetLocationFromName(lineA);
            ui.TellUser(hunterToInfluence.Destination.Name);
            hunterToInfluence.TravelType = ui.AskHunterWhatTravelTypeForSpy(hunterToInfluence.Name);
            ui.TellUser(hunterToInfluence.TravelType);
            Logger.WriteToDebugLog(hunterToInfluence.Name + "'s next move recorded as " + hunterToInfluence.Destination.Name + " by " + hunterToInfluence.TravelType);
        }

        private void PlayNightVisit(GameState g, UserInterface ui)
        {
            
            Hunter hunterToVisit = logic.DecideWhoToNightVisit(g);
            Logger.WriteToDebugLog("Playing Night Visit on " + hunterToVisit.Name);
            ui.TellUser(hunterToVisit.Name + " was visited by Dracula in the night and loses 2 health");
            hunterToVisit.Health -= 2;
            g.HandlePossibleHunterDeath(ui);
        }

        private void PlayEvasion(GameState g, UserInterface ui)
        {
            LocationDetail locationToMoveTo = logic.DecideWhereToEvadeTo(g);
            Logger.WriteToDebugLog("Moving to " + locationToMoveTo.Name + " with Evasion");
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
                if (allyToKeep == allyDrawn.name)
                {
                    g.RemoveDraculaAlly();
                    g.SetDraculaAlly(g.GetEventByNameFromEventDeck(allyToKeep));
                }
                else
                {
                    g.DiscardEventCard(allyDrawn.name);
                }
                switch (allyDiscarded)
                {
                    case "Immanuel Hildesheim":
                        {
                            Logger.WriteToDebugLog("Discarded Immanuel Hildesheim, discarding events down to 4");
                            EventHandSize = 4;
                            DiscardEventsDownTo(g, EventHandSize, ui);
                            break;
                        }
                    case "Dracula's Brides":
                        {
                            Logger.WriteToDebugLog("Discarding Dracula's Brides, discarding encounters down to 5");
                            EncounterHandSize = 5;
                            DiscardEncountersDownTo(g, EncounterHandSize);
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
                        EncounterHandSize = 7;
                        break;
                    }
                case "Immanuel Hildesheim":
                    {
                        Logger.WriteToDebugLog("Immanueal Hildesheim is in play, event hand size is 6");
                        EventHandSize = 6;
                        break;
                    }
            }

        }

        public void DiscardEncountersDownTo(GameState g, int encountersToKeep)
        {
            while (EncounterHand.Count() > encountersToKeep)
            {
                Encounter encounterToDiscard = logic.DecideWhichEncounterToDiscard(g, this);
                EncounterHand.Remove(encounterToDiscard);
                g.AddEncounterToEncounterPool(encounterToDiscard);
                Logger.WriteToDebugLog("Dracula discarded " + encounterToDiscard.name);
                Logger.WriteToGameLog("Dracula discarded " + encounterToDiscard.name);
            }

        }

        public void DiscardEventsDownTo(GameState g, int numberOfCards, UserInterface ui)
        {
            while (EventCardsInHand.Count() > numberOfCards)
            {
                Event cardToDiscard = logic.DecideWhichEventToDiscard(g, this);
                EventCardsInHand.Remove(cardToDiscard);
                g.AddEventToEventDiscard(cardToDiscard);
                Logger.WriteToDebugLog("Dracula discarded " + cardToDiscard.name);
                Logger.WriteToGameLog("Dracula discarded " + cardToDiscard.name);
                ui.TellUser("Dracula discarded " + cardToDiscard.name);
            }
        }

        internal void OrderEncounters(Hunter hunter, LocationDetail location)
        {
            logic.DecideOrderOfEncountersAtLocation(hunter, location);
        }

        public void DeterminePossibleLocations()
        {
            Logger.WriteToDebugLog("Dracula is determining possible locations to move to");
            Logger.WriteToDebugLog("Clearing the old list of possible locations");
            PossibleMoves.Clear();
            Logger.WriteToDebugLog("Clearing the old list of possible locations that can be moved to with Double Back");
            PossibleDoubleBackMoves.Clear();
            for (int i = 0; i < CurrentLocation.ByRoad.Count(); i++)
            {
                if (CurrentLocation.ByRoad[i].type != LocationType.Hospital)
                {
                    Logger.WriteToDebugLog("Adding location " + CurrentLocation.ByRoad[i].name);
                    PossibleMoves.Add(CurrentLocation.ByRoad[i]);
                }
                else
                {
                    Logger.WriteToDebugLog("Not adding location " + CurrentLocation.ByRoad[i].name);
                }

            }

            for (int i = 0; i < CurrentLocation.BySea.Count(); i++)
            {
                if (CurrentLocation.BySea[i].turnsUntilStormSubsides < 1)
                {
                    Logger.WriteToDebugLog("Adding location " + CurrentLocation.BySea[i].name);
                    PossibleMoves.Add(CurrentLocation.BySea[i]);
                }
            }

            Logger.WriteToDebugLog("Checking trail locations against possible moves");
            for (int i = 0; i < Trail.Count(); i++)
            {
                if (Trail[i].Name != "Dark Call" && Trail[i].Name != "Hide" && Trail[i].Name != "Feed")
                {
                    if (PossibleMoves.Contains(Trail[i]))
                    {
                        Logger.WriteToDebugLog("Moving location " + Trail[i].Name + " from possible moves to Double Back locations because it is in Dracula's trail");
                        PossibleDoubleBackMoves.Add(Trail[i]);
                        PossibleMoves.Remove(Trail[i]);
                    }
                }
            }
            Logger.WriteToDebugLog("Checking Catacombs locations against possible moves");
            for (int i = 0; i < 3; i++)
            {
                if (Catacombs[i] != null && PossibleMoves.Contains(Catacombs[i]))
                {
                    Logger.WriteToDebugLog("Moving " + Catacombs[i].Name + " from possible moves to possible Double Back moves because it is in the Catacombs");
                    PossibleDoubleBackMoves.Add(Catacombs[i]);
                    PossibleMoves.Remove(Catacombs[i]);
                }
            }
            Logger.WriteToDebugLog("Checking possible move locations for Heavenly Hosts and Consecrated Ground");
            List<LocationDetail> movesToRemove = new List<LocationDetail>();
            foreach (LocationDetail loc in PossibleMoves)
            {
                if (loc.HasHost || loc.IsConsecrated)
                {
                    movesToRemove.Add(loc);
                }
            }
            foreach (LocationDetail mov in movesToRemove)
            {
                PossibleMoves.Remove(mov);
            }
            Logger.WriteToDebugLog("Checking possible double back locations for Heavenly Hosts and Consecrated Ground");
            movesToRemove.Clear();
            foreach (LocationDetail loc in PossibleDoubleBackMoves)
            {
                if (loc.HasHost || loc.IsConsecrated)
                {
                    movesToRemove.Add(loc);
                }
            }
            foreach (LocationDetail mov in movesToRemove)
            {
                PossibleDoubleBackMoves.Remove(mov);
            }
        }

        public void DeterminePossibleWolfFormLocations()
        {
            Logger.WriteToDebugLog("Dracula is determining which locations can be moved to using Wolf Form, starting with locations connected by one road");
            Logger.WriteToDebugLog("Clearing the old list of possible locations");
            PossibleMoves.Clear();
            if (CurrentLocation.Type == LocationType.Sea)
            {
                return;
            }
            for (int i = 0; i < CurrentLocation.ByRoad.Count(); i++)
            {
                if (CurrentLocation.ByRoad[i].type != LocationType.Hospital && !CurrentLocation.ByRoad[i].hasHost && !CurrentLocation.ByRoad[i].isConsecrated)
                {
                    Logger.WriteToDebugLog("Adding location " + CurrentLocation.ByRoad[i].name);
                    PossibleMoves.Add(CurrentLocation.ByRoad[i]);
                }
                else
                {
                    Logger.WriteToDebugLog("Not adding location " + CurrentLocation.ByRoad[i].name);
                }
            }

            // extend every remaining location by road
            Logger.WriteToDebugLog("Dracula is now adding locations that are connected by two roads");
            List<LocationDetail> extendedLocations = new List<LocationDetail>();
            for (int i = 0; i < PossibleMoves.Count(); i++)
            {
                for (int j = 0; j < PossibleMoves[i].ByRoad.Count(); j++)
                {
                    if (!PossibleMoves.Contains(PossibleMoves[i].ByRoad[j]) && !extendedLocations.Contains(PossibleMoves[i].ByRoad[j]) && PossibleMoves[i].ByRoad[j].type != LocationType.Hospital && !PossibleMoves[i].ByRoad[j].hasHost && !PossibleMoves[i].ByRoad[j].isConsecrated)
                    {
                        Logger.WriteToDebugLog("Adding location " + PossibleMoves[i].ByRoad[j].name);
                        extendedLocations.Add(PossibleMoves[i].ByRoad[j]);
                    }
                }

            }
            PossibleMoves.AddRange(extendedLocations);
            Logger.WriteToDebugLog("Removing current location, " + CurrentLocation.Name + " from the list of possible moves");
            PossibleMoves.Remove(CurrentLocation);
            for (int i = 0; i < Trail.Count(); i++)
            {
                Logger.WriteToDebugLog("Removing " + Trail[i].Name + " from the list, as it is in Dracula's trail");
                PossibleMoves.Remove(Trail[i]);
            }

        }

        public void DeterminePossiblePowers(int timeOfDay)
        {
            Logger.WriteToDebugLog("Determining possible powers to use");
            Logger.WriteToDebugLog("Clearing the old list of possible powers to use");
            PossiblePowers.Clear();
            if (CurrentLocation.Type == LocationType.Sea)
            {
                Logger.WriteToDebugLog("Dracula cannot use any powers at sea");
                return;
            }
            for (int i = 0; i < Powers.Count(); i++)
            {
                if (Powers[i].positionInTrail > 5 && (timeOfDay > 2 || Powers[i].canBeUsedDuringDaylight))
                {
                    if (Powers[i].name != "Double Back" || PossibleDoubleBackMoves.Count() > 0)
                    {
                        if (Powers[i].name != "Hide" || (CurrentLocation.Type != LocationType.Sea && CurrentLocation.Type != LocationType.Castle))
                        {
                            if (Powers[i].name == "Wolf Form")
                            {
                                // Wolf Form
                                DeterminePossibleWolfFormLocations();
                                if (PossibleMoves.Count() > 0 && Blood > 1)
                                {
                                    Logger.WriteToDebugLog("Adding Wolf Form to the list");
                                    PossiblePowers.Add(Powers[i]);
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
                                if (CurrentLocation.IsConsecrated || CurrentLocation.HasHost)
                                {
                                    Logger.WriteToDebugLog("Not adding power " + Powers[i].name + " to the list as it would cause Dracula to stay in the same location as a Host/Consecrated Ground");
                                }
                                else
                                {
                                    if (Powers[i].name != "Dark Call" || Blood > 2)
                                    {
                                        Logger.WriteToDebugLog("Adding power " + Powers[i].name + " to the list");
                                        PossiblePowers.Add(Powers[i]);
                                    }
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

        public void MoveByRoadOrSea(GameState g, LocationDetail goingTo, UserInterface ui)
        {
            Logger.WriteToDebugLog("Moving Dracula to a new location");
            Logger.WriteToDebugLog("Remembering that Dracula is moving from a location of type " + CurrentLocation.Type);
            LocationType previousLocationType = CurrentLocation.Type;
            if (PossibleMoves.Count() == 0)
            {
                Logger.WriteToDebugLog("SOMEHOW DRACULA IS TRYING TO MOVE WHEN HE HAS NO POSSIBLE MOVES");
            }
            try
            {
                Logger.WriteToDebugLog("Dracula is moving from " + CurrentLocation.Name + " to " + goingTo.Name);
                CurrentLocation = goingTo;
            }
            catch (ArgumentOutOfRangeException)
            {
                ui.TellUser("Dracula tried to do something illegal");
                Logger.WriteToDebugLog("DRACULA TRIED TO DO SOMETHING ILLEGAL");
            }

            Logger.WriteToDebugLog("Adding " + CurrentLocation.Name + " to the head of the trail");
            Trail.Insert(0, CurrentLocation);
            MovePowersAlongTrail();
            CheckBloodLossAtSea(previousLocationType, g.NameOfHunterAlly());
            Logger.WriteToDebugLog("Checking if Dracula is in Castle Dracula");
            if (CurrentLocation.Type == LocationType.Castle)
            {
                Logger.WriteToDebugLog("Revealing Castle Dracula");
                CurrentLocation.IsRevealed = true;
                if (LocationWhereHideWasUsed != null)
                {
                    if (LocationWhereHideWasUsed.Type == LocationType.Castle)
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

            while (Trail.Count() > length)
            {
                for (int i = 0; i < Powers.Count(); i++)
                {
                    if (Powers[i].positionInTrail > length - 1)
                    {
                        Powers[i].positionInTrail = 6;
                    }
                }
                if (Trail.Last().Name == "Hide")
                {
                    LocationWhereHideWasUsed = null;
                }
                Trail.Last().IsRevealed = false;
                if (Trail.Last() != CurrentLocation)
                {
                    while (Trail.Last().Encounters.Count() > 0)
                    {
                        Logger.WriteToDebugLog("Moving encounter " + Trail.Last().Encounters[0].name + " back to the encounter pool");
                        g.AddEncounterToEncounterPool(Trail.Last().Encounters[0]);
                        Trail.Last().Encounters[0].isRevealed = false;
                        Trail.Last().Encounters.Remove(Trail.Last().Encounters[0]);
                    }
                }
                else
                {
                    Logger.WriteToDebugLog("Not moving this location's encounters back to the encounter pool because it is Dracula's current location");
                }
                Logger.WriteToDebugLog("Removing " + Trail.Last().Name + " from the trail");
                Trail.Remove(Trail.Last());
            }
            if (!Trail.Contains(CurrentLocation))
            {
                Logger.WriteToDebugLog("Dracula's current location was removed from the trail, adding it back at what will be the end of the trail");
                Trail.Insert(length - 1, CurrentLocation);
                Logger.WriteToDebugLog("Removing " + Trail.Last().Name);
                Trail.Remove(Trail.Last());
            }
        }

        public void MovePowersAlongTrail()
        {
            for (int i = 0; i < Powers.Count(); i++)
            {
                Logger.WriteToDebugLog("Moving power " + Powers[i].name + " from position " + Powers[i].positionInTrail + " to " + (Powers[i].positionInTrail + 1));
                Powers[i].positionInTrail++;
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
            Trail[hideIndex].Encounters.Clear();


            // remove Hide from the trail
            Logger.WriteToDebugLog("Removing Hide from the trail");
            Trail.Remove(Trail[hideIndex]);
            LocationWhereHideWasUsed = null;
            // move all power cards beyond that point up one position
            for (int i = 0; i < Powers.Count(); i++)
            {
                if (Powers[i].name == "Hide")
                {
                    Logger.WriteToDebugLog("Putting the Hide power into position 6 (out of the trail)");
                    Powers[i].positionInTrail = 6;
                }
                else if (Powers[i].positionInTrail > hideIndex && Powers[i].positionInTrail < 6)
                {
                    Logger.WriteToDebugLog("Moving power " + Powers[i].name + " further up the trail");
                    Powers[i].positionInTrail--;
                }
            }

            ui.TellUser("Dracula was hiding");
        }

        public void RevealHide(UserInterface ui)
        {
            RevealHide(Trail.FindIndex(location => location.Name == "Hide"), ui);
        }

        public void CheckBloodLossAtSea(LocationType locationType, string hunterAllyName)
        {
            Logger.WriteToDebugLog("Checking if Dracula should lose blood due to moving at sea");
            if (CurrentLocation.Type == LocationType.Sea && locationType != LocationType.Sea)
            {
                Logger.WriteToDebugLog("Dracula has embarked this turn, losing one blood");
                Blood--;
                LostBloodAtSeaOnLatestTurn = true;
            }
            else if (CurrentLocation.Type == LocationType.Sea && hunterAllyName == "Rufus Smith")
            {
                Logger.WriteToDebugLog("Dracula is at sea and Rufus Smith is in play, losing one blood");
                Blood--;
                LostBloodAtSeaOnLatestTurn = true;
            }
            else if (CurrentLocation.Type == LocationType.Sea && !LostBloodAtSeaOnLatestTurn)
            {
                Logger.WriteToDebugLog("Dracula is at sea and did not lose blood last turn, losing one blood");
                Blood--;
                LostBloodAtSeaOnLatestTurn = true;
            }
            else if (CurrentLocation.Type == LocationType.Sea)
            {
                Logger.WriteToDebugLog("Dracula is at sea but lost blood last turn");
                LostBloodAtSeaOnLatestTurn = false;
            }
            else
            {
                Logger.WriteToDebugLog("Dracula is not at sea");
                LostBloodAtSeaOnLatestTurn = false;
            }
        }

        public void AddDummyPowerCardToTrail(string powerName)
        {
            // add the power card to the location trail by using a dummy "location"
            LocationDetail powerCard = new LocationDetail();
            powerCard.Name = powerName;
            powerCard.Abbreviation = powerName.Substring(0, 3).ToUpper();
            if (powerCard.Name != "Hide")
            {
                Logger.WriteToDebugLog("Power card is not Hide, so it is added to the trail revealed");
                powerCard.IsRevealed = true;
                powerCard.Type = LocationType.Power;
            }
            Logger.WriteToDebugLog("Adding the power card " + powerCard.Name + " to the head of trail");
            Trail.Insert(0, powerCard);
            MovePowersAlongTrail();
        }

        public void DoDoubleBackMove(GameState g, LocationDetail goingTo, UserInterface ui)
        {
            Logger.WriteToDebugLog("Remembering that Dracula is moving from a location of type " + CurrentLocation.Type);
            LocationType previousLocationType = CurrentLocation.Type;

            bool doublingBackToCatacombs = false;

            int doubleBackIndex = Trail.FindIndex(location => location == goingTo);
            if (doubleBackIndex > -1)
            {
                Logger.WriteToDebugLog("Doubling back to a location in the trail");
                if (doubleBackIndex > 0)
                {
                    if (Trail[doubleBackIndex - 1].Name == "Hide")
                    {
                        Logger.WriteToDebugLog("Dracula Doubled Back to " + goingTo + " and Hide is the next card in the trail");
                        ui.TellUser("Dracula Doubled Back to a location where he previously used Hide (position " + (doubleBackIndex - 1) + ")");
                        RevealHide(doubleBackIndex - 1, ui);
                    }
                    else if (doubleBackIndex > 1)
                    {
                        if (Trail[doubleBackIndex - 1].Type == LocationType.Power && Trail[doubleBackIndex - 2].Name == "Hide")
                        {
                            Logger.WriteToDebugLog("Dracula Doubled Back to " + goingTo + " and Hide is the next card in the trail, after " + Trail[doubleBackIndex - 1].Name);
                            ui.TellUser("Dracula Doubled Back to a location where he previously used Hide (position " + (doubleBackIndex - 1) + ")");
                            RevealHide(doubleBackIndex - 2, ui);
                        }
                        else if (doubleBackIndex > 2)
                        {
                            if (Trail[doubleBackIndex - 1].Type == LocationType.Power && Trail[doubleBackIndex - 2].Type == LocationType.Power && Trail[doubleBackIndex - 3].Name == "Hide")
                            {
                                Logger.WriteToDebugLog("Dracula Doubled Back to " + goingTo + " and Hide is the next card in the trail, after " + Trail[doubleBackIndex - 1].Name + " and " + Trail[doubleBackIndex - 2].Name);
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

            int doubleBackLocation = Trail.FindIndex(loc => loc == goingTo);
            // move location to the front of the trail
            Logger.WriteToDebugLog("Temporarily holding " + goingTo.Name);
            if (doublingBackToCatacombs)
            {
                Logger.WriteToDebugLog("Removing " + goingTo.Name + " from the Catacombs");
                Catacombs[Array.IndexOf(Catacombs, goingTo)] = null;
                Encounter encounterToDiscard = logic.DecideWhichCatacombsEncounterToDiscard(g, goingTo, ui);
                goingTo.Encounters.Remove(encounterToDiscard);
                g.AddEncounterToEncounterPool(encounterToDiscard);
            }
            else
            {
                Logger.WriteToDebugLog("Removing " + goingTo.Name + " from the trail");
                Trail.Remove(goingTo);
            }
            Logger.WriteToDebugLog("Putting " + goingTo.Name + " back at the head of the trail");
            Trail.Insert(0, goingTo);
            Logger.WriteToDebugLog("Dracula's current location is now " + CurrentLocation.Name);
            CurrentLocation = Trail[0];

            Logger.WriteToGameLog("Dracula Doubled Back to " + CurrentLocation.Name + " from " + (doublingBackToCatacombs ? " the Catacombs" : " his trail"));

            // move the power cards that are in the trail
            for (int i = 0; i < Powers.Count(); i++)
            {
                if (Powers[i].positionInTrail == doubleBackLocation)
                {
                    Logger.WriteToDebugLog("Moving Double Back power position to the head of the trail");
                    Powers[i].positionInTrail = 0;
                }
                else if (Powers[i].positionInTrail < doubleBackLocation)
                {
                    Logger.WriteToDebugLog("Moving power " + Powers[i].name + " position further along the trail");
                    Powers[i].positionInTrail++;
                }
            }
            CheckBloodLossAtSea(previousLocationType, g.NameOfHunterAlly());
        }

        public void DoWolfFormMove(GameState g, LocationDetail destination, UserInterface ui)
        {
            // determine possible locations for wolf form (road only, up to two moves away)
            DeterminePossibleWolfFormLocations();
            // carry out move
            MoveByRoadOrSea(g, destination, ui);
            Logger.WriteToGameLog("Dracula used Wolf Form to move to " + CurrentLocation.Name);
        }

        public void DrawEncounters(GameState g, int totalNumberOfEncounters)
        {
            Logger.WriteToDebugLog("DRAWING ENCOUNTERS");
            Logger.WriteToDebugLog("Dracula is drawing up to " + totalNumberOfEncounters + " encounters");
            while (EncounterHand.Count() < totalNumberOfEncounters)
            {
                Encounter tempEncounter = g.DrawEncounterFromEncounterPool();
                Logger.WriteToDebugLog("Dracula drew encounter " + tempEncounter.name);
                Logger.WriteToGameLog("Dracula drew encounter " + tempEncounter.name);
                EncounterHand.Add(tempEncounter);
                g.RemoveEncounterFromEncounterPool(tempEncounter);
            }
        }

        public void PlaceEncounterIfLegal(GameState g, LocationDetail location)
        {
            if (location.Type == LocationType.Castle)
            {
                Logger.WriteToDebugLog("Not placing an encounter here as this is Castle Dracula");
                return;
            }
            if (location.Type == LocationType.Sea)
            {
                Logger.WriteToDebugLog("Not placing an encounter here as it is a sea location");
                return;
            }
            Logger.WriteToDebugLog("Checking if Dracula used a power at this location");
            int locationIndex = Trail.FindIndex(loc => loc == location);
            if (locationIndex > -1)
            {
                Logger.WriteToDebugLog("This location is in the trail, checking if a power was used here");
                if (Trail[locationIndex].Name == "Dark Call")
                {
                    Logger.WriteToDebugLog("Not placing an encounter here as Dracula used Dark Call");
                    return;
                }
                if (Trail[locationIndex].Name == "Feed")
                {
                    Logger.WriteToDebugLog("Not placing an encounter here as Dracula used Feed");
                    return;
                }
                for (int i = 0; i < Powers.Count(); i++)
                {
                    if (Powers[i].name == "Double Back" && Powers[i].positionInTrail == locationIndex)
                    {
                        Logger.WriteToDebugLog("Not placing an encounter here as Dracula used Double Back");
                        return;
                    }
                }
                Logger.WriteToDebugLog("Checking if this location already has an encounter");
                if (Trail[locationIndex].Encounters.Count() > 0)
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
            location.Encounters.Add(chosenEncounter);
            EncounterHand.Remove(chosenEncounter);
        }

        internal Event WillPlayDevilishPower(GameState g, UserInterface ui)
        {
            Event eventToReturn = logic.DecideWhetherToPlayDevilishPower(g, this);
            if (eventToReturn != null)
            {
                int hunterIndex = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                if (hunterIndex > 0)
                {
                    g.DiscardEventFromHunterAtIndex("Good Luck", hunterIndex, ui);
                    EventCardsInHand.Remove(EventCardsInHand.Find(c => c.name == "Devilish Power"));
                    eventToReturn = null;
                }
            }
            return eventToReturn;
        }

        public void HandleDroppedOffLocations(GameState g, UserInterface ui)
        {
            Logger.WriteToDebugLog("DEALING WITH DROPPED OFF LOCATIONS");
            while (Trail.Count > 6)
            {
                if (Trail.Last() == LocationWhereHideWasUsed)
                {
                    Logger.WriteToDebugLog("Hide location has left the trail, revealing the Hide card");
                    RevealHide(Trail.FindIndex(location => location.Name == "Hide"), ui);
                }
                Logger.WriteToDebugLog("Deciding what to do with " + Trail.Last().Name);
                if (Trail.Last().Type == LocationType.Power)
                {
                    Logger.WriteToDebugLog(Trail.Last().Name + " is a power card, so it gets discarded");
                    TrimTrail(g, Trail.Count() - 1);
                    return;
                }
                if (Trail.Last().Type == LocationType.Sea)
                {
                    Logger.WriteToDebugLog(Trail.Last().Name + " is a sea location, so it gets discarded");
                    TrimTrail(g, Trail.Count() - 1);
                    return;
                }
                int emptyIndex = Array.IndexOf(Catacombs, null);
                if (emptyIndex > -1)
                {
                    Logger.WriteToDebugLog("There is a space in the Catacombs");
                    if (logic.DecideToPutLocationInCatacombs(g, this))
                    {
                        Logger.WriteToDebugLog("Putting " + Trail.Last().Name + " into the Catacombs");
                        Catacombs[emptyIndex] = Trail.Last();
                        Trail.Remove(Trail.Last());
                        PlaceEncounterIfLegal(g, Catacombs[emptyIndex]);
                        Logger.WriteToGameLog("Dracula put " + Catacombs[emptyIndex].Name + " into the Catacombs with encounters");
                    }
                    else
                    {
                        Logger.WriteToDebugLog("Discarding " + Trail.Last().Name);
                        while (Trail.Last().Encounters.Count() > 0)
                        {
                            Logger.WriteToDebugLog("Hanging on to " + Trail.Last().Encounters.First().name + " to mature it later");
                            EncountersToMature.Add(Trail.Last().Encounters.First());
                            Trail.Last().Encounters.Remove(Trail.Last().Encounters.First());
                        }
                        Logger.WriteToDebugLog("Discarding " + Trail.Last().Name);
                        TrimTrail(g, Trail.Count() - 1);
                    }
                }
                else
                {
                    Logger.WriteToDebugLog("There is no space in the Catacombs, discarding " + Trail.Last().Name);
                    TrimTrail(g, Trail.Count() - 1);
                }
            }
        }

        public void TakeStartOfTurnActions(GameState g, UserInterface ui)
        {
            if (g.NameOfDraculaAlly() == "Quincey P. Morris")
            {
                Hunter victim = logic.DecideWhichHunterToAttackWithQuincey(g.GetHunters());
                ui.TellUser("Dracula has chosen " + victim.Name + " to affect with Quincey P. Morris");
                switch (ui.GetHunterHolyItems(victim.Name)) {
                    case 0:
                        ui.TellUser(victim.Name + " loses 1 health");
                        victim.Health--;
                        g.HandlePossibleHunterDeath(ui);
                        break;
                    case 1:
                        if (victim.ItemsKnownToDracula.FindIndex(item => item.name == "Crucifix") == -1)
                        {   
                            g.AddToHunterItemsKnownToDracula(victim, "Crucifix");
                        }
                        ui.TellUser("No effect from Quincey P. Morris");
                        break;
                    case 2:
                        if (victim.ItemsKnownToDracula.FindIndex(item => item.name == "Heavenly Host") == -1)
                        {
                            g.AddToHunterItemsKnownToDracula(victim, "Heavenly Host");
                        }
                        ui.TellUser("No effect from Quincey P. Morris");
                        break;
                }
            }
            Logger.WriteToDebugLog("Deciding what to do with Catacombs locations");
            for (int i = 0; i < 3; i++)
            {
                if (Catacombs[i] != null)
                {
                    Logger.WriteToDebugLog("Deciding what to do with location " + Catacombs[i].Name);
                    if (logic.DecideToDiscardCatacombLocation(g, this))
                    {
                        Logger.WriteToDebugLog("Discarding " + Catacombs[i].Name);
                        while (Catacombs[i].Encounters.Count() > 0)
                        {
                            Logger.WriteToDebugLog("Putting encounter " + Catacombs[i].Encounters.First().name + " back into the encounter pool");
                            g.AddEncounterToEncounterPool(Catacombs[i].Encounters.First());
                            Catacombs[i].Encounters.Remove(Catacombs[i].Encounters.First());
                        }
                        Logger.WriteToDebugLog("Emptying " + Catacombs[i].Name + " from Catacombs");
                        Catacombs[i] = null;
                    }
                }
            }
        }

        public void DoActionPhase(GameState g, UserInterface ui)
        {
            Logger.WriteToDebugLog("PERFORMING ACTION PHASE");
            if (g.IndexOfHunterAtLocation(CurrentLocation) > -1 && LocationWhereHideWasUsed != CurrentLocation && CurrentLocation.Type != LocationType.Sea)
            {
                ui.TellUser("Dracula moved into " + CurrentLocation.Name + " and is entering combat with " + g.NameOfHunterAtIndex(g.IndexOfHunterAtLocation(CurrentLocation)));
                g.RevealLocationAtTrailIndex(0, ui);
                ui.drawGameState(g);
                g.ResolveCombat(g.IndexOfHunterAtLocation(CurrentLocation), 1, false, ui);
            }
            else
            {
                Logger.WriteToDebugLog("Placing an encounter");
                PlaceEncounterIfLegal(g, Trail[0]);
            }
        }

        public void MatureEncounters(GameState g, UserInterface ui)
        {
            Logger.WriteToDebugLog("MATURING ENCOUNTERS");
            while (EncountersToMature.Count > 0)
            {
                Logger.WriteToDebugLog("Maturing encounter " + EncountersToMature.First().name);
                Logger.WriteToGameLog(EncountersToMature.First().name + " matured");
                g.MatureEncounter(EncountersToMature.First().name, ui);
                g.AddEncounterToEncounterPool(EncountersToMature.First());
                EncountersToMature.Remove(EncountersToMature.First());
            }
        }

        internal void DiscardHunterCard(GameState g, HunterHandler.Hunter hunter, UserInterface ui)
        {
            string cardToDiscard = logic.DecideToDiscardEventOrItem(g, this, hunter);
            if (cardToDiscard != "no cards")
            {
                ui.TellUser(hunter.Name + " must discard an " + cardToDiscard);
                ui.TellUser("Don't forget to tell me what was discarded");
            }
        }

        internal bool WillCancelTrain(GameState g, HunterHandler.Hunter hunter)
        {
            return logic.DecideToPlayFalseTipOff(g, this, hunter);
        }

        internal void DiscardEventFromHand(GameState g, string p)
        {
            g.DiscardEventCard(p);
            EventCardsInHand.Remove(GetEventCardFromHand(p));
        }

        private Event GetEventCardFromHand(string p)
        {
            return EventCardsInHand[EventCardsInHand.FindIndex(card => card.name == p)];
        }

        internal Item ChooseCombatCardAndTarget(List<Hunter> huntersFigting, List<Item> combatCards, CombatRoundResult result, string hunterAllyName, out string name)
        {
            name = logic.DecideHunterToAttack(huntersFigting, combatCards, result);
            return logic.DecideWhichCombatCardToPlay(huntersFigting, this, combatCards, hunterAllyName, result);
        }

        internal LocationDetail DecideWhereToSendHunterWithBats(GameState g, Hunter hunter, UserInterface ui)
        {
            List<LocationDetail> batsMoves = new List<LocationDetail>();
            foreach (LocationDetail loc in hunter.CurrentLocation.ByRoad)
            {
                batsMoves.Add(loc);
            }
            return logic.DecideWhereToSendHuntersWithBats(g, hunter, batsMoves);
        }

        internal void DoEscapeAsBatsMove(GameState g, UserInterface ui)
        {
            DeterminePossibleWolfFormLocations();
            foreach (LocationDetail loc in PossibleMoves)
            {
                if (g.NumberOfHuntersAtLocation(loc.Name) > 0)
                {
                    PossibleMoves.Remove(loc);
                }
            }
            PossiblePowers.Clear();
            string throwAway; // declaring this only so that it can be fed into the DecideMove function; no purpose for it after that
            LocationDetail locationToMoveTo = logic.DecideMove(g, this, out throwAway);
            MoveByRoadOrSea(g, locationToMoveTo, ui);
        }

        internal Event PlayControlStorms(GameState g)
        {
            return logic.DecideToPlayControlStorms(g, this);
        }

        internal Event PlayEventCardAtStartOfDraculaTurn(GameState g)
        {
            return logic.DecideEventCardToPlayAtStartOfDraculaTurn(g, this);
        }

        internal Event PlaySeductionCard(GameState g)
        {
            return logic.DecideToPlaySeductionDuringVampireEncounter(g, this);
        }

        internal Event PlayEventCardAtStartOfCombat(GameState g, bool trapPlayed, bool hunterMoved, int enemyType)
        {
            return logic.DecideToPlayCardAtStartOfCombat(g, this, trapPlayed, hunterMoved, enemyType);
        }

        internal LocationDetail DecideWhereToSendHunterWithControlStorms(int hunterIndex, List<LocationDetail> possiblePorts, GameState g)
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

        internal LocationDetail ChooseLocationToSendHuntersToWithWildHorses(GameState g, List<Hunter> hunters)
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

        internal LocationDetail DecideWhichPortToGoToAfterStormySeas(GameState g, LocationDetail locationStormed)
        {
            return logic.DecideWhichPortToGoToAfterStormySeas(g, locationStormed);
        }

        internal void PlayDevilishPowerToRemoveHeavenlyHostOrHunterAlly(GameState g, UserInterface ui)
        {
            List<LocationDetail> map = g.GetMap();
            if (map.FindIndex(l => l.HasHost == true) > -1)
            {
                if (g.HuntersHaveAlly())
                {
                }
                else
                {
                    LocationDetail locationToRemoveHost = logic.DecideWhichLocationToRemoveHeavenlyHostFrom(g);
                    Logger.WriteToDebugLog("Dracula played Devilish Power to discard a Heavenly Host from " + locationToRemoveHost.Name);
                    ui.TellUser("Dracula played Devilish Power to discard a Heavenly Host from " + locationToRemoveHost.Name);
                    int hunterIndex = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                    if (hunterIndex > 0)
                    {
                        g.DiscardEventFromHunterAtIndex("Good Luck", hunterIndex, ui);
                    }
                    else
                    {
                        locationToRemoveHost.HasHost = false;
                    }
                }

            }
            else
            {
                ui.TellUser("Dracula played Devilish Power to discard the Hunters' Ally from play");
                Logger.WriteToDebugLog("Dracula played Devilish Power to discard the Hunters' Ally from play");
                int hunterIndex = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                if (hunterIndex > 0)
                {
                    g.DiscardEventFromHunterAtIndex("Good Luck", hunterIndex, ui);
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
            return logic.DecideWhichEncounterToAmbushHunterWith(EncounterHand);
        }

        internal LocationDetail ChooseStartLocation(GameState g)
        {
            return logic.DecideDraculaStartLocation(g);
        }

        internal LocationDetail ChooseMoveForHypnosis(GameState g, out string powerUsed)
        {
            return logic.DecideMove(g, this, out powerUsed);
        }

        internal static bool LocationIsInTrail(Location location)
        {
            return Trail.Contains(location);
        }

        internal bool LocationIsInCatacombs(Location location)
        {
            return Catacombs.Contains(loation);
        }
    }

    [DataContract]
    public class DraculaPower
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public bool canBeUsedDuringDaylight { get; set; }
        [DataMember]
        public int positionInTrail { get; set; }

        public DraculaPower(string newName, bool daylightPower)
        {
            name = newName;
            canBeUsedDuringDaylight = daylightPower;
            positionInTrail = 6;
        }
    }

}
