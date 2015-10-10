﻿using LocationHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterHandler
{
    public class Hunter
    {
        public string name;
        public int health;
        public Location currentLocation;
        public int numberOfEvents;
        public int numberOfItems;
        public int numberOfBites;
        public int bitesRequiredToKill;

        public Hunter(string newName, int newHealth, int newNumberOfBites, int newBitesRequiredToKill)
        {
            name = newName;
            health = newHealth;
            numberOfBites = newNumberOfBites;
            bitesRequiredToKill = newBitesRequiredToKill;
        }
    }

}
