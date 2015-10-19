using LocationHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraculaSimulator
{
    public class Map
    {
        private LocationDetail[] network;

        public Map()
        {
            network = CreateNetwork();
        }

        private LocationDetail[] CreateNetwork()
        {
            LocationDetail[] tempMap = new LocationDetail[72];
            LocationDetail nowhere = new LocationDetail();
            LocationDetail galway = new LocationDetail();
            LocationDetail dublin = new LocationDetail();
            LocationDetail liverpool = new LocationDetail();
            LocationDetail edinburgh = new LocationDetail();
            LocationDetail manchester = new LocationDetail();
            LocationDetail swansea = new LocationDetail();
            LocationDetail plymouth = new LocationDetail();
            LocationDetail nantes = new LocationDetail();
            LocationDetail lehavre = new LocationDetail();
            LocationDetail london = new LocationDetail();
            LocationDetail paris = new LocationDetail();
            LocationDetail brussels = new LocationDetail();
            LocationDetail amsterdam = new LocationDetail();
            LocationDetail strasbourg = new LocationDetail();
            LocationDetail cologne = new LocationDetail();
            LocationDetail hamburg = new LocationDetail();
            LocationDetail frankfurt = new LocationDetail();
            LocationDetail nuremburg = new LocationDetail();
            LocationDetail leipzig = new LocationDetail();
            LocationDetail berlin = new LocationDetail();
            LocationDetail prague = new LocationDetail();
            LocationDetail castledracula = new LocationDetail();
            LocationDetail santander = new LocationDetail();
            LocationDetail saragossa = new LocationDetail();
            LocationDetail bordeaux = new LocationDetail();
            LocationDetail toulouse = new LocationDetail();
            LocationDetail barcelona = new LocationDetail();
            LocationDetail clermontferrand = new LocationDetail();
            LocationDetail marseilles = new LocationDetail();
            LocationDetail geneva = new LocationDetail();
            LocationDetail genoa = new LocationDetail();
            LocationDetail milan = new LocationDetail();
            LocationDetail zurich = new LocationDetail();
            LocationDetail florence = new LocationDetail();
            LocationDetail venice = new LocationDetail();
            LocationDetail munich = new LocationDetail();
            LocationDetail zagreb = new LocationDetail();
            LocationDetail vienna = new LocationDetail();
            LocationDetail stjosephandstmary = new LocationDetail();
            LocationDetail sarajevo = new LocationDetail();
            LocationDetail szeged = new LocationDetail();
            LocationDetail budapest = new LocationDetail();
            LocationDetail belgrade = new LocationDetail();
            LocationDetail klausenburg = new LocationDetail();
            LocationDetail sofia = new LocationDetail();
            LocationDetail bucharest = new LocationDetail();
            LocationDetail galatz = new LocationDetail();
            LocationDetail varna = new LocationDetail();
            LocationDetail constanta = new LocationDetail();
            LocationDetail lisbon = new LocationDetail();
            LocationDetail cadiz = new LocationDetail();
            LocationDetail madrid = new LocationDetail();
            LocationDetail granada = new LocationDetail();
            LocationDetail alicante = new LocationDetail();
            LocationDetail cagliari = new LocationDetail();
            LocationDetail rome = new LocationDetail();
            LocationDetail naples = new LocationDetail();
            LocationDetail bari = new LocationDetail();
            LocationDetail valona = new LocationDetail();
            LocationDetail salonica = new LocationDetail();
            LocationDetail athens = new LocationDetail();
            LocationDetail atlanticocean = new LocationDetail();
            LocationDetail irishsea = new LocationDetail();
            LocationDetail englishchannel = new LocationDetail();
            LocationDetail northsea = new LocationDetail();
            LocationDetail bayofbiscay = new LocationDetail();
            LocationDetail mediterraneansea = new LocationDetail();
            LocationDetail tyrrheniansea = new LocationDetail();
            LocationDetail adriaticsea = new LocationDetail();
            LocationDetail ioniansea = new LocationDetail();
            LocationDetail blacksea = new LocationDetail();

            nowhere.Name = "Nowhere";
            nowhere.Abbreviation = "NOW";
            tempMap[(int)Location.Nowhere] = nowhere;

            galway.Name = "Galway";
            galway.Abbreviation = "GAW";
            galway.Type = LocationType.Town;
            galway.IsEastern = false;
            galway.ByRoad.Add(Location.Dublin);
            galway.BySea.Add(Location.AtlanticOcean);
            tempMap[(int)Location.Galway] = galway;

            dublin.Name = "Dublin";
            dublin.Abbreviation = "DUB";
            dublin.Type = LocationType.Town;
            dublin.IsEastern = false;
            dublin.ByRoad.Add(Location.Galway);
            dublin.BySea.Add(Location.IrishSea);
            tempMap[(int)Location.Dublin] = dublin;

            liverpool.Name = "Liverpool";
            liverpool.Abbreviation = "LIV";
            liverpool.Type = LocationType.City;
            liverpool.IsEastern = false;
            liverpool.ByRoad.Add(Location.Manchester);
            liverpool.ByRoad.Add(Location.Swansea);
            liverpool.ByTrain.Add(Location.Manchester);
            liverpool.BySea.Add(Location.IrishSea);
            tempMap[(int)Location.Liverpool] = liverpool;

            edinburgh.Name = "Edinburgh";
            edinburgh.Abbreviation = "EDI";
            edinburgh.Type = LocationType.City;
            edinburgh.IsEastern = false;
            edinburgh.ByRoad.Add(Location.Manchester);
            edinburgh.ByTrain.Add(Location.Manchester);
            edinburgh.BySea.Add(Location.NorthSea);
            tempMap[(int)Location.Edinburgh] = edinburgh;

            manchester.Name = "Manchester";
            manchester.Abbreviation = "MAN";
            manchester.Type = LocationType.City;
            manchester.IsEastern = false;
            manchester.ByRoad.Add(Location.Edinburgh);
            manchester.ByRoad.Add(Location.Liverpool);
            manchester.ByRoad.Add(Location.London);
            manchester.ByTrain.Add(Location.Edinburgh);
            manchester.ByTrain.Add(Location.Liverpool);
            manchester.ByTrain.Add(Location.London);
            tempMap[(int)Location.Manchester] = manchester;

            swansea.Name = "Swansea";
            swansea.Abbreviation = "SWA";
            swansea.Type = LocationType.Town;
            swansea.IsEastern = false;
            swansea.ByRoad.Add(Location.Liverpool);
            swansea.ByRoad.Add(Location.London);
            swansea.ByTrain.Add(Location.London);
            swansea.BySea.Add(Location.IrishSea);
            tempMap[(int)Location.Swansea] = swansea;

            plymouth.Name = "Plymouth";
            plymouth.Abbreviation = "PLY";
            plymouth.Type = LocationType.Town;
            plymouth.IsEastern = false;
            plymouth.ByRoad.Add(Location.London);
            plymouth.BySea.Add(Location.EnglishChannel);
            tempMap[(int)Location.Plymouth] = plymouth;

            nantes.Name = "Nantes";
            nantes.Abbreviation = "NAN";
            nantes.Type = LocationType.City;
            nantes.IsEastern = false;
            nantes.ByRoad.Add(Location.LeHavre);
            nantes.ByRoad.Add(Location.Paris);
            nantes.ByRoad.Add(Location.ClermontFerrand);
            nantes.ByRoad.Add(Location.Bordeaux);
            nantes.BySea.Add(Location.BayOfBiscay);
            tempMap[(int)Location.Nantes] = nantes;

            lehavre.Name = "Le Havre";
            lehavre.Abbreviation = "LEH";
            lehavre.Type = LocationType.Town;
            lehavre.IsEastern = false;
            lehavre.ByRoad.Add(Location.Nantes);
            lehavre.ByRoad.Add(Location.Paris);
            lehavre.ByRoad.Add(Location.Brussels);
            lehavre.ByTrain.Add(Location.Paris);
            lehavre.BySea.Add(Location.EnglishChannel);
            tempMap[(int)Location.LeHavre] = lehavre;

            london.Name = "London";
            london.Abbreviation = "LON";
            london.Type = LocationType.City;
            london.IsEastern = false;
            london.ByRoad.Add(Location.Manchester);
            london.ByRoad.Add(Location.Swansea);
            london.ByRoad.Add(Location.Plymouth);
            london.ByTrain.Add(Location.Manchester);
            london.ByTrain.Add(Location.Swansea);
            london.BySea.Add(Location.EnglishChannel);
            tempMap[(int)Location.London] = london;

            paris.Name = "Paris";
            paris.Abbreviation = "PAR";
            paris.Type = LocationType.City;
            paris.IsEastern = false;
            paris.ByRoad.Add(Location.Nantes);
            paris.ByRoad.Add(Location.LeHavre);
            paris.ByRoad.Add(Location.Brussels);
            paris.ByRoad.Add(Location.Strasbourg);
            paris.ByRoad.Add(Location.Geneva);
            paris.ByRoad.Add(Location.ClermontFerrand);
            paris.ByTrain.Add(Location.LeHavre);
            paris.ByTrain.Add(Location.Brussels);
            paris.ByTrain.Add(Location.Marseilles);
            paris.ByTrain.Add(Location.Bordeaux);
            tempMap[(int)Location.Paris] = paris;

            brussels.Name = "Brussels";
            brussels.Abbreviation = "BRU";
            brussels.Type = LocationType.City;
            brussels.IsEastern = false;
            brussels.ByRoad.Add(Location.LeHavre);
            brussels.ByRoad.Add(Location.Amsterdam);
            brussels.ByRoad.Add(Location.Cologne);
            brussels.ByRoad.Add(Location.Strasbourg);
            brussels.ByRoad.Add(Location.Paris);
            brussels.ByTrain.Add(Location.Cologne);
            brussels.ByTrain.Add(Location.Paris);
            tempMap[(int)Location.Brussels] = brussels;

            amsterdam.Name = "Amsterdam";
            amsterdam.Abbreviation = "AMS";
            amsterdam.Type = LocationType.City;
            amsterdam.IsEastern = false;
            amsterdam.ByRoad.Add(Location.Brussels);
            amsterdam.ByRoad.Add(Location.Cologne);
            amsterdam.BySea.Add(Location.NorthSea);
            tempMap[(int)Location.Amsterdam] = amsterdam;

            strasbourg.Name = "Strasbourg";
            strasbourg.Abbreviation = "STR";
            strasbourg.Type = LocationType.Town;
            strasbourg.IsEastern = false;
            strasbourg.ByRoad.Add(Location.Paris);
            strasbourg.ByRoad.Add(Location.Brussels);
            strasbourg.ByRoad.Add(Location.Cologne);
            strasbourg.ByRoad.Add(Location.Frankfurt);
            strasbourg.ByRoad.Add(Location.Nuremburg);
            strasbourg.ByRoad.Add(Location.Munich);
            strasbourg.ByRoad.Add(Location.Zurich);
            strasbourg.ByRoad.Add(Location.Geneva);
            strasbourg.ByTrain.Add(Location.Frankfurt);
            strasbourg.ByTrain.Add(Location.Zurich);
            tempMap[(int)Location.Strasbourg] = strasbourg;

            cologne.Name = "Cologne";
            cologne.Abbreviation = "COL";
            cologne.Type = LocationType.City;
            cologne.IsEastern = false;
            cologne.ByRoad.Add(Location.Brussels);
            cologne.ByRoad.Add(Location.Amsterdam);
            cologne.ByRoad.Add(Location.Hamburg);
            cologne.ByRoad.Add(Location.Leipzig);
            cologne.ByRoad.Add(Location.Frankfurt);
            cologne.ByRoad.Add(Location.Strasbourg);
            cologne.ByTrain.Add(Location.Brussels);
            cologne.ByTrain.Add(Location.Frankfurt);
            tempMap[(int)Location.Cologne] = cologne;

            hamburg.Name = "Hamburg";
            hamburg.Abbreviation = "HAM";
            hamburg.Type = LocationType.City;
            hamburg.IsEastern = false;
            hamburg.ByRoad.Add(Location.Cologne);
            hamburg.ByRoad.Add(Location.Berlin);
            hamburg.ByRoad.Add(Location.Leipzig);
            hamburg.ByTrain.Add(Location.Berlin);
            hamburg.BySea.Add(Location.NorthSea);
            tempMap[(int)Location.Hamburg] = hamburg;

            frankfurt.Name = "Frankfurt";
            frankfurt.Abbreviation = "FRA";
            frankfurt.Type = LocationType.Town;
            frankfurt.IsEastern = false;
            frankfurt.ByRoad.Add(Location.Strasbourg);
            frankfurt.ByRoad.Add(Location.Cologne);
            frankfurt.ByRoad.Add(Location.Leipzig);
            frankfurt.ByRoad.Add(Location.Nuremburg);
            frankfurt.ByTrain.Add(Location.Strasbourg);
            frankfurt.ByTrain.Add(Location.Cologne);
            frankfurt.ByTrain.Add(Location.Leipzig);
            tempMap[(int)Location.Frankfurt] = frankfurt;

            nuremburg.Name = "Nuremburg";
            nuremburg.Abbreviation = "NUR";
            nuremburg.Type = LocationType.Town;
            nuremburg.IsEastern = false;
            nuremburg.ByRoad.Add(Location.Strasbourg);
            nuremburg.ByRoad.Add(Location.Frankfurt);
            nuremburg.ByRoad.Add(Location.Leipzig);
            nuremburg.ByRoad.Add(Location.Prague);
            nuremburg.ByRoad.Add(Location.Munich);
            nuremburg.ByTrain.Add(Location.Leipzig);
            nuremburg.ByTrain.Add(Location.Munich);
            tempMap[(int)Location.Nuremburg] = nuremburg;

            leipzig.Name = "Leipzig";
            leipzig.Abbreviation = "LEI";
            leipzig.Type = LocationType.City;
            leipzig.IsEastern = false;
            leipzig.ByRoad.Add(Location.Cologne);
            leipzig.ByRoad.Add(Location.Hamburg);
            leipzig.ByRoad.Add(Location.Berlin);
            leipzig.ByRoad.Add(Location.Nuremburg);
            leipzig.ByRoad.Add(Location.Frankfurt);
            leipzig.ByTrain.Add(Location.Frankfurt);
            leipzig.ByTrain.Add(Location.Berlin);
            leipzig.ByTrain.Add(Location.Nuremburg);
            tempMap[(int)Location.Leipzig] = leipzig;

            berlin.Name = "Berlin";
            berlin.Abbreviation = "BER";
            berlin.Type = LocationType.City;
            berlin.IsEastern = false;
            berlin.ByRoad.Add(Location.Hamburg);
            berlin.ByRoad.Add(Location.Prague);
            berlin.ByRoad.Add(Location.Leipzig);
            berlin.ByTrain.Add(Location.Hamburg);
            berlin.ByTrain.Add(Location.Leipzig);
            berlin.ByTrain.Add(Location.Prague);
            tempMap[(int)Location.Berlin] = berlin;

            prague.Name = "Prague";
            prague.Abbreviation = "PRA";
            prague.Type = LocationType.City;
            prague.IsEastern = true;
            prague.ByRoad.Add(Location.Berlin);
            prague.ByRoad.Add(Location.Vienna);
            prague.ByRoad.Add(Location.Nuremburg);
            prague.ByTrain.Add(Location.Berlin);
            prague.ByTrain.Add(Location.Vienna);
            tempMap[(int)Location.Prague] = prague;

            castledracula.Name = "Castle Dracula";
            castledracula.Abbreviation = "CAS";
            castledracula.Type = LocationType.Castle;
            castledracula.IsEastern = true;
            castledracula.ByRoad.Add(Location.Klausenburg);
            castledracula.ByRoad.Add(Location.Galatz);
            tempMap[(int)Location.CastleDracula] = castledracula;

            santander.Name = "Santander";
            santander.Abbreviation = "SAN";
            santander.Type = LocationType.Town;
            santander.IsEastern = false;
            santander.ByRoad.Add(Location.Lisbon);
            santander.ByRoad.Add(Location.Madrid);
            santander.ByRoad.Add(Location.Saragossa);
            santander.ByTrain.Add(Location.Madrid);
            santander.BySea.Add(Location.BayOfBiscay);
            tempMap[(int)Location.Santander] = santander;

            saragossa.Name = "Saragossa";
            saragossa.Abbreviation = "SAG";
            saragossa.Type = LocationType.Town;
            saragossa.IsEastern = false;
            saragossa.ByRoad.Add(Location.Madrid);
            saragossa.ByRoad.Add(Location.Santander);
            saragossa.ByRoad.Add(Location.Bordeaux);
            saragossa.ByRoad.Add(Location.Toulouse);
            saragossa.ByRoad.Add(Location.Barcelona);
            saragossa.ByRoad.Add(Location.Alicante);
            saragossa.ByTrain.Add(Location.Madrid);
            saragossa.ByTrain.Add(Location.Bordeaux);
            saragossa.ByTrain.Add(Location.Barcelona);
            tempMap[(int)Location.Saragossa] = saragossa;

            bordeaux.Name = "Bordeaux";
            bordeaux.Abbreviation = "BOR";
            bordeaux.Type = LocationType.City;
            bordeaux.IsEastern = false;
            bordeaux.ByRoad.Add(Location.Saragossa);
            bordeaux.ByRoad.Add(Location.Nantes);
            bordeaux.ByRoad.Add(Location.ClermontFerrand);
            bordeaux.ByRoad.Add(Location.Toulouse);
            bordeaux.ByTrain.Add(Location.Paris);
            bordeaux.ByTrain.Add(Location.Saragossa);
            bordeaux.BySea.Add(Location.BayOfBiscay);
            tempMap[(int)Location.Bordeaux] = bordeaux;

            toulouse.Name = "Toulouse";
            toulouse.Abbreviation = "TOU";
            toulouse.Type = LocationType.Town;
            toulouse.IsEastern = false;
            toulouse.ByRoad.Add(Location.Saragossa);
            toulouse.ByRoad.Add(Location.Bordeaux);
            toulouse.ByRoad.Add(Location.ClermontFerrand);
            toulouse.ByRoad.Add(Location.Marseilles);
            toulouse.ByRoad.Add(Location.Barcelona);
            tempMap[(int)Location.Toulouse] = toulouse;

            barcelona.Name = "Barcelona";
            barcelona.Abbreviation = "BAC";
            barcelona.Type = LocationType.City;
            barcelona.IsEastern = false;
            barcelona.ByRoad.Add(Location.Saragossa);
            barcelona.ByRoad.Add(Location.Toulouse);
            barcelona.ByTrain.Add(Location.Saragossa);
            barcelona.ByTrain.Add(Location.Alicante);
            barcelona.BySea.Add(Location.MediterraneanSea);
            tempMap[(int)Location.Barcelona] = barcelona;

            clermontferrand.Name = "Clermont Ferrand";
            clermontferrand.Abbreviation = "CLE";
            clermontferrand.Type = LocationType.Town;
            clermontferrand.IsEastern = false;
            clermontferrand.ByRoad.Add(Location.Bordeaux);
            clermontferrand.ByRoad.Add(Location.Nantes);
            clermontferrand.ByRoad.Add(Location.Paris);
            clermontferrand.ByRoad.Add(Location.Geneva);
            clermontferrand.ByRoad.Add(Location.Marseilles);
            clermontferrand.ByRoad.Add(Location.Toulouse);
            tempMap[(int)Location.ClermontFerrand] = clermontferrand;

            marseilles.Name = "Marseilles";
            marseilles.Abbreviation = "MAR";
            marseilles.Type = LocationType.City;
            marseilles.IsEastern = false;
            marseilles.ByRoad.Add(Location.Toulouse);
            marseilles.ByRoad.Add(Location.ClermontFerrand);
            marseilles.ByRoad.Add(Location.Geneva);
            marseilles.ByRoad.Add(Location.Zurich);
            marseilles.ByRoad.Add(Location.Milan);
            marseilles.ByRoad.Add(Location.Genoa);
            marseilles.ByTrain.Add(Location.Paris);
            marseilles.BySea.Add(Location.MediterraneanSea);
            tempMap[(int)Location.Marseilles] = marseilles);

            geneva.Name = "Geneva";
            geneva.Abbreviation = "GEV";
            geneva.Type = LocationType.Town;
            geneva.IsEastern = false;
            geneva.ByRoad.Add(Location.Marseilles);
            geneva.ByRoad.Add(Location.ClermontFerrand);
            geneva.ByRoad.Add(Location.Paris);
            geneva.ByRoad.Add(Location.Strasbourg);
            geneva.ByRoad.Add(Location.Zurich);
            geneva.ByTrain.Add(Location.Milan);
            tempMap[(int)Location.Geneva] = geneva;

            genoa.Name = "Genoa";
            genoa.Abbreviation = "GEO";
            genoa.Type = LocationType.City;
            genoa.IsEastern = true;
            genoa.ByRoad.Add(Location.Marseilles);
            genoa.ByRoad.Add(Location.Milan);
            genoa.ByRoad.Add(Location.Venice);
            genoa.ByRoad.Add(Location.Florence);
            genoa.ByTrain.Add(Location.Milan);
            genoa.BySea.Add(Location.TyrrhenianSea);
            tempMap[(int)Location.Genoa = genoa;

            milan.Name = "Milan";
            milan.Abbreviation = "MIL";
            milan.Type = LocationType.City;
            milan.IsEastern = true;
            milan.ByRoad.Add(Location.Marseilles);
            milan.ByRoad.Add(Location.Zurich);
            milan.ByRoad.Add(Location.Munich);
            milan.ByRoad.Add(Location.Venice);
            milan.ByRoad.Add(Location.Genoa);
            milan.ByTrain.Add(Location.Geneva);
            milan.ByTrain.Add(Location.Zurich);
            milan.ByTrain.Add(Location.Florence);
            milan.ByTrain.Add(Location.Genoa);
            tempMap[(int)Location.Milan] = milan;

            zurich.Name = "Zurich";
            zurich.Abbreviation = "ZUR";
            zurich.Type = LocationType.Town;
            zurich.IsEastern = false;
            zurich.ByRoad.Add(Location.Marseilles);
            zurich.ByRoad.Add(Location.Geneva);
            zurich.ByRoad.Add(Location.Strasbourg);
            zurich.ByRoad.Add(Location.Munich);
            zurich.ByRoad.Add(Location.Milan);
            zurich.ByTrain.Add(Location.Strasbourg);
            zurich.ByTrain.Add(Location.Milan);
            tempMap[(int)Location.Zurich] = zurich;

            florence.Name = "Florence";
            florence.Abbreviation = "FLO";
            florence.Type = LocationType.Town;
            florence.IsEastern = true;
            florence.ByRoad.Add(Location.Genoa);
            florence.ByRoad.Add(Location.Venice);
            florence.ByRoad.Add(Location.Rome);
            florence.ByTrain.Add(Location.Milan);
            florence.ByTrain.Add(Location.Rome);
            tempMap[(int)Location.Florence] = florence;

            venice.Name = "Venice";
            venice.Abbreviation = "VEN";
            venice.Type = LocationType.Town;
            venice.IsEastern = true;
            venice.ByRoad.Add(Location.Florence);
            venice.ByRoad.Add(Location.Genoa);
            venice.ByRoad.Add(Location.Milan);
            venice.ByRoad.Add(Location.Munich);
            venice.ByTrain.Add(Location.Vienna);
            venice.BySea.Add(Location.AdriaticSea);
            tempMap[(int)Location.Venice] = venice;

            munich.Name = "Munich";
            munich.Abbreviation = "MUN";
            munich.Type = LocationType.City;
            munich.IsEastern = false;
            munich.ByRoad.Add(Location.Milan);
            munich.ByRoad.Add(Location.Zurich);
            munich.ByRoad.Add(Location.Strasbourg);
            munich.ByRoad.Add(Location.Nuremburg);
            munich.ByRoad.Add(Location.Vienna);
            munich.ByRoad.Add(Location.Zagreb);
            munich.ByRoad.Add(Location.Venice);
            munich.ByTrain.Add(Location.Nuremburg);
            tempMap[(int)Location.Munich] = munich;

            zagreb.Name = "Zagreb";
            zagreb.Abbreviation = "ZAG";
            zagreb.Type = LocationType.Town;
            zagreb.IsEastern = true;
            zagreb.ByRoad.Add(Location.Munich);
            zagreb.ByRoad.Add(Location.Vienna);
            zagreb.ByRoad.Add(Location.Budapest);
            zagreb.ByRoad.Add(Location.Szeged);
            zagreb.ByRoad.Add(Location.StJosephAndStMary);
            zagreb.ByRoad.Add(Location.Sarajevo);
            tempMap[(int)Location.Zagreb] = zagreb;

            vienna.Name = "Vienna";
            vienna.Abbreviation = "VIE";
            vienna.Type = LocationType.City;
            vienna.IsEastern = true;
            vienna.ByRoad.Add(Location.Munich);
            vienna.ByRoad.Add(Location.Prague);
            vienna.ByRoad.Add(Location.Budapest);
            vienna.ByRoad.Add(Location.Zagreb);
            vienna.ByTrain.Add(Location.Venice);
            vienna.ByTrain.Add(Location.Prague);
            vienna.ByTrain.Add(Location.Budapest);
            tempMap[(int)Location.Vienna] = vienna;

            stjosephandstmary.Name = "St. Joseph & St. Mary";
            stjosephandstmary.Abbreviation = "STJ";
            stjosephandstmary.Type = LocationType.Hospital;
            stjosephandstmary.IsEastern = true;
            stjosephandstmary.ByRoad.Add(Location.Zagreb);
            stjosephandstmary.ByRoad.Add(Location.Szeged);
            stjosephandstmary.ByRoad.Add(Location.Belgrade);
            stjosephandstmary.ByRoad.Add(Location.Sarajevo);
            tempMap[(int)Location.StJosephAndStMary] = stjosephandstmary;

            sarajevo.Name = "Sarajevo";
            sarajevo.Abbreviation = "SAJ";
            sarajevo.Type = LocationType.Town;
            sarajevo.IsEastern = true;
            sarajevo.ByRoad.Add(Location.Zagreb);
            sarajevo.ByRoad.Add(Location.StJosephAndStMary);
            sarajevo.ByRoad.Add(Location.Belgrade);
            sarajevo.ByRoad.Add(Location.Sofia);
            sarajevo.ByRoad.Add(Location.Valona);
            tempMap[(int)Location.Sarajevo] = sarajevo;

            szeged.Name = "Szeged";
            szeged.Abbreviation = "SZE";
            szeged.Type = LocationType.Town;
            szeged.IsEastern = true;
            szeged.ByRoad.Add(Location.Zagreb);
            szeged.ByRoad.Add(Location.Budapest);
            szeged.ByRoad.Add(Location.Klausenburg);
            szeged.ByRoad.Add(Location.Belgrade);
            szeged.ByRoad.Add(Location.StJosephAndStMary);
            szeged.ByTrain.Add(Location.Budapest);
            szeged.ByTrain.Add(Location.Bucharest);
            szeged.ByTrain.Add(Location.Belgrade);
            tempMap[(int)Location.Szeged] = szeged;

            budapest.Name = "Budapest";
            budapest.Abbreviation = "BUD";
            budapest.Type = LocationType.City;
            budapest.IsEastern = true;
            budapest.ByRoad.Add(Location.Vienna);
            budapest.ByRoad.Add(Location.Klausenburg);
            budapest.ByRoad.Add(Location.Szeged);
            budapest.ByRoad.Add(Location.Zagreb);
            budapest.ByTrain.Add(Location.Vienna);
            budapest.ByTrain.Add(Location.Szeged);
            tempMap[(int)Location.Budapest] = budapest;

            belgrade.Name = "Belgrade";
            belgrade.Abbreviation = "BEL";
            belgrade.Type = LocationType.Town;
            belgrade.IsEastern = true;
            belgrade.ByRoad.Add(Location.StJosephAndStMary);
            belgrade.ByRoad.Add(Location.Szeged);
            belgrade.ByRoad.Add(Location.Klausenburg);
            belgrade.ByRoad.Add(Location.Bucharest);
            belgrade.ByRoad.Add(Location.Sofia);
            belgrade.ByRoad.Add(Location.Sarajevo);
            belgrade.ByTrain.Add(Location.Szeged);
            belgrade.ByTrain.Add(Location.Sofia);
            tempMap[(int)Location.Belgrade] = belgrade;

            klausenburg.Name = "Klausenburg";
            klausenburg.Abbreviation = "KLA";
            klausenburg.Type = LocationType.Town;
            klausenburg.IsEastern = true;
            klausenburg.ByRoad.Add(Location.Budapest);
            klausenburg.ByRoad.Add(Location.CastleDracula);
            klausenburg.ByRoad.Add(Location.Galatz);
            klausenburg.ByRoad.Add(Location.Bucharest);
            klausenburg.ByRoad.Add(Location.Belgrade);
            klausenburg.ByRoad.Add(Location.Szeged);
            tempMap[(int)Location.Klausenburg] = klausenburg;

            sofia.Name = "Sofia";
            sofia.Abbreviation = "SOF";
            sofia.Type = LocationType.Town;
            sofia.IsEastern = true;
            sofia.ByRoad.Add(Location.Sarajevo);
            sofia.ByRoad.Add(Location.Belgrade);
            sofia.ByRoad.Add(Location.Bucharest);
            sofia.ByRoad.Add(Location.Varna);
            sofia.ByRoad.Add(Location.Salonica);
            sofia.ByRoad.Add(Location.Valona);
            sofia.ByTrain.Add(Location.Belgrade);
            sofia.ByTrain.Add(Location.Salonica);
            tempMap[(int)Location.Sofia] = sofia;

            bucharest.Name = "Bucharest";
            bucharest.Abbreviation = "BUC";
            bucharest.Type = LocationType.City;
            bucharest.IsEastern = true;
            bucharest.ByRoad.Add(Location.Belgrade);
            bucharest.ByRoad.Add(Location.Klausenburg);
            bucharest.ByRoad.Add(Location.Galatz);
            bucharest.ByRoad.Add(Location.Constanta);
            bucharest.ByRoad.Add(Location.Sofia);
            bucharest.ByTrain.Add(Location.Szeged);
            bucharest.ByTrain.Add(Location.Galatz);
            bucharest.ByTrain.Add(Location.Constanta);
            tempMap[(int)Location.Bucharest] = bucharest;

            galatz.Name = "Galatz";
            galatz.Abbreviation = "GAT";
            galatz.Type = LocationType.Town;
            galatz.IsEastern = true;
            galatz.ByRoad.Add(Location.Klausenburg);
            galatz.ByRoad.Add(Location.CastleDracula);
            galatz.ByRoad.Add(Location.Constanta);
            galatz.ByRoad.Add(Location.Bucharest);
            galatz.ByTrain.Add(Location.Bucharest);
            tempMap[(int)Location.Galatz] = galatz;

            varna.Name = "Varna";
            varna.Abbreviation = "VAR";
            varna.Type = LocationType.City;
            varna.IsEastern = true;
            varna.ByRoad.Add(Location.Sofia);
            varna.ByRoad.Add(Location.Constanta);
            varna.ByTrain.Add(Location.Sofia);
            varna.BySea.Add(Location.BlackSea);
            tempMap[(int)Location.Varna] = varna;

            constanta.Name = "Constanta";
            constanta.Abbreviation = "CON";
            constanta.Type = LocationType.City;
            constanta.IsEastern = true;
            constanta.ByRoad.Add(Location.Galatz);
            constanta.ByRoad.Add(Location.Varna);
            constanta.ByRoad.Add(Location.Bucharest);
            constanta.ByTrain.Add(Location.Bucharest);
            constanta.BySea.Add(Location.BlackSea);
            tempMap[(int)Location.Constanta] = constanta;

            lisbon.Name = "Lisbon";
            lisbon.Abbreviation = "LIS";
            lisbon.Type = LocationType.City;
            lisbon.IsEastern = false;
            lisbon.ByRoad.Add(Location.Santander);
            lisbon.ByRoad.Add(Location.Madrid);
            lisbon.ByRoad.Add(Location.Cadiz);
            lisbon.ByTrain.Add(Location.Madrid);
            lisbon.BySea.Add(Location.AtlanticOcean);
            tempMap[(int)Location.Lisbon] = lisbon;

            cadiz.Name = "Cadiz";
            cadiz.Abbreviation = "CAD";
            cadiz.Type = LocationType.City;
            cadiz.IsEastern = false;
            cadiz.ByRoad.Add(Location.Lisbon);
            cadiz.ByRoad.Add(Location.Madrid);
            cadiz.ByRoad.Add(Location.Granada);
            cadiz.BySea.Add(Location.AtlanticOcean);
            tempMap[(int)Location.Cadiz] = cadiz;

            madrid.Name = "Madrid";
            madrid.Abbreviation = "MAD";
            madrid.Type = LocationType.City;
            madrid.IsEastern = false;
            madrid.ByRoad.Add(Location.Lisbon);
            madrid.ByRoad.Add(Location.Santander);
            madrid.ByRoad.Add(Location.Saragossa);
            madrid.ByRoad.Add(Location.Alicante);
            madrid.ByRoad.Add(Location.Granada);
            madrid.ByRoad.Add(Location.Cadiz);
            madrid.ByTrain.Add(Location.Lisbon);
            madrid.ByTrain.Add(Location.Santander);
            madrid.ByTrain.Add(Location.Saragossa);
            madrid.ByTrain.Add(Location.Alicante);
            tempMap[(int)Location.Madrid] = madrid;

            granada.Name = "Granada";
            granada.Abbreviation = "GRA";
            granada.Type = LocationType.Town;
            granada.IsEastern = false;
            granada.ByRoad.Add(Location.Cadiz);
            granada.ByRoad.Add(Location.Madrid);
            granada.ByRoad.Add(Location.Alicante);
            tempMap[(int)Location.Granada] = granada;

            alicante.Name = "Alicante";
            alicante.Abbreviation = "ALI";
            alicante.Type = LocationType.Town;
            alicante.IsEastern = false;
            alicante.ByRoad.Add(Location.Granada);
            alicante.ByRoad.Add(Location.Madrid);
            alicante.ByRoad.Add(Location.Saragossa);
            alicante.ByTrain.Add(Location.Madrid);
            alicante.ByTrain.Add(Location.Barcelona);
            alicante.BySea.Add(Location.MediterraneanSea);
            tempMap[(int)Location.Alicante] = alicante;

            cagliari.Name = "Cagliari";
            cagliari.Abbreviation = "CAG";
            cagliari.Type = LocationType.Town;
            cagliari.IsEastern = true;
            cagliari.BySea.Add(Location.MediterraneanSea);
            cagliari.BySea.Add(Location.TyrrhenianSea);
            tempMap[(int)Location.Cagliari] = cagliari;

            rome.Name = "Rome";
            rome.Abbreviation = "ROM";
            rome.Type = LocationType.City;
            rome.IsEastern = true;
            rome.ByRoad.Add(Location.Florence);
            rome.ByRoad.Add(Location.Bari);
            rome.ByRoad.Add(Location.Naples);
            rome.ByTrain.Add(Location.Florence);
            rome.ByTrain.Add(Location.Naples);
            rome.BySea.Add(Location.TyrrhenianSea);
            tempMap[(int)Location.Rome] = rome;

            naples.Name = "Naples";
            naples.Abbreviation = "NAP";
            naples.Type = LocationType.City;
            naples.IsEastern = true;
            naples.ByRoad.Add(Location.Rome);
            naples.ByRoad.Add(Location.Bari);
            naples.ByTrain.Add(Location.Rome);
            naples.ByTrain.Add(Location.Bari);
            naples.BySea.Add(Location.TyrrhenianSea);
            tempMap[(int)Location.Naples] = naples;

            bari.Name = "Bari";
            bari.Abbreviation = "BAI";
            bari.Type = LocationType.Town;
            bari.IsEastern = true;
            bari.ByRoad.Add(Location.Naples);
            bari.ByRoad.Add(Location.Rome);
            bari.ByTrain.Add(Location.Naples);
            bari.BySea.Add(Location.AdriaticSea);
            tempMap[(int)Location.Bari] = bari;

            valona.Name = "Valona";
            valona.Abbreviation = "VAL";
            valona.Type = LocationType.Town;
            valona.IsEastern = true;
            valona.ByRoad.Add(Location.Sarajevo);
            valona.ByRoad.Add(Location.Sofia);
            valona.ByRoad.Add(Location.Salonica);
            valona.ByRoad.Add(Location.Athens);
            valona.BySea.Add(Location.IonianSea);
            tempMap[(int)Location.Valona] = valona;

            salonica.Name = "Salonica";
            salonica.Abbreviation = "SAL";
            salonica.Type = LocationType.Town;
            salonica.IsEastern = true;
            salonica.ByRoad.Add(Location.Valona);
            salonica.ByRoad.Add(Location.Sofia);
            salonica.ByTrain.Add(Location.Sofia);
            salonica.BySea.Add(Location.IonianSea);
            tempMap[(int)Location.Salonica] = salonica;

            athens.Name = "Athens";
            athens.Abbreviation = "ATH";
            athens.Type = LocationType.City;
            athens.IsEastern = true;
            athens.ByRoad.Add(Location.Valona);
            athens.BySea.Add(Location.IonianSea);
            tempMap[(int)Location.Athens] = athens;

            atlanticocean.Name = "Atlantic Ocean";
            atlanticocean.Abbreviation = "ATL";
            atlanticocean.Type = LocationType.Sea;
            atlanticocean.IsEastern = false;
            atlanticocean.BySea.Add(Location.NorthSea);
            atlanticocean.BySea.Add(Location.IrishSea);
            atlanticocean.BySea.Add(Location.EnglishChannel);
            atlanticocean.BySea.Add(Location.BayOfBiscay);
            atlanticocean.BySea.Add(Location.MediterraneanSea);
            atlanticocean.BySea.Add(Location.Galway);
            atlanticocean.BySea.Add(Location.Lisbon);
            atlanticocean.BySea.Add(Location.Cadiz);
            tempMap[(int)Location.AtlanticOcean] = atlanticocean;

            irishsea.Name = "Irish Sea";
            irishsea.Abbreviation = "IRI";
            irishsea.Type = LocationType.Sea;
            irishsea.IsEastern = false;
            irishsea.BySea.Add(Location.AtlanticOcean);
            irishsea.BySea.Add(Location.Dublin);
            irishsea.BySea.Add(Location.Liverpool);
            irishsea.BySea.Add(Location.Swansea);
            tempMap[(int)Location.IrishSea] = irishsea;

            englishchannel.Name = "English Channel";
            englishchannel.Abbreviation = "ENG";
            englishchannel.Type = LocationType.Sea;
            englishchannel.IsEastern = false;
            englishchannel.BySea.Add(Location.AtlanticOcean);
            englishchannel.BySea.Add(Location.NorthSea);
            englishchannel.BySea.Add(Location.Plymouth);
            englishchannel.BySea.Add(Location.London);
            englishchannel.BySea.Add(Location.LeHavre);
            tempMap[(int)Location.EnglishChannel] = englishchannel;

            northsea.Name = "North Sea";
            northsea.Abbreviation = "NOR";
            northsea.Type = LocationType.Sea;
            northsea.IsEastern = false;
            northsea.BySea.Add(Location.AtlanticOcean);
            northsea.BySea.Add(Location.EnglishChannel);
            northsea.BySea.Add(Location.Edinburgh);
            northsea.BySea.Add(Location.Amsterdam);
            northsea.BySea.Add(Location.Hamburg);
            tempMap[(int)Location.NorthSea] = northsea;

            bayofbiscay.Name = "Bay of Biscay";
            bayofbiscay.Abbreviation = "BAY";
            bayofbiscay.Type = LocationType.Sea;
            bayofbiscay.IsEastern = false;
            bayofbiscay.BySea.Add(Location.AtlanticOcean);
            bayofbiscay.BySea.Add(Location.Nantes);
            bayofbiscay.BySea.Add(Location.Bordeaux);
            bayofbiscay.BySea.Add(Location.Santander);
            tempMap[(int)Location.BayOfBiscay] = bayofbiscay;

            mediterraneansea.Name = "Mediterranean Sea";
            mediterraneansea.Abbreviation = "MED";
            mediterraneansea.Type = LocationType.Sea;
            mediterraneansea.IsEastern = true;
            mediterraneansea.BySea.Add(Location.AtlanticOcean);
            mediterraneansea.BySea.Add(Location.TyrrhenianSea);
            mediterraneansea.BySea.Add(Location.Alicante);
            mediterraneansea.BySea.Add(Location.Barcelona);
            mediterraneansea.BySea.Add(Location.Marseilles);
            mediterraneansea.BySea.Add(Location.Cagliari);
            tempMap[(int)Location.MediterraneanSea] = mediterraneansea;

            tyrrheniansea.Name = "Tyrrhenian Sea";
            tyrrheniansea.Abbreviation = "TYR";
            tyrrheniansea.Type = LocationType.Sea;
            tyrrheniansea.IsEastern = false;
            tyrrheniansea.BySea.Add(Location.MediterraneanSea);
            tyrrheniansea.BySea.Add(Location.IonianSea);
            tyrrheniansea.BySea.Add(Location.Cagliari);
            tyrrheniansea.BySea.Add(Location.Genoa);
            tyrrheniansea.BySea.Add(Location.Rome);
            tyrrheniansea.BySea.Add(Location.Naples);
            tempMap[(int)Location.TyrrhenianSea] = tyrrheniansea;

            adriaticsea.Name = "Adriatic Sea";
            adriaticsea.Abbreviation = "ADR";
            adriaticsea.Type = LocationType.Sea;
            adriaticsea.IsEastern = false;
            adriaticsea.BySea.Add(Location.IonianSea);
            adriaticsea.BySea.Add(Location.Bari);
            adriaticsea.BySea.Add(Location.Venice);
            tempMap[(int)Location.AdriaticSea] = adriaticsea;

            ioniansea.Name = "Ionian Sea";
            ioniansea.Abbreviation = "ION";
            ioniansea.Type = LocationType.Sea;
            ioniansea.IsEastern = false;
            ioniansea.BySea.Add(Location.MediterraneanSea);
            ioniansea.BySea.Add(Location.AdriaticSea);
            ioniansea.BySea.Add(Location.BlackSea);
            ioniansea.BySea.Add(Location.Valona);
            ioniansea.BySea.Add(Location.Athens);
            ioniansea.BySea.Add(Location.Salonica);
            tempMap[(int)Location.IonianSea] = ioniansea;

            blacksea.Name = "Black Sea";
            blacksea.Abbreviation = "BLA";
            blacksea.Type = LocationType.Sea;
            blacksea.IsEastern = false;
            blacksea.BySea.Add(Location.IonianSea);
            blacksea.BySea.Add(Location.Varna);
            blacksea.BySea.Add(Location.Constanta);
            tempMap[(int)Location.BlackSea] = blacksea;

            return tempMap;
        }

        public Location GetLocationFromString(string line)
        {
            Location tempLocation = Location.Nowhere;
            int countOfMatches = 0;
            foreach (LocationDetail loc in network)
            {
                if (loc.Name.StartsWith(line.ToLower()))
                {
                    countOfMatches++;
                    if (tempLocation == Location.Nowhere)
                    {
                        tempLocation = loc.Index;
                    }
                }
            }
            if (countOfMatches == 1)
            {
                return tempLocation;
            } else
            {
                return Location.Nowhere;
            }
        }

        public string LocationName(Location location)
        {
            return network[(int)location].Name;
        }

        public string LocationAbbreviation(Location location)
        {
            return network[(int)location].Abbreviation;
        }

        public LocationType TypeOfLocation(Location location)
        {
            return network[(int)location].Type;
        }

        public bool IsEastern(Location location)
        {
            return network[(int)location].IsEastern;
        }

        public List<Location> LocationsConnectedByRoadOrSeaTo(Location location)
        {
            List<Location> tempList = new List<Location>();
            tempList.AddRange(network[(int)location].ByRoad);
            tempList.AddRange(network[(int)location].BySea);
            return tempList;
        }
    }
}
