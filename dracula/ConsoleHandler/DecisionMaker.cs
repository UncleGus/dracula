using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationHandler;
using DraculaHandler;
using EncounterHandler;
using EventHandler;
using HunterHandler;
using LogHandler;

namespace DraculaSimulator
{
    public class DecisionMaker
    {
        // done
        internal Location DecideDraculaStartLocation(GameState g)
        {
            Logger.WriteToDebugLog("Starting to decide where Dracula will start");
            Location startLocation;
            do
            {
                startLocation = g.LocationAtMapIndex(new Random().Next(0, g.MapSize()));
            } while (startLocation.type == LocationType.Hospital || startLocation.type == LocationType.Sea || startLocation.type == LocationType.Castle || g.IndexOfHunterAtLocation(startLocation) > -1);
            Logger.WriteToDebugLog("Returning" + startLocation);
            return startLocation;
        }

        // done
        internal Location DecideMove(GameState g, Dracula dracula, out string powerName)
        {
            Logger.WriteToDebugLog("Starting to decide what move to make");
            Location goingTo;
            if (dracula.advanceMovePower != null || dracula.advanceMoveDestination != null)
            {
                powerName = dracula.advanceMovePower;
                goingTo = dracula.advanceMoveDestination;
                dracula.advanceMovePower = null;
                dracula.advanceMoveDestination = null;
            }
            int chosenActionIndex = new Random().Next(0, dracula.possibleMoves.Count() + dracula.possiblePowers.Count());
            if (chosenActionIndex > dracula.possibleMoves.Count() - 1)
            {
                // choosing a power
                chosenActionIndex -= dracula.possibleMoves.Count();
                powerName = dracula.possiblePowers[chosenActionIndex].name;
                if (powerName == "Dark Call" || powerName == "Feed" || powerName == "Hide")
                {
                    goingTo = null;
                }
                else if (powerName == "Double Back")
                {
                    goingTo = dracula.possibleDoubleBackMoves[new Random().Next(0, dracula.possibleDoubleBackMoves.Count())];
                }
                else if (powerName == "Wolf Form")
                {
                    dracula.DeterminePossibleWolfFormLocations();
                    goingTo = dracula.possibleMoves[new Random().Next(0, dracula.possibleMoves.Count())];
                }
                else
                {
                    goingTo = new Location();
                    goingTo.name = "Unknown location";
                }
            }
            else
            {
                powerName = "no power";
                goingTo = dracula.possibleMoves[chosenActionIndex];
            }
            Logger.WriteToDebugLog("Returning " + powerName + " and " + goingTo.name);
            return goingTo;
        }

        // done
        internal string DecideWhichAllyToKeep(string firstAllyName, string secondAllyName)
        {
            Logger.WriteToDebugLog("Deciding which of two allies to keep");
            if (new Random().Next(0, 2) > 0)
            {
                Logger.WriteToDebugLog("Returning " + firstAllyName);
                return firstAllyName;
            }
            else
            {
                Logger.WriteToDebugLog("Returning " + secondAllyName);
                return secondAllyName;
            }
        }

        // done
        internal Encounter DecideWhichEncounterToDiscard(GameState g, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding which encounter to discard");
            Encounter encounterToDiscard = dracula.encounterHand[new Random().Next(0, dracula.encounterHand.Count())];
            Logger.WriteToDebugLog("Returning " + encounterToDiscard.name);
            return encounterToDiscard;
        }

        // done
        internal Event DecideWhichEventToDiscard(GameState g, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding which event to discard");
            Event eventToDiscard = dracula.eventCardsInHand[new Random().Next(0, dracula.eventCardsInHand.Count())];
            Logger.WriteToDebugLog("Returning " + eventToDiscard.name);
            return eventToDiscard;
        }

        // done
        internal Encounter DecideWhichEncounterToPlace(GameState g, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding which encounter to place");
            Encounter encounterToPlace = dracula.encounterHand[new Random().Next(0, dracula.encounterHand.Count())];
            Logger.WriteToDebugLog("Returning " + encounterToPlace.name);
            return encounterToPlace;
        }

