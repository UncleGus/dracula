using System;
using System.Collections.Generic;
using System.Linq;
using FuryOfDracula.GameLogic;

namespace FuryOfDracula.ArtificialIntelligence
{
    public class DecisionMaker
    {
        public int NumberOfPossibleCurrentLocations
        {
            get
            {
                List<Location> possibleCurrentLocations = new List<Location>();
                foreach (var trail in PossibilityTree)
                {
                    Location currentLocation = Location.Nowhere;
                    for (int i = 0; i < 5; i++)
                    {
                        if (trail[i].Location != Location.Nowhere)
                        {
                            currentLocation = trail[i].Location;
                            break;
                        }
                    }
                    if (!possibleCurrentLocations.Contains(currentLocation))
                    {
                        possibleCurrentLocations.Add(currentLocation);
                    }
                }
                return possibleCurrentLocations.Count();
            }
        }

        public List<PossibleTrailSlot[]> PossibilityTree = new List<PossibleTrailSlot[]>();

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
            PossibleTrailSlot[] actualTrail = GetActualTrail(game);
            List<PossibleTrailSlot[]> possibilityTree = new List<PossibleTrailSlot[]>();
            possibilityTree.Add(actualTrail);
            List<int> numberOfPossibilities;
            List<PossibleTrailSlot> sneakiestPossibleMoves = CalculateSneakiestTrail(game, possibilityTree, 6, 0, out numberOfPossibilities);

