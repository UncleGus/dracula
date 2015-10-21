using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    public class DraculaCardSet
    {
        private DraculaCard[] draculaCardDeck;

        public DraculaCardSet()
        {
            draculaCardDeck = CreateDraculaCardDeck();
        }

        private DraculaCard[] CreateDraculaCardDeck()
        {
            DraculaCard[] tempDraculaCardDeck = new DraculaCard[77];
            tempDraculaCardDeck[0] = new DraculaCard("NOW", Location.Nowhere, Power.None);
            tempDraculaCardDeck[1] = new DraculaCard("ADR", Location.AdriaticSea, Power.None);
            tempDraculaCardDeck[2] = new DraculaCard("ALI", Location.Alicante, Power.None);
            tempDraculaCardDeck[3] = new DraculaCard("AMS", Location.Amsterdam, Power.None);
            tempDraculaCardDeck[4] = new DraculaCard("ATH", Location.Athens, Power.None);
            tempDraculaCardDeck[5] = new DraculaCard("ATL", Location.AtlanticOcean, Power.None);
            tempDraculaCardDeck[6] = new DraculaCard("BAC", Location.Barcelona, Power.None);
            tempDraculaCardDeck[7] = new DraculaCard("BAI", Location.Bari, Power.None);
            tempDraculaCardDeck[8] = new DraculaCard("BAY", Location.BayOfBiscay, Power.None);
            tempDraculaCardDeck[9] = new DraculaCard("BEL", Location.Belgrade, Power.None);
            tempDraculaCardDeck[10] = new DraculaCard("BER", Location.Berlin, Power.None);
            tempDraculaCardDeck[11] = new DraculaCard("BLA", Location.BlackSea, Power.None);
            tempDraculaCardDeck[12] = new DraculaCard("BOR", Location.Bordeaux, Power.None);
            tempDraculaCardDeck[13] = new DraculaCard("BRU", Location.Brussels, Power.None);
            tempDraculaCardDeck[14] = new DraculaCard("BUC", Location.Bucharest, Power.None);
            tempDraculaCardDeck[15] = new DraculaCard("BUD", Location.Budapest, Power.None);
            tempDraculaCardDeck[16] = new DraculaCard("CAD", Location.Cadiz, Power.None);
            tempDraculaCardDeck[17] = new DraculaCard("CAG", Location.Cagliari, Power.None);
            tempDraculaCardDeck[18] = new DraculaCard("CAS", Location.CastleDracula, Power.None);
            tempDraculaCardDeck[19] = new DraculaCard("CLE", Location.ClermontFerrand, Power.None);
            tempDraculaCardDeck[20] = new DraculaCard("COL", Location.Cologne, Power.None);
            tempDraculaCardDeck[21] = new DraculaCard("CON", Location.Constanta, Power.None);
            tempDraculaCardDeck[22] = new DraculaCard("DUB", Location.Dublin, Power.None);
            tempDraculaCardDeck[23] = new DraculaCard("EDI", Location.Edinburgh, Power.None);
            tempDraculaCardDeck[24] = new DraculaCard("ENG", Location.EnglishChannel, Power.None);
            tempDraculaCardDeck[25] = new DraculaCard("FLO", Location.Florence, Power.None);
            tempDraculaCardDeck[26] = new DraculaCard("FRA", Location.Frankfurt, Power.None);
            tempDraculaCardDeck[27] = new DraculaCard("GAT", Location.Galatz, Power.None);
            tempDraculaCardDeck[28] = new DraculaCard("GAW", Location.Galway, Power.None);
            tempDraculaCardDeck[29] = new DraculaCard("GEV", Location.Geneva, Power.None);
            tempDraculaCardDeck[30] = new DraculaCard("GEO", Location.Genoa, Power.None);
            tempDraculaCardDeck[31] = new DraculaCard("GRA", Location.Granada, Power.None);
            tempDraculaCardDeck[32] = new DraculaCard("HAM", Location.Hamburg, Power.None);
            tempDraculaCardDeck[33] = new DraculaCard("ION", Location.IonianSea, Power.None);
            tempDraculaCardDeck[34] = new DraculaCard("IRI", Location.IrishSea, Power.None);
            tempDraculaCardDeck[35] = new DraculaCard("KLA", Location.Klausenburg, Power.None);
            tempDraculaCardDeck[36] = new DraculaCard("LEH", Location.LeHavre, Power.None);
            tempDraculaCardDeck[37] = new DraculaCard("LEI", Location.Leipzig, Power.None);
            tempDraculaCardDeck[38] = new DraculaCard("LIS", Location.Lisbon, Power.None);
            tempDraculaCardDeck[39] = new DraculaCard("LIV", Location.Liverpool, Power.None);
            tempDraculaCardDeck[40] = new DraculaCard("LON", Location.London, Power.None);
            tempDraculaCardDeck[41] = new DraculaCard("MAD", Location.Madrid, Power.None);
            tempDraculaCardDeck[42] = new DraculaCard("MAN", Location.Manchester, Power.None);
            tempDraculaCardDeck[43] = new DraculaCard("MAR", Location.Marseilles, Power.None);
            tempDraculaCardDeck[44] = new DraculaCard("MED", Location.MediterraneanSea, Power.None);
            tempDraculaCardDeck[45] = new DraculaCard("MIL", Location.Milan, Power.None);
            tempDraculaCardDeck[46] = new DraculaCard("MUN", Location.Munich, Power.None);
            tempDraculaCardDeck[47] = new DraculaCard("NAN", Location.Nantes, Power.None);
            tempDraculaCardDeck[48] = new DraculaCard("NAP", Location.Naples, Power.None);
            tempDraculaCardDeck[49] = new DraculaCard("NOR", Location.NorthSea, Power.None);
            tempDraculaCardDeck[50] = new DraculaCard("NUR", Location.Nuremburg, Power.None);
            tempDraculaCardDeck[51] = new DraculaCard("PAR", Location.Paris, Power.None);
            tempDraculaCardDeck[52] = new DraculaCard("PLY", Location.Plymouth, Power.None);
            tempDraculaCardDeck[53] = new DraculaCard("PRA", Location.Prague, Power.None);
            tempDraculaCardDeck[54] = new DraculaCard("ROM", Location.Rome, Power.None);
            tempDraculaCardDeck[55] = new DraculaCard("SAL", Location.Salonica, Power.None);
            tempDraculaCardDeck[56] = new DraculaCard("SAN", Location.Santander, Power.None);
            tempDraculaCardDeck[57] = new DraculaCard("SAG", Location.Saragossa, Power.None);
            tempDraculaCardDeck[58] = new DraculaCard("SAJ", Location.Sarajevo, Power.None);
            tempDraculaCardDeck[59] = new DraculaCard("SOF", Location.Sofia, Power.None);
            tempDraculaCardDeck[61] = new DraculaCard("STR", Location.Strasbourg, Power.None);
            tempDraculaCardDeck[62] = new DraculaCard("SWA", Location.Swansea, Power.None);
            tempDraculaCardDeck[63] = new DraculaCard("SZE", Location.Szeged, Power.None);
            tempDraculaCardDeck[64] = new DraculaCard("TOU", Location.Toulouse, Power.None);
            tempDraculaCardDeck[65] = new DraculaCard("TYR", Location.TyrrhenianSea, Power.None);
            tempDraculaCardDeck[66] = new DraculaCard("VAL", Location.Valona, Power.None);
            tempDraculaCardDeck[67] = new DraculaCard("VAR", Location.Varna, Power.None);
            tempDraculaCardDeck[68] = new DraculaCard("VEN", Location.Venice, Power.None);
            tempDraculaCardDeck[69] = new DraculaCard("VIE", Location.Vienna, Power.None);
            tempDraculaCardDeck[70] = new DraculaCard("ZAG", Location.Zagreb, Power.None);
            tempDraculaCardDeck[71] = new DraculaCard("ZUR", Location.Zurich, Power.None);
            tempDraculaCardDeck[72] = new DraculaCard("HID", Location.Nowhere, Power.Hide);
            tempDraculaCardDeck[73] = new DraculaCard("DAR", Location.Nowhere, Power.DarkCall);
            tempDraculaCardDeck[74] = new DraculaCard("FEE", Location.Nowhere, Power.Feed);
            tempDraculaCardDeck[75] = new DraculaCard("WOL", Location.Nowhere, Power.WolfForm);
            tempDraculaCardDeck[76] = new DraculaCard("DOU", Location.Nowhere, Power.DoubleBack);
            return tempDraculaCardDeck;
        }

        public DraculaCard GetDraculaCardForLocation(Location location)
        {
            return draculaCardDeck[(int)location];
        }

        public DraculaCard GetDraculaCardForPower(Power power)
        {
            for (int i = 72; i < 77; i ++ )
            {
                if (draculaCardDeck[i].Power == power)
                {
                    return draculaCardDeck[i];
                }
            }
            return null;
        }
    }
}