        // done
        internal bool DecideToPutLocationInCatacombs(GameState g, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding whether to put a location in the catacombs");
            bool isPuttingLocationInCatacombs = new Random().Next(0, 6) > 4;
            Logger.WriteToDebugLog("Returning " + isPuttingLocationInCatacombs);
            return isPuttingLocationInCatacombs;
        }

        // done
        internal bool DecideToDiscardCatacombLocation(GameState g, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding whether to discard location from catacombs");
            bool isDiscardingCatacombLocation = new Random().Next(0, 5) > 3;
            Logger.WriteToDebugLog("Returning " + isDiscardingCatacombLocation);
            return isDiscardingCatacombLocation;
        }

        // done
        internal string DecideToDiscardEventOrItem(GameState g, Dracula dracula, Hunter hunter)
        {
            Logger.WriteToDebugLog("Deciding whether to discard hunter's event or item");
            if (hunter.numberOfEvents + hunter.numberOfItems > 0)
            {
                int cardToDiscard = new Random().Next(0, hunter.numberOfEvents + hunter.numberOfItems);
                if (cardToDiscard + 1 > hunter.numberOfEvents)
                {
                    Logger.WriteToDebugLog("Returning item");
                    return "item";
                }
                else
                {
                    Logger.WriteToDebugLog("Returning event");
                    return "event";
                }
            }
            Logger.WriteToDebugLog("Returning no cards");
            return "no cards";
        }

        // done
        internal bool DecideToPlayFalseTipOff(GameState g, Dracula dracula, Hunter hunter)
        {
            Logger.WriteToDebugLog("Deciding whether to play False Tip-Off");
            if (dracula.eventCardsInHand.FindIndex(card => card.name == "False Tip-Off") > -1)
            {
                if (new Random().Next(0, 2) > 0)
                {
                    Logger.WriteToDebugLog("Returning true");
                    return true;
                }
            }
            Logger.WriteToDebugLog("Returning false");
            return false;
        }

        // done
        internal Item DecideWhichCombatCardToPlay(List<Hunter> huntersFighting, Dracula dracula, List<Item> combatCards, string hunterAllyName, CombatRoundResult result)
        {
            Logger.WriteToDebugLog("Deciding which combat card to play");
            if (hunterAllyName == "Sister Agatha" && dracula.blood < 3)
            {
                if (combatCards.FindIndex(card => card.name == "Fangs") > -1)
                {
                    combatCards.Remove(combatCards.Find(card => card.name == "Fangs"));
                }
                if (combatCards.FindIndex(card => card.name == "Escape (Man)") > -1)
                {
                    combatCards.Remove(combatCards.Find(card => card.name == "Fangs"));
                }
                if (combatCards.FindIndex(card => card.name == "Escape (Bat)") > -1)
                {
                    combatCards.Remove(combatCards.Find(card => card.name == "Fangs"));
                }
                if (combatCards.FindIndex(card => card.name == "Escape (Mist)") > -1)
                {
                    combatCards.Remove(combatCards.Find(card => card.name == "Fangs"));
                }
            }
            int chosenCardIndex;
            do
            {
                chosenCardIndex = new Random().Next(0, combatCards.Count());
            } while (combatCards[chosenCardIndex].name == result.enemyCardUsed || (result.outcome == "Repel" && combatCards[chosenCardIndex].name != "Dodge" && combatCards[chosenCardIndex].name != "Escape (Man)" && combatCards[chosenCardIndex].name != "Esacpe (Bat)" && combatCards[chosenCardIndex].name != "Escape (Mist)"));
            Logger.WriteToDebugLog("Returning " + combatCards[chosenCardIndex].name);
            return combatCards[chosenCardIndex];
        }

        // done
        internal void DecideOrderOfEncountersAtLocation(Hunter hunter, Location location)
        {
            Logger.WriteToDebugLog("Deciding order of encounters at location");
            Logger.WriteToDebugLog("Nothing to perform or return");
            // do nothing for now
        }

