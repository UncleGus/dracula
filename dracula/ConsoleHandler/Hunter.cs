using DraculaSimulator;
using EventHandler;
using LocationHandler;
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
        public bool hasDogsFaceUp;
        public List<Hunter> huntersInGroup = new List<Hunter>();
        public string lastItemUsedInCombat;
        public List<Event> eventsKnownToDracula = new List<Event>();
        public List<Item> itemsKnownToDracula = new List<Item>();
        public string travelType = null;
        public Location destination = null;
        public Item itemShownToDraculaForBeingBitten = null;
        public Event eventShownToDraculaForBeingBitten = null;

        public Hunter(string newName, int newHealth, int newNumberOfBites, int newBitesRequiredToKill)
        {
            name = newName;
            health = newHealth;
            numberOfBites = newNumberOfBites;
            bitesRequiredToKill = newBitesRequiredToKill;
            hasDogsFaceUp = false;
            huntersInGroup.Add(this);
        }
    }

}
