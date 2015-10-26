using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    [DataContract]
    public class Dracula
    {
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
        private List<DraculaCard> _draculaCardDeck;
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
            private set
            {
                _draculaCardDeck = value;
            }
        }


        [DataMember]
        public Location LocationWhereHideWasUsed { get; private set; }

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

        public DraculaCardSlot MoveTo(Location destination, Power power)
        {
            DraculaCardSlot doubleBackedCard = null;
            int catacombsSlot = 0;
            if (power == Power.DoubleBack)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (Trail[i] != null && Trail[i].DraculaCards.First().Location == destination)
                    {
                        doubleBackedCard = Trail[i];
                        for (int j = i; j > 0; j--)
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
                for (int i = 0; i < 3; i++)
                {
                    if (Catacombs[i] != null && Catacombs[i].DraculaCards.First().Location == destination)
                    {
                        doubleBackedCard = Catacombs[i];
                        catacombsSlot = i;
                    }
                }
            }
            DraculaCardSlot cardSlotDroppedOffTrail = Trail[5];
            for (int i = 5; i > 0; i--)
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

        public void RevealEncountersAtPositionInTrail(GameState game, int positionRevealed)
        {
            if (positionRevealed < 6)
            {
                foreach (EncounterTile enc in Trail[positionRevealed].EncounterTiles)
                {
                    enc.IsRevealed = true;
                }
            }
            else
            {
                positionRevealed -= 6;
                foreach (EncounterTile enc in Catacombs[positionRevealed].EncounterTiles)
                {
                    enc.IsRevealed = true;
                }
            }
        }

        public int RevealCardInTrailWithLocation(Location location)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Trail[i].DraculaCards.First().Location == location)
                {
                    Trail[i].DraculaCards.First().IsRevealed = true;
                    return i;
                }
            }
            return -1;
        }

        public void DiscardEvent(EventCard eventToDiscard, List<EventCard> discardPile)
        {
            EventHand.Remove(eventToDiscard);
            discardPile.Add(eventToDiscard);
        }

        public void DiscardCatacombsCards(GameState game, List<int> list)
        {
            List<EncounterTile> encounterTilesDiscarded = new List<EncounterTile>();
            foreach (int i in list)
            {
                encounterTilesDiscarded.AddRange(Catacombs[i].EncounterTiles);
                Catacombs[i] = null;
            }
            foreach (EncounterTile enc in encounterTilesDiscarded)
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
                    foreach (DraculaCard card in Trail[position].DraculaCards)
                    {
                        card.IsRevealed = true;
                    }
                    return true;
                }
            }
            else if (Catacombs[position - 6] != null)
            {
                foreach (DraculaCard card in Catacombs[position - 6].DraculaCards)
                {
                    card.IsRevealed = true;
                }
                return true;
            }
            return false;
        }

        public void DiscardEncounterTileFromCardSlot(EncounterTile encounterTileToDiscard, DraculaCardSlot draculaCardSlot, List<EncounterTile> encounterPool)
        {
            draculaCardSlot.EncounterTiles.Remove(encounterTileToDiscard);
            encounterPool.Add(encounterTileToDiscard);
        }

        public void DrawEncounter(List<EncounterTile> encounterPool)
        {
            EncounterTile encounterDrawn = encounterPool[new Random().Next(0, encounterPool.Count())];
            EncounterHand.Add(encounterDrawn);
            encounterPool.Remove(encounterDrawn);
        }

        public void PlaceEncounterTileOnCard(EncounterTile encounterToPlace, DraculaCardSlot card)
        {
            if (encounterToPlace != null)
            {
                card.EncounterTiles.Add(encounterToPlace);
                EncounterHand.Remove(encounterToPlace);
            }
        }

        public List<EncounterTile> DiscardHide()
        {
            List<EncounterTile> encountersDiscarded = new List<EncounterTile>();
            for (int i = 0; i < 6; i++)
            {
                if (Trail[i].DraculaCards.First().Power == Power.Hide)
                {
                    Trail[i].DraculaCards.First().IsRevealed = false;
                    LocationWhereHideWasUsed = Location.Nowhere;
                    foreach (EncounterTile enc in Trail[i].EncounterTiles)
                    {
                        encountersDiscarded.Add(enc);
                    }
                    for (int j = i; j < 5; j++)
                    {
                        Trail[j] = Trail[j + 1];
                    }
                    Trail[5] = null;
                    return encountersDiscarded;
                }
            }
            return encountersDiscarded;
        }

        public bool TakeEvent(List<EventCard> eventDeck)
        {
            EventCard cardDrawn;
            do
            {
                cardDrawn = eventDeck[new Random().Next(0, eventDeck.Count())];
            } while (!cardDrawn.IsDraculaCard);
            EventHand.Add(cardDrawn);
            eventDeck.Remove(cardDrawn);
            if (EventHand.Count() > EventHandSize)
            {
                return true;
            }
            return false;
        }

        public int RevealHideCard()
        {
            for (int i = 0; i < 6; i++)
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
            List<DraculaCard> tempDraculaCardDeck = new List<DraculaCard>();
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

        public void AdjustBlood(int adjustment)
        {
            Blood += adjustment;
        }

        public void EscapeAsBat(GameState game, Location destination)
        {
            foreach (DraculaCard card in Trail[0].DraculaCards)
            {
                card.IsRevealed = false;
            }
            foreach (EncounterTile enc in Trail[0].EncounterTiles)
            {
                enc.IsRevealed = false;
                game.EncounterPool.Add(enc);
            }
            Trail[0] = new DraculaCardSlot(DraculaCardDeck.Find(card => card.Location == destination));
            if (destination == Location.CastleDracula) {
                Trail[0].DraculaCards.First().IsRevealed = true;
            }
            CurrentLocation = destination;
        }

        public void DiscardEncounterTile(List<EncounterTile> encounterPool)
        {
            EncounterTile encounterToDiscard = EncounterHand[new Random().Next(0, EncounterHand.Count())];
            encounterPool.Add(encounterToDiscard);
            EncounterHand.Remove(encounterToDiscard);
        }
    }
}
