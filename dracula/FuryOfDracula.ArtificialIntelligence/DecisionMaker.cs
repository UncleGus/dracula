using System;
using System.Collections.Generic;
using System.Linq;
using FuryOfDracula.GameLogic;

namespace FuryOfDracula.ArtificialIntelligence
{
    public class DecisionMaker
    {
        private float SEAMOVEMULTIPLIER = 0.5F;
        private float DISTANCEFROMHUNTERMULTIPLIER = 0.5F;
        private float DARKCALLMULTIPLIER = 0.5F;
        private float WOLFFORMMULTIPLIER = 0.75F;
        private int CATACOMBSCHANCE = 4;

        public Strategy Strategy = Strategy.Sneaky;

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
            List<PossibleTrailSlot> possibleMoves = new List<PossibleTrailSlot>();

            if (Strategy == Strategy.Sneaky)
            {
                possibleMoves = DetermineOrderedMoves(game, possibilityTree, 6, 0, out numberOfPossibilities);

                if (possibleMoves.Any())
                {
                    List<int> distances = new List<int>();
                    int currentDistanceFromHunters = game.GetDistanceToClosestHunter(game.Dracula.CurrentLocation, true);
                    foreach (PossibleTrailSlot move in possibleMoves)
                    {
                        if (move.Location != Location.Nowhere)
                        {
                            distances.Add(game.GetDistanceToClosestHunter(move.Location, true));
                            GC.Collect();
                        }
                        else
                        {
                            distances.Add(currentDistanceFromHunters);
                        }
                    }
                    int index = -1;
                    foreach (PossibleTrailSlot move in possibleMoves)
                    {
                        index++;
                        if (game.Map.TypeOfLocation(move.Location) == LocationType.Sea)
                        {
                            numberOfPossibilities[index] = (int)(numberOfPossibilities[index] * SEAMOVEMULTIPLIER);
                        }
                        if (currentDistanceFromHunters - distances[index] > 0)
                        {
                            numberOfPossibilities[index] = (int)(numberOfPossibilities[index] * DISTANCEFROMHUNTERMULTIPLIER);
                        }
                        if (currentDistanceFromHunters - distances[index] > 1)
                        {
                            numberOfPossibilities[index] = (int)(numberOfPossibilities[index] * DISTANCEFROMHUNTERMULTIPLIER);
                        }
                        if (currentDistanceFromHunters - distances[index] < 0)
                        {
                            numberOfPossibilities[index] = (int)(numberOfPossibilities[index] / DISTANCEFROMHUNTERMULTIPLIER);
                        }
                        if (currentDistanceFromHunters - distances[index] < 1)
                        {
                            numberOfPossibilities[index] = (int)(numberOfPossibilities[index] / DISTANCEFROMHUNTERMULTIPLIER);
                        }
                        if (move.Power == Power.DarkCall)
                        {
                            numberOfPossibilities[index] = (int)(numberOfPossibilities[index] * DARKCALLMULTIPLIER);
                        }
                        if (move.Power == Power.WolfForm)
                        {
                            numberOfPossibilities[index] = (int)(numberOfPossibilities[index] * WOLFFORMMULTIPLIER);
                        }
                    }
                    int totalCombinations = 0;
                    foreach (int i in numberOfPossibilities)
                    {
                        totalCombinations += i;
                    }
                    int randomNumber = new Random().Next(0, totalCombinations);
                    index = -1;
                    int count = 0;
                    foreach (int i in numberOfPossibilities)
                    {
                        index++;
                        count += i;
                        if (count > randomNumber)
                        {
                            power = possibleMoves[index].Power;
                            return possibleMoves[index].Location;
                        }
                    }
                    power = possibleMoves[0].Power;
                    return possibleMoves[0].Location;
                }
            }

            possibleMoves = DetermineOrderedMoves(game, possibilityTree, 1, 0, out numberOfPossibilities);
            if (!possibleMoves.Any())
            {
                power = Power.None;
                return Location.Nowhere;
            }

            if (Strategy == Strategy.Aggressive)
            {
                List<int> distances = new List<int>();
                int currentDistanceFromHunters = game.GetDistanceToClosestHunter(game.Dracula.CurrentLocation, false);
                foreach (PossibleTrailSlot move in possibleMoves)
                {
                    if (move.Location != Location.Nowhere)
                    {
                        distances.Add(game.GetDistanceToClosestHunter(move.Location, false));
                        GC.Collect();
                    }
                    else
                    {
                        distances.Add(currentDistanceFromHunters);
                    }
                }
                int shortestDistance = distances.First();
                foreach (int i in distances)
                {
                    if (i < shortestDistance)
                    {
                        shortestDistance = i;
                    }
                }
                List<PossibleTrailSlot> shortList = new List<PossibleTrailSlot>();
                int index = -1;
                foreach (PossibleTrailSlot move in possibleMoves)
                {
                    index++;
                    if (distances[index] == shortestDistance)
                    {
                        shortList.Add(move);
                    }
                }
                int randomNumber = new Random().Next(0, shortList.Count());
                power = shortList[randomNumber].Power;
                return shortList[randomNumber].Location;
            }

            if (Strategy == Strategy.FleeToCastleDracula)
            {
                List<int> distances = new List<int>();
                int currentDistanceFromCastleDracula = game.DistanceByRoadOrSeaBetween(game.Dracula.CurrentLocation, Location.CastleDracula, false);
                foreach (PossibleTrailSlot move in possibleMoves)
                {
                    if (move.Location != Location.Nowhere)
                    {
                        distances.Add(game.DistanceByRoadOrSeaBetween(move.Location, Location.CastleDracula, false));
                        GC.Collect();
                    }
                    else
                    {
                        distances.Add(currentDistanceFromCastleDracula);
                    }
                }
                int shortestDistance = distances.First();
                foreach (int i in distances)
                {
                    if (i < shortestDistance)
                    {
                        shortestDistance = i;
                    }
                }
                List<PossibleTrailSlot> shortList = new List<PossibleTrailSlot>();
                int index = -1;
                foreach (PossibleTrailSlot move in possibleMoves)
                {
                    index++;
                    if (distances[index] == shortestDistance)
                    {
                        shortList.Add(move);
                    }
                }
                int randomNumber = new Random().Next(0, shortList.Count());
                power = shortList[randomNumber].Power;
                return shortList[randomNumber].Location;
            }

            int rand = new Random().Next(0, possibleMoves.Count());
            power = possibleMoves[rand].Power;
            return possibleMoves[rand].Location;
        }


