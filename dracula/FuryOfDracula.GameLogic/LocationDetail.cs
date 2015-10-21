using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    public class LocationDetail
    {
        public Location Location;
        public LocationType LocationType;
        public bool IsEastern;
        public List<Location> ByRoad = new List<Location>();
        public List<Location> ByTrain = new List<Location>();
        public List<Location> BySea = new List<Location>();

        public LocationDetail(Location location)
        {
            Location = location;
        }
    }
}

