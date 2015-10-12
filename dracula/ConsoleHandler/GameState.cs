﻿using DraculaHandler;
using EncounterHandler;
using EventHandler;
using HunterHandler;
using LocationHandler;
using LogHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleHandler
{
    public class GameState
    {
        private Dracula dracula;
        private Hunter[] hunters = new Hunter[4];
        private List<Location> map = new List<Location>();
        private List<Encounter> encounterPool = new List<Encounter>();
        private List<Encounter> encounterLimbo = new List<Encounter>();
        private List<Event> eventDeck = new List<Event>();
        private List<Event> eventDiscard = new List<Event>();
        private List<Item> itemDeck = new List<Item>();
        private List<Item> itemDiscard = new List<Item>();
        private Event draculaAlly;
        private Event hunterAlly;
        private int time;
        private string[] timesOfDay;
        private int resolve;
        private int vampireTracker;

        public GameState()
        {
            hunters[0] = new Hunter("Lord Godalming", 12, 0, 2);
            hunters[1] = new Hunter("Van Helsing", 8, 0, 3);
            hunters[2] = new Hunter("Dr. Seward", 10, 0, 2);
            hunters[3] = new Hunter("Mina Harker", 8, 1, 2);

            resolve = -1;
            vampireTracker = -1;

            time = -1;
            timesOfDay = new string[6] { "Dawn", "Noon", "Dusk", "Twilight", "Midnight", "Small Hours" };

            SetUpMap();
            SetUpEncounters();
            SetUpEvents();
            SetUpItems();

            dracula = new Dracula();
        }

        private void SetUpItems()
        {
            itemDeck.Add(new Item("Crucifix"));
            itemDeck.Add(new Item("Crucifix"));
            itemDeck.Add(new Item("Crucifix"));
            itemDeck.Add(new Item("Dogs"));
            itemDeck.Add(new Item("Dogs"));
            itemDeck.Add(new Item("Fast Horse"));
            itemDeck.Add(new Item("Fast Horse"));
            itemDeck.Add(new Item("Fast Horse"));
            itemDeck.Add(new Item("Garlic"));
            itemDeck.Add(new Item("Garlic"));
            itemDeck.Add(new Item("Garlic"));
            itemDeck.Add(new Item("Garlic"));
            itemDeck.Add(new Item("Heavenly Host"));
            itemDeck.Add(new Item("Heavenly Host"));
            itemDeck.Add(new Item("Holy Water"));
            itemDeck.Add(new Item("Holy Water"));
            itemDeck.Add(new Item("Holy Water"));
            itemDeck.Add(new Item("Knife"));
            itemDeck.Add(new Item("Knife"));
            itemDeck.Add(new Item("Knife"));
            itemDeck.Add(new Item("Knife"));
            itemDeck.Add(new Item("Knife"));
            itemDeck.Add(new Item("Local Rumours"));
            itemDeck.Add(new Item("Local Rumours"));
            itemDeck.Add(new Item("Pistol"));
            itemDeck.Add(new Item("Pistol"));
            itemDeck.Add(new Item("Pistol"));
            itemDeck.Add(new Item("Pistol"));
            itemDeck.Add(new Item("Pistol"));
            itemDeck.Add(new Item("Sacred Bullets"));
            itemDeck.Add(new Item("Sacred Bullets"));
            itemDeck.Add(new Item("Sacred Bullets"));
            itemDeck.Add(new Item("Stake"));
            itemDeck.Add(new Item("Stake"));
            itemDeck.Add(new Item("Stake"));
            itemDeck.Add(new Item("Stake"));
            itemDeck.Add(new Item("Rifle"));
            itemDeck.Add(new Item("Rifle"));
            itemDeck.Add(new Item("Rifle"));
            itemDeck.Add(new Item("Rifle"));
        }

        internal void AddHunterAToHunterBGroup(int hunterToAdd, int hunterIndex)
        {
            hunters[hunterIndex].huntersInGroup.Add(hunters[hunterToAdd]);
        }

        internal void RemoveHunterAFromHunterBGroup(int hunterToAdd, int hunterIndex)
        {
            hunters[hunterIndex].huntersInGroup.Remove(hunters[hunterToAdd]);
        }

        internal bool HunterAIsInHunterBGroup(int hunterToAdd, int hunterIndex)
        {
            return hunters[hunterIndex].huntersInGroup.Contains(hunters[hunterToAdd]);
        }

        internal int GetHunterToAddToGroup(GameState g, int hunterIndex, UserInterface ui)
        {
            int hunterToAdd = ui.GetHunterToAddToGroup(hunters[hunterIndex].name);
            if (hunterToAdd != -2 && hunterToAdd < hunterIndex)
            {
                ui.TellUser(hunters[hunterToAdd].name + " cannot join " + hunters[hunterIndex].name + "'s group, instead " + hunters[hunterIndex].name + " should be added to " + hunters[hunterToAdd].name + "'s group");
                return -2;
            }
            return hunterToAdd;
        }

        private void SetUpEvents()
        {
            eventDeck.Add(new Event("Rufus Smith", false, EventType.Ally));
            eventDeck.Add(new Event("Jonathan Harker", false, EventType.Ally));
            eventDeck.Add(new Event("Sister Agatha", false, EventType.Ally));
            eventDeck.Add(new Event("Heroic Leap", false, EventType.Keep));
            eventDeck.Add(new Event("Great Strength", false, EventType.Keep));
            eventDeck.Add(new Event("Money Trail", false, EventType.Keep));
            eventDeck.Add(new Event("Sense of Emergency", false, EventType.Keep));
            eventDeck.Add(new Event("Sense of Emergency", false, EventType.Keep));
            eventDeck.Add(new Event("Vampiric Lair", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Long Day", false, EventType.Keep));
            eventDeck.Add(new Event("Long Day", false, EventType.Keep));
            eventDeck.Add(new Event("Mystic Research", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Mystic Research", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Advance Planning", false, EventType.Keep));
            eventDeck.Add(new Event("Advance Planning", false, EventType.Keep));
            eventDeck.Add(new Event("Advance Planning", false, EventType.Keep));
            eventDeck.Add(new Event("Newspaper Reports", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Newspaper Reports", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Newspaper Reports", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Newspaper Reports", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Newspaper Reports", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Re-Equip", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Re-Equip", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Re-Equip", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Consecrated Ground", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Telegraph Ahead", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Telegraph Ahead", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Hypnosis", false, EventType.Keep));
            eventDeck.Add(new Event("Hypnosis", false, EventType.Keep));
            eventDeck.Add(new Event("Stormy Seas", false, EventType.Keep));
            eventDeck.Add(new Event("Surprising Return", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Surprising Return", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Good Luck", false, EventType.Keep));
            eventDeck.Add(new Event("Good Luck", false, EventType.Keep));
            eventDeck.Add(new Event("Blood Transfusion", false, EventType.Keep));
            eventDeck.Add(new Event("Secret Weapon", false, EventType.Keep));
            eventDeck.Add(new Event("Secret Weapon", false, EventType.Keep));
            eventDeck.Add(new Event("Forewarned", false, EventType.Keep));
            eventDeck.Add(new Event("Forewarned", false, EventType.Keep));
            eventDeck.Add(new Event("Forewarned", false, EventType.Keep));
            eventDeck.Add(new Event("Chartered Carriage", false, EventType.Keep));
            eventDeck.Add(new Event("Chartered Carriage", false, EventType.Keep));
            eventDeck.Add(new Event("Chartered Carriage", false, EventType.Keep));
            eventDeck.Add(new Event("Excellent Weather", false, EventType.Keep));
            eventDeck.Add(new Event("Excellent Weather", false, EventType.Keep));
            eventDeck.Add(new Event("Escape Route", false, EventType.Keep));
            eventDeck.Add(new Event("Escape Route", false, EventType.Keep));
            eventDeck.Add(new Event("Hired Scouts", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Hired Scouts", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Hired Scouts", false, EventType.PlayImmediately));
            eventDeck.Add(new Event("Dracula's Brides", true, EventType.Ally));
            eventDeck.Add(new Event("Immanuel Hildesheim", true, EventType.Ally));
            eventDeck.Add(new Event("Quincey P. Morris", true, EventType.Ally));
            eventDeck.Add(new Event("Roadblock", true, EventType.Keep));
            eventDeck.Add(new Event("Unearthly Swiftness", true, EventType.Keep));
            eventDeck.Add(new Event("Time Runs Short", true, EventType.Keep));
            eventDeck.Add(new Event("Customs Search", true, EventType.Keep));
            eventDeck.Add(new Event("Devilish Power", true, EventType.Keep));
            eventDeck.Add(new Event("Devilish Power", true, EventType.Keep));
            eventDeck.Add(new Event("Vampiric Influence", true, EventType.PlayImmediately));
            eventDeck.Add(new Event("Vampiric Influence", true, EventType.PlayImmediately));
            eventDeck.Add(new Event("Night Visit", true, EventType.PlayImmediately));
            eventDeck.Add(new Event("Evasion", true, EventType.PlayImmediately));
            eventDeck.Add(new Event("Wild Horses", true, EventType.Keep));
            eventDeck.Add(new Event("False Tip-off", true, EventType.Keep));
            eventDeck.Add(new Event("False Tip-off", true, EventType.Keep));
            eventDeck.Add(new Event("Sensationalist Press", true, EventType.Keep));
            eventDeck.Add(new Event("Rage", true, EventType.Keep));
            eventDeck.Add(new Event("Seduction", true, EventType.Keep));
            eventDeck.Add(new Event("Control Storms", true, EventType.Keep));
            eventDeck.Add(new Event("Relentless Minion", true, EventType.Keep));
            eventDeck.Add(new Event("Relentless Minion", true, EventType.Keep));
            eventDeck.Add(new Event("Trap", true, EventType.Keep));
            eventDeck.Add(new Event("Trap", true, EventType.Keep));
            eventDeck.Add(new Event("Trap", true, EventType.Keep));
        }

        internal void GetHunterGroupMemberNamesAtHunterIndex(int hunterIndex, string[] names)
        {
            for (int i = 0; i < hunters[hunterIndex].huntersInGroup.Count(); i++)
            {
                names[i] = hunters[hunterIndex].huntersInGroup[i].name;
            }
        }

        internal void SearchWithHunterAtIndex(int hunterIndex, Location location, UserInterface ui)
        {
            if (LocationIsInTrail(location) || LocationIsInCatacombs(location))
            {
                ResolveEncountersAtLocation(hunters[hunterIndex], location, ui);
                if (location == dracula.currentLocation)
                {
                    ResolveCombat(hunterIndex, 1, ui);
                }
            }
            else
            {
                ui.TellUser("Search reveals nothing");
            }
        }

        private void ResolveEncountersAtLocation(Hunter hunter, Location location, UserInterface ui)
        {
            dracula.OrderEncounters(hunter, location);
            foreach (Encounter enc in location.encounters)
            {
                enc.isRevealed = true;
            }
            ui.drawGameState(this);
            bool resolveNextEncounter = true;
            bool discardEncounter = true;
            List<Encounter> encountersBeingDiscarded = new List<Encounter>();
            Encounter firstEncounter = null;
            Encounter secondEncounter = null;

            if (location.encounters.Count() > 0)
            {
                firstEncounter = location.encounters.First();
            }
            if (location.encounters.Count() > 1)
            {
                secondEncounter = location.encounters[1];
            }
            if (firstEncounter != null)
            {
                resolveNextEncounter = ResolveEncounter(firstEncounter, hunter, out discardEncounter, ui);
                if (discardEncounter)
                {
                    location.encounters.Remove(firstEncounter);
                    encounterPool.Remove(firstEncounter);
                }
                else if (firstEncounter.name == "Bats" || firstEncounter.name == "Fog")
                {
                    encounterLimbo.Add(firstEncounter);
                }
            }
            if (secondEncounter != null)
            {
                resolveNextEncounter = ResolveEncounter(secondEncounter, hunter, out discardEncounter, ui);
                if (discardEncounter)
                {
                    location.encounters.Remove(secondEncounter);
                    encounterPool.Remove(secondEncounter);
                }
                else if (secondEncounter.name == "Bats" || secondEncounter.name == "Fog")
                {
                    encounterLimbo.Add(secondEncounter);
                }
            }

        }

        private bool ResolveEncounter(Encounter enc, Hunter hunter, out bool discard, UserInterface ui)
        {
            bool resolveNextEncounter = true;
            discard = true;
            switch (enc.name)
            {
                case "Ambush":
                    resolveNextEncounter = ResolveAmbush(hunter.huntersInGroup, ui);
                    break;
                case "Assasin":
                    resolveNextEncounter = ResolveAssassin(hunter.huntersInGroup, ui);
                    break;
                case "Bats":
                    resolveNextEncounter = ResolveBats(hunter.huntersInGroup, ui);
                    discard = false;
                    break;
                case "Desecrated Soil":
                    resolveNextEncounter = ResolveDesecratedSoil(hunter.huntersInGroup, ui);
                    break;
                case "Fog":
                    resolveNextEncounter = ResolveFog(hunter.huntersInGroup, ui);
                    discard = false;
                    break;
                case "Minion with Knife":
                    resolveNextEncounter = ResolveMinionWithKnife(hunter.huntersInGroup, ui);
                    break;
                case "Minion with Knife and Pistol":
                    resolveNextEncounter = ResolveMinionWithKnifeAndPistol(hunter.huntersInGroup, ui);
                    break;
                case "Minion with Knife and Rifle":
                    resolveNextEncounter = ResolveMinionWithKnifeAndRifle(hunter.huntersInGroup, ui);
                    break;
                case "Hoax":
                    resolveNextEncounter = ResolveHoax(hunter.huntersInGroup, ui);
                    break;
                case "Lightning":
                    resolveNextEncounter = ResolveLightning(hunter.huntersInGroup, ui);
                    break;
                case "Peasants":
                    resolveNextEncounter = ResolvePeasants(hunter.huntersInGroup, ui);
                    break;
                case "Plague":
                    resolveNextEncounter = ResolvePlague(hunter.huntersInGroup, ui);
                    break;
                case "Rats":
                    resolveNextEncounter = ResolveRats(hunter.huntersInGroup, ui);
                    break;
                case "Saboteur":
                    resolveNextEncounter = ResolveSaboteur(hunter.huntersInGroup, ui);
                    break;
                case "Spy":
                    resolveNextEncounter = ResolveSpy(hunter.huntersInGroup, ui);
                    break;
                case "Thief":
                    resolveNextEncounter = ResolveThief(hunter.huntersInGroup, ui);
                    break;
                case "New Vampire":
                    bool discardVampire = true;
                    resolveNextEncounter = ResolveNewVampire(hunter.huntersInGroup, out discardVampire, ui);
                    discard = discardVampire;
                    break;
                case "Wolves":
                    resolveNextEncounter = ResolveWolves(hunter.huntersInGroup, ui);
                    break;
            }
            return resolveNextEncounter;
        }

        private bool LocationIsInCatacombs(Location location)
        {
            return dracula.catacombs.Contains(location);
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
            Event playedCard = GetEventByNameFromEventDeck(cardName);
            AddEventToEventDiscard(playedCard);
            RemoveEventFromEventDeck(playedCard);
        }

        internal void PlayHiredScouts(UserInterface ui)
        {
            string line = "";
            Location locationToReveal;
            do
            {
                line = ui.GetFirstLocationNameForHiredScouts();
                locationToReveal = GetLocationFromName(line);
                ui.TellUser(locationToReveal.name);
            } while (locationToReveal.name == "Unknown location");
            if (LocationIsInTrail(locationToReveal))
            {
                locationToReveal.isRevealed = true;
                ui.TellUser("Revealing " + locationToReveal.name);
                for (int i = 0; i < locationToReveal.encounters.Count(); i++)
                {
                    locationToReveal.encounters[i].isRevealed = true;
                    ui.TellUser(" and " + locationToReveal.encounters[i].name);
                }
                ui.TellUser("");
                ui.drawGameState(this);
            }
            else
            {
                ui.TellUser(locationToReveal.name + " is not in Dracula's trail");
            }
            do
            {
                line = ui.GetSecondLocationNameForHiredScouts();
                locationToReveal = GetLocationFromName(line);
                ui.TellUser(locationToReveal.name);
            } while (locationToReveal.name == "Unknown location");
            if (LocationIsInTrail(locationToReveal))
            {
                locationToReveal.isRevealed = true;
                ui.TellUser("Revealing " + locationToReveal.name);
                for (int i = 0; i < locationToReveal.encounters.Count(); i++)
                {
                    locationToReveal.encounters[i].isRevealed = true;
                    ui.TellUser(" and " + locationToReveal.encounters[i].name);
                }
                ui.TellUser("");
            }
            else
            {
                ui.TellUser(locationToReveal.name + " is not in Dracula's trail");
            }
            DiscardEventCard("Hired Scouts");

        }

        internal void PlayEscapeRoute(UserInterface ui)
        {
            ui.TellUser("Forewarned is supposed to be played at the start of combat");
        }

        internal void PlayExcellentWeather(UserInterface ui)
        {
            ui.TellUser("You may move up to four sea moves this turn");
            DiscardEventCard("Excellent Weather");
        }

        internal void PlayCharteredCarriage()
        {
            throw new NotImplementedException();
        }

        internal void PlayForewarned(UserInterface ui)
        {
            ui.TellUser("Forewarned is supposed to be played when Dracula reveals an encounter at your location");
        }

        internal void PlaySecretWeapon()
        {
            throw new NotImplementedException();
        }

        internal void PlayBloodTransfusion()
        {
            throw new NotImplementedException();
        }

        internal void PlayGoodLuck()
        {
            throw new NotImplementedException();
        }

        internal void PlaySurprisingReturn()
        {
            throw new NotImplementedException();
        }

        internal void PlayStormySeas()
        {
            throw new NotImplementedException();
        }

        internal void PlayHypnosis()
        {
            throw new NotImplementedException();
        }

        internal void PlayTelegraphAhead()
        {
            throw new NotImplementedException();
        }

        internal void PlayConsecratedGround()
        {
            throw new NotImplementedException();
        }

        internal void PlayReEquip()
        {
            throw new NotImplementedException();
        }

        internal void PlayNewspaperReports(UserInterface ui)
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
                RevealLocationAtTrailIndex(checkingLocationIndex, ui);
                ui.TellUser("Revealing " + NameOfLocationAtTrailIndex(checkingLocationIndex));
            }
            DiscardEventCard("Newspaper Reports");
        }

        internal void PlayAdvancePlanning(UserInterface ui)
        {
            ui.TellUser("Advance Planning is supposed to be played at the start of a combat");
        }

        internal void PlayMysticResearch()
        {
            throw new NotImplementedException();
        }

        internal void PlayLongDay(UserInterface ui)
        {
            if (Time() < 1)
            {
                ui.TellUser("You cannot play Long Day during Dawn");
            }
            else
            {
                AdjustTime(-1);
                DiscardEventCard("Long Day");
            }
        }

        internal void PlayVampiricLair()
        {
            throw new NotImplementedException();
        }

        internal void PlaySenseOfEmergency()
        {
            throw new NotImplementedException();
        }

        internal void PlayMoneyTrail(UserInterface ui)
        {
            ui.TellUser("Revealing all sea locations in Dracula's trail");
            for (int i = 0; i < TrailLength(); i++)
            {
                if (TypeOfLocationAtTrailIndex(i) == LocationType.Sea)
                {
                    RevealLocationAtTrailIndex(i, ui);
                    LocationHelper.RevealLocation(this, i, ui);
                }
            }
            DiscardEventCard("Money Trail");
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

        private void SetUpEncounters()
        {
            encounterPool.Add(new Encounter("Ambush", "AMB"));
            encounterPool.Add(new Encounter("Ambush", "AMB"));
            encounterPool.Add(new Encounter("Ambush", "AMB"));
            encounterPool.Add(new Encounter("Assasin", "ASS"));
            encounterPool.Add(new Encounter("Bats", "BAT"));
            encounterPool.Add(new Encounter("Bats", "BAT"));
            encounterPool.Add(new Encounter("Bats", "BAT"));
            encounterPool.Add(new Encounter("Desecrated Soil", "DES"));
            encounterPool.Add(new Encounter("Desecrated Soil", "DES"));
            encounterPool.Add(new Encounter("Desecrated Soil", "DES"));
            encounterPool.Add(new Encounter("Fog", "FOG"));
            encounterPool.Add(new Encounter("Fog", "FOG"));
            encounterPool.Add(new Encounter("Fog", "FOG"));
            encounterPool.Add(new Encounter("Fog", "FOG"));
            encounterPool.Add(new Encounter("Minion with Knife", "MIK"));
            encounterPool.Add(new Encounter("Minion with Knife", "MIK"));
            encounterPool.Add(new Encounter("Minion with Knife", "MIK"));
            encounterPool.Add(new Encounter("Minion with Knife and Pistol", "MIP"));
            encounterPool.Add(new Encounter("Minion with Knife and Pistol", "MIP"));
            encounterPool.Add(new Encounter("Minion with Knife and Rifle", "MIR"));
            encounterPool.Add(new Encounter("Minion with Knife and Rifle", "MIR"));
            encounterPool.Add(new Encounter("Hoax", "HOA"));
            encounterPool.Add(new Encounter("Hoax", "HOA"));
            encounterPool.Add(new Encounter("Lightning", "LIG"));
            encounterPool.Add(new Encounter("Lightning", "LIG"));
            encounterPool.Add(new Encounter("Peasants", "PEA"));
            encounterPool.Add(new Encounter("Peasants", "PEA"));
            encounterPool.Add(new Encounter("Plague", "PLA"));
            encounterPool.Add(new Encounter("Rats", "RAT"));
            encounterPool.Add(new Encounter("Rats", "RAT"));
            encounterPool.Add(new Encounter("Saboteur", "SAB"));
            encounterPool.Add(new Encounter("Saboteur", "SAB"));
            encounterPool.Add(new Encounter("Spy", "SPY"));
            encounterPool.Add(new Encounter("Spy", "SPY"));
            encounterPool.Add(new Encounter("Thief", "THI"));
            encounterPool.Add(new Encounter("Thief", "THI"));
            encounterPool.Add(new Encounter("New Vampire", "VAM"));
            encounterPool.Add(new Encounter("New Vampire", "VAM"));
            encounterPool.Add(new Encounter("New Vampire", "VAM"));
            encounterPool.Add(new Encounter("New Vampire", "VAM"));
            encounterPool.Add(new Encounter("New Vampire", "VAM"));
            encounterPool.Add(new Encounter("New Vampire", "VAM"));
            encounterPool.Add(new Encounter("Wolves", "WOL"));
            encounterPool.Add(new Encounter("Wolves", "WOL"));
            encounterPool.Add(new Encounter("Wolves", "WOL"));
        }

        private void SetUpMap()
        {
            Location galway = new Location();
            Location dublin = new Location();
            Location liverpool = new Location();
            Location edinburgh = new Location();
            Location manchester = new Location();
            Location swansea = new Location();
            Location plymouth = new Location();
            Location nantes = new Location();
            Location lehavre = new Location();
            Location london = new Location();
            Location paris = new Location();
            Location brussels = new Location();
            Location amsterdam = new Location();
            Location strasbourg = new Location();
            Location cologne = new Location();
            Location hamburg = new Location();
            Location frankfurt = new Location();
            Location nuremburg = new Location();
            Location leipzig = new Location();
            Location berlin = new Location();
            Location prague = new Location();
            Location castledracula = new Location();
            Location santander = new Location();
            Location saragossa = new Location();
            Location bordeaux = new Location();
            Location toulouse = new Location();
            Location barcelona = new Location();
            Location clermontferrand = new Location();
            Location marseilles = new Location();
            Location geneva = new Location();
            Location genoa = new Location();
            Location milan = new Location();
            Location zurich = new Location();
            Location florence = new Location();
            Location venice = new Location();
            Location munich = new Location();
            Location zagreb = new Location();
            Location vienna = new Location();
            Location stjosephandstmary = new Location();
            Location sarajevo = new Location();
            Location szeged = new Location();
            Location budapest = new Location();
            Location belgrade = new Location();
            Location klausenburg = new Location();
            Location sofia = new Location();
            Location bucharest = new Location();
            Location galatz = new Location();
            Location varna = new Location();
            Location constanta = new Location();
            Location lisbon = new Location();
            Location cadiz = new Location();
            Location madrid = new Location();
            Location granada = new Location();
            Location alicante = new Location();
            Location cagliari = new Location();
            Location rome = new Location();
            Location naples = new Location();
            Location bari = new Location();
            Location valona = new Location();
            Location salonica = new Location();
            Location athens = new Location();
            Location atlanticocean = new Location();
            Location irishsea = new Location();
            Location englishchannel = new Location();
            Location northsea = new Location();
            Location bayofbiscay = new Location();
            Location mediterraneansea = new Location();
            Location tyrrheniansea = new Location();
            Location adriaticsea = new Location();
            Location ioniansea = new Location();
            Location blacksea = new Location();

            galway.name = "Galway";
            galway.abbreviation = "GAW";
            galway.type = LocationType.Town;
            galway.isEastern = false;
            galway.byRoad.Add(dublin);
            galway.bySea.Add(atlanticocean);
            map.Add(galway);

            dublin.name = "Dublin";
            dublin.abbreviation = "DUB";
            dublin.type = LocationType.Town;
            dublin.isEastern = false;
            dublin.byRoad.Add(galway);
            dublin.bySea.Add(irishsea);
            map.Add(dublin);

            liverpool.name = "Liverpool";
            liverpool.abbreviation = "LIV";
            liverpool.type = LocationType.City;
            liverpool.isEastern = false;
            liverpool.byRoad.Add(manchester);
            liverpool.byRoad.Add(swansea);
            liverpool.byTrain.Add(manchester);
            liverpool.bySea.Add(irishsea);
            map.Add(liverpool);

            edinburgh.name = "Edinburgh";
            edinburgh.abbreviation = "EDI";
            edinburgh.type = LocationType.City;
            edinburgh.isEastern = false;
            edinburgh.byRoad.Add(manchester);
            edinburgh.byTrain.Add(manchester);
            edinburgh.bySea.Add(northsea);
            map.Add(edinburgh);

            manchester.name = "Manchester";
            manchester.abbreviation = "MAN";
            manchester.type = LocationType.City;
            manchester.isEastern = false;
            manchester.byRoad.Add(edinburgh);
            manchester.byRoad.Add(liverpool);
            manchester.byRoad.Add(london);
            manchester.byTrain.Add(edinburgh);
            manchester.byTrain.Add(liverpool);
            manchester.byTrain.Add(london);
            map.Add(manchester);

            swansea.name = "Swansea";
            swansea.abbreviation = "SWA";
            swansea.type = LocationType.Town;
            swansea.isEastern = false;
            swansea.byRoad.Add(liverpool);
            swansea.byRoad.Add(london);
            swansea.byTrain.Add(london);
            swansea.bySea.Add(irishsea);
            map.Add(swansea);

            plymouth.name = "Plymouth";
            plymouth.abbreviation = "PLY";
            plymouth.type = LocationType.Town;
            plymouth.isEastern = false;
            plymouth.byRoad.Add(london);
            plymouth.bySea.Add(englishchannel);
            map.Add(plymouth);

            nantes.name = "Nantes";
            nantes.abbreviation = "NAN";
            nantes.type = LocationType.City;
            nantes.isEastern = false;
            nantes.byRoad.Add(lehavre);
            nantes.byRoad.Add(paris);
            nantes.byRoad.Add(clermontferrand);
            nantes.byRoad.Add(bordeaux);
            nantes.bySea.Add(bayofbiscay);
            map.Add(nantes);

            lehavre.name = "Le Havre";
            lehavre.abbreviation = "LEH";
            lehavre.type = LocationType.Town;
            lehavre.isEastern = false;
            lehavre.byRoad.Add(nantes);
            lehavre.byRoad.Add(paris);
            lehavre.byRoad.Add(brussels);
            lehavre.byTrain.Add(paris);
            lehavre.bySea.Add(englishchannel);
            map.Add(lehavre);

            london.name = "London";
            london.abbreviation = "LON";
            london.type = LocationType.City;
            london.isEastern = false;
            london.byRoad.Add(manchester);
            london.byRoad.Add(swansea);
            london.byRoad.Add(plymouth);
            london.byTrain.Add(manchester);
            london.byTrain.Add(swansea);
            london.bySea.Add(englishchannel);
            map.Add(london);

            paris.name = "Paris";
            paris.abbreviation = "PAR";
            paris.type = LocationType.City;
            paris.isEastern = false;
            paris.byRoad.Add(nantes);
            paris.byRoad.Add(lehavre);
            paris.byRoad.Add(brussels);
            paris.byRoad.Add(strasbourg);
            paris.byRoad.Add(geneva);
            paris.byRoad.Add(clermontferrand);
            paris.byTrain.Add(lehavre);
            paris.byTrain.Add(brussels);
            paris.byTrain.Add(marseilles);
            paris.byTrain.Add(bordeaux);
            map.Add(paris);

            brussels.name = "Brussels";
            brussels.abbreviation = "BRU";
            brussels.type = LocationType.City;
            brussels.isEastern = false;
            brussels.byRoad.Add(lehavre);
            brussels.byRoad.Add(amsterdam);
            brussels.byRoad.Add(cologne);
            brussels.byRoad.Add(strasbourg);
            brussels.byRoad.Add(paris);
            brussels.byTrain.Add(cologne);
            brussels.byTrain.Add(paris);
            map.Add(brussels);

            amsterdam.name = "Amsterdam";
            amsterdam.abbreviation = "AMS";
            amsterdam.type = LocationType.City;
            amsterdam.isEastern = false;
            amsterdam.byRoad.Add(brussels);
            amsterdam.byRoad.Add(cologne);
            amsterdam.bySea.Add(northsea);
            map.Add(amsterdam);

            strasbourg.name = "Strasbourg";
            strasbourg.abbreviation = "STR";
            strasbourg.type = LocationType.Town;
            strasbourg.isEastern = false;
            strasbourg.byRoad.Add(paris);
            strasbourg.byRoad.Add(brussels);
            strasbourg.byRoad.Add(cologne);
            strasbourg.byRoad.Add(frankfurt);
            strasbourg.byRoad.Add(nuremburg);
            strasbourg.byRoad.Add(munich);
            strasbourg.byRoad.Add(zurich);
            strasbourg.byRoad.Add(geneva);
            strasbourg.byTrain.Add(frankfurt);
            strasbourg.byTrain.Add(zurich);
            map.Add(strasbourg);

            cologne.name = "Cologne";
            cologne.abbreviation = "COL";
            cologne.type = LocationType.City;
            cologne.isEastern = false;
            cologne.byRoad.Add(brussels);
            cologne.byRoad.Add(amsterdam);
            cologne.byRoad.Add(hamburg);
            cologne.byRoad.Add(leipzig);
            cologne.byRoad.Add(frankfurt);
            cologne.byRoad.Add(strasbourg);
            cologne.byTrain.Add(brussels);
            cologne.byTrain.Add(frankfurt);
            map.Add(cologne);

            hamburg.name = "Hamburg";
            hamburg.abbreviation = "HAM";
            hamburg.type = LocationType.City;
            hamburg.isEastern = false;
            hamburg.byRoad.Add(cologne);
            hamburg.byRoad.Add(berlin);
            hamburg.byRoad.Add(leipzig);
            hamburg.byTrain.Add(berlin);
            hamburg.bySea.Add(northsea);
            map.Add(hamburg);

            frankfurt.name = "Frankfurt";
            frankfurt.abbreviation = "FRA";
            frankfurt.type = LocationType.Town;
            frankfurt.isEastern = false;
            frankfurt.byRoad.Add(strasbourg);
            frankfurt.byRoad.Add(cologne);
            frankfurt.byRoad.Add(leipzig);
            frankfurt.byRoad.Add(nuremburg);
            frankfurt.byTrain.Add(strasbourg);
            frankfurt.byTrain.Add(cologne);
            frankfurt.byTrain.Add(leipzig);
            map.Add(frankfurt);

            nuremburg.name = "Nuremburg";
            nuremburg.abbreviation = "NUR";
            nuremburg.type = LocationType.Town;
            nuremburg.isEastern = false;
            nuremburg.byRoad.Add(strasbourg);
            nuremburg.byRoad.Add(frankfurt);
            nuremburg.byRoad.Add(leipzig);
            nuremburg.byRoad.Add(prague);
            nuremburg.byRoad.Add(munich);
            nuremburg.byTrain.Add(leipzig);
            nuremburg.byTrain.Add(munich);
            map.Add(nuremburg);

            leipzig.name = "Leipzig";
            leipzig.abbreviation = "LEI";
            leipzig.type = LocationType.City;
            leipzig.isEastern = false;
            leipzig.byRoad.Add(cologne);
            leipzig.byRoad.Add(hamburg);
            leipzig.byRoad.Add(berlin);
            leipzig.byRoad.Add(nuremburg);
            leipzig.byRoad.Add(frankfurt);
            leipzig.byTrain.Add(frankfurt);
            leipzig.byTrain.Add(berlin);
            leipzig.byTrain.Add(nuremburg);
            map.Add(leipzig);

            berlin.name = "Berlin";
            berlin.abbreviation = "BER";
            berlin.type = LocationType.City;
            berlin.isEastern = false;
            berlin.byRoad.Add(hamburg);
            berlin.byRoad.Add(prague);
            berlin.byRoad.Add(leipzig);
            berlin.byTrain.Add(hamburg);
            berlin.byTrain.Add(leipzig);
            berlin.byTrain.Add(prague);
            map.Add(berlin);

            prague.name = "Prague";
            prague.abbreviation = "PRA";
            prague.type = LocationType.City;
            prague.isEastern = true;
            prague.byRoad.Add(berlin);
            prague.byRoad.Add(vienna);
            prague.byRoad.Add(nuremburg);
            prague.byTrain.Add(berlin);
            prague.byTrain.Add(vienna);
            map.Add(prague);

            castledracula.name = "Castle Dracula";
            castledracula.abbreviation = "CAS";
            castledracula.type = LocationType.Castle;
            castledracula.isEastern = true;
            castledracula.byRoad.Add(klausenburg);
            castledracula.byRoad.Add(galatz);
            map.Add(castledracula);

            santander.name = "Santander";
            santander.abbreviation = "SAN";
            santander.type = LocationType.Town;
            santander.isEastern = false;
            santander.byRoad.Add(lisbon);
            santander.byRoad.Add(madrid);
            santander.byRoad.Add(saragossa);
            santander.byTrain.Add(madrid);
            santander.bySea.Add(bayofbiscay);
            map.Add(santander);

            saragossa.name = "Saragossa";
            saragossa.abbreviation = "SAG";
            saragossa.type = LocationType.Town;
            saragossa.isEastern = false;
            saragossa.byRoad.Add(madrid);
            saragossa.byRoad.Add(santander);
            saragossa.byRoad.Add(bordeaux);
            saragossa.byRoad.Add(toulouse);
            saragossa.byRoad.Add(barcelona);
            saragossa.byRoad.Add(alicante);
            saragossa.byTrain.Add(madrid);
            saragossa.byTrain.Add(bordeaux);
            saragossa.byTrain.Add(barcelona);
            map.Add(saragossa);

            bordeaux.name = "Bordeaux";
            bordeaux.abbreviation = "BOR";
            bordeaux.type = LocationType.City;
            bordeaux.isEastern = false;
            bordeaux.byRoad.Add(saragossa);
            bordeaux.byRoad.Add(nantes);
            bordeaux.byRoad.Add(clermontferrand);
            bordeaux.byRoad.Add(toulouse);
            bordeaux.byTrain.Add(paris);
            bordeaux.byTrain.Add(saragossa);
            bordeaux.bySea.Add(bayofbiscay);
            map.Add(bordeaux);

            toulouse.name = "Toulouse";
            toulouse.abbreviation = "TOU";
            toulouse.type = LocationType.Town;
            toulouse.isEastern = false;
            toulouse.byRoad.Add(saragossa);
            toulouse.byRoad.Add(bordeaux);
            toulouse.byRoad.Add(clermontferrand);
            toulouse.byRoad.Add(marseilles);
            toulouse.byRoad.Add(barcelona);
            map.Add(toulouse);

            barcelona.name = "Barcelona";
            barcelona.abbreviation = "BAC";
            barcelona.type = LocationType.City;
            barcelona.isEastern = false;
            barcelona.byRoad.Add(saragossa);
            barcelona.byRoad.Add(toulouse);
            barcelona.byTrain.Add(saragossa);
            barcelona.byTrain.Add(alicante);
            barcelona.bySea.Add(mediterraneansea);
            map.Add(barcelona);

            clermontferrand.name = "Clermont Ferrand";
            clermontferrand.abbreviation = "CLE";
            clermontferrand.type = LocationType.Town;
            clermontferrand.isEastern = false;
            clermontferrand.byRoad.Add(bordeaux);
            clermontferrand.byRoad.Add(nantes);
            clermontferrand.byRoad.Add(paris);
            clermontferrand.byRoad.Add(geneva);
            clermontferrand.byRoad.Add(marseilles);
            clermontferrand.byRoad.Add(toulouse);
            map.Add(clermontferrand);

            marseilles.name = "Marseilles";
            marseilles.abbreviation = "MAR";
            marseilles.type = LocationType.City;
            marseilles.isEastern = false;
            marseilles.byRoad.Add(toulouse);
            marseilles.byRoad.Add(clermontferrand);
            marseilles.byRoad.Add(geneva);
            marseilles.byRoad.Add(zurich);
            marseilles.byRoad.Add(milan);
            marseilles.byRoad.Add(genoa);
            marseilles.byTrain.Add(paris);
            marseilles.bySea.Add(mediterraneansea);
            map.Add(marseilles);

            geneva.name = "Geneva";
            geneva.abbreviation = "GEV";
            geneva.type = LocationType.Town;
            geneva.isEastern = false;
            geneva.byRoad.Add(marseilles);
            geneva.byRoad.Add(clermontferrand);
            geneva.byRoad.Add(paris);
            geneva.byRoad.Add(strasbourg);
            geneva.byRoad.Add(zurich);
            geneva.byTrain.Add(milan);
            map.Add(geneva);

            genoa.name = "Genoa";
            genoa.abbreviation = "GEO";
            genoa.type = LocationType.City;
            genoa.isEastern = true;
            genoa.byRoad.Add(marseilles);
            genoa.byRoad.Add(milan);
            genoa.byRoad.Add(venice);
            genoa.byRoad.Add(florence);
            genoa.byTrain.Add(milan);
            genoa.bySea.Add(tyrrheniansea);
            map.Add(genoa);

            milan.name = "Milan";
            milan.abbreviation = "MIL";
            milan.type = LocationType.City;
            milan.isEastern = true;
            milan.byRoad.Add(marseilles);
            milan.byRoad.Add(zurich);
            milan.byRoad.Add(munich);
            milan.byRoad.Add(venice);
            milan.byRoad.Add(genoa);
            milan.byTrain.Add(geneva);
            milan.byTrain.Add(zurich);
            milan.byTrain.Add(florence);
            milan.byTrain.Add(genoa);
            map.Add(milan);

            zurich.name = "Zurich";
            zurich.abbreviation = "ZUR";
            zurich.type = LocationType.Town;
            zurich.isEastern = false;
            zurich.byRoad.Add(marseilles);
            zurich.byRoad.Add(geneva);
            zurich.byRoad.Add(strasbourg);
            zurich.byRoad.Add(munich);
            zurich.byRoad.Add(milan);
            zurich.byTrain.Add(strasbourg);
            zurich.byTrain.Add(milan);
            map.Add(zurich);

            florence.name = "Florence";
            florence.abbreviation = "FLO";
            florence.type = LocationType.Town;
            florence.isEastern = true;
            florence.byRoad.Add(genoa);
            florence.byRoad.Add(venice);
            florence.byRoad.Add(rome);
            florence.byTrain.Add(milan);
            florence.byTrain.Add(rome);
            map.Add(florence);

            venice.name = "Venice";
            venice.abbreviation = "VEN";
            venice.type = LocationType.Town;
            venice.isEastern = true;
            venice.byRoad.Add(florence);
            venice.byRoad.Add(genoa);
            venice.byRoad.Add(milan);
            venice.byRoad.Add(munich);
            venice.byTrain.Add(vienna);
            venice.bySea.Add(adriaticsea);
            map.Add(venice);

            munich.name = "Munich";
            munich.abbreviation = "MUN";
            munich.type = LocationType.City;
            munich.isEastern = false;
            munich.byRoad.Add(milan);
            munich.byRoad.Add(zurich);
            munich.byRoad.Add(strasbourg);
            munich.byRoad.Add(nuremburg);
            munich.byRoad.Add(vienna);
            munich.byRoad.Add(zagreb);
            munich.byRoad.Add(venice);
            munich.byTrain.Add(nuremburg);
            map.Add(munich);

            zagreb.name = "Zagreb";
            zagreb.abbreviation = "ZAG";
            zagreb.type = LocationType.Town;
            zagreb.isEastern = true;
            zagreb.byRoad.Add(munich);
            zagreb.byRoad.Add(vienna);
            zagreb.byRoad.Add(budapest);
            zagreb.byRoad.Add(szeged);
            zagreb.byRoad.Add(stjosephandstmary);
            zagreb.byRoad.Add(sarajevo);
            map.Add(zagreb);

            vienna.name = "Vienna";
            vienna.abbreviation = "VIE";
            vienna.type = LocationType.City;
            vienna.isEastern = true;
            vienna.byRoad.Add(munich);
            vienna.byRoad.Add(prague);
            vienna.byRoad.Add(budapest);
            vienna.byRoad.Add(zagreb);
            vienna.byTrain.Add(venice);
            vienna.byTrain.Add(prague);
            vienna.byTrain.Add(budapest);
            map.Add(vienna);

            stjosephandstmary.name = "St. Joseph & St. Mary";
            stjosephandstmary.abbreviation = "STJ";
            stjosephandstmary.type = LocationType.Hospital;
            stjosephandstmary.isEastern = true;
            stjosephandstmary.byRoad.Add(zagreb);
            stjosephandstmary.byRoad.Add(szeged);
            stjosephandstmary.byRoad.Add(belgrade);
            stjosephandstmary.byRoad.Add(sarajevo);
            map.Add(stjosephandstmary);

            sarajevo.name = "Sarajevo";
            sarajevo.abbreviation = "SAJ";
            sarajevo.type = LocationType.Town;
            sarajevo.isEastern = true;
            sarajevo.byRoad.Add(zagreb);
            sarajevo.byRoad.Add(stjosephandstmary);
            sarajevo.byRoad.Add(belgrade);
            sarajevo.byRoad.Add(sofia);
            sarajevo.byRoad.Add(valona);
            map.Add(sarajevo);

            szeged.name = "Szeged";
            szeged.abbreviation = "SZE";
            szeged.type = LocationType.Town;
            szeged.isEastern = true;
            szeged.byRoad.Add(zagreb);
            szeged.byRoad.Add(budapest);
            szeged.byRoad.Add(klausenburg);
            szeged.byRoad.Add(belgrade);
            szeged.byRoad.Add(stjosephandstmary);
            szeged.byTrain.Add(budapest);
            szeged.byTrain.Add(bucharest);
            szeged.byTrain.Add(belgrade);
            map.Add(szeged);

            budapest.name = "Budapest";
            budapest.abbreviation = "BUD";
            budapest.type = LocationType.City;
            budapest.isEastern = true;
            budapest.byRoad.Add(vienna);
            budapest.byRoad.Add(klausenburg);
            budapest.byRoad.Add(szeged);
            budapest.byRoad.Add(zagreb);
            budapest.byTrain.Add(vienna);
            budapest.byTrain.Add(szeged);
            map.Add(budapest);

            belgrade.name = "Belgrade";
            belgrade.abbreviation = "BEL";
            belgrade.type = LocationType.Town;
            belgrade.isEastern = true;
            belgrade.byRoad.Add(stjosephandstmary);
            belgrade.byRoad.Add(szeged);
            belgrade.byRoad.Add(klausenburg);
            belgrade.byRoad.Add(bucharest);
            belgrade.byRoad.Add(sofia);
            belgrade.byRoad.Add(sarajevo);
            belgrade.byTrain.Add(szeged);
            belgrade.byTrain.Add(sofia);
            map.Add(belgrade);

            klausenburg.name = "Klausenburg";
            klausenburg.abbreviation = "KLA";
            klausenburg.type = LocationType.Town;
            klausenburg.isEastern = true;
            klausenburg.byRoad.Add(budapest);
            klausenburg.byRoad.Add(castledracula);
            klausenburg.byRoad.Add(galatz);
            klausenburg.byRoad.Add(bucharest);
            klausenburg.byRoad.Add(belgrade);
            klausenburg.byRoad.Add(szeged);
            map.Add(klausenburg);

            sofia.name = "Sofia";
            sofia.abbreviation = "SOF";
            sofia.type = LocationType.Town;
            sofia.isEastern = true;
            sofia.byRoad.Add(sarajevo);
            sofia.byRoad.Add(belgrade);
            sofia.byRoad.Add(bucharest);
            sofia.byRoad.Add(varna);
            sofia.byRoad.Add(salonica);
            sofia.byRoad.Add(valona);
            sofia.byTrain.Add(belgrade);
            sofia.byTrain.Add(salonica);
            map.Add(sofia);

            bucharest.name = "Bucharest";
            bucharest.abbreviation = "BUC";
            bucharest.type = LocationType.City;
            bucharest.isEastern = true;
            bucharest.byRoad.Add(belgrade);
            bucharest.byRoad.Add(klausenburg);
            bucharest.byRoad.Add(galatz);
            bucharest.byRoad.Add(constanta);
            bucharest.byRoad.Add(sofia);
            bucharest.byTrain.Add(szeged);
            bucharest.byTrain.Add(galatz);
            bucharest.byTrain.Add(constanta);
            map.Add(bucharest);

            galatz.name = "Galatz";
            galatz.abbreviation = "GAT";
            galatz.type = LocationType.Town;
            galatz.isEastern = true;
            galatz.byRoad.Add(klausenburg);
            galatz.byRoad.Add(castledracula);
            galatz.byRoad.Add(constanta);
            galatz.byRoad.Add(bucharest);
            galatz.byTrain.Add(bucharest);
            map.Add(galatz);

            varna.name = "Varna";
            varna.abbreviation = "VAR";
            varna.type = LocationType.City;
            varna.isEastern = true;
            varna.byRoad.Add(sofia);
            varna.byRoad.Add(constanta);
            varna.byTrain.Add(sofia);
            varna.bySea.Add(blacksea);
            map.Add(varna);

            constanta.name = "Constanta";
            constanta.abbreviation = "CON";
            constanta.type = LocationType.City;
            constanta.isEastern = true;
            constanta.byRoad.Add(galatz);
            constanta.byRoad.Add(varna);
            constanta.byRoad.Add(bucharest);
            constanta.byTrain.Add(bucharest);
            constanta.bySea.Add(blacksea);
            map.Add(constanta);

            lisbon.name = "Lisbon";
            lisbon.abbreviation = "LIS";
            lisbon.type = LocationType.City;
            lisbon.isEastern = false;
            lisbon.byRoad.Add(santander);
            lisbon.byRoad.Add(madrid);
            lisbon.byRoad.Add(cadiz);
            lisbon.byTrain.Add(madrid);
            lisbon.bySea.Add(atlanticocean);
            map.Add(lisbon);

            cadiz.name = "Cadiz";
            cadiz.abbreviation = "CAD";
            cadiz.type = LocationType.City;
            cadiz.isEastern = false;
            cadiz.byRoad.Add(lisbon);
            cadiz.byRoad.Add(madrid);
            cadiz.byRoad.Add(granada);
            cadiz.bySea.Add(atlanticocean);
            map.Add(cadiz);

            madrid.name = "Madrid";
            madrid.abbreviation = "MAD";
            madrid.type = LocationType.City;
            madrid.isEastern = false;
            madrid.byRoad.Add(lisbon);
            madrid.byRoad.Add(santander);
            madrid.byRoad.Add(saragossa);
            madrid.byRoad.Add(alicante);
            madrid.byRoad.Add(granada);
            madrid.byRoad.Add(cadiz);
            madrid.byTrain.Add(lisbon);
            madrid.byTrain.Add(santander);
            madrid.byTrain.Add(saragossa);
            madrid.byTrain.Add(alicante);
            map.Add(madrid);

            granada.name = "Granada";
            granada.abbreviation = "GRA";
            granada.type = LocationType.Town;
            granada.isEastern = false;
            granada.byRoad.Add(cadiz);
            granada.byRoad.Add(madrid);
            granada.byRoad.Add(alicante);
            map.Add(granada);

            alicante.name = "Alicante";
            alicante.abbreviation = "ALI";
            alicante.type = LocationType.Town;
            alicante.isEastern = false;
            alicante.byRoad.Add(granada);
            alicante.byRoad.Add(madrid);
            alicante.byRoad.Add(saragossa);
            alicante.byTrain.Add(madrid);
            alicante.byTrain.Add(barcelona);
            alicante.bySea.Add(mediterraneansea);
            map.Add(alicante);

            cagliari.name = "Cagliari";
            cagliari.abbreviation = "CAG";
            cagliari.type = LocationType.Town;
            cagliari.isEastern = true;
            cagliari.bySea.Add(mediterraneansea);
            cagliari.bySea.Add(tyrrheniansea);
            map.Add(cagliari);

            rome.name = "Rome";
            rome.abbreviation = "ROM";
            rome.type = LocationType.City;
            rome.isEastern = true;
            rome.byRoad.Add(florence);
            rome.byRoad.Add(bari);
            rome.byRoad.Add(naples);
            rome.byTrain.Add(florence);
            rome.byTrain.Add(naples);
            rome.bySea.Add(tyrrheniansea);
            map.Add(rome);

            naples.name = "Naples";
            naples.abbreviation = "NAP";
            naples.type = LocationType.City;
            naples.isEastern = true;
            naples.byRoad.Add(rome);
            naples.byRoad.Add(bari);
            naples.byTrain.Add(rome);
            naples.byTrain.Add(bari);
            naples.bySea.Add(tyrrheniansea);
            map.Add(naples);

            bari.name = "Bari";
            bari.abbreviation = "BAI";
            bari.type = LocationType.Town;
            bari.isEastern = true;
            bari.byRoad.Add(naples);
            bari.byRoad.Add(rome);
            bari.byTrain.Add(naples);
            bari.bySea.Add(adriaticsea);
            map.Add(bari);

            valona.name = "Valona";
            valona.abbreviation = "VAL";
            valona.type = LocationType.Town;
            valona.isEastern = true;
            valona.byRoad.Add(sarajevo);
            valona.byRoad.Add(sofia);
            valona.byRoad.Add(salonica);
            valona.byRoad.Add(athens);
            valona.bySea.Add(ioniansea);
            map.Add(valona);

            salonica.name = "Salonica";
            salonica.abbreviation = "SAL";
            salonica.type = LocationType.Town;
            salonica.isEastern = true;
            salonica.byRoad.Add(valona);
            salonica.byRoad.Add(sofia);
            salonica.byTrain.Add(sofia);
            salonica.bySea.Add(ioniansea);
            map.Add(salonica);

            athens.name = "Athens";
            athens.abbreviation = "ATH";
            athens.type = LocationType.City;
            athens.isEastern = true;
            athens.byRoad.Add(valona);
            athens.bySea.Add(ioniansea);
            map.Add(athens);

            atlanticocean.name = "Atlantic Ocean";
            atlanticocean.abbreviation = "ATL";
            atlanticocean.type = LocationType.Sea;
            atlanticocean.isEastern = false;
            atlanticocean.bySea.Add(northsea);
            atlanticocean.bySea.Add(irishsea);
            atlanticocean.bySea.Add(englishchannel);
            atlanticocean.bySea.Add(bayofbiscay);
            atlanticocean.bySea.Add(mediterraneansea);
            atlanticocean.bySea.Add(galway);
            atlanticocean.bySea.Add(lisbon);
            atlanticocean.bySea.Add(cadiz);
            map.Add(atlanticocean);

            irishsea.name = "Irish Sea";
            irishsea.abbreviation = "IRI";
            irishsea.type = LocationType.Sea;
            irishsea.isEastern = false;
            irishsea.bySea.Add(atlanticocean);
            irishsea.bySea.Add(dublin);
            irishsea.bySea.Add(liverpool);
            irishsea.bySea.Add(swansea);
            map.Add(irishsea);

            englishchannel.name = "English Channel";
            englishchannel.abbreviation = "ENG";
            englishchannel.type = LocationType.Sea;
            englishchannel.isEastern = false;
            englishchannel.bySea.Add(atlanticocean);
            englishchannel.bySea.Add(northsea);
            englishchannel.bySea.Add(plymouth);
            englishchannel.bySea.Add(london);
            englishchannel.bySea.Add(lehavre);
            map.Add(englishchannel);

            northsea.name = "North Sea";
            northsea.abbreviation = "NOR";
            northsea.type = LocationType.Sea;
            northsea.isEastern = false;
            northsea.bySea.Add(atlanticocean);
            northsea.bySea.Add(englishchannel);
            northsea.bySea.Add(edinburgh);
            northsea.bySea.Add(amsterdam);
            northsea.bySea.Add(hamburg);
            map.Add(northsea);

            bayofbiscay.name = "Bay of Biscay";
            bayofbiscay.abbreviation = "BAY";
            bayofbiscay.type = LocationType.Sea;
            bayofbiscay.isEastern = false;
            bayofbiscay.bySea.Add(atlanticocean);
            bayofbiscay.bySea.Add(nantes);
            bayofbiscay.bySea.Add(bordeaux);
            bayofbiscay.bySea.Add(santander);
            map.Add(bayofbiscay);

            mediterraneansea.name = "Mediterranean Sea";
            mediterraneansea.abbreviation = "MED";
            mediterraneansea.type = LocationType.Sea;
            mediterraneansea.isEastern = true;
            mediterraneansea.bySea.Add(atlanticocean);
            mediterraneansea.bySea.Add(tyrrheniansea);
            mediterraneansea.bySea.Add(alicante);
            mediterraneansea.bySea.Add(barcelona);
            mediterraneansea.bySea.Add(marseilles);
            mediterraneansea.bySea.Add(cagliari);
            map.Add(mediterraneansea);

            tyrrheniansea.name = "Tyrrhenian Sea";
            tyrrheniansea.abbreviation = "TYR";
            tyrrheniansea.type = LocationType.Sea;
            tyrrheniansea.isEastern = false;
            tyrrheniansea.bySea.Add(mediterraneansea);
            tyrrheniansea.bySea.Add(ioniansea);
            tyrrheniansea.bySea.Add(cagliari);
            tyrrheniansea.bySea.Add(genoa);
            tyrrheniansea.bySea.Add(rome);
            tyrrheniansea.bySea.Add(naples);
            map.Add(tyrrheniansea);

            adriaticsea.name = "Adriatic Sea";
            adriaticsea.abbreviation = "ADR";
            adriaticsea.type = LocationType.Sea;
            adriaticsea.isEastern = false;
            adriaticsea.bySea.Add(ioniansea);
            adriaticsea.bySea.Add(bari);
            adriaticsea.bySea.Add(venice);
            map.Add(adriaticsea);

            ioniansea.name = "Ionian Sea";
            ioniansea.abbreviation = "ION";
            ioniansea.type = LocationType.Sea;
            ioniansea.isEastern = false;
            ioniansea.bySea.Add(mediterraneansea);
            ioniansea.bySea.Add(adriaticsea);
            ioniansea.bySea.Add(blacksea);
            ioniansea.bySea.Add(valona);
            ioniansea.bySea.Add(athens);
            ioniansea.bySea.Add(salonica);
            map.Add(ioniansea);

            blacksea.name = "Black Sea";
            blacksea.abbreviation = "BLA";
            blacksea.type = LocationType.Sea;
            blacksea.isEastern = false;
            blacksea.bySea.Add(ioniansea);
            blacksea.bySea.Add(varna);
            blacksea.bySea.Add(constanta);
            map.Add(blacksea);
        }

        internal Location LocationAtMapIndex(int v)
        {
            return map[v];
        }

        internal int MapSize()
        {
            return map.Count();
        }

        internal void SetLocationForHunterAt(int v, Location location)
        {
            hunters[v].currentLocation = location;
            Logger.WriteToDebugLog(hunters[v].name + " started in " + location.name);
            Logger.WriteToGameLog(hunters[v].name + " started in " + location.name);
        }

        internal void PlaceDraculaAtStartLocation()
        {
            dracula.currentLocation = dracula.logic.DecideDraculaStartLocation(this);
            dracula.trail.Add(dracula.currentLocation);
            Logger.WriteToDebugLog("Dracula started in " + dracula.currentLocation.name);
            Logger.WriteToGameLog("Dracula started in " + dracula.currentLocation.name);
        }

        internal string TimeOfDay()
        {
            return timesOfDay[Math.Max(0, time)];
        }

        internal void AdjustTime(int v)
        {
            time += v;
        }

        internal int Time()
        {
            return time;
        }

        internal void SetHunterAlly(string v)
        {
            hunterAlly = GetEventByNameFromEventDeck(v);
        }

        internal string NameOfHunterAlly()
        {
            if (HuntersHaveAlly())
            {
                return hunterAlly.name;
            }
            else
            {
                return "No ally";
            }
        }

        internal bool HuntersHaveAlly()
        {
            return hunterAlly != null;
        }

        internal string NameOfDraculaAlly()
        {
            return draculaAlly.name;
        }

        internal void SetDraculaAlly(Event allyDrawn)
        {
            draculaAlly = allyDrawn;
        }

        internal bool DraculaHasAlly()
        {
            return draculaAlly != null;
        }

        internal void AddEventToEventDiscard(Event allyDiscarded)
        {
            eventDiscard.Add(allyDiscarded);
        }

        internal Event GetEventByNameFromEventDeck(string v)
        {
            try
            {
                return eventDeck[eventDeck.FindIndex(card => card.name.ToLower().StartsWith(v.ToLower()))];
            }
            catch (ArgumentOutOfRangeException)
            {
                return new Event("Unknown event", false, EventType.Keep);
            }
        }

        internal string NameOfEventCardAtIndex(int eventIndex)
        {
            return eventDeck[eventIndex].name;
        }

        internal bool EventCardIsDraculaCardAtIndex(int eventIndex)
        {
            return eventDeck[eventIndex].isDraculaCard;
        }

        internal int IndexOfEventCardInEventDeck(string argument2)
        {
            return eventDeck.FindIndex(card => card.name.ToUpper().StartsWith(argument2.ToUpper()));
        }

        internal void RemoveEventFromEventDeck(Event eventCardDrawn)
        {
            eventDeck.Remove(eventCardDrawn);
        }

        internal Event DrawEventCard()
        {
            return eventDeck[new Random().Next(0, eventDeck.Count())];
        }

        internal void RemoveEncounterFromEncounterPool(Encounter tempEncounter)
        {
            encounterPool.Remove(tempEncounter);
        }

        internal Encounter DrawEncounterFromEncounterPool()
        {
            return encounterPool[new Random().Next(0, encounterPool.Count())];
        }

        internal void AddEncounterToEncounterPool(Encounter encounterToDiscard)
        {
            encounterPool.Add(encounterToDiscard);
        }

        internal void MoveHunterToLocationAtHunterIndex(int hunterIndex, Location locationToMoveTo)
        {
            foreach (Hunter h in hunters[hunterIndex].huntersInGroup)
            {
                h.currentLocation = locationToMoveTo;
                Logger.WriteToDebugLog("Moved " + h.name + " to " + locationToMoveTo.name);
                Logger.WriteToGameLog(h.name + " moved to " + locationToMoveTo.name);
            }
        }

        internal string NameOfHunterAtIndex(int hunterIndex)
        {
            return hunters[hunterIndex].name;
        }

        internal void DrawEncounterAtCatacombIndex(int i, bool v)
        {
            dracula.catacombs[i].DrawEncounter(true);
        }

        internal void DrawEncounterAtTrailIndex(int i)
        {
            dracula.trail[i].DrawEncounter();
        }

        internal void DrawEncounterAtCatacombIndex(int i)
        {
            dracula.catacombs[i].DrawEncounter();
        }

        internal int NumberOfEncountersAtLocationAtCatacombIndex(int i)
        {
            return dracula.catacombs[i].encounters.Count();
        }

        internal string DraculaPowerNameAtPowerIndex(int i)
        {
            return dracula.powers[i].name;
        }

        internal bool DraculaPowerAtPowerIndexIsAtLocationIndex(int i, int counter)
        {
            return dracula.powers[i].positionInTrail == counter;
        }

        internal int NumberOfDraculaPowers()
        {
            return dracula.powers.Count();
        }

        internal int NumberOfEventCardsInDraculaHand()
        {
            return dracula.eventCardsInHand.Count();
        }

        internal string LocationAbbreviationAtCatacombIndex(int i)
        {
            return dracula.catacombs[i].abbreviation;
        }

        internal bool LocationIsRevealedAtCatacombIndex(int i)
        {
            return dracula.catacombs[i].isRevealed;
        }

        internal bool LocationIsEmptyAtCatacombIndex(int i)
        {
            return dracula.catacombs[i] == null;
        }

        internal int VampireTracker()
        {
            return vampireTracker;
        }

        internal int DraculaBloodLevel()
        {
            return dracula.blood;
        }

        internal void DrawLocationAtTrailIndex(int i)
        {
            dracula.trail[i].DrawLocation();
        }

        internal string NameOfLocationAtTrailIndex(int checkingLocationIndex)
        {
            return dracula.trail[checkingLocationIndex].name;
        }

        internal void RevealHide(UserInterface ui)
        {
            dracula.RevealHide(ui);
        }

        internal bool LocationWhereHideWasUsedIsDraculaCurrentLocation()
        {
            return dracula.locationWhereHideWasUsed == dracula.currentLocation;
        }

        internal bool DraculaCurrentLocationIsAtTrailIndex(int checkingLocationIndex)
        {
            return dracula.trail[checkingLocationIndex] == dracula.currentLocation;
        }

        internal bool LocationIsRevealedAtTrailIndex(int checkingLocationIndex)
        {
            return dracula.trail[checkingLocationIndex].isRevealed;
        }

        internal LocationType TypeOfLocationAtTrailIndex(int checkingLocationIndex)
        {
            return dracula.trail[checkingLocationIndex].type;
        }

        internal int TrailLength()
        {
            return dracula.trail.Count();
        }

        internal bool LocationIsInTrail(object locationToReveal)
        {
            return dracula.trail.Contains(locationToReveal);
        }

        internal void PerformDraculaTurn(UserInterface ui)
        {
            if (dracula.currentLocation.type != LocationType.Sea)
            {
                time = (time + 1) % 6;
                Logger.WriteToDebugLog("Time is now " + timesOfDay[time]);
                if (time == 0)
                {
                    vampireTracker++;
                    resolve++;
                    Logger.WriteToDebugLog("Increasing vampire track to " + vampireTracker);
                    Logger.WriteToDebugLog("Increasing resolve to " + resolve);
                    if (vampireTracker > 0)
                    {
                        Logger.WriteToGameLog("Dracula earned a point, up to " + vampireTracker);
                        Logger.WriteToGameLog("Hunters gained a point of resolve, up to " + resolve);
                    }
                }
            }
            else
            {
                Logger.WriteToDebugLog("Dracula is at sea, skipping Timekeeping phase so time remains " + timesOfDay[Math.Max(0, time)]);
            }
            dracula.TakeStartOfTurnActions(this, ui);
            dracula.MoveDracula(this, ui);
            dracula.HandleDroppedOffLocations(this, ui);
            dracula.DoActionPhase(this, ui);
            dracula.MatureEncounters(this, ui);
            dracula.DrawEncounters(this, dracula.encounterHandSize);
            foreach (Encounter enc in encounterLimbo)
            {
                encounterPool.Add(enc);
                encounterLimbo.Remove(enc);
            }

        }

        internal void RevealEncounterInTrail(int v)
        {
            try
            {
                for (int i = 0; i < dracula.trail[v].encounters.Count(); i++)
                {
                    dracula.trail[v].encounters[i].isRevealed = true;
                }
            }
            catch (ArgumentOutOfRangeException)
            { }
        }

        internal void TrimDraculaTrail(int trailLength)
        {
            dracula.TrimTrail(this, Math.Max(1, trailLength));
        }

        internal void DiscardDraculaCardsDownToHandSize(UserInterface ui)
        {
            dracula.DiscardEventsDownTo(this, dracula.eventHandSize, ui);
        }

        internal void DrawEventCardForDracula(UserInterface ui)
        {
            dracula.DrawEventCard(this, ui);
        }

        internal void RevealLocationAtTrailIndex(int trailIndex, UserInterface ui)
        {
            if (dracula.trail[trailIndex].name == "Hide")
            {
                dracula.RevealHide(ui);
            }
            else
            {
                dracula.trail[trailIndex].isRevealed = true;
            }
        }

        internal string DraculaCurrentLocationName()
        {
            return dracula.currentLocation.name;
        }

        public Location GetLocationFromName(string locationName)
        {
            for (int i = 0; i < map.Count(); i++)
            {
                if ((map[i].name.ToLower().StartsWith(locationName.ToLower())) || (map[i].abbreviation.ToLower() == locationName.ToLower()))
                {
                    return map[i];
                }
            }
            Location unknownLocation = new Location();
            unknownLocation.name = "Unknown location";
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
            vampireTracker += 2;
            dracula.TrimTrail(this, 1);
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
                Event cardDrawn = eventDeck[new Random().Next(0, eventDeck.Count())];
                if (!cardDrawn.isDraculaCard)
                {
                    ui.TellUser("Dracula drew " + cardDrawn.name + ", discarded");
                    eventDeck.Remove(cardDrawn);
                    eventDiscard.Add(cardDrawn);
                }
                else
                {
                    switch (cardDrawn.type)
                    {
                        case EventType.Ally: dracula.PlayAlly(this, cardDrawn, ui); break;
                        case EventType.Keep: dracula.eventCardsInHand.Add(cardDrawn); break;
                        case EventType.PlayImmediately: dracula.PlayImmediately(cardDrawn); break;
                    }
                }
            }
            dracula.DiscardEventsDownTo(this, dracula.eventHandSize, ui);
            dracula.TrimTrail(this, 3);
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
            throw new NotImplementedException();
        }

        private bool ResolveAmbush(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Ambush");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Ambush");
            ui.TellUser(huntersEncountered.First().name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].name + " ");
            }
            ui.TellUser("encountered an Ambush");
            dracula.DrawEncounters(this, dracula.encounterHand.Count() + 1);
            dracula.DiscardEncountersDownTo(this, dracula.encounterHandSize);
            return true;
        }

        public bool ResolveAssassin(List<Hunter> huntersEncountered, UserInterface ui)
        {
            ui.TellUser("Conduct a combat with an Assasin");
            int hunterIndex = 0;
            switch (huntersEncountered[0].name)
            {
                case "Lord Godalming": hunterIndex = 0; break;
                case "Van Helsing": hunterIndex = 1; break;
                case "Dr. Seward": hunterIndex = 2; break;
                case "Mina Harker": hunterIndex = 3; break;
            }
            if (ResolveCombat(hunterIndex, 5, ui) == "Enemy killed")
            {
                return true;
            }
            return false;
        }

        private bool ResolveBats(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Bats");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Bats");
            ui.TellUser(huntersEncountered.First().name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].name + " ");
            }
            ui.TellUser("encountered Bats");
            ui.TellUser("Tell me at the start of your next turn and I will move you");
            return false;
        }

        private bool ResolveDesecratedSoil(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Desecrated Soil");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Desecrated Soil");
            ui.TellUser(huntersEncountered.First().name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].name + " ");
            }
            ui.TellUser("encountered Desecrated Soil");
            Event cardDrawn = eventDeck[new Random().Next(0, eventDeck.Count())];
            if (!cardDrawn.isDraculaCard)
            {
                ui.TellUser("Dracula drew " + cardDrawn.name + ", discarded");
                eventDeck.Remove(cardDrawn);
                eventDiscard.Add(cardDrawn);
            }
            else
            {
                switch (cardDrawn.type)
                {
                    case EventType.Ally: dracula.PlayAlly(this, cardDrawn, ui); break;
                    case EventType.Keep: dracula.eventCardsInHand.Add(cardDrawn); break;
                    case EventType.PlayImmediately: dracula.PlayImmediately(cardDrawn); break;
                }
            }
            dracula.DiscardEventsDownTo(this, dracula.eventHandSize, ui);
            return true;
        }

        public bool ResolveFog(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Desecrated Fog");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Desecrated Fog");
            ui.TellUser(huntersEncountered.First().name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i] + " ");
            }
            ui.TellUser("encountered Fog");
            ui.TellUser("Place Fog in front of you until the end of your turn");
            return false;
        }

        public bool ResolveMinionWithKnife(List<Hunter> huntersEncountered, UserInterface ui)
        {
            ui.TellUser("Conduct a combat with a Minion with Knife");
            int hunterIndex = 0;
            switch (huntersEncountered[0].name)
            {
                case "Lord Godalming": hunterIndex = 0; break;
                case "Van Helsing": hunterIndex = 1; break;
                case "Dr. Seward": hunterIndex = 2; break;
                case "Mina Harker": hunterIndex = 3; break;
            }
            if (ResolveCombat(hunterIndex, 2, ui) == "Enemy killed")
            {
                return true;
            }
            return false;
        }

        public bool ResolveMinionWithKnifeAndPistol(List<Hunter> huntersEncountered, UserInterface ui)
        {
            ui.TellUser("Conduct a combat with a Minion with Knife and Pistol");
            int hunterIndex = 0;
            switch (huntersEncountered[0].name)
            {
                case "Lord Godalming": hunterIndex = 0; break;
                case "Van Helsing": hunterIndex = 1; break;
                case "Dr. Seward": hunterIndex = 2; break;
                case "Mina Harker": hunterIndex = 3; break;
            }
            if (ResolveCombat(hunterIndex, 3, ui) == "Enemy killed")
            {
                return true;
            }
            return false;
        }

        public bool ResolveMinionWithKnifeAndRifle(List<Hunter> huntersEncountered, UserInterface ui)
        {
            ui.TellUser("Conduct a combat with a Minion with Knife and Rifle");
            int hunterIndex = 0;
            switch (huntersEncountered[0].name)
            {
                case "Lord Godalming": hunterIndex = 0; break;
                case "Van Helsing": hunterIndex = 1; break;
                case "Dr. Seward": hunterIndex = 2; break;
                case "Mina Harker": hunterIndex = 3; break;
            }
            if (ResolveCombat(hunterIndex, 4, ui) == "Enemy killed")
            {
                return true;
            }
            return false;
        }

        public bool ResolveHoax(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Hoax");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Hoax");
            ui.TellUser(huntersEncountered.First().name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].name + " ");
            }
            ui.TellUser("encountered Hoax");
            ui.TellUser("Discard " + (huntersEncountered.First().currentLocation.isEastern ? "one" : "all") + " of your event cards (don't forget to tell me what is discarded");
            return true;
        }

        public bool ResolveLightning(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Lightning");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Lightning");
            ui.TellUser(huntersEncountered.First().name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].name + " ");
            }
            ui.TellUser("encountered Lightning");
            for (int i = 0; i < huntersEncountered.Count(); i++)
            {
                int answer = ui.GetHunterHolyItems(huntersEncountered[i].name);
                if (answer > 0)
                {
                    Logger.WriteToDebugLog(huntersEncountered[i].name + " negated the encounter with " + (answer == 1 ? "a crucifix" : "a heavenly host"));
                    Logger.WriteToGameLog(huntersEncountered[i].name + " negated the encounter with " + (answer == 1 ? "a crucifix" : "a heavenly host"));
                    ui.TellUser(huntersEncountered[i].name + " negated the encounter with " + (answer == 1 ? "a crucifix" : "a heavenly host"));
                    return true;
                }
            }
            for (int i = 0; i < huntersEncountered.Count(); i++)
            {
                Logger.WriteToDebugLog(huntersEncountered[i].name + " loses 2 health and discards 1 item");
                Logger.WriteToGameLog(huntersEncountered[i].name + " loses 2 health and discards 1 item");
                ui.TellUser(huntersEncountered[i].name + " loses 2 health and discards 1 item");
                huntersEncountered[i].health -= 2;
            }
            ui.TellUser("Don't forget to tell me what was discarded");
            return !HandlePossibleHunterDeath(ui);
        }

        public bool ResolvePeasants(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Peasants");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Peasants");
            ui.TellUser(huntersEncountered.First().name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].name + " ");
            }
            ui.TellUser("encountered Peasants");
            ui.TellUser("Discard " + (huntersEncountered.First().currentLocation.isEastern ? "one" : "all") + " of your item cards and redraw randomly (don't forget to tell me what is discarded");
            return true;
        }

        private bool ResolvePlague(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Plague");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Plague");
            ui.TellUser(huntersEncountered.First().name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].name + " ");
            }
            ui.TellUser("encountered Plague");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                Logger.WriteToDebugLog(huntersEncountered[i].name + " loses 2 health");
                Logger.WriteToGameLog(huntersEncountered[i].name + " loses 2 health");
                ui.TellUser(huntersEncountered[i].name + " loses 2 health");
                huntersEncountered[i].health -= 2;
            }
            return !HandlePossibleHunterDeath(ui);
        }

        public bool ResolveRats(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Rats");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Rats");
            ui.TellUser(huntersEncountered.First().name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].name + " ");
            }
            ui.TellUser("encountered Rats");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                if (huntersEncountered[i].hasDogsFaceUp)
                {
                    ui.TellUser(huntersEncountered[i].name + " has Dogs face up, Rats have no effect");
                    Logger.WriteToDebugLog(huntersEncountered[i].name + " has Dogs face up, Rats have no effect");
                    Logger.WriteToGameLog(huntersEncountered[i].name + " has Dogs face up, Rats have no effect");
                    return true;
                }
            }
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("Roll dice for " + huntersEncountered[i].name);
                int loss = ui.GetHunterHealthLost(huntersEncountered[i].name);
                huntersEncountered[i].health -= loss;
                Logger.WriteToDebugLog(huntersEncountered[i] + " lost " + loss + " health");
                Logger.WriteToGameLog(huntersEncountered[i] + " lost " + loss + " health");
            }
            return !HandlePossibleHunterDeath(ui);            
        }

        public bool ResolveSaboteur(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Saboteur");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Saboteur");
            ui.TellUser(huntersEncountered.First().name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].name + " ");
            }
            ui.TellUser("encountered Saboteur");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                if (huntersEncountered[i].hasDogsFaceUp)
                {
                    ui.TellUser(huntersEncountered[i].name + " has Dogs face up, Saboteur has no effect");
                    Logger.WriteToDebugLog(huntersEncountered[i].name + " has Dogs face up, Saboteur has no effect");
                    Logger.WriteToGameLog(huntersEncountered[i].name + " has Dogs face up, Saboteur has no effect");
                    return true;
                }
            }
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser(huntersEncountered[i].name + " must discard 1 item or event (don't forget to tell me what was discarded");
            }
            return false;
        }

        public bool ResolveSpy(List<Hunter> huntersEncountered, UserInterface ui)
        {
            throw new NotImplementedException();
        }

        public bool ResolveThief(List<Hunter> huntersEncountered, UserInterface ui)
        {
            Logger.WriteToDebugLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Thief");
            Logger.WriteToGameLog("Hunter" + (huntersEncountered.Count() > 0 ? "s" : "") + " encountered Thief");
            ui.TellUser(huntersEncountered.First().name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].name + " ");
            }
            ui.TellUser("encountered Thief");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                if (huntersEncountered[i].hasDogsFaceUp)
                {
                    ui.TellUser(huntersEncountered[i].name + " has Dogs face up, Thief has no effect");
                    Logger.WriteToDebugLog(huntersEncountered[i].name + " has Dogs face up, Thief has no effect");
                    Logger.WriteToGameLog(huntersEncountered[i].name + " has Dogs face up, Thief has no effect");
                    return true;
                }
            }
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                dracula.DiscardHunterCard(this, huntersEncountered[i], ui);
            }
            return true;
        }

        public bool ResolveNewVampire(List<Hunter> huntersEncountered, out bool discard, UserInterface ui)
        {
            if (time < 3)
            {
                ui.TellUser(huntersEncountered[0].name + " encountered a New Vampire and disposed of it during the day");
                discard = true;
                return true;
            }
            else
            {
                if (ui.GetDieRoll() < 4)
                {
                    ui.TellUser("The Vampire attempts to bite you");
                    for (int i = 0; i < huntersEncountered.Count(); i++)
                    {
                        int answer = ui.GetHunterHolyItems(huntersEncountered[i].name);
                        if (answer > 0)
                        {
                            Logger.WriteToDebugLog(huntersEncountered[i].name + " negated the encounter with " + (answer == 1 ? "a crucifix" : "a heavenly host"));
                            Logger.WriteToGameLog(huntersEncountered[i].name + " negated the encounter with " + (answer == 1 ? "a crucifix" : "a heavenly host"));
                            ui.TellUser(huntersEncountered[i].name + " negated the encounter with " + (answer == 1 ? "a crucifix" : "a heavenly host"));
                            discard = true;
                            return true;
                        }
                    }
                    for (int i = 0; i < huntersEncountered.Count(); i++)
                    {
                        Logger.WriteToDebugLog(huntersEncountered[i].name + " is bitten");
                        Logger.WriteToGameLog(huntersEncountered[i].name + " is bitten");
                        ui.TellUser(huntersEncountered[i].name + " is bitten");
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
                        int answer = ui.GetHunterSharpItems(huntersEncountered[i].name);
                        if (answer > 0)
                        {
                            Logger.WriteToDebugLog(huntersEncountered[i].name + " prevented the Vampire escaping with " + (answer == 1 ? "a Knife" : "a Stake"));
                            Logger.WriteToGameLog(huntersEncountered[i].name + " prevented the Vampire escaping with " + (answer == 1 ? "a Knife" : "a Stake"));
                            ui.TellUser(huntersEncountered[i].name + " prevented the Vampire escaping with " + (answer == 1 ? "a Knife" : "a Stake"));
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
            ui.TellUser(huntersEncountered.First().name + " ");
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                ui.TellUser("and " + huntersEncountered[i].name + " ");
            }
            ui.TellUser("encountered Wolves");
            bool hasPistol = false;
            bool hasRifle = false;
            for (int i = 1; i < huntersEncountered.Count(); i++)
            {
                int answer = ui.GetHunterEquipmentForWolves(huntersEncountered[i].name);
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
                    Logger.WriteToDebugLog(huntersEncountered[i].name + " loses " + (numberOfWeaponTypes == 1 ? "1" : "2") + " health");
                    Logger.WriteToGameLog(huntersEncountered[i].name + " loses " + (numberOfWeaponTypes == 1 ? "1" : "2") + " health");
                    ui.TellUser(huntersEncountered[i].name + " loses " + (numberOfWeaponTypes == 1 ? "1" : "2") + " health");
                    huntersEncountered[i].health -= (2 - numberOfWeaponTypes);
                }
                
            }
            return !HandlePossibleHunterDeath(ui);
        }

        internal void DrawEncountersUpToHandSize()
        {
            dracula.DrawEncounters(this, dracula.encounterHandSize);
        }

        internal void DraculaCancelTrain(int hunterIndex, UserInterface ui)
        {
            Logger.WriteToDebugLog("Dracula is deciding whether to cancel the train");

            if (dracula.WillCancelTrain(this, hunters[hunterIndex]))
            {
                PlayFalseTipOff(ui);
            }
        }

        private void PlayFalseTipOff(UserInterface ui)
        {
            ui.TellUser("I am playing my False Tip-Off card to cancel your train");
            dracula.DiscardEventFromHand(this, "False Tip-Off");
        }

        internal void AddEventCardToHunterAtIndex(int hunterIndex)
        {
            hunters[hunterIndex].numberOfEvents++;
            Logger.WriteToDebugLog(hunters[hunterIndex].name + " draw an event card, up to " + hunters[hunterIndex].numberOfEvents);
            Logger.WriteToGameLog(hunters[hunterIndex].name + " draw an event card, up to " + hunters[hunterIndex].numberOfEvents);
        }

        internal void AddItemCardToHunterAtIndex(int hunterIndex)
        {
            hunters[hunterIndex].numberOfItems++;
            Logger.WriteToDebugLog(hunters[hunterIndex].name + " draw an item card, up to " + hunters[hunterIndex].numberOfItems);
            Logger.WriteToGameLog(hunters[hunterIndex].name + " draw an item card, up to " + hunters[hunterIndex].numberOfItems);
        }

        internal int ResolveTracker()
        {
            return resolve;
        }

        internal void DiscardEventFromHunterAtIndex(string eventName, int hunterIndex)
        {
            Event eventToDiscard = GetEventByNameFromEventDeck(eventName);
            eventDeck.Remove(eventToDiscard);
            eventDiscard.Add(eventToDiscard);
            hunters[hunterIndex].numberOfEvents--;
            Logger.WriteToDebugLog(hunters[hunterIndex].name + " discarded " + eventName + " down to " + hunters[hunterIndex].numberOfEvents);
            Logger.WriteToGameLog(hunters[hunterIndex].name + " discarded " + eventName + " down to " + hunters[hunterIndex].numberOfEvents);
        }

        internal int NumberOfEventCardsAtHunterIndex(int hunterIndex)
        {
            return hunters[hunterIndex].numberOfEvents;
        }

        internal int NumberOfItemCardsAtHunterIndex(int hunterIndex)
        {
            return hunters[hunterIndex].numberOfItems;
        }

        internal string HunterShouldDiscardAtHunterIndex(int hunterIndex)
        {
            if (hunterIndex == 2)
            {
                if (hunters[2].numberOfEvents == 4 && hunters[2].numberOfItems == 4)
                {
                    return "item or event";
                }
                else if (hunters[2].numberOfEvents > 4)
                {
                    return "event";
                }
                else if (hunters[2].numberOfItems > 4)
                {
                    return "item";
                }
            }
            else
            {
                if (hunters[hunterIndex].numberOfEvents > 3)
                {
                    return "event";
                }
                if (hunters[hunterIndex].numberOfItems > 3)
                {
                    return "item";
                }
            }
            return "I don't know, bro";
        }

        internal Item GetItemByNameFromItemDeck(string argument2)
        {
            try
            {
                return itemDeck[itemDeck.FindIndex(card => card.name.ToLower().StartsWith(argument2.ToLower()))];
            }
            catch (ArgumentOutOfRangeException)
            {
                return new Item("Unknown item");
            }
        }

        internal void DiscardItemFromHunterAtIndex(string itemName, int hunterIndex)
        {
            Item itemToDiscard = GetItemByNameFromItemDeck(itemName);
            itemDeck.Remove(itemToDiscard);
            itemDiscard.Add(itemToDiscard);
            hunters[hunterIndex].numberOfItems--;
            Logger.WriteToDebugLog(hunters[hunterIndex].name + " discarded " + itemName + " down to " + hunters[hunterIndex].numberOfEvents);
            Logger.WriteToGameLog(hunters[hunterIndex].name + " discarded " + itemName + " down to " + hunters[hunterIndex].numberOfEvents);
        }

        internal string ResolveCombat(int hunterIndex, int enemyInCombat, UserInterface ui)
        {
            List<Item> enemyCombatCards = new List<Item>();
            List<Item> hunterBasicCards = new List<Item>();
            hunterBasicCards.Add(new Item("Punch"));
            hunterBasicCards.Add(new Item("Escape"));
            string enemyName = "nobody";
            switch (enemyInCombat)
            {
                case 1:
                    {
                        enemyName = "Dracula";
                        enemyCombatCards.Add(new Item("Claws"));
                        enemyCombatCards.Add(new Item("Escape (Man)"));
                        if (time > 2)
                        {
                            enemyCombatCards.Add(new Item("Strength"));
                            enemyCombatCards.Add(new Item("Escape (Bat)"));
                            enemyCombatCards.Add(new Item("Escape (Mist)"));
                            enemyCombatCards.Add(new Item("Fangs"));
                            enemyCombatCards.Add(new Item("Mesmerize"));
                        }
                        break;
                    }
                case 2:
                    {
                        enemyName = "Minion with Knife";
                        enemyCombatCards.Add(new Item("Punch"));
                        enemyCombatCards.Add(new Item("Knife"));
                        break;
                    }
                case 3:
                    {
                        enemyName = "Minion with Knife and Pistol";
                        enemyCombatCards.Add(new Item("Punch"));
                        enemyCombatCards.Add(new Item("Knife"));
                        enemyCombatCards.Add(new Item("Pistol"));
                        break;
                    }
                case 4:
                    {
                        enemyName = "Minion with Knife and Rifle";
                        enemyCombatCards.Add(new Item("Punch"));
                        enemyCombatCards.Add(new Item("Knife"));
                        enemyCombatCards.Add(new Item("Rifle"));
                        break;
                    }
                case 5:
                    {
                        enemyName = "Assassin";
                        enemyCombatCards.Add(new Item("Punch"));
                        enemyCombatCards.Add(new Item("Knife"));
                        enemyCombatCards.Add(new Item("Pistol"));
                        enemyCombatCards.Add(new Item("Rifle"));
                        break;
                    }
                case 6:
                    {
                        enemyName = "New Vampire";
                        enemyCombatCards.Add(new Item("Claws"));
                        enemyCombatCards.Add(new Item("Escape (Man)"));
                        if (time > 2)
                        {
                            enemyCombatCards.Add(new Item("Strength"));
                            enemyCombatCards.Add(new Item("Escape (Bat)"));
                            enemyCombatCards.Add(new Item("Escape (Mist)"));
                            enemyCombatCards.Add(new Item("Fangs"));
                            enemyCombatCards.Add(new Item("Mesmerize"));
                        }
                        break;
                    }

            }
            ui.TellUser(hunters[hunterIndex].name + " is entering combat against " + enemyName + (hunters[hunterIndex].huntersInGroup.Count() > 0 ? " with his group" : ""));
            CombatRoundResult roundResult = new CombatRoundResult();
            roundResult = ResolveRoundOfCombat(hunterIndex, enemyCombatCards, hunterBasicCards, roundResult, ui);
            enemyCombatCards.Add(new Item("Dodge"));
            hunterBasicCards.Add(new Item("Dodge"));
            while (roundResult.outcome != "Bite" && roundResult.outcome != "Enemy killed" && roundResult.outcome != "Hunter killed" && roundResult.outcome != "End")
            {
                roundResult = ResolveRoundOfCombat(hunterIndex, enemyCombatCards, hunterBasicCards, roundResult, ui);
            }
            foreach (Hunter h in hunters[hunterIndex].huntersInGroup)
            {
                h.health -= ui.GetHunterHealthLost(h.name);
            }
            if (enemyInCombat == 1)
            {
                dracula.blood -= ui.GetDraculaBloodLost();
            }
            if (roundResult.outcome == "Bite" || roundResult.outcome == "Enemy killed" || roundResult.outcome == "Hunter killed" || roundResult.outcome == "End")
            {
                return roundResult.outcome;
            }
            return ui.GetCombatRoundOutcome();
        }

        private CombatRoundResult ResolveRoundOfCombat(int hunterIndex, List<Item> combatCards, List<Item> hunterBasicCards, CombatRoundResult result, UserInterface ui)
        {
            string targetHunterName;
            string newEnemyCardUsed = dracula.ChooseCombatCardAndTarget(hunters[hunterIndex], combatCards, result, out targetHunterName).name;
            string newHunterCardUsed = "nothing";
            foreach (Hunter h in hunters[hunterIndex].huntersInGroup)
            {
                do
                {
                    newHunterCardUsed = ui.GetCombatCardFromHunter(h.name);
                    if (GetItemByNameFromItemDeck(newHunterCardUsed).name == "Unknown item")
                    {
                        if (GetItemByNameFromList(newHunterCardUsed, hunterBasicCards).name == "Unknown item")
                        {
                            ui.TellUser("I didn't recognise that item name");
                        }
                    }
                } while (GetItemByNameFromItemDeck(newHunterCardUsed).name == "Unknown item" && GetItemByNameFromList(newHunterCardUsed, hunterBasicCards).name == "Unknown item");
                h.lastItemUsedInCombat = newHunterCardUsed;
            }
            ui.TellUser("Enemy chose " + newEnemyCardUsed + " against " + targetHunterName);
            string newOutcome = ui.GetCombatRoundOutcome();
            CombatRoundResult newResult = new CombatRoundResult();
            newResult.enemyCardUsed = newEnemyCardUsed;
            newResult.outcome = newOutcome;
            return newResult;
        }

        private Item GetItemByNameFromList(string itemName, List<Item> itemList)
        {
            try
            {
                return itemList[itemList.FindIndex(card => card.name.ToLower().StartsWith(itemName.ToLower()))];
            }
            catch (ArgumentOutOfRangeException)
            {
                return new Item("Unknown item");
            }
        }

        internal void PerformBatsMoveForHunter(int hunterIndex, UserInterface ui)
        {
            Location locationToMoveTo = dracula.DecideWhereToSendHunterWithBats(this, hunters[hunterIndex], ui);
            MoveHunterToLocationAtHunterIndex(hunterIndex, locationToMoveTo);
            ui.TellUser(hunters[hunterIndex].name + (hunters[hunterIndex].huntersInGroup.Count() > 0 ? " and group" : "") + " moved to " + locationToMoveTo.name + " by Bats");
        }

        internal void TradeBetweenHunters(int hunterIndexA, int hunterIndexB, UserInterface ui)
        {
            int hunterAGiven = ui.GetNumberOfCardsGivenByHunter(hunters[hunterIndexA].name);
            int hunterBGiven = ui.GetNumberOfCardsGivenByHunter(hunters[hunterIndexB].name);
            hunters[hunterIndexA].numberOfItems = hunters[hunterIndexA].numberOfItems + hunterBGiven - hunterAGiven;
            hunters[hunterIndexB].numberOfItems = hunters[hunterIndexB].numberOfItems + hunterAGiven - hunterBGiven;
            ui.TellUser(hunters[hunterIndexA].name + " now has " + hunters[hunterIndexA].numberOfItems + " items and " + hunters[hunterIndexB].name + " now has " + hunters[hunterIndexB].numberOfItems + " items");
        }

        internal void UseItemByHunterAtHunterIndex(string itemName, int hunterIndex, UserInterface ui)
        {
            switch (itemName)
            {
                case "Local Rumors": PlayLocalRumors(hunterIndex, ui); break;
                case "Dogs": hunters[hunterIndex].hasDogsFaceUp = true; break;
                case "Fast Horse": DiscardItemFromHunterAtIndex("Fast Horse", hunterIndex); break;
                case "Heavenly Host": PlayHeavenlyHost(hunterIndex, ui); break;
                case "Holy Water": PlayHolyWater(hunterIndex, ui); break;
                default: break;
            }
        }

        private void PlayHolyWater(int hunterIndex, UserInterface ui)
        {
            List<Hunter> huntersWithBitesAtThisLocation = new List<Hunter>();
            for (int i = 0; i < 3; i++)
            {
                if (hunters[i].currentLocation == hunters[hunterIndex].currentLocation && hunters[i].numberOfBites > 0)
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
                        if (huntersWithBitesAtThisLocation.Contains(hunters[i]))
                        {
                            if (ui.GetHunterHeal(hunters[hunterIndex].name))
                            {
                                hunterToHeal = hunters[i];
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
                case 1: hunterToHeal.health -= 2; break;
                case 2: break;
                case 3: hunterToHeal.numberOfBites--; break;
            }
        }

        private void PlayHeavenlyHost(int hunterIndex, UserInterface ui)
        {
            hunters[hunterIndex].currentLocation.hasHost = true;
        }

        private void PlayLocalRumors(int hunterIndex, UserInterface ui)
        {
            int locationIndex = 0;
            do
            {
                ui.GetLocationIndexOfEncounterToReveal();
            }
            while (dracula.trail[locationIndex] == null && (locationIndex > 5 ? dracula.catacombs[locationIndex - 6] == null : true));
            Location locationWhereEncounterIsBeingRevealed;
            if (locationIndex < 6)
            {
                locationWhereEncounterIsBeingRevealed = dracula.trail[locationIndex];
            }
            else
            {
                locationWhereEncounterIsBeingRevealed = dracula.catacombs[locationIndex - 6];
            }
            int encounterToReveal = 0;
            if (locationWhereEncounterIsBeingRevealed.encounters.Count() > 0)
            {
                encounterToReveal = ui.GetIndexOfEncounterToReveal();
            }
            locationWhereEncounterIsBeingRevealed.encounters[encounterToReveal].isRevealed = true;
            DiscardItemFromHunterAtIndex("Local Rumors", hunterIndex);
        }

        internal bool HunterHasGroupAtHunterIndex(int hunterIndex)
        {
            return hunters[hunterIndex].huntersInGroup.Count() > 1;
        }

        internal void ApplyBiteToHunter(int hunterIndex, UserInterface ui)
        {
            hunters[hunterIndex].numberOfBites++;
            ui.TellUser(hunters[hunterIndex].name + " was bitten");
        }

        internal void ApplyBiteToOneOfMultipleHunters(int hunterIndex, UserInterface ui)
        {
            int hunterIndexBitten;
            do
            {
                hunterIndexBitten = ui.GetIndexOfHunterBitten();
                if (hunters[hunterIndexBitten].currentLocation != hunters[hunterIndex].currentLocation)
                {
                    ui.TellUser(hunters[hunterIndexBitten].name + " is not at the same location as " + hunters[hunterIndex].name);
                }
            } while (hunters[hunterIndexBitten].currentLocation != hunters[hunterIndex].currentLocation);
            ApplyBiteToHunter(hunterIndexBitten, ui);
        }

        internal string LocationOfHunterAtHunterIndex(int hunterIndex)
        {
            return hunters[hunterIndex].currentLocation.name;
        }

        internal int NumberOfHuntersAtLocation(string locationName)
        {
            Location location = GetLocationFromName(locationName);
            int total = 0;
            foreach (Hunter h in hunters)
            {
                if (h.currentLocation == location)
                {
                    total++;
                }
            }
            return total;
        }

        internal void HandleDraculaEscape(UserInterface ui)
        {
            if (ui.GetDidDraculaEscape())
            {
                if (ui.GetDraculaEscapeForm() == 3)
                {
                    dracula.DoEscapeAsBatsMove(this, ui);
                }
            }
        }

        internal bool HandlePossibleHunterDeath(UserInterface ui)
        {
            bool hunterDied = false;
            foreach (Hunter h in hunters)
            {
                if (h.health < 1)
                {
                    hunterDied = true;
                    ui.TellUser(h.name + " is defeated");
                    h.currentLocation = GetLocationFromName("St. Joseph & St. Mary");
                    int hunterIndex = 0;
                    switch (h.name)
                    {
                        case "Lord Godalming":
                            h.health = 12;
                            h.numberOfBites = 0;
                            hunterIndex = 0;
                            break;
                        case "Van Helsing":
                            h.health = 8;
                            h.numberOfBites = 0;
                            hunterIndex = 1;
                            break;
                        case "Dr. Seward":
                            h.health = 10;
                            h.numberOfBites = 0;
                            hunterIndex = 2;
                            break;
                        case "Mina Harker":
                            h.health = 8;
                            h.numberOfBites = 1;
                            hunterIndex = 3;
                            break;
                    }
                    while (h.numberOfEvents > 0)
                    {
                        string eventName = "Unknown event";
                        while (eventName == "Unknown event")
                        {
                            eventName = GetEventByNameFromEventDeck(ui.GetNameOfEventDiscardedByHunter(h.name)).name;
                            if (eventName == "Unknown event")
                            {
                                ui.TellUser("I can't find that event");
                            }
                        }
                        DiscardEventFromHunterAtIndex(eventName, hunterIndex);                        
                    }
                    while (h.numberOfItems > 0)
                    {
                        string itemName = "Unknown item";
                        while (itemName == "Unknown item")
                        {
                            itemName = GetItemByNameFromItemDeck(ui.GetNameOfItemDiscardedByHunter(h.name)).name;
                            if (itemName == "Unknown item")
                            {
                                ui.TellUser("I can't find that item");
                            }
                        }
                        DiscardItemFromHunterAtIndex(itemName, hunterIndex);
                    }
                }
            }
            return hunterDied;
        }
    }
}