        // done
        internal string DecideHunterToAttack(List<Hunter> huntersFighting, List<Item> combatCards, CombatRoundResult result)
        {
            Logger.WriteToDebugLog("Deciding which hunter to attack");
            string nameOfHunterToAttack = huntersFighting[new Random().Next(0, huntersFighting.Count())].name;
            Logger.WriteToDebugLog("Returning " + nameOfHunterToAttack);
            return nameOfHunterToAttack;
        }

        // done
        internal Encounter DecideWhichCatacombsEncounterToDiscard(GameState g, Location goingTo, UserInterface ui)
        {
            Logger.WriteToDebugLog("Deciding which encounter to discard off a location previously in the catacombs");
            if (goingTo.encounters.Count() > 1)
            {
                int random = new Random().Next(0, 2);
                ui.TellUser("Dracula discarded the encounter in slot " + random);
                Logger.WriteToDebugLog("Returning " + goingTo.encounters[random]);
                return goingTo.encounters[random];
            }
            else
            {
                Logger.WriteToDebugLog("Returning " + goingTo.encounters.First().name);
                return goingTo.encounters.First();
            }
        }

        // done
        internal Event DecideToPlayControlStorms(GameState g, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding whether to play Control Storms");
            if (dracula.eventCardsInHand.FindIndex(e => e.name == "Control Storms") > -1) {
                if (new Random().Next(0, 2) > 0)
                {
                    Logger.WriteToDebugLog("Returning Control Storms");
                    return dracula.eventCardsInHand.Find(e => e.name == "Control Storms");
                }
            }
            Logger.WriteToDebugLog("Returning null");
            return null;
        }

        // done
        internal Event DecideEventCardToPlayAtStartOfDraculaTurn(GameState g, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding what card to play at the start of Dracula's turn");
            List<Event> eventCardsThatCanBePlayed = new List<Event>();
            if (dracula.eventCardsInHand.FindIndex(ev => ev.name == "Time Runs Short") > -1) {
                eventCardsThatCanBePlayed.Add(dracula.eventCardsInHand.Find(ev => ev.name == "Time Runs Short"));
            }
            if (dracula.eventCardsInHand.FindIndex(ev => ev.name == "Unearthly Swiftness") > -1) {
                eventCardsThatCanBePlayed.Add(dracula.eventCardsInHand.Find(ev => ev.name == "Unearthly Swiftness"));
            }
            if (dracula.eventCardsInHand.FindIndex(ev => ev.name == "Roadblock") > -1) {
                eventCardsThatCanBePlayed.Add(dracula.eventCardsInHand.Find(ev => ev.name == "Roadblock"));
            }
            if (dracula.eventCardsInHand.FindIndex(ev => ev.name == "Devilish Power") > -1 && (g.HuntersHaveAlly() || g.HeavenlyHostIsInPlay())) {
                eventCardsThatCanBePlayed.Add(dracula.eventCardsInHand.Find(ev => ev.name == "Devilish Power"));
            }
            if (eventCardsThatCanBePlayed.Count() > 0 && new Random().Next(0, 3) > 1)
            {
                Event cardToPlay = eventCardsThatCanBePlayed[new Random().Next(0, eventCardsThatCanBePlayed.Count())];
                Logger.WriteToDebugLog("Returning " + cardToPlay.name);
            }
            Logger.WriteToDebugLog("Returning null");
            return null;
        }

        // done
        internal Event DecideToPlaySeductionDuringVampireEncounter(GameState g, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding to play Seduction");
            if (dracula.eventCardsInHand.FindIndex(e => e.name == "Seduction") > -1)
            {
                if (new Random().Next(0, 2) > 0)
                {
                    Logger.WriteToDebugLog("Returning Seduction");
                    return dracula.eventCardsInHand.Find(e => e.name == "Seduction");
                }
            }
            Logger.WriteToDebugLog("Returning null");
            return null;
        }

