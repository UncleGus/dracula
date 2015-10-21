using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DraculaSimulator
{
    [DataContract]
    public class GameState
    {
        [DataMember]
        public Dracula Dracula { get; set; }
        [DataMember]
        public Hunter[] Hunters { get; set; }
        [DataMember]
        public List<Encounter> EncounterPool { get; set; }
        [DataMember]
        public List<Encounter> EncounterLimbo { get; set; }

        internal void SetHunterStartLocations(Map map, UserInterface ui)
        {
            foreach (Hunter h in Hunters)
            {
                string line;
                do
                {
                    line = ui.GetHunterStartLocation(h.Name);
                    ui.TellUser(map.LocationName(map.GetLocationFromString(line)));
                } while (map.GetLocationFromString(line) == Location.Nowhere);
            }
        }

        [DataMember]
        public List<Event> EventDiscard { get; set; }
        [DataMember]
        public List<Item> ItemDiscard { get; set; }
        [DataMember]
        public EventDetail DraculaAlly { get; set; }
        [DataMember]
        public EventDetail HunterAlly { get; set; }
        [DataMember]
        public int TimeIndex { get; set; }
        public string[] TimesOfDay = new string[6] { "Dawn", "Noon", "Dusk", "Twilight", "Midnight", "Small Hours" };
        [DataMember]
        public int Resolve { get; set; }
        [DataMember]
        public int VampirePointTracker { get; set; }
        [DataMember]
        public Location HeavenlyHostLocationA { get; set; }
        [DataMember]
        public Location HeavenlyHostLocationB { get; set; }
        [DataMember]
        public List<Location> RevealedLocations { get; set; }
        [DataMember]
        public List<Encounter> RevealedEncounters { get; set; }
        [DataMember]
        public Location RoadblockLocation1 { get; set; }
        [DataMember]
        public Location RoadblockLocation2 { get; set; }
        [DataMember]
        public LocationType RoadblockType { get; set; }

        public GameState()
        {
            Hunters = new Hunter[4];
            Hunters[(int)HunterName.LordGodalming] = new Hunter((int)HunterName.LordGodalming, "Lord Godalming", 12, 0, 2);
            Hunters[(int)HunterName.DrSeward] = new Hunter((int)HunterName.DrSeward, "Dr. Seward", 10, 0, 2);
            Hunters[(int)HunterName.VanHelsing] = new Hunter((int)HunterName.VanHelsing, "Van Helsing", 8, 0, 3);
            Hunters[(int)HunterName.MinaHarker] = new Hunter((int)HunterName.MinaHarker, "Mina Harker", 8, 1, 2);

            Resolve = -1;
            VampirePointTracker = -1;

            TimeIndex = -1;

            RevealedLocations = new List<Location>();
            EncounterLimbo = new List<Encounter>();
            RevealedEncounters = new List<Encounter>();
            EventDiscard = new List<Event>();
            ItemDiscard = new List<Item>();
            Dracula = new Dracula();
        }

        //private void SetUpItems()
        //{
        //    ItemDeck.Add(new ItemDetail("Crucifix"));
        //    ItemDeck.Add(new ItemDetail("Crucifix"));
        //    ItemDeck.Add(new ItemDetail("Crucifix"));
        //    ItemDeck.Add(new ItemDetail("Dogs"));
        //    ItemDeck.Add(new ItemDetail("Dogs"));
        //    ItemDeck.Add(new ItemDetail("Fast Horse"));
        //    ItemDeck.Add(new ItemDetail("Fast Horse"));
        //    ItemDeck.Add(new ItemDetail("Fast Horse"));
        //    ItemDeck.Add(new ItemDetail("Garlic"));
        //    ItemDeck.Add(new ItemDetail("Garlic"));
        //    ItemDeck.Add(new ItemDetail("Garlic"));
        //    ItemDeck.Add(new ItemDetail("Garlic"));
        //    ItemDeck.Add(new ItemDetail("Heavenly Host"));
        //    ItemDeck.Add(new ItemDetail("Heavenly Host"));
        //    ItemDeck.Add(new ItemDetail("Holy Water"));
        //    ItemDeck.Add(new ItemDetail("Holy Water"));
        //    ItemDeck.Add(new ItemDetail("Holy Water"));
        //    ItemDeck.Add(new ItemDetail("Knife"));
        //    ItemDeck.Add(new ItemDetail("Knife"));
        //    ItemDeck.Add(new ItemDetail("Knife"));
        //    ItemDeck.Add(new ItemDetail("Knife"));
        //    ItemDeck.Add(new ItemDetail("Knife"));
        //    ItemDeck.Add(new ItemDetail("Local Rumours"));
        //    ItemDeck.Add(new ItemDetail("Local Rumours"));
        //    ItemDeck.Add(new ItemDetail("Pistol"));
        //    ItemDeck.Add(new ItemDetail("Pistol"));
        //    ItemDeck.Add(new ItemDetail("Pistol"));
        //    ItemDeck.Add(new ItemDetail("Pistol"));
        //    ItemDeck.Add(new ItemDetail("Pistol"));
        //    ItemDeck.Add(new ItemDetail("Sacred Bullets"));
        //    ItemDeck.Add(new ItemDetail("Sacred Bullets"));
        //    ItemDeck.Add(new ItemDetail("Sacred Bullets"));
        //    ItemDeck.Add(new ItemDetail("Stake"));
        //    ItemDeck.Add(new ItemDetail("Stake"));
        //    ItemDeck.Add(new ItemDetail("Stake"));
        //    ItemDeck.Add(new ItemDetail("Stake"));
        //    ItemDeck.Add(new ItemDetail("Rifle"));
        //    ItemDeck.Add(new ItemDetail("Rifle"));
        //    ItemDeck.Add(new ItemDetail("Rifle"));
        //    ItemDeck.Add(new ItemDetail("Rifle"));
        //}

        //private void SetUpEvents()
        //{
        //    EventDeck.Add(new EventDetail("Rufus Smith", false, EventType.Ally));
        //    EventDeck.Add(new EventDetail("Jonathan Harker", false, EventType.Ally));
        //    EventDeck.Add(new EventDetail("Sister Agatha", false, EventType.Ally));
        //    EventDeck.Add(new EventDetail("Heroic Leap", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Great Strength", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Money Trail", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Sense of Emergency", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Sense of Emergency", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Vampire Lair", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Long Day", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Long Day", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Mystic Research", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Mystic Research", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Advance Planning", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Advance Planning", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Advance Planning", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Newspaper Reports", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Newspaper Reports", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Newspaper Reports", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Newspaper Reports", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Newspaper Reports", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Re-Equip", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Re-Equip", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Re-Equip", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Consecrated Ground", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Telegraph Ahead", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Telegraph Ahead", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Hypnosis", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Hypnosis", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Stormy Seas", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Surprising Return", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Surprising Return", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Good Luck", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Good Luck", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Blood Transfusion", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Secret Weapon", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Secret Weapon", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Forewarned", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Forewarned", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Forewarned", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Chartered Carriage", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Chartered Carriage", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Chartered Carriage", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Excellent Weather", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Excellent Weather", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Escape Route", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Escape Route", false, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Hired Scouts", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Hired Scouts", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Hired Scouts", false, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Dracula's Brides", true, EventType.Ally));
        //    EventDeck.Add(new EventDetail("Immanuel Hildesheim", true, EventType.Ally));
        //    EventDeck.Add(new EventDetail("Quincey P. Morris", true, EventType.Ally));
        //    EventDeck.Add(new EventDetail("Roadblock", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Unearthly Swiftness", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Time Runs Short", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Customs Search", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Devilish Power", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Devilish Power", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Vampiric Influence", true, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Vampiric Influence", true, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Night Visit", true, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Evasion", true, EventType.PlayImmediately));
        //    EventDeck.Add(new EventDetail("Wild Horses", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("False Tip-off", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("False Tip-off", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Sensationalist Press", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Rage", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Seduction", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Control Storms", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Relentless Minion", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Relentless Minion", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Trap", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Trap", true, EventType.Keep));
        //    EventDeck.Add(new EventDetail("Trap", true, EventType.Keep));
        //}

        // remove
        //internal void MoveEventFromEventDeckToKnownEvents(Hunter hunterToInfluence, Event e)
        //{
        //    EventDeck.Remove(e);
        //    hunterToInfluence.EventsKnownToDracula.Add(e);
        //}

        // remove
        //internal void MoveItemFromItemDeckToKnownItems(Hunter hunterToInfluence, Item item)
        //{
        //    ItemDeck.Remove(item);
        //    hunterToInfluence.ItemsKnownToDracula.Add(item);
        //}

        // remove
        //internal void MoveEventFromKnownEventsToEventDeck(Hunter hunterToInfluence, Event e)
        //{
        //    EventDeck.Add(e);
        //    hunterToInfluence.EventsKnownToDracula.Remove(e);
        //}

        // remove
        //internal void MoveItemFromKnownItemsToItemDeck(Hunter hunterToInfluence, Item item)
        //{
        //    ItemDeck.Add(item);
        //    hunterToInfluence.ItemsKnownToDracula.Remove(item);
        //}

        // remove
        //internal void GetHunterGroupMemberNamesAtHunterIndex(int hunterIndex, string[] names)
        //{
        //    for (int i = 0; i < Hunters[hunterIndex].HuntersInGroup.Count(); i++)
        //    {
        //        names[i] = Hunters[hunterIndex].HuntersInGroup[i].Name;
        //    }
        //}

        internal void SearchWithHunter(Hunter hunter, Map map, Location location, UserInterface ui)
        {
            if ((Dracula.LocationIsInTrail(location) || Dracula.LocationIsInCatacombs(location)) && map.TypeOfLocation(location) != LocationType.Sea)
            {
                Logger.WriteToDebugLog("Hunter moved to a location that Dracula has visited");
                if (location == Dracula.CurrentLocation)
                {
                    ui.TellUser("Dracula is here!");
                }
                else
                {
                    ui.TellUser("Search reveals evidence of Dracula's visit");
                }
                RevealedLocations.Add(location);
                bool canFightDracula = ResolveEncountersAtLocation(hunter, map, location, ui);
                if (location == Dracula.CurrentLocation && canFightDracula)
                {
                    ResolveCombat(hunter, 1, true, ui);
                }
            }
            else if (location.Type != LocationType.Hospital && location.Type != LocationType.Sea && location.Type != LocationType.Castle)
            {
                ui.TellUser("Search reveals nothing in " + location.Name);
            }
        }

        private bool ResolveEncountersAtLocation(Hunter hunter, Map map, Location location, UserInterface ui)
        {
            Dracula.OrderEncounters(hunter, location);
            foreach (EncounterDetail enc in map.LocationDetails(location).Encounters)
            {
                enc.isRevealed = true;
                ui.TellUser(enc.name + " is revealed");
            }
            ui.drawGameState(this);
            bool resolveNextEncounter = true;
            bool discardEncounter = true;
            List<EncounterDetail> encountersBeingDiscarded = new List<EncounterDetail>();
            EncounterDetail firstEncounter = null;
            EncounterDetail secondEncounter = null;


            if (map.LocationDetails(location).Encounters.Count() > 0)
            {
                firstEncounter = map.LocationDetails(location).Encounters.First();
            }
            if (map.LocationDetails(location).Encounters.Count() > 1)
            {
                secondEncounter = map.LocationDetails(location).Encounters[1];
            }
            if (firstEncounter != null)
            {
                bool encounterCancelled = false;
                foreach (Hunter h in Hunters)
                {
                    int hunterIndex = IndexOfHunter(h);
                    if (h.CurrentLocation == location)
                    {
                        if (ui.AskIfHunterIsPlayingSecretWeapon(h.Name))
                        {

                            DiscardEventFromHunterAtIndex("Secret Weapon", hunterIndex, ui);
                            Logger.WriteToDebugLog(h.Name + " played Secret Weapon");
                            Logger.WriteToGameLog(h.Name + " played Secret Weapon");
                            EventDetail draculaEventCardA = Dracula.WillPlayDevilishPower(this, ui);
                            bool eventIsCancelled = false;
                            if (draculaEventCardA != null)
                            {
                                switch (draculaEventCardA.name)
                                {
                                    case "DevilishPower":
                                        ui.TellUser("Dracula played Devilish power to cancel this event");
                                        Logger.WriteToDebugLog("Dracula played Devilish Power");
                                        DiscardEventFromDracula("Devilish Power");
                                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                        if (hunterPlayingGoodluck > -1)
                                        {
                                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                        }
                                        else
                                        {
                                            eventIsCancelled = true;
                                        }
                                        break;
                                }
                            }
                            if (!eventIsCancelled)
                            {
                                PlaySecretWeaponBeforeEncounter(hunterIndex, ui);
                            }
                        }
                        EventDetail draculaEventCard;
                        if (ui.AskIfHunterIsPlayingForeWarned(h.Name))
                        {
                            draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
                            bool eventIsCancelled = false;
                            if (draculaEventCard != null)
                            {
                                switch (draculaEventCard.name)
                                {
                                    case "DevilishPower":
                                        ui.TellUser("Dracula played Devilish power to cancel this event");
                                        DiscardEventFromDracula("Devilish Power");
                                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                        if (hunterPlayingGoodluck > -1)
                                        {
                                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                        }
                                        else
                                        {
                                            eventIsCancelled = true;
                                        }

                                        break;
                                }
                            }
                            if (!eventIsCancelled)
                            {
                                PlayForewarnedBeforeEncounter(hunterIndex, ui);
                                encounterCancelled = true;
                            }
                        }
                    }
                }
                if (encounterCancelled)
                {
                    resolveNextEncounter = true;
                    discardEncounter = true;
                }
                else
                {
                    resolveNextEncounter = ResolveEncounter(firstEncounter, hunter, out discardEncounter, ui);
                }
                if (discardEncounter)
                {
                    map.LocationDetails(location).Encounters.Remove(firstEncounter);
                    EncounterPool.Add(firstEncounter);
                    firstEncounter.isRevealed = false;
                }
                else if (firstEncounter.name == "Bats" || firstEncounter.name == "Fog")
                {
                    EncounterLimbo.Add(firstEncounter);
                    map.LocationDetails(location).Encounters.Remove(firstEncounter);
                }
            }
            if (secondEncounter != null)
            {
                bool encounterCancelled = false;
                foreach (Hunter h in Hunters)
                {
                    int hunterIndex = IndexOfHunter(h);
                    if (h.CurrentLocation == location)
                    {
                        if (ui.AskIfHunterIsPlayingSecretWeapon(h.Name))
                        {

                            DiscardEventFromHunterAtIndex("Secret Weapon", hunterIndex, ui);
                            Logger.WriteToDebugLog(h.Name + " played Secret Weapon");
                            Logger.WriteToGameLog(h.Name + " played Secret Weapon");
                            EventDetail draculaEventCardA = Dracula.WillPlayDevilishPower(this, ui);
                            bool eventIsCancelled = false;
                            if (draculaEventCardA != null)
                            {
                                switch (draculaEventCardA.name)
                                {
                                    case "DevilishPower":
                                        ui.TellUser("Dracula played Devilish power to cancel this event");
                                        Logger.WriteToDebugLog("Dracula played Devilish Power");
                                        DiscardEventFromDracula("Devilish Power");
                                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                        if (hunterPlayingGoodluck > -1)
                                        {
                                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                        }
                                        else
                                        {
                                            eventIsCancelled = true;
                                        }
                                        break;
                                }
                            }
                            if (!eventIsCancelled)
                            {
                                PlaySecretWeaponBeforeEncounter(hunterIndex, ui);
                            }
                        }
                        EventDetail draculaEventCard;
                        if (ui.AskIfHunterIsPlayingForeWarned(h.Name))
                        {
                            draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
                            bool eventIsCancelled = false;
                            if (draculaEventCard != null)
                            {
                                switch (draculaEventCard.name)
                                {
                                    case "DevilishPower":
                                        ui.TellUser("Dracula played Devilish power to cancel this event");
                                        DiscardEventFromDracula("Devilish Power");
                                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                        if (hunterPlayingGoodluck > -1)
                                        {
                                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                        }
                                        else
                                        {
                                            eventIsCancelled = true;
                                        }

                                        break;
                                }
                            }
                            if (!eventIsCancelled)
                            {
                                PlayForewarnedBeforeEncounter(hunterIndex, ui);
                                encounterCancelled = true;
                            }
                        }
                    }
                }
                if (encounterCancelled)
                {
                    resolveNextEncounter = true;
                    discardEncounter = true;
                }
                else
                {
                    resolveNextEncounter = ResolveEncounter(secondEncounter, hunter, out discardEncounter, ui);
                }
                if (discardEncounter)
                {
                    map.LocationDetails(location).Encounters.Remove(secondEncounter);
                    EncounterPool.Add(secondEncounter);
                }
                else if (secondEncounter.name == "Bats" || secondEncounter.name == "Fog")
                {
                    EncounterLimbo.Add(secondEncounter);
                    map.LocationDetails(location).Encounters.Remove(secondEncounter);
                }
            }
            return resolveNextEncounter;
        }














        // remove
        private int IndexOfHunter(Hunter h)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Hunters[i].Name == h.Name)
                {
                    return i;
                }
            }
            return -1;
        }

        private void PlaySecretWeaponBeforeEncounter(int hunterIndex, UserInterface ui)
        {
            string line = "";
            do
            {
                line = ui.GetNameOfItemDiscardedByHunter(Hunters[hunterIndex].Name);
            } while (GetItemByNameFromItemDeck(line).Name == "Unknown item");
            DiscardItemFromHunterAtIndex(line, hunterIndex, ui);
            do
            {
                line = ui.GetNameOfItemRetrievedFromDiscardByHunter(Hunters[hunterIndex].Name);
            } while (GetItemByNameFromItemDiscard(line).Name == "Unknown item");
            Hunters[hunterIndex].ItemsKnownToDracula.Add(GetItemByNameFromItemDiscard(line));
            ItemDiscard.Remove(GetItemByNameFromItemDiscard(line));
            Hunters[hunterIndex].NumberOfItems++;

        }

        private ItemDetail GetItemByNameFromItemDiscard(string line)
        {
            try
            {
                return ItemDiscard[ItemDiscard.FindIndex(card => card.Name.ToLower().StartsWith(line.ToLower()))];
            }
            catch (ArgumentOutOfRangeException)
            {
                return new ItemDetail("Unknown item");
            }
        }

        private bool ResolveEncounter(EncounterDetail enc, Hunter hunter, out bool discard, UserInterface ui)
        {
            bool resolveNextEncounter = true;
            discard = true;

            List<Hunter> huntersInvolved = new List<Hunter>();
            foreach (int ind in hunter.HuntersInGroup)
            {
                huntersInvolved.Add(Hunters[ind]);
            }

            switch (enc.name)
            {
                case "Ambush":
                    resolveNextEncounter = ResolveAmbush(huntersInvolved, ui);
                    break;
                case "Assasin":
                    resolveNextEncounter = ResolveAssassin(huntersInvolved, ui);
                    break;
                case "Bats":
                    resolveNextEncounter = ResolveBats(huntersInvolved, ui);
                    discard = false;
                    break;
                case "Desecrated Soil":
                    resolveNextEncounter = ResolveDesecratedSoil(huntersInvolved, ui);
                    break;
                case "Fog":
                    resolveNextEncounter = ResolveFog(huntersInvolved, ui);
                    discard = false;
                    break;
                case "Minion with Knife":
                    resolveNextEncounter = ResolveMinionWithKnife(huntersInvolved, ui);
                    break;
                case "Minion with Knife and Pistol":
                    resolveNextEncounter = ResolveMinionWithKnifeAndPistol(huntersInvolved, ui);
                    break;
                case "Minion with Knife and Rifle":
                    resolveNextEncounter = ResolveMinionWithKnifeAndRifle(huntersInvolved, ui);
                    break;
                case "Hoax":
                    resolveNextEncounter = ResolveHoax(huntersInvolved, ui);
                    break;
                case "Lightning":
                    resolveNextEncounter = ResolveLightning(huntersInvolved, ui);
                    break;
                case "Peasants":
                    resolveNextEncounter = ResolvePeasants(huntersInvolved, ui);
                    break;
                case "Plague":
                    resolveNextEncounter = ResolvePlague(huntersInvolved, ui);
                    break;
                case "Rats":
                    resolveNextEncounter = ResolveRats(huntersInvolved, ui);
                    break;
                case "Saboteur":
                    resolveNextEncounter = ResolveSaboteur(huntersInvolved, ui);
                    break;
                case "Spy":
                    resolveNextEncounter = ResolveSpy(huntersInvolved, ui);
                    break;
                case "Thief":
                    resolveNextEncounter = ResolveThief(huntersInvolved, ui);
                    break;
                case "New Vampire":
                    bool discardVampire = true;
                    resolveNextEncounter = ResolveNewVampire(huntersInvolved, out discardVampire, ui);
                    discard = discardVampire;
                    break;
                case "Wolves":
                    resolveNextEncounter = ResolveWolves(huntersInvolved, ui);
                    break;

            }
            return resolveNextEncounter;
        }

        internal bool DraculaWillPlayControlStorms(int hunterIndex, UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.PlayControlStorms(this);
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "Control Storms":
                        Logger.WriteToDebugLog("Dracula played Control Storms");
                        Logger.WriteToGameLog("Dracula played Control Storms");
                        ui.TellUser("Dracula played Control Storms");
                        DiscardEventFromDracula("Control Storms");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                            Logger.WriteToDebugLog(Hunters[hunterPlayingGoodluck].Name + " played Good Luck");
                            Logger.WriteToGameLog(Hunters[hunterPlayingGoodluck].Name + " played Good Luck");
                        }
                        else
                        {
                            PlayControlStorms(hunterIndex, ui); return true;
                        }
                        break;
                }
            }
            return false;
        }

        private void PlayForewarnedBeforeEncounter(int hunterIndex, UserInterface ui)
        {
            DiscardEventFromHunterAtIndex("Forewarned", hunterIndex, ui);
            ui.TellUser("Encounter cancelled");
        }

        // remove
        public bool LocationIsInCatacombs(LocationDetail location)
        {
            return Dracula.Catacombs.Contains(location);
        }

        internal void PlayRufusSmith()
        {
            if (HuntersHaveAlly())
            {
                AddEventToEventDiscard(GetEventByNameFromEventDeck(NameOfHunterAlly()));
            }
            SetHunterAlly("Rufus Smith");
            RemoveEventFromEventDeck(GetEventByNameFromEventDeck(NameOfHunterAlly()));
        }

        internal void DiscardEventCard(string cardName)
        {
            EventDetail playedCard = GetEventByNameFromEventDeck(cardName);
            AddEventToEventDiscard(playedCard);
            RemoveEventFromEventDeck(playedCard);
        }

        internal void PlayHiredScouts(UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }

                        break;
                }
            }
            if (!eventIsCancelled)
            {
                string line = "";
                LocationDetail locationToReveal;
                do
                {
                    line = ui.GetFirstLocationNameForHiredScouts();
                    locationToReveal = GetLocationFromName(line);
                    ui.TellUser(locationToReveal.Name);
                } while (locationToReveal.Name == "Unknown location" || locationToReveal.Name == "Multiple locations");
                if (LocationIsInTrail(locationToReveal))
                {
                    bool revealingThis = true;
                    draculaEventCard = Dracula.WillPlaySensationalistPress(this, 0);
                    if (draculaEventCard != null)
                    {
                        switch (draculaEventCard.name)
                        {
                            case "Sensationalist Press":
                                ui.TellUser("Dracula played Sensationalist Press to cancel revealing" + locationToReveal.Name);
                                DiscardEventFromDracula("Sensationalist Press");
                                int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                if (hunterPlayingGoodluck > -1)
                                {
                                    DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                }
                                else
                                {
                                    revealingThis = false;
                                }
                                break;
                        }
                    }
                    if (revealingThis)
                    {
                        locationToReveal.IsRevealed = true;
                        ui.TellUser("Revealing " + locationToReveal.Name);
                        for (int i = 0; i < locationToReveal.Encounters.Count(); i++)
                        {
                            locationToReveal.Encounters[i].isRevealed = true;
                            ui.TellUser(" and " + locationToReveal.Encounters[i].name);
                        }
                        ui.TellUser("");
                        ui.drawGameState(this);
                    }
                }
                else
                {
                    ui.TellUser(locationToReveal.Name + " is not in Dracula's trail");
                }
                do
                {
                    line = ui.GetSecondLocationNameForHiredScouts();
                    locationToReveal = GetLocationFromName(line);
                    ui.TellUser(locationToReveal.Name);
                } while (locationToReveal.Name == "Unknown location" || locationToReveal.Name == "Multiple locations");
                if (LocationIsInTrail(locationToReveal))
                {
                    locationToReveal.IsRevealed = true;
                    ui.TellUser("Revealing " + locationToReveal.Name);
                    for (int i = 0; i < locationToReveal.Encounters.Count(); i++)
                    {
                        locationToReveal.Encounters[i].isRevealed = true;
                        ui.TellUser(" and " + locationToReveal.Encounters[i].name);
                    }
                    ui.TellUser("");
                }
                else
                {
                    ui.TellUser(locationToReveal.Name + " is not in Dracula's trail");
                }
            }
        }

        internal void PlayEscapeRoute(UserInterface ui)
        {
            ui.TellUser("Escape Route is supposed to be played at the start of combat");
        }

        internal void PlayExcellentWeather(UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }
                        break;
                }
            }
            if (!eventIsCancelled)
            {
                ui.TellUser("You may move up to four sea moves this turn");
            }
        }

        internal void PlayCharteredCarriage(UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayCardToCancelCharteredCarriage(this);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "False Tip-off":
                        ui.TellUser("Dracula played False Tip-off to cancel your Chartered Carriage");
                        DiscardEventFromDracula("False Tip-off");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }
                        break;
                    case "Devilish Power":
                        ui.TellUser("Dracula played Devilish Power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluckB = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluckB > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluckB, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }
                        break;
                }
            }
            if (!eventIsCancelled)
            {
                ui.TellUser("You may catch a fast train this turn");
            }
        }

        internal void PlayForewarned(UserInterface ui)
        {
            ui.TellUser("Forewarned is supposed to be played when Dracula reveals an encounter at your location");
        }

        internal void PlaySecretWeapon(UserInterface ui)
        {
            ui.TellUser("Secret Weapon is supposed to be played when Dracula reveals an encounter at your location");
        }

        internal void PlayBloodTransfusion(UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }
                        break;
                }
            }
            if (!eventIsCancelled)
            {
                int hunterIndexA = ui.GetIndexOfHunterGivingBloodTransfusion();
                int hunterIndexB = ui.GetIndexOfHunterReceivingBloodTransfusion();
                Hunters[hunterIndexA].Health--;
                Hunters[hunterIndexB].NumberOfBites--;
                if (Hunters[hunterIndexB].NumberOfBites < 1)
                {
                    Hunters[hunterIndexB].ItemShownToDraculaForBeingBitten = null;
                    Hunters[hunterIndexB].EventShownToDraculaForBeingBitten = null;
                }
                ui.TellUser(Hunters[hunterIndexA].Name + " donated blood (1 health) to " + Hunters[hunterIndexB].Name + " who was cured of a Bite");
                Logger.WriteToDebugLog(Hunters[hunterIndexA].Name + " donated blood (1 health) to " + Hunters[hunterIndexB].Name + " who was cured of a Bite");
                Logger.WriteToGameLog(Hunters[hunterIndexA].Name + " donated blood (1 health) to " + Hunters[hunterIndexB].Name + " who was cured of a Bite");
            }
        }

        internal void PlayGoodLuck(int hunterIndex, UserInterface ui)
        {
            int response = ui.AskHunterDiscardAllyOrRoadblock();
            switch (response)
            {
                case 1:
                    EventDiscard.Add(DraculaAlly);
                    DraculaAlly = null;
                    DiscardEventFromHunterAtIndex("Good Luck", hunterIndex, ui);
                    break;
                case 2:
                    RoadblockCounter = new Roadblock();
                    DiscardEventFromHunterAtIndex("Good Luck", hunterIndex, ui);
                    break;
            }
        }

        internal void PlaySurprisingReturn(int hunterIndex, UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }
                        break;
                }
            }
            if (!eventIsCancelled)
            {
                string cardToReclaim;
                do
                {
                    cardToReclaim = ui.GetEventCardNameBeingReturned();
                } while (cardToReclaim.ToLower() != "none" && GetEventByNameFromEventDiscard(cardToReclaim).name == "Unknown event");
                if (cardToReclaim.ToLower() != "none")
                {
                    AddEventCardToHunterAtIndex(hunterIndex, ui);
                }
            }
        }

        private EventDetail GetEventByNameFromEventDiscard(string cardToReclaim)
        {
            try
            {
                return EventDiscard[EventDiscard.FindIndex(card => card.name.ToLower().StartsWith(cardToReclaim.ToLower()))];
            }
            catch (ArgumentOutOfRangeException)
            {
                return new EventDetail("Unknown event", false, EventType.Keep);
            }
        }

        internal void PlayStormySeas(UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }
                        break;
                }
            }
            if (!eventIsCancelled)
            {
                LocationDetail locationToStorm;
                do
                {
                    locationToStorm = GetLocationFromName(ui.GetNameOfLocationWhereStormySeasIsBeingPlayed());
                    ui.TellUser(locationToStorm.Name);
                } while (locationToStorm.Name == "Unknown location" || locationToStorm.Name == "Multiple locations" || locationToStorm.Type != LocationType.Sea);
                Logger.WriteToDebugLog("Stormy Seas was played in " + locationToStorm.Name);
                Logger.WriteToGameLog("Stormy Seas was played in " + locationToStorm.Name);
                locationToStorm.TurnsUntilStormSubsides = 2;
                if (locationToStorm == Dracula.CurrentLocation)
                {
                    locationToStorm.IsRevealed = true;
                    ui.TellUser("That is where Dracula was");
                    LocationDetail locationToMoveTo = Dracula.DecideWhichPortToGoToAfterStormySeas(this, locationToStorm);
                    Dracula.MoveByRoadOrSea(this, locationToMoveTo, ui);
                    draculaEventCard = Dracula.WillPlaySensationalistPress(this, 0);
                    bool revealing = true;
                    if (draculaEventCard != null)
                    {
                        switch (draculaEventCard.name)
                        {
                            case "Sensationalist Press":
                                ui.TellUser("Dracula played Sensationalist Press to cancel revealing the port he went to");
                                DiscardEventFromDracula("Sensationalist Press");
                                int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                if (hunterPlayingGoodluck > -1)
                                {
                                    DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                }
                                else
                                {
                                    revealing = false;
                                }
                                break;
                        }
                    }
                    if (revealing)
                    {
                        Dracula.LocationTrail[0].IsRevealed = true;
                    }
                    Dracula.HandleDroppedOffLocations(this, ui);
                    Dracula.MatureEncounters(this, ui);
                }
            }
        }

        // remove
        internal Hunter[] GetHunters()
        {
            return Hunters;
        }

        internal void PlayHypnosis(UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }
                        break;
                }
            }
            if (!eventIsCancelled)
            {
                if (ui.GetDieRoll() < 3)
                {
                    ui.TellUser("Nothing happened");
                    Logger.WriteToDebugLog("Hypnosis was unsuccessful");
                }
                else
                {
                    ui.TellUser("The hypnosis was successful");
                    Logger.WriteToDebugLog("The hypnosis was successful");
                    Logger.WriteToGameLog("The hypnosis was successful");
                    draculaEventCard = Dracula.WillPlaySensationalistPress(this, 0);
                    bool revealing = true;
                    if (draculaEventCard != null)
                    {
                        switch (draculaEventCard.name)
                        {
                            case "Sensationalist Press":
                                ui.TellUser("Dracula played Sensationalist Press to cancel revealing his current location");
                                DiscardEventFromDracula("Sensationalist Press");
                                int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                if (hunterPlayingGoodluck > -1)
                                {
                                    DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                }
                                else
                                {
                                    revealing = false;
                                }
                                break;
                        }
                    }
                    if (revealing)
                    {
                        ui.TellUser("Dracula's current location is " + Dracula.CurrentLocation.Name);
                        Dracula.CurrentLocation.IsRevealed = true;
                    }
                    foreach (LocationDetail l in Dracula.LocationTrail)
                    {
                        foreach (EncounterDetail e in l.Encounters)
                        {
                            if (e.name == "New Vampire")
                            {
                                e.isRevealed = true;
                                Logger.WriteToDebugLog("Vampire revealed at " + l.Name);
                                Logger.WriteToGameLog("Vampire revealed at " + l.Name);
                            }
                        }
                    }
                    foreach (LocationDetail l in Dracula.Catacombs)
                    {
                        foreach (EncounterDetail e in l.Encounters)
                        {
                            if (e.name == "New Vampire")
                            {
                                e.isRevealed = true;
                                Logger.WriteToDebugLog("Vampire revealed at " + l.Name);
                                Logger.WriteToGameLog("Vampire revealed at " + l.Name);
                            }
                        }
                    }
                    Dracula.DeterminePossibleLocations();
                    Dracula.DeterminePossiblePowers(Dracula.CurrentLocation.Type == LocationType.Sea ? TimeIndex : TimeIndex + 1);

                    Logger.WriteToDebugLog("Checking if there are legal moves");
                    if (Dracula.PossibleMoves.Count() + Dracula.PossiblePowers.Count() == 0)
                    {
                        Logger.WriteToDebugLog("Dracula has no legal moves");
                        ui.TellUser("Dracula is cornered by his own trail");
                    }
                    else if (Dracula.PossibleMoves.Count() == 0 && Dracula.PossiblePowers.Count() == 1 && Dracula.PossiblePowers.Contains(Dracula.Powers[1]))
                    {
                        Logger.WriteToDebugLog("Dracula has no regular moves available");
                        Dracula.DeterminePossibleWolfFormLocations();
                        if (Dracula.PossibleMoves.Count() == 0)
                        {
                            Logger.WriteToDebugLog("Dracula has no moves available by Wolf Form");
                            ui.TellUser("Dracula is cornered by his own trail");
                        }
                        Dracula.DeterminePossibleLocations();
                    }

                    string powerUsed;
                    LocationDetail destination = Dracula.ChooseMoveForHypnosis(this, out powerUsed);
                    ui.TellUser("Dracula will use " + powerUsed + " and move to " + destination.Name);
                    Dracula.AdvanceMovePower = powerUsed;
                    Dracula.AdvanceMoveDestination = destination;
                }
            }
        }

        internal void PlayTelegraphAhead(int hunterIndex, UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }
                        break;
                }
            }
            if (!eventIsCancelled)
            {
                int locationsRevealed = 0;
                foreach (LocationDetail loc in Hunters[hunterIndex].CurrentLocation.ByRoad)
                {
                    draculaEventCard = Dracula.WillPlaySensationalistPress(this, 0);
                    bool revealing = true;
                    if (draculaEventCard != null)
                    {
                        switch (draculaEventCard.name)
                        {
                            case "Sensationalist Press":
                                ui.TellUser("Dracula played Sensationalist Press to cancel revealing a location");
                                DiscardEventFromDracula("Sensationalist Press");
                                int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                if (hunterPlayingGoodluck > -1)
                                {
                                    DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                    DiscardEventFromDracula("Control Storms");
                                }
                                else
                                {
                                    revealing = false;
                                }
                                break;
                        }
                    }
                    if (revealing)
                    {
                        loc.IsRevealed = true;
                        locationsRevealed++;
                    }
                }
                if (locationsRevealed == 0)
                {
                    ui.TellUser("No locations revealed");
                }
            }
        }

        internal void PlayConsecratedGround(int hunterIndex, UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }

                        break;
                }
            }
            if (!eventIsCancelled)
            {
                LocationDetail locationToConsecrate;
                do
                {
                    locationToConsecrate = GetLocationFromName(ui.GetNameOfLocationToConsecrate());
                    ui.TellUser(locationToConsecrate.Name);
                } while (locationToConsecrate.Name == "Unknown location" || locationToConsecrate.Name == "Multiple locations" || (locationToConsecrate.Type != LocationType.City && locationToConsecrate.Type != LocationType.Town) || locationToConsecrate.Name == "Galatz" || locationToConsecrate.Name == "Klausenburg");
                Logger.WriteToDebugLog("Consecrate Ground was played in " + locationToConsecrate.Name);
                Logger.WriteToGameLog("Consecrate Ground was played in " + locationToConsecrate.Name);

                foreach (LocationDetail loc in Map)
                {
                    loc.IsConsecrated = false;
                }
                locationToConsecrate.IsConsecrated = true;
                ui.TellUser(locationToConsecrate.Name + " is now consecrated ground");

                if (locationToConsecrate == Dracula.CurrentLocation)
                {
                    locationToConsecrate.IsRevealed = true;
                    List<EncounterDetail> encountersToDiscard = new List<EncounterDetail>();
                    foreach (EncounterDetail e in locationToConsecrate.Encounters)
                    {
                        ui.TellUser(e.name + " was discarded");
                        encountersToDiscard.Add(e);
                        e.isRevealed = false;
                        EncounterPool.Add(e);
                    }
                    foreach (EncounterDetail e in encountersToDiscard)
                    {
                        locationToConsecrate.Encounters.Remove(e);
                    }
                }
            }
        }

        internal void PlayReEquip(UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }

                        break;
                }
            }
            if (!eventIsCancelled)
            {
                ui.TellUser("If you choose to discard an item, please tell me what it is and tell me that you have drawn another item");
            }
        }

        internal void PlayNewspaperReports(UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }

                        break;
                }
            }
            if (!eventIsCancelled)
            {
                int checkingLocationIndex = TrailLength();
                do
                {
                    checkingLocationIndex--;
                } while ((TypeOfLocationAtTrailIndex(checkingLocationIndex) != LocationType.Castle && TypeOfLocationAtTrailIndex(checkingLocationIndex) != LocationType.City && TypeOfLocationAtTrailIndex(checkingLocationIndex) != LocationType.Sea && TypeOfLocationAtTrailIndex(checkingLocationIndex) != LocationType.Town) || LocationIsRevealedAtTrailIndex(checkingLocationIndex));

                if (DraculaCurrentLocationIsAtTrailIndex(checkingLocationIndex))
                {
                    ui.TellUser("The oldest unrevealed location in Dracula's trail is his current location");
                    if (LocationWhereHideWasUsedIsDraculaCurrentLocation())
                    {
                        ui.TellUser("Here's the Hide card to prove it");
                        RevealHide(ui);
                    }
                }
                else
                {
                    draculaEventCard = Dracula.WillPlaySensationalistPress(this, 0);
                    bool revealing = true;
                    if (draculaEventCard != null)
                    {
                        switch (draculaEventCard.name)
                        {
                            case "Sensationalist Press":
                                ui.TellUser("Dracula played Sensationalist Press to cancel revealing the location");
                                DiscardEventFromDracula("Sensationalist Press");
                                int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                if (hunterPlayingGoodluck > -1)
                                {
                                    DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                }
                                else
                                {
                                    revealing = false;
                                }
                                break;
                        }
                    }
                    if (revealing)
                    {
                        RevealLocationAtTrailIndex(checkingLocationIndex, ui);
                        ui.TellUser("Revealing " + NameOfLocationAtTrailIndex(checkingLocationIndex));
                    }
                }
            }
        }

        internal void PlayAdvancePlanning(UserInterface ui)
        {
            ui.TellUser("Advance Planning is supposed to be played at the start of a combat");
        }

        internal void PlayMysticResearch(UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }

                        break;
                }
            }
            if (!eventIsCancelled)
            {
                ui.TellUser("These are Draculas cards:");
                foreach (EventDetail card in Dracula.EventCardsInHand)
                {
                    ui.TellUser(card.name);
                }
            }
        }

        internal void PlayLongDay(UserInterface ui)
        {
            {
                EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
                bool eventIsCancelled = false;
                if (draculaEventCard != null)
                {
                    switch (draculaEventCard.name)
                    {
                        case "DevilishPower":
                            ui.TellUser("Dracula played Devilish power to cancel this event");
                            DiscardEventFromDracula("Devilish Power");
                            int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                            if (hunterPlayingGoodluck > -1)
                            {
                                DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                            }
                            else
                            {
                                eventIsCancelled = true;
                            }

                            break;
                    }
                }
                if (!eventIsCancelled)
                {
                    AdjustTime(-1);
                }
            }
        }

        internal void PlayVampireLair(int hunterIndex, UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }

                        break;
                }
            }
            if (!eventIsCancelled)
            {
                switch (ResolveCombat(hunterIndex, 6, true, ui))
                {
                    case "Bite":
                        if (NumberOfHuntersAtLocation(LocationOfHunterAtHunterIndex(hunterIndex)) > 1)
                        {
                            ApplyBiteToOneOfMultipleHunters(hunterIndex, ui);
                        }
                        else
                        {
                            ApplyBiteToHunter(hunterIndex, ui);
                        }
                        break;
                    case "Enemy wounded":
                        VampirePointTracker--;
                        break;
                    case "Enemy killed":
                        VampirePointTracker--;
                        break;
                    case "Hunter killed":
                        Hunters[ui.GetNameOfHunterKilled()].Health = 0;
                        HandlePossibleHunterDeath(ui);
                        break;
                }
            }
        }

        internal void PlaySenseOfEmergency(int hunterIndex, UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }

                        break;
                }
            }
            if (!eventIsCancelled)
            {
                Hunters[hunterIndex].Health -= 6;
                Hunters[hunterIndex].Health += VampirePointTracker;
                ui.TellUser("Adjust " + Hunters[hunterIndex].Name + "'s health and then perform a move to any location");
            }
        }

        internal void PlayMoneyTrail(UserInterface ui)
        {
            EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
            bool eventIsCancelled = false;
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "DevilishPower":
                        ui.TellUser("Dracula played Devilish power to cancel this event");
                        DiscardEventFromDracula("Devilish Power");
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        }
                        else
                        {
                            eventIsCancelled = true;
                        }

                        break;
                }
            }
            if (!eventIsCancelled)
            {
                ui.TellUser("Revealing all sea locations in Dracula's trail");
                for (int i = 0; i < TrailLength(); i++)
                {
                    if (TypeOfLocationAtTrailIndex(i) == LocationType.Sea)
                    {
                        draculaEventCard = Dracula.WillPlaySensationalistPress(this, i);
                        bool revealing = true;
                        if (draculaEventCard != null)
                        {
                            switch (draculaEventCard.name)
                            {
                                case "Sensationalist Press":
                                    ui.TellUser("Dracula played Sensationalist Press to cancel revealing location at position " + i);
                                    DiscardEventFromDracula("Sensationalist Press");
                                    int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                    if (hunterPlayingGoodluck > -1)
                                    {
                                        DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                    }
                                    else
                                    {
                                        revealing = false;
                                    }
                                    break;
                            }
                        }
                        if (revealing)
                        {
                            RevealLocationAtTrailIndex(i, ui);
                        }
                    }
                }
            }
        }

        internal void PlayGreatStrength(UserInterface ui)
        {
            ui.TellUser("Great Strength is supposed to be played when a Hunter receives damage or a bite");
        }

        internal void PlayHeroicLeap(UserInterface ui)
        {
            ui.TellUser("Heroic Leap is supposed to be played at the start of a combat");
        }

        internal void PlaySisterAgatha()
        {
            if (HuntersHaveAlly())
            {
                AddEventToEventDiscard(GetEventByNameFromEventDeck(NameOfHunterAlly()));
            }
            SetHunterAlly("Sister Agatha");
            RemoveEventFromEventDeck(GetEventByNameFromEventDeck(NameOfHunterAlly()));
        }

        internal void PlayJonathanHarker()
        {
            if (HuntersHaveAlly())
            {
                AddEventToEventDiscard(GetEventByNameFromEventDeck(NameOfHunterAlly()));
            }
            SetHunterAlly("Jonathan Harker");
            RemoveEventFromEventDeck(GetEventByNameFromEventDeck(NameOfHunterAlly()));
        }

        //private void SetUpEncounters()
        //{
        //    EncounterPool.Add(new EncounterDetail("Ambush", "AMB"));
        //    EncounterPool.Add(new EncounterDetail("Ambush", "AMB"));
        //    EncounterPool.Add(new EncounterDetail("Ambush", "AMB"));
        //    EncounterPool.Add(new EncounterDetail("Assasin", "ASS"));
        //    EncounterPool.Add(new EncounterDetail("Bats", "BAT"));
        //    EncounterPool.Add(new EncounterDetail("Bats", "BAT"));
        //    EncounterPool.Add(new EncounterDetail("Bats", "BAT"));
        //    EncounterPool.Add(new EncounterDetail("Desecrated Soil", "DES"));
        //    EncounterPool.Add(new EncounterDetail("Desecrated Soil", "DES"));
        //    EncounterPool.Add(new EncounterDetail("Desecrated Soil", "DES"));
        //    EncounterPool.Add(new EncounterDetail("Fog", "FOG"));
        //    EncounterPool.Add(new EncounterDetail("Fog", "FOG"));
        //    EncounterPool.Add(new EncounterDetail("Fog", "FOG"));
        //    EncounterPool.Add(new EncounterDetail("Fog", "FOG"));
        //    EncounterPool.Add(new EncounterDetail("Minion with Knife", "MIK"));
        //    EncounterPool.Add(new EncounterDetail("Minion with Knife", "MIK"));
        //    EncounterPool.Add(new EncounterDetail("Minion with Knife", "MIK"));
        //    EncounterPool.Add(new EncounterDetail("Minion with Knife and Pistol", "MIP"));
        //    EncounterPool.Add(new EncounterDetail("Minion with Knife and Pistol", "MIP"));
        //    EncounterPool.Add(new EncounterDetail("Minion with Knife and Rifle", "MIR"));
        //    EncounterPool.Add(new EncounterDetail("Minion with Knife and Rifle", "MIR"));
        //    EncounterPool.Add(new EncounterDetail("Hoax", "HOA"));
        //    EncounterPool.Add(new EncounterDetail("Hoax", "HOA"));
        //    EncounterPool.Add(new EncounterDetail("Lightning", "LIG"));
        //    EncounterPool.Add(new EncounterDetail("Lightning", "LIG"));
        //    EncounterPool.Add(new EncounterDetail("Peasants", "PEA"));
        //    EncounterPool.Add(new EncounterDetail("Peasants", "PEA"));
        //    EncounterPool.Add(new EncounterDetail("Plague", "PLA"));
        //    EncounterPool.Add(new EncounterDetail("Rats", "RAT"));
        //    EncounterPool.Add(new EncounterDetail("Rats", "RAT"));
        //    EncounterPool.Add(new EncounterDetail("Saboteur", "SAB"));
        //    EncounterPool.Add(new EncounterDetail("Saboteur", "SAB"));
        //    EncounterPool.Add(new EncounterDetail("Spy", "SPY"));
        //    EncounterPool.Add(new EncounterDetail("Spy", "SPY"));
        //    EncounterPool.Add(new EncounterDetail("Thief", "THI"));
        //    EncounterPool.Add(new EncounterDetail("Thief", "THI"));
        //    EncounterPool.Add(new EncounterDetail("New Vampire", "VAM"));
        //    EncounterPool.Add(new EncounterDetail("New Vampire", "VAM"));
        //    EncounterPool.Add(new EncounterDetail("New Vampire", "VAM"));
        //    EncounterPool.Add(new EncounterDetail("New Vampire", "VAM"));
        //    EncounterPool.Add(new EncounterDetail("New Vampire", "VAM"));
        //    EncounterPool.Add(new EncounterDetail("New Vampire", "VAM"));
        //    EncounterPool.Add(new EncounterDetail("Wolves", "WOL"));
        //    EncounterPool.Add(new EncounterDetail("Wolves", "WOL"));
        //    EncounterPool.Add(new EncounterDetail("Wolves", "WOL"));
        //}

        // remove
        //internal void RemoveHunterAlly()
        //{
        //    EventDiscard.Add(HunterAlly);
        //    HunterAlly = null;
        //}

        //private List<LocationDetail> CreateMap()
        //{
        //    List<LocationDetail> tempMap = new List<LocationDetail>();
        //    LocationDetail galway = new LocationDetail();
        //    LocationDetail dublin = new LocationDetail();
        //    LocationDetail liverpool = new LocationDetail();
        //    LocationDetail edinburgh = new LocationDetail();
        //    LocationDetail manchester = new LocationDetail();
        //    LocationDetail swansea = new LocationDetail();
        //    LocationDetail plymouth = new LocationDetail();
        //    LocationDetail nantes = new LocationDetail();
        //    LocationDetail lehavre = new LocationDetail();
        //    LocationDetail london = new LocationDetail();
        //    LocationDetail paris = new LocationDetail();
        //    LocationDetail brussels = new LocationDetail();
        //    LocationDetail amsterdam = new LocationDetail();
        //    LocationDetail strasbourg = new LocationDetail();
        //    LocationDetail cologne = new LocationDetail();
        //    LocationDetail hamburg = new LocationDetail();
        //    LocationDetail frankfurt = new LocationDetail();
        //    LocationDetail nuremburg = new LocationDetail();
        //    LocationDetail leipzig = new LocationDetail();
        //    LocationDetail berlin = new LocationDetail();
        //    LocationDetail prague = new LocationDetail();
        //    LocationDetail castledracula = new LocationDetail();
        //    LocationDetail santander = new LocationDetail();
        //    LocationDetail saragossa = new LocationDetail();
        //    LocationDetail bordeaux = new LocationDetail();
        //    LocationDetail toulouse = new LocationDetail();
        //    LocationDetail barcelona = new LocationDetail();
        //    LocationDetail clermontferrand = new LocationDetail();
        //    LocationDetail marseilles = new LocationDetail();
        //    LocationDetail geneva = new LocationDetail();
        //    LocationDetail genoa = new LocationDetail();
        //    LocationDetail milan = new LocationDetail();
        //    LocationDetail zurich = new LocationDetail();
        //    LocationDetail florence = new LocationDetail();
        //    LocationDetail venice = new LocationDetail();
        //    LocationDetail munich = new LocationDetail();
        //    LocationDetail zagreb = new LocationDetail();
        //    LocationDetail vienna = new LocationDetail();
        //    LocationDetail stjosephandstmary = new LocationDetail();
        //    LocationDetail sarajevo = new LocationDetail();
        //    LocationDetail szeged = new LocationDetail();
        //    LocationDetail budapest = new LocationDetail();
        //    LocationDetail belgrade = new LocationDetail();
        //    LocationDetail klausenburg = new LocationDetail();
        //    LocationDetail sofia = new LocationDetail();
        //    LocationDetail bucharest = new LocationDetail();
        //    LocationDetail galatz = new LocationDetail();
        //    LocationDetail varna = new LocationDetail();
        //    LocationDetail constanta = new LocationDetail();
        //    LocationDetail lisbon = new LocationDetail();
        //    LocationDetail cadiz = new LocationDetail();
        //    LocationDetail madrid = new LocationDetail();
        //    LocationDetail granada = new LocationDetail();
        //    LocationDetail alicante = new LocationDetail();
        //    LocationDetail cagliari = new LocationDetail();
        //    LocationDetail rome = new LocationDetail();
        //    LocationDetail naples = new LocationDetail();
        //    LocationDetail bari = new LocationDetail();
        //    LocationDetail valona = new LocationDetail();
        //    LocationDetail salonica = new LocationDetail();
        //    LocationDetail athens = new LocationDetail();
        //    LocationDetail atlanticocean = new LocationDetail();
        //    LocationDetail irishsea = new LocationDetail();
        //    LocationDetail englishchannel = new LocationDetail();
        //    LocationDetail northsea = new LocationDetail();
        //    LocationDetail bayofbiscay = new LocationDetail();
        //    LocationDetail mediterraneansea = new LocationDetail();
        //    LocationDetail tyrrheniansea = new LocationDetail();
        //    LocationDetail adriaticsea = new LocationDetail();
        //    LocationDetail ioniansea = new LocationDetail();
        //    LocationDetail blacksea = new LocationDetail();

        //    galway.Name = "Galway";
        //    galway.Abbreviation = "GAW";
        //    galway.Type = LocationType.Town;
        //    galway.IsEastern = false;
        //    galway.ByRoad.Add(Location.dublin);
        //    galway.BySea.Add(Location.atlanticocean);
        //    tempMap.Add(galway);

        //    dublin.Name = "Dublin";
        //    dublin.Abbreviation = "DUB";
        //    dublin.Type = LocationType.Town;
        //    dublin.IsEastern = false;
        //    dublin.ByRoad.Add(Location.galway);
        //    dublin.BySea.Add(Location.irishsea);
        //    tempMap.Add(dublin);

        //    liverpool.Name = "Liverpool";
        //    liverpool.Abbreviation = "LIV";
        //    liverpool.Type = LocationType.City;
        //    liverpool.IsEastern = false;
        //    liverpool.ByRoad.Add(Location.manchester);
        //    liverpool.ByRoad.Add(Location.swansea);
        //    liverpool.ByTrain.Add(Location.manchester);
        //    liverpool.BySea.Add(Location.irishsea);
        //    tempMap.Add(liverpool);

        //    edinburgh.Name = "Edinburgh";
        //    edinburgh.Abbreviation = "EDI";
        //    edinburgh.Type = LocationType.City;
        //    edinburgh.IsEastern = false;
        //    edinburgh.ByRoad.Add(Location.manchester);
        //    edinburgh.ByTrain.Add(Location.manchester);
        //    edinburgh.BySea.Add(Location.northsea);
        //    tempMap.Add(edinburgh);

        //    manchester.Name = "Manchester";
        //    manchester.Abbreviation = "MAN";
        //    manchester.Type = LocationType.City;
        //    manchester.IsEastern = false;
        //    manchester.ByRoad.Add(Location.edinburgh);
        //    manchester.ByRoad.Add(Location.liverpool);
        //    manchester.ByRoad.Add(Location.london);
        //    manchester.ByTrain.Add(Location.edinburgh);
        //    manchester.ByTrain.Add(Location.liverpool);
        //    manchester.ByTrain.Add(Location.london);
        //    tempMap.Add(manchester);

        //    swansea.Name = "Swansea";
        //    swansea.Abbreviation = "SWA";
        //    swansea.Type = LocationType.Town;
        //    swansea.IsEastern = false;
        //    swansea.ByRoad.Add(Location.liverpool);
        //    swansea.ByRoad.Add(Location.london);
        //    swansea.ByTrain.Add(Location.london);
        //    swansea.BySea.Add(Location.irishsea);
        //    tempMap.Add(swansea);

        //    plymouth.Name = "Plymouth";
        //    plymouth.Abbreviation = "PLY";
        //    plymouth.Type = LocationType.Town;
        //    plymouth.IsEastern = false;
        //    plymouth.ByRoad.Add(Location.london);
        //    plymouth.BySea.Add(Location.englishchannel);
        //    tempMap.Add(plymouth);

        //    nantes.Name = "Nantes";
        //    nantes.Abbreviation = "NAN";
        //    nantes.Type = LocationType.City;
        //    nantes.IsEastern = false;
        //    nantes.ByRoad.Add(Location.lehavre);
        //    nantes.ByRoad.Add(Location.paris);
        //    nantes.ByRoad.Add(Location.clermontferrand);
        //    nantes.ByRoad.Add(Location.bordeaux);
        //    nantes.BySea.Add(Location.bayofbiscay);
        //    tempMap.Add(nantes);

        //    lehavre.Name = "Le Havre";
        //    lehavre.Abbreviation = "LEH";
        //    lehavre.Type = LocationType.Town;
        //    lehavre.IsEastern = false;
        //    lehavre.ByRoad.Add(Location.nantes);
        //    lehavre.ByRoad.Add(Location.paris);
        //    lehavre.ByRoad.Add(Location.brussels);
        //    lehavre.ByTrain.Add(Location.paris);
        //    lehavre.BySea.Add(Location.englishchannel);
        //    tempMap.Add(lehavre);

        //    london.Name = "London";
        //    london.Abbreviation = "LON";
        //    london.Type = LocationType.City;
        //    london.IsEastern = false;
        //    london.ByRoad.Add(Location.manchester);
        //    london.ByRoad.Add(Location.swansea);
        //    london.ByRoad.Add(Location.plymouth);
        //    london.ByTrain.Add(Location.manchester);
        //    london.ByTrain.Add(Location.swansea);
        //    london.BySea.Add(Location.englishchannel);
        //    tempMap.Add(london);

        //    paris.Name = "Paris";
        //    paris.Abbreviation = "PAR";
        //    paris.Type = LocationType.City;
        //    paris.IsEastern = false;
        //    paris.ByRoad.Add(Location.nantes);
        //    paris.ByRoad.Add(Location.lehavre);
        //    paris.ByRoad.Add(Location.brussels);
        //    paris.ByRoad.Add(Location.strasbourg);
        //    paris.ByRoad.Add(Location.geneva);
        //    paris.ByRoad.Add(Location.clermontferrand);
        //    paris.ByTrain.Add(Location.lehavre);
        //    paris.ByTrain.Add(Location.brussels);
        //    paris.ByTrain.Add(Location.marseilles);
        //    paris.ByTrain.Add(Location.bordeaux);
        //    tempMap.Add(paris);

        //    brussels.Name = "Brussels";
        //    brussels.Abbreviation = "BRU";
        //    brussels.Type = LocationType.City;
        //    brussels.IsEastern = false;
        //    brussels.ByRoad.Add(Location.lehavre);
        //    brussels.ByRoad.Add(Location.amsterdam);
        //    brussels.ByRoad.Add(Location.cologne);
        //    brussels.ByRoad.Add(Location.strasbourg);
        //    brussels.ByRoad.Add(Location.paris);
        //    brussels.ByTrain.Add(Location.cologne);
        //    brussels.ByTrain.Add(Location.paris);
        //    tempMap.Add(brussels);

        //    amsterdam.Name = "Amsterdam";
        //    amsterdam.Abbreviation = "AMS";
        //    amsterdam.Type = LocationType.City;
        //    amsterdam.IsEastern = false;
        //    amsterdam.ByRoad.Add(Location.brussels);
        //    amsterdam.ByRoad.Add(Location.cologne);
        //    amsterdam.BySea.Add(Location.northsea);
        //    tempMap.Add(amsterdam);

        //    strasbourg.Name = "Strasbourg";
        //    strasbourg.Abbreviation = "STR";
        //    strasbourg.Type = LocationType.Town;
        //    strasbourg.IsEastern = false;
        //    strasbourg.ByRoad.Add(Location.paris);
        //    strasbourg.ByRoad.Add(Location.brussels);
        //    strasbourg.ByRoad.Add(Location.cologne);
        //    strasbourg.ByRoad.Add(Location.frankfurt);
        //    strasbourg.ByRoad.Add(Location.nuremburg);
        //    strasbourg.ByRoad.Add(Location.munich);
        //    strasbourg.ByRoad.Add(Location.zurich);
        //    strasbourg.ByRoad.Add(Location.geneva);
        //    strasbourg.ByTrain.Add(Location.frankfurt);
        //    strasbourg.ByTrain.Add(Location.zurich);
        //    tempMap.Add(strasbourg);

        //    cologne.Name = "Cologne";
        //    cologne.Abbreviation = "COL";
        //    cologne.Type = LocationType.City;
        //    cologne.IsEastern = false;
        //    cologne.ByRoad.Add(Location.brussels);
        //    cologne.ByRoad.Add(Location.amsterdam);
        //    cologne.ByRoad.Add(Location.hamburg);
        //    cologne.ByRoad.Add(Location.leipzig);
        //    cologne.ByRoad.Add(Location.frankfurt);
        //    cologne.ByRoad.Add(Location.strasbourg);
        //    cologne.ByTrain.Add(Location.brussels);
        //    cologne.ByTrain.Add(Location.frankfurt);
        //    tempMap.Add(cologne);

        //    hamburg.Name = "Hamburg";
        //    hamburg.Abbreviation = "HAM";
        //    hamburg.Type = LocationType.City;
        //    hamburg.IsEastern = false;
        //    hamburg.ByRoad.Add(Location.cologne);
        //    hamburg.ByRoad.Add(Location.berlin);
        //    hamburg.ByRoad.Add(Location.leipzig);
        //    hamburg.ByTrain.Add(Location.berlin);
        //    hamburg.BySea.Add(Location.northsea);
        //    tempMap.Add(hamburg);

        //    frankfurt.Name = "Frankfurt";
        //    frankfurt.Abbreviation = "FRA";
        //    frankfurt.Type = LocationType.Town;
        //    frankfurt.IsEastern = false;
        //    frankfurt.ByRoad.Add(Location.strasbourg);
        //    frankfurt.ByRoad.Add(Location.cologne);
        //    frankfurt.ByRoad.Add(Location.leipzig);
        //    frankfurt.ByRoad.Add(Location.nuremburg);
        //    frankfurt.ByTrain.Add(Location.strasbourg);
        //    frankfurt.ByTrain.Add(Location.cologne);
        //    frankfurt.ByTrain.Add(Location.leipzig);
        //    tempMap.Add(frankfurt);

        //    nuremburg.Name = "Nuremburg";
        //    nuremburg.Abbreviation = "NUR";
        //    nuremburg.Type = LocationType.Town;
        //    nuremburg.IsEastern = false;
        //    nuremburg.ByRoad.Add(Location.strasbourg);
        //    nuremburg.ByRoad.Add(Location.frankfurt);
        //    nuremburg.ByRoad.Add(Location.leipzig);
        //    nuremburg.ByRoad.Add(Location.prague);
        //    nuremburg.ByRoad.Add(Location.munich);
        //    nuremburg.ByTrain.Add(Location.leipzig);
        //    nuremburg.ByTrain.Add(Location.munich);
        //    tempMap.Add(nuremburg);

        //    leipzig.Name = "Leipzig";
        //    leipzig.Abbreviation = "LEI";
        //    leipzig.Type = LocationType.City;
        //    leipzig.IsEastern = false;
        //    leipzig.ByRoad.Add(Location.cologne);
        //    leipzig.ByRoad.Add(Location.hamburg);
        //    leipzig.ByRoad.Add(Location.berlin);
        //    leipzig.ByRoad.Add(Location.nuremburg);
        //    leipzig.ByRoad.Add(Location.frankfurt);
        //    leipzig.ByTrain.Add(Location.frankfurt);
        //    leipzig.ByTrain.Add(Location.berlin);
        //    leipzig.ByTrain.Add(Location.nuremburg);
        //    tempMap.Add(leipzig);

        //    berlin.Name = "Berlin";
        //    berlin.Abbreviation = "BER";
        //    berlin.Type = LocationType.City;
        //    berlin.IsEastern = false;
        //    berlin.ByRoad.Add(Location.hamburg);
        //    berlin.ByRoad.Add(Location.prague);
        //    berlin.ByRoad.Add(Location.leipzig);
        //    berlin.ByTrain.Add(Location.hamburg);
        //    berlin.ByTrain.Add(Location.leipzig);
        //    berlin.ByTrain.Add(Location.prague);
        //    tempMap.Add(berlin);

        //    prague.Name = "Prague";
        //    prague.Abbreviation = "PRA";
        //    prague.Type = LocationType.City;
        //    prague.IsEastern = true;
        //    prague.ByRoad.Add(Location.berlin);
        //    prague.ByRoad.Add(Location.vienna);
        //    prague.ByRoad.Add(Location.nuremburg);
        //    prague.ByTrain.Add(Location.berlin);
        //    prague.ByTrain.Add(Location.vienna);
        //    tempMap.Add(prague);

        //    castledracula.Name = "Castle Dracula";
        //    castledracula.Abbreviation = "CAS";
        //    castledracula.Type = LocationType.Castle;
        //    castledracula.IsEastern = true;
        //    castledracula.ByRoad.Add(Location.klausenburg);
        //    castledracula.ByRoad.Add(Location.galatz);
        //    tempMap.Add(castledracula);

        //    santander.Name = "Santander";
        //    santander.Abbreviation = "SAN";
        //    santander.Type = LocationType.Town;
        //    santander.IsEastern = false;
        //    santander.ByRoad.Add(Location.lisbon);
        //    santander.ByRoad.Add(Location.madrid);
        //    santander.ByRoad.Add(Location.saragossa);
        //    santander.ByTrain.Add(Location.madrid);
        //    santander.BySea.Add(Location.bayofbiscay);
        //    tempMap.Add(santander);

        //    saragossa.Name = "Saragossa";
        //    saragossa.Abbreviation = "SAG";
        //    saragossa.Type = LocationType.Town;
        //    saragossa.IsEastern = false;
        //    saragossa.ByRoad.Add(Location.madrid);
        //    saragossa.ByRoad.Add(Location.santander);
        //    saragossa.ByRoad.Add(Location.bordeaux);
        //    saragossa.ByRoad.Add(Location.toulouse);
        //    saragossa.ByRoad.Add(Location.barcelona);
        //    saragossa.ByRoad.Add(Location.alicante);
        //    saragossa.ByTrain.Add(Location.madrid);
        //    saragossa.ByTrain.Add(Location.bordeaux);
        //    saragossa.ByTrain.Add(Location.barcelona);
        //    tempMap.Add(saragossa);

        //    bordeaux.Name = "Bordeaux";
        //    bordeaux.Abbreviation = "BOR";
        //    bordeaux.Type = LocationType.City;
        //    bordeaux.IsEastern = false;
        //    bordeaux.ByRoad.Add(Location.saragossa);
        //    bordeaux.ByRoad.Add(Location.nantes);
        //    bordeaux.ByRoad.Add(Location.clermontferrand);
        //    bordeaux.ByRoad.Add(Location.toulouse);
        //    bordeaux.ByTrain.Add(Location.paris);
        //    bordeaux.ByTrain.Add(Location.saragossa);
        //    bordeaux.BySea.Add(Location.bayofbiscay);
        //    tempMap.Add(bordeaux);

        //    toulouse.Name = "Toulouse";
        //    toulouse.Abbreviation = "TOU";
        //    toulouse.Type = LocationType.Town;
        //    toulouse.IsEastern = false;
        //    toulouse.ByRoad.Add(Location.saragossa);
        //    toulouse.ByRoad.Add(Location.bordeaux);
        //    toulouse.ByRoad.Add(Location.clermontferrand);
        //    toulouse.ByRoad.Add(Location.marseilles);
        //    toulouse.ByRoad.Add(Location.barcelona);
        //    tempMap.Add(toulouse);

        //    barcelona.Name = "Barcelona";
        //    barcelona.Abbreviation = "BAC";
        //    barcelona.Type = LocationType.City;
        //    barcelona.IsEastern = false;
        //    barcelona.ByRoad.Add(Location.saragossa);
        //    barcelona.ByRoad.Add(Location.toulouse);
        //    barcelona.ByTrain.Add(Location.saragossa);
        //    barcelona.ByTrain.Add(Location.alicante);
        //    barcelona.BySea.Add(Location.mediterraneansea);
        //    tempMap.Add(barcelona);

        //    clermontferrand.Name = "Clermont Ferrand";
        //    clermontferrand.Abbreviation = "CLE";
        //    clermontferrand.Type = LocationType.Town;
        //    clermontferrand.IsEastern = false;
        //    clermontferrand.ByRoad.Add(Location.bordeaux);
        //    clermontferrand.ByRoad.Add(Location.nantes);
        //    clermontferrand.ByRoad.Add(Location.paris);
        //    clermontferrand.ByRoad.Add(Location.geneva);
        //    clermontferrand.ByRoad.Add(Location.marseilles);
        //    clermontferrand.ByRoad.Add(Location.toulouse);
        //    tempMap.Add(clermontferrand);

        //    marseilles.Name = "Marseilles";
        //    marseilles.Abbreviation = "MAR";
        //    marseilles.Type = LocationType.City;
        //    marseilles.IsEastern = false;
        //    marseilles.ByRoad.Add(Location.toulouse);
        //    marseilles.ByRoad.Add(Location.clermontferrand);
        //    marseilles.ByRoad.Add(Location.geneva);
        //    marseilles.ByRoad.Add(Location.zurich);
        //    marseilles.ByRoad.Add(Location.milan);
        //    marseilles.ByRoad.Add(Location.genoa);
        //    marseilles.ByTrain.Add(Location.paris);
        //    marseilles.BySea.Add(Location.mediterraneansea);
        //    tempMap.Add(marseilles);

        //    geneva.Name = "Geneva";
        //    geneva.Abbreviation = "GEV";
        //    geneva.Type = LocationType.Town;
        //    geneva.IsEastern = false;
        //    geneva.ByRoad.Add(Location.marseilles);
        //    geneva.ByRoad.Add(Location.clermontferrand);
        //    geneva.ByRoad.Add(Location.paris);
        //    geneva.ByRoad.Add(Location.strasbourg);
        //    geneva.ByRoad.Add(Location.zurich);
        //    geneva.ByTrain.Add(Location.milan);
        //    tempMap.Add(geneva);

        //    genoa.Name = "Genoa";
        //    genoa.Abbreviation = "GEO";
        //    genoa.Type = LocationType.City;
        //    genoa.IsEastern = true;
        //    genoa.ByRoad.Add(Location.marseilles);
        //    genoa.ByRoad.Add(Location.milan);
        //    genoa.ByRoad.Add(Location.venice);
        //    genoa.ByRoad.Add(Location.florence);
        //    genoa.ByTrain.Add(Location.milan);
        //    genoa.BySea.Add(Location.tyrrheniansea);
        //    tempMap.Add(genoa);

        //    milan.Name = "Milan";
        //    milan.Abbreviation = "MIL";
        //    milan.Type = LocationType.City;
        //    milan.IsEastern = true;
        //    milan.ByRoad.Add(Location.marseilles);
        //    milan.ByRoad.Add(Location.zurich);
        //    milan.ByRoad.Add(Location.munich);
        //    milan.ByRoad.Add(Location.venice);
        //    milan.ByRoad.Add(Location.genoa);
        //    milan.ByTrain.Add(Location.geneva);
        //    milan.ByTrain.Add(Location.zurich);
        //    milan.ByTrain.Add(Location.florence);
        //    milan.ByTrain.Add(Location.genoa);
        //    tempMap.Add(milan);

        //    zurich.Name = "Zurich";
        //    zurich.Abbreviation = "ZUR";
        //    zurich.Type = LocationType.Town;
        //    zurich.IsEastern = false;
        //    zurich.ByRoad.Add(Location.marseilles);
        //    zurich.ByRoad.Add(Location.geneva);
        //    zurich.ByRoad.Add(Location.strasbourg);
        //    zurich.ByRoad.Add(Location.munich);
        //    zurich.ByRoad.Add(Location.milan);
        //    zurich.ByTrain.Add(Location.strasbourg);
        //    zurich.ByTrain.Add(Location.milan);
        //    tempMap.Add(zurich);

        //    florence.Name = "Florence";
        //    florence.Abbreviation = "FLO";
        //    florence.Type = LocationType.Town;
        //    florence.IsEastern = true;
        //    florence.ByRoad.Add(Location.genoa);
        //    florence.ByRoad.Add(Location.venice);
        //    florence.ByRoad.Add(Location.rome);
        //    florence.ByTrain.Add(Location.milan);
        //    florence.ByTrain.Add(Location.rome);
        //    tempMap.Add(florence);

        //    venice.Name = "Venice";
        //    venice.Abbreviation = "VEN";
        //    venice.Type = LocationType.Town;
        //    venice.IsEastern = true;
        //    venice.ByRoad.Add(Location.florence);
        //    venice.ByRoad.Add(Location.genoa);
        //    venice.ByRoad.Add(Location.milan);
        //    venice.ByRoad.Add(Location.munich);
        //    venice.ByTrain.Add(Location.vienna);
        //    venice.BySea.Add(Location.adriaticsea);
        //    tempMap.Add(venice);

        //    munich.Name = "Munich";
        //    munich.Abbreviation = "MUN";
        //    munich.Type = LocationType.City;
        //    munich.IsEastern = false;
        //    munich.ByRoad.Add(Location.milan);
        //    munich.ByRoad.Add(Location.zurich);
        //    munich.ByRoad.Add(Location.strasbourg);
        //    munich.ByRoad.Add(Location.nuremburg);
        //    munich.ByRoad.Add(Location.vienna);
        //    munich.ByRoad.Add(Location.zagreb);
        //    munich.ByRoad.Add(Location.venice);
        //    munich.ByTrain.Add(Location.nuremburg);
        //    tempMap.Add(munich);

        //    zagreb.Name = "Zagreb";
        //    zagreb.Abbreviation = "ZAG";
        //    zagreb.Type = LocationType.Town;
        //    zagreb.IsEastern = true;
        //    zagreb.ByRoad.Add(Location.munich);
        //    zagreb.ByRoad.Add(Location.vienna);
        //    zagreb.ByRoad.Add(Location.budapest);
        //    zagreb.ByRoad.Add(Location.szeged);
        //    zagreb.ByRoad.Add(Location.stjosephandstmary);
        //    zagreb.ByRoad.Add(Location.sarajevo);
        //    tempMap.Add(zagreb);

        //    vienna.Name = "Vienna";
        //    vienna.Abbreviation = "VIE";
        //    vienna.Type = LocationType.City;
        //    vienna.IsEastern = true;
        //    vienna.ByRoad.Add(Location.munich);
        //    vienna.ByRoad.Add(Location.prague);
        //    vienna.ByRoad.Add(Location.budapest);
        //    vienna.ByRoad.Add(Location.zagreb);
        //    vienna.ByTrain.Add(Location.venice);
        //    vienna.ByTrain.Add(Location.prague);
        //    vienna.ByTrain.Add(Location.budapest);
        //    tempMap.Add(vienna);

        //    stjosephandstmary.Name = "St. Joseph & St. Mary";
        //    stjosephandstmary.Abbreviation = "STJ";
        //    stjosephandstmary.Type = LocationType.Hospital;
        //    stjosephandstmary.IsEastern = true;
        //    stjosephandstmary.ByRoad.Add(Location.zagreb);
        //    stjosephandstmary.ByRoad.Add(Location.szeged);
        //    stjosephandstmary.ByRoad.Add(Location.belgrade);
        //    stjosephandstmary.ByRoad.Add(Location.sarajevo);
        //    tempMap.Add(stjosephandstmary);

        //    sarajevo.Name = "Sarajevo";
        //    sarajevo.Abbreviation = "SAJ";
        //    sarajevo.Type = LocationType.Town;
        //    sarajevo.IsEastern = true;
        //    sarajevo.ByRoad.Add(Location.zagreb);
        //    sarajevo.ByRoad.Add(Location.stjosephandstmary);
        //    sarajevo.ByRoad.Add(Location.belgrade);
        //    sarajevo.ByRoad.Add(Location.sofia);
        //    sarajevo.ByRoad.Add(Location.valona);
        //    tempMap.Add(sarajevo);

        //    szeged.Name = "Szeged";
        //    szeged.Abbreviation = "SZE";
        //    szeged.Type = LocationType.Town;
        //    szeged.IsEastern = true;
        //    szeged.ByRoad.Add(Location.zagreb);
        //    szeged.ByRoad.Add(Location.budapest);
        //    szeged.ByRoad.Add(Location.klausenburg);
        //    szeged.ByRoad.Add(Location.belgrade);
        //    szeged.ByRoad.Add(Location.stjosephandstmary);
        //    szeged.ByTrain.Add(Location.budapest);
        //    szeged.ByTrain.Add(Location.bucharest);
        //    szeged.ByTrain.Add(Location.belgrade);
        //    tempMap.Add(szeged);

        //    budapest.Name = "Budapest";
        //    budapest.Abbreviation = "BUD";
        //    budapest.Type = LocationType.City;
        //    budapest.IsEastern = true;
        //    budapest.ByRoad.Add(Location.vienna);
        //    budapest.ByRoad.Add(Location.klausenburg);
        //    budapest.ByRoad.Add(Location.szeged);
        //    budapest.ByRoad.Add(Location.zagreb);
        //    budapest.ByTrain.Add(Location.vienna);
        //    budapest.ByTrain.Add(Location.szeged);
        //    tempMap.Add(budapest);

        //    belgrade.Name = "Belgrade";
        //    belgrade.Abbreviation = "BEL";
        //    belgrade.Type = LocationType.Town;
        //    belgrade.IsEastern = true;
        //    belgrade.ByRoad.Add(Location.stjosephandstmary);
        //    belgrade.ByRoad.Add(Location.szeged);
        //    belgrade.ByRoad.Add(Location.klausenburg);
        //    belgrade.ByRoad.Add(Location.bucharest);
        //    belgrade.ByRoad.Add(Location.sofia);
        //    belgrade.ByRoad.Add(Location.sarajevo);
        //    belgrade.ByTrain.Add(Location.szeged);
        //    belgrade.ByTrain.Add(Location.sofia);
        //    tempMap.Add(belgrade);

        //    klausenburg.Name = "Klausenburg";
        //    klausenburg.Abbreviation = "KLA";
        //    klausenburg.Type = LocationType.Town;
        //    klausenburg.IsEastern = true;
        //    klausenburg.ByRoad.Add(Location.budapest);
        //    klausenburg.ByRoad.Add(Location.castledracula);
        //    klausenburg.ByRoad.Add(Location.galatz);
        //    klausenburg.ByRoad.Add(Location.bucharest);
        //    klausenburg.ByRoad.Add(Location.belgrade);
        //    klausenburg.ByRoad.Add(Location.szeged);
        //    tempMap.Add(klausenburg);

        //    sofia.Name = "Sofia";
        //    sofia.Abbreviation = "SOF";
        //    sofia.Type = LocationType.Town;
        //    sofia.IsEastern = true;
        //    sofia.ByRoad.Add(Location.sarajevo);
        //    sofia.ByRoad.Add(Location.belgrade);
        //    sofia.ByRoad.Add(Location.bucharest);
        //    sofia.ByRoad.Add(Location.varna);
        //    sofia.ByRoad.Add(Location.salonica);
        //    sofia.ByRoad.Add(Location.valona);
        //    sofia.ByTrain.Add(Location.belgrade);
        //    sofia.ByTrain.Add(Location.salonica);
        //    tempMap.Add(sofia);

        //    bucharest.Name = "Bucharest";
        //    bucharest.Abbreviation = "BUC";
        //    bucharest.Type = LocationType.City;
        //    bucharest.IsEastern = true;
        //    bucharest.ByRoad.Add(Location.belgrade);
        //    bucharest.ByRoad.Add(Location.klausenburg);
        //    bucharest.ByRoad.Add(Location.galatz);
        //    bucharest.ByRoad.Add(Location.constanta);
        //    bucharest.ByRoad.Add(Location.sofia);
        //    bucharest.ByTrain.Add(Location.szeged);
        //    bucharest.ByTrain.Add(Location.galatz);
        //    bucharest.ByTrain.Add(Location.constanta);
        //    tempMap.Add(bucharest);

        //    galatz.Name = "Galatz";
        //    galatz.Abbreviation = "GAT";
        //    galatz.Type = LocationType.Town;
        //    galatz.IsEastern = true;
        //    galatz.ByRoad.Add(Location.klausenburg);
        //    galatz.ByRoad.Add(Location.castledracula);
        //    galatz.ByRoad.Add(Location.constanta);
        //    galatz.ByRoad.Add(Location.bucharest);
        //    galatz.ByTrain.Add(Location.bucharest);
        //    tempMap.Add(galatz);

        //    varna.Name = "Varna";
        //    varna.Abbreviation = "VAR";
        //    varna.Type = LocationType.City;
        //    varna.IsEastern = true;
        //    varna.ByRoad.Add(Location.sofia);
        //    varna.ByRoad.Add(Location.constanta);
        //    varna.ByTrain.Add(Location.sofia);
        //    varna.BySea.Add(Location.blacksea);
        //    tempMap.Add(varna);

        //    constanta.Name = "Constanta";
        //    constanta.Abbreviation = "CON";
        //    constanta.Type = LocationType.City;
        //    constanta.IsEastern = true;
        //    constanta.ByRoad.Add(Location.galatz);
        //    constanta.ByRoad.Add(Location.varna);
        //    constanta.ByRoad.Add(Location.bucharest);
        //    constanta.ByTrain.Add(Location.bucharest);
        //    constanta.BySea.Add(Location.blacksea);
        //    tempMap.Add(constanta);

        //    lisbon.Name = "Lisbon";
        //    lisbon.Abbreviation = "LIS";
        //    lisbon.Type = LocationType.City;
        //    lisbon.IsEastern = false;
        //    lisbon.ByRoad.Add(Location.santander);
        //    lisbon.ByRoad.Add(Location.madrid);
        //    lisbon.ByRoad.Add(Location.cadiz);
        //    lisbon.ByTrain.Add(Location.madrid);
        //    lisbon.BySea.Add(Location.atlanticocean);
        //    tempMap.Add(lisbon);

        //    cadiz.Name = "Cadiz";
        //    cadiz.Abbreviation = "CAD";
        //    cadiz.Type = LocationType.City;
        //    cadiz.IsEastern = false;
        //    cadiz.ByRoad.Add(Location.lisbon);
        //    cadiz.ByRoad.Add(Location.madrid);
        //    cadiz.ByRoad.Add(Location.granada);
        //    cadiz.BySea.Add(Location.atlanticocean);
        //    tempMap.Add(cadiz);

        //    madrid.Name = "Madrid";
        //    madrid.Abbreviation = "MAD";
        //    madrid.Type = LocationType.City;
        //    madrid.IsEastern = false;
        //    madrid.ByRoad.Add(Location.lisbon);
        //    madrid.ByRoad.Add(Location.santander);
        //    madrid.ByRoad.Add(Location.saragossa);
        //    madrid.ByRoad.Add(Location.alicante);
        //    madrid.ByRoad.Add(Location.granada);
        //    madrid.ByRoad.Add(Location.cadiz);
        //    madrid.ByTrain.Add(Location.lisbon);
        //    madrid.ByTrain.Add(Location.santander);
        //    madrid.ByTrain.Add(Location.saragossa);
        //    madrid.ByTrain.Add(Location.alicante);
        //    tempMap.Add(madrid);

        //    granada.Name = "Granada";
        //    granada.Abbreviation = "GRA";
        //    granada.Type = LocationType.Town;
        //    granada.IsEastern = false;
        //    granada.ByRoad.Add(Location.cadiz);
        //    granada.ByRoad.Add(Location.madrid);
        //    granada.ByRoad.Add(Location.alicante);
        //    tempMap.Add(granada);

        //    alicante.Name = "Alicante";
        //    alicante.Abbreviation = "ALI";
        //    alicante.Type = LocationType.Town;
        //    alicante.IsEastern = false;
        //    alicante.ByRoad.Add(Location.granada);
        //    alicante.ByRoad.Add(Location.madrid);
        //    alicante.ByRoad.Add(Location.saragossa);
        //    alicante.ByTrain.Add(Location.madrid);
        //    alicante.ByTrain.Add(Location.barcelona);
        //    alicante.BySea.Add(Location.mediterraneansea);
        //    tempMap.Add(alicante);

        //    cagliari.Name = "Cagliari";
        //    cagliari.Abbreviation = "CAG";
        //    cagliari.Type = LocationType.Town;
        //    cagliari.IsEastern = true;
        //    cagliari.BySea.Add(Location.mediterraneansea);
        //    cagliari.BySea.Add(Location.tyrrheniansea);
        //    tempMap.Add(cagliari);

        //    rome.Name = "Rome";
        //    rome.Abbreviation = "ROM";
        //    rome.Type = LocationType.City;
        //    rome.IsEastern = true;
        //    rome.ByRoad.Add(Location.florence);
        //    rome.ByRoad.Add(Location.bari);
        //    rome.ByRoad.Add(Location.naples);
        //    rome.ByTrain.Add(Location.florence);
        //    rome.ByTrain.Add(Location.naples);
        //    rome.BySea.Add(Location.tyrrheniansea);
        //    tempMap.Add(rome);

        //    naples.Name = "Naples";
        //    naples.Abbreviation = "NAP";
        //    naples.Type = LocationType.City;
        //    naples.IsEastern = true;
        //    naples.ByRoad.Add(Location.rome);
        //    naples.ByRoad.Add(Location.bari);
        //    naples.ByTrain.Add(Location.rome);
        //    naples.ByTrain.Add(Location.bari);
        //    naples.BySea.Add(Location.tyrrheniansea);
        //    tempMap.Add(naples);

        //    bari.Name = "Bari";
        //    bari.Abbreviation = "BAI";
        //    bari.Type = LocationType.Town;
        //    bari.IsEastern = true;
        //    bari.ByRoad.Add(Location.naples);
        //    bari.ByRoad.Add(Location.rome);
        //    bari.ByTrain.Add(Location.naples);
        //    bari.BySea.Add(Location.adriaticsea);
        //    tempMap.Add(bari);

        //    valona.Name = "Valona";
        //    valona.Abbreviation = "VAL";
        //    valona.Type = LocationType.Town;
        //    valona.IsEastern = true;
        //    valona.ByRoad.Add(Location.sarajevo);
        //    valona.ByRoad.Add(Location.sofia);
        //    valona.ByRoad.Add(Location.salonica);
        //    valona.ByRoad.Add(Location.athens);
        //    valona.BySea.Add(Location.ioniansea);
        //    tempMap.Add(valona);

        //    salonica.Name = "Salonica";
        //    salonica.Abbreviation = "SAL";
        //    salonica.Type = LocationType.Town;
        //    salonica.IsEastern = true;
        //    salonica.ByRoad.Add(Location.valona);
        //    salonica.ByRoad.Add(Location.sofia);
        //    salonica.ByTrain.Add(Location.sofia);
        //    salonica.BySea.Add(Location.ioniansea);
        //    tempMap.Add(salonica);

        //    athens.Name = "Athens";
        //    athens.Abbreviation = "ATH";
        //    athens.Type = LocationType.City;
        //    athens.IsEastern = true;
        //    athens.ByRoad.Add(Location.valona);
        //    athens.BySea.Add(Location.ioniansea);
        //    tempMap.Add(athens);

        //    atlanticocean.Name = "Atlantic Ocean";
        //    atlanticocean.Abbreviation = "ATL";
        //    atlanticocean.Type = LocationType.Sea;
        //    atlanticocean.IsEastern = false;
        //    atlanticocean.BySea.Add(Location.northsea);
        //    atlanticocean.BySea.Add(Location.irishsea);
        //    atlanticocean.BySea.Add(Location.englishchannel);
        //    atlanticocean.BySea.Add(Location.bayofbiscay);
        //    atlanticocean.BySea.Add(Location.mediterraneansea);
        //    atlanticocean.BySea.Add(Location.galway);
        //    atlanticocean.BySea.Add(Location.lisbon);
        //    atlanticocean.BySea.Add(Location.cadiz);
        //    tempMap.Add(atlanticocean);

        //    irishsea.Name = "Irish Sea";
        //    irishsea.Abbreviation = "IRI";
        //    irishsea.Type = LocationType.Sea;
        //    irishsea.IsEastern = false;
        //    irishsea.BySea.Add(Location.atlanticocean);
        //    irishsea.BySea.Add(Location.dublin);
        //    irishsea.BySea.Add(Location.liverpool);
        //    irishsea.BySea.Add(Location.swansea);
        //    tempMap.Add(irishsea);

        //    englishchannel.Name = "English Channel";
        //    englishchannel.Abbreviation = "ENG";
        //    englishchannel.Type = LocationType.Sea;
        //    englishchannel.IsEastern = false;
        //    englishchannel.BySea.Add(Location.atlanticocean);
        //    englishchannel.BySea.Add(Location.northsea);
        //    englishchannel.BySea.Add(Location.plymouth);
        //    englishchannel.BySea.Add(Location.london);
        //    englishchannel.BySea.Add(Location.lehavre);
        //    tempMap.Add(englishchannel);

        //    northsea.Name = "North Sea";
        //    northsea.Abbreviation = "NOR";
        //    northsea.Type = LocationType.Sea;
        //    northsea.IsEastern = false;
        //    northsea.BySea.Add(Location.atlanticocean);
        //    northsea.BySea.Add(Location.englishchannel);
        //    northsea.BySea.Add(Location.edinburgh);
        //    northsea.BySea.Add(Location.amsterdam);
        //    northsea.BySea.Add(Location.hamburg);
        //    tempMap.Add(northsea);

        //    bayofbiscay.Name = "Bay of Biscay";
        //    bayofbiscay.Abbreviation = "BAY";
        //    bayofbiscay.Type = LocationType.Sea;
        //    bayofbiscay.IsEastern = false;
        //    bayofbiscay.BySea.Add(Location.atlanticocean);
        //    bayofbiscay.BySea.Add(Location.nantes);
        //    bayofbiscay.BySea.Add(Location.bordeaux);
        //    bayofbiscay.BySea.Add(Location.santander);
        //    tempMap.Add(bayofbiscay);

        //    mediterraneansea.Name = "Mediterranean Sea";
        //    mediterraneansea.Abbreviation = "MED";
        //    mediterraneansea.Type = LocationType.Sea;
        //    mediterraneansea.IsEastern = true;
        //    mediterraneansea.BySea.Add(Location.atlanticocean);
        //    mediterraneansea.BySea.Add(Location.tyrrheniansea);
        //    mediterraneansea.BySea.Add(Location.alicante);
        //    mediterraneansea.BySea.Add(Location.barcelona);
        //    mediterraneansea.BySea.Add(Location.marseilles);
        //    mediterraneansea.BySea.Add(Location.cagliari);
        //    tempMap.Add(mediterraneansea);

        //    tyrrheniansea.Name = "Tyrrhenian Sea";
        //    tyrrheniansea.Abbreviation = "TYR";
        //    tyrrheniansea.Type = LocationType.Sea;
        //    tyrrheniansea.IsEastern = false;
        //    tyrrheniansea.BySea.Add(Location.mediterraneansea);
        //    tyrrheniansea.BySea.Add(Location.ioniansea);
        //    tyrrheniansea.BySea.Add(Location.cagliari);
        //    tyrrheniansea.BySea.Add(Location.genoa);
        //    tyrrheniansea.BySea.Add(Location.rome);
        //    tyrrheniansea.BySea.Add(Location.naples);
        //    tempMap.Add(tyrrheniansea);

        //    adriaticsea.Name = "Adriatic Sea";
        //    adriaticsea.Abbreviation = "ADR";
        //    adriaticsea.Type = LocationType.Sea;
        //    adriaticsea.IsEastern = false;
        //    adriaticsea.BySea.Add(Location.ioniansea);
        //    adriaticsea.BySea.Add(Location.bari);
        //    adriaticsea.BySea.Add(Location.venice);
        //    tempMap.Add(adriaticsea);

        //    ioniansea.Name = "Ionian Sea";
        //    ioniansea.Abbreviation = "ION";
        //    ioniansea.Type = LocationType.Sea;
        //    ioniansea.IsEastern = false;
        //    ioniansea.BySea.Add(Location.mediterraneansea);
        //    ioniansea.BySea.Add(Location.adriaticsea);
        //    ioniansea.BySea.Add(Location.blacksea);
        //    ioniansea.BySea.Add(Location.valona);
        //    ioniansea.BySea.Add(Location.athens);
        //    ioniansea.BySea.Add(Location.salonica);
        //    tempMap.Add(ioniansea);

        //    blacksea.Name = "Black Sea";
        //    blacksea.Abbreviation = "BLA";
        //    blacksea.Type = LocationType.Sea;
        //    blacksea.IsEastern = false;
        //    blacksea.BySea.Add(Location.ioniansea);
        //    blacksea.BySea.Add(Location.varna);
        //    blacksea.BySea.Add(Location.constanta);
        //    tempMap.Add(blacksea);

        //    return tempMap;
        //}

        // remove
        internal LocationDetail LocationAtMapIndex(int v)
        {
            return Map[v];
        }

        // remove
        internal void SetLocationForHunterAt(int v, LocationDetail location)
        {
            Hunters[v].CurrentLocation = location;
            Logger.WriteToDebugLog(Hunters[v].Name + " started in " + location.Name);
            Logger.WriteToGameLog(Hunters[v].Name + " started in " + location.Name);
        }

        internal void PlaceDraculaAtStartLocation()
        {
            Dracula.CurrentLocation = Dracula.ChooseStartLocation(map, Hunters);
            Dracula.LocationTrail.Add(Dracula.CurrentLocation);
            Logger.WriteToDebugLog("Dracula started in " + Dracula.CurrentLocation.Name);
            Logger.WriteToGameLog("Dracula started in " + Dracula.CurrentLocation.Name);
        }

        // remove
        internal string TimeOfDay()
        {
            return TimesOfDay[Math.Max(0, TimeIndex)];
        }

        // remove
        internal void AdjustTime(int v)
        {
            TimeIndex += v;
        }

        // remove
        internal int Time()
        {
            return TimeIndex;
        }

        // remove
        internal void SetHunterAlly(string v)
        {
            HunterAlly = GetEventByNameFromEventDeck(v);
        }

        // remove
        internal string NameOfHunterAlly()
        {
            if (HuntersHaveAlly())
            {
                return HunterAlly.name;
            }
            else
            {
                return "No ally";
            }
        }

        // remove
        internal bool HuntersHaveAlly()
        {
            return HunterAlly != null;
        }

        // remove
        internal string NameOfDraculaAlly()
        {
            if (DraculaAlly == null)
            {
                return "No ally";
            }
            return DraculaAlly.name;
        }

        // remove
        internal void SetDraculaAlly(EventDetail allyDrawn)
        {
            DraculaAlly = allyDrawn;
        }

        // remove
        internal bool DraculaHasAlly()
        {
            return DraculaAlly != null;
        }

        // remove
        internal void AddEventToEventDiscard(EventDetail allyDiscarded)
        {
            EventDiscard.Add(allyDiscarded);
        }

        internal EventDetail GetEventByNameFromEventDeck(string v)
        {
            try
            {
                return EventDeck[EventDeck.FindIndex(card => card.name.ToLower().StartsWith(v.ToLower()))];
            }
            catch (ArgumentOutOfRangeException)
            {
                return new EventDetail("Unknown event", false, EventType.Keep);
            }
        }

        // remove
        internal string NameOfEventCardAtIndex(int eventIndex)
        {
            return EventDeck[eventIndex].name;
        }

        // remove
        internal bool EventCardIsDraculaCardAtIndex(int eventIndex)
        {
            return EventDeck[eventIndex].isDraculaCard;
        }

        // remove
        internal int IndexOfEventCardInEventDeck(string argument2)
        {
            return EventDeck.FindIndex(card => card.name.ToUpper().StartsWith(argument2.ToUpper()));
        }

        // remove
        internal void RemoveEventFromEventDeck(EventDetail eventCardDrawn)
        {
            EventDeck.Remove(eventCardDrawn);
        }

        internal EventDetail DrawEventCard()
        {
            return EventDeck[new Random().Next(0, EventDeck.Count())];
        }

        // remove
        internal void RemoveEncounterFromEncounterPool(EncounterDetail tempEncounter)
        {
            EncounterPool.Remove(tempEncounter);
        }

        internal EncounterDetail DrawEncounterFromEncounterPool()
        {
            return EncounterPool[new Random().Next(0, EncounterPool.Count())];
        }

        // remove
        internal void AddEncounterToEncounterPool(EncounterDetail encounterToDiscard)
        {
            EncounterPool.Add(encounterToDiscard);
        }

        internal void MoveHunterToLocationAtHunterIndex(int hunterIndex, LocationDetail locationToMoveTo, UserInterface ui)
        {
            foreach (int ind in Hunters[hunterIndex].HuntersInGroup)
            {
                Logger.WriteToDebugLog("Moved " + Hunters[ind].Name + " to " + locationToMoveTo.Name);
                Logger.WriteToGameLog(Hunters[ind].Name + " moved from " + Hunters[ind].CurrentLocation.Name + " to " + locationToMoveTo.Name);
                ui.TellUser(Hunters[ind].Name + " moved from " + Hunters[ind].CurrentLocation.Name + " to " + locationToMoveTo.Name);
                Hunters[ind].CurrentLocation = locationToMoveTo;
            }
        }

        private void PlayControlStorms(int hunterIndex, UserInterface ui)
        {
            List<LocationDetail> possiblePorts = new List<LocationDetail>();
            List<LocationDetail> extensionLocations = new List<LocationDetail>();
            foreach (LocationDetail loc in Hunters[hunterIndex].CurrentLocation.BySea)
            {
                possiblePorts.Add(loc);
            }
            foreach (LocationDetail loc in possiblePorts)
            {
                foreach (LocationDetail ext in loc.BySea)
                {
                    extensionLocations.Add(ext);
                }
            }
            foreach (LocationDetail loc in extensionLocations)
            {
                if (!possiblePorts.Contains(loc))
                {
                    possiblePorts.Add(loc);
                }
            }
            extensionLocations.Clear();
            foreach (LocationDetail loc in possiblePorts)
            {
                foreach (LocationDetail ext in loc.BySea)
                {
                    extensionLocations.Add(ext);
                }
            }
            foreach (LocationDetail loc in extensionLocations)
            {
                if (!possiblePorts.Contains(loc))
                {
                    possiblePorts.Add(loc);
                }
            }
            extensionLocations.Clear();
            foreach (LocationDetail loc in possiblePorts)
            {
                if (loc.Type != LocationType.City && loc.Type != LocationType.Town)
                {
                    extensionLocations.Add(loc);
                }
            }
            foreach (LocationDetail ext in extensionLocations)
            {
                possiblePorts.Remove(ext);
            }
            LocationDetail locationToSendHunterTo = Dracula.DecideWhereToSendHunterWithControlStorms(hunterIndex, possiblePorts, this);
            MoveHunterToLocationAtHunterIndex(hunterIndex, locationToSendHunterTo, ui);
            DiscardEventFromDracula("Control Storms");
        }

        public void DiscardEventFromDracula(string cardName)
        {
            EventDetail cardToDiscard = Dracula.EventCardsInHand.Find(card => card.name == cardName);
            EventDiscard.Add(cardToDiscard);
            Dracula.EventCardsInHand.Remove(cardToDiscard);
        }

        // remove
        internal string NameOfHunterAtIndex(int hunterIndex)
        {
            return Hunters[hunterIndex].Name;
        }

        // remove
        internal void DrawEncounterAtCatacombIndex(int i, bool v)
        {
            Dracula.Catacombs[i].DrawEncounter(true);
        }

        // remove
        internal void DrawEncounterAtTrailIndex(int i)
        {
            Dracula.LocationTrail[i].DrawEncounter();
        }

        // remove
        internal void DrawEncounterAtCatacombIndex(int i)
        {
            Dracula.Catacombs[i].DrawEncounter();
        }

        // remove
        internal int NumberOfEncountersAtLocationAtCatacombIndex(int i)
        {
            return Dracula.Catacombs[i].Encounters.Count();
        }

        // remove
        internal string DraculaPowerNameAtPowerIndex(int i)
        {
            return Dracula.Powers[i].name;
        }

        // remove
        internal bool DraculaPowerAtPowerIndexIsAtLocationIndex(int i, int counter)
        {
            return Dracula.Powers[i].positionInTrail == counter;
        }

        // remove
        internal int NumberOfDraculaPowers()
        {
            return Dracula.Powers.Count();
        }

        // remove
        internal int NumberOfEventCardsInDraculaHand()
        {
            return Dracula.EventCardsInHand.Count();
        }

        // remove
        internal string LocationAbbreviationAtCatacombIndex(int i)
        {
            return Dracula.Catacombs[i].Abbreviation;
        }

        // remove
        internal bool LocationIsRevealedAtCatacombIndex(int i)
        {
            return Dracula.Catacombs[i].IsRevealed;
        }

        // remove
        internal bool LocationIsEmptyAtCatacombIndex(int i)
        {
            return Dracula.Catacombs[i] == null;
        }

        // remove
        internal int VampireTracker()
        {
            return VampirePointTracker;
        }

        // remove
        internal int DraculaBloodLevel()
        {
            return Dracula.Blood;
        }

        // remove
        internal void DrawLocationAtTrailIndex(int i)
        {
            Dracula.LocationTrail[i].DrawLocation();
        }

        // remove
        internal string NameOfLocationAtTrailIndex(int checkingLocationIndex)
        {
            return Dracula.LocationTrail[checkingLocationIndex].Name;
        }

        // remove
        internal void RevealHide(UserInterface ui)
        {
            Dracula.RevealHide(ui);
        }

        // remove
        internal bool LocationWhereHideWasUsedIsDraculaCurrentLocation()
        {
            return Dracula.LocationWhereHideWasUsed == Dracula.CurrentLocation;
        }

        // remove
        internal bool DraculaCurrentLocationIsAtTrailIndex(int checkingLocationIndex)
        {
            return Dracula.LocationTrail[checkingLocationIndex] == Dracula.CurrentLocation;
        }

        // remove
        internal bool LocationIsRevealedAtTrailIndex(int checkingLocationIndex)
        {
            return Dracula.LocationTrail[checkingLocationIndex].IsRevealed;
        }

        // remove
        internal LocationType TypeOfLocationAtTrailIndex(int checkingLocationIndex)
        {
            return Dracula.LocationTrail[checkingLocationIndex].Type;
        }

        // remove
        internal int TrailLength()
        {
            return Dracula.LocationTrail.Count();
        }

        // remove
        internal bool LocationIsInTrail(Map map, Location locationToReveal)
        {
            return Dracula.LocationTrail.Contains(locationToReveal);
        }

        internal void PerformDraculaTurn(UserInterface ui)
        {
            if (Dracula.CurrentLocation.Type != LocationType.Sea)
            {
                TimeIndex = (TimeIndex + 1) % 6;
                Logger.WriteToDebugLog("Time is now " + TimesOfDay[TimeIndex]);
                if (TimeIndex == 0)
                {
                    VampirePointTracker++;
                    Resolve++;
                    Logger.WriteToDebugLog("Increasing vampire track to " + VampirePointTracker);
                    Logger.WriteToDebugLog("Increasing resolve to " + Resolve);
                    if (VampirePointTracker > 0)
                    {
                        Logger.WriteToGameLog("Dracula earned a point, up to " + VampirePointTracker);
                        Logger.WriteToGameLog("Hunters gained a point of resolve, up to " + Resolve);
                    }
                }
            }
            else
            {
                Logger.WriteToDebugLog("Dracula is at sea, skipping Timekeeping phase so time remains " + TimesOfDay[Math.Max(0, TimeIndex)]);
            }
            EventDetail draculaEventCard = null;
            do
            {
                draculaEventCard = Dracula.PlayEventCardAtStartOfDraculaTurn(this);
                if (draculaEventCard != null)
                {
                    switch (draculaEventCard.name)
                    {
                        case "Time Runs Short":
                            ui.TellUser("Dracula played Time Runs Short");
                            DiscardEventFromDracula(draculaEventCard.name);
                            int hunterPlayingGoodluckC = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                            if (hunterPlayingGoodluckC > -1)
                            {
                                DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluckC, ui);
                            }
                            else
                            {
                                TimeIndex++;
                            }
                            break;
                        case "Unearthly Swiftness":
                            ui.TellUser("Dracula played Unearthly Swiftness");
                            int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                            if (hunterPlayingGoodluck > -1)
                            {
                                DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                            }
                            else
                            {
                                DiscardEventFromDracula(draculaEventCard.name);
                                Dracula.DraculaMovementPhase(this, ui);
                                Dracula.DoActionPhase(this, ui);
                            }
                            break;
                        case "Roadblock":
                            ui.TellUser("Dracula played Roadblock ");
                            DiscardEventFromDracula("Roadblock");
                            int hunterPlayingGoodluckB = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                            if (hunterPlayingGoodluckB > -1)
                            {
                                DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluckB, ui);
                            }
                            else
                            {
                                Dracula.PlaceRoadBlockCounter(this, RoadblockCounter);
                                ui.TellUser("Dracula played Roadblock and placed the Roadblock counter on the " + RoadblockCounter.connectionType + " between " + RoadblockCounter.firstLocation + " and " + RoadblockCounter.secondLocation);
                            }
                            break;
                        case "Devilish Power":
                            Dracula.PlayDevilishPowerToRemoveHeavenlyHostOrHunterAlly(this, ui); // Good luck is handled within this function
                            DiscardEventFromDracula("Devilish Power");
                            break;
                    }
                }
            } while (draculaEventCard != null);
            Dracula.TakeStartOfTurnActions(this, ui);
            Dracula.DraculaMovementPhase(this, ui);
            Dracula.HandleDroppedOffLocations(this, ui);
            Dracula.DoActionPhase(this, ui);
            Dracula.MatureEncounters(this, ui);
            Dracula.DrawEncounters(this, Dracula.EncounterHandSize);
            foreach (EncounterDetail enc in EncounterLimbo)
            {
                EncounterPool.Add(enc);
                enc.isRevealed = false;
            }
            EncounterLimbo.Clear();
            if (HuntersHaveAlly())
            {
                if (HunterAlly.name == "Jonathan Harker")
                {
                    if (Dracula.LocationTrail.Count() > 5)
                    {
                        Dracula.LocationTrail[5].IsRevealed = true;
                    }
                }
            }
        }

        // remove
        internal void RevealEncounterInTrail(int v)
        {
            foreach (EncounterDetail enc in Dracula.LocationTrail[v].Encounters)
            {
                enc.isRevealed = true;
            }
        }

        // remove
        internal void TrimDraculaTrail(int trailLength)
        {
            Dracula.TrimTrail(this, Math.Max(1, trailLength));
        }

        // remove
        internal void DiscardDraculaCardsDownToHandSize(UserInterface ui)
        {
            Dracula.DiscardEventsDownTo(this, Dracula.EventHandSize, ui);
        }

        // remove
        internal void DrawEventCardForDracula(UserInterface ui)
        {
            Dracula.DrawEventCard(this, ui);
        }

        internal void RevealLocationAtTrailIndex(int trailIndex, UserInterface ui)
        {
            if (Dracula.LocationTrail[trailIndex].Name == "Hide")
            {
                Dracula.RevealHide(ui);
            }
            else
            {
                Dracula.LocationTrail[trailIndex].IsRevealed = true;
            }
        }

        // remove
        internal string DraculaCurrentLocationName()
        {
            return Dracula.CurrentLocation.Name;
        }

        public LocationDetail GetLocationFromName(string locationName)
        {
            for (int i = 0; i < 71; i++)
            {
                if ((Map[i].Name.ToLower().StartsWith(locationName.ToLower())) || (Map[i].Abbreviation.ToLower() == locationName.ToLower()))
                {
                    string tempName = Map[i].Name;
                    Map[i].Name = "";
                    for (int j = i; j < 71; j++)
                    {
                        if (Map[j].Name.ToLower().StartsWith(locationName.ToLower()))
                        {
                            LocationDetail multipleLocations = new LocationDetail();
                            multipleLocations.Name = "Multiple locations";
                            Map[i].Name = tempName;
                            return multipleLocations;
                        }
                    }
                    Map[i].Name = tempName;
                    return Map[i];
                }
            }
            LocationDetail unknownLocation = new LocationDetail();
            unknownLocation.Name = "Unknown location";
            return unknownLocation;
        }

        public void MatureEncounter(string encounterName, UserInterface ui)
        {
            switch (encounterName)
            {
                case "Ambush": MatureAmbush(ui); break;
                case "Assasin": MatureAssassin(ui); break;
                case "Bats": MatureBats(ui); break;
                case "Desecrated Soil": MatureDesecratedSoil(ui); break;
                case "Fog": MatureFog(ui); break;
                case "Minion with Knife": MatureMinionWithKnife(ui); break;
                case "Minion with Knife and Pistol": MatureMinionWithKnifeAndPistol(ui); break;
                case "Minion with Knife and Rifle": MatureMinionWithKnifeAndRifle(ui); break;
                case "Hoax": MatureHoax(ui); break;
                case "Lightning": MatureLightning(ui); break;
                case "Peasants": MaturePeasants(ui); break;
                case "Plague": MaturePlague(ui); break;
                case "Rats": MatureRats(ui); break;
                case "Saboteur": MatureSaboteur(ui); break;
                case "Spy": MatureSpy(ui); break;
                case "Thief": MatureThief(ui); break;
                case "New Vampire": MatureNewVampire(ui); break;
                case "Wolves": MatureWolves(ui); break;
            }
        }

        private void MatureWolves(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Wolves (no effect)");
            Logger.WriteToGameLog("Dracula matured Wolves (no effect)");
        }

        private void MatureNewVampire(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured New Vampire");
            Logger.WriteToGameLog("Dracula matured New Vampire");
            ui.TellUser("Dracula matured a New Vampire");
            VampirePointTracker += 2;
            Dracula.TrimTrail(this, 1);
        }

        private void MatureThief(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Thief (no effect)");
            Logger.WriteToGameLog("Dracula matured Thief (no effect)");
        }

        private void MatureSpy(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Spy (no effect)");
            Logger.WriteToGameLog("Dracula matured Spy (no effect)");
        }

        private void MatureSaboteur(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Saboteur (no effect)");
            Logger.WriteToGameLog("Dracula matured Saboteur (no effect)");
        }

        private void MatureRats(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Rats (no effect)");
            Logger.WriteToGameLog("Dracula matured Rats (no effect)");
        }

        private void MaturePlague(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Plague (no effect)");
            Logger.WriteToGameLog("Dracula matured Plague (no effect)");
        }

        private void MaturePeasants(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Peasants (no effect)");
            Logger.WriteToGameLog("Dracula matured Peasants (no effect)");
        }

        private void MatureLightning(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Lightning (no effect)");
            Logger.WriteToGameLog("Dracula matured Lightning (no effect)");
        }

        private void MatureHoax(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Hoax (no effect)");
            Logger.WriteToGameLog("Dracula matured Hoax (no effect)");
        }

        private void MatureMinionWithKnifeAndRifle(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Minion With Knife and Rifle (no effect)");
            Logger.WriteToGameLog("Dracula matured Minion With Knife and Rifle (no effect)");
        }

        private void MatureMinionWithKnifeAndPistol(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Minion With Knife and Pistol (no effect)");
            Logger.WriteToGameLog("Dracula matured Minion With Knife and Pistol (no effect)");
        }

        private void MatureMinionWithKnife(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Minion With Knife (no effect)");
            Logger.WriteToGameLog("Dracula matured Minion With Knife (no effect)");
        }

        private void MatureFog(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Fog (no effect)");
            Logger.WriteToGameLog("Dracula matured Fog (no effect)");
        }

        private void MatureDesecratedSoil(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Desecrated Soil");
            Logger.WriteToGameLog("Dracula matured Desecrated Soil");
            ui.TellUser("Dracula matured Desecrated Soil");
            for (int i = 0; i < 2; i++)
            {
                EventDetail cardDrawn;
                string line;
                do
                {
                    line = ui.GetEventCardDrawnByDesecratedSoil();
                    cardDrawn = GetEventByNameFromEventDeck(line);
                } while (cardDrawn.name == "Unknown event" && !"dracula".StartsWith(line.ToLower()));
                if (!"dracula".StartsWith(line.ToLower()))
                {
                    ui.TellUser(cardDrawn.name + " is discarded");
                    EventDeck.Remove(cardDrawn);
                    EventDiscard.Add(cardDrawn);
                }
                else
                {
                    switch (cardDrawn.type)
                    {
                        case EventType.Ally: Dracula.PlayAlly(this, cardDrawn, ui); break;
                        case EventType.Keep:
                            Dracula.EventCardsInHand.Add(cardDrawn);
                            EventDeck.Remove(cardDrawn);
                            break;
                        case EventType.PlayImmediately:
                            Dracula.PlayImmediately(this, cardDrawn, ui);
                            EventDeck.Remove(cardDrawn);
                            break;
                    }
                }
            }
            Dracula.DiscardEventsDownTo(this, Dracula.EventHandSize, ui);
            Dracula.TrimTrail(this, 3);
        }

        private void MatureBats(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Bats (no effect)");
            Logger.WriteToGameLog("Dracula matured Bats (no effect)");
        }

        private void MatureAssassin(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Assassin (no effect)");
            Logger.WriteToGameLog("Dracula matured Assassin (no effect)");
        }

        private void MatureAmbush(UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula matured Ambush");
            Logger.WriteToGameLog("Dracula matured Ambush");
            ui.TellUser("Dracula matured Ambush");
            bool discard;
            ResolveEncounter(Dracula.ChooseEncounterToAmbushHunter(), Dracula.ChooseHunterToAmbush(Hunters), out discard, ui);
        }

        private bool ResolveAmbush(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Ambush");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Ambush");
            ui.TellUser(huntersEncountered.First().Name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].Name + " ");
            }
            ui.TellUser("encountered an Ambush");
            Dracula.DrawEncounters(this, Dracula.EncounterHand.Count() + 1);
            Dracula.DiscardEncountersDownTo(this, Dracula.EncounterHandSize);
            return true;
        }

        public bool ResolveAssassin(List<Hunter> huntersEncountered, UserInterface ui)
        {
            ui.TellUser("Conduct a combat with an Assasin");
            int hunterIndex = 0;
            switch (huntersEncountered[0].Name)
            {
                case "Lord Godalming": hunterIndex = 0; break;
                case "Van Helsing": hunterIndex = 1; break;
                case "Dr. Seward": hunterIndex = 2; break;
                case "Mina Harker": hunterIndex = 3; break;
            }
            if (ResolveCombat(hunterIndex, 5, true, ui) == "Enemy killed")
            {
                return true;
            }
            return false;
        }

        private bool ResolveBats(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Bats");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Bats");
            ui.TellUser(huntersEncountered.First().Name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].Name + " ");
            }
            ui.TellUser("encountered Bats");
            ui.TellUser("Tell me at the start of your next turn and I will move you");
            return false;
        }

        private bool ResolveDesecratedSoil(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Desecrated Soil");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Desecrated Soil");
            ui.TellUser(huntersEncountered.First().Name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].Name + " ");
            }
            ui.TellUser("encountered Desecrated Soil");
            EventDetail cardDrawn;
            string line;
            do
            {
                line = ui.GetEventCardDrawnByDesecratedSoil();
                cardDrawn = GetEventByNameFromEventDeck(line);
            } while (cardDrawn.name == "Unknown event" && !"dracula".StartsWith(line.ToLower()));
            if (!"dracula".StartsWith(line.ToLower()))
            {
                ui.TellUser(cardDrawn.name + " is discarded");
                EventDeck.Remove(cardDrawn);
                EventDiscard.Add(cardDrawn);
            }
            else
            {
                switch (cardDrawn.type)
                {
                    case EventType.Ally: Dracula.PlayAlly(this, cardDrawn, ui); break;
                    case EventType.Keep: Dracula.EventCardsInHand.Add(cardDrawn); break;
                    case EventType.PlayImmediately: Dracula.PlayImmediately(this, cardDrawn, ui); break;
                }
            }
            Dracula.DiscardEventsDownTo(this, Dracula.EventHandSize, ui);
            return true;
        }

        public bool ResolveFog(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Fog");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Fog");
            ui.TellUser(huntersEncountered.First().Name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i] + " ");
            }
            ui.TellUser("encountered Fog");
            ui.TellUser("Place Fog in front of you until the end of your next turn");
            return false;
        }

        public bool ResolveMinionWithKnife(List<Hunter> huntersEncountered, UserInterface ui)
        {
            int hunterIndex = 0;
            switch (huntersEncountered[0].Name)
            {
                case "Lord Godalming": hunterIndex = 0; break;
                case "Van Helsing": hunterIndex = 1; break;
                case "Dr. Seward": hunterIndex = 2; break;
                case "Mina Harker": hunterIndex = 3; break;
            }
            string combatResult = ResolveCombat(hunterIndex, 2, true, ui);
            if (combatResult == "Enemy killed" || combatResult == "End")
            {
                EventDetail draculaEventCard = Dracula.WillPlayRelentlessMinion(this, huntersEncountered, "Minion with Knife");
                if (draculaEventCard != null)
                {
                    switch (draculaEventCard.name)
                    {
                        case "Relentless Minion":
                            ui.TellUser("Dracula played Relentless Minion");
                            DiscardEventFromDracula("Relentless Minion");
                            int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                            if (hunterPlayingGoodluck > -1)
                            {
                                DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                            }
                            else
                            {

                                combatResult = ResolveCombat(hunterIndex, 2, true, ui);
                            }
                            break;
                    }
                }
                if (combatResult == "Enemy killed")
                {
                    return true;
                }
            }
            return false;
        }

        public bool ResolveMinionWithKnifeAndPistol(List<Hunter> huntersEncountered, UserInterface ui)
        {
            int hunterIndex = 0;
            switch (huntersEncountered[0].Name)
            {
                case "Lord Godalming": hunterIndex = 0; break;
                case "Van Helsing": hunterIndex = 1; break;
                case "Dr. Seward": hunterIndex = 2; break;
                case "Mina Harker": hunterIndex = 3; break;
            }
            string combatResult = ResolveCombat(hunterIndex, 3, true, ui);
            if (combatResult == "Enemy killed" || combatResult == "End")
            {
                EventDetail draculaEventCard = Dracula.WillPlayRelentlessMinion(this, huntersEncountered, "Minion with Knife and Pistol");
                if (draculaEventCard != null)
                {
                    switch (draculaEventCard.name)
                    {
                        case "Relentless Minion":
                            ui.TellUser("Dracula played Relentless Minion");
                            DiscardEventFromDracula("Relentless Minion");
                            combatResult = ResolveCombat(hunterIndex, 3, true, ui);
                            break;
                    }
                }
                if (combatResult == "Enemy killed")
                {
                    return true;
                }
            }
            return false;
        }

        public bool ResolveMinionWithKnifeAndRifle(List<Hunter> huntersEncountered, UserInterface ui)
        {
            int hunterIndex = 0;
            switch (huntersEncountered[0].Name)
            {
                case "Lord Godalming": hunterIndex = 0; break;
                case "Van Helsing": hunterIndex = 1; break;
                case "Dr. Seward": hunterIndex = 2; break;
                case "Mina Harker": hunterIndex = 3; break;
            }
            string combatResult = ResolveCombat(hunterIndex, 4, true, ui);
            if (combatResult == "Enemy killed" || combatResult == "End")
            {
                EventDetail draculaEventCard = Dracula.WillPlayRelentlessMinion(this, huntersEncountered, "Minion with Knife and Rifle");
                if (draculaEventCard != null)
                {
                    switch (draculaEventCard.name)
                    {
                        case "Relentless Minion":
                            ui.TellUser("Dracula played Relentless Minion");
                            DiscardEventFromDracula("Relentless Minion");
                            combatResult = ResolveCombat(hunterIndex, 4, true, ui);
                            break;
                    }
                }
                if (combatResult == "Enemy killed")
                {
                    return true;
                }
            }
            return false;
        }

        public bool ResolveHoax(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Hoax");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Hoax");
            ui.TellUser(huntersEncountered.First().Name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].Name + " ");
            }
            ui.TellUser("encountered Hoax");
            ui.TellUser("Discard " + (huntersEncountered.First().CurrentLocation.IsEastern ? "one" : "all") + " of your event cards (don't forget to tell me what is discarded");
            return true;
        }

        public bool ResolveLightning(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Lightning");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Lightning");
            ui.TellUser(huntersEncountered.First().Name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].Name + " ");
            }
            ui.TellUser("encountered Lightning");
            for (int i = 0; i < huntersEncountered.Count(); i++)
            {
                int answer = ui.GetHunterHolyItems(huntersEncountered[i].Name);
                if (answer > 0)
                {
                    Logger.WriteToDebugLog(huntersEncountered[i].Name + " negated the encounter with " + (answer == 1 ? "a crucifix" : "a heavenly host"));
                    Logger.WriteToGameLog(huntersEncountered[i].Name + " negated the encounter with " + (answer == 1 ? "a crucifix" : "a heavenly host"));
                    ui.TellUser(huntersEncountered[i].Name + " negated the encounter with " + (answer == 1 ? "a crucifix" : "a heavenly host"));
                    return true;
                }
            }
            for (int i = 0; i < huntersEncountered.Count(); i++)
            {
                Logger.WriteToDebugLog(huntersEncountered[i].Name + " loses 2 health and discards 1 item");
                Logger.WriteToGameLog(huntersEncountered[i].Name + " loses 2 health and discards 1 item");
                ui.TellUser(huntersEncountered[i].Name + " loses 2 health and discards 1 item");
                huntersEncountered[i].Health -= 2;
            }
            ui.TellUser("Don't forget to tell me what was discarded");
            return !HandlePossibleHunterDeath(ui);
        }

        public bool ResolvePeasants(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Peasants");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Peasants");
            ui.TellUser(huntersEncountered.First().Name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].Name + " ");
            }
            ui.TellUser("encountered Peasants");
            ui.TellUser("Discard " + (huntersEncountered.First().CurrentLocation.IsEastern ? "one" : "all") + " of your item cards and redraw randomly (don't forget to tell me what is discarded");
            return true;
        }

        private bool ResolvePlague(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Plague");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Plague");
            ui.TellUser(huntersEncountered.First().Name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].Name + " ");
            }
            ui.TellUser("encountered Plague");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                Logger.WriteToDebugLog(huntersEncountered[i].Name + " loses 2 health");
                Logger.WriteToGameLog(huntersEncountered[i].Name + " loses 2 health");
                ui.TellUser(huntersEncountered[i].Name + " loses 2 health");
                huntersEncountered[i].Health -= 2;
            }
            return !HandlePossibleHunterDeath(ui);
        }

        public bool ResolveRats(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Rats");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Rats");
            ui.TellUser(huntersEncountered.First().Name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].Name + " ");
            }
            ui.TellUser("encountered Rats");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                if (huntersEncountered[i].HasDogsFaceUp)
                {
                    ui.TellUser(huntersEncountered[i].Name + " has Dogs face up, Rats have no effect");
                    Logger.WriteToDebugLog(huntersEncountered[i].Name + " has Dogs face up, Rats have no effect");
                    Logger.WriteToGameLog(huntersEncountered[i].Name + " has Dogs face up, Rats have no effect");
                    return true;
                }
            }
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("Roll dice for " + huntersEncountered[i].Name);
                int loss = ui.GetHunterHealthLost(huntersEncountered[i].Name);
                huntersEncountered[i].Health -= loss;
                Logger.WriteToDebugLog(huntersEncountered[i] + " lost " + loss + " health");
                Logger.WriteToGameLog(huntersEncountered[i] + " lost " + loss + " health");
            }
            return !HandlePossibleHunterDeath(ui);
        }

        public bool ResolveSaboteur(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Saboteur");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Saboteur");
            ui.TellUser(huntersEncountered.First().Name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].Name + " ");
            }
            ui.TellUser("encountered Saboteur");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                if (huntersEncountered[i].HasDogsFaceUp)
                {
                    ui.TellUser(huntersEncountered[i].Name + " has Dogs face up, Saboteur has no effect");
                    Logger.WriteToDebugLog(huntersEncountered[i].Name + " has Dogs face up, Saboteur has no effect");
                    Logger.WriteToGameLog(huntersEncountered[i].Name + " has Dogs face up, Saboteur has no effect");
                    return true;
                }
            }
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser(huntersEncountered[i].Name + " must discard 1 item or event (don't forget to tell me what was discarded");
            }
            return false;
        }

        public bool ResolveSpy(List<Hunter> huntersEncountered, UserInterface ui)
        {
            ui.TellUser(huntersEncountered.First().Name + " encountered a spy" + (huntersEncountered.Count() > 1 ? " with his group" : ""));
            foreach (Hunter h in huntersEncountered)
            {
                List<String> itemsAlreadyKnownToDracula = new List<String>();
                for (int i = 0; i < h.NumberOfItems; i++)
                {
                    string line = "";
                    do
                    {
                        line = ui.GetNameOfItemInHandFromHunter(h.Name);
                        if (GetItemByNameFromItemDeck(line).Name == "Unknown item" && h.ItemsKnownToDracula.FindIndex(item => item.Name == line) == -1)
                        {
                            ui.TellUser("I didn't recognise that card name");
                        }
                    } while (GetItemByNameFromItemDeck(line).Name == "Unknown item" && h.ItemsKnownToDracula.FindIndex(item => item.Name == line) == -1);
                    itemsAlreadyKnownToDracula.Add(line);
                }
                List<ItemDetail> itemsToAddToKnownItems = new List<ItemDetail>();
                itemsToAddToKnownItems.AddRange(h.ItemsKnownToDracula);
                foreach (string name in itemsAlreadyKnownToDracula)
                {
                    if (itemsToAddToKnownItems.FindIndex(it => it.Name == name) > -1)
                    {
                        itemsToAddToKnownItems.Remove(itemsToAddToKnownItems.Find(it => it.Name == name));
                    }
                    else
                    {
                        h.ItemsKnownToDracula.Add(GetItemByNameFromItemDeck(name));
                    }
                }
                List<String> eventsAlreadyKnownToDracula = new List<String>();
                for (int i = 0; i < h.NumberOfItems; i++)
                {
                    string line = "";
                    do
                    {
                        line = ui.GetNameOfEventInHandFromHunter(h.Name);
                        if (GetEventByNameFromEventDeck(line).name == "Unknown event" && h.EventsKnownToDracula.FindIndex(ev => ev.name == line) == -1)
                        {
                            ui.TellUser("I didn't recognise that card name");
                        }
                    } while (GetEventByNameFromEventDeck(line).name == "Unknown event" && h.EventsKnownToDracula.FindIndex(ev => ev.name == line) == -1);
                    eventsAlreadyKnownToDracula.Add(line);
                }
                List<EventDetail> eventsToAddToKnownEvents = new List<EventDetail>();
                eventsToAddToKnownEvents.AddRange(h.EventsKnownToDracula);
                foreach (string name in eventsAlreadyKnownToDracula)
                {
                    if (eventsToAddToKnownEvents.FindIndex(eve => eve.name == name) > -1)
                    {
                        eventsToAddToKnownEvents.Remove(eventsToAddToKnownEvents.Find(eve => eve.name == name));
                    }
                    else
                    {
                        h.EventsKnownToDracula.Add(GetEventByNameFromEventDeck(name));
                    }
                }
                h.TravelType = ui.AskHunterWhatTravelTypeForSpy(h.Name);
                string lineB;
                do
                {
                    lineB = ui.AskHunterWhichLocationTheyAreMovingToNextTurn(h.Name);
                } while (GetLocationFromName(lineB).Name == "Unknown location" || GetLocationFromName(lineB).Name == "Multiple locations");
                h.Destination = GetLocationFromName(lineB);
            }
            return true;
        }

        public bool ResolveThief(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Thief");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Thief");
            ui.TellUser(huntersEncountered.First().Name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].Name + " ");
            }
            ui.TellUser("encountered Thief");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                if (huntersEncountered[i].HasDogsFaceUp)
                {
                    ui.TellUser(huntersEncountered[i].Name + " has Dogs face up, Thief has no effect");
                    Logger.WriteToDebugLog(huntersEncountered[i].Name + " has Dogs face up, Thief has no effect");
                    Logger.WriteToGameLog(huntersEncountered[i].Name + " has Dogs face up, Thief has no effect");
                    return true;
                }
            }
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                Dracula.DiscardHunterCard(this, huntersEncountered[i], ui);
            }
            return true;
        }

        public bool ResolveNewVampire(List<Hunter> huntersEncountered, out bool discard, UserInterface ui)
        {
            if (TimeIndex < 3)
            {
                ui.TellUser(huntersEncountered[0].Name + " encountered a New Vampire and disposed of it during the day");
                discard = true;
                return true;
            }
            else
            {
                EventDetail draculaEventCard = Dracula.PlaySeductionCard(this);
                DiscardEventFromDracula("Seduction");
                ui.TellUser("Dracula played Seduction");
                int dieRoll;
                if (draculaEventCard != null)
                {
                    int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                    if (hunterPlayingGoodluck > -1)
                    {
                        DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                        dieRoll = ui.GetDieRoll();
                    }
                    else
                    {
                        dieRoll = 4;
                    }
                }
                else
                {
                    dieRoll = ui.GetDieRoll();
                }
                if (dieRoll < 4)
                {
                    ui.TellUser("The Vampire attempts to bite you");
                    for (int i = 0; i < huntersEncountered.Count(); i++)
                    {
                        int answer;
                        if (draculaEventCard != null)
                        {
                            answer = 0;
                        }
                        else
                        {
                            answer = ui.GetHunterHolyItems(huntersEncountered[i].Name);
                        }
                        if (answer > 0)
                        {
                            Logger.WriteToDebugLog(huntersEncountered[i].Name + " negated the encounter with " + (answer == 1 ? "a crucifix" : "a heavenly host"));
                            Logger.WriteToGameLog(huntersEncountered[i].Name + " negated the encounter with " + (answer == 1 ? "a crucifix" : "a heavenly host"));
                            ui.TellUser(huntersEncountered[i].Name + " negated the encounter with " + (answer == 1 ? "a crucifix" : "a heavenly host"));
                            discard = true;
                            return true;
                        }
                    }
                    foreach (Hunter h in huntersEncountered)
                    {
                        Logger.WriteToDebugLog(h.Name + " is bitten");
                        Logger.WriteToGameLog(h.Name + " is bitten");
                        ui.TellUser(h.Name + " is bitten");
                        discard = true;
                        return (!HandlePossibleHunterDeath(ui));
                    }
                    discard = true;
                    return true;
                }
                else
                {
                    ui.TellUser("The Vampire attempts to escape");
                    for (int i = 0; i < huntersEncountered.Count(); i++)
                    {
                        int answer = ui.GetHunterSharpItems(huntersEncountered[i].Name);
                        if (answer > 0)
                        {
                            Logger.WriteToDebugLog(huntersEncountered[i].Name + " prevented the Vampire escaping with " + (answer == 1 ? "a Knife" : "a Stake"));
                            Logger.WriteToGameLog(huntersEncountered[i].Name + " prevented the Vampire escaping with " + (answer == 1 ? "a Knife" : "a Stake"));
                            ui.TellUser(huntersEncountered[i].Name + " prevented the Vampire escaping with " + (answer == 1 ? "a Knife" : "a Stake"));
                            ui.TellUser("Don't forget to discard the item");
                            discard = true;
                            return true;
                        }
                    }
                    ui.TellUser("The Vampire escaped");
                    discard = false;
                    return true;
                }
            }
        }

        public bool ResolveWolves(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Wolves");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Wolves");
            ui.TellUser(huntersEncountered.First().Name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].Name + " ");
            }
            ui.TellUser("encountered Wolves");
            bool hasPistol = false;
            bool hasRifle = false;
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                int answer = ui.GetHunterEquipmentForWolves(huntersEncountered[i].Name);
                switch (answer)
                {
                    case 1: hasPistol = true; break;
                    case 2: hasRifle = true; break;
                    case 3: hasPistol = true; hasRifle = true; break;
                }
            }
            int numberOfWeaponTypes = (hasPistol ? 1 : 0) + (hasRifle ? 1 : 0);
            if (numberOfWeaponTypes == 2)
            {
                Logger.WriteToDebugLog("Wolves are negated by Pistol and Rifle");
                Logger.WriteToGameLog("Wolves are negated by Pistol and Rifle");
                ui.TellUser("Wolves are negated by Pistol and Rifle");
            }
            else
            {
                for (int i = 1; i < huntersEncountered.Count(); i++)
                {
                    Logger.WriteToDebugLog(huntersEncountered[i].Name + " loses " + (numberOfWeaponTypes == 1 ? "1" : "2") + " health");
                    Logger.WriteToGameLog(huntersEncountered[i].Name + " loses " + (numberOfWeaponTypes == 1 ? "1" : "2") + " health");
                    ui.TellUser(huntersEncountered[i].Name + " loses " + (numberOfWeaponTypes == 1 ? "1" : "2") + " health");
                    huntersEncountered[i].Health -= (2 - numberOfWeaponTypes);
                }

            }
            return !HandlePossibleHunterDeath(ui);
        }

        // remove
        internal void DrawEncountersUpToHandSize()
        {
            Dracula.DrawEncounters(this, Dracula.EncounterHandSize);
        }

        internal bool DraculaCancelTrain(int hunterIndex, UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula is deciding whether to cancel the train");

            if (Dracula.WillCancelTrain(this, Hunters[hunterIndex]))
            {
                PlayFalseTipOff(ui);
                return true;
            }
            return false;
        }

        private void PlayFalseTipOff(UserInterface ui)
        {
            ui.TellUser("I am playing my False Tip-Off card to cancel your train");
            Dracula.DiscardEventFromHand(this, "False Tip-Off");
            int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
            if (hunterPlayingGoodluck > -1)
            {
                DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
            }
        }

        internal void AddEventCardToHunterAtIndex(int hunterIndex, UserInterface ui)
        {
            Hunters[hunterIndex].NumberOfEvents++;
            Logger.WriteToDebugLog(Hunters[hunterIndex].Name + " drew an event card, up to " + Hunters[hunterIndex].NumberOfEvents);
            Logger.WriteToGameLog(Hunters[hunterIndex].Name + " drew an event card, up to " + Hunters[hunterIndex].NumberOfEvents);
            CheckBittenHunterCards(ui);
        }

        internal void AddItemCardToHunterAtIndex(int hunterIndex, UserInterface ui)
        {
            Hunters[hunterIndex].NumberOfItems++;
            Logger.WriteToDebugLog(Hunters[hunterIndex].Name + " drew an item card, up to " + Hunters[hunterIndex].NumberOfItems);
            Logger.WriteToGameLog(Hunters[hunterIndex].Name + " drew an item card, up to " + Hunters[hunterIndex].NumberOfItems);
            CheckBittenHunterCards(ui);
        }

        // remove
        internal int ResolveTracker()
        {
            return Resolve;
        }

        internal void DiscardEventFromHunterAtIndex(string eventName, int hunterIndex, UserInterface ui)
        {
            EventDetail eventToDiscard = GetEventByNameFromEventDeck(eventName);
            EventDeck.Remove(eventToDiscard);
            if (Hunters[hunterIndex].EventShownToDraculaForBeingBitten == eventToDiscard)
            {
                Hunters[hunterIndex].EventShownToDraculaForBeingBitten = null;
                CheckBittenHunterCards(ui);
            }
            EventDiscard.Add(eventToDiscard);
            Hunters[hunterIndex].NumberOfEvents--;
            Logger.WriteToDebugLog(Hunters[hunterIndex].Name + " discarded " + eventName + " down to " + Hunters[hunterIndex].NumberOfEvents);
            Logger.WriteToGameLog(Hunters[hunterIndex].Name + " discarded " + eventName + " down to " + Hunters[hunterIndex].NumberOfEvents);
        }

        // remove
        internal int NumberOfEventCardsAtHunterIndex(int hunterIndex)
        {
            return Hunters[hunterIndex].NumberOfEvents;
        }

        // remove
        internal int NumberOfItemCardsAtHunterIndex(int hunterIndex)
        {
            return Hunters[hunterIndex].NumberOfItems;
        }

        internal string HunterShouldDiscardAtHunterIndex(int hunterIndex)
        {
            if (hunterIndex == 1)
            {
                if (Hunters[1].NumberOfEvents == 4 && Hunters[1].NumberOfItems == 4)
                {
                    return "item or event";
                }
                else if (Hunters[1].NumberOfEvents > 4)
                {
                    return "event";
                }
                else if (Hunters[1].NumberOfItems > 4)
                {
                    return "item";
                }
            }
            else
            {
                if (Hunters[hunterIndex].NumberOfEvents > 3)
                {
                    return "event";
                }
                if (Hunters[hunterIndex].NumberOfItems > 3)
                {
                    return "item";
                }
            }
            return "I don't know, bro";
        }

        internal ItemDetail GetItemByNameFromItemDeck(string argument2)
        {
            try
            {
                return ItemDeck[ItemDeck.FindIndex(card => card.Name.ToLower().StartsWith(argument2.ToLower()))];
            }
            catch (ArgumentOutOfRangeException)
            {
                return new ItemDetail("Unknown item");
            }
        }

        internal void DiscardItemFromHunterAtIndex(string itemName, int hunterIndex, UserInterface ui)
        {
            ItemDetail itemToDiscard;
            if (Hunters[hunterIndex].ItemsKnownToDracula.FindIndex(i => i.Name == itemName) > -1)
            {
                itemToDiscard = Hunters[hunterIndex].ItemsKnownToDracula.Find(i => i.Name == itemName);
                Hunters[hunterIndex].ItemsKnownToDracula.Remove(itemToDiscard);
            }
            else
            {
                itemToDiscard = GetItemByNameFromItemDeck(itemName);
                ItemDeck.Remove(itemToDiscard);
            }
            if (Hunters[hunterIndex].ItemShownToDraculaForBeingBitten == itemToDiscard)
            {
                Hunters[hunterIndex].ItemShownToDraculaForBeingBitten = null;
                CheckBittenHunterCards(ui);
            }
            ItemDiscard.Add(itemToDiscard);
            Hunters[hunterIndex].NumberOfItems--;
            if (itemName == "Dogs")
            {
                Hunters[hunterIndex].HasDogsFaceUp = false;
            }
            Logger.WriteToDebugLog(Hunters[hunterIndex].Name + " discarded " + itemName + " down to " + Hunters[hunterIndex].NumberOfEvents);
            Logger.WriteToGameLog(Hunters[hunterIndex].Name + " discarded " + itemName + " down to " + Hunters[hunterIndex].NumberOfEvents);
        }

        internal string ResolveCombat(Hunter hunter, int enemyInCombat, bool hunterMoved, UserInterface ui)
        {
            List<ItemDetail> enemyCombatCards = new List<ItemDetail>();
            List<ItemDetail> hunterBasicCards = new List<ItemDetail>();
            List<Hunter> huntersInCombat = new List<Hunter>();
            if (hunterMoved)
            {
                foreach (HunterName h in hunter.HuntersInGroup)
                {
                    huntersInCombat.Add(Hunters[(int)h]);
                }
            }
            else
            {
                foreach (Hunter h in Hunters)
                {
                    if (h.CurrentLocation == Hunters[hunterIndex].CurrentLocation)
                    {
                        huntersInCombat.Add(h);
                    }
                }
            }
            hunterBasicCards.Add(new ItemDetail("Punch"));
            hunterBasicCards.Add(new ItemDetail("Escape"));
            bool trapPlayed = false;
            int rageRounds = 0;
            string enemyName = "nobody";
            switch (enemyInCombat)
            {
                case 1:
                    {
                        enemyName = "Dracula";
                        enemyCombatCards.Add(new ItemDetail("Claws"));
                        enemyCombatCards.Add(new ItemDetail("Escape (Man)"));
                        if (TimeIndex > 2)
                        {
                            enemyCombatCards.Add(new ItemDetail("Strength"));
                            enemyCombatCards.Add(new ItemDetail("Escape (Bat)"));
                            enemyCombatCards.Add(new ItemDetail("Escape (Mist)"));
                            enemyCombatCards.Add(new ItemDetail("Fangs"));
                            enemyCombatCards.Add(new ItemDetail("Mesmerize"));
                        }
                        break;
                    }
                case 2:
                    {
                        enemyName = "Minion with Knife";
                        enemyCombatCards.Add(new ItemDetail("Punch"));
                        enemyCombatCards.Add(new ItemDetail("Knife"));
                        break;
                    }
                case 3:
                    {
                        enemyName = "Minion with Knife and Pistol";
                        enemyCombatCards.Add(new ItemDetail("Punch"));
                        enemyCombatCards.Add(new ItemDetail("Knife"));
                        enemyCombatCards.Add(new ItemDetail("Pistol"));
                        break;
                    }
                case 4:
                    {
                        enemyName = "Minion with Knife and Rifle";
                        enemyCombatCards.Add(new ItemDetail("Punch"));
                        enemyCombatCards.Add(new ItemDetail("Knife"));
                        enemyCombatCards.Add(new ItemDetail("Rifle"));
                        break;
                    }
                case 5:
                    {
                        enemyName = "Assassin";
                        enemyCombatCards.Add(new ItemDetail("Punch"));
                        enemyCombatCards.Add(new ItemDetail("Knife"));
                        enemyCombatCards.Add(new ItemDetail("Pistol"));
                        enemyCombatCards.Add(new ItemDetail("Rifle"));
                        break;
                    }
                case 6:
                    {
                        enemyName = "New Vampire";
                        enemyCombatCards.Add(new ItemDetail("Claws"));
                        enemyCombatCards.Add(new ItemDetail("Escape (Man)"));
                        if (TimeIndex > 2)
                        {
                            enemyCombatCards.Add(new ItemDetail("Strength"));
                            enemyCombatCards.Add(new ItemDetail("Escape (Bat)"));
                            enemyCombatCards.Add(new ItemDetail("Escape (Mist)"));
                            enemyCombatCards.Add(new ItemDetail("Fangs"));
                            enemyCombatCards.Add(new ItemDetail("Mesmerize"));
                        }
                        break;
                    }

            }
            ui.TellUser(Hunters[hunterIndex].Name + " is entering combat against " + enemyName + (huntersInCombat.Count() > 1 ? " with " + huntersInCombat[1].Name : "") + (huntersInCombat.Count() > 2 ? " and " + huntersInCombat[2].Name : "") + (huntersInCombat.Count() > 3 ? " and " + huntersInCombat[3].Name : ""));
            EventDetail draculaEventCard = null;
            do
            {
                draculaEventCard = Dracula.PlayEventCardAtStartOfCombat(this, trapPlayed, hunterMoved, enemyInCombat);
                if (draculaEventCard != null)
                {
                    DiscardEventFromDracula(draculaEventCard.name);
                    switch (draculaEventCard.name)
                    {
                        case "Trap":
                            PlayTrap(ui);
                            int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                            if (hunterPlayingGoodluck > -1)
                            {
                                DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                            }
                            else
                            {
                                trapPlayed = true;
                                break;
                            }
                            break;
                        case "Rage":
                            PlayRage(huntersInCombat, ui);
                            int hunterPlayingGoodluckB = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                            if (hunterPlayingGoodluckB > -1)
                            {
                                DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluckB, ui);
                            }
                            else
                            {
                                rageRounds = 3;
                                try
                                {
                                    enemyCombatCards.Remove(enemyCombatCards.Find(card => card.Name == "Escape (Man)"));
                                }
                                catch (Exception e) { }
                                try
                                {
                                    enemyCombatCards.Remove(enemyCombatCards.Find(card => card.Name == "Escape (Bat)"));
                                }
                                catch (Exception e) { }
                                try
                                {
                                    enemyCombatCards.Remove(enemyCombatCards.Find(card => card.Name == "Escape (Mist)"));
                                }
                                catch (Exception e) { }
                            }
                            break;

                        case "Wild Horses": PlayWildHorses(huntersInCombat, ui); return "Wild Horses";
                    }
                }
            } while (draculaEventCard != null);
            bool[] hunterPlayedCard = new bool[4] { true, true, true, true };
            int hunterPlayingCard = 0;
            while (hunterPlayedCard[0] || hunterPlayedCard[1] || hunterPlayedCard[2] || hunterPlayedCard[3])
            {
                if (ui.GetHunterPlayingCard(Hunters[hunterPlayingCard].Name))
                {
                    string cardName = "";
                    do
                    {
                        cardName = ui.GetNameOfHunterCardPlayedAtStartOfCombat(Hunters[hunterPlayingCard].Name);
                    } while (GetEventByNameFromEventDeck(cardName).name == "Unknown event" && GetItemByNameFromItemDeck(cardName).Name == "Unknown item" && cardName.ToLower() != "cancel");
                    if (cardName != "cancel")
                    {
                        hunterPlayedCard[hunterPlayingCard] = true;
                        switch (cardName)
                        {
                            case "Advance Planning":
                                DiscardEventFromHunterAtIndex(cardName, hunterPlayingCard, ui);
                                draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
                                bool eventIsCancelled = false;
                                if (draculaEventCard != null)
                                {
                                    switch (draculaEventCard.name)
                                    {
                                        case "DevilishPower":
                                            ui.TellUser("Dracula played Devilish power to cancel this event");
                                            DiscardEventFromDracula("Devilish Power");
                                            int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                            if (hunterPlayingGoodluck > -1)
                                            {
                                                DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                            }
                                            else
                                            {
                                                eventIsCancelled = true;
                                            }

                                            break;
                                    }
                                }
                                if (!eventIsCancelled)
                                {
                                    PlayAdvancePlanningInCombat(ui);
                                }
                                break;
                            case "Escape Route":
                                DiscardEventFromHunterAtIndex(cardName, hunterPlayingCard, ui);
                                draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
                                bool eventIsCancelledB = false;
                                if (draculaEventCard != null)
                                {
                                    switch (draculaEventCard.name)
                                    {
                                        case "DevilishPower":
                                            ui.TellUser("Dracula played Devilish power to cancel this event");
                                            DiscardEventFromDracula("Devilish Power");
                                            int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                            if (hunterPlayingGoodluck > -1)
                                            {
                                                DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                            }
                                            else
                                            {
                                                eventIsCancelledB = true;
                                            }

                                            break;
                                    }
                                }
                                if (!eventIsCancelledB)
                                {
                                    PlayEscapeRouteInCombat(hunterPlayingCard, ui);
                                    return "Escape Route";
                                }
                                break;
                            case "Heroic Leap":
                                DiscardEventFromHunterAtIndex(cardName, hunterPlayingCard, ui);
                                draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
                                bool eventIsCancelledC = false;
                                if (draculaEventCard != null)
                                {
                                    switch (draculaEventCard.name)
                                    {
                                        case "DevilishPower":
                                            ui.TellUser("Dracula played Devilish power to cancel this event");
                                            DiscardEventFromDracula("Devilish Power");
                                            int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                            if (hunterPlayingGoodluck > -1)
                                            {
                                                DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                            }
                                            else
                                            {
                                                eventIsCancelledC = true;
                                            }

                                            break;
                                    }
                                }
                                if (!eventIsCancelledC)
                                {
                                    PlayHeroicLeapInCombat(hunterPlayingCard, ui);
                                }
                                break;
                            case "Garlic":
                                rageRounds = 3;
                                if (enemyCombatCards.FindIndex(card => card.Name == "Escape (Man)") > -1)
                                {
                                    enemyCombatCards.Remove(enemyCombatCards.Find(card => card.Name == "Escape (Man)"));
                                }
                                if (enemyCombatCards.FindIndex(card => card.Name == "Escape (Bat)") > -1)
                                {
                                    enemyCombatCards.Remove(enemyCombatCards.Find(card => card.Name == "Escape (Bat)"));
                                }
                                if (enemyCombatCards.FindIndex(card => card.Name == "Escape (Mist)") > -1)
                                {
                                    enemyCombatCards.Remove(enemyCombatCards.Find(card => card.Name == "Escape (Mist)"));
                                }
                                DiscardItemFromHunterAtIndex(cardName, hunterPlayingCard, ui);
                                break;
                        }

                    }
                }
                else
                {
                    hunterPlayedCard[hunterPlayingCard] = false;
                }
                hunterPlayingCard = (hunterPlayingCard + 1) % 4;
            }
            CombatRoundResult roundResult = new CombatRoundResult();
            roundResult = ResolveRoundOfCombat(huntersInCombat, enemyCombatCards, hunterBasicCards, roundResult, hunterMoved, enemyInCombat, ui);
            rageRounds--;
            if (rageRounds == 0)
            {
                enemyCombatCards.Add(new ItemDetail("Escape (Man)"));
                if (TimeIndex > 2)
                {
                    enemyCombatCards.Add(new ItemDetail("Escape (Bat)"));
                    enemyCombatCards.Add(new ItemDetail("Escape (Mist)"));
                }
            }
            enemyCombatCards.Add(new ItemDetail("Dodge"));
            hunterBasicCards.Add(new ItemDetail("Dodge"));

            while (roundResult.outcome != "Bite" && roundResult.outcome != "Enemy killed" && roundResult.outcome != "Hunter killed" && roundResult.outcome != "End")
            {
                if (roundResult.outcome == "Hunter wounded")
                {
                    if (ui.AskIfHunterIsUsingGreatStrengthToCancelDamage(roundResult.hunterTargeted))
                    {
                        DiscardEventFromHunterAtIndex("Great Strength", Array.FindIndex(Hunters, h => h.Name == roundResult.hunterTargeted), ui);
                        draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
                        if (draculaEventCard != null)
                        {
                            switch (draculaEventCard.name)
                            {
                                case "DevilishPower":
                                    ui.TellUser("Dracula played Devilish power to cancel this event");
                                    DiscardEventFromDracula("Devilish Power");
                                    int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                                    if (hunterPlayingGoodluck > -1)
                                    {
                                        DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                                    }
                                    break;
                            }
                        }
                    }
                }
                roundResult = ResolveRoundOfCombat(huntersInCombat, enemyCombatCards, hunterBasicCards, roundResult, hunterMoved, enemyInCombat, ui);
            }
            foreach (Hunter h in huntersInCombat)
            {
                h.Health -= ui.GetHunterHealthLost(h.Name);
            }
            if (enemyInCombat == 1)
            {
                Dracula.Blood -= ui.GetDraculaBloodLost();
            }
            ui.TellUser("Be sure to discard any items that were destroyed/consumed in this combat");
            if (roundResult.outcome == "Bite" || roundResult.outcome == "Enemy killed" || roundResult.outcome == "Hunter killed" || roundResult.outcome == "End")
            {
                switch (roundResult.outcome)
                {
                    case "Bite":
                        ApplyBiteToHunter(Array.FindIndex(Hunters, hunt => hunt.Name == roundResult.hunterTargeted), ui);
                        HandleDraculaEscape(ui);
                        break;
                    case "End":
                        if (TimeIndex > 2)
                        {
                            HandleDraculaEscape(ui);
                        }
                        break;
                }
                return roundResult.outcome;
            }
            return ui.GetCombatRoundOutcome();
        }

        private void PlayHeroicLeapInCombat(int hunterIndex, UserInterface ui)
        {
            ui.TellUser("First roll a die for you and then roll a die for Dracula and tell me the results");
            int hunterHealthLost = ui.GetDieRoll();
            int draculaBloodLost = ui.GetDieRoll();
            Hunters[hunterIndex].Health -= hunterHealthLost;
            Dracula.Blood -= draculaBloodLost;
            HandlePossibleHunterDeath(ui);
        }

        // remove
        private void PlayEscapeRouteInCombat(int hunterIndex, UserInterface ui)
        {
            ui.TellUser("Combat is cancelled");
        }

        // remove
        private void PlayAdvancePlanningInCombat(UserInterface ui)
        {
            ui.TellUser("Add 1 to all dice rolls for that hunter");
        }

        private void PlayWildHorses(List<Hunter> huntersBeingMoved, UserInterface ui)
        {
            ui.TellUser("Dracula played Wild Horses");
            int hunterIndex = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
            if (hunterIndex > -1)
            {
                DiscardEventFromHunterAtIndex("Good Luck", hunterIndex, ui);
            }
            else
            {
                LocationDetail locationToMoveHuntersToWithWildHorses = Dracula.ChooseLocationToSendHuntersToWithWildHorses(this, huntersBeingMoved);
                ui.TellUser("Dracula moved you to " + locationToMoveHuntersToWithWildHorses.Name);
                foreach (Hunter h in huntersBeingMoved)
                {
                    h.CurrentLocation = locationToMoveHuntersToWithWildHorses;
                }
            }
        }

        private void PlayRage(List<Hunter> huntersInCombat, UserInterface ui)
        {
            Hunter ragedHunter = Dracula.ChooseHunterToRage(huntersInCombat);
            ui.TellUser(ragedHunter.Name + " must show me all items and I will discard one");
            List<ItemDetail> itemsInHunterHand = new List<ItemDetail>();
            string cardInRagedHunterHand;
            do
            {
                cardInRagedHunterHand = ui.GetNameOfCardInRagedHunterHand();
                if (GetItemByNameFromItemDeck(cardInRagedHunterHand).Name != "Unknown item")
                {
                    itemsInHunterHand.Add(GetItemByNameFromItemDeck(cardInRagedHunterHand));
                }
            } while (GetItemByNameFromItemDeck(cardInRagedHunterHand).Name == "Unknown item" && cardInRagedHunterHand.ToLower() != "none");
            ItemDetail itemToDiscard = Dracula.ChooseItemCardToDiscard(itemsInHunterHand);
            ui.TellUser("Discarding " + itemToDiscard.Name);
            DiscardItemFromHunterAtIndex(itemToDiscard.Name, Array.FindIndex(Hunters, h => h.Name == huntersInCombat.First().Name), ui);
        }

        // remove
        private void PlayTrap(UserInterface ui)
        {
            ui.TellUser("Dracula played Trap");
        }

        private CombatRoundResult ResolveRoundOfCombat(List<Hunter> huntersFighting, List<ItemDetail> combatCards, List<ItemDetail> hunterBasicCards, CombatRoundResult result, bool hunterMoved, int enemyType, UserInterface ui)
        {
            string targetHunterName;
            string newEnemyCardUsed = Dracula.ChooseCombatCardAndTarget(huntersFighting, combatCards, result, NameOfHunterAlly(), out targetHunterName).Name;
            string newHunterCardUsed = "nothing";
            foreach (Hunter h in huntersFighting)
            {
                do
                {
                    newHunterCardUsed = ui.GetCombatCardFromHunter(h.Name);
                    if (GetItemByNameFromItemDeck(newHunterCardUsed).Name == "Unknown item")
                    {
                        if (GetItemByNameFromList(newHunterCardUsed, hunterBasicCards).Name == "Unknown item")
                        {
                            ui.TellUser("I didn't recognise that item name");
                        }
                    }
                } while (GetItemByNameFromItemDeck(newHunterCardUsed).Name == "Unknown item" && GetItemByNameFromList(newHunterCardUsed, hunterBasicCards).Name == "Unknown item");
                if (newHunterCardUsed != "Punch" && newHunterCardUsed != "Dodge" && newHunterCardUsed != "Escape")
                {
                    if (h.ItemsKnownToDracula.FindIndex(it => it.Name == newHunterCardUsed) == -1)
                    {
                        h.ItemsKnownToDracula.Add(GetItemByNameFromItemDeck(newHunterCardUsed));
                    }
                    else if (newHunterCardUsed == h.LastItemUsedInCombat)
                    {
                        List<ItemDetail> copyOfHunterItemsKnown = new List<ItemDetail>();
                        copyOfHunterItemsKnown.AddRange(h.ItemsKnownToDracula);
                        copyOfHunterItemsKnown.Remove(copyOfHunterItemsKnown.Find(it => it.Name == newHunterCardUsed));
                        if (copyOfHunterItemsKnown.FindIndex(it => it.Name == newHunterCardUsed) == -1)
                        {
                            h.ItemsKnownToDracula.Add(GetItemByNameFromItemDeck(newHunterCardUsed));
                        }
                    }
                    h.LastItemUsedInCombat = newHunterCardUsed;
                }
            }
            ui.TellUser("Enemy chose " + newEnemyCardUsed + " against " + targetHunterName);
            if (NameOfHunterAlly() == "Sister Agatha" && enemyType == 6)
            {
                if (newEnemyCardUsed == "Fangs" || newEnemyCardUsed == "Escape (Man)" || newEnemyCardUsed == "Escape (Bat)" || newEnemyCardUsed == "Escape (Mist)")
                {
                    ui.TellUser("Dracula spent two blood to play his card");
                    Dracula.Blood -= 2;
                }
            }
            string newOutcome = ui.GetCombatRoundOutcome();
            CombatRoundResult newResult = new CombatRoundResult();
            newResult.enemyCardUsed = newEnemyCardUsed;
            newResult.outcome = newOutcome;
            newResult.hunterTargeted = targetHunterName;
            return newResult;
        }

        // remove
        private ItemDetail GetItemByNameFromList(string itemName, List<ItemDetail> itemList)
        {
            try
            {
                return itemList[itemList.FindIndex(card => card.Name.ToLower().StartsWith(itemName.ToLower()))];
            }
            catch (ArgumentOutOfRangeException)
            {
                return new ItemDetail("Unknown item");
            }
        }

        internal void PerformBatsMoveForHunter(int hunterIndex, UserInterface ui)
        {
            LocationDetail locationToMoveTo = Dracula.DecideWhereToSendHunterWithBats(this, Hunters[hunterIndex], ui);
            MoveHunterToLocationAtHunterIndex(hunterIndex, locationToMoveTo, ui);
            ui.TellUser(Hunters[hunterIndex].Name + (Hunters[hunterIndex].HuntersInGroup.Count() > 0 ? " and group" : "") + " moved to " + locationToMoveTo.Name + " by Bats");
        }

        internal void TradeBetweenHunters(int hunterIndexA, int hunterIndexB, UserInterface ui)
        {
            int hunterAGiven = ui.GetNumberOfCardsGivenByHunter(Hunters[hunterIndexA].Name);
            int hunterBGiven = ui.GetNumberOfCardsGivenByHunter(Hunters[hunterIndexB].Name);
            Hunters[hunterIndexA].NumberOfItems = Hunters[hunterIndexA].NumberOfItems + hunterBGiven - hunterAGiven;
            Hunters[hunterIndexB].NumberOfItems = Hunters[hunterIndexB].NumberOfItems + hunterAGiven - hunterBGiven;
            ui.TellUser(Hunters[hunterIndexA].Name + " now has " + Hunters[hunterIndexA].NumberOfItems + " items and " + Hunters[hunterIndexB].Name + " now has " + Hunters[hunterIndexB].NumberOfItems + " items");
        }

        internal void UseItemByHunterAtHunterIndex(string itemName, int hunterIndex, UserInterface ui)
        {
            ui.TellUser(Hunters[hunterIndex].Name + " used " + itemName);
            switch (itemName)
            {
                case "Local Rumors": PlayLocalRumors(hunterIndex, ui); break;
                case "Dogs": PlayDogs(hunterIndex, ui); break;
                case "Fast Horse": ui.TellUser("You may now move two road spaces"); DiscardItemFromHunterAtIndex("Fast Horse", hunterIndex, ui); break;
                case "Heavenly Host": PlayHeavenlyHost(hunterIndex, ui); break;
                case "Holy Water": PlayHolyWater(hunterIndex, ui); break;
                default: break;
            }
        }

        private void PlayDogs(int hunterIndex, UserInterface ui)
        {
            Hunters[hunterIndex].HasDogsFaceUp = true;
            if (Hunters[hunterIndex].ItemsKnownToDracula.FindIndex(item => item.Name == "Dogs") == -1)
            {
                Hunters[hunterIndex].ItemsKnownToDracula.Add(GetItemByNameFromItemDeck("Dogs"));
            }
        }

        private void PlayHolyWater(int hunterIndex, UserInterface ui)
        {
            List<Hunter> huntersWithBitesAtThisLocation = new List<Hunter>();
            for (int i = 0; i < 3; i++)
            {
                if (Hunters[i].CurrentLocation == Hunters[hunterIndex].CurrentLocation && Hunters[i].NumberOfBites > 0)
                {
                    huntersWithBitesAtThisLocation.Add(huntersWithBitesAtThisLocation[i]);
                }
            }
            Hunter hunterToHeal = null;
            if (huntersWithBitesAtThisLocation.Count > 1)
            {
                do
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (huntersWithBitesAtThisLocation.Contains(Hunters[i]))
                        {
                            if (ui.GetHunterHeal(Hunters[hunterIndex].Name))
                            {
                                hunterToHeal = Hunters[i];
                            }
                        }
                    }
                } while (hunterToHeal == null);
            }
            else
            {
                hunterToHeal = huntersWithBitesAtThisLocation.First();
            }
            switch (ui.GetHolyWaterResult())
            {
                case 1:
                    hunterToHeal.Health -= 2;
                    HandlePossibleHunterDeath(ui); break;
                case 2: break;
                case 3:
                    hunterToHeal.NumberOfBites--;
                    if (hunterToHeal.NumberOfBites < 1)
                    {
                        hunterToHeal.ItemShownToDraculaForBeingBitten = null;
                        hunterToHeal.EventShownToDraculaForBeingBitten = null;
                    }
                    break;
            }
            DiscardItemFromHunterAtIndex("Holy Water", hunterIndex, ui);
        }

        private void PlayHeavenlyHost(int hunterIndex, UserInterface ui)
        {
            Hunters[hunterIndex].CurrentLocation.HasHost = true;
            DiscardItemFromHunterAtIndex("Heavenly Host", hunterIndex, ui);
        }

        private void PlayLocalRumors(int hunterIndex, UserInterface ui)
        {
            int locationIndex = -1;
            do
            {
                locationIndex = ui.GetLocationIndexOfEncounterToReveal();
            }
            while (Dracula.LocationTrail[locationIndex] == null && (locationIndex > 5 ? Dracula.Catacombs[locationIndex - 6] == null : true));
            LocationDetail locationWhereEncounterIsBeingRevealed;
            if (locationIndex < 6)
            {
                locationWhereEncounterIsBeingRevealed = Dracula.LocationTrail[locationIndex];
            }
            else
            {
                locationWhereEncounterIsBeingRevealed = Dracula.Catacombs[locationIndex - 6];
            }
            int encounterToReveal = 0;
            if (locationWhereEncounterIsBeingRevealed.Encounters.Count() > 0)
            {
                encounterToReveal = ui.GetIndexOfEncounterToReveal();
            }
            locationWhereEncounterIsBeingRevealed.Encounters[encounterToReveal].isRevealed = true;
            DiscardItemFromHunterAtIndex("Local Rumors", hunterIndex, ui);
        }

        // remove
        internal bool HunterHasGroupAtHunterIndex(int hunterIndex)
        {
            return Hunters[hunterIndex].HuntersInGroup.Count() > 1;
        }

        internal void ApplyBiteToHunter(int hunterIndex, UserInterface ui)
        {
            if (ui.AskIfHunterIsUsingGreatStrengthToCancelBite(Hunters[hunterIndex].Name))
            {
                DiscardEventFromHunterAtIndex("Great Strength", hunterIndex, ui);
                EventDetail draculaEventCard = Dracula.WillPlayDevilishPower(this, ui);
                bool eventIsCancelled = false;
                if (draculaEventCard != null)
                {
                    switch (draculaEventCard.name)
                    {
                        case "DevilishPower":
                            ui.TellUser("Dracula played Devilish power to cancel this event");
                            DiscardEventFromDracula("Devilish Power");
                            int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                            if (hunterPlayingGoodluck > -1)
                            {
                                DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                            }
                            else
                            {
                                eventIsCancelled = true;
                            }

                            break;
                    }
                }
                if (!eventIsCancelled)
                {
                    CheckBittenHunterCards(ui);
                    return;
                }
            }
            Hunters[hunterIndex].NumberOfBites++;
            ui.TellUser(Hunters[hunterIndex].Name + " was bitten");
        }

        internal void ApplyBiteToOneOfMultipleHunters(int hunterIndex, UserInterface ui)
        {
            int hunterIndexBitten;
            do
            {
                hunterIndexBitten = ui.GetIndexOfHunterBitten();
                if (Hunters[hunterIndexBitten].CurrentLocation != Hunters[hunterIndex].CurrentLocation)
                {
                    ui.TellUser(Hunters[hunterIndexBitten].Name + " is not at the same location as " + Hunters[hunterIndex].Name);
                }
            } while (Hunters[hunterIndexBitten].CurrentLocation != Hunters[hunterIndex].CurrentLocation);
            ApplyBiteToHunter(hunterIndexBitten, ui);
        }

        // remove
        internal string LocationOfHunterAtHunterIndex(int hunterIndex)
        {
            return Hunters[hunterIndex].CurrentLocation.Name;
        }

        internal int NumberOfHuntersAtLocation(string locationName)
        {
            LocationDetail location = GetLocationFromName(locationName);
            int total = 0;
            foreach (Hunter h in Hunters)
            {
                if (h.CurrentLocation == location)
                {
                    total++;
                }
            }
            return total;
        }

        internal void HandleDraculaEscape(UserInterface ui)
        {
            if (ui.GetDidDraculaEscapeAsBat())
            {
                Dracula.DoEscapeAsBatsMove(this, ui);
            }
        }

        internal bool HandlePossibleHunterDeath(UserInterface ui)
        {
            bool hunterDied = false;
            foreach (Hunter h in Hunters)
            {
                if (h.Health < 1 || (h.Name != "Van Helsing" && h.NumberOfBites > 1) || h.NumberOfBites > 2)
                {
                    hunterDied = true;
                    ui.TellUser(h.Name + " is defeated");
                    h.CurrentLocation = GetLocationFromName("St. Joseph & St. Mary");
                    int hunterIndex = 0;
                    switch (h.Name)
                    {
                        case "Lord Godalming":
                            h.Health = 12;
                            h.NumberOfBites = 0;
                            hunterIndex = 0;
                            break;
                        case "Van Helsing":
                            h.Health = 8;
                            h.NumberOfBites = 0;
                            hunterIndex = 1;
                            break;
                        case "Dr. Seward":
                            h.Health = 10;
                            h.NumberOfBites = 0;
                            hunterIndex = 2;
                            break;
                        case "Mina Harker":
                            h.Health = 8;
                            h.NumberOfBites = 1;
                            hunterIndex = 3;
                            break;
                    }
                    while (h.NumberOfEvents > 0)
                    {
                        string eventName = "Unknown event";
                        while (eventName == "Unknown event")
                        {
                            eventName = GetEventByNameFromEventDeck(ui.GetNameOfEventDiscardedByHunter(h.Name)).name;
                            if (eventName == "Unknown event")
                            {
                                ui.TellUser("I can't find that event");
                            }
                        }
                        DiscardEventFromHunterAtIndex(eventName, hunterIndex, ui);
                    }
                    while (h.NumberOfItems > 0)
                    {
                        string itemName = "Unknown item";
                        while (itemName == "Unknown item")
                        {
                            itemName = GetItemByNameFromItemDeck(ui.GetNameOfItemDiscardedByHunter(h.Name)).Name;
                            if (itemName == "Unknown item")
                            {
                                ui.TellUser("I can't find that item");
                            }
                        }
                        DiscardItemFromHunterAtIndex(itemName, hunterIndex, ui);
                    }
                }
            }
            return hunterDied;
        }

        internal void RestHunterAtHunterIndex(int hunterIndex, UserInterface ui)
        {
            int numberOfEvents = 2;
            if (Hunters[hunterIndex].CurrentLocation == Hunters[1].CurrentLocation)
            {
                numberOfEvents = 1;
            }

            for (int i = 0; i < numberOfEvents; i++)
            {
                int eventDrawnFor = ui.GetEventDrawnFor();
                if (eventDrawnFor == 2)
                {
                    DrawEventCardForDracula(ui);
                }
                else
                {
                    DiscardEventCard(ui.GetEventCardName());
                }
            }
            int maxHealth = 8;
            switch (hunterIndex)
            {
                case 0:
                    maxHealth = 12;
                    break;
                case 2:
                    maxHealth = 10;
                    break;
            }
            Hunters[hunterIndex].Health = Math.Min(maxHealth, Hunters[hunterIndex].Health + 2);
            ui.TellUser(Hunters[hunterIndex].Name + " now has " + Hunters[hunterIndex].Health + " health");
        }

        internal void BlessHunterAtHunterIndex(int hunterIndex, UserInterface ui)
        {
            switch (ui.GetHolyWaterResult())
            {
                case 1:
                    Hunters[hunterIndex].Health -= 2;
                    HandlePossibleHunterDeath(ui);
                    break;
                case 2: break;
                case 3:
                    Hunters[hunterIndex].NumberOfBites--;
                    if (Hunters[hunterIndex].NumberOfBites < 1)
                    {
                        Hunters[hunterIndex].ItemShownToDraculaForBeingBitten = null;
                        Hunters[hunterIndex].EventShownToDraculaForBeingBitten = null;
                    }
                    break;
            }

        }

        internal void PerformNewspaperReportsFromResolve(UserInterface ui)
        {
            Resolve--;
            int checkingLocationIndex = TrailLength();
            do
            {
                checkingLocationIndex--;
            } while ((TypeOfLocationAtTrailIndex(checkingLocationIndex) != LocationType.Castle && TypeOfLocationAtTrailIndex(checkingLocationIndex) != LocationType.City && TypeOfLocationAtTrailIndex(checkingLocationIndex) != LocationType.Sea && TypeOfLocationAtTrailIndex(checkingLocationIndex) != LocationType.Town) || LocationIsRevealedAtTrailIndex(checkingLocationIndex));

            if (DraculaCurrentLocationIsAtTrailIndex(checkingLocationIndex))
            {
                ui.TellUser("The oldest unrevealed location in Dracula's trail is his current location");
                if (LocationWhereHideWasUsedIsDraculaCurrentLocation())
                {
                    ui.TellUser("Here's the Hide card to prove it");
                    RevealHide(ui);
                }
            }
            else
            {
                RevealLocationAtTrailIndex(checkingLocationIndex, ui);
                ui.TellUser("Revealing " + NameOfLocationAtTrailIndex(checkingLocationIndex));
            }
        }

        internal void PerformSenseOfEmergencyFromResolve(int hunterIndex, UserInterface ui)
        {
            Resolve--;
            Hunters[hunterIndex].Health -= 6;
            Hunters[hunterIndex].Health += VampirePointTracker;
            ui.TellUser("Adjust " + Hunters[hunterIndex].Name + "'s health and then perform a move to any location");
        }

        internal void PerformInnerStrengthFromResolve(int hunterIndex, UserInterface ui)
        {
            Resolve--;
            switch (hunterIndex)
            {
                case 0: Hunters[hunterIndex].Health = Math.Min(Hunters[hunterIndex].Health + 4, 12); break;
                case 1: Hunters[hunterIndex].Health = Math.Min(Hunters[hunterIndex].Health + 4, 8); break;
                case 2: Hunters[hunterIndex].Health = Math.Min(Hunters[hunterIndex].Health + 4, 10); break;
                case 3: Hunters[hunterIndex].Health = Math.Min(Hunters[hunterIndex].Health + 4, 8); break;
            }
            Hunters[hunterIndex].Health += 4;
            ui.TellUser("Adjust " + Hunters[hunterIndex].Name + "'s health");
        }

        internal bool DraculaWillPlayCustomsSearch(int hunterIndex, UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula is deciding whether to play Customs Search");

            EventDetail draculaEventCard = Dracula.WillPlayCustomsSearch(this, Hunters[hunterIndex]);
            if (draculaEventCard != null)
            {
                switch (draculaEventCard.name)
                {
                    case "Customs Search":
                        int hunterPlayingGoodluck = ui.AskWhichHunterIsUsingGoodLuckToCancelEvent();
                        if (hunterPlayingGoodluck > -1)
                        {
                            DiscardEventFromHunterAtIndex("Good Luck", hunterPlayingGoodluck, ui);
                            DiscardEventFromDracula("Customs Search");
                        }
                        else
                        {

                            PlayCustomsSearch(hunterIndex, ui);
                            return true;
                        }
                        break;
                }
            }
            return false;
        }

        private void PlayCustomsSearch(int hunterIndex, UserInterface ui)
        {
            ui.TellUser("Dracula played Customs Search");
            ui.TellUser(Hunters[hunterIndex].Name + " must discard all items, don't forget to tell me what they are");
            DiscardEventFromDracula("Customs Search");
        }

        // remove
        internal List<LocationDetail> GetMap()
        {
            return Map;
        }

        internal List<Hunter> GetBittenHunters()
        {
            List<Hunter> huntersToReturn = new List<Hunter>();
            foreach (Hunter h in Hunters)
            {
                if (h.NumberOfBites > 0)
                {
                    huntersToReturn.Add(h);
                }
            }
            return huntersToReturn;
        }

        // remove
        internal int IndexOfHunterAtLocation(LocationDetail location)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Hunters[i].CurrentLocation == location)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void ShowStateOfGame(UserInterface ui)
        {
            ui.TellUser("This is how I see things");
            foreach (Hunter h in Hunters)
            {
                ui.TellUser(h.Name + " is in " + h.CurrentLocation.Name + " with " + h.Health + " health, " + h.NumberOfBites + " bites, " + h.NumberOfItems + " item card(s), " + h.NumberOfEvents + " event card(s)" + (h.HasDogsFaceUp ? " and has Dogs face up" : ""));
            }
            foreach (LocationDetail loc in Map)
            {
                if (loc.HasHost)
                {
                    ui.TellUser("There is a Heavenly Host at " + loc.Name);
                }
                if (loc.IsConsecrated)
                {
                    ui.TellUser(loc.Name + " is consecrated");
                }
                if (loc.TurnsUntilStormSubsides > 0)
                {
                    ui.TellUser("There will be a storm in " + loc.Name + " for " + loc.TurnsUntilStormSubsides + " more turn(s)");
                }
            }
            if (RoadblockCounter.connectionType != null)
            {
                ui.TellUser("There is a roadblock on the " + RoadblockCounter.connectionType + " between " + RoadblockCounter.firstLocation.Name + " and " + RoadblockCounter.secondLocation.Name);
            }
            ui.TellUser("These things are in the item discard");
            foreach (ItemDetail i in ItemDiscard)
            {
                ui.TellUser(i.Name);
            }
            ui.TellUser("These things are in the event discard");
            foreach (EventDetail e in EventDiscard)
            {
                try
                {
                    ui.TellUser(e.name);
                }
                catch (NullReferenceException)
                {
                    Logger.WriteToDebugLog("A null event ended up in the discard");
                }
            }

        }

        internal bool HeavenlyHostIsInPlay()
        {
            foreach (LocationDetail loc in Map)
            {
                if (loc.HasHost)
                {
                    return true;
                }
            }
            return false;
        }

        internal void TellMeWhatYouKnow(UserInterface ui)
        {
            ui.TellUser("This is what I know");
            foreach (Hunter h in Hunters)
            {
                ui.TellUser(h.Name + " has:");
                foreach (ItemDetail i in h.ItemsKnownToDracula)
                {
                    ui.TellUser(i.Name);
                }
                foreach (EventDetail e in h.EventsKnownToDracula)
                {
                    ui.TellUser(e.name);
                }
                if (h.Destination != null)
                {
                    ui.TellUser(h.Name + " will move to " + h.Destination + " by " + h.TravelType + " next turn");
                }
            }
        }

        internal void CheckBittenHunterCards(UserInterface ui)
        {
            foreach (Hunter h in Hunters)
            {
                if (h.NumberOfBites > 0)
                {
                    if (h.NumberOfItems > 0 && h.ItemShownToDraculaForBeingBitten == null)
                    {
                        string line;
                        do
                        {
                            line = ui.AskHunterToRevealItemForBeingBitten(h.Name);
                            ui.TellUser(GetItemByNameFromItemDeck(line).Name);
                        } while (GetItemByNameFromItemDeck(line).Name == "Unknown item" && h.ItemsKnownToDracula.FindIndex(itm => itm.Name == line) == -1);
                        if (h.ItemsKnownToDracula.FindIndex(itm => itm.Name == line) == -1)
                        {
                            h.ItemsKnownToDracula.Add(GetItemByNameFromItemDeck(line));
                        }
                        h.ItemShownToDraculaForBeingBitten = GetItemByNameFromItemDeck(line);
                    }
                    if (h.NumberOfEvents > 0 && h.EventShownToDraculaForBeingBitten == null)
                    {
                        string line;
                        do
                        {
                            line = ui.AskHunterToRevealEvent(h.Name);
                            ui.TellUser(GetEventByNameFromEventDeck(line).name);
                        } while (GetEventByNameFromEventDeck(line).name == "Unknown event" && h.EventsKnownToDracula.FindIndex(ev => ev.name == line) == -1);
                        if (h.EventsKnownToDracula.FindIndex(ev => ev.name == line) == -1)
                        {
                            h.EventsKnownToDracula.Add(GetEventByNameFromEventDeck(line));
                        }
                        h.EventShownToDraculaForBeingBitten = GetEventByNameFromEventDeck(line);
                    }
                }
            }
        }

        // remove
        internal void RemoveDraculaAlly()
        {
            EventDiscard.Add(DraculaAlly);
            DraculaAlly = null;
        }

        // remove
        internal void AdjustHealthOfHunterAtIndex(int hunterIndex, int p)
        {
            Hunters[hunterIndex].Health += p;
        }

        // remove
        internal bool HunterHasItemKnownToDracula(int hunterIndex, string itemName)
        {
            if (Hunters[hunterIndex].ItemsKnownToDracula.FindIndex(card => card.Name == itemName) > -1)
            {
                return true;
            }
            return false;
        }

        // remove
        internal void AddToHunterItemsKnownToDracula(Hunter hunter, string p)
        {
            hunter.ItemsKnownToDracula.Add(GetItemByNameFromItemDeck(p));
            ItemDeck.Remove(GetItemByNameFromItemDeck(p));
        }
    }
}
