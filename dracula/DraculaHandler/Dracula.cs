using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationHandler;

namespace DraculaHandler
{
    public class Dracula
    {
        Location currentLocation;
        public List<Location> trail = new List<Location>();
        public List<DraculaPower> powers = new List<DraculaPower>();
        List<Location> possibleMoves = new List<Location>();
        List<Location> possibleDoubleBackMoves = new List<Location>();
        List<DraculaPower> possiblePowers = new List<DraculaPower>();

        public Dracula(Location startLocation)
        {
            currentLocation = startLocation;
            trail.Add(currentLocation);
            powers.Add(new DraculaPower("Dark Call", true));
            powers.Add(new DraculaPower("Double Back", true));
            powers.Add(new DraculaPower("Feed", false));
            powers.Add(new DraculaPower("Hide", true));
            powers.Add(new DraculaPower("Wolf Form", false));
        }

        public bool MoveDracula(int timeOfDay)
        {

            // build list of possible locations to move to
            determinePossibleLocations();

            // build list of possible powers to play
            determinePossiblePowers(timeOfDay);

            // check if there are no legal moves
            if (possibleMoves.Count() + possiblePowers.Count() == 0)
            {
                Console.WriteLine("Dracula is cornered by his own trail");
                return false;
            } else if (possibleMoves.Count() == 0 && possiblePowers.Count() == 1 && possiblePowers.Contains(powers[1]))
            {
                determinePossibleWolfFormLocations();
                if (possibleMoves.Count() == 0)
                {
                    Console.WriteLine("Dracula is cornered by his own trail");
                    return false;
                } else
                {
                    determinePossibleLocations();
                }
            }
            //Console.WriteLine("Dracula has " + possibleMoves.Count() + " possible moves and " + possiblePowers.Count() + " possible powers");


            // choose an action from all possible actions
            int chosenActionIndex = new Random().Next(0, possibleMoves.Count() + possiblePowers.Count());
            //Console.WriteLine("Dracula is choosing action " + chosenActionIndex);

            // choose a power
            if (chosenActionIndex > possibleMoves.Count() - 1)
            {
                chosenActionIndex -= possibleMoves.Count();

                if (possiblePowers[chosenActionIndex].name == "Dark Call" || possiblePowers[chosenActionIndex].name == "Feed" || possiblePowers[chosenActionIndex].name == "Hide")
                {
                    // add the power card to the location trail by using a dummy "location"
                    Location powerCard = new Location();
                    powerCard.name = possiblePowers[chosenActionIndex].name;
                    powerCard.abbreviation = possiblePowers[chosenActionIndex].name.Substring(0, 3).ToUpper();
                    if (powerCard.name != "Hide")
                    {
                        powerCard.isRevealed = true;
                        powerCard.type = LocationType.Power;
                    }
                    trail.Insert(0, powerCard);
                    MovePowersAlongTrail();
                    TrimTrail(6);

                }
                else if (possiblePowers[chosenActionIndex].name == "Double Back")
                {
                    // choose a location from the possible double back moves
                    int doubleBackLocation = new Random().Next(0, possibleDoubleBackMoves.Count());

                    int doubleBackPositionInTrail = trail.FindIndex(location => location == possibleDoubleBackMoves[doubleBackLocation]);
                    if (doubleBackPositionInTrail > 0)
                    {
                        if (trail[doubleBackPositionInTrail - 1].name == "Hide")
                        {
                            Console.WriteLine("Dracula Doubled Back to a location where he previously used Hide");
                            RevealHide(doubleBackPositionInTrail - 1);
                        }
                        else if (doubleBackPositionInTrail > 1)
                        {
                            if (trail[doubleBackPositionInTrail - 1].type == LocationType.Power && trail[doubleBackPositionInTrail - 2].name == "Hide")
                            {
                                Console.WriteLine("Dracula Doubled Back to a location where he previously used Hide");
                                RevealHide(doubleBackPositionInTrail - 2);
                            }
                            else if (doubleBackPositionInTrail > 2)
                            {
                                if (trail[doubleBackPositionInTrail - 1].type == LocationType.Power && trail[doubleBackPositionInTrail - 2].type == LocationType.Power && trail[doubleBackPositionInTrail - 3].name == "Hide")
                                {
                                    Console.WriteLine("Dracula Doubled Back to a location where he previously used Hide");
                                    RevealHide(doubleBackPositionInTrail - 3);
                                }
                            }
                        }
                    }

                    // move location to the front of the trail
                    Location tempLocation = possibleDoubleBackMoves[doubleBackLocation];
                    trail.Remove(tempLocation);
                    trail.Insert(0, tempLocation);
                    currentLocation = trail[0];

                    // move the power cards that are in the trail
                    for (int i = 0; i < powers.Count(); i++)
                    {
                        if (powers[i].positionInTrail == doubleBackLocation)
                        {
                            powers[i].positionInTrail = 0;
                        }
                        else if (powers[i].positionInTrail < doubleBackLocation)
                        {
                            powers[i].positionInTrail++;
                        }
                    }

                }
                else if (possiblePowers[chosenActionIndex].name == "Wolf Form")
                {
                    // determine possible locations for wolf form (road only, up to two moves away)
                    determinePossibleWolfFormLocations();
                    // carry out move
                    moveByRoad();
                }

                // put the power used at the head of the trail
                possiblePowers[chosenActionIndex].positionInTrail = 0;
            }
            else
            {
                // choose a location
                
                //Console.WriteLine("Dracula is moving");
                moveByRoad();
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
            possibleMoves.Clear();
            possibleDoubleBackMoves.Clear();
            for (int i = 0; i < currentLocation.byRoad.Count(); i++)
            {
                if (currentLocation.byRoad[i].abbreviation != "STJ")
                {
                    possibleMoves.Add(currentLocation.byRoad[i]);
                }
            }

            for (int i = 0; i < currentLocation.bySea.Count(); i++)
            {
                possibleMoves.Add(currentLocation.bySea[i]);
            }

            for (int i = 0; i < trail.Count(); i++)
            {
                if (trail[i].name != "Dark Call" && trail[i].name != "Hide" && trail[i].name != "Feed")
                {
                    possibleDoubleBackMoves.Add(trail[i]);
                }
                possibleMoves.Remove(trail[i]);
            }
            possibleDoubleBackMoves.Remove(currentLocation);
        }

        public void determinePossibleWolfFormLocations()
        {
            possibleMoves.Clear();
            for (int i = 0; i < currentLocation.byRoad.Count(); i++)
            {
                possibleMoves.Add(currentLocation.byRoad[i]);
            }

            // extend every remaining location by road
            List<Location> extendedLocations = new List<Location>();
            for (int i = 0; i < possibleMoves.Count(); i++)
            {
                for (int j = 0; j < possibleMoves[i].byRoad.Count(); j++)
                {
                    if (!possibleMoves.Contains(possibleMoves[i].byRoad[j]) && !extendedLocations.Contains(possibleMoves[i].byRoad[j]))
                    {
                        extendedLocations.Add(possibleMoves[i].byRoad[j]);
                    }
                }

            }
            possibleMoves.AddRange(extendedLocations);
            possibleMoves.Remove(currentLocation);
            for (int i = 0; i < trail.Count(); i++)
            {
                possibleMoves.Remove(trail[i]);
            }

        }

        public void determinePossiblePowers(int timeOfDay)
        {
            possiblePowers.Clear();
            for (int i = 0; i < powers.Count(); i++)
            {
                if (powers[i].positionInTrail > 5 && (timeOfDay > 2 || powers[i].canBeUsedDuringDaylight))
                {
                    if (powers[i].name != "Double Back" || possibleDoubleBackMoves.Count() > 0)
                    {
                        possiblePowers.Add(powers[i]);
                    }
                }
            }
        }

        public void moveByRoad()
        {
            currentLocation = possibleMoves[new Random().Next(0, possibleMoves.Count())];

            trail.Insert(0, currentLocation);
            MovePowersAlongTrail();
            TrimTrail(6);
        }

        public void TrimTrail(int length)
        {
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
                trail.Remove(trail.Last());
            }
        }

        public void MovePowersAlongTrail()
        {
            for (int i = 0; i < powers.Count(); i++)
            {
                powers[i].positionInTrail++;
            }
        }

        public void RevealHide(int hideIndex)
        {
            // remove Hide from the trail
            trail.Remove(trail[hideIndex]);
            // move all power cards beyond that point up one position
            for (int i = 0; i < powers.Count(); i++)
            {
                if (powers[i].name == "Hide")
                {
                    powers[i].positionInTrail = 6;
                }
                else if (powers[i].positionInTrail > hideIndex && powers[i].positionInTrail < 6)
                {
                    powers[i].positionInTrail--;
                }
            }
            // set Hide position out of the trail

            Console.WriteLine("Dracula was hiding");
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