        // done
        internal Event DecideToPlayCardAtStartOfCombat(GameState g, Dracula dracula, bool trapPlayed, bool hunterMoved, int enemyType)
        {
            List<Event> eventCardsThatCanBePlayed = new List<Event>();
            if (dracula.eventCardsInHand.FindIndex(ev => ev.name == "Trap") > -1)
            {
                eventCardsThatCanBePlayed.Add(dracula.eventCardsInHand.Find(ev => ev.name == "Trap"));
            }
            if (dracula.eventCardsInHand.FindIndex(ev => ev.name == "Rage") > -1)
            {
                if (enemyType == 1)
                {
                    eventCardsThatCanBePlayed.Add(dracula.eventCardsInHand.Find(ev => ev.name == "Rage"));
                }
            }
            if (dracula.eventCardsInHand.FindIndex(ev => ev.name == "Wild Horses") > -1)
            {
                if (!hunterMoved && enemyType == 1)
                {
                    eventCardsThatCanBePlayed.Add(dracula.eventCardsInHand.Find(ev => ev.name == "Wild Horses"));
                }
            }
            if (eventCardsThatCanBePlayed.Count() > 0 && new Random().Next(0, 2) > 0)
            {
                return eventCardsThatCanBePlayed[new Random().Next(0, eventCardsThatCanBePlayed.Count())];
            }
            return null;
        }

        // done
        internal Location DecideLocationToSendHunterWithControlStorms(GameState g, int hunterIndex, List<Location> possiblePorts)
        {
            return possiblePorts[new Random().Next(0, possiblePorts.Count())];
        }

        // done
        internal Event DecideToPlayCustomsSearch(GameState g, Dracula dracula, Hunter hunter)
        {
            if (dracula.eventCardsInHand.FindIndex(e => e.name == "Customs Search") > -1)
            {
                if (new Random().Next(0, 2) > 0)
                {
                    return dracula.eventCardsInHand.Find(e => e.name == "Customs Search");
                }
            }
            return null;
        }

        // done
        internal Location DecideWhereToEvadeTo(GameState g)
        {
            List<Location> map = g.GetMap();
            Location locationToReturn;
            do {
            locationToReturn = map[new Random().Next(0, map.Count())];
            } while (g.LocationIsInTrail(locationToReturn) || g.LocationIsInCatacombs(locationToReturn));
            return locationToReturn;
        }

        // done
        internal Hunter DecideWhoToNightVisit(GameState g)
        {
            List <Hunter> bittenHunters = g.GetBittenHunters();
            return bittenHunters[new Random().Next(0, bittenHunters.Count())];
        }

        // done
        internal Hunter DecideWhoToInfluence(GameState g)
        {
            List<Hunter> bittenHunters = g.GetBittenHunters();
            return bittenHunters[new Random().Next(0, bittenHunters.Count())];
        }

        // done
        internal Hunter DecideWhichHunterToRage(List<Hunter> huntersInCombat)
        {
            return huntersInCombat[new Random().Next(0, huntersInCombat.Count())];
        }

        // done
        internal Item DecideWhichItemToDiscard(List<Item> items)
        {
            return items[new Random().Next(0, items.Count())];
        }

        // done
        internal Location DecideWhereToSendHuntersWithWildHorses(GameState g, List<Hunter> hunters)
        {
            return hunters.First().currentLocation.byRoad[new Random().Next(0, hunters.First().currentLocation.byRoad.Count())];
        }

        // done
        internal Event DecideWhetherToPlayRelentlessMinion(GameState g, List<Hunter> huntersEncountered, string enemyType, Dracula dracula)
        {
            if (dracula.eventCardsInHand.FindIndex(e => e.name == "Relentless Minion") > -1)
            {
                if (new Random().Next(0, 2) > 0)
                {
                    return dracula.eventCardsInHand.Find(e => e.name == "Relentless Minion");
                }
            }
            return null;
        }

