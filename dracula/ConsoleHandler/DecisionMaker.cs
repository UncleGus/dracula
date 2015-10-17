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

namespace ConsoleHandler
{
    public class DecisionMaker
    {
        // done
        internal Location DecideDraculaStartLocation(GameState g)
        {
            Location startLocation;
            do
            {
                startLocation = g.LocationAtMapIndex(new Random().Next(0, g.MapSize()));
            } while (startLocation.type == LocationType.Hospital || startLocation.type == LocationType.Sea || startLocation.type == LocationType.Castle || g.IndexOfHunterAtLocation(startLocation) > -1);
            return startLocation;
        }

        // done
        internal Location DecideMove(GameState g, Dracula dracula, out string powerName)
        {
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
            return goingTo;
        }

        // done
        internal int DecideDoubleBackLocation(GameState g, Dracula dracula)
        {
            return new Random().Next(0, dracula.possibleDoubleBackMoves.Count());
        }

        // done
        internal string DecideWhichAllyToKeep(string p1, string p2)
        {
            if (new Random().Next(0, 2) > 0)
            {
                return p1;
            }
            else
            {
                return p2;
            }
        }

        // done
        internal Encounter DecideWhichEncounterToDiscard(GameState g, Dracula dracula)
        {
            return dracula.encounterHand[new Random().Next(0, dracula.encounterHand.Count())];
        }

        // done
        internal Event DecideWhichEventToDiscard(GameState g, Dracula dracula)
        {
            return dracula.eventCardsInHand[new Random().Next(0, dracula.eventCardsInHand.Count())];
        }

        // done
        internal Encounter DecideWhichEncounterToPlace(GameState g, Dracula dracula)
        {
            return dracula.encounterHand[new Random().Next(0, dracula.encounterHand.Count())];
        }

        // done
        internal bool DecideToPutLocationInCatacombs(GameState g, Dracula dracula)
        {
            return new Random().Next(0, 6) > 4;
        }

        // done
        internal bool DecideToDiscardCatacombLocation(GameState g, Dracula dracula)
        {
            return new Random().Next(0, 5) > 3;
        }

        // done
        internal string DecideToDiscardEventOrItem(GameState g, Dracula dracula, Hunter hunter)
        {
            if (hunter.numberOfEvents + hunter.numberOfItems > 0)
            {
                int cardToDiscard = new Random().Next(0, hunter.numberOfEvents + hunter.numberOfItems);
                if (cardToDiscard + 1 > hunter.numberOfEvents)
                {
                    return "item";
                }
                else
                {
                    return "event";
                }
            }
            return "no cards";
        }

        // done
        internal bool DecideToCancelHunterTrain(GameState g, Dracula dracula, Hunter hunter)
        {
            if (dracula.eventCardsInHand.FindIndex(card => card.name == "False Tip-Off") > -1)
            {
                if (new Random().Next(0, 2) > 0) {
                    return true;
                }
                Logger.WriteToDebugLog("Dracula decided not to play his False Tip-Off");
            }
            Logger.WriteToDebugLog("Dracula does not have a False Tip-Off");
            return false;
        }

        // done
        internal Item DecideWhichCombatCardToPlay(List<Hunter> huntersFighting, Dracula dracula, List<Item> combatCards, string hunterAllyName, CombatRoundResult result)
        {
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
            return combatCards[chosenCardIndex];
        }

        // done
        internal void DecideOrderOfEncountersAtLocation(Hunter hunter, Location location)
        {
            // do nothing for now
        }

        // done
        internal string DecideHunterToAttack(List<Hunter> huntersFighting, List<Item> combatCards, CombatRoundResult result)
        {
            return huntersFighting[new Random().Next(0, huntersFighting.Count())].name;
        }

        // done
        internal Encounter DecideWhichCatacombsEncounterToDiscard(GameState g, Location goingTo, UserInterface ui)
        {
            if (goingTo.encounters.Count() > 1)
            {
                int random = new Random().Next(0, 2);
                ui.TellUser("Dracula discarded the encounter in slot " + random);
                return goingTo.encounters[random];
            }
            else
            {
                return goingTo.encounters.First();
            }
        }

        // done
        internal Event DecideEventCardToPlayAtStartOfHunterMovement(GameState g, Dracula dracula)
        {
            if (dracula.eventCardsInHand.FindIndex(e => e.name == "Control Storms") > -1) {
                if (new Random().Next(0, 2) > 0)
                {
                    return dracula.eventCardsInHand.Find(e => e.name == "Control Storms");
                }
            }
            return null;
        }

        // done
        internal Event DecideEventCardToPlayAtStartOfDraculaTurn(GameState g, Dracula dracula)
        {
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
                return eventCardsThatCanBePlayed[new Random().Next(0, eventCardsThatCanBePlayed.Count())];
            }
            return null;
        }

        // done
        internal Event DecideToPlaySeductionDuringVampireEncounter(GameState g, Dracula dracula)
        {
            if (dracula.eventCardsInHand.FindIndex(e => e.name == "Seduction") > -1)
            {
                if (new Random().Next(0, 2) > 0)
                {
                    return dracula.eventCardsInHand.Find(e => e.name == "Seduction");
                }
            }
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
