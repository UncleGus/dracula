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
        public bool isRevealed;

        public Location()
        {
            byRoad = new List<Location>();
            byTrain = new List<Location>();
            bySea = new List<Location>();
            isRevealed = false;
        }

        public bool IsPort()
        {
            return bySea.Count() > 0 && (type == LocationType.Town || type == LocationType.City);
        }

        public void DrawLocation()
        {
            if (type == LocationType.Sea)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else if (type == LocationType.Power)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            } else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            if (isRevealed)
            {
                Console.Write(abbreviation + " ");
            }
            else
            {
                Console.Write("### ");
            }
            Console.ResetColor();
        }
    }

    public enum LocationType
    {
        Town,
        City,
        Sea,
        Castle,
        Hospital,
        Power
    }

    public static class LocationHelper
    {
        public static void ShowLocationDetails(Location location)
        {
            Console.WriteLine("Name: " + location.name + " (" + location.abbreviation + ")");
            Console.WriteLine("Type: " + location.type);
            if (location.type != LocationType.Sea)
            {
                Console.WriteLine("Realm: " + (location.isEastern ? "East" : "West"));
            }
            if (location.byRoad.Count() > 0)
            {
                Console.WriteLine("Connected by road to:");
                for (int i = 0; i < location.byRoad.Count(); i++)
                {
                    Console.WriteLine("    " + location.byRoad[i].name);
                }
            }
            if (location.byTrain.Count() > 0)
            {

                Console.WriteLine("Connected by train to:");
                for (int i = 0; i < location.byTrain.Count(); i++)
                {
                    Console.WriteLine("    " + location.byTrain[i].name);
                }
            }
            if (location.bySea.Count() > 0)
            {

                Console.WriteLine("Connected by sea to:");
                for (int i = 0; i < location.bySea.Count(); i++)
                {
                    Console.WriteLine("    " + location.bySea[i].name);
                }
            }
        }

        public static void RevealLocation(List<Location> trail, int trailIndex)
        {
            try
            {
                trail[trailIndex].isRevealed = true;
            } catch (ArgumentOutOfRangeException)
            { }
        }

    }
}
