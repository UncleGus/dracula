using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    [DataContract(IsReference = true)]
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
        [DataMember]
        public HunterPlayer[] Hunters { get; private set; }
        [DataMember]
        public Dracula Dracula { get; private set; }
        private List<EventCard> _eventDeck;
        [DataMember]
        public List<EventCard> EventDeck
        {
            get
            {
                if (_eventDeck == null)
                {
                    _eventDeck = CreateEventDeck();
                }
                return _eventDeck;

            }
            private set
            {
                _eventDeck = value;
            }
        }
        [DataMember]
        public List<EventCard> EventDiscard { get; private set; }
        private List<ItemCard> _itemDeck;
        [DataMember]
        public List<ItemCard> ItemDeck
        {
            get
            {
                if (_itemDeck == null)
                {
                    _itemDeck = CreateItemDeck();
                }
                return _itemDeck;
            }
            private set
            {
                _itemDeck = value;
            }
        }
        [DataMember]
        public List<ItemCard> ItemDiscard { get; private set; }
        private List<EncounterTile> _encounterPool;
        [DataMember]
        public List<EncounterTile> EncounterPool
        {
            get
            {
                if (_encounterPool == null)
                {
                    _encounterPool = CreateEncounterPool();
                }
                return _encounterPool;
            }
            private set
            {
                _encounterPool = value;
            }
        }
        [DataMember]
        public EventCard DraculaAlly { get; set; }
        [DataMember]
        public EventCard HunterAlly { get; set; }
        [DataMember]
        public TimeOfDay TimeOfDay { get; private set; }
        [DataMember]
        public int Resolve { get; private set; }
        [DataMember]
        public int Vampires { get; private set; }
        [DataMember]
        public Location HeavenlyHostLocation1 { get; set; }
        [DataMember]
        public Location HeavenlyHostLocation2 { get; set; }
        [DataMember]
        public Location ConsecratedGroundLocation{ get; set; }
        [DataMember]
        public Location StormySeasLocation { get; set; }
        [DataMember]
        public int StormySeasRounds { get; set; }
        [DataMember]
        public Location RoadBlockLocation1 { get; set; }
        [DataMember]
        public Location RoadBlockLocation2 { get; set; }
        [DataMember]
        public ConnectionType RoadBlockConnectionType { get; set; }

        public GameState()
        {
            Hunters = new HunterPlayer[5] { null, new HunterPlayer(Hunter.LordGodalming, 12, 0, 2), new HunterPlayer(Hunter.DrSeward, 8, 0, 2), new HunterPlayer(Hunter.VanHelsing, 10, 0, 3), new HunterPlayer(Hunter.MinaHarker, 8, 1, 2) };
            Dracula = new Dracula();
            ItemDiscard = new List<ItemCard>();
            EventDiscard = new List<EventCard>();
            Resolve = -1;
            Vampires = -1;
        }

        public List<Location> GetBlockedLocations()
        {
            List<Location> tempLocationList = new List<Location>();
            if (HeavenlyHostLocation1 != Location.Nowhere)
            {
                tempLocationList.Add(HeavenlyHostLocation1);
            }
            if (HeavenlyHostLocation2 != Location.Nowhere)
            {
                tempLocationList.Add(HeavenlyHostLocation2);
            }
            if (ConsecratedGroundLocation!= Location.Nowhere)
            {
                tempLocationList.Add(ConsecratedGroundLocation);
            }
            if (StormySeasRounds > 0 && StormySeasLocation != Location.Nowhere)
            {
                tempLocationList.Add(StormySeasLocation);
            }
            return tempLocationList;
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
            StormySeasRounds--;
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

        private List<ItemCard> CreateItemDeck()
        {
            List<ItemCard> tempItemDeck = new List<ItemCard>();
            tempItemDeck.Add(new ItemCard(Item.Crucifix));
            tempItemDeck.Add(new ItemCard(Item.Crucifix));
            tempItemDeck.Add(new ItemCard(Item.Crucifix));
            tempItemDeck.Add(new ItemCard(Item.Dogs));
            tempItemDeck.Add(new ItemCard(Item.Dogs));
            tempItemDeck.Add(new ItemCard(Item.FastHorse));
            tempItemDeck.Add(new ItemCard(Item.FastHorse));
            tempItemDeck.Add(new ItemCard(Item.FastHorse));
            tempItemDeck.Add(new ItemCard(Item.Garlic));
            tempItemDeck.Add(new ItemCard(Item.Garlic));
            tempItemDeck.Add(new ItemCard(Item.Garlic));
            tempItemDeck.Add(new ItemCard(Item.Garlic));
            tempItemDeck.Add(new ItemCard(Item.HeavenlyHost));
            tempItemDeck.Add(new ItemCard(Item.HeavenlyHost));
            tempItemDeck.Add(new ItemCard(Item.HolyWater));
            tempItemDeck.Add(new ItemCard(Item.HolyWater));
            tempItemDeck.Add(new ItemCard(Item.HolyWater));
            tempItemDeck.Add(new ItemCard(Item.Knife));
            tempItemDeck.Add(new ItemCard(Item.Knife));
            tempItemDeck.Add(new ItemCard(Item.Knife));
            tempItemDeck.Add(new ItemCard(Item.Knife));
            tempItemDeck.Add(new ItemCard(Item.Knife));
            tempItemDeck.Add(new ItemCard(Item.LocalRumors));
            tempItemDeck.Add(new ItemCard(Item.LocalRumors));
            tempItemDeck.Add(new ItemCard(Item.Pistol));
            tempItemDeck.Add(new ItemCard(Item.Pistol));
            tempItemDeck.Add(new ItemCard(Item.Pistol));
            tempItemDeck.Add(new ItemCard(Item.Pistol));
            tempItemDeck.Add(new ItemCard(Item.Pistol));
            tempItemDeck.Add(new ItemCard(Item.Rifle));
            tempItemDeck.Add(new ItemCard(Item.Rifle));
            tempItemDeck.Add(new ItemCard(Item.Rifle));
            tempItemDeck.Add(new ItemCard(Item.Rifle));
            tempItemDeck.Add(new ItemCard(Item.SacredBullets));
            tempItemDeck.Add(new ItemCard(Item.SacredBullets));
            tempItemDeck.Add(new ItemCard(Item.SacredBullets));
            tempItemDeck.Add(new ItemCard(Item.Stake));
            tempItemDeck.Add(new ItemCard(Item.Stake));
            tempItemDeck.Add(new ItemCard(Item.Stake));
            tempItemDeck.Add(new ItemCard(Item.Stake));
            return tempItemDeck;
        }

        private List<EventCard> CreateEventDeck()
        {
            List<EventCard> tempEventDeck = new List<EventCard>();
            tempEventDeck.Add(new EventCard(Event.AdvancePlanning, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.AdvancePlanning, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.AdvancePlanning, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.BloodTransfusion, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.CharteredCarriage, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.CharteredCarriage, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.CharteredCarriage, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.ConsecratedGround, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.ControlStorms, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.CustomsSearch, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.DevilishPower, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.DevilishPower, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.DraculasBrides, true, EventType.Ally));
            tempEventDeck.Add(new EventCard(Event.EscapeRoute, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.EscapeRoute, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.Evasion, true, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.ExcellentWeather, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.ExcellentWeather, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.FalseTipoff, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.FalseTipoff, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.Forewarned, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.Forewarned, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.Forewarned, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.GoodLuck, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.GoodLuck, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.GreatStrength, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.HeroicLeap, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.HiredScouts, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.HiredScouts, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.HiredScouts, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.Hypnosis, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.Hypnosis, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.ImmanuelHildesheim, true, EventType.Ally));
            tempEventDeck.Add(new EventCard(Event.JonathanHarker, false, EventType.Ally));
            tempEventDeck.Add(new EventCard(Event.LongDay, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.LongDay, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.MoneyTrail, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.MysticResearch, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.MysticResearch, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.NewspaperReports, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.NewspaperReports, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.NewspaperReports, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.NewspaperReports, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.NewspaperReports, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.NightVisit, true, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.QuinceyPMorris, true, EventType.Ally));
            tempEventDeck.Add(new EventCard(Event.Rage, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.ReEquip, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.ReEquip, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.ReEquip, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.RelentlessMinion, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.RelentlessMinion, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.Roadblock, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.RufusSmith, false, EventType.Ally));
            tempEventDeck.Add(new EventCard(Event.SecretWeapon, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.SecretWeapon, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.Seduction, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.SensationalistPress, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.SenseofEmergency, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.SenseofEmergency, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.SisterAgatha, false, EventType.Ally));
            tempEventDeck.Add(new EventCard(Event.StormySeas, false, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.SurprisingReturn, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.SurprisingReturn, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.TelegraphAhead, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.TelegraphAhead, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.TimeRunsShort, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.Trap, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.Trap, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.Trap, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.UnearthlySwiftness, true, EventType.Keep));
            tempEventDeck.Add(new EventCard(Event.VampireLair, false, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.VampiricInfluence, true, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.VampiricInfluence, true, EventType.PlayImmediately));
            tempEventDeck.Add(new EventCard(Event.WildHorses, true, EventType.Keep));
            return tempEventDeck;

        }

        private List<EncounterTile> CreateEncounterPool()
        {
            List<EncounterTile> tempEncounterDeck = new List<EncounterTile>();
            tempEncounterDeck.Add(new EncounterTile(Encounter.Ambush));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Ambush));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Ambush));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Assassin));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Bats));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Bats));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Bats));
            tempEncounterDeck.Add(new EncounterTile(Encounter.DesecratedSoil));
            tempEncounterDeck.Add(new EncounterTile(Encounter.DesecratedSoil));
            tempEncounterDeck.Add(new EncounterTile(Encounter.DesecratedSoil));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Fog));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Fog));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Fog));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Fog));
            tempEncounterDeck.Add(new EncounterTile(Encounter.MinionWithKnife, "MIK"));
            tempEncounterDeck.Add(new EncounterTile(Encounter.MinionWithKnife, "MIK"));
            tempEncounterDeck.Add(new EncounterTile(Encounter.MinionWithKnife, "MIK"));
            tempEncounterDeck.Add(new EncounterTile(Encounter.MinionWithKnifeAndPistol, "MIP"));
            tempEncounterDeck.Add(new EncounterTile(Encounter.MinionWithKnifeAndPistol, "MIP"));
            tempEncounterDeck.Add(new EncounterTile(Encounter.MinionWithKnifeAndRifle, "MIR"));
            tempEncounterDeck.Add(new EncounterTile(Encounter.MinionWithKnifeAndRifle, "MIR"));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Hoax));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Hoax));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Lightning));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Lightning));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Peasants));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Peasants));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Plague));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Rats));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Rats));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Saboteur));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Saboteur));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Spy));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Spy));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Thief));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Thief));
            tempEncounterDeck.Add(new EncounterTile(Encounter.NewVampire, "VAM"));
            tempEncounterDeck.Add(new EncounterTile(Encounter.NewVampire, "VAM"));
            tempEncounterDeck.Add(new EncounterTile(Encounter.NewVampire, "VAM"));
            tempEncounterDeck.Add(new EncounterTile(Encounter.NewVampire, "VAM"));
            tempEncounterDeck.Add(new EncounterTile(Encounter.NewVampire, "VAM"));
            tempEncounterDeck.Add(new EncounterTile(Encounter.NewVampire, "VAM"));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Wolves));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Wolves));
            tempEncounterDeck.Add(new EncounterTile(Encounter.Wolves));
            return tempEncounterDeck;

        }

        public void RegressTimeTracker()
        {
            TimeOfDay = TimeOfDay - 1;
        }

        private void SaveState(string fileName) {
            try
            {
                string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                foreach (char c in invalid)
                {
                    fileName = fileName.Replace(c.ToString(), "");
                }
                fileName = fileName + ".sav";
                DataContractSerializer fileWriter = new DataContractSerializer(typeof(GameState));
                FileStream writeStream = File.OpenWrite(fileName);

                fileWriter.WriteObject(writeStream, this);
                writeStream.Close();
            }
            catch 
            {
                throw;
            }
        }

        public void AdjustVampires(int adjustment)
        {
            Vampires += adjustment;
        }

        public void AdjustResolve(int adjustment)
        {
            Resolve += adjustment;
        }
    }
}
