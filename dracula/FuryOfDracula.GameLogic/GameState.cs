using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    [DataContract]
    public class GameState
    {
        private LocationSet _map;
        public LocationSet Map
        {
            get
            {
                if (_map == null)
                {
                    _map = new LocationSet();
                }
                return _map;
            }
            private set
            {
                _map = value;
            }
        }
        private ItemSet _items;
        public ItemSet Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ItemSet();
                }
                return _items;
            }
            private set
            {
                _items = value;
            }
        }
        private EventSet _events;
        public EventSet Events
        {
            get
            {
                if (_events == null)
                {
                    _events = new EventSet();
                }
                return _events;
            }
            private set
            {
                _events = value;
            }
        }
        private EncounterSet _encounters;
        public EncounterSet Encounters
        {
            get
            {
                if (_encounters == null)
                {
                    _encounters = new EncounterSet();
                }
                return _encounters;
            }
            private set
            {
                _encounters = value;
            }
        }
        private DraculaCardSet _draculaCardDeck;
        public DraculaCardSet DraculaCardDeck
        {
            get
            {
                if (_draculaCardDeck == null)
                {
                    _draculaCardDeck = new DraculaCardSet();
                }
                return _draculaCardDeck;
            }
            private set
            {
                _draculaCardDeck = value;
            }
        }
        [DataMember]
        public HunterPlayer[] Hunters { get; private set; }
        [DataMember]
        public Dracula Dracula { get; private set; }
        [DataMember]
        public List<Event> EventDeck { get; private set; }
        [DataMember]
        public List<Event> EventDiscard { get; private set; }
        [DataMember]
        public List<Item> ItemDeck { get; private set; }
        [DataMember]
        public List<Item> ItemDiscard { get; private set; }
        [DataMember]
        public List<Encounter> EncounterPool { get; private set; }
        [DataMember]
        public Event DraculaAlly { get; private set; }
        [DataMember]
        public Event HunterAlly { get; private set; }
        [DataMember]
        public TimeOfDay TimeOfDay { get; private set; }
        [DataMember]
        public int Resolve { get; private set; }
        [DataMember]
        public int Vampires { get; private set; }

        public GameState()
        {
            Hunters = new HunterPlayer[5] {null, new HunterPlayer(Hunter.LordGodalming, 12, 0, 2), new HunterPlayer(Hunter.DrSeward, 8, 0, 2), new HunterPlayer(Hunter.VanHelsing, 10, 0, 3), new HunterPlayer(Hunter.MinaHarker, 8, 1, 2) };
            Dracula = new Dracula();
            ItemDeck = new List<Item>();
            ItemDiscard = new List<Item>();
            EventDeck = new List<Event>();
            EventDiscard = new List<Event>();
            EncounterPool = Encounters.GetAllEncounters();
            for (int i = 1; i < 41; i++)
            {
                ItemDeck.Add((Item)i);
            }
            for (int i = 1; i < 76; i++)
            {
                EventDeck.Add((Event)i);
            }
            Resolve = 0;
            Vampires = 0;
        }

        public Hunter GetHunterFromString(string hunterName)
        {
            for (int i = 1; i < 5; i++)
            {
                if (Hunters[i].Hunter.Name().ToLower().StartsWith(hunterName.ToLower()))
                {
                    return Hunters[i].Hunter;
                }
            }
            return Hunter.Nobody;
        }

        public Hunter GetHunterFromInt(int index)
        {
            try
            {
                return Hunters[index].Hunter;
            }
            catch (IndexOutOfRangeException)
            {
                return Hunter.Nobody;
            }
        }

        public void AdvanceTimeTracker()
        {
            if (Map.TypeOfLocation(Dracula.CurrentLocation) != LocationType.Sea)
            {

                TimeOfDay = (TimeOfDay)((int)TimeOfDay % 6 + 1);
                if (TimeOfDay == TimeOfDay.Dawn)
                {
                    Vampires++;
                    Resolve++;
                }
            }
        }
    }
}
