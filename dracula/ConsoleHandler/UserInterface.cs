using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationHandler;

namespace ConsoleHandler
{
    class UserInterface
    {
        internal void TellUser(string v)
        {
            Console.WriteLine(v);
        }

        internal Location GetHunterStartLocation(GameState g, int v)
        {
            string line;
            Location hunterStartLocation;
            do
            {
                Console.WriteLine("Where is " + g.NameOfHunterAtIndex(v) + "?");
                line = Console.ReadLine();
                hunterStartLocation = g.GetLocationFromName(line);
                Console.WriteLine(hunterStartLocation.name);
            } while (hunterStartLocation.name == "Unknown location");
            return hunterStartLocation;
        }
    }
}
