using System;
using System.Collections.Generic;
using System.Linq;
using FuryOfDracula.GameLogic;

namespace FuryOfDracula.ArtificialIntelligence
{
    public class DecisionMaker
    {
        public Location ChooseDestinationAndPower(GameState game, out Power power)
        {
            Location destination;
            if (game.Dracula.AdvanceMoveLocation != Location.Nowhere || game.Dracula.AdvanceMovePower != Power.None)
            {
                power = game.Dracula.AdvanceMovePower;
                destination = game.Dracula.AdvanceMoveLocation;
                game.Dracula.AdvanceMoveLocation = Location.Nowhere;
                game.Dracula.AdvanceMovePower = Power.None;
                return destination;
            }
            var possiblePowers = new List<Power>();
            if (game.Map.TypeOfLocation(game.Dracula.CurrentLocation) != LocationType.Sea)
            {
                possiblePowers = Enumerations.GetAvailablePowers(game.TimeOfDay);
            }
            var possibleDestinations = game.Map.LocationsConnectedByRoadOrSeaTo(game.Dracula.CurrentLocation);
            var possibleDoubleBackDestinations = new List<Location>();
            var possibleWolfFormDestinations = game.Map.LocationsConnectedByRoadTo(game.Dracula.CurrentLocation);
            var tempWolfFormExtensionsList = new List<Location>();
            foreach (var loc in possibleWolfFormDestinations)
            {
                foreach (var ext in game.Map.LocationsConnectedByRoadTo(loc))
                {
                    if (!possibleWolfFormDestinations.Contains(ext) && !tempWolfFormExtensionsList.Contains(ext))
                    {
                        tempWolfFormExtensionsList.Add(ext);
                    }
                }
            }
            possibleWolfFormDestinations.AddRange(tempWolfFormExtensionsList);
            for (var i = 0; i < 6; i++)
            {
                if (game.Dracula.Trail[i] != null)
                {
                    foreach (var card in game.Dracula.Trail[i].DraculaCards)
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
            for (var i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null)
                {
                    foreach (var card in game.Dracula.Catacombs[i].DraculaCards)
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
            var locationsWithHeavenlyHostOrConsecratedGround = game.GetBlockedLocations();
            foreach (var loc in locationsWithHeavenlyHostOrConsecratedGround)
            {
                possibleDestinations.Remove(loc);
                possibleWolfFormDestinations.Remove(loc);
                possibleDoubleBackDestinations.Remove(loc);
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
            var totalPossibleMoves = possibleDestinations.Count() + possiblePowers.Count();
            if (totalPossibleMoves == 0)
            {
                power = Power.None;
                return Location.Nowhere;
            }

            var randomNumber = new Random().Next(0, totalPossibleMoves);
            if (randomNumber < possibleDestinations.Count())
            {
                do
                {
                    destination = possibleDestinations[new Random().Next(0, possibleDestinations.Count())];
                } while (game.Map.TypeOfLocation(destination) == LocationType.Hospital);
                power = Power.None;
                return destination;
            }
            power = possiblePowers[new Random().Next(0, possiblePowers.Count())];
            switch (power)
            {
                case Power.DarkCall:
                case Power.Feed:
                case Power.Hide:
                    return Location.Nowhere;
                case Power.WolfForm:
                    do
                    {
                        destination =
                            possibleWolfFormDestinations[new Random().Next(0, possibleWolfFormDestinations.Count())];
                    } while (game.Map.TypeOfLocation(destination) == LocationType.Hospital);
                    return destination;
                case Power.DoubleBack:
                    do
                    {
                        destination =
                            possibleDoubleBackDestinations[new Random().Next(0, possibleDoubleBackDestinations.Count())];
                    } while (game.Map.TypeOfLocation(destination) == LocationType.Hospital);
                    return destination;
            }
            do
            {
                destination = possibleDestinations[new Random().Next(0, possibleDestinations.Count())];
            } while (game.Map.TypeOfLocation(destination) == LocationType.Hospital);
            return destination;
        }

        public bool ChooseToDelayHunterWithFalseTipoff(GameState game)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.FalseTipoff) != null &&
                new Random().Next(0, 2) == 0)
            {
                return true;
            }
            return false;
        }

        public EncounterTile ChooseEncounterToResolveOnSearchingHunter(GameState game,
            List<EncounterTile> encounterTilesToResolve)
        {
            return encounterTilesToResolve[new Random().Next(0, encounterTilesToResolve.Count())];
        }

        public List<int> ChooseWhichCatacombsCardsToDiscard(GameState game)
        {
            var cardsToDiscard = new List<int>();
            for (var i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null && new Random().Next(0, 6) == 0)
                {
                    cardsToDiscard.Add(i);
                }
            }
            return cardsToDiscard;
        }

