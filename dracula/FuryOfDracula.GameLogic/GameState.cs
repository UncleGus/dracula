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
        [DataMember]
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
        }
        [DataMember]
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
        }
        [DataMember]
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
        }
        [DataMember]
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
        }
        [DataMember]
        public HunterPlayer[] Hunters;
        [DataMember]
        public Dracula Dracula;
        [DataMember]
        public List<Event> EventDeck;
        [DataMember]
        public List<Event> EventDiscard;
        [DataMember]
        public List<Item> ItemDeck;
        [DataMember]
        public List<Item> ItemDiscard;
        [DataMember]
        public List<Encounter> EncounterPool;

        public GameState()
        {
            Hunters = new HunterPlayer[5] { new HunterPlayer(Hunter.Nobody, 0, 0, 0), new HunterPlayer(Hunter.LordGodalming, 12, 0, 2), new HunterPlayer(Hunter.DrSeward, 8, 0, 2), new HunterPlayer(Hunter.VanHelsing, 10, 0, 3), new HunterPlayer(Hunter.MinaHarker, 8, 1, 2) };
            Dracula = new Dracula();
            for (int i = 1; i < 41; i++ )
            {
                ItemDeck.Add((Item)i);
            }
            for (int i = 1; i < 76; i++)
            {
                EventDeck.Add((Event)i);
            }
            for (int i = 1; i < 46; i++)
            {
                EncounterPool.Add((Encounter)i);
            }
        }
    }
}