        public bool ChooseToDelayHunterWithFalseTipoff(GameState game)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.FalseTipoff) != null)
            {
                if (new Random().Next(0, (int)(NumberOfPossibleCurrentLocations * 0.75)) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public EncounterTile ChooseEncounterToResolveOnSearchingHunter(GameState game,
            List<EncounterTile> encounterTilesToResolve, Hunter hunterSearching)
        {
            if (encounterTilesToResolve.Count() == 1)
            {
                return encounterTilesToResolve.First();
            }
            int index = encounterTilesToResolve.FindIndex(enc => enc.Encounter == Encounter.Bats || enc.Encounter == Encounter.Fog || enc.Encounter == Encounter.Saboteur);
            if (index > -1)
            {
                return encounterTilesToResolve[index == 0 ? 1 : 0];
            }
            index = encounterTilesToResolve.FindIndex(enc => enc.Encounter == Encounter.NewVampire);
            if (index > -1)
            {
                return encounterTilesToResolve[index == 0 ? 1 : 0];
            }
            index = encounterTilesToResolve.FindIndex(enc => enc.Encounter == Encounter.Assassin || enc.Encounter == Encounter.MinionWithKnife || enc.Encounter == Encounter.MinionWithKnifeAndPistol || enc.Encounter == Encounter.MinionWithKnifeAndRifle);
            if (index > -1)
            {
                return encounterTilesToResolve[index == 0 ? 1 : 0];
            }
            return encounterTilesToResolve[new Random().Next(0, encounterTilesToResolve.Count())];
        }

        public List<int> ChooseWhichCatacombsCardsToDiscard(GameState game, Location destination)
        {
            var cardsToDiscard = new List<int>();
            for (var i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null && (new Random().Next(0, 6) == 0 || game.Dracula.Catacombs[i].DraculaCards.First().Location == destination))
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
            int index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.NewVampire);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.Ambush);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.DesecratedSoil);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.Saboteur);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.Fog);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.Bats);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.Assassin);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.MinionWithKnifeAndRifle);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.MinionWithKnifeAndPistol);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.MinionWithKnife);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.Spy);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.Hoax);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.Rats);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.Plague);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.Lightning);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.Peasants);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.Thief);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }
            index = game.Dracula.Trail[0].EncounterTiles.FindIndex(enc => enc.Encounter == Encounter.Wolves);
            if (index > -1)
            {
                return game.Dracula.Trail[0].EncounterTiles[index == 0 ? 1 : 0];
            }

            return new Random().Next(0, 2) == 0
                ? game.Dracula.Trail[0].EncounterTiles.First()
                : game.Dracula.Trail[0].EncounterTiles[1];
        }

        public bool ChooseToCancelEventWithDevilishPower(GameState game, Event eventBeingPlayedNow,
            Event eventInitiallyPlayed)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.DevilishPower) != null)
            {
                int randomChances = 4;
                if (eventInitiallyPlayed == Event.DevilishPower && game.HunterAlly != null & game.HunterAlly.Event == Event.SisterAgatha)
                {
                    randomChances = 0;
                }
                switch (eventInitiallyPlayed)
                {
                    case Event.Evasion: randomChances = (int)(NumberOfPossibleCurrentLocations / 3); break;
                    case Event.TimeRunsShort: randomChances = 3; break;
                    case Event.GoodLuck: randomChances = 2; break;
                    case Event.HiredScouts: randomChances = (int)((0.75 * (NumberOfPossibleCurrentLocations - 5)) * (0.75 * (NumberOfPossibleCurrentLocations - 5))); break;
                }
                if (new Random().Next(0, randomChances) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public Event ChooseEventToDiscard(GameState game)
        {
            EventCard eventCardToDiscard = null;
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.ControlStorms);
            }
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.Trap);
            }
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.RelentlessMinion);
            }
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.FalseTipoff);
            }
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.Roadblock);
            }
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.Rage);
            }
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.CustomsSearch);
            }
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.SensationalistPress);
            }
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.TimeRunsShort);
            }
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.UnearthlySwiftness);
            }
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.WildHorses);
            }
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.UnearthlySwiftness);
            }
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.Seduction);
            }
            if (eventCardToDiscard == null)
            {
                eventCardToDiscard = game.Dracula.EventHand.Find(card => card.Event == Event.DevilishPower);
            }
            return eventCardToDiscard.Event;
        }

        public Location ChooseStartLocation(GameState game)
        {
            var allLocations = Enumerations.GetAllLocations();
            List<Location> locationsToExclude = new List<Location>();
            foreach (Location location in allLocations)
            {
                if ((game.Map.TypeOfLocation(location) != LocationType.SmallCity && game.Map.TypeOfLocation(location) != LocationType.LargeCity) || game.HuntersAt(location).Any())
                {
                    locationsToExclude.Add(location);
                }
            }
            foreach (Location location in locationsToExclude)
            {
                allLocations.Remove(location);
            }
            List<int> distancesToNearestHunter = new List<int>();
            foreach (Location location in allLocations)
            {
                distancesToNearestHunter.Add(game.GetDistanceToClosestHunter(location, true));
            }
            int totalDistanceWeights = 0;
            for (int i = 0; i < distancesToNearestHunter.Count(); i++)
            {
                distancesToNearestHunter[i] *= distancesToNearestHunter[i];
                totalDistanceWeights += distancesToNearestHunter[i];
            }
            int randomNumber = new Random().Next(0, totalDistanceWeights);
            int count = 0;
            int index = -1;
            foreach (int distance in distancesToNearestHunter)
            {
                index++;
                count += distance;
                if (count > randomNumber)
                {
                    break;
                }
            }
            return allLocations[index];
        }

        public int ChooseToPutDroppedOffCardInCatacombs(GameState game, DraculaCardSlot cardDroppedOffTrail)
        {
            if (cardDroppedOffTrail.DraculaCards.First().Location != Location.Nowhere &&
                game.Map.TypeOfLocation(cardDroppedOffTrail.DraculaCards.First().Location) != LocationType.Sea &&
                game.Map.TypeOfLocation(cardDroppedOffTrail.DraculaCards.First().Location) != LocationType.Castle)
            {
                if (cardDroppedOffTrail.EncounterTiles.Find(enc => enc.Encounter == Encounter.Ambush || enc.Encounter == Encounter.DesecratedSoil || enc.Encounter == Encounter.NewVampire) != null)
                {
                    return -1;
                }
                if (new Random().Next(0, CATACOMBSCHANCE) == 0)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        if (game.Dracula.Catacombs[i] == null)
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        public Location ChooseBatsDestination(GameState game, Hunter hunterWithEncounter)
        {
            List<Location> possibleDestinations = game.Map.LocationsConnectedByRoadTo(game.Hunters[(int)hunterWithEncounter].CurrentLocation);
            List<int> distances = new List<int>();
            int totalDistanceWeights = 0;
            foreach (Location location in possibleDestinations)
            {
                distances.Add(game.DistanceByRoadOrSeaBetween(location, game.Dracula.CurrentLocation, false));
                totalDistanceWeights += distances.Last();
            }
            if (Strategy == Strategy.Sneaky)
            {
                int randomNumber = new Random().Next(0, totalDistanceWeights);
                int count = 0;
                int index = -1;
                foreach (int distance in distances)
                {
                    index++;
                    count += distance;
                    if (count > randomNumber)
                    {
                        return possibleDestinations[index];
                    }
                }
            }
            else if (Strategy == Strategy.Aggressive)
            {
                List<Location> shortList = new List<Location>();
                int shortestDistance = distances.First();
                foreach (int distance in distances)
                {
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                    }
                }
                int index = -1;
                foreach (int distance in distances)
                {
                    index++;
                    if (distance == shortestDistance)
                    {
                        shortList.Add(possibleDestinations[index]);
                    }
                }
                return shortList[new Random().Next(0, shortList.Count())];
            }
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
            if (game.Dracula.EncounterHand.Find(enc => enc.Encounter == Encounter.NewVampire) != null)
            {
                if (NumberOfPossibleCurrentLocations > 3)
                {
                    return game.Dracula.EncounterHand.Find(enc => enc.Encounter == Encounter.NewVampire);
                }
            }
            if (game.Dracula.EncounterHand.Find(enc => enc.Encounter == Encounter.Ambush) != null)
            {
                if (NumberOfPossibleCurrentLocations > 3)
                {
                    return game.Dracula.EncounterHand.Find(enc => enc.Encounter == Encounter.Ambush);
                }
            }
            if (game.Dracula.EncounterHand.Find(enc => enc.Encounter == Encounter.DesecratedSoil) != null)
            {
                if (NumberOfPossibleCurrentLocations > 2)
                {
                    return game.Dracula.EncounterHand.Find(enc => enc.Encounter == Encounter.DesecratedSoil);
                }
            }
            if (game.Dracula.EncounterHand.Find(enc => enc.Encounter == Encounter.Fog) != null)
            {
                if (NumberOfPossibleCurrentLocations < 4)
                {
                    return game.Dracula.EncounterHand.Find(enc => enc.Encounter == Encounter.Fog);
                }
            }
            if (game.Dracula.EncounterHand.Find(enc => enc.Encounter == Encounter.Bats) != null)
            {
                if (NumberOfPossibleCurrentLocations < 4)
                {
                    return game.Dracula.EncounterHand.Find(enc => enc.Encounter == Encounter.Bats);
                }
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
            if (enemyCombatCards.Contains(EnemyCombatCard.Claws) && (int)game.TimeOfDay < 4 && cardUsedLastRound != EnemyCombatCard.EscapeMan)
            {
                enemyTarget = huntersInvolved[new Random().Next(0, huntersInvolved.Count())].Hunter;
                return EnemyCombatCard.EscapeMan;
            }
            if (Strategy == Strategy.FleeToCastleDracula && game.Dracula.CurrentLocation != Location.CastleDracula)
            {
                List<EnemyCombatCard> escapeCards = new List<EnemyCombatCard>();
                foreach (EnemyCombatCard card in enemyCombatCards)
                {
                    if (card.Name().Contains("Escape") && cardUsedLastRound != card)
                    {
                        escapeCards.Add(card);
                    }
                }
                if (escapeCards.Any())
                {
                    enemyTarget = huntersInvolved[new Random().Next(0, huntersInvolved.Count())].Hunter;
                    return escapeCards[new Random().Next(0, escapeCards.Count())];
                }
            }
            if (!repelled)
            {
                if (enemyCombatCards.Contains(EnemyCombatCard.Fangs) || enemyCombatCards.Contains(EnemyCombatCard.Mesmerize))
                {
                    List<HunterPlayer> huntersVulnerableToBite = new List<HunterPlayer>();
                    foreach (HunterPlayer h in huntersInvolved)
                    {
                        if (h.NumberOfKnownItemsOfType(Item.Crucifix) == 0 && h.NumberOfKnownItemsOfType(Item.HeavenlyHost) == 0)
                        {
                            huntersVulnerableToBite.Add(h);
                        }
                        else if (h.NumberOfKnownItemsOfType(Item.Crucifix) == 1 && h.NumberOfKnownItemsOfType(Item.HeavenlyHost) == 0 && h.LastCombatCardChosen == Item.Crucifix)
                        {
                            huntersVulnerableToBite.Add(h);
                        }
                        else if (h.NumberOfKnownItemsOfType(Item.Crucifix) == 0 && h.NumberOfKnownItemsOfType(Item.HeavenlyHost) == 1 && h.LastCombatCardChosen == Item.HeavenlyHost)
                        {
                            huntersVulnerableToBite.Add(h);
                        }
                    }
                    enemyTarget = huntersVulnerableToBite[new Random().Next(0, huntersVulnerableToBite.Count())].Hunter;
                    if (cardUsedLastRound == EnemyCombatCard.Fangs)
                    {
                        return EnemyCombatCard.Mesmerize;
                    }
                    else if (enemyCombatCards.Contains(EnemyCombatCard.Fangs) && (!sisterAgathaInEffect || new Random().Next(0, 3) == 0))
                    {
                        return EnemyCombatCard.Fangs;
                    }
                }
                else if (enemyCombatCards.Contains(EnemyCombatCard.Strength) && cardUsedLastRound != EnemyCombatCard.Strength)
                {
                    List<HunterPlayer> huntersWithWeaponsToDestroy = new List<HunterPlayer>();
                    foreach (HunterPlayer h in huntersInvolved)
                    {
                        if (h.NumberOfKnownItemsOfType(Item.Crucifix) > 1 || (h.NumberOfKnownItemsOfType(Item.Crucifix) > 0 && h.LastCombatCardChosen != Item.Crucifix))
                        {
                            huntersWithWeaponsToDestroy.Add(h);
                        }
                        else if (h.NumberOfKnownItemsOfType(Item.Rifle) > 1 || (h.NumberOfKnownItemsOfType(Item.Rifle) > 0 && h.LastCombatCardChosen != Item.Rifle))
                        {
                            huntersWithWeaponsToDestroy.Add(h);
                        }
                        else if (h.NumberOfKnownItemsOfType(Item.Pistol) > 1 || (h.NumberOfKnownItemsOfType(Item.Pistol) > 0 && h.LastCombatCardChosen != Item.Pistol))
                        {
                            huntersWithWeaponsToDestroy.Add(h);
                        }
                        else if (h.NumberOfKnownItemsOfType(Item.Stake) > 1 || (h.NumberOfKnownItemsOfType(Item.Stake) > 0 && h.LastCombatCardChosen != Item.Stake))
                        {
                            huntersWithWeaponsToDestroy.Add(h);
                        }
                    }
                }
            }
            int numberOfAttempts = 2;
            do
            {
                numberOfAttempts--;
                cardChosen = enemyCombatCards[new Random().Next(0, enemyCombatCards.Count())];
            } while ((cardChosen == EnemyCombatCard.Dodge && firstRound) || cardChosen == cardUsedLastRound ||
                     (repelled &&
                      (cardChosen == EnemyCombatCard.Fangs || cardChosen == EnemyCombatCard.Mesmerize ||
                       cardChosen == EnemyCombatCard.Strength)) ||
                     (roundsWithoutEscape > 0) &&
                     (cardChosen == EnemyCombatCard.EscapeBat || cardChosen == EnemyCombatCard.EscapeMan ||
                      cardChosen == EnemyCombatCard.EscapeMist) || ((cardChosen == EnemyCombatCard.Claws || cardChosen == EnemyCombatCard.Punch)) && numberOfAttempts > 0);
            enemyTarget = huntersInvolved[new Random().Next(0, huntersInvolved.Count())].Hunter;
            return cardChosen;
        }

        public bool ChooseToCancelCharteredCarriageWithFalseTipoff(GameState game)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.FalseTipoff) != null)
            {
                if (new Random().Next(0, (int)(NumberOfPossibleCurrentLocations * 0.75)) == 0)
                {
                    return true;
                }
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
            if (!validPorts.Any())
            {
                return Location.Nowhere;
            }

            List<int> distances = new List<int>();
            if (Strategy == Strategy.Sneaky)
            {
                int totalDistanceWeights = 0;
                foreach (Location location in validPorts)
                {
                    int distance = game.GetDistanceToClosestHunter(location, false);
                    distances.Add(distance * distance);
                    totalDistanceWeights += distances.Last();
                }
                int randomNumber = new Random().Next(0, totalDistanceWeights);
                int count = 0;
                int index = -1;
                foreach (int dist in distances)
                {
                    index++;
                    count += dist;
                    if (count > randomNumber)
                    {
                        return validPorts[index];
                    }
                }
            }
            else if (Strategy == Strategy.Aggressive)
            {
                foreach (Location location in validPorts)
                {
                    int distance = game.GetDistanceToClosestHunter(location, false);
                }
                int shortestDistance = distances.First();
                foreach (int dist in distances)
                {
                    if (dist < shortestDistance)
                    {
                        shortestDistance = dist;
                    }
                }
                List<Location> shortList = new List<Location>();
                int index = -1;
                foreach (Location port in validPorts)
                {
                    index++;
                    if (distances[index] == shortestDistance)
                    {
                        shortList.Add(port);
                    }
                }
                return shortList[new Random().Next(0, shortList.Count())];
            }
            else if (Strategy == Strategy.FleeToCastleDracula)
            {
                foreach (Location location in validPorts)
                {
                    int distance = game.DistanceByRoadOrSeaBetween(location, Location.CastleDracula, false);
                }
                int shortestDistance = distances.First();
                foreach (int dist in distances)
                {
                    if (dist < shortestDistance)
                    {
                        shortestDistance = dist;
                    }
                }
                List<Location> shortList = new List<Location>();
                int index = -1;
                foreach (Location port in validPorts)
                {
                    index++;
                    if (distances[index] == shortestDistance)
                    {
                        shortList.Add(port);
                    }
                }
                return shortList[new Random().Next(0, shortList.Count())];
            }
            return validPorts[new Random().Next(0, validPorts.Count())];
        }

        public CardType ChooseToDiscardItemFromHunterInsteadOfEvent(HunterPlayer hunterDiscardingCard)
        {
            int chanceOfDiscardingGoodLuck = hunterDiscardingCard.NumberOfKnownEventsOfType(Event.GoodLuck) / hunterDiscardingCard.EventCount;
            int chanceOfDiscardingForewarned = hunterDiscardingCard.NumberOfKnownEventsOfType(Event.Forewarned) / hunterDiscardingCard.EventCount;
            int chanceOfDiscardingHeroicLeap = hunterDiscardingCard.NumberOfKnownEventsOfType(Event.HeroicLeap) / hunterDiscardingCard.EventCount;
            int chanceOfDiscardingStake = hunterDiscardingCard.NumberOfKnownItemsOfType(Item.Stake) / hunterDiscardingCard.ItemCount;
            int chanceOfDiscardingCrucifix = hunterDiscardingCard.NumberOfKnownItemsOfType(Item.Crucifix) / hunterDiscardingCard.ItemCount;
            int chanceOfDiscardingHeavenlyHost = hunterDiscardingCard.NumberOfKnownItemsOfType(Item.HeavenlyHost) / hunterDiscardingCard.ItemCount;
            if (chanceOfDiscardingCrucifix + chanceOfDiscardingForewarned + chanceOfDiscardingGoodLuck + chanceOfDiscardingHeavenlyHost + chanceOfDiscardingHeroicLeap + chanceOfDiscardingStake > 0)
            {
                if (chanceOfDiscardingCrucifix + chanceOfDiscardingHeavenlyHost + chanceOfDiscardingStake > chanceOfDiscardingForewarned + chanceOfDiscardingGoodLuck + chanceOfDiscardingHeroicLeap)
                {
                    return CardType.Item;
                }
                else
                {
                    return CardType.Event;
                }
            }
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

            int index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Wolves);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Thief);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Peasants);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Lightning);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Plague);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Rats);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Hoax);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Spy);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.MinionWithKnife);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.MinionWithKnifeAndPistol);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.MinionWithKnifeAndRifle);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Assassin);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Bats);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Fog);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Saboteur);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.DesecratedSoil);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Ambush);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.NewVampire);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }

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

            int index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.NewVampire && (int)game.TimeOfDay > 3);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Assassin);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.MinionWithKnifeAndRifle);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.MinionWithKnifeAndPistol);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.MinionWithKnife);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Spy);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Hoax);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Rats && !game.Hunters[(int)hunterToAmbush].HasDogsFaceUp);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Plague);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Lightning && game.Hunters[(int)hunterToAmbush].NumberOfKnownItemsOfType(Item.Crucifix) == 0 && game.Hunters[(int)hunterToAmbush].NumberOfKnownItemsOfType(Item.HeavenlyHost) == 0);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Peasants);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Thief && !game.Hunters[(int)hunterToAmbush].HasDogsFaceUp);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Wolves && game.Hunters[(int)hunterToAmbush].NumberOfKnownItemsOfType(Item.Pistol) == 0 && game.Hunters[(int)hunterToAmbush].NumberOfKnownItemsOfType(Item.Rifle) == 0);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Ambush);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.DesecratedSoil);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Saboteur && !game.Hunters[(int)hunterToAmbush].HasDogsFaceUp);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Fog);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
            index = game.Dracula.EncounterHand.FindIndex(enc => enc.Encounter == Encounter.Bats);
            if (index > -1)
            {
                return game.Dracula.EncounterHand[index];
            }
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
            if (eventsThatCanBePlayed.Any())
            {
                if (game.HunterAlly != null && game.HunterAlly.Event == Event.SisterAgatha && eventsThatCanBePlayed.Contains(Event.DevilishPower))
                {
                    devilishPowerTarget = ChooseDevilishPowerTarget(game);
                    roadBlock1 = roadBlock2 = Location.Nowhere;
                    roadBlockType = ConnectionType.None;
                    return Event.DevilishPower;
                }



                if (new Random().Next(0, 3) == 0)
                {
                    var eventChosen = eventsThatCanBePlayed[new Random().Next(0, eventsThatCanBePlayed.Count())];
                    switch (eventChosen)
                    {
                        case Event.Roadblock:
                            roadBlock1 = ChooseSetupForRoadBlock(game, out roadBlock2, out roadBlockType);
                            devilishPowerTarget = DevilishPowerTarget.None;
                            break;
                        case Event.DevilishPower:
                            if (new Random().Next(0, 3) != 0)
                            {
                                eventChosen = Event.None;
                            }
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
            }
            devilishPowerTarget = DevilishPowerTarget.None;
            roadBlock1 = roadBlock2 = Location.Nowhere;
            roadBlockType = ConnectionType.None;
            return Event.None;
        }

        private DevilishPowerTarget ChooseDevilishPowerTarget(GameState game)
        {
            if (game.HunterAlly != null)
            {
                return DevilishPowerTarget.HunterAlly;
            }
            var possibleTargets = new List<DevilishPowerTarget>();
            if (game.HeavenlyHostLocation1 != Location.Nowhere)
            {
                possibleTargets.Add(DevilishPowerTarget.HeavenlyHost1);
            }
            if (game.HeavenlyHostLocation2 != Location.Nowhere)
            {
                possibleTargets.Add(DevilishPowerTarget.HeavenlyHost2);
            }
            return possibleTargets[new Random().Next(0, possibleTargets.Count())];
        }

        private Location ChooseSetupForRoadBlock(GameState game, out Location roadBlock2,
            out ConnectionType roadBlockType)
        {
            // if agressive, block nearest hunter's retreat
            if (Strategy == Strategy.Aggressive)
            {
                // choose the closest hunter, or a random one of the closest ones
                List<HunterPlayer> closestHunters = game.HuntersClosestTo(game.Dracula.CurrentLocation);
                HunterPlayer target = closestHunters[new Random().Next(0, closestHunters.Count())];

                // choose a location connected to that hunter's location by road that is further away from dracula than their current location
                List<Location> connectedLocations = game.Map.LocationsConnectedByRoadTo(target.CurrentLocation);
                List<int> distances = new List<int>();
                int longestDistance = 0;
                foreach (Location location in connectedLocations)
                {
                    distances.Add(game.DistanceByRoadOrSeaBetween(location, game.Dracula.CurrentLocation, false));
                    if (distances.Last() > longestDistance)
                    {
                        longestDistance = distances.Last();
                    }
                }
                List<Location> shortlist = new List<Location>();
                int index = -1;
                foreach (int dist in distances)
                {
                    index++;
                    if (dist == longestDistance)
                    {
                        shortlist.Add(connectedLocations[index]);
                    }
                }

                // put the roadblock there
                roadBlock2 = shortlist[new Random().Next(0, shortlist.Count())];
                roadBlockType = ConnectionType.Road;
                return target.CurrentLocation;
            }
            else if (Strategy == Strategy.Sneaky && NumberOfPossibleCurrentLocations > 4)
            {
                // choose two hunters that are not in the same location
                int distanceBetweenLordGodalmingAndDrSeward = game.DistanceByRoadBetween(game.Hunters[(int)Hunter.LordGodalming].CurrentLocation, game.Hunters[(int)Hunter.DrSeward].CurrentLocation, true);
                int distanceBetweenLordGodalmingAndVanHelsing = game.DistanceByRoadBetween(game.Hunters[(int)Hunter.LordGodalming].CurrentLocation, game.Hunters[(int)Hunter.VanHelsing].CurrentLocation, true);
                int distanceBetweenLordGodalmingAndMinaHarker = game.DistanceByRoadBetween(game.Hunters[(int)Hunter.LordGodalming].CurrentLocation, game.Hunters[(int)Hunter.MinaHarker].CurrentLocation, true);
                int distanceBetweenDrSewardAndVanHelsing = game.DistanceByRoadBetween(game.Hunters[(int)Hunter.DrSeward].CurrentLocation, game.Hunters[(int)Hunter.VanHelsing].CurrentLocation, true);
                int distanceBetweenDrSewardAndMinaHarker = game.DistanceByRoadBetween(game.Hunters[(int)Hunter.DrSeward].CurrentLocation, game.Hunters[(int)Hunter.MinaHarker].CurrentLocation, true);
                int distanceBetweenVanHelsingAndMinaHarker = game.DistanceByRoadBetween(game.Hunters[(int)Hunter.VanHelsing].CurrentLocation, game.Hunters[(int)Hunter.MinaHarker].CurrentLocation, true);

                HunterPlayer firstHunter = null;
                HunterPlayer secondHunter = null;

                int shortestNonZeroDistance = 99;
                if (distanceBetweenLordGodalmingAndDrSeward > 0 && distanceBetweenLordGodalmingAndDrSeward < shortestNonZeroDistance)
                {
                    shortestNonZeroDistance = distanceBetweenLordGodalmingAndDrSeward;
                    firstHunter = game.Hunters[(int)Hunter.LordGodalming];
                    secondHunter = game.Hunters[(int)Hunter.DrSeward];
                }
                if (distanceBetweenLordGodalmingAndVanHelsing > 0 && distanceBetweenLordGodalmingAndVanHelsing < shortestNonZeroDistance)
                {
                    shortestNonZeroDistance = distanceBetweenLordGodalmingAndVanHelsing;
                    firstHunter = game.Hunters[(int)Hunter.LordGodalming];
                    secondHunter = game.Hunters[(int)Hunter.VanHelsing];
                }
                if (distanceBetweenLordGodalmingAndMinaHarker > 0 && distanceBetweenLordGodalmingAndMinaHarker < shortestNonZeroDistance)
                {
                    shortestNonZeroDistance = distanceBetweenLordGodalmingAndMinaHarker;
                    firstHunter = game.Hunters[(int)Hunter.LordGodalming];
                    secondHunter = game.Hunters[(int)Hunter.MinaHarker];
                }
                if (distanceBetweenDrSewardAndVanHelsing > 0 && distanceBetweenDrSewardAndVanHelsing < shortestNonZeroDistance)
                {
                    shortestNonZeroDistance = distanceBetweenDrSewardAndVanHelsing;
                    firstHunter = game.Hunters[(int)Hunter.DrSeward];
                    secondHunter = game.Hunters[(int)Hunter.VanHelsing];
                }
                if (distanceBetweenDrSewardAndMinaHarker > 0 && distanceBetweenDrSewardAndMinaHarker < shortestNonZeroDistance)
                {
                    shortestNonZeroDistance = distanceBetweenDrSewardAndMinaHarker;
                    firstHunter = game.Hunters[(int)Hunter.DrSeward];
                    secondHunter = game.Hunters[(int)Hunter.MinaHarker];
                }
                if (distanceBetweenVanHelsingAndMinaHarker > 0 && distanceBetweenVanHelsingAndMinaHarker < shortestNonZeroDistance)
                {
                    shortestNonZeroDistance = distanceBetweenVanHelsingAndMinaHarker;
                    firstHunter = game.Hunters[(int)Hunter.VanHelsing];
                    secondHunter = game.Hunters[(int)Hunter.MinaHarker];
                }
                if (firstHunter != null)
                {
                    // figure out a road that is between them
                    if (shortestNonZeroDistance == 1)
                    {
                        roadBlock2 = secondHunter.CurrentLocation;
                        roadBlockType = ConnectionType.Road;
                        return firstHunter.CurrentLocation;
                    }
                    if (shortestNonZeroDistance == 2)
                    {
                        List<Location> firstHuntersConnections = game.Map.LocationsConnectedByRoadTo(firstHunter.CurrentLocation);
                        List<Location> secondHuntersConnections = game.Map.LocationsConnectedByRoadTo(secondHunter.CurrentLocation);
                        List<Location> overlap = new List<Location>();
                        foreach (Location location in firstHuntersConnections)
                        {
                            if (secondHuntersConnections.Contains(location))
                            {
                                overlap.Add(location);
                            }
                        }
                        // block that road
                        roadBlock2 = overlap[new Random().Next(0, overlap.Count())];
                        roadBlockType = ConnectionType.Road;
                        if (new Random().Next(0, 1) == 0)
                        {
                            return firstHunter.CurrentLocation;
                        }
                        else
                        {
                            return secondHunter.CurrentLocation;
                        }
                    }
                }
            }
            else if ((Strategy == Strategy.Sneaky && NumberOfPossibleCurrentLocations < 5) || Strategy == Strategy.FleeToCastleDracula)
            {
                // choose the closest hunter, or a random one of the closest ones
                List<HunterPlayer> closestHunters = game.HuntersClosestTo(game.Dracula.CurrentLocation);
                HunterPlayer target = closestHunters[new Random().Next(0, closestHunters.Count())];

                // choose a location connected to that hunter's location by road that is closer to dracula than their current location
                List<Location> connectedLocations = game.Map.LocationsConnectedByRoadTo(target.CurrentLocation);
                List<int> distances = new List<int>();
                int shortestDistance = 100;
                foreach (Location location in connectedLocations)
                {
                    distances.Add(game.DistanceByRoadOrSeaBetween(location, game.Dracula.CurrentLocation, false));
                    if (distances.Last() < shortestDistance)
                    {
                        shortestDistance = distances.Last();
                    }
                }
                List<Location> shortlist = new List<Location>();
                int index = -1;
                foreach (int dist in distances)
                {
                    index++;
                    if (dist == shortestDistance)
                    {
                        shortlist.Add(connectedLocations[index]);
                    }
                }

                // put the roadblock there
                roadBlock2 = shortlist[new Random().Next(0, shortlist.Count())];
                roadBlockType = ConnectionType.Road;
                return target.CurrentLocation;
            }

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
            List<int> distances = new List<int>();
            int longestDistance = 0;
            foreach (Location location in possibleDestination)
            {
                distances.Add(game.DistanceByRoadOrSeaBetween(location, Location.CastleDracula, false));
                if (distances.Last() > longestDistance)
                {
                    longestDistance = distances.Last();
                }
            }

            List<Location> shortList = new List<Location>();
            int index = -1;
            foreach (Location location in possibleDestination)
            {
                index++;
                if (distances[index] == longestDistance)
                {
                    shortList.Add(location);
                }
            }
            return shortList[new Random().Next(0, shortList.Count())];
        }

        public bool ChooseToPlayWildHorses(GameState game)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.WildHorses) != null)
            {
                if (Strategy == Strategy.Aggressive)
                {
                    return false;
                }
                if (Strategy == Strategy.FleeToCastleDracula || Strategy == Strategy.Sneaky)
                {
                    return true;
                }
                if (new Random().Next(0, 3) == 0)
                {
                    return true;
                }
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
                ((hunterWasAtSea && hunterIsNowOnLand) || (hunterMovedAcrossLand && hunterCrossedBorder)))
            {
                if (game.Hunters[(int)hunterMoved].NumberOfKnownItemsOfType(Item.Crucifix) > 0)
                {
                    return true;
                }
                if (game.Hunters[(int)hunterMoved].NumberOfKnownItemsOfType(Item.HeavenlyHost) > 0)
                {
                    return true;
                }
                if (game.Hunters[(int)hunterMoved].NumberOfKnownItemsOfType(Item.Stake) > 0)
                {
                    return true;
                }
                if (new Random().Next(0, 5 - game.Hunters[(int)hunterMoved].ItemCount) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ChooseToPlaySeduction(GameState game)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.Seduction) != null && (int)game.TimeOfDay > 3)
            {
                return true;
            }
            return false;
        }

        public bool ChooseToPlayControlStorms(GameState game, Hunter hunterMoved)
        {
            int numberOfHuntersAtSea = 0;
            for (int i = 1; i < 5; i++)
            {
                if (game.Map.TypeOfLocation(game.Hunters[i].CurrentLocation) == LocationType.Sea)
                {
                    numberOfHuntersAtSea++;
                }
            }
                if (game.Dracula.EventHand.Find(card => card.Event == Event.ControlStorms) != null &&
                    game.Map.TypeOfLocation(game.Hunters[(int)hunterMoved].CurrentLocation) == LocationType.Sea &&
                    new Random().Next(0, numberOfHuntersAtSea) == 0)
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
            if (game.Dracula.EventHand.Find(card => card.Event == Event.Trap) != null)
            {
                if (opponent == Opponent.Dracula)
                {
                    return true;
                }
                if (new Random().Next(0, 2) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public Item ChooseItemToDiscardWithRage(GameState game, Hunter rageTarget)
        {
            if (game.Hunters[(int)rageTarget].ItemsKnownToDracula.Find(card => card.Item == Item.Crucifix) != null)
            {
                return Item.Crucifix;
            }
            if (game.Hunters[(int)rageTarget].ItemsKnownToDracula.Find(card => card.Item == Item.Stake) != null)
            {
                return Item.Stake;
            }
            if (game.Hunters[(int)rageTarget].ItemsKnownToDracula.Find(card => card.Item == Item.HeavenlyHost) != null)
            {
                return Item.HeavenlyHost;
            }
            if (game.Hunters[(int)rageTarget].ItemsKnownToDracula.Find(card => card.Item == Item.Dogs) != null)
            {
                return Item.Dogs;
            }
            if (game.Hunters[(int)rageTarget].ItemsKnownToDracula.Find(card => card.Item == Item.Rifle) != null)
            {
                return Item.Rifle;
            }
            if (game.Hunters[(int)rageTarget].ItemsKnownToDracula.Find(card => card.Item == Item.Pistol) != null)
            {
                return Item.Pistol;
            }
            if (game.Hunters[(int)rageTarget].ItemsKnownToDracula.Find(card => card.Item == Item.Knife) != null)
            {
                return Item.Knife;
            }
            
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
            if ((int)game.TimeOfDay < 4 || Strategy == Strategy.FleeToCastleDracula)
            {
                return Hunter.Nobody;
            }
            if (game.Dracula.EventHand.Find(card => card.Event == Event.Rage) != null)
            {
                List<HunterPlayer> huntersToTarget = new List<HunterPlayer>();

                foreach (HunterPlayer h in huntersInvolved)
                {
                    if (h.NumberOfKnownItemsOfType(Item.Crucifix) > 0)
                    {
                        huntersToTarget.Add(h);
                    }
                    if (h.NumberOfKnownItemsOfType(Item.Stake) > 0)
                    {
                        huntersToTarget.Add(h);
                    }
                }
                if (huntersToTarget.Any())
                {
                    return huntersToTarget[new Random().Next(0, huntersToTarget.Count())].Hunter;
                }
                if (new Random().Next(0, 2) == 0)
                {
                    return Hunter.Nobody;
                }
                foreach (HunterPlayer h in huntersInvolved)
                {
                    if (h.NumberOfKnownItemsOfType(Item.HeavenlyHost) > 0)
                    {
                        huntersToTarget.Add(h);
                    }
                    if (h.NumberOfKnownItemsOfType(Item.Knife) > 0)
                    {
                        huntersToTarget.Add(h);
                    }
                }
                if (huntersToTarget.Any())
                {
                    return huntersToTarget[new Random().Next(0, huntersToTarget.Count())].Hunter;
                }
                if (new Random().Next(0, 3) != 0)
                {
                    return Hunter.Nobody;
                }
                foreach (HunterPlayer h in huntersInvolved)
                {
                    if (h.ItemCount > 0)
                    {
                        huntersToTarget.Add(h);
                    }
                }
                if (huntersToTarget.Any())
                {
                    return huntersToTarget[new Random().Next(0, huntersToTarget.Count())].Hunter;
                }
            }
            return Hunter.Nobody;
        }

        public bool ChooseToPlaySensationalistPress(GameState game, Location location)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.SensationalistPress) != null)
            {
                int randomChances = (int)((0.75 * (NumberOfPossibleCurrentLocations - 5)) * (0.75 * (NumberOfPossibleCurrentLocations - 5)));
                if (new Random().Next(0, randomChances) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public HunterPlayer ChooseNightVisitVictim(List<HunterPlayer> bittenHunters)
        {
            int lowestHealth = bittenHunters.First().Health;
            foreach (HunterPlayer h in bittenHunters)
            {
                if (h.Health < lowestHealth)
                {
                    lowestHealth = h.Health;
                }
            }
            List<HunterPlayer> shortlist = new List<HunterPlayer>();
            foreach (HunterPlayer h in bittenHunters)
            {
                if (h.Health == lowestHealth)
                {
                    shortlist.Add(h);
                }
            }
            return shortlist[new Random().Next(0, shortlist.Count())];
        }

        public Event ChooseAllyToKeep(GameState game, Event existingAlly, Event newAlly)
        {
            if (newAlly == Event.QuinceyPMorris || newAlly == Event.QuinceyPMorris)
            {
                for (int i = 1; i < 5; i++)
                {
                    if (game.Hunters[i].NumberOfKnownItemsOfType(Item.Crucifix) == 0 && game.Hunters[i].NumberOfKnownItemsOfType(Item.HeavenlyHost) == 0)
                    {
                        return newAlly == Event.QuinceyPMorris ? newAlly : existingAlly;
                    }
                }
                return existingAlly == Event.QuinceyPMorris ? newAlly : existingAlly;
            }
            if (game.Dracula.EventHand.Count() > 5)
            {
                return existingAlly;
            }
            if (new Random().Next(0, 2) == 0)
            {
                return newAlly;
            }
            return existingAlly;
        }

        public HunterPlayer ChooseVampiricInfluenceVictim(List<HunterPlayer> bittenHunters)
        {
            int mostUnrevealedCards = bittenHunters.First().ItemCount + bittenHunters.First().EventCount - bittenHunters.First().ItemsKnownToDracula.Count() - bittenHunters.First().EventsKnownToDracula.Count();
            foreach (HunterPlayer h in bittenHunters)
            {
                if (h.ItemCount + h.EventCount - h.ItemsKnownToDracula.Count() - h.EventsKnownToDracula.Count() > mostUnrevealedCards)
                {
                    mostUnrevealedCards = h.ItemCount + h.EventCount - h.ItemsKnownToDracula.Count() - h.EventsKnownToDracula.Count();
                }
            }
            List<HunterPlayer> shortlist = new List<HunterPlayer>();
            foreach (HunterPlayer h in bittenHunters)
            {
                if (h.ItemCount + h.EventCount - h.ItemsKnownToDracula.Count() - h.EventsKnownToDracula.Count() == mostUnrevealedCards)
                {
                    shortlist.Add(h);
                }
            }
            return shortlist[new Random().Next(0, shortlist.Count())];
        }

        public Location ChooseWhereToEvadeTo(GameState game)
        {
            var allLocations = Enumerations.GetAllLocations();
            List<Location> locationsToExclude = new List<Location>();
            foreach (Location location in allLocations)
            {
                if ((game.Map.TypeOfLocation(location) != LocationType.SmallCity && game.Map.TypeOfLocation(location) != LocationType.LargeCity) || game.HuntersAt(location).Any() || game.Dracula.TrailContains(location))
                {
                    locationsToExclude.Add(location);
                }
            }
            foreach (Location location in locationsToExclude)
            {
                allLocations.Remove(location);
            }
            List<int> distancesToNearestHunter = new List<int>();
            foreach (Location location in allLocations)
            {
                distancesToNearestHunter.Add(game.GetDistanceToClosestHunter(location, true));
            }
            int totalDistanceWeights = 0;
            for (int i = 0; i < distancesToNearestHunter.Count(); i++)
            {
                distancesToNearestHunter[i] *= distancesToNearestHunter[i];
                totalDistanceWeights += distancesToNearestHunter[i];
            }
            int randomNumber = new Random().Next(0, totalDistanceWeights);
            int count = 0;
            int index = -1;
            foreach (int distance in distancesToNearestHunter)
            {
                index++;
                count += distance;
                if (count > randomNumber)
                {
                    break;
                }
            }
            return allLocations[index];
        }

        public Hunter ChooseVictimForQuinceyPMorris(GameState game)
        {
            List<HunterPlayer> potentialVictims = new List<HunterPlayer>();
            for (int i = 1; i < 5; i++)
            {
                if (game.Hunters[i].NumberOfKnownItemsOfType(Item.Crucifix) == 0 && game.Hunters[i].NumberOfKnownItemsOfType(Item.HeavenlyHost) == 0)
                {
                    potentialVictims.Add(game.Hunters[i]);
                }
            }
            if (potentialVictims.Any())
            {
                return potentialVictims[new Random().Next(0, potentialVictims.Count())].Hunter;
            }
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
            if (PossibilityTree.Count() == 0)
            {
                PossibilityTree.Add(GetActualTrail(game));
            }
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
            if (PossibilityTree.Count() == 0)
            {
                PossibilityTree.Add(GetActualTrail(game));
            }
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
            if (PossibilityTree.Count() == 0)
            {
                PossibilityTree.Add(GetActualTrail(game));
            }
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
            if (PossibilityTree.Count() == 0)
            {
                PossibilityTree.Add(GetActualTrail(game));
            }
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
                int stop = length;
                for (int i = 5; i >= stop; i--)
                {
                    if (trail[i] != null)
                    {
                        if (trail[i].Location != currentLocation)
                        {
                            trail[i] = null;
                        }
                        else
                        {
                            stop--;
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

        public void EliminateTrailsThatContainLocation(GameState game, Location location)
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
            if (PossibilityTree.Count() == 0)
            {
                PossibilityTree.Add(GetActualTrail(game));
            }
        }

        public void EliminateTrailsThatDoNotContainLocationAtPosition(GameState game, Location location, int position)
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
            if (PossibilityTree.Count() == 0)
            {
                PossibilityTree.Add(GetActualTrail(game));
            }
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
            if (PossibilityTree.Count() == 0)
            {
                PossibilityTree.Add(GetActualTrail(game));
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

        public void EliminateTrailsThatDoNotContainHideAtPosition(GameState game, int position)
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
            if (PossibilityTree.Count() == 0)
            {
                PossibilityTree.Add(GetActualTrail(game));
            }
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
            if (PossibilityTree.Count() == 0)
            {
                PossibilityTree.Add(GetActualTrail(game));
            }
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
            if (PossibilityTree.Count() == 0)
            {
                PossibilityTree.Add(GetActualTrail(game));
            }
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
            if (PossibilityTree.Count() == 0)
            {
                PossibilityTree.Add(GetActualTrail(game));
            }
        }

        public void EliminateTrailsThatDoNotContainLocation(GameState game, Location location)
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
            if (PossibilityTree.Count() == 0)
            {
                PossibilityTree.Add(GetActualTrail(game));
            }
        }

        public void EliminateTrailsThatContainLocationAtPosition(GameState game, Location location, int position)
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
            if (PossibilityTree.Count() == 0)
            {
                PossibilityTree.Add(GetActualTrail(game));
            }
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

        public List<PossibleTrailSlot> DetermineOrderedMoves(GameState game, List<PossibleTrailSlot[]> potentialityTree, int depth, int originatingSlot, out List<int> numberOfPossibilities)
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
            return DetermineOrderedMoves(game, newPotentialityTree, depth - 1, originatingSlot + 1, out numberOfPossibilities);
        }

        private int IndexOfLocationInTrail(PossibleTrailSlot[] trail, Location loc)
        {
            for (int i = 0; i < 6; i++)
            {
                if (trail[i].Location == loc)
                {
                    return i;
                }
            }
            return -1;
        }

        public void UpdateStrategy(GameState game)
        {
            int distanceToNearestHunter = game.GetDistanceToClosestHunter(game.Dracula.CurrentLocation, false) - 1;
            for (int i = 0; i < 6; i++)
            {
                if (game.Dracula.Trail[i] != null && game.Dracula.Trail[i].DraculaCards.Count() > 1 && game.Dracula.Trail[i].DraculaCards[1].Power == Power.WolfForm)
                {
                    distanceToNearestHunter++;
                }
            }
            // if it's dark and I can get to a Hunter before light, go aggressive
            int turnsUntilDark = 0;
            int turnsUntilLight = 0;
            if ((int)game.TimeOfDay > 3)
            {
                turnsUntilLight = 7 - (int)game.TimeOfDay;
            }
            if ((int)game.TimeOfDay < 4)
            {
                turnsUntilDark = 4 - (int)game.TimeOfDay;
            }
            if (turnsUntilLight > distanceToNearestHunter)
            {
                Strategy = Strategy.Aggressive;
            }
            else if (game.Dracula.Blood < 6)
            {
                Strategy = Strategy.FleeToCastleDracula;
            }
            else
            {
                Strategy = Strategy.Sneaky;
            }
        }
    }
}