            if (sneakiestPossibleMoves.Any())
            {
                power = sneakiestPossibleMoves[0].Power;
                return sneakiestPossibleMoves[0].Location;
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
                        if (card.Location != Location.Nowhere && possibleDestinations.Contains(card.Location))
                        {
                            possibleDestinations.Remove(card.Location);
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
                        if (card.Location != Location.Nowhere && possibleDestinations.Contains(card.Location))
                        {
                            possibleDoubleBackDestinations.Add(card.Location);
                            possibleDestinations.Remove(card.Location);
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
            if (game.Dracula.Blood < 3)
            {
                possiblePowers.Remove(Power.DarkCall);
            }
            var totalPossibleMoves = possibleDestinations.Count() + possiblePowers.Count();
            if (totalPossibleMoves == 0)
            {
                power = Power.None;
                return Location.Nowhere;
            }
            if (!possiblePowers.Any() || (possibleDestinations.Any() && new Random().Next(0, 4) > 0))
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
                if (h != null && game.Map.TypeOfLocation(h.CurrentLocation) != LocationType.Hospital &&
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
            var connectedSeaLocations = new List<Location> { game.Hunters[(int)hunterMoved].CurrentLocation };
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
            var hunterIsNowOnLand = game.Map.TypeOfLocation(game.Hunters[(int)hunterMoved].CurrentLocation) ==
                                    LocationType.SmallCity ||
                                    game.Map.TypeOfLocation(game.Hunters[(int)hunterMoved].CurrentLocation) ==
                                    LocationType.LargeCity;
            var hunterMovedAcrossLand = game.Map.TypeOfLocation(game.Hunters[(int)hunterMoved].CurrentLocation) !=
                                        LocationType.Sea && game.Map.TypeOfLocation(origin) != LocationType.Sea;
            var hunterCrossedBorder = game.Map.IsEastern(origin) !=
                                      game.Map.IsEastern(game.Hunters[(int)hunterMoved].CurrentLocation);
            var destinationHasEncounters =
                game.Dracula.NumberOfEncountersAtLocation(game.Hunters[(int)hunterMoved].CurrentLocation) > 0;
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
                game.Map.TypeOfLocation(game.Hunters[(int)hunterMoved].CurrentLocation) == LocationType.Sea &&
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
            if (game.Hunters[(int)rageTarget].ItemCount == 0)
            {
                return Item.None;
            }
            return
                game.Hunters[(int)rageTarget].ItemsKnownToDracula[
                    new Random().Next(0, game.Hunters[(int)rageTarget].ItemsKnownToDracula.Count())].Item;
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
            return (Hunter)(new Random().Next(1, 5));
        }

        public void InitialisePossibilityTree(GameState game)
        {
            PossibilityTree.Clear();
            List<Location> allLocations = Enumerations.GetAllLocations();
            foreach (Location location in allLocations)
            {
                if (!game.HuntersAt(location).Any() && game.Map.TypeOfLocation(location) != LocationType.Sea && game.Map.TypeOfLocation(location) != LocationType.Castle && game.Map.TypeOfLocation(location) != LocationType.Hospital)
                {
                    PossibilityTree.Add(new PossibleTrailSlot[6] { new PossibleTrailSlot(location, Power.None), null, null, null, null, null });
                }
            }
        }

        public void AddOrangeBackedCardToAllPossibleTrails(GameState game)
        {
            List<PossibleTrailSlot[]> newPossibilityTree = new List<PossibleTrailSlot[]>();
            foreach (PossibleTrailSlot[] trail in PossibilityTree)
            {
                Location currentLocation = Location.Nowhere;
                for (int i = 0; i < 6; i++)
                {
                    if (trail[i].Location != Location.Nowhere)
                    {
                        currentLocation = trail[i].Location;
                        break;
                    }
                }
                List<PossibleTrailSlot> possibleCards = new List<PossibleTrailSlot>();
                List<Location> possibleLocations = game.Map.LocationsConnectedByRoadTo(currentLocation);
                foreach (Location location in possibleLocations)
                {
                    if (!TrailContainsLocation(trail, location) && !game.LocationIsBlocked(location))
                    {
                        possibleCards.Add(new PossibleTrailSlot(location, Power.None));
                    }
                }
                if (!TrailContainsPower(trail, Power.Hide))
                {
                    possibleCards.Add(new PossibleTrailSlot(Location.Nowhere, Power.Hide));
                }
                foreach (PossibleTrailSlot possibleCard in possibleCards)
                {
                    PossibleTrailSlot[] newTrail = new PossibleTrailSlot[6];
                    for (int i = 5; i > 0; i--)
                    {
                        newTrail[i] = trail[i - 1];
                    }
                    newTrail[0] = possibleCard;
                    newPossibilityTree.Add(newTrail);
                }
            }
            PossibilityTree = newPossibilityTree;
        }

        public void AddBlueBackedCardToAllPossibleTrails(GameState game)
        {
            List<PossibleTrailSlot[]> newPossibilityTree = new List<PossibleTrailSlot[]>();
            foreach (PossibleTrailSlot[] trail in PossibilityTree)
            {
                Location currentLocation = Location.Nowhere;
                for (int i = 0; i < 6; i++)
                {
                    if (trail[i].Location != Location.Nowhere)
                    {
                        currentLocation = trail[i].Location;
                        break;
                    }
                }
                List<PossibleTrailSlot> possibleCards = new List<PossibleTrailSlot>();
                List<Location> possibleLocations = game.Map.LocationsConnectedBySeaTo(currentLocation);
                foreach (Location location in possibleLocations)
                {
                    if (!TrailContainsLocation(trail, location) && game.Map.TypeOfLocation(location) == LocationType.Sea && !game.LocationIsBlocked(location))
                    {
                        possibleCards.Add(new PossibleTrailSlot(location, Power.None));
                    }
                }
                foreach (PossibleTrailSlot possibleCard in possibleCards)
                {
                    PossibleTrailSlot[] newTrail = new PossibleTrailSlot[6];
                    for (int i = 5; i > 0; i--)
                    {
                        newTrail[i] = trail[i - 1];
                    }
                    newTrail[0] = possibleCard;
                    newPossibilityTree.Add(newTrail);
                }
            }
            PossibilityTree = newPossibilityTree;
        }

        public void AddPowerCardToAllPossibleTrails(Power power)
        {
            foreach (PossibleTrailSlot[] trail in PossibilityTree)
            {
                for (int i = 5; i > 0; i--)
                {
                    trail[i] = trail[i - 1];
                }
                trail[0] = new PossibleTrailSlot(Location.Nowhere, power);
            }
        }

        public void AddDoubleBackToAllPossibleTrails(GameState game, int doubleBackSlot)
        {
            List<PossibleTrailSlot[]> newPossibilityTree = new List<PossibleTrailSlot[]>();
            foreach (PossibleTrailSlot[] trail in PossibilityTree)
            {
                Location currentLocation = Location.Nowhere;
                for (int i = 0; i < 6; i++)
                {
                    if (trail[i].Location != Location.Nowhere)
                    {
                        currentLocation = trail[i].Location;
                        break;
                    }
                }
                if (trail[doubleBackSlot].Power != Power.Hide && game.Map.LocationsConnectedByRoadOrSeaTo(currentLocation).Contains(trail[doubleBackSlot].Location) && !game.LocationIsBlocked(trail[doubleBackSlot].Location))
                {
                    PossibleTrailSlot[] newPossibleTrail = new PossibleTrailSlot[6];
                    for (int i = 5; i > doubleBackSlot + 1; i--)
                    {
                        newPossibleTrail[i] = trail[i - 1];
                    }
                    for (int i = doubleBackSlot; i > 0; i--)
                    {
                        newPossibleTrail[i] = trail[i - 1];
                    }
                    newPossibleTrail[0] = trail[doubleBackSlot];
                    newPossibleTrail[0].Power = Power.DoubleBack;
                    newPossibilityTree.Add(newPossibleTrail);
                }
            }
            PossibilityTree = newPossibilityTree;
        }

        public void AddWolfFormToAllPossibleTrails(GameState game)
        {
            List<PossibleTrailSlot[]> newPossibilityTree = new List<PossibleTrailSlot[]>();
            foreach (PossibleTrailSlot[] trail in PossibilityTree)
            {
                Location currentLocation = Location.Nowhere;
                for (int i = 0; i < 6; i++)
                {
                    if (trail[i].Location != Location.Nowhere)
                    {
                        currentLocation = trail[i].Location;
                        break;
                    }
                }
                List<Location> possibleDestinations = new List<Location>();
                List<Location> locationsToAdd = game.Map.LocationsConnectedByRoadTo(currentLocation);
                foreach (Location location in locationsToAdd)
                {
                    if (!game.LocationIsBlocked(location) && !possibleDestinations.Contains(location))
                    {
                        possibleDestinations.Add(location);
                    }
                }
                List<Location> moreLocationsToAdd = new List<Location>();
                foreach (Location location in possibleDestinations)
                {
                    locationsToAdd = game.Map.LocationsConnectedByRoadTo(location);
                    foreach (Location loc in locationsToAdd)
                    {
                        if (!game.LocationIsBlocked(loc) && !possibleDestinations.Contains(loc) && !TrailContainsLocation(trail, loc))
                        {
                            moreLocationsToAdd.Add(loc);
                        }
                    }
                }
                possibleDestinations.AddRange(moreLocationsToAdd);
                List<PossibleTrailSlot> possibleCards = new List<PossibleTrailSlot>();
                foreach (Location location in possibleDestinations)
                {
                    possibleCards.Add(new PossibleTrailSlot(location, Power.WolfForm));
                }
                foreach (PossibleTrailSlot possibleCard in possibleCards)
                {
                    PossibleTrailSlot[] newTrail = new PossibleTrailSlot[6];
                    for (int i = 5; i > 0; i--)
                    {
                        newTrail[i] = trail[i - 1];
                    }
                    newTrail[0] = possibleCard;
                    newPossibilityTree.Add(newTrail);
                }
            }
            PossibilityTree = newPossibilityTree;
        }

        public void TrimAllPossibleTrails(int length)
        {
            foreach (PossibleTrailSlot[] trail in PossibilityTree)
            {
                Location currentLocation = Location.Nowhere;
                for (int i = 0; i < 6; i++)
                {
                    if (trail[i].Location != Location.Nowhere)
                    {
                        currentLocation = trail[i].Location;
                        break;
                    }

                }
                for (int i = 5; i >= length; i--)
                {
                    if (trail[i] != null)
                    {
                        if (trail[i].Location != currentLocation)
                        {
                            trail[i] = null;
                        }
                        else
                        {
                            length--;
                        }
                    }
                }
                for (int i = 0; i < 6; i++)
                {
                    if (trail[i] == null)
                    {
                        for (var j = i + 1; j < 6; j++)
                        {
                            if (trail[j] != null)
                            {
                                trail[i] = trail[j];
                                trail[j] = null;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void EliminateTrailsThatContainLocation(Location location)
        {
            List<PossibleTrailSlot[]> newPossibilityTree = new List<PossibleTrailSlot[]>();
            foreach (PossibleTrailSlot[] trail in PossibilityTree)
            {
                if (!TrailContainsLocation(trail, location))
                {
                    newPossibilityTree.Add(trail);
                }
            }
            PossibilityTree = newPossibilityTree;
        }

        public void EliminateTrailsThatDoNotContainLocationAtPosition(Location location, int position)
        {
            List<PossibleTrailSlot[]> newPossibilityTree = new List<PossibleTrailSlot[]>();
            foreach (PossibleTrailSlot[] trail in PossibilityTree)
            {
                if (trail[position].Location == location)
                {
                    newPossibilityTree.Add(trail);
                }
            }
            PossibilityTree = newPossibilityTree;
        }

        public void AddEvasionCardToTrail(GameState game)
        {
            List<PossibleTrailSlot[]> newPossibilityTree = new List<PossibleTrailSlot[]>();
            if (game.Dracula.Trail[0].DraculaCards.First().IsRevealed)
            {
                foreach (PossibleTrailSlot[] trail in PossibilityTree)
                {
                    {
                        PossibleTrailSlot[] newPossibleTrail = new PossibleTrailSlot[6];
                        for (int i = 5; i > 0; i--)
                        {
                            newPossibleTrail[i] = trail[i - 1];
                        }
                        newPossibleTrail[0] = new PossibleTrailSlot(game.Dracula.Trail[0].DraculaCards.First().Location, Power.None);
                        newPossibilityTree.Add(newPossibleTrail);
                    }
                }
            }
            else if (game.Map.TypeOfLocation(game.Dracula.Trail[0].DraculaCards.First().Location) == LocationType.SmallCity ||
       game.Map.TypeOfLocation(game.Dracula.Trail[0].DraculaCards.First().Location) == LocationType.LargeCity)
            {
                List<Location> allCities = new List<Location>();
                List<Location> allLocations = Enumerations.GetAllLocations();
                foreach (Location loc in allLocations)
                {
                    if (game.Map.TypeOfLocation(loc) == LocationType.SmallCity || game.Map.TypeOfLocation(loc) == LocationType.LargeCity)
                    {
                        allCities.Add(loc);
                    }
                }
                foreach (PossibleTrailSlot[] trail in PossibilityTree)
                {
                    foreach (Location location in allCities)
                    {
                        PossibleTrailSlot[] newPossibleTrail = new PossibleTrailSlot[6];
                        for (int i = 5; i > 0; i--)
                        {
                            newPossibleTrail[i] = trail[i - 1];
                        }
                        newPossibleTrail[0] = new PossibleTrailSlot(location, Power.None);
                        newPossibilityTree.Add(newPossibleTrail);
                    }
                }
            }
            else if (game.Map.TypeOfLocation(game.Dracula.Trail[0].DraculaCards.First().Location) == LocationType.Sea)
            {
                List<Location> allSeas = new List<Location>();
                List<Location> allLocations = Enumerations.GetAllLocations();
                foreach (Location loc in allLocations)
                {
                    if (game.Map.TypeOfLocation(loc) == LocationType.Sea)
                    {
                        allSeas.Add(loc);
                    }
                }
                foreach (PossibleTrailSlot[] trail in PossibilityTree)
                {
                    foreach (Location location in allSeas)
                    {
                        PossibleTrailSlot[] newPossibleTrail = new PossibleTrailSlot[6];
                        for (int i = 5; i > 0; i--)
                        {
                            newPossibleTrail[i] = trail[i - 1];
                        }
                        newPossibleTrail[0] = new PossibleTrailSlot(location, Power.None);
                        newPossibilityTree.Add(newPossibleTrail);
                    }
                }
            }
            else if (game.Map.TypeOfLocation(game.Dracula.Trail[0].DraculaCards.First().Location) == LocationType.Castle)
            {
                foreach (PossibleTrailSlot[] trail in PossibilityTree)
                {
                    PossibleTrailSlot[] newPossibleTrail = new PossibleTrailSlot[6];
                    for (int i = 5; i > 0; i--)
                    {
                        newPossibleTrail[i] = trail[i - 1];
                    }
                    newPossibleTrail[0] = new PossibleTrailSlot(Location.CastleDracula, Power.None);
                    newPossibilityTree.Add(newPossibleTrail);
                }
            }
            PossibilityTree = newPossibilityTree;
        }

        private bool TrailContainsPower(PossibleTrailSlot[] trail, Power power)
        {
            for (int i = 0; i < 6; i++)
            {
                if (trail[i] != null && trail[i].Power == power)
                {
                    return true;
                }
            }
            return false;
        }

        private bool TrailContainsLocation(PossibleTrailSlot[] trail, Location location)
        {
            for (int i = 0; i < 6; i++)
            {
                if (trail[i] != null && trail[i].Location == location)
                {
                    return true;
                }
            }
            return false;
        }

        public void EliminateTrailsThatDoNotContainHideAtPosition(int position)
        {
            List<PossibleTrailSlot[]> newPossibilityTree = new List<PossibleTrailSlot[]>();
            foreach (PossibleTrailSlot[] trail in PossibilityTree)
            {
                if (trail[position].Power == Power.Hide)
                {
                    newPossibilityTree.Add(trail);
                }
            }
            PossibilityTree = newPossibilityTree;
        }

        public void AddEscapeAsBatCardToAllTrails(GameState game)
        {
            List<PossibleTrailSlot[]> newPossibilityTree = new List<PossibleTrailSlot[]>();
            foreach (PossibleTrailSlot[] trail in PossibilityTree)
            {
                Location currentLocation = Location.Nowhere;
                for (int i = 0; i < 6; i++)
                {
                    if (trail[i].Location != Location.Nowhere)
                    {
                        currentLocation = trail[i].Location;
                        break;
                    }
                }
                List<Location> possibleDestinations = new List<Location>();
                List<Location> locationsToAdd = game.Map.LocationsConnectedByRoadTo(currentLocation);
                foreach (Location location in locationsToAdd)
                {
                    if (!game.LocationIsBlocked(location) && !possibleDestinations.Contains(location))
                    {
                        possibleDestinations.Add(location);
                    }
                }
                List<Location> moreLocationsToAdd = new List<Location>();
                foreach (Location location in possibleDestinations)
                {
                    locationsToAdd = game.Map.LocationsConnectedByRoadTo(location);
                    foreach (Location loc in locationsToAdd)
                    {
                        if (!game.LocationIsBlocked(loc) && !possibleDestinations.Contains(loc) && !TrailContainsLocation(trail, loc))
                        {
                            moreLocationsToAdd.Add(loc);
                        }
                    }
                }
                possibleDestinations.AddRange(moreLocationsToAdd);
                List<PossibleTrailSlot> possibleCards = new List<PossibleTrailSlot>();
                foreach (Location location in possibleDestinations)
                {
                    possibleCards.Add(new PossibleTrailSlot(location, Power.None));
                }
                foreach (PossibleTrailSlot possibleCard in possibleCards)
                {
                    PossibleTrailSlot[] newTrail = new PossibleTrailSlot[6];
                    for (int i = 5; i > 0; i--)
                    {
                        newTrail[i] = trail[i];
                    }
                    newTrail[0] = possibleCard;
                    newPossibilityTree.Add(newTrail);
                }
            }
            PossibilityTree = newPossibilityTree;
        }

        public void AddDisembarkedCardToAllPossibleTrails(GameState game)
        {
            List<PossibleTrailSlot[]> newPossibilityTree = new List<PossibleTrailSlot[]>();
            foreach (PossibleTrailSlot[] trail in PossibilityTree)
            {
                Location currentLocation = Location.Nowhere;
                for (int i = 0; i < 6; i++)
                {
                    if (trail[i].Location != Location.Nowhere)
                    {
                        currentLocation = trail[i].Location;
                        break;
                    }
                }
                List<PossibleTrailSlot> possibleCards = new List<PossibleTrailSlot>();
                List<Location> possibleLocations = game.Map.LocationsConnectedBySeaTo(currentLocation);
                foreach (Location location in possibleLocations)
                {
                    if (!TrailContainsLocation(trail, location) && game.Map.TypeOfLocation(location) != LocationType.Sea && !game.LocationIsBlocked(location))
                    {
                        possibleCards.Add(new PossibleTrailSlot(location, Power.None));
                    }
                }
                foreach (PossibleTrailSlot possibleCard in possibleCards)
                {
                    PossibleTrailSlot[] newTrail = new PossibleTrailSlot[6];
                    for (int i = 5; i > 0; i--)
                    {
                        newTrail[i] = trail[i - 1];
                    }
                    newTrail[0] = possibleCard;
                    newPossibilityTree.Add(newTrail);
                }
            }
            PossibilityTree = newPossibilityTree;
        }

        public void EliminateTrailsThatHaveHuntersAtPosition(GameState game, int position)
        {
            var newPossibilityTree = new List<PossibleTrailSlot[]>();
            foreach (var trail in PossibilityTree)
            {
                if (!game.HuntersAt(trail[position].Location).Any())
                {
                    newPossibilityTree.Add(trail);
                }
            }
            PossibilityTree = newPossibilityTree;
        }

        public void EliminateTrailsThatDoNotContainLocation(Location location)
        {
            List<PossibleTrailSlot[]> newPossibilityTree = new List<PossibleTrailSlot[]>();
            foreach (PossibleTrailSlot[] trail in PossibilityTree)
            {
                if (TrailContainsLocation(trail, location))
                {
                    newPossibilityTree.Add(trail);
                }
            }
            PossibilityTree = newPossibilityTree;
        }

        public void EliminateTrailsThatContainLocationAtPosition(Location location, int position)
        {
            List<PossibleTrailSlot[]> newPossibilityTree = new List<PossibleTrailSlot[]>();
            foreach (PossibleTrailSlot[] trail in PossibilityTree)
            {
                if (trail[position].Location != location && !(trail[position].Power == Power.Hide && trail[position + 1].Location == location))
                {
                    newPossibilityTree.Add(trail);
                }
            }
            PossibilityTree = newPossibilityTree;
        }

        public PossibleTrailSlot[] GetActualTrail(GameState game)
        {
            PossibleTrailSlot[] actualTrail = new PossibleTrailSlot[6];
            for (int i = 0; i < 6; i++)
            {
                if (game.Dracula.Trail[i] != null)
                {
                    actualTrail[i] = new PossibleTrailSlot(Location.Nowhere, Power.None);
                    foreach (DraculaCard card in game.Dracula.Trail[i].DraculaCards)
                    {
                        if (card.Location != Location.Nowhere)
                        {
                            actualTrail[i].Location = card.Location;
                        }
                        if (card.Power != Power.None)
                        {
                            actualTrail[i].Power = card.Power;
                        }
                    }
                }
            }
            actualTrail[0].TimeOfDay = game.TimeOfDay;
            return actualTrail;
        }

        public List<PossibleTrailSlot> CalculateSneakiestTrail(GameState game, List<PossibleTrailSlot[]> potentialityTree, int depth, int originatingSlot, out List<int> numberOfPossibilities)
        {
            if (depth == 0)
            {
                List<PossibleTrailSlot> uniqueFirstMoves = new List<PossibleTrailSlot>();
                foreach (var trail in potentialityTree)
                {
                    if (uniqueFirstMoves.Find(slot => slot.Location == trail[originatingSlot - 1].Location && slot.Power == trail[originatingSlot - 1].Power) == null)
                    {
                        uniqueFirstMoves.Add(trail[originatingSlot - 1]);
                    }
                }
                List<int> numberOfMatches = new List<int>();
                foreach (var u in uniqueFirstMoves)
                {
                    int count = 0;
                    foreach (var trail in potentialityTree)
                    {
                        if (trail[originatingSlot - 1].Location == u.Location && trail[originatingSlot - 1].Power == u.Power)
                        {
                            count++;
                        }
                    }
                    numberOfMatches.Add(count);
                }
                //int indexOfHighestCount = 0;
                int currentIndex = -1;
                int highestCount = 0;
                foreach (int i in numberOfMatches)
                {
                    currentIndex++;
                    if (i > highestCount)
                    {
                        highestCount = i;
                        //indexOfHighestCount = currentIndex;
                    }
                }
                for (int i = numberOfMatches.Count() - 1; i >= 0; i--)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (numberOfMatches[j] < numberOfMatches[j + 1])
                        {
                            int temp = numberOfMatches[j];
                            numberOfMatches[j] = numberOfMatches[j + 1];
                            numberOfMatches[j + 1] = temp;
                            PossibleTrailSlot tmp = uniqueFirstMoves[j];
                            uniqueFirstMoves[j] = uniqueFirstMoves[j + 1];
                            uniqueFirstMoves[j + 1] = tmp;
                        }
                    }
                }
                numberOfPossibilities = numberOfMatches;
                return uniqueFirstMoves;
            }
            List<PossibleTrailSlot[]> newPotentialityTree = new List<PossibleTrailSlot[]>();
            foreach (var trail in potentialityTree)
            {
                List<Location> possibleDestinations = game.Map.LocationsConnectedByRoadOrSeaTo(game.Dracula.CurrentLocation);
                List<Location> locationsToRemoveFromPossibleDestinations = new List<Location>();
                foreach (Location loc in possibleDestinations)
                {
                    if (game.LocationIsBlocked(loc) || ((TrailContainsLocation(trail, loc) || game.CatacombsContainsLocation(loc)) && TrailContainsPower(trail, Power.DoubleBack)))
                    {
                        locationsToRemoveFromPossibleDestinations.Add(loc);
                    }
                    if (TrailContainsLocation(trail, loc))
                    {
                        Location currentLocation = Location.Nowhere;
                        for (int i = 0; i < 6; i++)
                        {
                            if (trail[i].Location != Location.Nowhere)
                            {
                                currentLocation = trail[i].Location;
                                break;
                            }
                        }
                        if (loc == currentLocation)
                        {
                            locationsToRemoveFromPossibleDestinations.Add(loc);
                        }
                    }
                }
                foreach (Location loc in locationsToRemoveFromPossibleDestinations)
                {
                    possibleDestinations.Remove(loc);
                }
                if ((int)trail[0].TimeOfDay > 3 && game.Dracula.Blood > 1 && !TrailContainsPower(trail, Power.WolfForm))
                {
                    List<Location> locationsToAdd = new List<Location>();
                    foreach (Location loc in possibleDestinations)
                    {
                        locationsToAdd.AddRange(game.Map.LocationsConnectedByRoadTo(loc));
                    }
                    foreach (Location loc in locationsToAdd)
                    {
                        if (!possibleDestinations.Contains(loc) && !game.LocationIsBlocked(loc) && !TrailContainsLocation(trail, loc))
                        {
                            possibleDestinations.Add(loc);
                        }
                    }
                }
                List<Power> possiblePowers = new List<Power>();
                if (game.Map.TypeOfLocation(trail[0].Location) != LocationType.Sea)
                {
                    if (!TrailContainsPower(trail, Power.Hide))
                    {
                        possiblePowers.Add(Power.Hide);
                    }
                    if ((int)trail[0].TimeOfDay > 3 && !TrailContainsPower(trail, Power.Feed))
                    {
                        possiblePowers.Add(Power.Feed);
                    }
                    if (game.Dracula.Blood > 2 && !TrailContainsPower(trail, Power.DarkCall))
                    {
                        possiblePowers.Add(Power.DarkCall);
                    }
                }
                foreach (Location loc in possibleDestinations)
                {
                    PossibleTrailSlot[] newTrail = new PossibleTrailSlot[6];
                    for (int i = 5; i > 0; i--)
                    {
                        newTrail[i] = trail[i - 1];
                    }
                    if (TrailContainsLocation(trail, loc) || game.CatacombsContainsLocation(loc))
                    {
                        newTrail[0] = new PossibleTrailSlot(loc, Power.DoubleBack);
                    }
                    else if (!game.Map.LocationsConnectedByRoadOrSeaTo(loc).Contains(game.Dracula.CurrentLocation))
                    {
                        newTrail[0] = new PossibleTrailSlot(loc, Power.WolfForm);
                    }
                    else
                    {
                        newTrail[0] = new PossibleTrailSlot(loc, Power.None);
                    }
                    if (game.Map.TypeOfLocation(trail[0].Location) == LocationType.Sea)
                    {
                        newTrail[0].TimeOfDay = trail[0].TimeOfDay;
                    }
                    else
                    {
                        newTrail[0].TimeOfDay = (TimeOfDay)(((int)trail[0].TimeOfDay) % 6 + 1);
                    }
                    newPotentialityTree.Add(newTrail);
                }
                foreach (Power p in possiblePowers)
                {
                    PossibleTrailSlot[] newTrail = new PossibleTrailSlot[6];
                    for (int i = 5; i > 0; i--)
                    {
                        newTrail[i] = trail[i - 1];
                    }
                    newTrail[0] = new PossibleTrailSlot(Location.Nowhere, p);
                    if (game.Map.TypeOfLocation(trail[0].Location) == LocationType.Sea)
                    {
                        newTrail[0].TimeOfDay = trail[0].TimeOfDay;
                    }
                    else
                    {
                        newTrail[0].TimeOfDay = (TimeOfDay)(((int)trail[0].TimeOfDay) % 6 + 1);
                    }
                    newPotentialityTree.Add(newTrail);
                }
            }
            return CalculateSneakiestTrail(game, newPotentialityTree, depth - 1, originatingSlot + 1, out numberOfPossibilities);
        }

        private int IndexOfLocationInTrail(PossibleTrailSlot[] trail, Location loc)
        {
            for (int i = 0; i < 6; i ++)
            {
                if (trail[i].Location == loc)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}