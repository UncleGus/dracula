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
        internal Location DecideDraculaStartLocation(GameState g)
        {
            Location startLocation;
            do
            {
                startLocation = g.LocationAtMapIndex(new Random().Next(0, g.MapSize()));
            } while (startLocation.type == LocationType.Hospital);
            return startLocation;
        }

        internal void DecideMove(GameState g, Dracula dracula, out string powerName, out Location goingTo)
        {
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
        }

        internal int DecideDoubleBackLocation(GameState g, Dracula dracula)
        {
            return new Random().Next(0, dracula.possibleDoubleBackMoves.Count());
        }

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

        internal Encounter DecideWhichEncounterToDiscard(GameState g, Dracula dracula)
        {
            return dracula.encounterHand[new Random().Next(0, dracula.encounterHand.Count())];
        }

        internal Event DecideWhichEventToDiscard(GameState g, Dracula dracula)
        {
            return dracula.eventCardsInHand[new Random().Next(0, dracula.eventCardsInHand.Count())];
        }

        internal Encounter DecideWhichEncounterToPlace(GameState g, Dracula dracula)
        {
            return dracula.encounterHand[new Random().Next(0, dracula.encounterHand.Count())];
        }

        internal bool DecideToPutLocationInCatacombs(GameState g, Dracula dracula)
        {
            return new Random().Next(0, 6) > 4;
        }

        internal bool DecideToDiscardCatacombLocation(GameState g, Dracula dracula)
        {
            return new Random().Next(0, 5) > 3;
        }

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

        internal bool DecideToCancelHunterTrain(GameState g, Dracula dracula, Hunter hunter)
        {
            if (dracula.eventCardsInHand.FindIndex(card => card.name == "False Tip-Off") > -1)
            {
                if (new Random().Next(0, 2) > 1) {
                    return true;
                }
                Logger.WriteToDebugLog("Dracula decided not to play his False Tip-Off");
            }
            Logger.WriteToDebugLog("Dracula does not have a False Tip-Off");
            return false;
        }

        internal Item DecideWhichCombatCardToPlay(Hunter hunter, List<Item> combatCards, CombatRoundResult result)
        {
            int chosenCardIndex;
            do
            {
                chosenCardIndex = new Random().Next(0, combatCards.Count());
            } while (combatCards[chosenCardIndex].name == result.enemyCardUsed);
            return combatCards[chosenCardIndex];
        }

        internal void DecideOrderOfEncountersAtLocation(Hunter hunter, Location location)
        {
            // do nothing for now
        }

        internal string DecideHunterToAttack(Hunter hunter, List<Item> combatCards, CombatRoundResult result)
        {
            return hunter.huntersInGroup[new Random().Next(0, hunter.huntersInGroup.Count())].name;
        }

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

        internal Event DecideEventCardToPlayAtStartOfHunterMovement(GameState g)
        {
            return null;
        }

        internal Event DecideEventCardToPlayAtEndOfHunterMovement(GameState g)
        {
            return null;
        }

        internal Event DecideEventCardToPlayAtStartOfDraculaTurn(GameState g)
        {
            return null;
        }

        internal Event DecideToPlaySeductionDuringVampireEncounter(GameState g)
        {
            return null;
        }

        internal Event DecideToPlayCardAtStartOfCombat(GameState g, bool trapPlayed)
        {
            return null;
        }

        internal Location DecideLocationToSendHunterWithControlStorms(GameState g, int hunterIndex, List<Location> possiblePorts)
        {
            return possiblePorts[new Random().Next(0, possiblePorts.Count())];
        }

        internal Event DecideToPlayCustomsSearch(GameState g, Hunter hunter)
        {
            return null;
        }

        internal Location DecideWhereToEvadeTo(GameState g)
        {
            List<Location> map = g.GetMap();
            Location locationToReturn;
            do {
            locationToReturn = map[new Random().Next(0, map.Count())];
            } while (g.LocationIsInTrail(locationToReturn) || g.LocationIsInCatacombs(locationToReturn));
            return locationToReturn;
        }

        internal Hunter DecideWhoToNightVisit(GameState g)
        {
            List <Hunter> bittenHunters = g.GetBittenHunters();
            return bittenHunters[new Random().Next(0, bittenHunters.Count())];
        }

        internal Hunter DecideWhoToInfluence(GameState g)
        {
            List<Hunter> bittenHunters = g.GetBittenHunters();
            return bittenHunters[new Random().Next(0, bittenHunters.Count())];
        }

        internal Hunter DecideWhichHunterToRage(List<Hunter> huntersInCombat)
        {
            return huntersInCombat[new Random().Next(0, huntersInCombat.Count())];
        }

        internal Item DecideWhichItemToDiscard(List<Item> items)
        {
            return items[new Random().Next(0, items.Count())];
        }

        internal Location DecideWhereToSendHuntersWithWildHorses(GameState g, List<Hunter> hunters)
        {
            return hunters.First().currentLocation.byRoad[new Random().Next(0, hunters.First().currentLocation.byRoad.Count())];
        }

        internal Event DecideWhetherToPlayRelentlessMinion(GameState g, List<Hunter> huntersEncountered, string enemyType)
        {
            return null;
        }

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

        internal Event DecideWhetherToPlaySensationalistPress(GameState g, int trailIndex)
        {
            return null;
        }

        internal Location DecideWhichPortToGoToAfterStormySeas(GameState g, Location locationStormed)
        {
            Location port;
            do
            {
                port = locationStormed.bySea[new Random().Next(0, locationStormed.bySea.Count())];
            } while (port.type != LocationType.City && port.type != LocationType.Town);
            return port;
        }
    }
}
