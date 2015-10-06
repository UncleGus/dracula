﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationHandler;

namespace ConsoleHandler
{
    class Program
    {

        static void Main(string[] args)
        {
            Location galway = new Location();
            Location dublin = new Location();
            Location liverpool = new Location();
            Location edinburgh = new Location();
            Location manchester = new Location();
            Location swansea = new Location();
            Location plymouth = new Location();
            Location nantes = new Location();
            Location lehavre = new Location();
            Location london = new Location();
            Location paris = new Location();
            Location brussels = new Location();
            Location amsterdam = new Location();
            Location strasbourg = new Location();
            Location cologne = new Location();
            Location hamburg = new Location();
            Location frankfurt = new Location();
            Location nuremburg = new Location();
            Location leipzig = new Location();
            Location berlin = new Location();
            Location prague = new Location();
            Location castledracula = new Location();
            Location santander = new Location();
            Location saragossa = new Location();
            Location bordeaux = new Location();
            Location toulouse = new Location();
            Location barcelona = new Location();
            Location clermontferrand = new Location();
            Location marseilles = new Location();
            Location geneva = new Location();
            Location genoa = new Location();
            Location milan = new Location();
            Location zurich = new Location();
            Location florence = new Location();
            Location venice = new Location();
            Location munich = new Location();
            Location zagreb = new Location();
            Location vienna = new Location();
            Location stjosephandstmary = new Location();
            Location sarajevo = new Location();
            Location szeged = new Location();
            Location budapest = new Location();
            Location belgrade = new Location();
            Location klausenburg = new Location();
            Location sofia = new Location();
            Location bucharest = new Location();
            Location galatz = new Location();
            Location varna = new Location();
            Location constanta = new Location();
            Location lisbon = new Location();
            Location cadiz = new Location();
            Location madrid = new Location();
            Location granada = new Location();
            Location alicante = new Location();
            Location cagliari = new Location();
            Location rome = new Location();
            Location naples = new Location();
            Location bari = new Location();
            Location valona = new Location();
            Location salonica = new Location();
            Location athens = new Location();
            Location atlanticocean = new Location();
            Location irishsea = new Location();
            Location englishchannel = new Location();
            Location northsea = new Location();
            Location bayofbiscay = new Location();
            Location mediterraneansea = new Location();
            Location tyrrheniansea = new Location();
            Location adriaticsea = new Location();
            Location ioniansea = new Location();
            Location blacksea = new Location();

            List<Location> Map = new List<Location>();

            galway.name = "Galway";
            galway.abbreviation = "GAL";
            galway.type = LocationType.Town;
            galway.isEastern = false;
            galway.byRoad.Add(dublin);
            galway.bySea.Add(atlanticocean);
            Map.Add(galway);

            dublin.name = "Dublin";
            dublin.abbreviation = "DUB";
            dublin.type = LocationType.Town;
            dublin.isEastern = false;
            dublin.byRoad.Add(galway);
            dublin.bySea.Add(irishsea);
            Map.Add(dublin);

            liverpool.name = "Liverpool";
            liverpool.abbreviation = "LIV";
            liverpool.type = LocationType.City;
            liverpool.isEastern = false;
            liverpool.byRoad.Add(manchester);
            liverpool.byRoad.Add(swansea);
            liverpool.byTrain.Add(manchester);
            liverpool.bySea.Add(irishsea);
            Map.Add(liverpool);

            edinburgh.name = "Edinburgh";
            edinburgh.abbreviation = "EDI";
            edinburgh.type = LocationType.City;
            edinburgh.isEastern = false;
            edinburgh.byRoad.Add(manchester);
            edinburgh.byTrain.Add(manchester);
            edinburgh.bySea.Add(northsea);
            Map.Add(edinburgh);

            manchester.name = "Manchester";
            manchester.abbreviation = "MAN";
            manchester.type = LocationType.City;
            manchester.isEastern = false;
            manchester.byRoad.Add(edinburgh);
            manchester.byRoad.Add(liverpool);
            manchester.byRoad.Add(london);
            manchester.byTrain.Add(edinburgh);
            manchester.byTrain.Add(liverpool);
            manchester.byTrain.Add(london);
            Map.Add(manchester);

            swansea.name = "Swansea";
            swansea.abbreviation = "SWA";
            swansea.type = LocationType.Town;
            swansea.isEastern = false;
            swansea.byRoad.Add(liverpool);
            swansea.byRoad.Add(london);
            swansea.byTrain.Add(london);
            swansea.bySea.Add(irishsea);
            Map.Add(swansea);

            plymouth.name = "Plymouth";
            plymouth.abbreviation = "PLY";
            plymouth.type = LocationType.Town;
            plymouth.isEastern = false;
            plymouth.byRoad.Add(london);
            plymouth.bySea.Add(englishchannel);
            Map.Add(plymouth);

            nantes.name = "Nantes";
            nantes.abbreviation = "NAN";
            nantes.type = LocationType.City;
            nantes.isEastern = false;
            nantes.byRoad.Add(lehavre);
            nantes.byRoad.Add(paris);
            nantes.byRoad.Add(clermontferrand);
            nantes.byRoad.Add(bordeaux);
            nantes.bySea.Add(bayofbiscay);
            Map.Add(nantes);

            lehavre.name = "Le Havre";
            lehavre.abbreviation = "LEH";
            lehavre.type = LocationType.Town;
            lehavre.isEastern = false;
            lehavre.byRoad.Add(nantes);
            lehavre.byRoad.Add(paris);
            lehavre.byRoad.Add(brussels);
            lehavre.byTrain.Add(paris);
            lehavre.bySea.Add(englishchannel);
            Map.Add(lehavre);

            london.name = "London";
            london.abbreviation = "LON";
            london.type = LocationType.City;
            london.isEastern = false;
            london.byRoad.Add(manchester);
            london.byRoad.Add(swansea);
            london.byRoad.Add(plymouth);
            london.byTrain.Add(manchester);
            london.byTrain.Add(swansea);
            london.bySea.Add(englishchannel);
            Map.Add(london);

            paris.name = "Paris";
            paris.abbreviation = "PAR";
            paris.type = LocationType.City;
            paris.isEastern = false;
            paris.byRoad.Add(nantes);
            paris.byRoad.Add(lehavre);
            paris.byRoad.Add(brussels);
            paris.byRoad.Add(strasbourg);
            paris.byRoad.Add(geneva);
            paris.byRoad.Add(clermontferrand);
            paris.byTrain.Add(lehavre);
            paris.byTrain.Add(brussels);
            paris.byTrain.Add(marseilles);
            paris.byTrain.Add(bordeaux);
            Map.Add(paris);

            brussels.name = "Brussels";
            brussels.abbreviation = "BRU";
            brussels.type = LocationType.City;
            brussels.isEastern = false;
            brussels.byRoad.Add(lehavre);
            brussels.byRoad.Add(amsterdam);
            brussels.byRoad.Add(cologne);
            brussels.byRoad.Add(strasbourg);
            brussels.byRoad.Add(paris);
            brussels.byTrain.Add(cologne);
            brussels.byTrain.Add(paris);
            Map.Add(brussels);

            amsterdam.name = "Amsterdam";
            amsterdam.abbreviation = "AMS";
            amsterdam.type = LocationType.City;
            amsterdam.isEastern = false;
            amsterdam.byRoad.Add(brussels);
            amsterdam.byRoad.Add(cologne);
            amsterdam.bySea.Add(northsea);
            Map.Add(amsterdam);

            strasbourg.name = "Strasbourg";
            strasbourg.abbreviation = "STR";
            strasbourg.type = LocationType.Town;
            strasbourg.isEastern = false;
            strasbourg.byRoad.Add(paris);
            strasbourg.byRoad.Add(brussels);
            strasbourg.byRoad.Add(cologne);
            strasbourg.byRoad.Add(frankfurt);
            strasbourg.byRoad.Add(nuremburg);
            strasbourg.byRoad.Add(munich);
            strasbourg.byRoad.Add(zurich);
            strasbourg.byRoad.Add(geneva);
            strasbourg.byTrain.Add(frankfurt);
            strasbourg.byTrain.Add(zurich);
            Map.Add(strasbourg);

            cologne.name = "Cologne";
            cologne.abbreviation = "COL";
            cologne.type = LocationType.City;
            cologne.isEastern = false;
            cologne.byRoad.Add(brussels);
            cologne.byRoad.Add(amsterdam);
            cologne.byRoad.Add(hamburg);
            cologne.byRoad.Add(leipzig);
            cologne.byRoad.Add(frankfurt);
            cologne.byRoad.Add(strasbourg);
            cologne.byTrain.Add(brussels);
            cologne.byTrain.Add(frankfurt);
            Map.Add(cologne);

            hamburg.name = "Hamburg";
            hamburg.abbreviation = "HAM";
            hamburg.type = LocationType.City;
            hamburg.isEastern = false;
            hamburg.byRoad.Add(cologne);
            hamburg.byRoad.Add(berlin);
            hamburg.byRoad.Add(leipzig);
            hamburg.byTrain.Add(berlin);
            hamburg.bySea.Add(northsea);
            Map.Add(hamburg);

            frankfurt.name = "Frankfurt";
            frankfurt.abbreviation = "FRA";
            frankfurt.type = LocationType.Town;
            frankfurt.isEastern = false;
            frankfurt.byRoad.Add(strasbourg);
            frankfurt.byRoad.Add(cologne);
            frankfurt.byRoad.Add(leipzig);
            frankfurt.byRoad.Add(nuremburg);
            frankfurt.byTrain.Add(strasbourg);
            frankfurt.byTrain.Add(cologne);
            frankfurt.byTrain.Add(leipzig);
            Map.Add(frankfurt);

            nuremburg.name = "Nuremburg";
            nuremburg.abbreviation = "NUR";
            nuremburg.type = LocationType.Town;
            nuremburg.isEastern = false;
            nuremburg.byRoad.Add(strasbourg);
            nuremburg.byRoad.Add(frankfurt);
            nuremburg.byRoad.Add(leipzig);
            nuremburg.byRoad.Add(prague);
            nuremburg.byRoad.Add(munich);
            nuremburg.byTrain.Add(leipzig);
            nuremburg.byTrain.Add(munich);
            Map.Add(nuremburg);

            leipzig.name = "Leipzig";
            leipzig.abbreviation = "LEI";
            leipzig.type = LocationType.City;
            leipzig.isEastern = false;
            leipzig.byRoad.Add(cologne);
            leipzig.byRoad.Add(hamburg);
            leipzig.byRoad.Add(berlin);
            leipzig.byRoad.Add(nuremburg);
            leipzig.byRoad.Add(frankfurt);
            leipzig.byTrain.Add(frankfurt);
            leipzig.byTrain.Add(berlin);
            leipzig.byTrain.Add(nuremburg);
            Map.Add(leipzig);

            berlin.name = "Berlin";
            berlin.abbreviation = "BER";
            berlin.type = LocationType.City;
            berlin.isEastern = false;
            berlin.byRoad.Add(hamburg);
            berlin.byRoad.Add(prague);
            berlin.byRoad.Add(leipzig);
            berlin.byTrain.Add(hamburg);
            berlin.byTrain.Add(leipzig);
            berlin.byTrain.Add(prague);
            Map.Add(berlin);

            prague.name = "Prague";
            prague.abbreviation = "PRA";
            prague.type = LocationType.City;
            prague.isEastern = true;
            prague.byRoad.Add(berlin);
            prague.byRoad.Add(vienna);
            prague.byRoad.Add(nuremburg);
            prague.byTrain.Add(berlin);
            prague.byTrain.Add(vienna);
            Map.Add(prague);

            castledracula.name = "Castle Dracula";
            castledracula.abbreviation = "CAS";
            castledracula.type = LocationType.Castle;
            castledracula.isEastern = true;
            castledracula.byRoad.Add(klausenburg);
            castledracula.byRoad.Add(galatz);
            Map.Add(castledracula);

            santander.name = "Santander";
            santander.abbreviation = "SAN";
            santander.type = LocationType.Town;
            santander.isEastern = false;
            santander.byRoad.Add(lisbon);
            santander.byRoad.Add(madrid);
            santander.byRoad.Add(saragossa);
            santander.byTrain.Add(madrid);
            santander.bySea.Add(bayofbiscay);
            Map.Add(santander);

            saragossa.name = "Saragossa";
            saragossa.abbreviation = "SAG";
            saragossa.type = LocationType.Town;
            saragossa.isEastern = false;
            saragossa.byRoad.Add(madrid);
            saragossa.byRoad.Add(santander);
            saragossa.byRoad.Add(bordeaux);
            saragossa.byRoad.Add(toulouse);
            saragossa.byRoad.Add(barcelona);
            saragossa.byRoad.Add(alicante);
            saragossa.byTrain.Add(madrid);
            saragossa.byTrain.Add(bordeaux);
            saragossa.byTrain.Add(barcelona);
            Map.Add(saragossa);

            bordeaux.name = "Bordeaux";
            bordeaux.abbreviation = "BOR";
            bordeaux.type = LocationType.City;
            bordeaux.isEastern = false;
            bordeaux.byRoad.Add(saragossa);
            bordeaux.byRoad.Add(nantes);
            bordeaux.byRoad.Add(clermontferrand);
            bordeaux.byRoad.Add(toulouse);
            bordeaux.byTrain.Add(paris);
            bordeaux.byTrain.Add(saragossa);
            bordeaux.bySea.Add(bayofbiscay);
            Map.Add(bordeaux);

            toulouse.name = "Toulouse";
            toulouse.abbreviation = "TOU";
            toulouse.type = LocationType.Town;
            toulouse.isEastern = false;
            toulouse.byRoad.Add(saragossa);
            toulouse.byRoad.Add(bordeaux);
            toulouse.byRoad.Add(clermontferrand);
            toulouse.byRoad.Add(marseilles);
            toulouse.byRoad.Add(barcelona);
            Map.Add(toulouse);

            barcelona.name = "Barcelona";
            barcelona.abbreviation = "BAR";
            barcelona.type = LocationType.City;
            barcelona.isEastern = false;
            barcelona.byRoad.Add(saragossa);
            barcelona.byRoad.Add(toulouse);
            barcelona.byTrain.Add(saragossa);
            barcelona.byTrain.Add(alicante);
            barcelona.bySea.Add(mediterraneansea);
            Map.Add(barcelona);

            clermontferrand.name = "Clermont Ferrand";
            clermontferrand.abbreviation = "CLE";
            clermontferrand.type = LocationType.Town;
            clermontferrand.isEastern = false;
            clermontferrand.byRoad.Add(bordeaux);
            clermontferrand.byRoad.Add(nantes);
            clermontferrand.byRoad.Add(paris);
            clermontferrand.byRoad.Add(geneva);
            clermontferrand.byRoad.Add(marseilles);
            clermontferrand.byRoad.Add(toulouse);
            Map.Add(clermontferrand);

            marseilles.name = "Marseilles";
            marseilles.abbreviation = "MAR";
            marseilles.type = LocationType.City;
            marseilles.isEastern = false;
            marseilles.byRoad.Add(toulouse);
            marseilles.byRoad.Add(clermontferrand);
            marseilles.byRoad.Add(geneva);
            marseilles.byRoad.Add(zurich);
            marseilles.byRoad.Add(milan);
            marseilles.byRoad.Add(genoa);
            marseilles.byTrain.Add(paris);
            marseilles.bySea.Add(mediterraneansea);
            Map.Add(marseilles);

            geneva.name = "Geneva";
            geneva.abbreviation = "GEV";
            geneva.type = LocationType.Town;
            geneva.isEastern = false;
            geneva.byRoad.Add(marseilles);
            geneva.byRoad.Add(clermontferrand);
            geneva.byRoad.Add(paris);
            geneva.byRoad.Add(strasbourg);
            geneva.byRoad.Add(zurich);
            geneva.byTrain.Add(milan);
            Map.Add(geneva);

            genoa.name = "Genoa";
            genoa.abbreviation = "GEO";
            genoa.type = LocationType.City;
            genoa.isEastern = true;
            genoa.byRoad.Add(marseilles);
            genoa.byRoad.Add(milan);
            genoa.byRoad.Add(venice);
            genoa.byRoad.Add(florence);
            genoa.byTrain.Add(milan);
            genoa.bySea.Add(tyrrheniansea);
            Map.Add(genoa);

            milan.name = "Milan";
            milan.abbreviation = "MIL";
            milan.type = LocationType.City;
            milan.isEastern = true;
            milan.byRoad.Add(marseilles);
            milan.byRoad.Add(zurich);
            milan.byRoad.Add(munich);
            milan.byRoad.Add(venice);
            milan.byRoad.Add(genoa);
            milan.byTrain.Add(geneva);
            milan.byTrain.Add(zurich);
            milan.byTrain.Add(florence);
            milan.byTrain.Add(genoa);
            Map.Add(milan);

            zurich.name = "Zurich";
            zurich.abbreviation = "ZUR";
            zurich.type = LocationType.Town;
            zurich.isEastern = false;
            zurich.byRoad.Add(marseilles);
            zurich.byRoad.Add(geneva);
            zurich.byRoad.Add(strasbourg);
            zurich.byRoad.Add(munich);
            zurich.byRoad.Add(milan);
            zurich.byTrain.Add(strasbourg);
            zurich.byTrain.Add(milan);
            Map.Add(zurich);

            florence.name = "Florence";
            florence.abbreviation = "FLO";
            florence.type = LocationType.Town;
            florence.isEastern = true;
            florence.byRoad.Add(genoa);
            florence.byRoad.Add(venice);
            florence.byRoad.Add(rome);
            florence.byTrain.Add(milan);
            florence.byTrain.Add(rome);
            Map.Add(florence);

            venice.name = "Venice";
            venice.abbreviation = "Ven";
            venice.type = LocationType.Town;
            venice.isEastern = true;
            venice.byRoad.Add(florence);
            venice.byRoad.Add(genoa);
            venice.byRoad.Add(milan);
            venice.byRoad.Add(munich);
            venice.byTrain.Add(vienna);
            venice.bySea.Add(adriaticsea);
            Map.Add(venice);

            munich.name = "Munich";
            munich.abbreviation = "MUN";
            munich.type = LocationType.City;
            munich.isEastern = false;
            munich.byRoad.Add(milan);
            munich.byRoad.Add(zurich);
            munich.byRoad.Add(strasbourg);
            munich.byRoad.Add(nuremburg);
            munich.byRoad.Add(vienna);
            munich.byRoad.Add(zagreb);
            munich.byRoad.Add(venice);
            munich.byTrain.Add(nuremburg);
            Map.Add(munich);

            zagreb.name = "Zagreb";
            zagreb.abbreviation = "ZAG";
            zagreb.type = LocationType.Town;
            zagreb.isEastern = true;
            zagreb.byRoad.Add(munich);
            zagreb.byRoad.Add(vienna);
            zagreb.byRoad.Add(budapest);
            zagreb.byRoad.Add(szeged);
            zagreb.byRoad.Add(stjosephandstmary);
            zagreb.byRoad.Add(sarajevo);
            Map.Add(zagreb);

            vienna.name = "Vienna";
            vienna.abbreviation = "VIE";
            vienna.type = LocationType.City;
            vienna.isEastern = true;
            vienna.byRoad.Add(munich);
            vienna.byRoad.Add(prague);
            vienna.byRoad.Add(budapest);
            vienna.byRoad.Add(zagreb);
            vienna.byTrain.Add(venice);
            vienna.byTrain.Add(prague);
            vienna.byTrain.Add(budapest);
            Map.Add(vienna);

            stjosephandstmary.name = "St. Joseph & St. Mary";
            stjosephandstmary.abbreviation = "STJ";
            stjosephandstmary.type = LocationType.Hospital;
            stjosephandstmary.isEastern = true;
            stjosephandstmary.byRoad.Add(zagreb);
            stjosephandstmary.byRoad.Add(szeged);
            stjosephandstmary.byRoad.Add(belgrade);
            stjosephandstmary.byRoad.Add(sarajevo);
            Map.Add(stjosephandstmary);

            sarajevo.name = "Sarajevo";
            sarajevo.abbreviation = "SAJ";
            sarajevo.type = LocationType.Town;
            sarajevo.isEastern = true;
            sarajevo.byRoad.Add(zagreb);
            sarajevo.byRoad.Add(stjosephandstmary);
            sarajevo.byRoad.Add(belgrade);
            sarajevo.byRoad.Add(sofia);
            sarajevo.byRoad.Add(valona);
            Map.Add(sarajevo);

            szeged.name = "Szeged";
            szeged.abbreviation = "SZE";
            szeged.type = LocationType.Town;
            szeged.isEastern = true;
            szeged.byRoad.Add(zagreb);
            szeged.byRoad.Add(budapest);
            szeged.byRoad.Add(klausenburg);
            szeged.byRoad.Add(belgrade);
            szeged.byRoad.Add(stjosephandstmary);
            szeged.byTrain.Add(budapest);
            szeged.byTrain.Add(bucharest);
            szeged.byTrain.Add(belgrade);
            Map.Add(szeged);

            string line;
            string command;
            string argument1;
            do
            {
                line = Console.ReadLine();
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
                }
                catch (ArgumentOutOfRangeException)
                {
                    argument1 = "no argument";
                }

                switch (command)
                {
                    case "showlocationdetails": LocationHelper.ShowLocationDetails(GetLocationFromName(argument1, Map)); break;
                }

            } while (command != "exit");
        }

        public static Location GetLocationFromName(string locationName, List<Location> locationList)
        {
            for (int i = 0; i < locationList.Count(); i++)
            {
                if (locationList[i].name == locationName)
                {
                    return locationList[i];
                }
            }
            Location unknownLocation = new Location();
            unknownLocation.name = "Unknown location";
            return unknownLocation;
        }

    }
}
