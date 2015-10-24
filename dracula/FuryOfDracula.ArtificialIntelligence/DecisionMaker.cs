﻿using FuryOfDracula.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.ArtificialIntelligence
{
    public class DecisionMaker
    {
        public Location ChooseDestinationAndPower(GameState game, out Power power)
        {
            Location destination;
            List<Power> possiblePowers = game.Dracula.DraculaCardDeck.GetAvailablePowers(game.TimeOfDay);
            List<Location> possibleDestinations = game.Map.LocationsConnectedByRoadOrSeaTo(game.Dracula.CurrentLocation);
            List<Location> possibleDoubleBackDestinations = new List<Location>();
            List<Location> possibleWolfFormDestinations = game.Map.LocationsConnectedByRoadTo(game.Dracula.CurrentLocation);
            List<Location> tempWolfFormExtensionsList = new List<Location>();
            foreach (Location loc in possibleWolfFormDestinations)
            {
                foreach (Location ext in game.Map.LocationsConnectedByRoadTo(loc))
                {
                    if (!possibleWolfFormDestinations.Contains(ext) && !tempWolfFormExtensionsList.Contains(ext))
                    {
                        tempWolfFormExtensionsList.Add(ext);
                    }
                }
            }
            possibleWolfFormDestinations.AddRange(tempWolfFormExtensionsList);
            for (int i = 0; i < 6; i++)
            {
                if (game.Dracula.Trail[i] != null)
                {
                    possibleDestinations.Remove(game.Dracula.Trail[i].DraculaCards[0].Location);
                    if (game.Dracula.Trail[i].DraculaCards[0].Location != Location.Nowhere)
                    {
                        possibleDoubleBackDestinations.Add(game.Dracula.Trail[i].DraculaCards[0].Location);
                    }
                    possibleWolfFormDestinations.Remove(game.Dracula.Trail[i].DraculaCards[0].Location);
                    possiblePowers.Remove(game.Dracula.Trail[i].DraculaCards[0].Power);
                    if (game.Dracula.Trail[i].DraculaCards[1] != null)
                    {
                        possibleDestinations.Remove(game.Dracula.Trail[i].DraculaCards[1].Location);
                        possibleWolfFormDestinations.Remove(game.Dracula.Trail[i].DraculaCards[1].Location);
                        possiblePowers.Remove(game.Dracula.Trail[i].DraculaCards[1].Power);
                    }
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null)
                {
                    possibleDestinations.Remove(game.Dracula.Catacombs[i].DraculaCards[0].Location);
                    if (game.Dracula.Catacombs[i].DraculaCards[0].Location != Location.Nowhere)
                    {
                        possibleDoubleBackDestinations.Add(game.Dracula.Catacombs[i].DraculaCards[0].Location);
                    }
                    possibleWolfFormDestinations.Remove(game.Dracula.Catacombs[i].DraculaCards[0].Location);
                    possiblePowers.Remove(game.Dracula.Catacombs[i].DraculaCards[0].Power);
                    if (game.Dracula.Catacombs[i].DraculaCards[1] != null)
                    {
                        possibleDestinations.Remove(game.Dracula.Catacombs[i].DraculaCards[1].Location);
                        possibleWolfFormDestinations.Remove(game.Dracula.Catacombs[i].DraculaCards[1].Location);
                        possiblePowers.Remove(game.Dracula.Catacombs[i].DraculaCards[1].Power);
                    }
                }
            }
            possibleDoubleBackDestinations.Remove(game.Dracula.CurrentLocation);
            if (possibleDoubleBackDestinations.Count == 0)
            {
                possiblePowers.Remove(Power.DoubleBack);
            }
            if (possibleWolfFormDestinations.Count() == 0)
            {
                possiblePowers.Remove(Power.WolfForm);
            }
            int randomNumber = new Random().Next(0, possibleDestinations.Count() + possiblePowers.Count());
            if (randomNumber < possibleDestinations.Count())
            {
                do
                {
                    destination = possibleDestinations[new Random().Next(0, possibleDestinations.Count())];
                } while (game.Map.TypeOfLocation(destination) == LocationType.Hospital);
                power = Power.None;
                return destination;
            }
            else
            {
                power = possiblePowers[new Random().Next(0, possiblePowers.Count())];
                switch (power)
                {
                    case Power.DarkCall:
                    case Power.Feed:
                    case Power.Hide: return Location.Nowhere;
                    case Power.WolfForm: do
                        {
                            destination = possibleWolfFormDestinations[new Random().Next(0, possibleWolfFormDestinations.Count())];
                        } while (game.Map.TypeOfLocation(destination) == LocationType.Hospital);
                        return destination;
                    case Power.DoubleBack: do
                        {
                            destination = possibleDoubleBackDestinations[new Random().Next(0, possibleDoubleBackDestinations.Count())];
                        } while (game.Map.TypeOfLocation(destination) == LocationType.Hospital);
                        return destination;
                }
                do
                {
                    destination = possibleDestinations[new Random().Next(0, possibleDestinations.Count())];
                } while (game.Map.TypeOfLocation(destination) == LocationType.Hospital);
                return destination;
            }
        }

        public Encounter ChooseEncounterToResolveOnSearchingHunter(GameState game, List<Encounter> encountersToResolve)
        {
            return encountersToResolve[new Random().Next(0, encountersToResolve.Count())];
        }

        public List<int> ChooseWhichCatacombsCardsToDiscard(GameState game)
        {
            List<int> cardsToDiscard = new List<int>();
            for (int i = 0; i < 3; i++) {
                if (game.Dracula.Catacombs[i] != null && new Random().Next(0, 6) == 0)
                {
                    cardsToDiscard.Add(i);
                }
            }
            return cardsToDiscard;
        }

        public Encounter ChooseEncounterToDiscardFromDoubleBackedCatacomnbsLocation(GameState game)
        {
            if (game.Dracula.Trail[0].Encounters[0] != Encounter.None)
            {
                if (game.Dracula.Trail[0].Encounters[1] != Encounter.None)
                {
                    if (new Random().Next(0, 2) == 0)
                    {
                        return game.Dracula.Trail[0].Encounters[1];
                    }
                }
                return game.Dracula.Trail[0].Encounters[0];
            } else if (game.Dracula.Trail[0].Encounters[1] != Encounter.None)
            {
                return game.Dracula.Trail[0].Encounters[1];
            }
            return Encounter.None;
        }

        public EventCard ChooseEventToDiscard(GameState game)
        {
            return game.Dracula.EventHand[new Random().Next(0, game.Dracula.EventHand.Count())];
        }

        public Location ChooseStartLocation(GameState game)
        {
            List<Location> allLocations = game.Map.GetAllLocations();
            Location startLocation;
            do
            {
                startLocation = allLocations[new Random().Next(0, allLocations.Count())];
            } while (game.Map.TypeOfLocation(startLocation) != LocationType.SmallCity && game.Map.TypeOfLocation(startLocation) != LocationType.LargeCity);
            return startLocation;
        }

        public int PutDroppedOffCardInCatacombs(GameState game, DraculaCardSlot cardDroppedOffTrail)
        {
            if (cardDroppedOffTrail.DraculaCards[0].Location != Location.Nowhere && game.Map.TypeOfLocation(cardDroppedOffTrail.DraculaCards[0].Location) != LocationType.Sea && game.Map.TypeOfLocation(cardDroppedOffTrail.DraculaCards[0].Location) != LocationType.Castle && new Random().Next(0, 5) == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (game.Dracula.Catacombs[i] == null)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public Encounter ChooseEncounterToPlaceOnDraculaCardSlot(GameState game, DraculaCardSlot slot)
        {
            if (game.Map.TypeOfLocation(slot.DraculaCards[0].Location) != LocationType.SmallCity && game.Map.TypeOfLocation(slot.DraculaCards[0].Location) != LocationType.LargeCity && (slot.DraculaCards[0].Power != Power.Hide || game.Dracula.CurrentLocation == Location.CastleDracula))
            {
                return Encounter.None;
            }
            return game.Dracula.EncounterHand[new Random().Next(0, game.Dracula.EncounterHand.Count())];
        }
    }
}