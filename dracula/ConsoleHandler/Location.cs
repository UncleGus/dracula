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
        public List<Encounter> Encounters;

        public LocationDetail()
        {
            ByRoad = new List<Location>();
            ByTrain = new List<Location>();
            BySea = new List<Location>();
            Encounters = new List<Encounter>();
        }

        // move to Map
        public bool IsPort()
        {
            return BySea.Count() > 0 && (Type == LocationType.Town || Type == LocationType.City);
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
        Town,
        City,
        Sea,
        Castle,
        Hospital,
        Power
    }

    public enum Location
    {
        adriaticsea,
        alicante,
        amsterdam,
        athens,
        atlanticocean,
        barcelona,
        bari,
        bayofbiscay,
        belgrade,
        berlin,
        blacksea,
        bordeaux,
        brussels,
        bucharest,
        budapest,
        cadiz,
        cagliari,
        castledracula,
        clermontferrand,
        cologne,
        constanta,
        dublin,
        edinburgh,
        englishchannel,
        florence,
        frankfurt,
        galatz,
        galway,
        geneva,
        genoa,
        granada,
        hamburg,
        ioniansea,
        irishsea,
        klausenburg,
        lehavre,
        leipzig,
        lisbon,
        liverpool,
        london,
        madrid,
        manchester,
        marseilles,
        mediterraneansea,
        milan,
        munich,
        nantes,
        naples,
        northsea,
        nuremburg,
        paris,
        plymouth,
        prague,
        rome,
        salonica,
        santander,
        saragossa,
        sarajevo,
        sofia,
        stjosephandstmary,
        strasbourg,
        swansea,
        szeged,
        toulouse,
        tyrrheniansea,
        valona,
        varna,
        venice,
        vienna,
        zagreb,
        zurich
    }
}
