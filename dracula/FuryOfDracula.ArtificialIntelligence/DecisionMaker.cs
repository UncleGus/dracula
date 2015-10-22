using FuryOfDracula.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.ArtificialIntelligence
{
    public class DecisionMaker
    {
        public Location ChooseDestination(GameState game)
        {
            Location destination;
            List<Location> possibleDestinations = game.Map.LocationsConnectedByRoadOrSeaTo(game.Dracula.CurrentLocation);
            do
            {
                destination = possibleDestinations[new Random().Next(0, possibleDestinations.Count())];
            } while (game.Map.TypeOfLocation(destination) == LocationType.Hospital);
            return destination;
        }

        public Location ChooseStartLocation(GameState game)
        {
            List<Location> allLocations = game.Map.GetAllLocations();
            Location startLocation;
            do
            {
                startLocation = allLocations[new Random().Next(0, allLocations.Count())];
            } while (game.Map.TypeOfLocation(startLocation) != LocationType.SmallCity && game.Map.TypeOfLocation(startLocation) != LocationType.LargeCity);
            return startLocation;
        }
    }
}
