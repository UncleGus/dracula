using FuryOfDracula.GameLogic;
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
            List<Power> possiblePowers = Enumerations.GetAvailablePowers(game.TimeOfDay);
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
                    foreach (DraculaCard card in game.Dracula.Trail[i].DraculaCards)
                    {
                        possibleDestinations.Remove(card.Location);
                        if (card.Location != Location.Nowhere)
                        {
                            possibleDoubleBackDestinations.Add(card.Location);
                        }
                        possibleWolfFormDestinations.Remove(card.Location);
                        possiblePowers.Remove(card.Power);
                    }
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null)
                {
                    foreach (DraculaCard card in game.Dracula.Catacombs[i].DraculaCards)
                    {
                        possibleDestinations.Remove(card.Location);
                        if (card.Location != Location.Nowhere)
                        {
                            possibleDoubleBackDestinations.Add(card.Location);
                        }
                        possibleWolfFormDestinations.Remove(card.Location);
                        possiblePowers.Remove(card.Power);
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
                    case Power.WolfForm:
                        do
                        {
                            destination = possibleWolfFormDestinations[new Random().Next(0, possibleWolfFormDestinations.Count())];
                        } while (game.Map.TypeOfLocation(destination) == LocationType.Hospital);
                        return destination;
                    case Power.DoubleBack:
                        do
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

        public EncounterTile ChooseEncounterToResolveOnSearchingHunter(GameState game, List<EncounterTile> encounterTilesToResolve)
        {
            return encounterTilesToResolve[new Random().Next(0, encounterTilesToResolve.Count())];
        }

        public List<int> ChooseWhichCatacombsCardsToDiscard(GameState game)
        {
            List<int> cardsToDiscard = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null && new Random().Next(0, 6) == 0)
                {
                    cardsToDiscard.Add(i);
                }
            }
            return cardsToDiscard;
        }

        public EncounterTile ChooseEncounterTileToDiscardFromDoubleBackedCatacomnbsLocation(GameState game)
        {
            if (game.Dracula.Trail[0].EncounterTiles.Count() > 1)
            {
                if (new Random().Next(0, 2) == 0)
                {
                    return game.Dracula.Trail[0].EncounterTiles.First();
                } else
                {
                    return game.Dracula.Trail[0].EncounterTiles[1];
                }
            }
            else
            {
                return game.Dracula.Trail[0].EncounterTiles.FirstOrDefault();
            }
        }

        public EventCard ChooseEventToDiscard(GameState game)
        {
            return game.Dracula.EventHand[new Random().Next(0, game.Dracula.EventHand.Count())];
        }

        public Location ChooseStartLocation(GameState game)
        {
            List<Location> allLocations = Enumerations.GetAllLocations();
            Location startLocation;
            do
            {
                startLocation = allLocations[new Random().Next(0, allLocations.Count())];
            } while (game.Map.TypeOfLocation(startLocation) != LocationType.SmallCity && game.Map.TypeOfLocation(startLocation) != LocationType.LargeCity);
            return startLocation;
        }

        public int PutDroppedOffCardInCatacombs(GameState game, DraculaCardSlot cardDroppedOffTrail)
        {
            if (cardDroppedOffTrail.DraculaCards.First().Location != Location.Nowhere && game.Map.TypeOfLocation(cardDroppedOffTrail.DraculaCards.First().Location) != LocationType.Sea && game.Map.TypeOfLocation(cardDroppedOffTrail.DraculaCards.First().Location) != LocationType.Castle && new Random().Next(0, 5) == 0)
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

        public EncounterTile ChooseEncounterTileToPlaceOnDraculaCardSlot(GameState game, DraculaCardSlot slot)
        {
            if (game.Map.TypeOfLocation(slot.DraculaCards.First().Location) != LocationType.SmallCity && game.Map.TypeOfLocation(slot.DraculaCards.First().Location) != LocationType.LargeCity && (slot.DraculaCards.First().Power != Power.Hide || game.Dracula.CurrentLocation == Location.CastleDracula))
            {
                return null;
            }
            return game.Dracula.EncounterHand[new Random().Next(0, game.Dracula.EncounterHand.Count())];
        }

        public EnemyCombatCard ChooseCombatCardAndTarget(List<HunterPlayer> huntersInvolved, List<EnemyCombatCard> enemyCombatCards, bool firstRound, out Hunter enemyTarget, EnemyCombatCard cardUsedLastRound, bool repelled)
        {
            EnemyCombatCard cardChosen = EnemyCombatCard.None;
            do
            {
                cardChosen = enemyCombatCards[new Random().Next(0, enemyCombatCards.Count())];
            } while ((cardChosen == EnemyCombatCard.Dodge && firstRound) || cardChosen == cardUsedLastRound || (repelled && (cardChosen == EnemyCombatCard.Fangs || cardChosen == EnemyCombatCard.Mesmerize || cardChosen == EnemyCombatCard.Strength)));
            enemyTarget = huntersInvolved[new Random().Next(0, huntersInvolved.Count())].Hunter;
            return cardChosen;
        }
    }
}
