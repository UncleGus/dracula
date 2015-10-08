using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationHandler;
using LogHandler;
using EncounterHandler;

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
        int encounterHandSize;
        List<Encounter> encounterPool = new List<Encounter>();
        List<Encounter> encounterHand = new List<Encounter>();
        Location locationWhereHideWasUsed;

        public Dracula(Location startLocation, List<Encounter> poolOfEncounters)
        {
            currentLocation = startLocation;
            trail.Add(currentLocation);
            powers.Add(new DraculaPower("Dark Call", true));
            powers.Add(new DraculaPower("Double Back", true));
            powers.Add(new DraculaPower("Feed", false));
            powers.Add(new DraculaPower("Hide", true));
            powers.Add(new DraculaPower("Wolf Form", false));
            blood = 15;
            lostBloodAtSeaOnLatestTurn = false;
            encounterHandSize = 5;
            encounterPool = poolOfEncounters;
            DrawEncounters(encounterHandSize);
        }

        public bool MoveDracula(int timeOfDay)
        {

            // build list of possible locations to move to
            determinePossibleLocations();

            // build list of possible powers to play
            determinePossiblePowers(timeOfDay);

            // check if there are no legal moves
            Logger.WriteToDebugLog("Checking if there are legal moves");
            if (possibleMoves.Count() + possiblePowers.Count() == 0)
            {
                Logger.WriteToDebugLog("Dracula has no legal moves");
                Console.WriteLine("Dracula is cornered by his own trail");
                return false;
            }
            else if (possibleMoves.Count() == 0 && possiblePowers.Count() == 1 && possiblePowers.Contains(powers[1]))
            {
                Logger.WriteToDebugLog("Dracula has no regular moves available");
                determinePossibleWolfFormLocations();
                if (possibleMoves.Count() == 0)
                {
                    Logger.WriteToDebugLog("Dracula has no moves available by Wolf Form");
                    Console.WriteLine("Dracula is cornered by his own trail");
                    return false;
                }
                determinePossibleLocations();
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
                    DoDoubleBackMove();
                }
                else if (possiblePowers[chosenActionIndex].name == "Wolf Form")
                {
                    DoWolfFormMove();
                }

                // put the power used at the head of the trail
                Logger.WriteToDebugLog("Putting power " + possiblePowers[chosenActionIndex].name + " at the head of the trail");
                possiblePowers[chosenActionIndex].positionInTrail = 0;
            }
            else
            {
                // choose a location
                moveByRoadOrSea();
                Logger.WriteToGameLog("Dracula moved to " + currentLocation.name);
            }
            Logger.WriteToDebugLog("Dracula has finished moving");
            Logger.WriteToDebugLog("Checking if the location where Dracula hid has been removed from the trail");
            if (locationWhereHideWasUsed != null)
            {
                if (!trail.Contains(locationWhereHideWasUsed))
                {
                    Logger.WriteToDebugLog("Hide location has left the trail, revealing the Hide card");
                    RevealHide(trail.FindIndex(location => location.name == "Hide"));
                }
            }
            return true;
        }

        public void ShowLocation()
        {
            Console.WriteLine("Dracula is currently in: " + currentLocation.name);
        }

        public void ShowTrail()
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
                Console.WriteLine(trail[i].name + powerName);
            }
        }

        public void determinePossibleLocations()
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

            for (int i = 0; i < trail.Count(); i++)
            {
                if (trail[i].name != "Dark Call" && trail[i].name != "Hide" && trail[i].name != "Feed")
                {
                    Logger.WriteToDebugLog("Moving location " + trail[i].name + " from possible moves to Double Back locations");
                    possibleDoubleBackMoves.Add(trail[i]);
                }
                Logger.WriteToDebugLog("Removing " + trail[i].name + " from possible moves as it is in Dracula's trail");
                possibleMoves.Remove(trail[i]);
            }
            Logger.WriteToDebugLog("Removing the current location, " + currentLocation.name + " from possible Double Back moves");
            possibleDoubleBackMoves.Remove(currentLocation);
        }

        public void determinePossibleWolfFormLocations()
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
                    Logger.WriteToDebugLog("Adding location " + currentLocation.byRoad[i]);
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

        public void determinePossiblePowers(int timeOfDay)
        {
            Logger.WriteToDebugLog("Determining possible powers to use");
            Logger.WriteToDebugLog("Clearing the old list of possible powers to use");
            possiblePowers.Clear();
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
                                determinePossibleWolfFormLocations();
                                if (possibleMoves.Count() > 0)
                                {
                                    Logger.WriteToDebugLog("Adding power " + powers[i].name + " to the list");
                                    possiblePowers.Add(powers[i]);
                                }
                                else
                                {
                                    Logger.WriteToDebugLog("Not adding Wolf Form to the list as it cannot be used validly from this location");
                                }
                                determinePossibleLocations();
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

        public void moveByRoadOrSea()
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
                Console.WriteLine("Dracula tried to do something illegal");
                Logger.WriteToDebugLog("DRACULA TRIED TO DO SOMETHING ILLEGAL");
            }

            Logger.WriteToDebugLog("Adding " + currentLocation.name + " to the head of the trail");
            trail.Insert(0, currentLocation);
            MovePowersAlongTrail();
            TrimTrail(6);
            CheckBloodLossAtSea(previousLocationType);
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
                trail.Last().isRevealed = false;
                Logger.WriteToDebugLog("Removing " + trail.Last().name);
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

        public void RevealHide(int hideIndex)
        {
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
            // set Hide position out of the trail

            Console.WriteLine("Dracula was hiding");
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
            TrimTrail(6);
        }

        public void DoDoubleBackMove()
        {
            Logger.WriteToDebugLog("Remembering that Dracula is moving from a location of type " + currentLocation.type);
            LocationType previousLocationType = currentLocation.type;


            // choose a location from the possible double back moves
            int doubleBackLocation = new Random().Next(0, possibleDoubleBackMoves.Count());

            int doubleBackPositionInTrail = trail.FindIndex(location => location == possibleDoubleBackMoves[doubleBackLocation]);
            if (doubleBackPositionInTrail > 0)
            {
                if (trail[doubleBackPositionInTrail - 1].name == "Hide")
                {
                    Logger.WriteToDebugLog("Dracula Doubled Back to " + possibleDoubleBackMoves[doubleBackLocation].name + " and Hide is the next card in the trail");
                    Console.WriteLine("Dracula Doubled Back to a location where he previously used Hide (position " + (doubleBackPositionInTrail - 1) + ")");
                    RevealHide(doubleBackPositionInTrail - 1);
                }
                else if (doubleBackPositionInTrail > 1)
                {
                    if (trail[doubleBackPositionInTrail - 1].type == LocationType.Power && trail[doubleBackPositionInTrail - 2].name == "Hide")
                    {
                        Logger.WriteToDebugLog("Dracula Doubled Back to " + possibleDoubleBackMoves[doubleBackLocation].name + " and Hide is the next card in the trail, after " + trail[doubleBackPositionInTrail - 1].name);
                        Console.WriteLine("Dracula Doubled Back to a location where he previously used Hide (position " + (doubleBackPositionInTrail - 1) + ")");
                        RevealHide(doubleBackPositionInTrail - 2);
                    }
                    else if (doubleBackPositionInTrail > 2)
                    {
                        if (trail[doubleBackPositionInTrail - 1].type == LocationType.Power && trail[doubleBackPositionInTrail - 2].type == LocationType.Power && trail[doubleBackPositionInTrail - 3].name == "Hide")
                        {
                            Logger.WriteToDebugLog("Dracula Doubled Back to " + possibleDoubleBackMoves[doubleBackLocation].name + " and Hide is the next card in the trail, after " + trail[doubleBackPositionInTrail - 1].name + " and " + trail[doubleBackPositionInTrail - 2].name);
                            Console.WriteLine("Dracula Doubled Back to a location where he previously used Hide (position " + (doubleBackPositionInTrail - 1) + ")");
                            RevealHide(doubleBackPositionInTrail - 3);
                        }
                    }
                }
            }

            // move location to the front of the trail
            Logger.WriteToDebugLog("Temporarily holding " + possibleDoubleBackMoves[doubleBackLocation].name);
            Location tempLocation = possibleDoubleBackMoves[doubleBackLocation];
            Logger.WriteToDebugLog("Removing " + tempLocation.name + " from the trail");
            trail.Remove(tempLocation);
            Logger.WriteToDebugLog("Putting " + tempLocation.name + " back at the head of the trail");
            trail.Insert(0, tempLocation);
            Logger.WriteToDebugLog("Dracula's current location is now " + currentLocation.name);
            currentLocation = trail[0];

            Logger.WriteToGameLog("Dracula Doubled Back to " + currentLocation.name);

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

        public void DoWolfFormMove()
        {
            // determine possible locations for wolf form (road only, up to two moves away)
            determinePossibleWolfFormLocations();
            // carry out move
            moveByRoadOrSea();
            Logger.WriteToGameLog("Dracula used Wolf Form to move to " + currentLocation.name);
        }

        public void DrawEncounters(int totalNumberOfEncounters)
        {
            Logger.WriteToDebugLog("Dracula is drawing up to " + totalNumberOfEncounters + " encounters");
            while (encounterHand.Count() < totalNumberOfEncounters)
            {
                Encounter tempEncounter = encounterPool[new Random().Next(0, encounterPool.Count())];
                Logger.WriteToDebugLog("Dracula drew encounter " + tempEncounter.name);
                Logger.WriteToGameLog("Dracula drew encounter " + tempEncounter.name);
                encounterHand.Add(tempEncounter);
                encounterPool.Remove(tempEncounter);
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
