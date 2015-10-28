using System.Collections.Generic;

namespace FuryOfDracula.GameLogic
{
    public class LocationDetail
    {
        public List<Location> ByRoad = new List<Location>();
        public List<Location> BySea = new List<Location>();
        public List<Location> ByTrain = new List<Location>();
        public bool IsEastern;
        public Location Location;
        public LocationType LocationType;

        public LocationDetail(Location location)
        {
            Location = location;
        }
    }
}