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
        internal LocationDetail DecideDraculaStartLocation(GameState g)
        {
            Logger.WriteToDebugLog("Starting to decide where Dracula will start");
            LocationDetail startLocation;
            do
            {
                startLocation = g.LocationAtMapIndex(new Random().Next(0, 71));
            } while (startLocation.Type == LocationType.Hospital || startLocation.Type == LocationType.Sea || startLocation.Type == LocationType.Castle || g.IndexOfHunterAtLocation(startLocation) > -1);
            Logger.WriteToDebugLog("Returning" + startLocation);
            return startLocation;
        }

        // done
        internal LocationDetail DecideMove(GameState g, Dracula dracula, out string powerName)
        {
            Logger.WriteToDebugLog("Starting to decide what move to make");
            LocationDetail goingTo;
            if (dracula.AdvanceMovePower != null || dracula.AdvanceMoveDestination != null)
            {
                powerName = dracula.AdvanceMovePower;
                goingTo = dracula.AdvanceMoveDestination;
                dracula.AdvanceMovePower = null;
                dracula.AdvanceMoveDestination = null;
            }
            int chosenActionIndex = new Random().Next(0, dracula.PossibleMoves.Count() + dracula.PossiblePowers.Count());
            if (chosenActionIndex > dracula.PossibleMoves.Count() - 1)
            {
                // choosing a power
                chosenActionIndex -= dracula.PossibleMoves.Count();
                powerName = dracula.PossiblePowers[chosenActionIndex].name;
                if (powerName == "Dark Call" || powerName == "Feed" || powerName == "Hide")
                {
                    goingTo = new LocationDetail();
                    goingTo.Name = "Nowhere";
                }
                else if (powerName == "Double Back")
                {
                    goingTo = dracula.PossibleDoubleBackMoves[new Random().Next(0, dracula.PossibleDoubleBackMoves.Count())];
                }
                else if (powerName == "Wolf Form")
                {
                    dracula.DeterminePossibleWolfFormLocations();
                    goingTo = dracula.PossibleMoves[new Random().Next(0, dracula.PossibleMoves.Count())];
                }
                else
                {
                    goingTo = new LocationDetail();
                    goingTo.Name = "Unknown location";
                }
            }
            else
            {
                powerName = "no power";
                goingTo = dracula.PossibleMoves[chosenActionIndex];
            }
            Logger.WriteToDebugLog("Returning " + powerName + " and " + goingTo == null ? "null" : goingTo.Name);
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
            Encounter encounterToDiscard = dracula.EncounterHand[new Random().Next(0, dracula.EncounterHand.Count())];
            Logger.WriteToDebugLog("Returning " + encounterToDiscard.name);
            return encounterToDiscard;
        }

        // done
        internal Event DecideWhichEventToDiscard(GameState g, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding which event to discard");
            Event eventToDiscard = dracula.EventCardsInHand[new Random().Next(0, dracula.EventCardsInHand.Count())];
            Logger.WriteToDebugLog("Returning " + eventToDiscard.name);
            return eventToDiscard;
        }

        // done
        internal Encounter DecideWhichEncounterToPlace(GameState g, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding which encounter to place");
            Encounter encounterToPlace = dracula.EncounterHand[new Random().Next(0, dracula.EncounterHand.Count())];
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
            if (hunter.NumberOfEvents + hunter.NumberOfItems > 0)
            {
                int cardToDiscard = new Random().Next(0, hunter.NumberOfEvents + hunter.NumberOfItems);
                if (cardToDiscard + 1 > hunter.NumberOfEvents)
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
            if (dracula.EventCardsInHand.FindIndex(card => card.name == "False Tip-Off") > -1)
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
            if (hunterAllyName == "Sister Agatha" && dracula.Blood < 3)
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
            string nameOfHunterToAttack = huntersFighting[new Random().Next(0, huntersFighting.Count())].Name;
            Logger.WriteToDebugLog("Returning " + nameOfHunterToAttack);
            return nameOfHunterToAttack;
        }

        // done
        internal Encounter DecideWhichCatacombsEncounterToDiscard(GameState g, LocationDetail goingTo, UserInterface ui)
        {
            Logger.WriteToDebugLog("Deciding which encounter to discard off a location previously in the catacombs");
            if (goingTo.Encounters.Count() > 1)
            {
                int random = new Random().Next(0, 2);
                ui.TellUser("Dracula discarded the encounter in slot " + random);
                Logger.WriteToDebugLog("Returning " + goingTo.Encounters[random]);
                return goingTo.Encounters[random];
            }
            else
            {
                Logger.WriteToDebugLog("Returning " + goingTo.Encounters.First().name);
                return goingTo.Encounters.First();
            }
        }

        // done
        internal Event DecideToPlayControlStorms(GameState g, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding whether to play Control Storms");
            if (dracula.EventCardsInHand.FindIndex(e => e.name == "Control Storms") > -1) {
                if (new Random().Next(0, 2) > 0)
                {
                    Logger.WriteToDebugLog("Returning Control Storms");
                    return dracula.EventCardsInHand.Find(e => e.name == "Control Storms");
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
            if (dracula.EventCardsInHand.FindIndex(ev => ev.name == "Time Runs Short") > -1) {
                eventCardsThatCanBePlayed.Add(dracula.EventCardsInHand.Find(ev => ev.name == "Time Runs Short"));
            }
            if (dracula.EventCardsInHand.FindIndex(ev => ev.name == "Unearthly Swiftness") > -1) {
                eventCardsThatCanBePlayed.Add(dracula.EventCardsInHand.Find(ev => ev.name == "Unearthly Swiftness"));
            }
            if (dracula.EventCardsInHand.FindIndex(ev => ev.name == "Roadblock") > -1) {
                eventCardsThatCanBePlayed.Add(dracula.EventCardsInHand.Find(ev => ev.name == "Roadblock"));
            }
            if (dracula.EventCardsInHand.FindIndex(ev => ev.name == "Devilish Power") > -1 && (g.HuntersHaveAlly() || g.HeavenlyHostIsInPlay())) {
                eventCardsThatCanBePlayed.Add(dracula.EventCardsInHand.Find(ev => ev.name == "Devilish Power"));
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
            if (dracula.EventCardsInHand.FindIndex(e => e.name == "Seduction") > -1)
            {
                if (new Random().Next(0, 2) > 0)
                {
                    Logger.WriteToDebugLog("Returning Seduction");
                    return dracula.EventCardsInHand.Find(e => e.name == "Seduction");
                }
            }
            Logger.WriteToDebugLog("Returning null");
            return null;
        }

        // done
        internal Event DecideToPlayCardAtStartOfCombat(GameState g, Dracula dracula, bool trapPlayed, bool hunterMoved, int enemyType)
        {
            Logger.WriteToDebugLog("Deciding which card to play at the start of combat");
            List<Event> eventCardsThatCanBePlayed = new List<Event>();
            if (dracula.EventCardsInHand.FindIndex(ev => ev.name == "Trap") > -1)
            {
                eventCardsThatCanBePlayed.Add(dracula.EventCardsInHand.Find(ev => ev.name == "Trap"));
            }
            if (dracula.EventCardsInHand.FindIndex(ev => ev.name == "Rage") > -1)
            {
                if (enemyType == 1)
                {
                    eventCardsThatCanBePlayed.Add(dracula.EventCardsInHand.Find(ev => ev.name == "Rage"));
                }
            }
            if (dracula.EventCardsInHand.FindIndex(ev => ev.name == "Wild Horses") > -1)
            {
                if (!hunterMoved && enemyType == 1)
                {
                    eventCardsThatCanBePlayed.Add(dracula.EventCardsInHand.Find(ev => ev.name == "Wild Horses"));
                }
            }
            if (eventCardsThatCanBePlayed.Count() > 0 && new Random().Next(0, 2) > 0)
            {
                Event cardToReturn = eventCardsThatCanBePlayed[new Random().Next(0, eventCardsThatCanBePlayed.Count())];
                Logger.WriteToDebugLog("Returning " + cardToReturn.name);
                return cardToReturn;
            }
            Logger.WriteToDebugLog("Returning null");
            return null;
        }

        // done
        internal LocationDetail DecideLocationToSendHunterWithControlStorms(GameState g, int hunterIndex, List<LocationDetail> possiblePorts)
        {
            Logger.WriteToDebugLog("Deciding where to send hunter with Control Storms");
            LocationDetail destination = possiblePorts[new Random().Next(0, possiblePorts.Count())];
            Logger.WriteToDebugLog("Returning " + destination.Name);
            return destination;
        }

        // done
        internal Event DecideToPlayCustomsSearch(GameState g, Dracula dracula, Hunter hunter)
        {
            Logger.WriteToDebugLog("Deciding whether to play Customs Search");
            if (dracula.EventCardsInHand.FindIndex(e => e.name == "Customs Search") > -1)
            {
                if (new Random().Next(0, 2) > 0)
                {
                    Logger.WriteToDebugLog("Returning Customs Search");
                    return dracula.EventCardsInHand.Find(e => e.name == "Customs Search");
                }
            }
            Logger.WriteToDebugLog("Returning null");
            return null;
        }

        // done
        internal LocationDetail DecideWhereToEvadeTo(GameState g)
        {
            Logger.WriteToDebugLog("Deciding where to move with Evade");
            List<LocationDetail> map = g.GetMap();
            LocationDetail locationToReturn;
            do {
            locationToReturn = map[new Random().Next(0, map.Count())];
            } while (g.LocationIsInTrail(locationToReturn) || g.LocationIsInCatacombs(locationToReturn));
            Logger.WriteToDebugLog("Returning " + locationToReturn.Name);
            return locationToReturn;
        }

        // done
        internal Hunter DecideWhoToNightVisit(GameState g)
        {
            Logger.WriteToDebugLog("Deciding who to attack with Night Visit");
            List <Hunter> bittenHunters = g.GetBittenHunters();
            Hunter victim = bittenHunters[new Random().Next(0, bittenHunters.Count())];
            Logger.WriteToDebugLog("Returning " + victim.Name);
            return victim;
        }

        // done
        internal Hunter DecideWhoToInfluence(GameState g)
        {
            Logger.WriteToDebugLog("Deciding which hunter to target with Vampiric Influence");
            List<Hunter> bittenHunters = g.GetBittenHunters();
            Hunter victim = bittenHunters[new Random().Next(0, bittenHunters.Count())];
            Logger.WriteToDebugLog("Returning " + victim.Name);
            return victim;
        }

        // done
        internal Hunter DecideWhichHunterToRage(List<Hunter> huntersInCombat)
        {
            Logger.WriteToDebugLog("Deciding which hunter to target with Rage");
            Hunter victim = huntersInCombat[new Random().Next(0, huntersInCombat.Count())];
            Logger.WriteToDebugLog("Returning " + victim.Name);
            return victim;
        }

        // done
        internal Item DecideWhichItemToDiscard(List<Item> items)
        {
            Logger.WriteToDebugLog("Deciding which item to discard from hunter");
            Item discardedItem =  items[new Random().Next(0, items.Count())];
            Logger.WriteToDebugLog("Returning " + discardedItem.name);
            return discardedItem;
        }

        // done
        internal LocationDetail DecideWhereToSendHuntersWithWildHorses(GameState g, List<Hunter> hunters)
        {
            Logger.WriteToDebugLog("Deciding where to send hunter with Wild Horses");
            LocationDetail destination = hunters.First().CurrentLocation.ByRoad[new Random().Next(0, hunters.First().CurrentLocation.ByRoad.Count())];
            Logger.WriteToDebugLog("Returning " + destination);
            return destination;
        }

        // done
        internal Event DecideWhetherToPlayRelentlessMinion(GameState g, List<Hunter> huntersEncountered, string enemyType, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding whether to play Relentless Minion");
            if (dracula.EventCardsInHand.FindIndex(e => e.name == "Relentless Minion") > -1)
            {
                if (new Random().Next(0, 2) > 0)
                {
                    Logger.WriteToDebugLog("Returning Relentless Minion");
                    return dracula.EventCardsInHand.Find(e => e.name == "Relentless Minion");
                }
            }
            Logger.WriteToDebugLog("Returning null");
            return null;
        }

        // done
        internal void DecideWhereToPutRoadblock(GameState g, Roadblock roadblockCounter)
        {
            Logger.WriteToDebugLog("Deciding where to put Roadblock");
            List<LocationDetail> map = g.GetMap();
            LocationDetail firstLocationToBlock;
            do
            {
                firstLocationToBlock = map[new Random().Next(0, map.Count())];
            } while (firstLocationToBlock.Type != LocationType.City && firstLocationToBlock.Type != LocationType.Town);
            List<LocationDetail> secondLocationToBlockCandidates = new List<LocationDetail>();
            foreach (LocationDetail loc in firstLocationToBlock.ByRoad)
            {
                secondLocationToBlockCandidates.Add(loc);
            }
            foreach (LocationDetail loc in firstLocationToBlock.ByTrain)
            {
                secondLocationToBlockCandidates.Add(loc);
            }
            LocationDetail secondLocationToBlock = secondLocationToBlockCandidates[new Random().Next(0, secondLocationToBlockCandidates.Count())];
            if (firstLocationToBlock.ByRoad.Contains(secondLocationToBlock))
            {
                if (firstLocationToBlock.ByTrain.Contains(secondLocationToBlock))
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
            Logger.WriteToDebugLog("Nothing is returned, but the Roadblock counter has been put on the " + roadblockCounter.connectionType + " between " + roadblockCounter.firstLocation + " and " + roadblockCounter.secondLocation);
        }

        // done
        internal Event DecideWhetherToPlaySensationalistPress(GameState g, int trailIndex, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding whether to play Sensationalist Press");
            if (dracula.EventCardsInHand.FindIndex(e => e.name == "Sensationalist Press") > -1)
            {
                if (new Random().Next(0, 2) > 0)
                {
                    Logger.WriteToDebugLog("Returning Sensationalist Press");
                    return dracula.EventCardsInHand.Find(e => e.name == "Sensationalist Press");
                }
            }
            Logger.WriteToDebugLog("Returning null");
            return null;
        }

        // done
        internal LocationDetail DecideWhichPortToGoToAfterStormySeas(GameState g, LocationDetail locationStormed)
        {
            Logger.WriteToDebugLog("Deciding which port to go to after having Stormy Seas played on current location");
            LocationDetail port;
            do
            {
                port = locationStormed.BySea[new Random().Next(0, locationStormed.BySea.Count())];
            } while (port.Type != LocationType.City && port.Type != LocationType.Town);
            Logger.WriteToDebugLog("Returning " + port.Name);
            return port;
        }

        // done
        internal Event DecideWhetherToCancelCharteredCarriage(GameState g, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding whether to cancel Chartered Carriage");
            List<Event> eventCardsThatCanBePlayed = new List<Event>();
            if (dracula.EventCardsInHand.FindIndex(ev => ev.name == "False Tip-off") > -1)
            {
                eventCardsThatCanBePlayed.Add(dracula.EventCardsInHand.Find(ev => ev.name == "False Tip-off"));
            }
            if (dracula.EventCardsInHand.FindIndex(ev => ev.name == "Devilish Power") > -1)
            {
                eventCardsThatCanBePlayed.Add(dracula.EventCardsInHand.Find(ev => ev.name == "Devilish Power"));
            }
            if (eventCardsThatCanBePlayed.Count() > 0 && new Random().Next(0, 2) > 0)
            {
                Event eventToReturn = eventCardsThatCanBePlayed[new Random().Next(0, eventCardsThatCanBePlayed.Count())];
                Logger.WriteToDebugLog("Returning " + eventToReturn.name);
                return eventToReturn;
            }
            Logger.WriteToDebugLog("Returning null");
            return null;
        }

        // done
        internal Event DecideWhetherToPlayDevilishPower(GameState g, Dracula dracula)
        {
            Logger.WriteToDebugLog("Deciding whether to play Devilish Power");
            if (dracula.EventCardsInHand.FindIndex(e => e.name == "Devilish Power") > -1)
            {
                if (new Random().Next(0, 4) > 2)
                {
                    Logger.WriteToDebugLog("Returning Devilish Power");
                    return dracula.EventCardsInHand.Find(e => e.name == "Devilish Power");
                }
            }
            Logger.WriteToDebugLog("Returning null");
            return null;
        }

        // done
        internal LocationDetail DecideWhichLocationToRemoveHeavenlyHostFrom(GameState g)
        {
            Logger.WriteToDebugLog("Deciding from which location to remove Heavenly Host");
            List<LocationDetail> map = g.GetMap();
            List<LocationDetail> locationsWithHost = new List<LocationDetail>();
            foreach (LocationDetail loc in map)
            {
                if (loc.HasHost)
                {
                    locationsWithHost.Add(loc);
                }
            }
            LocationDetail target = locationsWithHost[new Random().Next(0, locationsWithHost.Count())];
            Logger.WriteToDebugLog("Returning " + target.Name);
            return target;
        }

        // done
        internal Hunter DecideWhichHunterToAmbush(Hunter[] hunters)
        {
            Logger.WriteToDebugLog("Deciding which hunter to Ambush");
            Hunter victim = hunters[new Random().Next(0, 4)];
            Logger.WriteToDebugLog("Returning " + victim.Name);
            return victim;
        }

        // done
        internal Encounter DecideWhichEncounterToAmbushHunterWith(List<Encounter> encounterHand)
        {
            Logger.WriteToDebugLog("Decising with which encounter to Ambush hunter");
            Encounter ambush = encounterHand[new Random().Next(0, encounterHand.Count())];
            Logger.WriteToDebugLog("Returning " + ambush.name);
            return ambush;
        }

        // done
        internal Hunter DecideWhichHunterToAttackWithQuincey(Hunter[] hunters)
        {
            Logger.WriteToDebugLog("Deciding which hunter to target with Quincey P. Morris");
            Hunter victim = hunters[new Random().Next(0, 4)];
            Logger.WriteToDebugLog("Returning " + victim.Name);
            return victim;
        }

        // done
        internal LocationDetail DecideWhereToSendHuntersWithBats(GameState g, Hunter hunter, List<LocationDetail> batsMoves)
        {
            Logger.WriteToDebugLog("Deciding where to send hunter with Bats");
            LocationDetail destination = batsMoves[new Random().Next(0, batsMoves.Count())];
            Logger.WriteToDebugLog("Returning " + destination.Name);
            return destination;
        }
    }
}
