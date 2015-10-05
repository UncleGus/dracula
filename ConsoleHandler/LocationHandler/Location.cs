using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationHandler
{
    public class Location
    {
        public string name;
        public LocationType type;
        public bool isEastern;
        public Location[] byRoad;
        public Location[] byTrain;
        public Location[] bySea;

    }

    public enum LocationType
    {
        Town,
        TownPort,
        City,
        CityPort,
        Sea,
        Castle,
        Hospital
    }

}
