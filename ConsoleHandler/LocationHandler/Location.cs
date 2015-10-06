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
        public string abbreviation;
        public LocationType type;
        public bool isEastern;
        public List<Location> byRoad;
        public List<Location> byTrain;
        public List<Location> bySea;

        public Location()
        {
            byRoad = new List<Location>();
            byTrain = new List<Location>();
            bySea = new List<Location>();
        }

        public bool IsPort()
        {
            return bySea.Count() > 0;
        }
    }

    public enum LocationType
    {
        Town,
        City,
        Sea,
        Castle,
        Hospital
    }

    public static class LocationHelper
    {
        public static void ShowLocationDetails(Location location)
        {
            Console.WriteLine("Name: " + location.name + " (" + location.abbreviation + ")");
            Console.WriteLine("\nType: " + location.type);
            Console.WriteLine("\nRealm: " + (location.isEastern ? "East" : "West"));
            Console.WriteLine("\nConnected by road to:");
            for (int i = 0; i < location.byRoad.Count(); i++ )
            {
                Console.WriteLine("\n    " + location.byRoad[i].name);
            }
            Console.WriteLine("\nConnected by train to:");
            for (int i = 0; i < location.byTrain.Count(); i++)
            {
                Console.WriteLine("\n    " + location.byTrain[i].name);
            }
            Console.WriteLine("\nConnected by sea to:");
            for (int i = 0; i < location.bySea.Count(); i++)
            {
                Console.WriteLine("\n    " + location.bySea[i].name);
            }
        }

    }
}
