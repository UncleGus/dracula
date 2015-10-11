using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationHandler;

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
    }
}
