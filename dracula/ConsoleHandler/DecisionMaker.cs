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
                powerName = "No power";
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

        internal Event DecideToPlayCardAtStartOfCombat(GameState g)
        {
            return null;
        }
    }
}