        // done
        internal void DecideWhereToPutRoadblock(GameState g, Roadblock roadblockCounter)
        {
            List<Location> map = g.GetMap();
            Location firstLocationToBlock;
            do
            {
                firstLocationToBlock = map[new Random().Next(0, map.Count())];
            } while (firstLocationToBlock.type != LocationType.City && firstLocationToBlock.type != LocationType.Town);
            List<Location> secondLocationToBlockCandidates = new List<Location>();
            foreach (Location loc in firstLocationToBlock.byRoad)
            {
                secondLocationToBlockCandidates.Add(loc);
            }
            foreach (Location loc in firstLocationToBlock.byTrain)
            {
                secondLocationToBlockCandidates.Add(loc);
            }
            Location secondLocationToBlock = secondLocationToBlockCandidates[new Random().Next(0, secondLocationToBlockCandidates.Count())];
            if (firstLocationToBlock.byRoad.Contains(secondLocationToBlock))
            {
                if (firstLocationToBlock.byTrain.Contains(secondLocationToBlock))
                {
                    if (new Random().Next(0, 2) > 0)
                    {
                        roadblockCounter.connectionType = "road";
                    }
                    else
                    {
                        roadblockCounter.connectionType = "rail";
                    }
                }
                else
                {
                    roadblockCounter.connectionType = "road";
                }
            }
            else
            {
                roadblockCounter.connectionType = "rail";
            }
            roadblockCounter.firstLocation = firstLocationToBlock;
            roadblockCounter.secondLocation = secondLocationToBlock;
        }

        // done
        internal Event DecideWhetherToPlaySensationalistPress(GameState g, int trailIndex, Dracula dracula)
        {
            if (dracula.eventCardsInHand.FindIndex(e => e.name == "Sensationalist Press") > -1)
            {
                if (new Random().Next(0, 2) > 0)
                {
                    return dracula.eventCardsInHand.Find(e => e.name == "Sensationalist Press");
                }
            }
            return null;
        }

        // done
        internal Location DecideWhichPortToGoToAfterStormySeas(GameState g, Location locationStormed)
        {
            Location port;
            do
            {
                port = locationStormed.bySea[new Random().Next(0, locationStormed.bySea.Count())];
            } while (port.type != LocationType.City && port.type != LocationType.Town);
            return port;
        }

        // done
        internal Event DecideWhetherToCancelCharteredCarriage(GameState g, Dracula dracula)
        {
            List<Event> eventCardsThatCanBePlayed = new List<Event>();
            if (dracula.eventCardsInHand.FindIndex(ev => ev.name == "False Tip-off") > -1)
            {
                eventCardsThatCanBePlayed.Add(dracula.eventCardsInHand.Find(ev => ev.name == "False Tip-off"));
            }
            if (dracula.eventCardsInHand.FindIndex(ev => ev.name == "Devilish Power") > -1)
            {
                eventCardsThatCanBePlayed.Add(dracula.eventCardsInHand.Find(ev => ev.name == "Devilish Power"));
            }
            if (eventCardsThatCanBePlayed.Count() > 0 && new Random().Next(0, 2) > 0)
            {
                return eventCardsThatCanBePlayed[new Random().Next(0, eventCardsThatCanBePlayed.Count())];
            }
            return null;
        }

        // done
        internal Event DecideWhetherToPlayDevilishPower(GameState g, Dracula dracula)
        {
            if (dracula.eventCardsInHand.FindIndex(e => e.name == "Devilish Power") > -1)
            {
                if (new Random().Next(0, 4) > 2)
                {
                    return dracula.eventCardsInHand.Find(e => e.name == "Devilish Power");
                }
            }
            return null;
        }

        // done
        internal Location DecideWhichLocationToRemoveHeavenlyHostFrom(GameState g)
        {
            List<Location> map = g.GetMap();
            List<Location> locationsWithHost = new List<Location>();
            foreach (Location loc in map)
            {
                if (loc.hasHost)
                {
                    locationsWithHost.Add(loc);
                }
            }
            return locationsWithHost[new Random().Next(0, locationsWithHost.Count())];
        }

        // done
        internal Hunter DecideWhichHunterToAmbush(Hunter[] hunters)
        {
            return hunters[new Random().Next(0, 4)];
        }

        // done
        internal Encounter DecideWhichEncounterToAmbushHunterWith(List<Encounter> encounterHand)
        {
            return encounterHand[new Random().Next(0, encounterHand.Count())];
        }

        // done
        internal int DecideWhichHunterToAttackWithQuincey(GameState g)
        {
            return new Random().Next(0, 4);
        }
    }
}
