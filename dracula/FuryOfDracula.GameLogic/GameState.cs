﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;

namespace FuryOfDracula.GameLogic
{
    [DataContract(IsReference = true)]
    public class GameState
    {
        private List<EncounterTile> _encounterPool;
        private List<EventCard> _eventDeck;
        private List<ItemCard> _itemDeck;
        private LocationSet _map;

        public GameState()
        {
            Hunters = new HunterPlayer[5]
            {
                null, new HunterPlayer(Hunter.LordGodalming, 12, 0, 2), new HunterPlayer(Hunter.DrSeward, 10, 0, 2),
                new HunterPlayer(Hunter.VanHelsing, 8, 0, 3), new HunterPlayer(Hunter.MinaHarker, 8, 1, 2)
            };
            Dracula = new Dracula();
            ItemDiscard = new List<ItemCard>();
            EventDiscard = new List<EventCard>();
            Resolve = -1;
            Vampires = -1;
        }

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
            private set { _map = value; }
        }

        [DataMember]
        public HunterPlayer[] Hunters { get; private set; }

        [DataMember]
        public Dracula Dracula { get; private set; }

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
            private set { _eventDeck = value; }
        }

        [DataMember]
        public List<EventCard> EventDiscard { get; private set; }

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
            private set { _itemDeck = value; }
        }

        [DataMember]
        public List<ItemCard> ItemDiscard { get; private set; }

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
            private set { _encounterPool = value; }
        }

        [DataMember]
        public EventCard DraculaAlly { get; set; }

        [DataMember]
        public EventCard HunterAlly { get; set; }

        public int GetDistanceToClosestHunter(Location location, bool includeHospital)
        {
            int minDistance = DistanceByRoadOrSeaBetween(location, Hunters[(int)Hunter.LordGodalming].CurrentLocation, includeHospital);
            minDistance = Math.Min(minDistance, DistanceByRoadOrSeaBetween(location, Hunters[(int)Hunter.DrSeward].CurrentLocation, includeHospital));
            minDistance = Math.Min(minDistance, DistanceByRoadOrSeaBetween(location, Hunters[(int)Hunter.VanHelsing].CurrentLocation, includeHospital));
            minDistance = Math.Min(minDistance, DistanceByRoadOrSeaBetween(location, Hunters[(int)Hunter.MinaHarker].CurrentLocation, includeHospital));
            GC.Collect();
            return minDistance;
        }

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
        public Location ConsecratedGroundLocation { get; set; }

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

        public List<Location> GetBlockedLocations()
        {
            var tempLocationList = new List<Location>();
            if (HeavenlyHostLocation1 != Location.Nowhere)
            {
                tempLocationList.Add(HeavenlyHostLocation1);
            }
            if (HeavenlyHostLocation2 != Location.Nowhere)
            {
                tempLocationList.Add(HeavenlyHostLocation2);
            }
            if (ConsecratedGroundLocation != Location.Nowhere)
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
            for (var i = 1; i < 5; i++)
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
            var tempItemDeck = new List<ItemCard>();
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
            var tempEventDeck = new List<EventCard>();
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
            var tempEncounterDeck = new List<EncounterTile>();
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

        private void SaveState(string fileName)
        {
            try
            {
                var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                foreach (var c in invalid)
                {
                    fileName = fileName.Replace(c.ToString(), "");
                }
                fileName = fileName + ".sav";
                var fileWriter = new DataContractSerializer(typeof(GameState));
                var writeStream = File.OpenWrite(fileName);

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

        public void RecycleItemDiscard()
        {
            ItemDeck = ItemDiscard;
            ItemDiscard = new List<ItemCard>();
        }

        public void RecycleEventDiscard()
        {
            EventDeck = EventDiscard;
            EventDiscard = new List<EventCard>();
        }

        public List<Hunter> HuntersAt(Location location)
        {
            List<Hunter> huntersAtLocation = new List<Hunter>();
            for (int i = 1; i < 5; i++)
            {
                if (Hunters[i].CurrentLocation == location)
                {
                    huntersAtLocation.Add(Hunters[i].Hunter);
                }
            }
            return huntersAtLocation;
        }

        public bool LocationIsBlocked(Location location)
        {
            if (HeavenlyHostLocation1 == location || HeavenlyHostLocation2 == location || ConsecratedGroundLocation == location || (StormySeasLocation == location && StormySeasRounds > 0) || Map.TypeOfLocation(location) == LocationType.Hospital)
            {
                return true;
            }
            return false;
        }

        public bool CatacombsContainsLocation(Location loc)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Dracula.Catacombs[i] != null && Dracula.Catacombs[i].DraculaCards.First().Location == loc)
                {
                    return true;
                }
            }
            return false;
        }

        private int DistanceByRoadOrSeaBetweenRecursive(List<Location> searchSpace, int searchIndex, Location destination, int distance, bool includeHospital)
        {
            int stopIndex = searchSpace.Count();
            if (searchIndex == stopIndex)
            {
                return 99;
            }
            for (int i = searchIndex; i < stopIndex; i++)
            {
                if (searchSpace[i] == destination)
                {
                    return distance;
                }
                else
                {
                    List<Location> newConnectedLocations = Map.LocationsConnectedByRoadOrSeaTo(searchSpace[i]);
                    foreach (Location location in newConnectedLocations)
                    {
                        if (!searchSpace.Contains(location) && (includeHospital || Map.TypeOfLocation(location) != LocationType.Hospital))
                        {
                            searchSpace.Add(location);
                        }
                    }
                }
            }
            return DistanceByRoadOrSeaBetweenRecursive(searchSpace, stopIndex, destination, distance + 1, includeHospital);
        }

        private int DistanceByRoadBetweenRecursive(List<Location> searchSpace, int searchIndex, Location destination, int distance, bool includeHospital)
        {
            int stopIndex = searchSpace.Count();
            if (searchIndex == stopIndex)
            {
                return 99;
            }
            for (int i = searchIndex; i < stopIndex; i++)
            {
                if (searchSpace[i] == destination)
                {
                    return distance;
                }
                else
                {
                    List<Location> newConnectedLocations = Map.LocationsConnectedByRoadTo(searchSpace[i]);
                    foreach (Location location in newConnectedLocations)
                    {
                        if (!searchSpace.Contains(location) && (includeHospital || Map.TypeOfLocation(location) != LocationType.Hospital))
                        {
                            searchSpace.Add(location);
                        }
                    }
                }
            }
            return DistanceByRoadBetweenRecursive(searchSpace, stopIndex, destination, distance + 1, includeHospital);
        }

        public int DistanceByRoadBetween(Location start, Location finish, bool includeHospital)
        {
            List<Location> searchSpace = new List<Location>();
            searchSpace.Add(start);
            return DistanceByRoadBetweenRecursive(searchSpace, 0, finish, 0, includeHospital);
        }

        public int DistanceByTrainBetween(Location start, Location finish, bool includeHospital)
        {
            List<Location> searchSpace = new List<Location>();
            searchSpace.Add(start);
            return DistanceByTrainBetweenRecursive(searchSpace, 0, finish, 0, includeHospital);
        }

        public int DistanceByTrainBetweenRecursive(List<Location> searchSpace, int searchIndex, Location destination, int distance, bool includeHospital)
        {
            int stopIndex = searchSpace.Count();
            if (searchIndex == stopIndex)
            {
                return 99;
            }
            for (int i = searchIndex; i < stopIndex; i++)
            {
                if (searchSpace[i] == destination)
                {
                    return distance;
                }
                else
                {
                    List<Location> newConnectedLocations = Map.LocationsConnectedByTrainTo(searchSpace[i]);
                    foreach (Location location in newConnectedLocations)
                    {
                        if (!searchSpace.Contains(location) && (includeHospital || Map.TypeOfLocation(location) != LocationType.Hospital))
                        {
                            searchSpace.Add(location);
                        }
                    }
                }
            }
            return DistanceByTrainBetweenRecursive(searchSpace, stopIndex, destination, distance + 1, includeHospital);
        }

        public int DistanceByRoadOrSeaBetween(Location start, Location finish, bool includeHospital)
        {
            List<Location> searchSpace = new List<Location>();
            searchSpace.Add(start);
            return DistanceByRoadOrSeaBetweenRecursive(searchSpace, 0, finish, 0, includeHospital);
        }

        public List<HunterPlayer> HuntersClosestTo(Location location)
        {
            List<HunterPlayer> closestHunters = new List<HunterPlayer>();
            int shortestDistance = GetDistanceToClosestHunter(location, false);
            foreach (HunterPlayer h in Hunters)
            {
                if (h != null && DistanceByRoadOrSeaBetween(location, h.CurrentLocation, false) == shortestDistance)
                {
                    closestHunters.Add(h);
                }
            }
            return closestHunters;
        }

        public int NumberOfItemCardsInFullDeck
        {
            get
            {
                return 40;
            }
        }

        internal int NumberOfItemsOfType(Item item)
        {
            switch (item)
            {
                case Item.Crucifix: return 3;
                case Item.Dogs: return 2;
                case Item.FastHorse: return 3;
                case Item.Garlic: return 4;
                case Item.HeavenlyHost: return 2;
                case Item.HolyWater: return 3;
                case Item.Knife: return 5;
                case Item.LocalRumors: return 2;
                case Item.Pistol: return 5;
                case Item.Rifle: return 4;
                case Item.SacredBullets: return 3;
                case Item.Stake: return 4;
                default: return 0;
            }
        }

        internal int NumberOfRevealedItemsOfType(Item item)
        {
            int count = Hunters[(int)Hunter.LordGodalming].NumberOfKnownItemsOfType(item) + Hunters[(int)Hunter.DrSeward].NumberOfKnownItemsOfType(item) + Hunters[(int)Hunter.VanHelsing].NumberOfKnownItemsOfType(item) + Hunters[(int)Hunter.MinaHarker].NumberOfKnownItemsOfType(item) + ItemDiscard.Count(card => card.Item == item);
            return count;
        }

        public float CombatWorthOfItem(Item item)
        {
            switch (item)
            {
                case Item.Crucifix: return 6F;
                case Item.HeavenlyHost: return 1.75F;
                case Item.HolyWater: return 1F;
                case Item.Knife: return 2F;
                case Item.Pistol: return 1.5F;
                case Item.Rifle: return 3F;
                case Item.SacredBullets: return 1.25F;
                case Item.Stake: return 10F;
                default: return 0F;
            }
        }

        public float IndividualCombatScore(HunterPlayer hunter)
        {
            float individualCombatScore = 0;
            for (int i = 1; i < 13; i++)
            {
                individualCombatScore += hunter.LikelihoodOfHavingItemOfType(this, (Item)i) * CombatWorthOfItem((Item)i);
            }
            switch (hunter.BitesRequiredToKill - hunter.BiteCount)
            {
                case 1: individualCombatScore -= 8; break;
                case 3: individualCombatScore += 4; break;
            }
            return individualCombatScore;
        }

        public int GetDistanceToHunter(HunterPlayer victim)
        {
            return DistanceByRoadOrSeaBetween(Dracula.CurrentLocation, victim.CurrentLocation, false);
        }

        public List<HunterPlayer> HuntersWithinDistanceOf(Location location, int turns)
        {
            List<HunterPlayer> huntersWithinRange = new List<HunterPlayer>();
            for (int i = 1; i < 5; i++)
            {
                if (DistanceByRoadOrSeaBetween(location, Hunters[i].CurrentLocation, false) <= turns)
                {
                    huntersWithinRange.Add(Hunters[i]);
                }
            }
            return huntersWithinRange;
        }

        public float LikelihoodOfAnyHunterHavingEventOfType(Event ev)
        {
            float likelihoodOfNotHavingEvent = 1;
            for (int i = 1; i < 5; i++)
            {
                likelihoodOfNotHavingEvent *= 1 - Hunters[i].LikelihoodOfHavingEventOfType(this, ev);
            }
            return 1 - likelihoodOfNotHavingEvent;
        }

        internal int NumberOfEventsOfType(Event ev)
        {
            switch (ev)
            {
                case Event.AdvancePlanning: return 3;
                case Event.BloodTransfusion: return 1;
                case Event.CharteredCarriage: return 3;
                case Event.ConsecratedGround: return 1;
                case Event.ControlStorms: return 1;
                case Event.DevilishPower: return 2;
                case Event.DraculasBrides: return 1;
                case Event.EscapeRoute: return 2;
                case Event.Evasion: return 1;
                case Event.ExcellentWeather: return 2;
                case Event.FalseTipoff: return 2;
                case Event.Forewarned: return 3;
                case Event.GoodLuck: return 2;
                case Event.GreatStrength: return 1;
                case Event.HeroicLeap: return 1;
                case Event.HiredScouts: return 3;
                case Event.Hypnosis: return 2;
                case Event.ImmanuelHildesheim: return 1;
                case Event.JonathanHarker: return 1;
                case Event.LongDay: return 2;
                case Event.MoneyTrail: return 1;
                case Event.MysticResearch: return 2;
                case Event.NewspaperReports: return 5;
                case Event.NightVisit: return 1;
                case Event.QuinceyPMorris: return 1;
                case Event.Rage: return 1;
                case Event.ReEquip: return 3;
                case Event.RelentlessMinion: return 2;
                case Event.Roadblock: return 1;
                case Event.RufusSmith: return 1;
                case Event.SecretWeapon: return 2;
                case Event.Seduction: return 1;
                case Event.SensationalistPress: return 1;
                case Event.SenseofEmergency: return 2;
                case Event.TelegraphAhead: return 2;
                case Event.TimeRunsShort: return 1;
                case Event.Trap: return 3;
                case Event.UnearthlySwiftness: return 1;
                case Event.VampireLair: return 1;
                case Event.VampiricInfluence: return 2;
                case Event.WildHorses: return 1;
                default: return 0;
            }
        }

        internal int NumberOfRevealedEventsOfType(Event ev)
        {
            int count = Hunters[(int)Hunter.LordGodalming].NumberOfKnownEventsOfType(ev) + Hunters[(int)Hunter.DrSeward].NumberOfKnownEventsOfType(ev) + Hunters[(int)Hunter.VanHelsing].NumberOfKnownEventsOfType(ev) + Hunters[(int)Hunter.MinaHarker].NumberOfKnownEventsOfType(ev) + EventDiscard.Count(card => card.Event == ev);
            return count;
        }

        public Location OldestUnrevealedLocationInTrail()
        {
            for (int i = 5; i >= 0; i--) {
                if (Dracula.Trail[i] != null && Dracula.Trail[i].DraculaCards.First().Location != Location.Nowhere && Dracula.Trail[i].DraculaCards.First().IsRevealed)
                {
                    return Dracula.Trail[i].DraculaCards.First().Location;
                }
            }
            return Location.Nowhere;
        }
    }
}