        public EncounterTile ChooseEncounterTileToDiscardFromDoubleBackedCatacombsLocation(GameState game)
        {
            if (game.Dracula.Trail[0].EncounterTiles.Count() < 2)
            {
                return game.Dracula.Trail[0].EncounterTiles.FirstOrDefault();
            }
            return new Random().Next(0, 2) == 0
                ? game.Dracula.Trail[0].EncounterTiles.First()
                : game.Dracula.Trail[0].EncounterTiles[1];
        }

        public bool ChooseToCancelEventWithDevilishPower(GameState game, Event eventBeingPlayedNow,
            Event eventInitiallyPlayed)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.DevilishPower) != null &&
                new Random().Next(0, 4) == 0)
            {
                return true;
            }
            return false;
        }

        public Event ChooseEventToDiscard(GameState game)
        {
            return game.Dracula.EventHand[new Random().Next(0, game.Dracula.EventHand.Count())].Event;
        }

        public Location ChooseStartLocation(GameState game)
        {
            var allLocations = Enumerations.GetAllLocations();
            Location startLocation;
            do
            {
                startLocation = allLocations[new Random().Next(0, allLocations.Count())];
            } while ((game.Map.TypeOfLocation(startLocation) != LocationType.SmallCity &&
                     game.Map.TypeOfLocation(startLocation) != LocationType.LargeCity) ||
                game.Hunters[(int)Hunter.LordGodalming].CurrentLocation == startLocation ||
            game.Hunters[(int)Hunter.DrSeward].CurrentLocation == startLocation ||
            game.Hunters[(int)Hunter.VanHelsing].CurrentLocation == startLocation ||
            game.Hunters[(int)Hunter.MinaHarker].CurrentLocation == startLocation);
            return startLocation;
        }

        public int ChooseToPutDroppedOffCardInCatacombs(GameState game, DraculaCardSlot cardDroppedOffTrail)
        {
            if (cardDroppedOffTrail.DraculaCards.First().Location != Location.Nowhere &&
                game.Map.TypeOfLocation(cardDroppedOffTrail.DraculaCards.First().Location) != LocationType.Sea &&
                game.Map.TypeOfLocation(cardDroppedOffTrail.DraculaCards.First().Location) != LocationType.Castle &&
                new Random().Next(0, 5) == 0)
            {
                for (var i = 0; i < 3; i++)
                {
                    if (game.Dracula.Catacombs[i] == null)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public Location ChooseBatsDestination(GameState game, Hunter hunterWithEncounter)
        {
            List<Location> possibleDestinations = game.Map.LocationsConnectedByRoadTo(game.Hunters[(int)hunterWithEncounter].CurrentLocation);
            return possibleDestinations[new Random().Next(0, possibleDestinations.Count())];
        }

        public EncounterTile ChooseEncounterTileToPlaceOnDraculaCardSlot(GameState game, DraculaCardSlot slot)
        {
            if (game.Map.TypeOfLocation(slot.DraculaCards.First().Location) != LocationType.SmallCity &&
                game.Map.TypeOfLocation(slot.DraculaCards.First().Location) != LocationType.LargeCity &&
                (slot.DraculaCards.First().Power != Power.Hide || game.Dracula.CurrentLocation == Location.CastleDracula))
            {
                return null;
            }
            return game.Dracula.EncounterHand[new Random().Next(0, game.Dracula.EncounterHand.Count())];
        }

        public EnemyCombatCard ChooseCombatCardAndTarget(GameState game, List<HunterPlayer> huntersInvolved,
            List<EnemyCombatCard> enemyCombatCards, bool firstRound, out Hunter enemyTarget,
            EnemyCombatCard cardUsedLastRound, bool repelled, bool sisterAgathaInEffect, int roundsWithoutEscape)
        {
            if (sisterAgathaInEffect && game.Dracula.Blood < 3)
            {
                enemyCombatCards.Remove(EnemyCombatCard.EscapeBat);
                enemyCombatCards.Remove(EnemyCombatCard.EscapeMan);
                enemyCombatCards.Remove(EnemyCombatCard.EscapeMist);
                enemyCombatCards.Remove(EnemyCombatCard.Fangs);
            }
            EnemyCombatCard cardChosen;
            do
            {
                cardChosen = enemyCombatCards[new Random().Next(0, enemyCombatCards.Count())];
            } while ((cardChosen == EnemyCombatCard.Dodge && firstRound) || cardChosen == cardUsedLastRound ||
                     (repelled &&
                      (cardChosen == EnemyCombatCard.Fangs || cardChosen == EnemyCombatCard.Mesmerize ||
                       cardChosen == EnemyCombatCard.Strength)) ||
                     (roundsWithoutEscape > 0) &&
                     (cardChosen == EnemyCombatCard.EscapeBat || cardChosen == EnemyCombatCard.EscapeMan ||
                      cardChosen == EnemyCombatCard.EscapeMist));
            enemyTarget = huntersInvolved[new Random().Next(0, huntersInvolved.Count())].Hunter;
            return cardChosen;
        }

        public bool ChooseToCancelCharteredCarriageWithFalseTipoff(GameState game)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.FalseTipoff) != null &&
                new Random().Next(0, 2) == 0)
            {
                return true;
            }
            return false;
        }

        public Location ChooseEscapeAsBatDestination(GameState game)
        {
            var possibleDestinations = game.Map.LocationsConnectedByRoadTo(game.Dracula.CurrentLocation);
            var tempExtensionsList = new List<Location>();
            foreach (var loc in possibleDestinations)
            {
                foreach (var ext in game.Map.LocationsConnectedByRoadTo(loc))
                {
                    if (!possibleDestinations.Contains(ext) && !tempExtensionsList.Contains(ext))
                    {
                        tempExtensionsList.Add(ext);
                    }
                }
            }
            possibleDestinations.AddRange(tempExtensionsList);
            for (var i = 0; i < 6; i++)
            {
                if (game.Dracula.Trail[i] != null)
                {
                    foreach (var card in game.Dracula.Trail[i].DraculaCards)
                    {
                        possibleDestinations.Remove(card.Location);
                    }
                }
            }
            for (var i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null)
                {
                    foreach (var card in game.Dracula.Catacombs[i].DraculaCards)
                    {
                        possibleDestinations.Remove(card.Location);
                    }
                }
            }
            var hunterLocations = new List<Location>();
            foreach (var loc in possibleDestinations)
            {
                for (var i = 1; i < 5; i++)
                    if (game.Hunters[i].CurrentLocation == loc)
                    {
                        hunterLocations.Add(loc);
                    }
            }
            foreach (var loc in hunterLocations)
            {
                possibleDestinations.Remove(loc);
            }
            return possibleDestinations[new Random().Next(0, possibleDestinations.Count())];
        }

        public Location ChoosePortToGoToAfterStormySeas(GameState game)
        {
            var validPorts = game.Map.GetPortsAdjacentTo(game.Dracula.CurrentLocation);
            if (validPorts.Count() == 0)
            {
                return Location.Nowhere;
            }
            return validPorts[new Random().Next(0, validPorts.Count())];
        }

        public CardType ChooseToDiscardItemFromHunterInsteadOfEvent(HunterPlayer hunterDiscardingCard)
        {
            if (hunterDiscardingCard.ItemCount > 0)
            {
                if (hunterDiscardingCard.EventCount > 0)
                {
                    if (new Random().Next(0, 2) == 0)
                    {
                        return CardType.Item;
                    }
                    return CardType.Event;
                }
                return CardType.Item;
            }
            if (hunterDiscardingCard.EventCount > 0)
            {
                return CardType.Event;
            }
            return CardType.None;
        }

        public EncounterTile ChooseEncounterTileToDiscardFromEncounterHand(GameState game)
        {
            return game.Dracula.EncounterHand[new Random().Next(0, game.Dracula.EncounterHand.Count())];
        }

        public Hunter ChooseHunterToAmbush(GameState game)
        {
            var validHunters = new List<Hunter>();
            foreach (var h in game.Hunters)
            {
                if (game.Map.TypeOfLocation(h.CurrentLocation) != LocationType.Hospital &&
                    game.Map.TypeOfLocation(h.CurrentLocation) != LocationType.Sea)
                {
                    validHunters.Add(h.Hunter);
                }
            }
            if (validHunters.Count() == 0)
            {
                return Hunter.Nobody;
            }
            return validHunters[new Random().Next(0, validHunters.Count())];
        }

        public EncounterTile ChooseEncounterTileToAmbushHunterWith(GameState game, Hunter hunterToAmbush)
        {
            return game.Dracula.EncounterHand[new Random().Next(0, game.Dracula.EncounterHand.Count())];
        }

        public Event ChooseEventCardToPlayAtStartOfDraculaTurn(GameState game,
            out DevilishPowerTarget devilishPowerTarget, out Location roadBlock1, out Location roadBlock2,
            out ConnectionType roadBlockType)
        {
            var eventsThatCanBePlayed = new List<Event>();
            if (game.Dracula.EventHand.Find(card => card.Event == Event.Roadblock) != null)
            {
                eventsThatCanBePlayed.Add(Event.Roadblock);
            }
            if (game.Dracula.EventHand.Find(card => card.Event == Event.TimeRunsShort) != null &&
                game.TimeOfDay != TimeOfDay.SmallHours)
            {
                eventsThatCanBePlayed.Add(Event.TimeRunsShort);
            }
            if (game.Dracula.EventHand.Find(card => card.Event == Event.UnearthlySwiftness) != null)
            {
                eventsThatCanBePlayed.Add(Event.UnearthlySwiftness);
            }
            if (game.Dracula.EventHand.Find(card => card.Event == Event.DevilishPower) != null &&
                (game.HunterAlly != null || game.HeavenlyHostLocation1 != Location.Nowhere ||
                 game.HeavenlyHostLocation2 != Location.Nowhere))
            {
                eventsThatCanBePlayed.Add(Event.DevilishPower);
            }
            if (eventsThatCanBePlayed.Any() && new Random().Next(0, 2) == 0)
            {
                var eventChosen = eventsThatCanBePlayed[new Random().Next(0, eventsThatCanBePlayed.Count())];
                switch (eventChosen)
                {
                    case Event.Roadblock:
                        roadBlock1 = ChooseSetupForRoadBlock(game, out roadBlock2, out roadBlockType);
                        devilishPowerTarget = DevilishPowerTarget.None;
                        break;
                    case Event.DevilishPower:
                        devilishPowerTarget = ChooseDevilishPowerTarget(game);
                        roadBlock1 = roadBlock2 = Location.Nowhere;
                        roadBlockType = ConnectionType.None;
                        break;
                    default:
                        devilishPowerTarget = DevilishPowerTarget.None;
                        roadBlock1 = roadBlock2 = Location.Nowhere;
                        roadBlockType = ConnectionType.None;
                        break;
                }
                return eventChosen;
            }
            devilishPowerTarget = DevilishPowerTarget.None;
            roadBlock1 = roadBlock2 = Location.Nowhere;
            roadBlockType = ConnectionType.None;
            return Event.None;
        }

        private DevilishPowerTarget ChooseDevilishPowerTarget(GameState game)
        {
            var possibleTargets = new List<DevilishPowerTarget>();
            if (game.HeavenlyHostLocation1 != Location.Nowhere)
            {
                possibleTargets.Add(DevilishPowerTarget.HeavenlyHost1);
            }
            if (game.HeavenlyHostLocation2 != Location.Nowhere)
            {
                possibleTargets.Add(DevilishPowerTarget.HeavenlyHost2);
            }
            if (game.HunterAlly != null)
            {
                possibleTargets.Add(DevilishPowerTarget.HunterAlly);
            }
            return possibleTargets[new Random().Next(0, possibleTargets.Count())];
        }

        private Location ChooseSetupForRoadBlock(GameState game, out Location roadBlock2,
            out ConnectionType roadBlockType)
        {
            var allLocations = Enumerations.GetAllLocations();
            var allCities = new List<Location>();
            foreach (var loc in allLocations)
            {
                if (game.Map.TypeOfLocation(loc) == LocationType.SmallCity ||
                    game.Map.TypeOfLocation(loc) == LocationType.LargeCity)
                {
                    allCities.Add(loc);
                }
            }
            var roadBlock1 = allCities[new Random().Next(0, allCities.Count())];
            var citiesConnectedToRoadBlock1ByRoad = game.Map.LocationsConnectedByRoadTo(roadBlock1);
            var citiesConnectedToRoadBlock1ByTrain = game.Map.LocationsConnectedByTrainTo(roadBlock1);
            var citiesConnectedToRoadBlock1 = new List<Location>();
            citiesConnectedToRoadBlock1.AddRange(citiesConnectedToRoadBlock1ByRoad);
            citiesConnectedToRoadBlock1.AddRange(citiesConnectedToRoadBlock1ByTrain);
            roadBlock2 = citiesConnectedToRoadBlock1[new Random().Next(0, citiesConnectedToRoadBlock1.Count())];
            if (citiesConnectedToRoadBlock1ByRoad.Contains(roadBlock2))
            {
                if (citiesConnectedToRoadBlock1ByTrain.Contains(roadBlock2))
                {
                    if (new Random().Next(0, 2) == 0)
                    {
                        roadBlockType = ConnectionType.Road;
                    }
                    else
                    {
                        roadBlockType = ConnectionType.Rail;
                    }
                }
                else
                {
                    roadBlockType = ConnectionType.Road;
                }
            }
            else
            {
                roadBlockType = ConnectionType.Rail;
            }
            return roadBlock1;
        }

        public Location ChoosePortToMoveHuntersToWithControlStorms(GameState game, Hunter hunterMoved)
        {
            var validPorts = new List<Location>();
            var connectedSeaLocations = new List<Location> {game.Hunters[(int) hunterMoved].CurrentLocation};
            var seaExtension = new List<Location>();
            for (var i = 0; i < 2; i++)
            {
                seaExtension.Clear();
                foreach (var loc in connectedSeaLocations)
                {
                    seaExtension.AddRange(game.Map.LocationsConnectedBySeaTo(loc));
                }
                foreach (var loc in seaExtension)
                {
                    if (game.Map.TypeOfLocation(loc) == LocationType.Sea && !connectedSeaLocations.Contains(loc))
                    {
                        connectedSeaLocations.Add(loc);
                    }
                }
            }
            seaExtension.Clear();
            foreach (var loc in connectedSeaLocations)
            {
                seaExtension.AddRange(game.Map.LocationsConnectedBySeaTo(loc));
            }
            foreach (var loc in seaExtension)
            {
                if ((game.Map.TypeOfLocation(loc) == LocationType.SmallCity ||
                     game.Map.TypeOfLocation(loc) == LocationType.LargeCity) && !validPorts.Contains(loc))
                {
                    validPorts.Add(loc);
                }
            }
            return validPorts[new Random().Next(0, validPorts.Count())];
        }

        public Location ChooseDestinationForWildHorses(GameState game)
        {
            var possibleDestination = game.Map.LocationsConnectedByRoadTo(game.Dracula.CurrentLocation);
            return possibleDestination[new Random().Next(0, possibleDestination.Count())];
        }

        public bool ChooseToPlayWildHorses(GameState game)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.WildHorses) != null &&
                new Random().Next(0, 2) == 0)
            {
                return true;
            }
            return false;
        }

        public bool ChooseToPlayCustomsSearch(GameState game, Hunter hunterMoved, Location origin)
        {
            var haveCard = game.Dracula.EventHand.Find(card => card.Event == Event.CustomsSearch) != null;
            var hunterWasAtSea = game.Map.TypeOfLocation(origin) == LocationType.Sea;
            var hunterIsNowOnLand = game.Map.TypeOfLocation(game.Hunters[(int) hunterMoved].CurrentLocation) ==
                                    LocationType.SmallCity ||
                                    game.Map.TypeOfLocation(game.Hunters[(int) hunterMoved].CurrentLocation) ==
                                    LocationType.LargeCity;
            var hunterMovedAcrossLand = game.Map.TypeOfLocation(game.Hunters[(int) hunterMoved].CurrentLocation) !=
                                        LocationType.Sea && game.Map.TypeOfLocation(origin) != LocationType.Sea;
            var hunterCrossedBorder = game.Map.IsEastern(origin) !=
                                      game.Map.IsEastern(game.Hunters[(int) hunterMoved].CurrentLocation);
            var destinationHasEncounters =
                game.Dracula.NumberOfEncountersAtLocation(game.Hunters[(int) hunterMoved].CurrentLocation) > 0;
            if (haveCard && !destinationHasEncounters &&
                ((hunterWasAtSea && hunterIsNowOnLand) || (hunterMovedAcrossLand && hunterCrossedBorder)) &&
                new Random().Next(0, 2) == 0)
            {
                return true;
            }
            return false;
        }

        public bool ChooseToPlaySeduction(GameState game)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.Seduction) != null &&
                new Random().Next(0, 2) == 0)
            {
                return true;
            }
            return false;
        }

        public bool ChooseToPlayControlStorms(GameState game, Hunter hunterMoved)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.ControlStorms) != null &&
                game.Map.TypeOfLocation(game.Hunters[(int) hunterMoved].CurrentLocation) == LocationType.Sea &&
                new Random().Next(0, 2) == 0)
            {
                return true;
            }
            return false;
        }

        public bool ChooseToPlayRelentlessMinion(GameState game, List<Hunter> huntersInvolved, Opponent opponent)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.RelentlessMinion) != null &&
                new Random().Next(0, 2) == 0)
            {
                return true;
            }
            return false;
        }

        public bool ChooseToPlayTrap(GameState game, List<HunterPlayer> huntersInvolved, Opponent opponent)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.Trap) != null && new Random().Next(0, 2) == 0)
            {
                return true;
            }
            return false;
        }

        public Item ChooseItemToDiscardWithRage(GameState game, Hunter rageTarget)
        {
            if (game.Hunters[(int) rageTarget].ItemCount == 0)
            {
                return Item.None;
            }
            return
                game.Hunters[(int) rageTarget].ItemsKnownToDracula[
                    new Random().Next(0, game.Hunters[(int) rageTarget].ItemsKnownToDracula.Count())].Item;
        }

        public Hunter ChooseToPlayRage(GameState game, List<HunterPlayer> huntersInvolved)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.Rage) != null && new Random().Next(0, 2) == 0)
            {
                return huntersInvolved[new Random().Next(0, huntersInvolved.Count())].Hunter;
            }
            return Hunter.Nobody;
        }

        public bool ChooseToPlaySensationalistPress(GameState game, Location location)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.SensationalistPress) != null &&
                new Random().Next(0, 2) == 0)
            {
                return true;
            }
            return false;
        }

        public HunterPlayer ChooseNightVisitVictim(List<HunterPlayer> bittenHunters)
        {
            return bittenHunters[new Random().Next(0, bittenHunters.Count())];
        }

        public Event ChooseAllyToKeep(Event existingAlly, Event newAlly)
        {
            if (new Random().Next(0, 2) == 0)
            {
                return newAlly;
            }
            return existingAlly;
        }

        public HunterPlayer ChooseVampiricInfluenceVictim(List<HunterPlayer> bittenHunters)
        {
            return bittenHunters[new Random().Next(0, bittenHunters.Count())];
        }

        public Location ChooseWhereToEvadeTo(GameState game)
        {
            var allLocations = Enumerations.GetAllLocations();
            Location destination;
            do
            {
                destination = allLocations[new Random().Next(0, allLocations.Count())];
            } while (destination == Location.StJosephAndStMary);
            return destination;
        }

        public Hunter ChooseVictimForQuinceyPMorris(GameState game)
        {
            return (Hunter) (new Random().Next(1, 5));
        }
    }
}