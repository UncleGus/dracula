using DraculaSimulator;
using EncounterHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LocationHandler
{
    public class LocationDetail
    {
        public Location Index;
        public string Name;
        public string Abbreviation;
        public LocationType Type;
        public bool IsEastern;
        public List<Location> ByRoad;
        public List<Location> ByTrain;
        public List<Location> BySea;

        public LocationDetail()
        {
            ByRoad = new List<Location>();
            ByTrain = new List<Location>();
            BySea = new List<Location>();
        }

        // move to UserInterface
        public void DrawLocation(GameState g)
        {
            if (Type == LocationType.Sea)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else if (Type == LocationType.Power)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            if (g.RevealedLocations.Contains(Index))
            {
                Console.Write(Abbreviation + " ");
            }
            else
            {
                Console.Write("### ");
            }
            Console.ResetColor();
        }

        // move to UserInterface
        public void DrawEncounter()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (Encounters.Count() > 0)
            {
                if (Encounters[0].isRevealed)
                {
                    Console.Write(Encounters[0].abbreviation + " ");
                }
                else
                {
                    Console.Write(" ■  ");
                }
            }
            else
            {
                Console.Write("    ");
            }
        }

        // move to UserInterface
        public void DrawEncounter(bool drawingSecondEncounter)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (Encounters.Count() > 1)
            {
                if (Encounters[1].isRevealed)
                {
                    Console.Write(Encounters[1].abbreviation + " ");
                }
                else
                {
                    Console.Write(" ■  ");
                }
            }
            else
            {
                Console.Write("    ");
            }
        }
    }

    public enum LocationType
    {
        None,
        Town,
        City,
        Sea,
        Castle,
        Hospital,
        Power
    }

    public enum Location
    {
        Nowhere,
        AdriaticSea,
        Alicante,
        Amsterdam,
        Athens,
        AtlanticOcean,
        Barcelona,
        Bari,
        BayOfBiscay,
        Belgrade,
        Berlin,
        BlackSea,
        Bordeaux,
        Brussels,
        Bucharest,
        Budapest,
        Cadiz,
        Cagliari,
        CastleDracula,
        ClermontFerrand,
        Cologne,
        Constanta,
        Dublin,
        Edinburgh,
        EnglishChannel,
        Florence,
        Frankfurt,
        Galatz,
        Galway,
        Geneva,
        Genoa,
        Granada,
        Hamburg,
        IonianSea,
        IrishSea,
        Klausenburg,
        LeHavre,
        Leipzig,
        Lisbon,
        Liverpool,
        London,
        Madrid,
        Manchester,
        Marseilles,
        MediterraneanSea,
        Milan,
        Munich,
        Nantes,
        Naples,
        NorthSea,
        Nuremburg,
        Paris,
        Plymouth,
        Prague,
        Rome,
        Salonica,
        Santander,
        Saragossa,
        Sarajevo,
        Sofia,
        StJosephAndStMary,
        Strasbourg,
        Swansea,
        Szeged,
        Toulouse,
        TyrrhenianSea,
        Valona,
        Varna,
        Venice,
        Vienna,
        Zagreb,
        Zurich
    }
}
