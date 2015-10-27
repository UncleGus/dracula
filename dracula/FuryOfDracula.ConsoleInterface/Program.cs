﻿using FuryOfDracula.ArtificialIntelligence;
using FuryOfDracula.GameLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.ConsoleInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            GameState game = new GameState();
            if (args.Length > 0)
            {
                try
                {
                    game = new FileLoader().LoadGameState(args[0]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    Console.WriteLine("Where is {0} starting?", game.Hunters[i].Hunter.Name());
                    Location destination = Location.Nowhere;
                    string line = "";
                    while (destination == Location.Nowhere || (game.Map.TypeOfLocation(destination) != LocationType.SmallCity && game.Map.TypeOfLocation(destination) != LocationType.LargeCity))
                    {
                        line = Console.ReadLine();
                        destination = Enumerations.GetLocationFromString(line);
                        Console.WriteLine(destination.Name());
                        if (game.Map.TypeOfLocation(destination) != LocationType.SmallCity && game.Map.TypeOfLocation(destination) != LocationType.LargeCity)
                        {
                            Console.WriteLine("You must start in a city");
                        }
                    }
                    game.Hunters[i].MoveTo(destination);
                }
            }

            DecisionMaker logic = new DecisionMaker();
            game.Dracula.MoveTo(logic.ChooseStartLocation(game), Power.None);
            while (game.Dracula.EncounterHand.Count() < game.Dracula.EncounterHandSize)
            {
                game.Dracula.DrawEncounter(game.EncounterPool);
            }
            EndHunterTurn(game, logic);

            CommandSet commandSet = new CommandSet();

            do
            {
                DrawGameState(game);
                commandSet = GetCommandSet();

                switch (commandSet.command)
                {
                    case "c":
                    case "catch": CatchTrain(game, commandSet.argument1, logic); break;
                    case "e":
                    case "event": PlayEvent(game, commandSet.argument1, commandSet.argument2, logic); break;
                    case "u":
                    case "use": UseItem(game, commandSet.argument1, commandSet.argument2); break;
                    case "g":
                    case "group": SetupGroup(game, commandSet.argument1); break;
                    case "t":
                    case "take":
                        if (game.Dracula.TakeEvent(game.EventDeck))
                        {
                            game.Dracula.DiscardEvent(logic.ChooseEventToDiscard(game), game.EventDiscard);
                        }; break;
                    case "r":
                    case "reveal": RevealCardInTrailAtPosition(game, commandSet.argument1); break;
                    case "z":
                    case "end": EndHunterTurn(game, logic); break;
                    case "state": DisplayState(game); break;
                    case "discard": DiscardCard(game, commandSet.argument1, commandSet.argument2); break;
                    case "d":
                    case "draw": DrawCard(game, commandSet.argument1, commandSet.argument2); CheckForDiscardRequired(game); CheckForCardsRevealedForBeingBitten(game); break;
                    case "save": SaveGameState(game, commandSet.argument1); break;
                    case "load": game = new FileLoader().LoadGameState(commandSet.argument1); break;
                    case "m":
                    case "move": HandleMoveOperation(game, commandSet.argument1, commandSet.argument2, logic); break;
                    case "exit": Console.WriteLine("Fare well"); break;
                    default: Console.WriteLine("I didn't understand that"); break;
                }
            } while (commandSet.command != "exit");
        }

        private static void CatchTrain(GameState game, string hunterIndex, DecisionMaker logic)
        {
            Hunter hunterCatchingTrain = Hunter.Nobody;
            int index;
            if (Int32.TryParse(hunterIndex, out index))
            {
                hunterCatchingTrain = game.GetHunterFromInt(index);
            }
            string line = "";
            while (hunterCatchingTrain == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is catching the train? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (Int32.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    hunterCatchingTrain = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterCatchingTrain.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            if (DraculaIsPlayingFalseTipoffToDelayHunter(game, logic)) {
                Console.WriteLine("{0} is delayed over papers", hunterCatchingTrain.Name());
            } else
            {
                Console.WriteLine("{0} may catch a train", hunterCatchingTrain.Name());
            }
        }

        private static bool DraculaIsPlayingFalseTipoffToDelayHunter(GameState game, DecisionMaker logic)
        {
            if (logic.ChooseToDelayHunterWithFalseTipoff(game))
            {
                Console.WriteLine("Dracula is playing False Tip-off to delay the train");
                game.Dracula.DiscardEvent(Event.FalseTipoff, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.FalseTipoff, Event.CharteredCarriage, logic) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static void PlayEvent(GameState game, string eventName, string hunterIndex, DecisionMaker logic)
        {
            int index = 0;
            Hunter hunterPlayingEvent = Hunter.Nobody;
            if (Int32.TryParse(hunterIndex, out index))
            {
                hunterPlayingEvent = game.GetHunterFromInt(index);
            }
            string line = "";
            while (hunterPlayingEvent == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is playing an Event? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (Int32.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    hunterPlayingEvent = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterPlayingEvent.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            Event eventBeingPlayed = Enumerations.GetEventFromString(eventName);
            while (eventBeingPlayed == Event.None && line.ToLower() != "cancel")
            {
                Console.WriteLine("What is the name of the Event being played?");
                line = Console.ReadLine();
                eventBeingPlayed = Enumerations.GetEventFromString(line);
            }
            if (line.ToLower() == "cancel")
            {
                Console.WriteLine("Cancelled");
                return;
            }
            game.Hunters[(int)hunterPlayingEvent].DiscardEvent(game, eventBeingPlayed);
            if (DraculaIsPlayingDevilishPowerToCancelEvent(game, eventBeingPlayed, eventBeingPlayed, logic))
            {
                Console.WriteLine("{0} cancelled", eventBeingPlayed.Name());
                return;
            }
            switch (eventBeingPlayed)
            {
                case Event.JonathanHarker:
                case Event.RufusSmith:
                case Event.SisterAgatha:
                    PlayAlly(game, hunterPlayingEvent, eventBeingPlayed);
                    break;
                case Event.BloodTransfusion:
                    PlayBloodTransfusion(game, hunterPlayingEvent);
                    break;
                case Event.CharteredCarriage:
                    PlayCharteredCarriage(game, hunterPlayingEvent, logic);
                    break;
                case Event.ConsecratedGround:
                    PlayConsecratedGround(game, hunterPlayingEvent);
                    break;
                case Event.ExcellentWeather:
                    PlayExcellentWeather(game, hunterPlayingEvent);
                    break;
                case Event.GoodLuck:
                    PlayGoodLuck(game, hunterPlayingEvent);
                    break;
                case Event.HiredScouts:
                    PlayHiredScouts(game, hunterPlayingEvent);
                    break;
                case Event.Hypnosis:
                    PlayHypnosis(game, hunterPlayingEvent, logic);
                    break;
                case Event.LongDay:
                    PlayLongDay(game, hunterPlayingEvent);
                    break;
                case Event.MoneyTrail:
                    PlayMoneyTrail(game, hunterPlayingEvent);
                    break;
                case Event.MysticResearch:
                    PlayMysticResearch(game, hunterPlayingEvent);
                    break;
                case Event.NewspaperReports:
                    PlayNewsPaperReports(game, hunterPlayingEvent);
                    break;
                case Event.ReEquip:
                    PlayReEquip(game, hunterPlayingEvent);
                    break;
                case Event.SenseofEmergency:
                    PlaySenseOfEmergency(game, hunterPlayingEvent, logic);
                    break;
                case Event.StormySeas:
                    PlayStormySeas(game, hunterPlayingEvent, logic);
                    break;
                case Event.SurprisingReturn:
                    PlaySurprisingReturn(game, hunterPlayingEvent);
                    break;
                case Event.TelegraphAhead:
                    PlayTelegraphAhead(game, hunterPlayingEvent);
                    break;
                case Event.VampireLair:
                    PlayVampireLair(game, hunterPlayingEvent, logic);
                    break;
                default:
                    Console.WriteLine("It is not appropriate to play {0} at this time", eventBeingPlayed.Name());
                    break;
            }
        }

        private static void PlayVampireLair(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            if (game.Vampires < 1)
            {
                Console.WriteLine("No Vampires to battle");
                return;
            }
            if (ResolveCombat(game, new List<HunterPlayer> { game.Hunters[(int)hunterPlayingEvent] }, Opponent.NewVampire, logic))
            {
                game.AdjustVampires(-1);
            }
        }

        private static void PlayTelegraphAhead(GameState game, Hunter hunterPlayingEvent)
        {
            foreach (Location loc in game.Map.LocationsConnectedByRoadTo(game.Dracula.CurrentLocation))
            {
                game.Dracula.RevealCardInTrailWithLocation(loc);
            }
        }

        private static void PlaySurprisingReturn(GameState game, Hunter hunterPlayingEvent)
        {
            if (game.EventDiscard.Count == 0)
            {
                return;
            }
            Console.WriteLine("What is the name of the card you are taking from the Event discard pile?");
            Event eventRetrieved = Event.None;
            while (eventRetrieved == Event.None)
            {
                eventRetrieved = Enumerations.GetEventFromString(Console.ReadLine());
            }
            game.Hunters[(int)hunterPlayingEvent].DrawEventCard();
            EventCard eventCardRetrieved = game.EventDiscard.Find(card => card.Event == eventRetrieved);
            game.Hunters[(int)hunterPlayingEvent].EventsKnownToDracula.Add(eventCardRetrieved);
            game.EventDiscard.Remove(eventCardRetrieved);
        }

        private static void PlayStormySeas(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            Console.WriteLine("Where is {0} playing Stormy Seas?", hunterPlayingEvent.Name());
            Location stormySeasLocation = Location.Nowhere;
            while (stormySeasLocation == Location.Nowhere || game.Map.TypeOfLocation(stormySeasLocation) != LocationType.Sea)
            {
                stormySeasLocation = Enumerations.GetLocationFromString(Console.ReadLine());
            }
            game.StormySeasLocation = stormySeasLocation;
            game.StormySeasRounds = 2;
            if (stormySeasLocation == game.Dracula.CurrentLocation)
            {
                Console.WriteLine("Dracula was in {0}", stormySeasLocation.Name());
                Location destination = logic.ChoosePortToGoToAfterStormySeas(game);
                if (destination == Location.Nowhere)
                {
                    Console.WriteLine("Dracula cannot make a legal move");
                    return;
                }
                List<DraculaCardSlot> cardsDroppedOffTrail = new List<DraculaCardSlot>() { game.Dracula.MoveTo(destination, Power.None) };
                game.Dracula.RevealCardInTrailWithLocation(destination);
                DealWithDroppedOffCardSlots(game, cardsDroppedOffTrail, logic);
                CheckForJonathanHarker(game);
            }
        }

        private static void CheckForJonathanHarker(GameState game)
        {
            if (game.HunterAlly.Event == Event.JonathanHarker && game.Dracula.Trail[5] != null)
            {
                game.Dracula.RevealCardAtPosition(5);
            }
        }

        private static void DealWithDroppedOffCardSlots(GameState game, List<DraculaCardSlot> cardsDroppedOffTrail, DecisionMaker logic)
        {
            foreach (DraculaCardSlot cardDroppedOffTrail in cardsDroppedOffTrail)
            {
                if (cardDroppedOffTrail.DraculaCards.Count() > 1)
                {
                    cardDroppedOffTrail.DraculaCards.Remove(cardDroppedOffTrail.DraculaCards[1]);
                }
                int index = logic.ChooseToPutDroppedOffCardInCatacombs(game, cardDroppedOffTrail);
                if (index > -1)
                {
                    game.Dracula.Catacombs[index] = cardDroppedOffTrail;
                    game.Dracula.PlaceEncounterTileOnCard(logic.ChooseEncounterTileToPlaceOnDraculaCardSlot(game, game.Dracula.Catacombs[index]), cardDroppedOffTrail);
                }
                else
                {
                    cardDroppedOffTrail.DraculaCards.First().IsRevealed = false;
                    while (cardDroppedOffTrail.EncounterTiles.Count() > 0)
                    {
                        cardDroppedOffTrail.EncounterTiles.First().IsRevealed = false;
                        MatureEncounter(game, cardDroppedOffTrail.EncounterTiles.First(), logic);
                        cardDroppedOffTrail.EncounterTiles.Remove(cardDroppedOffTrail.EncounterTiles.First());
                    }
                }
                if (cardDroppedOffTrail.DraculaCards.First().Location == game.Dracula.LocationWhereHideWasUsed && game.Dracula.LocationWhereHideWasUsed != Location.Nowhere)
                {
                    List<EncounterTile> encountersToReturnToEncounterPool = game.Dracula.DiscardHide();
                    foreach (EncounterTile enc in encountersToReturnToEncounterPool)
                    {
                        enc.IsRevealed = false;
                        game.EncounterPool.Add(enc);
                    }
                    Console.WriteLine("The location where Dracula used Hide dropped off the trail, so the Hide card is also removed from the trail");
                }
            }
        }

        private static void PlaySenseOfEmergency(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            if (game.Hunters[(int)hunterPlayingEvent].Health <= 6 - game.Vampires)
            {
                Console.WriteLine("{0} does not have enough health", hunterPlayingEvent.Name());
                return;
            }
            Console.WriteLine("{0} loses {1} health", hunterPlayingEvent.Name(), 6 - game.Vampires);
            game.Hunters[(int)hunterPlayingEvent].AdjustHealth(game.Vampires - 6);
            Console.WriteLine("Where is {0} moving?");
            Location destination = Location.Nowhere;
            while (destination == Location.Nowhere)
            {
                destination = Enumerations.GetLocationFromString(Console.ReadLine());
            }
            HandleMoveOperation(game, ((int)hunterPlayingEvent).ToString(), destination.Name(), logic);
        }

        private static void PlayReEquip(GameState game, Hunter hunterPlayingEvent)
        {
            DiscardUnknownItemFromHunter(game, game.Hunters[(int)hunterPlayingEvent]);
            game.Hunters[(int)hunterPlayingEvent].DrawItemCard();
        }

        private static void PlayNewsPaperReports(GameState game, Hunter hunterPlayingEvent)
        {
            for (int i = 5; i > 0; i--)
            {
                if (game.Dracula.Trail[i] != null && game.Dracula.Trail[i].DraculaCards.First().Location != Location.Nowhere && game.Dracula.Trail[i].DraculaCards.First().Location != game.Dracula.CurrentLocation && !game.Dracula.Trail[i].DraculaCards.First().IsRevealed)
                {
                    game.Dracula.RevealCardAtPosition(i);
                    break;
                }
            }
        }

        private static void PlayMysticResearch(GameState game, Hunter hunterPlayingEvent)
        {
            Console.WriteLine("These are Dracula's Event cards: ");
            foreach (EventCard e in game.Dracula.EventHand)
            {
                Console.Write(" {0},", e.Event.Name());
            }
        }

        private static void PlayMoneyTrail(GameState game, Hunter hunterPlayingEvent)
        {
            for (int i = 5; i > 0; i--)
            {
                if (game.Dracula.Trail[i] != null && game.Map.TypeOfLocation(game.Dracula.Trail[i].DraculaCards.First().Location) == LocationType.Sea)
                {
                    game.Dracula.RevealCardAtPosition(i);
                }
            }
        }

        private static void PlayLongDay(GameState game, Hunter hunterPlayingEvent)
        {
            if (game.TimeOfDay == TimeOfDay.Dawn)
            {
                Console.WriteLine("You cannot play this card during Dawn. Take the card back.");
                EventCard longDayCard = game.EventDiscard.Find(card => card.Event == Event.LongDay);
                game.Hunters[(int)hunterPlayingEvent].EventsKnownToDracula.Add(longDayCard);
                game.EventDiscard.Remove(longDayCard);
                return;
            }
            game.RegressTimeTracker();
        }

        private static void PlayHypnosis(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            Console.WriteLine("Roll a die and enter the result");
            int dieRoll = 0;
            while (dieRoll == 0)
            {
                if (Int32.TryParse(Console.ReadLine(), out dieRoll))
                {
                    if (dieRoll < 1 || dieRoll > 7)
                    {
                        dieRoll = 0;
                    }
                }
            }
            switch (dieRoll)
            {
                case 1:
                case 2:
                    Console.WriteLine("No effect");
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                    game.Dracula.RevealCardInTrailWithLocation(game.Dracula.CurrentLocation);
                    game.Dracula.RevealAllVampires();
                    Power power = Power.None;
                    Location advanceMoveLocation = logic.ChooseDestinationAndPower(game, out power);
                    if (advanceMoveLocation == Location.Nowhere && power == Power.None)
                    {
                        Console.WriteLine("Dracula will have no legal moves next turn");
                    }
                    else
                    {
                        game.Dracula.AdvanceMoveLocation = advanceMoveLocation;
                        game.Dracula.AdvanceMovePower = power;
                        Console.WriteLine("Dracula's next move will be to {0}{1}", game.Dracula.AdvanceMoveLocation, power == Power.None ? "" : " using " + game.Dracula.AdvanceMovePower.Name());
                    }
                    break;
            }
        }

        private static void PlayHiredScouts(GameState game, Hunter hunterPlayingEvent)
        {
            Console.WriteLine("Name the first city");
            Location locationToReveal = Location.Nowhere;
            while (locationToReveal == Location.Nowhere)
            {
                locationToReveal = Enumerations.GetLocationFromString(Console.ReadLine());
            }
            int trailIndex = game.Dracula.RevealCardInTrailWithLocation(locationToReveal);
            if (trailIndex == -1)
            {
                trailIndex = game.Dracula.RevealCardInCatacombsWithLocation(locationToReveal);
            }
            if (trailIndex == -1)
            {
                Console.WriteLine("{0} is not in Dracula's trail or Catacombs", locationToReveal.Name());
            }
            else
            {
                Console.WriteLine("{0} revealed", locationToReveal.Name());
                game.Dracula.RevealEncountersAtPositionInTrail(game, trailIndex);
                DrawGameState(game);
            }
            locationToReveal = Location.Nowhere;
            Console.WriteLine("Name the second city");
            locationToReveal = Location.Nowhere;
            while (locationToReveal == Location.Nowhere)
            {
                locationToReveal = Enumerations.GetLocationFromString(Console.ReadLine());
            }
            trailIndex = game.Dracula.RevealCardInTrailWithLocation(locationToReveal);
            if (trailIndex == -1)
            {
                trailIndex = game.Dracula.RevealCardInCatacombsWithLocation(locationToReveal);
            }
            if (trailIndex == -1)
            {
                Console.WriteLine("{0} is not in Dracula's trail or Catacombs", locationToReveal.Name());
            }
            else
            {
                Console.WriteLine("{0} revealed", locationToReveal.Name());
                game.Dracula.RevealEncountersAtPositionInTrail(game, trailIndex);
                DrawGameState(game);
            }
        }

        private static void PlayGoodLuck(GameState game, Hunter hunterPlayingEvent)
        {
            Console.WriteLine("What would you like to discard? 0= Nothing (cancel), 1= Roadblock, 2= Dracula's Ally");
            int answer = -1;
            while (answer == -1)
            {
                if (Int32.TryParse(Console.ReadLine(), out answer))
                {
                    if (answer < 0 || answer > 2)
                    {
                        answer = -1;
                    }
                }
                switch (answer)
                {
                    case 0:
                        Console.WriteLine("Take the card back");
                        EventCard goodLuckCard = game.EventDiscard.Find(card => card.Event == Event.GoodLuck);
                        game.EventDiscard.Remove(goodLuckCard);
                        game.Hunters[(int)hunterPlayingEvent].EventsKnownToDracula.Add(goodLuckCard);
                        game.Hunters[(int)hunterPlayingEvent].DrawEventCard();
                        break;
                    case 1:
                        game.RoadBlockLocation1 = Location.Nowhere;
                        game.RoadBlockLocation2 = Location.Nowhere;
                        game.RoadBlockConnectionType = ConnectionType.None;
                        break;
                    case 2:
                        if (game.DraculaAlly != null)
                        {
                            game.EventDiscard.Add(game.DraculaAlly);
                            game.DraculaAlly = null;
                        }
                        break;
                }
            }
        }

        private static void PlayExcellentWeather(GameState game, Hunter hunterPlayingEvent)
        {
            Console.WriteLine("{0} may make up to four sea moves this turng", hunterPlayingEvent.Name());
        }

        private static void PlayConsecratedGround(GameState game, Hunter hunterPlayingEvent)
        {
            Console.WriteLine("Where would you like to place the Consecrated Ground marker?");
            Location ground = Location.Nowhere;
            while (ground == Location.Nowhere || ground == Location.CastleDracula || ground == Location.Galatz || ground == Location.Klausenburg || (game.Map.TypeOfLocation(ground) != LocationType.SmallCity && game.Map.TypeOfLocation(ground) != LocationType.LargeCity))
            {
                ground = Enumerations.GetLocationFromString(Console.ReadLine());
            }
            game.ConsecratedGroundLocation = ground;
        }

        private static void PlayCharteredCarriage(GameState game, Hunter hunterPlayingEvent, DecisionMaker logic)
        {
            if (DraculaIsPlayingFalseTipoffToCancelCharteredCarriage(game, logic))
            {
                Console.WriteLine("Chartered Carriage cancelled");
                return;
            }
            Console.WriteLine("{0} catches a fast train this turn", hunterPlayingEvent.Name());
        }

        private static void PlayBloodTransfusion(GameState game, Hunter hunterPlayingEvent)
        {
            Console.WriteLine("Who is receiving the transfusion? { 0}= { 1}, { 2}= { 3}, { 4}= { 5}, { 6}= { 7} (-1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
            int answer = 0;
            while (answer == 0)
            {
                if (Int32.TryParse(Console.ReadLine(), out answer))
                {
                    if (answer < 1 || answer > 4)
                    {
                        answer = 0;
                    }
                }
            }
            if ((Hunter)answer == hunterPlayingEvent)
            {
                Console.WriteLine("Who is giving the transfusion? { 0}= { 1}, { 2}= { 3}, { 4}= { 5}, { 6}= { 7} (-1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                answer = 0;
                while (answer == 0)
                {
                    if (Int32.TryParse(Console.ReadLine(), out answer))
                    {
                        if (answer < 1 || answer > 4)
                        {
                            answer = 0;
                        }
                    }
                }
                game.Hunters[(int)hunterPlayingEvent].AdjustBites(-1);
                game.Hunters[answer].AdjustHealth(-1);
                CheckForHunterDeath(game);
            }
            else
            {
                game.Hunters[(int)hunterPlayingEvent].AdjustHealth(-1);
                game.Hunters[answer].AdjustBites(-1);
                CheckForHunterDeath(game);
            }
        }

        private static void PlayAlly(GameState game, Hunter hunterPlayingEvent, Event allyBeingPlayed)
        {
            EventCard hunterAllyEventCard = game.Hunters[(int)hunterPlayingEvent].EventsKnownToDracula.Find(card => card.Event == allyBeingPlayed);
            if (hunterAllyEventCard == null)
            {
                hunterAllyEventCard = game.EventDeck.Find(card => card.Event == allyBeingPlayed);
            }
            if (game.HunterAlly.Event == Event.None)
            {
                game.HunterAlly = hunterAllyEventCard;
            }
            else
            {
                game.EventDiscard.Add(game.HunterAlly);
            }
        }

        private static bool DraculaIsPlayingFalseTipoffToCancelCharteredCarriage(GameState game, DecisionMaker logic)
        {
            if (logic.ChooseToCancelCharteredCarriageWithFalseTipoff(game))
            {
                Console.WriteLine("Dracula is playing False Tip-off to cancel Chartered Carriage");
                game.Dracula.DiscardEvent(Event.FalseTipoff, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.FalseTipoff, Event.CharteredCarriage, logic) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool DraculaIsPlayingDevilishPowerToCancelEvent(GameState game, Event eventBeingPlayedNow, Event eventInitiallyPlayed, DecisionMaker logic)
        {
            if (logic.ChooseToCancelEventWithDevilishPower(game, eventBeingPlayedNow, eventInitiallyPlayed))
            {
                Console.WriteLine("Dracula is playing Devilish Power to cancel {0}", eventBeingPlayedNow);
                game.Dracula.DiscardEvent(Event.DevilishPower, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.DevilishPower, eventInitiallyPlayed, logic) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static int HunterPlayingGoodLuckToCancelDraculaEvent(GameState game, Event draculaEventBeingPlayed, Event eventInitiallyPlayed, DecisionMaker logic)
        {
            Console.WriteLine("Will anyone play Good Luck to cancel {0}? 0= Nobody, {1}= {2}, {3}= {4}, {5}= {6}, {7}= {8}", draculaEventBeingPlayed.Name(), (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
            int answer = -1;
            while (answer == -1)
            {
                if (Int32.TryParse(Console.ReadLine(), out answer))
                {
                    if (answer < 0 || answer > 4)
                    {
                        answer = -1;
                    }
                }
            }
            if (answer > 0)
            {
                game.Hunters[answer].DiscardEvent(game, Event.GoodLuck);
                if (DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.GoodLuck, eventInitiallyPlayed, logic))
                {
                    game.Dracula.DiscardEvent(Event.DevilishPower, game.EventDiscard);
                    return 0;
                }
            }
            return answer;
        }

        private static void UseItem(GameState game, string itemName, string hunterIndex)
        {
            int index = 0;
            Hunter hunterUsingItem = Hunter.Nobody;
            if (Int32.TryParse(hunterIndex, out index))
            {
                hunterUsingItem = game.GetHunterFromInt(index);
            }
            string line = "";
            while (hunterUsingItem == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is using an Item? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (Int32.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    hunterUsingItem = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterUsingItem.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            Item itemBeingUsed = Enumerations.GetItemFromString(itemName);
            while (itemBeingUsed == Item.None && line.ToLower() != "cancel")
            {
                Console.WriteLine("What is the name of the Item being used?");
                line = Console.ReadLine();
                itemBeingUsed = Enumerations.GetItemFromString(line);
            }
            if (line.ToLower() == "cancel")
            {
                Console.WriteLine("Cancelled");
                return;
            }
            switch (itemBeingUsed)
            {
                case Item.Dogs:
                    game.Hunters[(int)hunterUsingItem].SetDogsFaceUp(true); AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)hunterUsingItem], Item.Dogs); break;
                case Item.LocalRumors:
                    int trailIndex = -1;
                    Console.WriteLine("In which trail position (1-6) or Catacombs position (7-9) would you like to reveal an Encounter?");
                    while (trailIndex == -1)
                    {
                        if (Int32.TryParse(Console.ReadLine(), out trailIndex))
                        {
                            if (trailIndex < 1 || trailIndex > 9)
                            {
                                trailIndex = -1;
                            }
                        }
                    }
                    if (trailIndex < 7)
                    {
                        game.Dracula.RevealEncountersAtPositionInTrail(game, trailIndex - 1);
                    }
                    else
                    {
                        if (game.Dracula.Catacombs[trailIndex - 7] != null && game.Dracula.Catacombs[trailIndex - 7].EncounterTiles.Count() > 1)
                        {
                            int encounterIndex = -1;
                            while (encounterIndex == -1)
                            {
                                Console.WriteLine("Which Encounter would you like to reveal? 1 or 2");
                                if (Int32.TryParse(Console.ReadLine(), out encounterIndex))
                                {
                                    if (encounterIndex < 1 || encounterIndex > 2)
                                    {
                                        encounterIndex = -1;
                                    }
                                }
                            }
                            game.Dracula.RevealEncounterAtPositionInTrail(game, trailIndex - 1, encounterIndex - 1);
                        }
                        else
                        {
                            game.Dracula.RevealEncountersAtPositionInTrail(game, trailIndex - 1);
                        }
                    }
                    game.Hunters[(int)hunterUsingItem].DiscardItem(game, Item.LocalRumors);
                    DrawGameState(game);
                    break;
                case Item.HolyWater:
                    UseHolyWaterOnHunter(game, hunterUsingItem);
                    game.Hunters[(int)hunterUsingItem].DiscardItem(game, Item.HolyWater);
                    break;
                case Item.FastHorse:
                    Console.WriteLine("{0} may travel two roads this turn", hunterUsingItem.Name());
                    game.Hunters[(int)hunterUsingItem].DiscardItem(game, Item.FastHorse);
                    break;
                case Item.HeavenlyHost:
                    Console.WriteLine("Heavenly Host placed in {0}", game.Hunters[(int)hunterUsingItem].CurrentLocation.Name());
                    PlaceHeavenlyHostIn(game, game.Hunters[(int)hunterUsingItem].CurrentLocation);
                    game.Hunters[(int)hunterUsingItem].DiscardItem(game, Item.HeavenlyHost);
                    break;
                default: Console.WriteLine("It is not appropriate to use {0} at this time", itemBeingUsed.Name()); break;
            }

        }

        private static void PlaceHeavenlyHostIn(GameState game, Location location)
        {
            if (game.HeavenlyHostLocation1 == Location.Nowhere)
            {
                game.HeavenlyHostLocation1 = location;
            }
            else if (game.HeavenlyHostLocation2 == Location.Nowhere)
            {
                game.HeavenlyHostLocation2 = location;
            }
            else
            {
                Console.WriteLine("Where are you moving a Heavenly Host token from? 1= {0}, 2= {1}", game.HeavenlyHostLocation1.Name(), game.HeavenlyHostLocation2.Name());
                int answer = 0;
                while (answer == 0)
                {
                    if (Int32.TryParse(Console.ReadLine(), out answer))
                    {
                        if (answer < 1 || answer > 2)
                        {
                            answer = 0;
                        }
                    }
                }
                switch (answer)
                {
                    case 1: game.HeavenlyHostLocation1 = location; break;
                    case 2: game.HeavenlyHostLocation2 = location; break;
                }
            }
        }

        private static void UseHolyWaterOnHunter(GameState game, Hunter hunterReceivingHolyWater)
        {
            if (hunterReceivingHolyWater == Hunter.MinaHarker)
            {
                Console.WriteLine("Mina's bite cannot be cured");
                return;
            }
            Console.WriteLine("Roll a die and enter the result");
            int dieRoll = 0;
            while (dieRoll == 0)
            {
                if (Int32.TryParse(Console.ReadLine(), out dieRoll))
                {
                    if (dieRoll < 1 || dieRoll > 7)
                    {
                        dieRoll = 0;
                    }
                }
            }
            switch (dieRoll)
            {
                case 1:
                    Console.WriteLine("{0} loses 2 health", hunterReceivingHolyWater.Name());
                    game.Hunters[(int)hunterReceivingHolyWater].AdjustHealth(-2);
                    CheckForHunterDeath(game);
                    break;
                case 2:
                case 3:
                case 4: Console.WriteLine("Nothing happens"); break;
                case 5:
                case 6:
                    Console.WriteLine("{0} is cured of a Bite", hunterReceivingHolyWater.Name());
                    game.Hunters[(int)hunterReceivingHolyWater].AdjustBites(-1);
                    break;
            }


        }

        /// <summary>
        /// Checks if a given item card is already known by Dracula to be in a given hunter's hand and adds it if not
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterRevealingCard">The Hunter revealing the card</param>
        /// <param name="itemBeingRevealed">The Item being revealed</param>
        /// <returns>The card corresponding to the Item revealed</returns>
        private static ItemCard AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(GameState game, HunterPlayer hunterRevealingCard, Item itemBeingRevealed)
        {
            ItemCard itemCardBeingRevealed = hunterRevealingCard.ItemsKnownToDracula.Find(card => card.Item == itemBeingRevealed);
            if (itemCardBeingRevealed == null)
            {
                itemCardBeingRevealed = game.ItemDeck.Find(card => card.Item == itemBeingRevealed);
                game.ItemDeck.Remove(itemCardBeingRevealed);
                hunterRevealingCard.ItemsKnownToDracula.Add(itemCardBeingRevealed);
            }
            return itemCardBeingRevealed;
        }

        /// <summary>
        /// Checks if a given event card is already known by Dracula to be in a given hunter's hand and adds it if not
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterRevealingCard">The Hunter revealing the card</param>
        /// <param name="eventBeingRevealed">The Event being revealed</param>
        /// <returns>The card corresponding to the Event revealed</returns>
        private static EventCard AddEventCardToDraculaKnownCardsIfNotAlreadyKnown(GameState game, HunterPlayer hunterRevealingCard, Event eventBeingRevealed)
        {
            EventCard eventCardBeingRevealed = hunterRevealingCard.EventsKnownToDracula.Find(card => card.Event == eventBeingRevealed);
            if (eventCardBeingRevealed == null)
            {
                eventCardBeingRevealed = game.EventDeck.Find(card => card.Event == eventBeingRevealed);
                game.EventDeck.Remove(eventCardBeingRevealed);
                hunterRevealingCard.EventsKnownToDracula.Add(eventCardBeingRevealed);
            }
            return eventCardBeingRevealed;
        }

        /// <summary>
        /// Checks all hunters to see if they are dead and adjusts them accordingly if they are
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <returns>True if a Hunter has died</returns>
        private static bool CheckForHunterDeath(GameState game)
        {
            bool hunterDied = false;
            foreach (HunterPlayer h in game.Hunters)
            {
                if (h != null && (h.Health == 0 || h.BiteCount == h.BitesRequiredToKill))
                {
                    Console.WriteLine("{0} has been defeated. All Items and Events discarded. Hunter moved to St. Joseph and St. Mary and must skip the next turn");
                    while (h.ItemCount > 0)
                    {
                        DiscardUnknownItemFromHunter(game, h);
                    }
                    while (h.EventCount > 0)
                    {
                        DiscardUnknownEventFromHunter(game, h);
                    }
                    h.MoveTo(Location.StJosephAndStMary);
                    h.AdjustHealth(h.MaxHealth);
                    h.AdjustBites(-3);
                    if (h.Hunter == Hunter.MinaHarker)
                    {
                        h.AdjustBites(1);
                    }
                    hunterDied = true;
                }
            }
            return hunterDied;
        }

        /// <summary>
        /// Asks the user to name an Item and then discards it from that hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunter">The Hunter discarding an Item</param>
        private static void DiscardUnknownItemFromHunter(GameState game, HunterPlayer hunter)
        {
            Console.WriteLine("Name the Item being discarded");
            Item itemBeingDiscarded = Item.None;
            while (itemBeingDiscarded == Item.None)
            {
                itemBeingDiscarded = Enumerations.GetItemFromString(Console.ReadLine());
            }
            hunter.DiscardItem(game, itemBeingDiscarded);
        }

        /// <summary>
        /// Asks the user to name an Event and then discards it from that hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunter">The Hunter discarding an Event</param>
        private static void DiscardUnknownEventFromHunter(GameState game, HunterPlayer hunter)
        {
            Console.WriteLine("Name the Event being discarded");
            Event eventBeingDiscarded = Event.None;
            while (eventBeingDiscarded == Event.None)
            {
                eventBeingDiscarded = Enumerations.GetEventFromString(Console.ReadLine());
            }
            hunter.DiscardEvent(game, eventBeingDiscarded);
        }

        /// <summary>
        /// Checks all hunters' cards and bites to see if they should be revealing a card to Dracula
        /// </summary>
        /// <param name="game">The GameState</param>
        private static void CheckForCardsRevealedForBeingBitten(GameState game)
        {
            foreach (HunterPlayer h in game.Hunters)
            {
                if (h != null)
                {
                    if (h.ItemCount > 0 && h.BiteCount > 0 && h.ItemShownToDraculaForBeingBitten == null)
                    {
                        Item itemToReveal = Item.None;
                        Console.WriteLine("{0} is bitten and does not have an Item revealed to Dracula. Please name the Item you are revealing", h.Hunter.Name());
                        while (itemToReveal == Item.None)
                        {
                            itemToReveal = Enumerations.GetItemFromString(Console.ReadLine());
                            if (itemToReveal == Item.None)
                            {
                                Console.WriteLine("I couldn't find an Item with that name");
                            }
                        }
                        ItemCard itemCardToReveal = AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, h, itemToReveal);
                        h.ItemShownToDraculaForBeingBitten = itemCardToReveal;
                        Console.WriteLine("{0} revealed to Dracula", itemCardToReveal.Item.Name());
                    }
                    if (h.EventCount > 0 && h.BiteCount > 0 && h.EventShownToDraculaForBeingBitten == null)
                    {
                        Event eventToReveal = Event.None;
                        Console.WriteLine("{0} is bitten and does not have an Event revealed to Dracula. Please name the Event you are revealing", h.Hunter.Name());
                        while (eventToReveal == Event.None)
                        {
                            eventToReveal = Enumerations.GetEventFromString(Console.ReadLine());
                            if (eventToReveal == Event.None)
                            {
                                Console.WriteLine("I couldn't find an Item with that name");
                            }
                        }
                        EventCard eventCardToReveal = AddEventCardToDraculaKnownCardsIfNotAlreadyKnown(game, h, eventToReveal);
                        h.EventShownToDraculaForBeingBitten = eventCardToReveal;
                        Console.WriteLine("{0} revealed to Dracula", eventCardToReveal.Event.Name());
                    }
                }
            }
        }

        /// <summary>
        /// Handles the entirety of a hunter's move, including search, encounters, combat etc.
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterIndex">The index of the Hunter (and group) to move</param>
        /// <param name="destination">The destination of the move</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void HandleMoveOperation(GameState game, string hunterIndex, string destination, DecisionMaker logic)
        {
            Hunter hunterMoved = Hunter.Nobody;
            Location originatingLocation;
            int trailIndex = MoveHunter(game, hunterIndex, destination, out hunterMoved, out originatingLocation, logic);
            if (DraculaIsPlayingCustomsSearch(game, hunterMoved, originatingLocation, logic))
            {
                foreach (Hunter h in game.Hunters[(int)hunterMoved].HuntersInGroup)
                {
                    Console.WriteLine("{0} must discard all Items", h.Name());
                    while (game.Hunters[(int)h].ItemCount > 0)
                    {
                        DiscardUnknownItemFromHunter(game, game.Hunters[(int)h]);
                    }
                }
            }
            if (trailIndex > -1)
            {
                game.Dracula.RevealCardAtPosition(trailIndex);
                game.Dracula.RevealEncountersAtPositionInTrail(game, trailIndex);
                DraculaCardSlot slotBeingRevealed;
                if (trailIndex < 6)
                {
                    slotBeingRevealed = game.Dracula.Trail[trailIndex];
                }
                else
                {
                    slotBeingRevealed = game.Dracula.Catacombs[trailIndex - 6];
                }
                if (slotBeingRevealed.DraculaCards.First().Location == game.Dracula.LocationWhereHideWasUsed)
                {
                    int positionRevealed = game.Dracula.RevealHideCard();
                    game.Dracula.RevealEncountersAtPositionInTrail(game, positionRevealed);
                }
                if (slotBeingRevealed.DraculaCards.First().Location == game.Dracula.CurrentLocation)
                {
                    Console.WriteLine("Dracula is here!");
                }
                else
                {
                    Console.WriteLine("Dracula has been here");
                }
                List<EncounterTile> encounterTilesToResolve = new List<EncounterTile>();
                foreach (EncounterTile enc in slotBeingRevealed.EncounterTiles)
                {
                    encounterTilesToResolve.Add(enc);
                }
                bool hunterCanContinueToResolveEncounters = true;
                while (encounterTilesToResolve.Count() > 0 && hunterCanContinueToResolveEncounters)
                {
                    EncounterTile encounterTileBeingResolved = logic.ChooseEncounterToResolveOnSearchingHunter(game, encounterTilesToResolve);
                    hunterCanContinueToResolveEncounters = ResolveEncounterTile(game, encounterTileBeingResolved, game.Hunters[(int)hunterMoved].HuntersInGroup, logic, trailIndex);
                    encounterTilesToResolve.Remove(encounterTileBeingResolved);
                }
                if (slotBeingRevealed.DraculaCards.First().Location == game.Dracula.CurrentLocation && hunterCanContinueToResolveEncounters)
                {
                    List<HunterPlayer> huntersAttacking = new List<HunterPlayer>();
                    foreach (Hunter h in game.Hunters[(int)hunterMoved].HuntersInGroup)
                    {
                        huntersAttacking.Add(game.Hunters[(int)h]);
                    }
                    Console.WriteLine("Entering combat with Dracula");
                    ResolveCombat(game, huntersAttacking, Opponent.Dracula, logic);
                    CheckForHunterDeath(game);
                }
            }
        }

        private static bool DraculaIsPlayingCustomsSearch(GameState game, Hunter hunterMoved, Location origin, DecisionMaker logic)
        {
            bool haveCard = game.Dracula.EventHand.Find(card => card.Event == Event.CustomsSearch) != null;
            bool hunterWasAtSea = game.Map.TypeOfLocation(origin) == LocationType.Sea;
            bool hunterIsNowOnLand = game.Map.TypeOfLocation(game.Hunters[(int)hunterMoved].CurrentLocation) == LocationType.SmallCity || game.Map.TypeOfLocation(game.Hunters[(int)hunterMoved].CurrentLocation) == LocationType.LargeCity;
            bool hunterMovedAcrossLand = game.Map.TypeOfLocation(game.Hunters[(int)hunterMoved].CurrentLocation) != LocationType.Sea && game.Map.TypeOfLocation(origin) != LocationType.Sea;
            bool hunterCrossedBorder = game.Map.IsEastern(origin) != game.Map.IsEastern(game.Hunters[(int)hunterMoved].CurrentLocation);
            bool destinationHasEncounters = game.Dracula.NumberOfEncountersAtLocation(game.Hunters[(int)hunterMoved].CurrentLocation) > 0;
            if (haveCard && !destinationHasEncounters && ((hunterWasAtSea && hunterIsNowOnLand) || (hunterMovedAcrossLand && hunterCrossedBorder)) && new Random().Next(0, 2) == 0)
            {
                Console.WriteLine("Dracula is playing Customs Search");
                game.Dracula.DiscardEvent(Event.CustomsSearch, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.CustomsSearch, Event.CustomsSearch, logic) > -1)
                {
                    Console.WriteLine("Customs Search cancelled");
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds and removes people from the given hunter's group
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterIndex">The index of the hunter with whom to set up a group</param>
        private static void SetupGroup(GameState game, string hunterIndex)
        {
            Hunter hunterFormingGroup = Hunter.Nobody;
            int index;
            if (Int32.TryParse(hunterIndex, out index))
            {
                hunterFormingGroup = game.GetHunterFromInt(index);
            }
            string line = "";
            while (hunterFormingGroup == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is forming a group? {0}= {1}, {2}= {3}, {4}= {5} (Mina Harker cannot lead a group, -1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name());
                line = Console.ReadLine();
                if (Int32.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    if (index == 4)
                    {
                        Console.WriteLine("Mina Harker cannot lead a group, add her to someone else's group instead");
                    }
                    else
                    {
                        hunterFormingGroup = game.GetHunterFromInt(index);
                        Console.WriteLine(hunterFormingGroup.Name());
                    }
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            while (index != -1)
            {
                Console.WriteLine("These are the people in {0}'s group:", hunterFormingGroup.Name());
                foreach (Hunter h in game.Hunters[(int)hunterFormingGroup].HuntersInGroup)
                {
                    if (h != hunterFormingGroup)
                    {
                        Console.WriteLine(h.Name());
                    }
                }
                Hunter hunterToAddOrRemove = Hunter.Nobody;
                while (hunterToAddOrRemove == Hunter.Nobody && index != -1)
                {
                    Console.WriteLine("Who is joining or leaving {0}'s group? {1}= {2}, {3}= {4}, {5}= {6} (Lord Godalming must lead any group he is in, -1 to cancel)", hunterFormingGroup.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                    line = Console.ReadLine();
                    if (Int32.TryParse(line, out index))
                    {
                        if (index == -1)
                        {
                            Console.WriteLine("Cancelled");
                            return;
                        }
                        if (index == 1)
                        {
                            Console.WriteLine("Lord Godalming must lead any group he is in");
                        }
                        else
                        {
                            hunterToAddOrRemove = game.GetHunterFromInt(index);
                            Console.WriteLine(hunterFormingGroup.Name());
                        }
                    }
                    else
                    {
                        Console.WriteLine("I didn't understand that");
                    }
                }
                if ((int)hunterToAddOrRemove < (int)hunterFormingGroup)
                {
                    Console.WriteLine("{0} cannot join {1}'s group, instead add {1} to {0}'s group", hunterToAddOrRemove.Name(), hunterFormingGroup.Name());
                }
                else if (hunterToAddOrRemove == hunterFormingGroup)
                {
                    Console.WriteLine("{0} is already in his own group, of course!", hunterFormingGroup.Name());
                }
                else if (game.Hunters[(int)hunterFormingGroup].HuntersInGroup.Contains(hunterToAddOrRemove))
                {
                    game.Hunters[(int)hunterFormingGroup].HuntersInGroup.Remove(hunterToAddOrRemove);
                }
                else
                {
                    game.Hunters[(int)hunterFormingGroup].HuntersInGroup.Add(hunterToAddOrRemove);
                }
            }
        }

        /// <summary>
        /// Matures encounters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="encounterTileBeingMatured">The EncounterTile being matured</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void MatureEncounter(GameState game, EncounterTile encounterTileBeingMatured, DecisionMaker logic)
        {
            switch (encounterTileBeingMatured.Encounter)
            {
                case Encounter.Ambush:
                    Console.WriteLine("Dracula matured Ambush");
                    Hunter hunterToAmbush = logic.ChooseHunterToAmbush(game);
                    if (hunterToAmbush != Hunter.Nobody)
                    {
                        EncounterTile encounterToAmbushWith = logic.ChooseEncounterTileToAmbushHunterWith(game, hunterToAmbush);
                        ResolveEncounterTile(game, encounterToAmbushWith, new List<Hunter> { hunterToAmbush }, logic, -1);
                        game.Dracula.DiscardEncounterTile(game, encounterToAmbushWith);
                        game.EncounterPool.Add(encounterToAmbushWith);
                    }
                    break;
                case Encounter.DesecratedSoil:
                    Console.WriteLine("Dracula matured Desecrated Soil");
                    game.Dracula.ClearTrailDownTo(game, 3);
                    for (int i = 0; i < 2; i++)
                    {
                        Console.WriteLine("Draw an Event card. If it is a Hunter card, name it, otherwise type \"Dracula\"");
                        string line = "";
                        Event eventDrawn = Event.None;
                        do
                        {
                            line = Console.ReadLine();
                            eventDrawn = Enumerations.GetEventFromString(line);
                        } while ((eventDrawn == Event.None || !game.EventDeck.Find(card => card.Event == eventDrawn).IsDraculaCard) && !"dracula".StartsWith(line.ToLower()));
                        if (eventDrawn == Event.None)
                        {
                            game.Dracula.TakeEvent(game.EventDeck);
                            if (game.Dracula.EventHand.Count() > game.Dracula.EventHandSize)
                            {
                                game.Dracula.DiscardEvent(logic.ChooseEventToDiscard(game), game.EventDiscard);
                            }
                        }
                        else
                        {
                            EventCard eventCardDiscarded = game.EventDeck.Find(card => card.Event == eventDrawn);
                            game.EventDeck.Remove(eventCardDiscarded);
                            game.EventDiscard.Add(eventCardDiscarded);
                        }
                    }
                    break;
                case Encounter.NewVampire:
                    Console.WriteLine("Dracula matured a New Vampire");
                    game.AdjustVampires(2);
                    game.Dracula.ClearTrailDownTo(game, 1);
                    break;
            }
            game.EncounterPool.Add(encounterTileBeingMatured);
        }

        /// <summary>
        /// Resolves an encounter on a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="encounterTileBeingResolved">The Encounter being resolved</param>
        /// <param name="trailIndex">The index of the trail slot (0-5), or catacombs slot (6-8) where the encounter is being resolved</param>
        /// <returns></returns>
        private static bool ResolveEncounterTile(GameState game, EncounterTile encounterTileBeingResolved, List<Hunter> huntersInvolved, DecisionMaker logic, int trailIndex)
        {
            Console.WriteLine("Resolving {0} with {1}{2}", encounterTileBeingResolved.Encounter.Name(), huntersInvolved.First().Name(), huntersInvolved.Count() > 1 ? " and his group" : "");
            bool continueEncounters = true;
            bool discardEncounterTile = true;
            foreach (Hunter h in huntersInvolved)
            {
                Console.WriteLine("Will {0} play Secret Weapon?", h.Name());
                string line = "";
                do
                {
                    line = Console.ReadLine();
                } while (!"yes".StartsWith(line.ToLower()) && !"no".StartsWith(line.ToLower()));
                if ("yes".StartsWith(line.ToLower()))
                {
                    game.Hunters[(int)h].DiscardEvent(game, Event.SecretWeapon);
                    if (DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.SecretWeapon, Event.SecretWeapon, logic))
                    {
                        Console.WriteLine("Secret Weapon cancelled");
                    }
                    else
                    {
                        DiscardUnknownItemFromHunter(game, game.Hunters[(int)h]);
                        Console.WriteLine("What is the name of the Item being retrieved from the discard pile?");
                        Item itemRetrieved = Item.None;
                        while (itemRetrieved == Item.None)
                        {
                            itemRetrieved = Enumerations.GetItemFromString(Console.ReadLine());
                        }
                        ItemCard itemCardRetrieved = game.ItemDiscard.Find(card => card.Item == itemRetrieved);
                        game.ItemDiscard.Remove(itemCardRetrieved);
                        game.Hunters[(int)h].ItemsKnownToDracula.Add(itemCardRetrieved);
                        game.Hunters[(int)h].DrawItemCard();
                    }
                }
            }

            foreach (Hunter h in huntersInvolved)
            {
                Console.WriteLine("Will {0} play Forewarned?", h.Name());
                string line = "";
                do
                {
                    line = Console.ReadLine();
                } while (!"yes".StartsWith(line.ToLower()) && !"no".StartsWith(line.ToLower()));
                if ("yes".StartsWith(line.ToLower()))
                {
                    game.Hunters[(int)h].DiscardEvent(game, Event.Forewarned);
                    if (DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.SecretWeapon, Event.SecretWeapon, logic))
                    {
                        Console.WriteLine("Forewarned cancelled");
                    }
                    else
                    {
                        Console.WriteLine("Encounter cancelled");
                        game.EncounterPool.Add(encounterTileBeingResolved);
                        if (trailIndex > -1)
                        {
                            game.Dracula.Trail[trailIndex].EncounterTiles.Remove(encounterTileBeingResolved);
                        }
                        else
                        {
                            game.Dracula.EncounterHand.Remove(encounterTileBeingResolved);
                        }
                        return true;
                    }
                }
            }

            switch (encounterTileBeingResolved.Encounter)
            {
                case Encounter.Ambush:
                    continueEncounters = ResolveAmbush(game, huntersInvolved, logic); break;
                case Encounter.Assassin:
                    continueEncounters = ResolveAssassin(game, huntersInvolved, logic); break;
                case Encounter.Bats:
                    discardEncounterTile = false;
                    game.Hunters[(int)huntersInvolved.First()].EncountersInFrontOfPlayer.Add(encounterTileBeingResolved);
                    if (trailIndex > -1)
                    {
                        game.Dracula.Trail[trailIndex].EncounterTiles.Remove(encounterTileBeingResolved);
                    }
                    else
                    {
                        game.Dracula.EncounterHand.Remove(encounterTileBeingResolved);
                    }
                    return false;
                case Encounter.DesecratedSoil:
                    continueEncounters = ResolveDesecratedSoil(game, huntersInvolved, logic); break;
                case Encounter.Fog:
                    discardEncounterTile = false;
                    game.Hunters[(int)huntersInvolved.First()].EncountersInFrontOfPlayer.Add(encounterTileBeingResolved);
                    if (trailIndex > -1)
                    {
                        game.Dracula.Trail[trailIndex].EncounterTiles.Remove(encounterTileBeingResolved);
                    }
                    else
                    {
                        game.Dracula.EncounterHand.Remove(encounterTileBeingResolved);
                    }
                    return false;
                case Encounter.MinionWithKnife:
                    continueEncounters = ResolveMinionWithKnife(game, huntersInvolved, logic); break;
                case Encounter.MinionWithKnifeAndPistol:
                    continueEncounters = ResolveMinionWithKnifeAndPistol(game, huntersInvolved, logic); break;
                case Encounter.MinionWithKnifeAndRifle:
                    continueEncounters = ResolveMinionWithKnifeAndRifle(game, huntersInvolved, logic); break;
                case Encounter.Hoax:
                    continueEncounters = ResolveHoax(game, huntersInvolved); break;
                case Encounter.Lightning:
                    continueEncounters = ResolveLightning(game, huntersInvolved); break;
                case Encounter.Peasants:
                    continueEncounters = ResolvePeasants(game, huntersInvolved); break;
                case Encounter.Plague:
                    continueEncounters = ResolvePlague(game, huntersInvolved); break;
                case Encounter.Rats:
                    continueEncounters = ResolveRats(game, huntersInvolved); break;
                case Encounter.Saboteur:
                    continueEncounters = ResolveSaboteur(game, huntersInvolved); break;
                case Encounter.Spy:
                    continueEncounters = ResolveSpy(game, huntersInvolved); break;
                case Encounter.Thief:
                    continueEncounters = ResolveThief(game, huntersInvolved, logic); break;
                case Encounter.NewVampire:
                    continueEncounters = ResolveNewVampire(game, huntersInvolved, logic, out discardEncounterTile); break;
                case Encounter.Wolves:
                    continueEncounters = ResolveWolves(game, huntersInvolved); break;
                default: return false;
            }
            if (discardEncounterTile)
            {
                game.EncounterPool.Add(encounterTileBeingResolved);
                if (trailIndex > -1)
                {
                    game.Dracula.Trail[trailIndex].EncounterTiles.Remove(encounterTileBeingResolved);
                }
                else
                {
                    game.Dracula.EncounterHand.Remove(encounterTileBeingResolved);
                }
            }
            return continueEncounters;
        }

        /// <summary>
        /// Resolves an encounter of Wolves for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveWolves(GameState game, List<Hunter> huntersInvolved)
        {
            bool groupHasPistol = false;
            bool groupHasRifle = false;
            foreach (Hunter h in huntersInvolved)
            {
                Console.WriteLine("What Items does {0} have? 0= nothing, 1= Pistol, 2=Rifle, 3=Both", h.Name());
                int answer = -1;
                while (answer == -1)
                {
                    if (!(Int32.TryParse(Console.ReadLine(), out answer) && answer > -1 && answer < 4))
                    {
                        answer = -1;
                    }
                }
                switch (answer)
                {
                    case 1:
                        groupHasPistol = true;
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], Item.Pistol);
                        break;
                    case 2:
                        groupHasRifle = true;
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], Item.Rifle);
                        break;
                    case 3:
                        groupHasPistol = true; groupHasRifle = true;
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], Item.Pistol);
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], Item.Rifle);
                        break;
                }
                if (groupHasPistol && groupHasRifle)
                {
                    break;
                }
            }
            if (groupHasPistol && groupHasRifle)
            {
                Console.WriteLine("Encounter neutralised");
            }
            else
            {
                foreach (Hunter h in huntersInvolved)
                {
                    Console.WriteLine("{0} loses {1} health", h.Name(), ((groupHasPistol ? 0 : 1) + (groupHasRifle ? 0 : 1)));
                }
                if (CheckForHunterDeath(game))
                {
                    return false;
                }

            }
            return true;
        }

        /// <summary>
        /// Resolves an encounter of New Vampire for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveNewVampire(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic, out bool discardNewVampire)
        {
            switch (game.TimeOfDay)
            {
                case TimeOfDay.Dawn:
                case TimeOfDay.Noon:
                case TimeOfDay.Dusk:
                    Console.WriteLine("New Vampire encountered during the day and disposed of");
                    discardNewVampire = true;
                    return true;
            }
            Console.WriteLine("Roll a die and enter the result");
            int dieRoll = 0;
            while (dieRoll == 0)
            {
                if (!(Int32.TryParse(Console.ReadLine(), out dieRoll) && dieRoll > 0 && dieRoll < 7))
                {
                    dieRoll = 0;
                }
            }
            switch (dieRoll)
            {
                case 1:
                case 2:
                case 3:
                    Console.WriteLine("The Vampire attempts to bite you");
                    bool groupHasHolyItem = false;
                    foreach (Hunter h in huntersInvolved)
                    {
                        Console.WriteLine("What Items does {0} have? 0= nothing, 1= Crucifix, 2= Heavenly Host", h.Name());
                        Item holyItem = Item.None;
                        int answer = -1;
                        while (answer == -1)
                        {
                            if (Int32.TryParse(Console.ReadLine(), out answer))
                            {
                                if (answer > -1 && answer < 3)
                                {
                                    switch (answer)
                                    {
                                        case 0: holyItem = Item.None; break;
                                        case 1: holyItem = Item.Crucifix; break;
                                        case 2: holyItem = Item.HeavenlyHost; break;
                                    }
                                }
                                else
                                {
                                    answer = -1;
                                }
                            }
                        }
                        if (holyItem != Item.None)
                        {
                            groupHasHolyItem = true;
                            AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], holyItem);
                            break;
                        }
                    }
                    if (groupHasHolyItem)
                    {
                        Console.WriteLine("Bite attempted negated by holy item");
                        discardNewVampire = true;
                        return true;
                    }
                    else
                    {
                        foreach (Hunter h in huntersInvolved)
                        {
                            Console.WriteLine("{0} is bitten!");
                            if (!HunterPlayingGreatStrengthToCancelBite(game, h, logic))
                            {
                                game.Hunters[(int)h].AdjustBites(1);
                            }
                            discardNewVampire = true;
                            return true;
                        }
                        if (CheckForHunterDeath(game))
                        {
                            discardNewVampire = true;
                            return false;
                        }
                    }
                    break;
                case 4:
                case 5:
                case 6:
                    Console.WriteLine("The Vampire attempts to escape");
                    bool groupHasSharpItem = false;
                    foreach (Hunter h in huntersInvolved)
                    {
                        Console.WriteLine("What Items does {0} have? 0= nothing, 1= Knife, 2= Stake", h.Name());
                        Item sharpItem = Item.None;
                        int answer = -1;
                        while (answer == -1)
                        {
                            if (Int32.TryParse(Console.ReadLine(), out answer))
                            {
                                if (answer > -1 && answer < 3)
                                {
                                    switch (answer)
                                    {
                                        case 0: sharpItem = Item.None; break;
                                        case 1: sharpItem = Item.Knife; break;
                                        case 2: sharpItem = Item.Stake; break;
                                    }
                                }
                                else
                                {
                                    answer = -1;
                                }
                            }
                        }
                        if (sharpItem != Item.None)
                        {
                            groupHasSharpItem = true;
                            game.Hunters[(int)h].DiscardItem(game, sharpItem);
                            break;
                        }
                    }
                    if (groupHasSharpItem)
                    {
                        Console.WriteLine("Escape attempted negated by sharp weapon");
                        discardNewVampire = true;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("The Vampire escapes and remains in the city");
                        discardNewVampire = false;
                        return true;
                    }
            }
            discardNewVampire = true;
            return true;
        }

        /// <summary>
        /// Resolves an encounter of Thief for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveThief(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            foreach (Hunter h in huntersInvolved)
            {
                if (game.Hunters[(int)h].HasDogsFaceUp)
                {
                    return true;
                }
            }
            foreach (Hunter h in huntersInvolved)
            {
                CardType typeOfCardToDiscard = logic.ChooseToDiscardItemFromHunterInsteadOfEvent(game.Hunters[(int)h]);
                if (typeOfCardToDiscard != CardType.None)
                {
                    Console.WriteLine("Dracula has decided to discard an {0} from {1}", typeOfCardToDiscard, h.Name());
                }
            }
            Console.WriteLine("Use the standard discard command to tell me what was discarded");
            return true;
        }

        /// <summary>
        /// Resolves an encounter of Spy for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveSpy(GameState game, List<Hunter> huntersInvolved)
        {
            foreach (Hunter h in huntersInvolved)
            {
                if (game.Hunters[(int)h].HasDogsFaceUp)
                {
                    return true;
                }
            }
            foreach (Hunter h in huntersInvolved)
            {
                Console.WriteLine("{0} must reveal all of {1} cards and declare {1} next move", h.Name(), h == Hunter.MinaHarker ? "her" : "his");
                game.ItemDeck.AddRange(game.Hunters[(int)h].ItemsKnownToDracula);
                game.Hunters[(int)h].ItemsKnownToDracula.Clear();
                for (int i = 0; i < game.Hunters[(int)h].ItemCount; i++)
                {
                    Console.WriteLine("Name the Item being revealed to Dracula");
                    Item itemBeingRevealed = Item.None;
                    while (itemBeingRevealed == Item.None)
                    {
                        itemBeingRevealed = Enumerations.GetItemFromString(Console.ReadLine());
                    }
                    AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], itemBeingRevealed);
                }
                game.EventDeck.AddRange(game.Hunters[(int)h].EventsKnownToDracula);
                game.Hunters[(int)h].EventsKnownToDracula.Clear();
                for (int i = 0; i < game.Hunters[(int)h].EventCount; i++)
                {
                    Console.WriteLine("Name the Event being revealed to Dracula");
                    Event eventBeingRevealed = Event.None;
                    while (eventBeingRevealed == Event.None)
                    {
                        eventBeingRevealed = Enumerations.GetEventFromString(Console.ReadLine());
                    }
                    AddEventCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], eventBeingRevealed);
                }
                Console.WriteLine("What will be {0}'s destination next turn?", h.Name());
                Location destination = Location.Nowhere;
                while (destination == Location.Nowhere)
                {
                    destination = Enumerations.GetLocationFromString(Console.ReadLine());
                }
                Console.WriteLine("What will be {0}'s means of travel next turn?", h.Name());
                ConnectionType methodOfTravel = ConnectionType.None;
                while (methodOfTravel == ConnectionType.None)
                {
                    methodOfTravel = Enumerations.GetConnectionTypeFromString(Console.ReadLine());
                }
                game.Hunters[(int)h].SetNextMoveDestination(destination);
                game.Hunters[(int)h].SetNextMoveConnectionType(methodOfTravel);
            }
            return true;
        }

        /// <summary>
        /// Resolves an encounter of Saboteur for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveSaboteur(GameState game, List<Hunter> huntersInvolved)
        {
            foreach (Hunter h in huntersInvolved)
            {
                if (game.Hunters[(int)h].HasDogsFaceUp)
                {
                    return true;
                }
            }
            foreach (Hunter h in huntersInvolved)
            {
                Item itemDiscarded = Item.None;
                Event eventDiscarded = Event.None;
                string line = "";
                if (game.Hunters[(int)h].ItemCount > 0)
                {
                    if (game.Hunters[(int)h].EventCount > 0)
                    {
                        Console.WriteLine("{0} must discard 1 Item or Event. What is the name of the card being discarded?", h.Name());
                        while (itemDiscarded == Item.None && eventDiscarded == Event.None)
                        {
                            line = Console.ReadLine();
                            itemDiscarded = Enumerations.GetItemFromString(line);
                            eventDiscarded = Enumerations.GetEventFromString(line);
                        }
                        if (itemDiscarded != Item.None)
                        {
                            game.Hunters[(int)h].DiscardItem(game, itemDiscarded);
                        }
                        else
                        {
                            game.Hunters[(int)h].DiscardEvent(game, eventDiscarded);
                        }
                    }
                    else
                    {
                        Console.WriteLine("{0} must discard 1 Item. What is the name of the card being discarded?", h.Name());
                        while (itemDiscarded == Item.None)
                        {
                            line = Console.ReadLine();
                            itemDiscarded = Enumerations.GetItemFromString(line);
                        }
                        game.Hunters[(int)h].DiscardItem(game, itemDiscarded);
                    }
                }
                else if (game.Hunters[(int)h].EventCount > 0)
                {
                    Console.WriteLine("{0} must discard 1 Event. What is the name of the card being discarded?", h.Name());
                    while (eventDiscarded == Event.None)
                    {
                        line = Console.ReadLine();
                        eventDiscarded = Enumerations.GetEventFromString(line);
                    }
                    game.Hunters[(int)h].DiscardEvent(game, eventDiscarded);
                }
            }
            return false;
        }

        /// <summary>
        /// Resolves an encounter of Rats for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveRats(GameState game, List<Hunter> huntersInvolved)
        {
            foreach (Hunter h in huntersInvolved)
            {
                if (game.Hunters[(int)h].HasDogsFaceUp)
                {
                    return true;
                }
                Console.WriteLine("How much health does {0} now have?");
                int amount = -1;
                while (amount == -1)
                {
                    if (Int32.TryParse(Console.ReadLine(), out amount))
                    {
                        game.Hunters[(int)h].AdjustHealth(amount - game.Hunters[(int)h].Health);
                        break;
                    }
                }
            }
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Resolves an encounter of Plague for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolvePlague(GameState game, List<Hunter> huntersInvolved)
        {
            foreach (Hunter h in huntersInvolved)
            {
                game.Hunters[(int)h].AdjustHealth(-2);
            }
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Resolves an encounter of Peasants for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolvePeasants(GameState game, List<Hunter> huntersInvolved)
        {
            if (!game.Map.IsEastern(game.Hunters[(int)huntersInvolved.First()].CurrentLocation))
            {
                foreach (Hunter h in huntersInvolved)
                {
                    if (game.Hunters[(int)h].ItemCount > 0)
                    {
                        Console.WriteLine("What Item is {0} discarding?", h.Name());
                        Item itemDiscarded = Item.None;
                        while (itemDiscarded == Item.None)
                        {
                            itemDiscarded = Enumerations.GetItemFromString(Console.ReadLine());
                        }
                        game.Hunters[(int)h].DiscardItem(game, itemDiscarded);
                        Console.WriteLine("{0} discarded. Draw another item when this encounter is over.", itemDiscarded.Name());
                    }
                }
            }
            else
            {
                foreach (Hunter h in huntersInvolved)
                {
                    while (game.Hunters[(int)h].ItemCount > 0)
                    {
                        Console.WriteLine("What Item is {0} discarding?", h.Name());
                        Item itemDiscarded = Item.None;
                        while (itemDiscarded == Item.None)
                        {
                            itemDiscarded = Enumerations.GetItemFromString(Console.ReadLine());
                        }
                        game.Hunters[(int)h].DiscardItem(game, itemDiscarded);
                        Console.WriteLine("{0} discarded.", itemDiscarded.Name());
                    }
                    Console.WriteLine("Redraw all Item cards from the Item deck when this encounter is over.");
                }
            }
            return true;
        }

        /// <summary>
        /// Resolves an encounter of Lightning for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveLightning(GameState game, List<Hunter> huntersInvolved)
        {
            int answer = 0;
            foreach (Hunter h in huntersInvolved)
            {
                Console.WriteLine("What Item does {0} have? 0= Nothing, 1= Crucifix, 2= Heavenly Host", h.Name());
                if (Int32.TryParse(Console.ReadLine(), out answer) && answer > 0 && answer < 3)
                {
                    if (answer == 1)
                    {
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], Item.Crucifix);
                        break;
                    }
                    else if (answer == 2)
                    {
                        AddItemCardToDraculaKnownCardsIfNotAlreadyKnown(game, game.Hunters[(int)h], Item.HeavenlyHost);
                        break;
                    }
                }
            }
            if (answer == 0)
            {
                foreach (Hunter h in huntersInvolved)
                {
                    game.Hunters[(int)h].AdjustHealth(-2);
                    if (game.Hunters[(int)h].ItemCount > 0)
                    {
                        Console.WriteLine("What Item is {0} discarding?", h.Name());
                        Item itemDiscarded = Item.None;
                        while (itemDiscarded == Item.None)
                        {
                            itemDiscarded = Enumerations.GetItemFromString(Console.ReadLine());
                        }
                        game.Hunters[(int)h].DiscardItem(game, itemDiscarded);
                        Console.WriteLine("{0} discarded", itemDiscarded.Name());
                        CheckForCardsRevealedForBeingBitten(game);
                    }
                }
            }
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Resolves an encounter of a Hoax for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveHoax(GameState game, List<Hunter> huntersInvolved)
        {
            foreach (Hunter h in huntersInvolved)
            {
                if (game.Map.IsEastern(game.Hunters[(int)h].CurrentLocation) && game.Hunters[(int)h].EventCount > 0)
                {
                    Console.WriteLine("{0} must discard an Event. What Event is being discarded?");
                    string line = "";
                    Event eventToDiscard = Event.None;
                    while (eventToDiscard == Event.None)
                    {
                        line = Console.ReadLine();
                        eventToDiscard = Enumerations.GetEventFromString(line);
                    }
                    game.Hunters[(int)h].DiscardEvent(game, eventToDiscard);
                }
                else
                {
                    Console.WriteLine("{0} must discard all Events.");
                    while (game.Hunters[(int)h].EventCount > 0)
                    {
                        Console.WriteLine("What Event is being discarded?");
                        string line = "";
                        Event eventToDiscard = Event.None;
                        while (eventToDiscard == Event.None)
                        {
                            line = Console.ReadLine();
                            eventToDiscard = Enumerations.GetEventFromString(line);
                        }
                        game.Hunters[(int)h].DiscardEvent(game, eventToDiscard);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Resolves an encounter of a Minion With Knife And Rifle for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the combat</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveMinionWithKnifeAndRifle(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            List<HunterPlayer> huntersInCombat = new List<HunterPlayer>();
            foreach (Hunter h in huntersInvolved)
            {
                huntersInCombat.Add(game.Hunters[(int)h]);
            }
            bool continueEncounters = ResolveCombat(game, huntersInCombat, Opponent.MinionWithKnifeAndRifle, logic);
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            return continueEncounters;
        }

        /// <summary>
        /// Resolves an encounter of a Minion With Knife And Pistol for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the combat</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveMinionWithKnifeAndPistol(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            List<HunterPlayer> huntersInCombat = new List<HunterPlayer>();
            foreach (Hunter h in huntersInvolved)
            {
                huntersInCombat.Add(game.Hunters[(int)h]);
            }
            bool continueEncounters = ResolveCombat(game, huntersInCombat, Opponent.MinionWithKnifeAndPistol, logic);
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            return continueEncounters;
        }

        /// <summary>
        /// Resolves an encounter of a Minion With Knife for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the combat</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveMinionWithKnife(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            List<HunterPlayer> huntersInCombat = new List<HunterPlayer>();
            foreach (Hunter h in huntersInvolved)
            {
                huntersInCombat.Add(game.Hunters[(int)h]);
            }
            bool continueEncounters = ResolveCombat(game, huntersInCombat, Opponent.MinionWithKnife, logic);
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            return continueEncounters;
        }

        /// <summary>
        /// Resolves an encounter of Desecrated Soil for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveDesecratedSoil(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            Console.WriteLine("Draw an Event card. If it is a Hunter card, name it, otherwise type \"Dracula\"");
            string line = "";
            Event eventDrawn = Event.None;
            do
            {
                line = Console.ReadLine();
                eventDrawn = Enumerations.GetEventFromString(line);
            } while (eventDrawn == Event.None && !"dracula".StartsWith(line.ToLower()));
            if (eventDrawn == Event.None)
            {
                game.Dracula.TakeEvent(game.EventDeck);
                if (game.Dracula.EventHand.Count() > game.Dracula.EventHandSize)
                {
                    game.Dracula.DiscardEvent(logic.ChooseEventToDiscard(game), game.EventDiscard);
                }
            }
            else
            {
                EventCard eventCardDiscarded = game.EventDeck.Find(card => card.Event == eventDrawn);
                game.EventDeck.Remove(eventCardDiscarded);
                game.EventDiscard.Add(eventCardDiscarded);
            }
            return true;
        }

        /// <summary>
        /// Resolves an encounter of Assassin for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the combat</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveAssassin(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            List<HunterPlayer> huntersInCombat = new List<HunterPlayer>();
            foreach (Hunter h in huntersInvolved)
            {
                huntersInCombat.Add(game.Hunters[(int)h]);
            }
            bool continueEncounters = ResolveCombat(game, huntersInCombat, Opponent.Assassin, logic);
            if (CheckForHunterDeath(game))
            {
                return false;
            }
            return continueEncounters;
        }

        /// <summary>
        /// Resolves an encounter of Ambush for a group of hunters
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of Hunters involved in the encounter</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether or not the Hunters can continue resolving encounters</returns>
        private static bool ResolveAmbush(GameState game, List<Hunter> huntersInvolved, DecisionMaker logic)
        {
            game.Dracula.DrawEncounter(game.EncounterPool);
            game.Dracula.DiscardEncounterTile(game, logic.ChooseEncounterTileToDiscardFromEncounterHand(game));
            return true;
        }

        /// <summary>
        /// Checks if any of the hunters should discard a card
        /// </summary>
        /// <param name="game">The GameState</param>
        private static void CheckForDiscardRequired(GameState game)
        {
            for (int i = 1; i < 5; i++)
            {
                if (game.Hunters[i].Hunter == Hunter.DrSeward)
                {
                    if (game.Hunters[i].ItemCount > 4 && game.Hunters[i].EventCount > 4)
                    {
                        Console.WriteLine("{0} must discard an Item or an Event", game.Hunters[i].Hunter.Name());
                    }
                    else
                    {

                        if (game.Hunters[i].ItemCount > 5)
                        {
                            Console.WriteLine("{0} must discard an Item", game.Hunters[i].Hunter.Name());
                        }
                        if (game.Hunters[i].EventCount > 5)
                        {
                            Console.WriteLine("{0} must discard an Event", game.Hunters[i].Hunter.Name());
                        }
                    }
                }
                else
                {
                    if (game.Hunters[i].ItemCount > 4)
                    {
                        Console.WriteLine("{0} must discard an Item", game.Hunters[i].Hunter.Name());
                    }
                    if (game.Hunters[i].EventCount > 4)
                    {
                        Console.WriteLine("{0} must discard an Event", game.Hunters[i].Hunter.Name());
                    }
                }
            }
        }

        /// <summary>
        /// Reveals a card and its encounters at a given position, for debugging
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="position">1-6 for positions in the trail, 7-9 for the three catacombs positions</param>
        private static void RevealCardInTrailAtPosition(GameState game, string position)
        {
            int index;
            if (!Int32.TryParse(position, out index))
            {
                string line = "";
                Console.WriteLine("What position do you want to reveal?");
                bool successful = false;
                do
                {
                    line = Console.ReadLine();
                    successful = Int32.TryParse(line, out index);
                } while (!successful);
            }
            if (!game.Dracula.RevealCardAtPosition(index - 1))
            {
                Console.WriteLine("Cannot reveal card at position {0}", index);
            }
            else
            {
                game.Dracula.RevealEncountersAtPositionInTrail(game, index - 1);
            }
        }

        /// <summary>
        /// Allows Dracula to take his turn
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="logic">The artificial intelligence component</param>
        private static void EndHunterTurn(GameState game, DecisionMaker logic)
        {
            game.AdvanceTimeTracker();

            List<int> catacombsSlotsCleared = logic.ChooseWhichCatacombsCardsToDiscard(game);
            if (catacombsSlotsCleared.Count() > 0)
            {
                Console.Write("Dracula discarded cards from Catacombs positions: ");
                foreach (int i in catacombsSlotsCleared)
                {
                    Console.Write("{0} ", i + 1);
                }
                Console.WriteLine("");
                game.Dracula.DiscardCatacombsCards(game, catacombsSlotsCleared);
            }
            Event eventPlayed = Event.None;
            int numberOfMoves = 1;
            do
            {
                DevilishPowerTarget devilishPowerTarget = DevilishPowerTarget.None;
                Location roadBlock1 = Location.Nowhere;
                Location roadBlock2 = Location.Nowhere;
                ConnectionType roadBlockType = ConnectionType.None;
                eventPlayed = logic.ChooseEventCardToPlayAtStartOfDraculaTurn(game, out devilishPowerTarget, out roadBlock1, out roadBlock2, out roadBlockType);
                if (eventPlayed != Event.None)
                {
                    Console.WriteLine("Dracula played {0}", eventPlayed.Name());
                    game.Dracula.DiscardEvent(eventPlayed, game.EventDiscard);
                    if (HunterPlayingGoodLuckToCancelDraculaEvent(game, eventPlayed, eventPlayed, logic) > -1)
                    {
                        Console.WriteLine("{0} cancelled", eventPlayed.Name());
                    }
                    else
                    {
                        switch (eventPlayed)
                        {
                            case Event.DevilishPower:
                                switch (devilishPowerTarget)
                                {
                                    case DevilishPowerTarget.HeavenlyHost1:
                                        Console.WriteLine("Heavenly Host discarded from {0}", game.HeavenlyHostLocation1);
                                        game.HeavenlyHostLocation1 = Location.Nowhere;
                                        break;
                                    case DevilishPowerTarget.HeavenlyHost2:
                                        Console.WriteLine("Heavenly Host discarded from {0}", game.HeavenlyHostLocation2);
                                        game.HeavenlyHostLocation2 = Location.Nowhere;
                                        break;
                                    case DevilishPowerTarget.HunterAlly:
                                        Console.WriteLine("{0} discarded from play", game.HunterAlly.Event.Name());
                                        game.EventDiscard.Add(game.HunterAlly);
                                        game.HunterAlly = null;
                                        break;
                                }
                                break;
                            case Event.UnearthlySwiftness:
                                numberOfMoves = 2;
                                break;
                            case Event.TimeRunsShort:
                                game.RegressTimeTracker();
                                break;
                            case Event.Roadblock:
                                game.RoadBlockLocation1 = roadBlock1;
                                game.RoadBlockLocation2 = roadBlock2;
                                game.RoadBlockConnectionType = roadBlockType;
                                break;
                        }
                    }
                }
            } while (eventPlayed != Event.None);

            Power power = Power.None;
            Location destination;
            List<DraculaCardSlot> cardsDroppedOffTrail = new List<DraculaCardSlot>();
            for (int i = 0; i < numberOfMoves; i++)
            {
                destination = logic.ChooseDestinationAndPower(game, out power);
                if (destination == Location.Nowhere && power == Power.None)
                {
                    Console.WriteLine("Dracula is cornered by his own trail and has no valid moves");
                    return;
                }
                cardsDroppedOffTrail.Add(game.Dracula.MoveTo(destination, power));
                if (game.Map.TypeOfLocation(destination) == LocationType.Sea)
                {
                    if ((!game.Dracula.LostBloodFromSeaMovementLastTurn || game.HunterAlly.Event == Event.RufusSmith))
                    {
                        game.Dracula.AdjustBlood(-1);
                        game.Dracula.LostBloodFromSeaMovementLastTurn = true;
                    }
                    else
                    {
                        game.Dracula.LostBloodFromSeaMovementLastTurn = false;
                    }
                }
            }
            if (game.Dracula.CurrentLocation != game.Hunters[(int)Hunter.LordGodalming].CurrentLocation && game.Dracula.CurrentLocation != game.Hunters[(int)Hunter.DrSeward].CurrentLocation && game.Dracula.CurrentLocation != game.Hunters[(int)Hunter.VanHelsing].CurrentLocation && game.Dracula.CurrentLocation != game.Hunters[(int)Hunter.MinaHarker].CurrentLocation)
            {
                switch (power)
                {
                    case Power.Hide:
                    case Power.None:
                    case Power.WolfForm:
                        game.Dracula.PlaceEncounterTileOnCard(logic.ChooseEncounterTileToPlaceOnDraculaCardSlot(game, game.Dracula.Trail[0]), game.Dracula.Trail[0]); break;
                    case Power.DoubleBack:
                        EncounterTile encounterTileToDiscard = logic.ChooseEncounterTileToDiscardFromDoubleBackedCatacombsLocation(game);
                        game.Dracula.DiscardEncounterTileFromCardSlot(encounterTileToDiscard, game.Dracula.Trail[0], game.EncounterPool); break;
                }
            }
            else
            {
                game.Dracula.Trail[0].DraculaCards.First().IsRevealed = true;
                Console.WriteLine("Dracula attacks!");
                List<HunterPlayer> huntersAttacked = new List<HunterPlayer>();
                foreach (HunterPlayer h in game.Hunters)
                {
                    if (h != null && h.CurrentLocation == game.Dracula.CurrentLocation)
                    {
                        huntersAttacked.Add(h);
                    }
                }
                ResolveCombat(game, huntersAttacked, Opponent.Dracula, logic);
            }
            if (game.Dracula.CurrentLocation == game.Dracula.LocationWhereHideWasUsed && power == Power.DoubleBack && game.Dracula.LocationWhereHideWasUsed != Location.Nowhere)
            {
                game.Dracula.RevealHideCard();
                Console.WriteLine("Dracula used Double Back to return to the location where he previously used Hide");
            }
            DealWithDroppedOffCardSlots(game, cardsDroppedOffTrail, logic);
            while (game.Dracula.EncounterHand.Count() < game.Dracula.EncounterHandSize)
            {
                game.Dracula.DrawEncounter(game.EncounterPool);
            }
        }

        /// <summary>
        /// Draws the gamestate on the screen
        /// </summary>
        /// <param name="game">The GameState</param>
        private static void DrawGameState(GameState game)
        {
            // line 1
            Console.WriteLine("Trail                       Catacombs       Time          Vampires   Resolve");
            for (int i = 5; i >= 0; i--)
            {
                if (game.Dracula.Trail[i] != null)
                {
                    Console.ForegroundColor = game.Dracula.Trail[i].DraculaCards.First().Color;
                    if (game.Dracula.Trail[i].DraculaCards.First().IsRevealed)
                    {
                        Console.Write(game.Dracula.Trail[i].DraculaCards.First().Abbreviation + " ");
                    }
                    else
                    {
                        Console.Write("### ");
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }
            Console.Write("    ");
            for (int i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null)
                {
                    Console.ForegroundColor = game.Dracula.Catacombs[i].DraculaCards.First().Color;
                    if (game.Dracula.Catacombs[i].DraculaCards.First().IsRevealed)
                    {
                        Console.Write(game.Dracula.Catacombs[i].DraculaCards.First().Abbreviation + " ");
                    }
                    else
                    {
                        Console.Write("### ");
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }
            switch (game.TimeOfDay)
            {
                case TimeOfDay.Dawn:
                case TimeOfDay.Dusk: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case TimeOfDay.Noon: Console.ForegroundColor = ConsoleColor.Yellow; break;
                case TimeOfDay.Midnight: Console.ForegroundColor = ConsoleColor.Blue; break;
                case TimeOfDay.Twilight:
                case TimeOfDay.SmallHours: Console.ForegroundColor = ConsoleColor.DarkBlue; break;
            }
            Console.Write("    {0}", game.TimeOfDay.Name());
            Console.ResetColor();
            for (int i = game.TimeOfDay.Name().Length; i < 14; i++)
            {
                Console.Write(" ");
            }
            Console.Write(game.Vampires);
            for (int i = game.Vampires.ToString().Length; i < 11; i++)
            {
                Console.Write(" ");
            }
            Console.Write(game.Resolve);
            Console.WriteLine("");
            // line 2
            for (int i = 5; i >= 0; i--)
            {
                if (game.Dracula.Trail[i] != null && game.Dracula.Trail[i].DraculaCards.Count() > 1)
                {
                    Console.ForegroundColor = game.Dracula.Trail[i].DraculaCards[1].Color;
                    if (game.Dracula.Trail[i].DraculaCards[1].IsRevealed)
                    {
                        Console.Write(game.Dracula.Trail[i].DraculaCards[1].Abbreviation + " ");
                    }
                    else
                    {
                        Console.Write("### ");
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }
            Console.Write("    ");
            for (int i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null && game.Dracula.Catacombs[i].EncounterTiles.Count() > 0)
                {
                    if (game.Dracula.Catacombs[i].EncounterTiles.First().IsRevealed)
                    {
                        Console.ResetColor();
                        Console.Write(game.Dracula.Catacombs[i].EncounterTiles.First().Abbreviation + " ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(" ■  ");
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }
            Console.WriteLine("");
            // line 3
            for (int i = 5; i >= 0; i--)
            {
                if (game.Dracula.Trail[i] != null && game.Dracula.Trail[i].EncounterTiles.Count() > 0)
                {
                    if (game.Dracula.Trail[i].EncounterTiles.First().IsRevealed)
                    {
                        Console.ResetColor();
                        Console.Write(game.Dracula.Trail[i].EncounterTiles.First().Abbreviation + " ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(" ■  ");
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }
            Console.Write("    ");
            for (int i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null && game.Dracula.Catacombs[i].EncounterTiles.Count() > 1)
                {
                    if (game.Dracula.Catacombs[i].EncounterTiles[1].IsRevealed)
                    {
                        Console.ResetColor();
                        Console.Write(game.Dracula.Catacombs[i].EncounterTiles[1].Abbreviation + " ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(" ■  ");
                    }
                }
                else
                {
                    Console.Write("    ");
                }
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        /// <summary>
        /// Provides details about various aspects of the gamestate that are not otherwise displayed on the screen, for debugging
        /// </summary>
        /// <param name="game">The GameState</param>
        private static void DisplayState(GameState game)
        {
            Console.WriteLine("The state of the game:");
            foreach (HunterPlayer h in game.Hunters)
            {
                if (h != null)
                {
                    Console.WriteLine("{0} is in {1} with {2} Items and {3} Events, on {4} health with {5} bites", h.Hunter.Name(), h.CurrentLocation.Name(), h.ItemCount, h.EventCount, h.Health, h.BiteCount);
                    if (h.ItemsKnownToDracula.Count() > 0)
                    {
                        Console.Write("These Items held by {0} are known to Dracula: ", h.Hunter.Name());
                        foreach (ItemCard i in h.ItemsKnownToDracula)
                        {
                            Console.Write("{0}, ", i.Item.Name());
                        }
                        Console.WriteLine("");
                    }
                    if (h.EventsKnownToDracula.Count() > 0)
                    {
                        Console.Write("These Events held by {0} are known to Dracula: ", h.Hunter.Name());
                        foreach (EventCard e in h.EventsKnownToDracula)
                        {
                            Console.Write("{0}, ", e.Event.Name());
                        }
                        Console.WriteLine("");
                    }
                }
            }
            Console.WriteLine("Dracula is in {0} with {1} blood and has {2} Events", game.Dracula.CurrentLocation.Name(), game.Dracula.Blood, game.Dracula.EventHand.Count());
            Console.Write("Dracula is holding these Encounter tiles: ");
            foreach (EncounterTile enc in game.Dracula.EncounterHand)
            {
                Console.Write("{0} ", enc.Encounter.Name());
            }
            if (game.Dracula.EventHand.Count() > 0)
            {
                Console.Write("Dracula is holding these Event cards: ");
                foreach (EventCard e in game.Dracula.EventHand)
                {
                    Console.Write("{0} ", e.Event.Name());
                }
            }
            Console.WriteLine("");
            Console.WriteLine("These Items are in the discard:");
            foreach (ItemCard i in game.ItemDiscard)
            {
                Console.WriteLine(i.Item.Name());
            }
            Console.WriteLine("These Events are in the discard:");
            foreach (EventCard e in game.EventDiscard)
            {
                Console.WriteLine(e.Event.Name());
            }
        }

        /// <summary>
        /// Discards a card of a given name from the given hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="cardName">The name of the card (Item or Event) to discard</param>
        /// <param name="hunterIndex">The number of the hunter (1-4)</param>
        private static void DiscardCard(GameState game, string cardName, string hunterIndex)
        {
            int index = 0;
            Hunter hunterToDiscard = Hunter.Nobody;
            if (Int32.TryParse(hunterIndex, out index) && index > 0 && index < 5)
            {
                hunterToDiscard = (Hunter)index;
            }
            string line = "";
            while (hunterToDiscard == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is discarding a card? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (Int32.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    hunterToDiscard = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterToDiscard.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            Item itemToDiscard = Enumerations.GetItemFromString(cardName);
            Event eventToDiscard = Enumerations.GetEventFromString(cardName);
            while (itemToDiscard == Item.None && eventToDiscard == Event.None && line.ToLower() != "cancel")
            {
                Console.WriteLine("What is the name of the card you are discarding? (type cancel to cancel)");
                line = Console.ReadLine().ToLower();
                if (line.ToLower() == "cancel")
                {
                    Console.WriteLine("Cancelled");
                    return;
                }

                itemToDiscard = Enumerations.GetItemFromString(line);
                eventToDiscard = Enumerations.GetEventFromString(line);
                if (itemToDiscard == Item.None && eventToDiscard == Event.None)
                {
                    Console.WriteLine("I couldn't find any card with that name");
                }
                else if (itemToDiscard != Item.None && eventToDiscard != Event.None)
                {
                    Console.WriteLine("I couldn't tell if you meant an Item or an Event card");
                }
            }
            if (itemToDiscard != Item.None)
            {
                game.Hunters[index].DiscardItem(game, itemToDiscard);
                Console.WriteLine("{0} discarded {1}, down to {2}", hunterToDiscard.Name(), itemToDiscard.Name(), game.Hunters[index].ItemCount);
            }
            else if (eventToDiscard != Event.None)
            {
                game.Hunters[index].DiscardEvent(game, eventToDiscard);
                Console.WriteLine("{0} discarded {1}, down to {2}", hunterToDiscard.Name(), eventToDiscard.Name(), game.Hunters[index].EventCount);
            }
            CheckForCardsRevealedForBeingBitten(game);
        }

        /// <summary>
        /// Adds 1 to the count of cards of the given type to the given hunter
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="cardType">Item or Event</param>
        /// <param name="hunterIndex">The number of the hunter (1-4)</param>
        private static void DrawCard(GameState game, string cardType, string hunterIndex)
        {
            int index = 0;
            Hunter hunterToDraw = Hunter.Nobody;
            if (Int32.TryParse(hunterIndex, out index))
            {
                hunterToDraw = game.GetHunterFromInt(index);
            }
            string line = "";
            while (hunterToDraw == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is drawing a card? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (Int32.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        return;
                    }
                    hunterToDraw = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterToDraw.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            do
            {
                switch (cardType)
                {
                    case "i":
                    case "item": game.Hunters[index].DrawItemCard(); Console.WriteLine("{0} drew an Item card, up to {1}", hunterToDraw.Name(), game.Hunters[index].ItemCount); return;
                    case "e":
                    case "event": game.Hunters[index].DrawEventCard(); Console.WriteLine("{0} drew an Event card, up to {1}", hunterToDraw.Name(), game.Hunters[index].EventCount); return;
                    default:
                        Console.WriteLine("What type of card is {0} drawing?", hunterToDraw.Name());
                        cardType = Console.ReadLine().ToLower();
                        break;
                }
            } while (cardType != "cancel");
            Console.WriteLine("Cancelled");
        }

        /// <summary>
        /// Moves the given hunter to the given location, along with all hunters in the given hunter's group
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="hunterIndex">The number of the hunter (1-4)</param>
        /// <param name="location">The name of the location to move to</param>        
        /// <returns>The position of the card in Dracula's trail (0-5) or catacombs (6-8) that corresponds to the given location, or -1 if not in the trail/catacombs</returns>
        private static int MoveHunter(GameState game, string hunterIndex, string location, out Hunter hunterMoved, out Location originatingLocation, DecisionMaker logic)
        {
            Hunter hunterToMove = Hunter.Nobody;
            int index;
            if (Int32.TryParse(hunterIndex, out index))
            {
                hunterToMove = game.GetHunterFromInt(index);
            }
            string line = "";
            while (hunterToMove == Hunter.Nobody && index != -1)
            {
                Console.WriteLine("Who is moving? {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7} (-1 to cancel)", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                line = Console.ReadLine();
                if (Int32.TryParse(line, out index))
                {
                    if (index == -1)
                    {
                        Console.WriteLine("Cancelled");
                        hunterMoved = Hunter.Nobody;
                        originatingLocation = Location.Nowhere;
                        return -1;
                    }
                    hunterToMove = game.GetHunterFromInt(index);
                    Console.WriteLine(hunterToMove.Name());
                }
                else
                {
                    Console.WriteLine("I didn't understand that");
                }
            }
            hunterMoved = (Hunter)index;
            originatingLocation = game.Hunters[(int)hunterMoved].CurrentLocation;
            Location destination;
            if (DraculaIsPlayingControlStorms(game, hunterMoved, logic))
            {
                Console.WriteLine("Dracula is controlling the ship's movement");
                destination = logic.ChoosePortToMoveHuntersToWithControlStorms(game, hunterMoved);
            }
            else
            {
                destination = Enumerations.GetLocationFromString(location);
                while (destination == Location.Nowhere && line.ToLower() != "cancel")
                {
                    Console.WriteLine("Where is {0} moving? (Type cancel to cancel)", hunterToMove.Name());
                    line = Console.ReadLine();
                    destination = Enumerations.GetLocationFromString(line);
                    Console.WriteLine(destination.Name());
                }
                if (line.ToLower() == "cancel")
                {
                    Console.WriteLine("Cancelled");
                    return -1;
                }
            }
            Console.Write("{0} moved from {1} to ", hunterToMove.Name(), game.Hunters[(int)hunterToMove].CurrentLocation.Name());
            foreach (Hunter h in game.Hunters[(int)hunterToMove].HuntersInGroup)
            {
                game.Hunters[(int)h].MoveTo(destination);
            }
            Console.WriteLine(destination.Name() + (game.Hunters[(int)hunterToMove].HuntersInGroup.Count() > 1 ? " with his group" : ""));
            for (int i = 0; i < 6; i++)
            {
                if (game.Dracula.Trail[i] != null && game.Dracula.Trail[i].DraculaCards.First().Location == destination)
                {
                    return i;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (game.Dracula.Catacombs[i] != null && game.Dracula.Catacombs[i].DraculaCards.First().Location == destination)
                {
                    return i + 6;
                }
            }
            return -1;
        }

        private static bool DraculaIsPlayingControlStorms(GameState game, Hunter hunterMoved, DecisionMaker logic)
        {
            if (game.Dracula.EventHand.Find(card => card.Event == Event.ControlStorms) != null && game.Map.TypeOfLocation(game.Hunters[(int)hunterMoved].CurrentLocation) == LocationType.Sea && new Random().Next(0, 2) == 0)
            {
                Console.WriteLine("Dracula is playing Control Storms to control your movement");
                game.Dracula.DiscardEvent(Event.ControlStorms, game.EventDiscard);
                if (HunterPlayingGoodLuckToCancelDraculaEvent(game, Event.ControlStorms, Event.ControlStorms, logic) > -1)
                {
                    Console.WriteLine("Control Storms cancelled");
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Serialises and saves a gamestate to a file with the given filename
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="fileName">The name of the file, without extension</param>
        private static void SaveGameState(GameState game, string fileName)
        {
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

                fileWriter.WriteObject(writeStream, game);
                writeStream.Close();
                Console.WriteLine(fileName + " saved");
            }
            catch (Exception e)
            {
                Console.WriteLine("File could not be saved because " + e.Message);
            }
        }

        /// <summary>
        /// Used to capture multiple strings from a single user console input
        /// </summary>
        /// <returns>A CommandSet</returns>
        internal static CommandSet GetCommandSet()
        {
            string line;
            string command;
            string argument1;
            string argument2 = "no argument";

            line = Console.ReadLine().ToLower();
            try
            {
                command = line.Substring(0, line.IndexOf(' '));
            }
            catch (ArgumentOutOfRangeException)
            {
                command = line;
            }

            try
            {
                argument1 = line.Substring(line.IndexOf(' ') + 1);
                try
                {
                    argument2 = argument1.Substring(argument1.IndexOf(' ') + 1);
                    argument1 = argument1.Substring(0, argument1.IndexOf(' '));
                }
                catch (ArgumentOutOfRangeException)
                {
                    argument2 = "no argument";
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                argument1 = "no argument";
            }

            return new CommandSet(command.ToLower(), argument1.ToLower(), argument2.ToLower());
        }

        /// <summary>
        /// Resolves a combat between a group of hunters and an opponent
        /// </summary>
        /// <param name="game">The GameState</param>
        /// <param name="huntersInvolved">A list of HunterPlayers involved in the combat</param>
        /// <param name="opponent">The opponent type</param>
        /// <param name="logic">The artificial intelligence component</param>
        /// <returns>A bool of whether the HunterPlayers can continue resolving encounters or not</returns>
        private static bool ResolveCombat(GameState game, List<HunterPlayer> huntersInvolved, Opponent opponent, DecisionMaker logic)
        {
            int roundsWithoutEscaping = 0;
            int answer = -1;
            do
            {
                Console.WriteLine("Is anyone playing an Event at the start of this combat? 0= Nobody, {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7}", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                while (answer == -1)
                {
                    if (Int32.TryParse(Console.ReadLine(), out answer))
                    {
                        if (answer < 0 || answer > 4)
                        {
                            answer = -1;
                        }
                    }
                }
                if (answer > 0)
                {
                    string line = "";
                    Console.WriteLine("What Event is {0} playing? (cancel to cancel)", ((Hunter)answer).Name());
                    Event eventBeingPlayed = Event.None;
                    while (eventBeingPlayed != Event.AdvancePlanning && eventBeingPlayed != Event.EscapeRoute && eventBeingPlayed != Event.HeroicLeap && line.ToLower() != "cancel")
                    {
                        line = Console.ReadLine();
                        eventBeingPlayed = Enumerations.GetEventFromString(line);
                    }
                    if (line.ToLower() == "cancel")
                    {
                        answer = -1;
                    }
                    else
                    {
                        switch (eventBeingPlayed)
                        {
                            case Event.AdvancePlanning:
                                game.Hunters[answer].DiscardEvent(game, Event.AdvancePlanning);
                                if (!DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.AdvancePlanning, Event.AdvancePlanning, logic))
                                {
                                    Console.WriteLine("{0}'s combat rolls are at +1 for this combat", ((Hunter)answer).Name());
                                }
                                else
                                {
                                    Console.WriteLine("Advance Planning cancelled");
                                }
                                break;
                            case Event.EscapeRoute:
                                game.Hunters[answer].DiscardEvent(game, Event.EscapeRoute);
                                if (!DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.EscapeRoute, Event.EscapeRoute, logic))
                                {
                                    Console.WriteLine("Combat cancelled", ((Hunter)answer).Name());
                                    return false;
                                }
                                else
                                {
                                    Console.WriteLine("Escape Route cancelled");
                                }
                                break;
                            case Event.HeroicLeap:
                                game.Hunters[answer].DiscardEvent(game, Event.HeroicLeap);
                                if (!DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.HeroicLeap, Event.HeroicLeap, logic))
                                {
                                    Console.WriteLine("Roll a die and enter the result");
                                    int dieRoll = 0;
                                    while (dieRoll == 0)
                                    {
                                        if (Int32.TryParse(Console.ReadLine(), out dieRoll))
                                        {
                                            if (dieRoll < 1 || dieRoll > 7)
                                            {
                                                dieRoll = 0;
                                            }
                                        }
                                    }
                                    game.Hunters[answer].AdjustHealth(-dieRoll);
                                    game.Dracula.AdjustBlood(-dieRoll);
                                    CheckForHunterDeath(game);
                                    return false;
                                }
                                else
                                {
                                    Console.WriteLine("Escape Route cancelled");
                                }
                                break;
                        }

                    }
                }
            } while (answer != 0);
            answer = -1;
            do
            {
                Console.WriteLine("Is anyone playing Garlic at the start of this combat? 0= Nobody, {0}= {1}, {2}= {3}, {4}= {5}, {6}= {7}", (int)Hunter.LordGodalming, Hunter.LordGodalming.Name(), (int)Hunter.DrSeward, Hunter.DrSeward.Name(), (int)Hunter.VanHelsing, Hunter.VanHelsing.Name(), (int)Hunter.MinaHarker, Hunter.MinaHarker.Name());
                while (answer == -1)
                {
                    if (Int32.TryParse(Console.ReadLine(), out answer))
                    {
                        if (answer < 0 || answer > 4)
                        {
                            answer = -1;
                        }
                    }
                }
                if (answer > 0)
                {
                    game.Hunters[answer].DiscardItem(game, Item.Garlic);
                    roundsWithoutEscaping = 3;
                }
            } while (answer != 0);
            List<ItemCard> basicHunterCombatCards = new List<ItemCard> { new ItemCard(Item.Punch), new ItemCard(Item.Dodge), new ItemCard(Item.Escape) };
            List<EnemyCombatCard> enemyCombatCards = new List<EnemyCombatCard>();
            switch (opponent)
            {
                case Opponent.MinionWithKnife:
                    enemyCombatCards.Add(EnemyCombatCard.Punch);
                    enemyCombatCards.Add(EnemyCombatCard.Dodge);
                    enemyCombatCards.Add(EnemyCombatCard.Knife); break;
                case Opponent.MinionWithKnifeAndPistol:
                    enemyCombatCards.Add(EnemyCombatCard.Punch);
                    enemyCombatCards.Add(EnemyCombatCard.Dodge);
                    enemyCombatCards.Add(EnemyCombatCard.Knife);
                    enemyCombatCards.Add(EnemyCombatCard.Pistol); break;
                case Opponent.MinionWithKnifeAndRifle:
                    enemyCombatCards.Add(EnemyCombatCard.Punch);
                    enemyCombatCards.Add(EnemyCombatCard.Dodge);
                    enemyCombatCards.Add(EnemyCombatCard.Knife);
                    enemyCombatCards.Add(EnemyCombatCard.Rifle); break;
                case Opponent.Assassin:
                    enemyCombatCards.Add(EnemyCombatCard.Punch);
                    enemyCombatCards.Add(EnemyCombatCard.Dodge);
                    enemyCombatCards.Add(EnemyCombatCard.Knife);
                    enemyCombatCards.Add(EnemyCombatCard.Pistol);
                    enemyCombatCards.Add(EnemyCombatCard.Rifle); break;
                case Opponent.Dracula:
                    enemyCombatCards.Add(EnemyCombatCard.Claws);
                    enemyCombatCards.Add(EnemyCombatCard.Dodge);
                    enemyCombatCards.Add(EnemyCombatCard.EscapeMan);
                    switch (game.TimeOfDay)
                    {
                        case TimeOfDay.Twilight:
                        case TimeOfDay.Midnight:
                        case TimeOfDay.SmallHours:
                            enemyCombatCards.Add(EnemyCombatCard.EscapeBat);
                            enemyCombatCards.Add(EnemyCombatCard.EscapeMist);
                            enemyCombatCards.Add(EnemyCombatCard.Fangs);
                            enemyCombatCards.Add(EnemyCombatCard.Mesmerize);
                            enemyCombatCards.Add(EnemyCombatCard.Strength); break;
                    }; break;
                case Opponent.NewVampire:
                    enemyCombatCards.Add(EnemyCombatCard.Claws);
                    enemyCombatCards.Add(EnemyCombatCard.Dodge);
                    switch (game.TimeOfDay)
                    {
                        case TimeOfDay.Twilight:
                        case TimeOfDay.Midnight:
                        case TimeOfDay.SmallHours:
                            enemyCombatCards.Add(EnemyCombatCard.Fangs);
                            enemyCombatCards.Add(EnemyCombatCard.Mesmerize);
                            enemyCombatCards.Add(EnemyCombatCard.Strength); break;
                    }; break;
            }
            bool firstRound = true;
            bool repelled = false;
            bool continueCombat = true;
            bool sisterAgathaInEffect = (opponent == Opponent.Dracula && game.HunterAlly.Event == Event.SisterAgatha);
            EnemyCombatCard enemyCombatCardChosen = EnemyCombatCard.None;
            Hunter enemyTarget = Hunter.Nobody;
            while (continueCombat)
            {
                foreach (HunterPlayer h in huntersInvolved)
                {
                    Item itemUsedByHunterLastRound = h.LastCombatCardChosen;
                    do
                    {
                        Console.WriteLine("Which combat card is {0} using this round?", h.Hunter.Name());
                        h.LastCombatCardChosen = Enumerations.GetItemFromString(Console.ReadLine());
                    } while (h.LastCombatCardChosen == Item.None);

                    if (basicHunterCombatCards.Find(card => card.Item == h.LastCombatCardChosen) == null && h.ItemsKnownToDracula.Find(item => item.Item == h.LastCombatCardChosen) == null)
                    {
                        ItemCard itemDraculaNowKnowsAbout = game.ItemDeck.Find(card => card.Item == h.LastCombatCardChosen);
                        h.ItemsKnownToDracula.Add(itemDraculaNowKnowsAbout);
                        game.ItemDeck.Remove(itemDraculaNowKnowsAbout);
                    }
                    else if (itemUsedByHunterLastRound == h.LastCombatCardChosen)
                    {
                        ItemCard itemDraculaAlreadyKnewAbout = h.ItemsKnownToDracula.Find(card => card.Item == itemUsedByHunterLastRound);
                        h.ItemsKnownToDracula.Remove(itemDraculaAlreadyKnewAbout);
                        if (h.ItemsKnownToDracula.Find(card => card.Item == itemUsedByHunterLastRound) == null)
                        {
                            ItemCard itemDraculaNowKnowsAbout = game.ItemDeck.Find(card => card.Item == h.LastCombatCardChosen);
                            h.ItemsKnownToDracula.Add(itemDraculaNowKnowsAbout);
                            game.ItemDeck.Remove(itemDraculaNowKnowsAbout);
                        }
                        h.ItemsKnownToDracula.Add(itemDraculaAlreadyKnewAbout);
                    }
                }
                enemyCombatCardChosen = logic.ChooseCombatCardAndTarget(game, huntersInvolved, enemyCombatCards, firstRound, out enemyTarget, enemyCombatCardChosen, repelled, sisterAgathaInEffect, roundsWithoutEscaping);
                roundsWithoutEscaping--;
                bool bloodCost = false;
                if (sisterAgathaInEffect && (enemyCombatCardChosen == EnemyCombatCard.Fangs || enemyCombatCardChosen == EnemyCombatCard.EscapeBat || enemyCombatCardChosen == EnemyCombatCard.EscapeMan || enemyCombatCardChosen == EnemyCombatCard.EscapeMist))
                {
                    game.Dracula.AdjustBlood(-2);
                    bloodCost = true;
                }
                Console.WriteLine("{0} used {1} against {2}{3}", opponent.Name(), enemyCombatCardChosen.Name(), enemyTarget.Name(), bloodCost ? " at a cost of 2 blood" : "");
                string line = "";
                int index = 0;
                do
                {
                    Console.WriteLine("What was the outcome? 1= Continue 2= Repel 3= Item destroyed 4= End");
                    line = Console.ReadLine();
                    if (Int32.TryParse(line, out index))
                    {
                        if (index < 1 || index > 4)
                        {
                            index = 0;
                        }
                    }
                } while (index == 0);
                switch (index)
                {
                    case 1: break;
                    case 2: repelled = true; break;
                    case 3:
                        HunterPlayer hunterToDiscardItem = null;
                        if (huntersInvolved.Count == 1)
                        {
                            hunterToDiscardItem = huntersInvolved.First();
                        }
                        else
                        {
                            int hunterIndex = 0;
                            do
                            {
                                Console.Write("Whose Item was destroyed? ");
                                foreach (HunterPlayer h in huntersInvolved)
                                {
                                    Console.Write("{0}= {1} ", (int)h.Hunter, h.Hunter.Name());
                                }
                                line = Console.ReadLine();
                                if (Int32.TryParse(line, out hunterIndex))
                                {
                                    foreach (HunterPlayer h in huntersInvolved)
                                    {
                                        if ((int)h.Hunter == hunterIndex)
                                        {
                                            hunterToDiscardItem = h;
                                            break;
                                        }
                                    }
                                }
                            } while (hunterToDiscardItem == null);
                        }
                        Item itemDestroyed = Item.None;
                        while (itemDestroyed == Item.None)
                        {
                            Console.WriteLine("What Item was destroyed?");
                            line = Console.ReadLine();
                            itemDestroyed = Enumerations.GetItemFromString(line);
                        }
                        hunterToDiscardItem.DiscardItem(game, itemDestroyed);
                        break;
                    case 4: continueCombat = false; break;
                }
            }
            int health = -1;
            foreach (HunterPlayer h in huntersInvolved)
            {
                Console.WriteLine("How much health does {0} have now?", h.Hunter.Name());
                do
                {
                    if (Int32.TryParse(Console.ReadLine(), out health))
                    {
                        health = Math.Max(0, Math.Min(h.MaxHealth, health));
                        Console.WriteLine(health);
                    }
                } while (health == -1);
                h.AdjustHealth(health - h.Health);
            }
            if (opponent == Opponent.Dracula)
            {
                Console.WriteLine("How much blood does Dracula have now?");
                do
                {
                    if (Int32.TryParse(Console.ReadLine(), out health))
                    {
                        health = Math.Max(0, Math.Min(15, health));
                        Console.WriteLine(health);
                    }
                } while (health == -1);
                game.Dracula.AdjustBlood(health - game.Dracula.Blood);
            }
            Console.WriteLine("Did {0} win? (An end result is a no)", huntersInvolved.Count() > 1 ? "the Hunters" : huntersInvolved.First().Hunter.Name());
            string input;
            do
            {
                input = Console.ReadLine();
            } while (!"yes".StartsWith(input.ToLower()) && !"no".StartsWith(input.ToLower()));
            if ("yes".StartsWith(input.ToLower()))
            {
                if (opponent == Opponent.NewVampire)
                {
                    game.AdjustVampires(-1);
                }
                Console.WriteLine("Don't forget to register any cards discarded, such as Great Strength used to prevent health loss, Items due to enemy Knife wounds, Items consumed");
                return true;
            }
            else
            {
                if (opponent == Opponent.Dracula)
                {
                    switch (enemyCombatCardChosen)
                    {
                        case EnemyCombatCard.Mesmerize:
                            Console.WriteLine("{0} is bitten and must discard all Items!", enemyTarget.Name());
                            if (!HunterPlayingGreatStrengthToCancelBite(game, enemyTarget, logic))
                            {
                                game.Hunters[(int)enemyTarget].AdjustBites(1);
                            }
                            while (game.Hunters[(int)enemyTarget].ItemCount > 0)
                            {
                                string line = "";
                                Item itemDiscarded = Item.None;
                                do
                                {
                                    Console.WriteLine("What is the name of the Item being discarded?");
                                    line = Console.ReadLine();
                                    itemDiscarded = Enumerations.GetItemFromString(line);
                                } while (itemDiscarded == Item.None);
                                game.Hunters[(int)enemyTarget].DiscardItem(game, itemDiscarded);
                            }
                            CheckForHunterDeath(game);
                            break;
                        case EnemyCombatCard.Fangs:
                            Console.WriteLine("{0} is bitten!", enemyTarget.Name());
                            if (!HunterPlayingGreatStrengthToCancelBite(game, enemyTarget, logic))
                            {
                                game.Hunters[(int)enemyTarget].AdjustBites(1);
                            }
                            CheckForHunterDeath(game);
                            goto case EnemyCombatCard.EscapeBat;
                        case EnemyCombatCard.EscapeBat:
                            Console.WriteLine("Dracula escaped in the form of a bat");
                            game.Dracula.EscapeAsBat(game, logic.ChooseEscapeAsBatDestination(game));
                            break;
                    }
                }
                Console.WriteLine("Don't forget to register any cards discarded, such as Great Strength used to prevent health loss or Items due to enemy Knife wounds");
                return false;
            }
        }

        private static bool HunterPlayingGreatStrengthToCancelBite(GameState game, Hunter bittenHunter, DecisionMaker logic)
        {
            Console.WriteLine("Will {0} play a Great Strength to cancel the Bite?", bittenHunter.Name());
            string input = "";
            do
            {
                input = Console.ReadLine();
            } while (!"yes".StartsWith(input.ToLower()) && !"no".StartsWith(input.ToLower()));
            if ("yes".StartsWith(input.ToLower()))
            {
                game.Hunters[(int)bittenHunter].DiscardEvent(game, Event.GreatStrength);
                if (!DraculaIsPlayingDevilishPowerToCancelEvent(game, Event.GreatStrength, Event.GreatStrength, logic))
                {
                    Console.WriteLine("Bite effect cancelled");
                    return true;
                }
                else
                {
                    Console.WriteLine("Great Strength cancelled");
                    return false;
                }
            }
            return false;
        }
    }
}
