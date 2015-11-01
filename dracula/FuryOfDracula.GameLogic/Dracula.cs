using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FuryOfDracula.GameLogic
{
    [DataContract(IsReference = true)]
    public class Dracula
    {
        private List<DraculaCard> _draculaCardDeck;

        public Dracula()
        {
            EventHandSize = 4;
            EncounterHandSize = 5;
            EventHand = new List<EventCard>();
            EncounterHand = new List<EncounterTile>();
            Blood = 15;
            Trail = new DraculaCardSlot[6];
            Catacombs = new DraculaCardSlot[3];
        }

        [DataMember]
        public int Blood { get; private set; }

        [DataMember]
        public Location CurrentLocation { get; private set; }

        [DataMember]
        public List<EncounterTile> EncounterHand { get; private set; }

        [DataMember]
        public List<EventCard> EventHand { get; private set; }

        [DataMember]
        public int EncounterHandSize { get; set; }

        [DataMember]
        public int EventHandSize { get; set; }

        [DataMember]
        public DraculaCardSlot[] Trail { get; private set; }

        [DataMember]
        public DraculaCardSlot[] Catacombs { get; private set; }

        [DataMember]
        public List<DraculaCard> DraculaCardDeck
        {
            get
            {
                if (_draculaCardDeck == null)
                {
                    _draculaCardDeck = CreateDraculaCardDeck();
                }
                return _draculaCardDeck;
            }
            private set { _draculaCardDeck = value; }
        }

        [DataMember]
        public Location LocationWhereHideWasUsed { get; private set; }

        [DataMember]
        public Location AdvanceMoveLocation { get; set; }

        [DataMember]
        public Power AdvanceMovePower { get; set; }

        [DataMember]
        public bool LostBloodFromSeaMovementLastTurn { get; set; }
        public int CurrentLocationPosition { get {
                for (int i = 0; i < 6; i++)
                {
                    if (Trail[i].DraculaCards.First().Location == CurrentLocation)
                    {
                        return i;
                    }
                }
                return -1;
            } }

        public DraculaCardSlot MoveTo(Location destination, Power power, out int doubleBackSlot)
        {
            DraculaCardSlot doubleBackedCard = null;
            doubleBackSlot = -1;
            var catacombsSlot = 0;
            if (power == Power.DoubleBack)
            {
                for (var i = 0; i < 6; i++)
                {
                    if (Trail[i] != null && Trail[i].DraculaCards.First().Location == destination)
                    {
                        doubleBackSlot = i;
                        doubleBackedCard = Trail[i];
                        for (var j = i; j > 0; j--)
                        {
                            Trail[j] = Trail[j - 1];
                        }
                        Trail[0] = doubleBackedCard;
                        Trail[0].DraculaCards.Add(DraculaCardDeck.Find(card => card.Power == Power.DoubleBack));
                        DraculaCardDeck.Find(card => card.Power == Power.DoubleBack).IsRevealed = true;
                        CurrentLocation = destination;
                        return null;
                    }
                }
                for (var i = 0; i < 3; i++)
                {
                    if (Catacombs[i] != null && Catacombs[i].DraculaCards.First().Location == destination)
                    {
                        doubleBackedCard = Catacombs[i];
                        catacombsSlot = i;
                    }
                }
            }
            var cardSlotDroppedOffTrail = Trail[5];
            for (var i = 5; i > 0; i--)
            {
                Trail[i] = Trail[i - 1];
            }
            if (destination != Location.Nowhere)
            {
                CurrentLocation = destination;
                if (doubleBackedCard == null)
                {
                    Trail[0] = new DraculaCardSlot(DraculaCardDeck.Find(card => card.Location == destination));
                    if (destination == Location.CastleDracula)
                    {
                        Trail[0].DraculaCards.First().IsRevealed = true;
                    }
                }
                else
                {
                    Trail[0] = doubleBackedCard;
                    Catacombs[catacombsSlot] = null;
                }
            }
            else
            {
                Trail[0] = new DraculaCardSlot();
            }
            if (power != Power.None)
            {
                Trail[0].DraculaCards.Add(DraculaCardDeck.Find(card => card.Power == power));
                if (power != Power.Hide || CurrentLocation == Location.CastleDracula)
                {
                    Trail[0].DraculaCards.Last().IsRevealed = true;
                }
            }
            if (power == Power.Hide)
            {
                LocationWhereHideWasUsed = CurrentLocation;
            }
            return cardSlotDroppedOffTrail;
        }

        public void RevealEncountersAtPositionInTrail(int positionRevealed)
        {
            if (positionRevealed < 6 && Trail[positionRevealed] != null)
            {
                foreach (var enc in Trail[positionRevealed].EncounterTiles)
                {
                    enc.IsRevealed = true;
                }
            }
            else if (Catacombs[positionRevealed - 6] != null)
            {
                positionRevealed -= 6;
                foreach (var enc in Catacombs[positionRevealed].EncounterTiles)
                {
                    enc.IsRevealed = true;
                }
            }
        }

        public int RevealCardInTrailWithLocation(Location location)
        {
            for (var i = 0; i < 6; i++)
            {
                if (location != Location.Nowhere && Trail[i] != null && Trail[i].DraculaCards.First().Location == location)
                {
                    Trail[i].DraculaCards.First().IsRevealed = true;
                    return i;
                }
            }
            return -1;
        }

        public Event DiscardEvent(Event eventToDiscard, List<EventCard> discardPile)
        {
            var eventCardToDiscard = EventHand.Find(card => card.Event == eventToDiscard);
            EventHand.Remove(eventCardToDiscard);
            discardPile.Add(eventCardToDiscard);
            return eventCardToDiscard.Event;
        }

        public void DiscardCatacombsCards(GameState game, List<int> list)
        {
            var encounterTilesDiscarded = new List<EncounterTile>();
            foreach (var i in list)
            {
                encounterTilesDiscarded.AddRange(Catacombs[i].EncounterTiles);
                foreach (var d in Catacombs[i].DraculaCards)
                {
                    d.IsRevealed = false;
                }
                Catacombs[i] = null;
            }
            foreach (var enc in encounterTilesDiscarded)
            {
                enc.IsRevealed = false;
                game.EncounterPool.Add(enc);
            }
        }

        public bool RevealCardAtPosition(int position)
        {
            if (position < 0 || position > 8)
            {
                return false;
            }
            if (position < 6)
            {
                if (Trail[position] != null)
                {
                    foreach (var card in Trail[position].DraculaCards)
                    {
                        card.IsRevealed = true;
                    }
                    return true;
                }
            }
            else if (Catacombs[position - 6] != null)
            {
                foreach (var card in Catacombs[position - 6].DraculaCards)
                {
                    card.IsRevealed = true;
                }
                return true;
            }
            return false;
        }

        public void DiscardEncounterTileFromCardSlot(EncounterTile encounterTileToDiscard,
            DraculaCardSlot draculaCardSlot, List<EncounterTile> encounterPool)
        {
            draculaCardSlot.EncounterTiles.Remove(encounterTileToDiscard);
            encounterPool.Add(encounterTileToDiscard);
        }

        public void DrawEncounter(List<EncounterTile> encounterPool)
        {
            var encounterDrawn = encounterPool[new Random().Next(0, encounterPool.Count())];
            EncounterHand.Add(encounterDrawn);
            encounterPool.Remove(encounterDrawn);
        }

        public void RevealEncounterAtPositionInTrail(GameState game, int positionRevealed, int encounterIndex)
        {
            if (positionRevealed < 6 && Trail[positionRevealed] != null)
            {
                if (Trail[positionRevealed].EncounterTiles.Count() > encounterIndex)
                {
                    Trail[positionRevealed].EncounterTiles[encounterIndex].IsRevealed = true;
                }
            }
            else if (Catacombs[positionRevealed - 6] != null)
            {
                if (Trail[positionRevealed].EncounterTiles.Count() > encounterIndex)
                {
                    Trail[positionRevealed].EncounterTiles[encounterIndex].IsRevealed = true;
                }
            }
        }

        public void PlaceEncounterTileOnCard(EncounterTile encounterToPlace, DraculaCardSlot card)
        {
            if (encounterToPlace != null)
            {
                card.EncounterTiles.Add(encounterToPlace);
                EncounterHand.Remove(encounterToPlace);
            }
        }

        public List<EncounterTile> DiscardHide(out int position)
        {
            position = -1;
            var encountersDiscarded = new List<EncounterTile>();
            for (var i = 0; i < 6; i++)
            {
                if (Trail[i] != null && Trail[i].DraculaCards.First().Power == Power.Hide)
                {
                    position = i;
                    Trail[i].DraculaCards.First().IsRevealed = false;
                    LocationWhereHideWasUsed = Location.Nowhere;
                    foreach (var enc in Trail[i].EncounterTiles)
                    {
                        encountersDiscarded.Add(enc);
                    }
                    for (var j = i; j < 5; j++)
                    {
                        Trail[j] = Trail[j + 1];
                    }
                    Trail[5] = null;
                    return encountersDiscarded;
                }
            }
            return encountersDiscarded;
        }

        public Event TakeEvent(List<EventCard> eventDeck, List<EventCard> eventDiscard)
        {
            EventCard cardDrawn;
            do
            {
                cardDrawn = eventDeck[new Random().Next(0, eventDeck.Count())];
            } while (!cardDrawn.IsDraculaCard);
            eventDeck.Remove(cardDrawn);
            if (cardDrawn.EventType == EventType.PlayImmediately || cardDrawn.EventType == EventType.Ally)
            {
                eventDiscard.Add(cardDrawn);
                return cardDrawn.Event;
            }
            EventHand.Add(cardDrawn);
            return Event.None;
        }

        public int RevealHideCard()
        {
            for (var i = 0; i < 6; i++)
            {
                if (Trail[i].DraculaCards.First().Power == Power.Hide)
                {
                    Trail[i].DraculaCards.First().IsRevealed = true;
                    return i;
                }
            }
            return -1;
        }

        private List<DraculaCard> CreateDraculaCardDeck()
        {
            var tempDraculaCardDeck = new List<DraculaCard>();
            tempDraculaCardDeck.Add(new DraculaCard("ADR", Location.AdriaticSea, Power.None, ConsoleColor.Cyan));
            tempDraculaCardDeck.Add(new DraculaCard("ALI", Location.Alicante, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("AMS", Location.Amsterdam, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("ATH", Location.Athens, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("ATL", Location.AtlanticOcean, Power.None, ConsoleColor.Cyan));
            tempDraculaCardDeck.Add(new DraculaCard("BAC", Location.Barcelona, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("BAI", Location.Bari, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("BAY", Location.BayOfBiscay, Power.None, ConsoleColor.Cyan));
            tempDraculaCardDeck.Add(new DraculaCard("BEL", Location.Belgrade, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("BER", Location.Berlin, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("BLA", Location.BlackSea, Power.None, ConsoleColor.Cyan));
            tempDraculaCardDeck.Add(new DraculaCard("BOR", Location.Bordeaux, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("BRU", Location.Brussels, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("BUC", Location.Bucharest, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("BUD", Location.Budapest, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("CAD", Location.Cadiz, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("CAG", Location.Cagliari, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("CAS", Location.CastleDracula, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("CLE", Location.ClermontFerrand, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("COL", Location.Cologne, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("CON", Location.Constanta, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("DUB", Location.Dublin, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("EDI", Location.Edinburgh, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("ENG", Location.EnglishChannel, Power.None, ConsoleColor.Cyan));
            tempDraculaCardDeck.Add(new DraculaCard("FLO", Location.Florence, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("FRA", Location.Frankfurt, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("GAT", Location.Galatz, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("GAW", Location.Galway, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("GEV", Location.Geneva, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("GEO", Location.Genoa, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("GRA", Location.Granada, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("HAM", Location.Hamburg, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("ION", Location.IonianSea, Power.None, ConsoleColor.Cyan));
            tempDraculaCardDeck.Add(new DraculaCard("IRI", Location.IrishSea, Power.None, ConsoleColor.Cyan));
            tempDraculaCardDeck.Add(new DraculaCard("KLA", Location.Klausenburg, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("LEH", Location.LeHavre, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("LEI", Location.Leipzig, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("LIS", Location.Lisbon, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("LIV", Location.Liverpool, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("LON", Location.London, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("MAD", Location.Madrid, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("MAN", Location.Manchester, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("MAR", Location.Marseilles, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("MED", Location.MediterraneanSea, Power.None, ConsoleColor.Cyan));
            tempDraculaCardDeck.Add(new DraculaCard("MIL", Location.Milan, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("MUN", Location.Munich, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("NAN", Location.Nantes, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("NAP", Location.Naples, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("NOR", Location.NorthSea, Power.None, ConsoleColor.Cyan));
            tempDraculaCardDeck.Add(new DraculaCard("NUR", Location.Nuremburg, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("PAR", Location.Paris, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("PLY", Location.Plymouth, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("PRA", Location.Prague, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("ROM", Location.Rome, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("SAL", Location.Salonica, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("SAN", Location.Santander, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("SAG", Location.Saragossa, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("SAJ", Location.Sarajevo, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("SOF", Location.Sofia, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("STR", Location.Strasbourg, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("SWA", Location.Swansea, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("SZE", Location.Szeged, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("TOU", Location.Toulouse, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("TYR", Location.TyrrhenianSea, Power.None, ConsoleColor.Cyan));
            tempDraculaCardDeck.Add(new DraculaCard("VAL", Location.Valona, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("VAR", Location.Varna, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("VEN", Location.Venice, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("VIE", Location.Vienna, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("ZAG", Location.Zagreb, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("ZUR", Location.Zurich, Power.None));
            tempDraculaCardDeck.Add(new DraculaCard("HID", Location.Nowhere, Power.Hide));
            tempDraculaCardDeck.Add(new DraculaCard("DAR", Location.Nowhere, Power.DarkCall, ConsoleColor.DarkGreen));
            tempDraculaCardDeck.Add(new DraculaCard("FEE", Location.Nowhere, Power.Feed, ConsoleColor.DarkGreen));
            tempDraculaCardDeck.Add(new DraculaCard("WOL", Location.Nowhere, Power.WolfForm, ConsoleColor.DarkGreen));
            tempDraculaCardDeck.Add(new DraculaCard("DOU", Location.Nowhere, Power.DoubleBack, ConsoleColor.DarkGreen));
            return tempDraculaCardDeck;
        }

        public void RevealAllVampires()
        {
            for (var i = 0; i < 6; i++)
            {
                if (Trail[i] != null)
                {
                    foreach (var enc in Trail[i].EncounterTiles)
                    {
                        if (enc.Encounter == Encounter.NewVampire)
                        {
                            enc.IsRevealed = true;
                        }
                    }
                }
            }
            for (var i = 0; i < 3; i++)
            {
                if (Catacombs[i] != null)
                {
                    foreach (var enc in Catacombs[i].EncounterTiles)
                    {
                        if (enc.Encounter == Encounter.NewVampire)
                        {
                            enc.IsRevealed = true;
                        }
                    }
                }
            }
        }

        public void AdjustBlood(int adjustment)
        {
            Blood += adjustment;
        }

        public void EscapeAsBat(GameState game, Location destination)
        {
            foreach (var card in Trail[0].DraculaCards)
            {
                card.IsRevealed = false;
            }
            foreach (var enc in Trail[0].EncounterTiles)
            {
                enc.IsRevealed = false;
                game.EncounterPool.Add(enc);
            }
            int i;
            for (i = 0; i < 6; i++)
            {
                if (Trail[i] != null && Trail[i].DraculaCards.First().Location == CurrentLocation)
                {
                    break;
                }
            }
            Trail[i] = new DraculaCardSlot(DraculaCardDeck.Find(card => card.Location == destination));

            if (destination == Location.CastleDracula)
            {
                Trail[i].DraculaCards.First().IsRevealed = true;
            }
            CurrentLocation = destination;
        }

        public int RevealCardInCatacombsWithLocation(Location location)
        {
            for (var i = 0; i < 3; i++)
            {
                if (Catacombs[i] != null && Catacombs[i].DraculaCards.First().Location == location)
                {
                    Catacombs[i].DraculaCards.First().IsRevealed = true;
                    return i + 6;
                }
            }
            return -1;
        }

        public void DiscardEncounterTile(GameState game, EncounterTile encounterToDiscard)
        {
            game.EncounterPool.Add(encounterToDiscard);
            EncounterHand.Remove(encounterToDiscard);
        }

        public void ClearTrailDownTo(GameState game, int length)
        {
            for (var i = 5; i >= length; i--)
            {
                if (Trail[i] != null)
                {
                    if (Trail[i].DraculaCards.First().Location != CurrentLocation)
                    {
                        foreach (var enc in Trail[i].EncounterTiles)
                        {
                            enc.IsRevealed = false;
                        }
                        game.EncounterPool.AddRange(Trail[i].EncounterTiles);
                        Trail[i].EncounterTiles.Clear();
                        foreach (var d in Trail[i].DraculaCards)
                        {
                            d.IsRevealed = false;
                        }
                        Trail[i] = null;
                    }
                    else
                    {
                        length--;
                    }
                }
            }
            for (var i = 0; i < 6; i++)
            {
                if (Trail[i] == null)
                {
                    for (var j = i + 1; j < 6; j++)
                    {
                        if (Trail[j] != null)
                        {
                            Trail[i] = Trail[j];
                            Trail[j] = null;
                            break;
                        }
                    }
                }
            }
        }

        public int NumberOfEncountersAtLocation(Location location)
        {
            for (var i = 0; i < 6; i++)
            {
                if (Trail[i] != null && Trail[i].DraculaCards.First().Location == location)
                {
                    return Trail[i].EncounterTiles.Count();
                }
            }
            for (var i = 0; i < 3; i++)
            {
                if (Catacombs[i] != null && Catacombs[i].DraculaCards.First().Location == location)
                {
                    return Catacombs[i].EncounterTiles.Count();
                }
            }
            return -1;
        }

        public DraculaCardSlot slotWhereHideCardIs()
        {
            for (int i = 0; i < 6; i++)
            {
                if (Trail[i] != null && Trail[i].DraculaCards.First().Power == Power.Hide)
                {
                    return Trail[i];
                }
            }
            return new DraculaCardSlot(new DraculaCard("NOW", Location.Nowhere, Power.None));
        }

        public int PositionWhereHideCardIs()
        {
            for (int i = 0; i < 6; i++)
            {
                if (Trail[i] != null && Trail[i].DraculaCards.First().Power == Power.Hide)
                {
                    return i;
                }
            }
            return -1;
        }

        public void TakePunishmentForCheating(GameState game)
        {
            Blood = (int)(Blood / 5) * 5;
            while (Trail[0].DraculaCards.First().Location != CurrentLocation)
            {
                foreach (DraculaCard card in Trail[0].DraculaCards)
                {
                    card.IsRevealed = false;
                }
                foreach (EncounterTile tile in Trail[0].EncounterTiles)
                {
                    tile.IsRevealed = false;
                    game.EncounterPool.Add(tile);
                }
                for (int i = 0; i < 5; i++)
                {
                    if (Trail[i] != null && Trail[i + 1] != null)
                    {
                        Trail[i] = Trail[i + 1];
                    }
                }
            }
            ClearTrailDownTo(game, 1);
            Trail[0].DraculaCards.First().IsRevealed = true;
        }
    }
}