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
            LocationDetail[] tempMap = new LocationDetail[71];
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

            galway.Name = "Galway";
            galway.Abbreviation = "GAW";
            galway.Type = LocationType.Town;
            galway.IsEastern = false;
            galway.ByRoad.Add(Location.dublin);
            galway.BySea.Add(Location.atlanticocean);
            tempMap[(int)Location.galway] = galway;

            dublin.Name = "Dublin";
            dublin.Abbreviation = "DUB";
            dublin.Type = LocationType.Town;
            dublin.IsEastern = false;
            dublin.ByRoad.Add(Location.galway);
            dublin.BySea.Add(Location.irishsea);
            tempMap[(int)Location.dublin] = dublin;

            liverpool.Name = "Liverpool";
            liverpool.Abbreviation = "LIV";
            liverpool.Type = LocationType.City;
            liverpool.IsEastern = false;
            liverpool.ByRoad.Add(Location.manchester);
            liverpool.ByRoad.Add(Location.swansea);
            liverpool.ByTrain.Add(Location.manchester);
            liverpool.BySea.Add(Location.irishsea);
            tempMap[(int)Location.liverpool] = liverpool;

            edinburgh.Name = "Edinburgh";
            edinburgh.Abbreviation = "EDI";
            edinburgh.Type = LocationType.City;
            edinburgh.IsEastern = false;
            edinburgh.ByRoad.Add(Location.manchester);
            edinburgh.ByTrain.Add(Location.manchester);
            edinburgh.BySea.Add(Location.northsea);
            tempMap[(int)Location.edinburgh] = edinburgh;

            manchester.Name = "Manchester";
            manchester.Abbreviation = "MAN";
            manchester.Type = LocationType.City;
            manchester.IsEastern = false;
            manchester.ByRoad.Add(Location.edinburgh);
            manchester.ByRoad.Add(Location.liverpool);
            manchester.ByRoad.Add(Location.london);
            manchester.ByTrain.Add(Location.edinburgh);
            manchester.ByTrain.Add(Location.liverpool);
            manchester.ByTrain.Add(Location.london);
            tempMap[(int)Location.manchester] = manchester;

            swansea.Name = "Swansea";
            swansea.Abbreviation = "SWA";
            swansea.Type = LocationType.Town;
            swansea.IsEastern = false;
            swansea.ByRoad.Add(Location.liverpool);
            swansea.ByRoad.Add(Location.london);
            swansea.ByTrain.Add(Location.london);
            swansea.BySea.Add(Location.irishsea);
            tempMap[(int)Location.swansea] = swansea;

            plymouth.Name = "Plymouth";
            plymouth.Abbreviation = "PLY";
            plymouth.Type = LocationType.Town;
            plymouth.IsEastern = false;
            plymouth.ByRoad.Add(Location.london);
            plymouth.BySea.Add(Location.englishchannel);
            tempMap[(int)Location.plymouth] = plymouth;

            nantes.Name = "Nantes";
            nantes.Abbreviation = "NAN";
            nantes.Type = LocationType.City;
            nantes.IsEastern = false;
            nantes.ByRoad.Add(Location.lehavre);
            nantes.ByRoad.Add(Location.paris);
            nantes.ByRoad.Add(Location.clermontferrand);
            nantes.ByRoad.Add(Location.bordeaux);
            nantes.BySea.Add(Location.bayofbiscay);
            tempMap[(int)Location.nantes] = nantes;

            lehavre.Name = "Le Havre";
            lehavre.Abbreviation = "LEH";
            lehavre.Type = LocationType.Town;
            lehavre.IsEastern = false;
            lehavre.ByRoad.Add(Location.nantes);
            lehavre.ByRoad.Add(Location.paris);
            lehavre.ByRoad.Add(Location.brussels);
            lehavre.ByTrain.Add(Location.paris);
            lehavre.BySea.Add(Location.englishchannel);
            tempMap[(int)Location.lehavre] = lehavre;

            london.Name = "London";
            london.Abbreviation = "LON";
            london.Type = LocationType.City;
            london.IsEastern = false;
            london.ByRoad.Add(Location.manchester);
            london.ByRoad.Add(Location.swansea);
            london.ByRoad.Add(Location.plymouth);
            london.ByTrain.Add(Location.manchester);
            london.ByTrain.Add(Location.swansea);
            london.BySea.Add(Location.englishchannel);
            tempMap[(int)Location.london] = london;

            paris.Name = "Paris";
            paris.Abbreviation = "PAR";
            paris.Type = LocationType.City;
            paris.IsEastern = false;
            paris.ByRoad.Add(Location.nantes);
            paris.ByRoad.Add(Location.lehavre);
            paris.ByRoad.Add(Location.brussels);
            paris.ByRoad.Add(Location.strasbourg);
            paris.ByRoad.Add(Location.geneva);
            paris.ByRoad.Add(Location.clermontferrand);
            paris.ByTrain.Add(Location.lehavre);
            paris.ByTrain.Add(Location.brussels);
            paris.ByTrain.Add(Location.marseilles);
            paris.ByTrain.Add(Location.bordeaux);
            tempMap[(int)Location.paris] = paris;

            brussels.Name = "Brussels";
            brussels.Abbreviation = "BRU";
            brussels.Type = LocationType.City;
            brussels.IsEastern = false;
            brussels.ByRoad.Add(Location.lehavre);
            brussels.ByRoad.Add(Location.amsterdam);
            brussels.ByRoad.Add(Location.cologne);
            brussels.ByRoad.Add(Location.strasbourg);
            brussels.ByRoad.Add(Location.paris);
            brussels.ByTrain.Add(Location.cologne);
            brussels.ByTrain.Add(Location.paris);
            tempMap[(int)Location.brussels] = brussels;

            amsterdam.Name = "Amsterdam";
            amsterdam.Abbreviation = "AMS";
            amsterdam.Type = LocationType.City;
            amsterdam.IsEastern = false;
            amsterdam.ByRoad.Add(Location.brussels);
            amsterdam.ByRoad.Add(Location.cologne);
            amsterdam.BySea.Add(Location.northsea);
            tempMap[(int)Location.amsterdam] = amsterdam;

            strasbourg.Name = "Strasbourg";
            strasbourg.Abbreviation = "STR";
            strasbourg.Type = LocationType.Town;
            strasbourg.IsEastern = false;
            strasbourg.ByRoad.Add(Location.paris);
            strasbourg.ByRoad.Add(Location.brussels);
            strasbourg.ByRoad.Add(Location.cologne);
            strasbourg.ByRoad.Add(Location.frankfurt);
            strasbourg.ByRoad.Add(Location.nuremburg);
            strasbourg.ByRoad.Add(Location.munich);
            strasbourg.ByRoad.Add(Location.zurich);
            strasbourg.ByRoad.Add(Location.geneva);
            strasbourg.ByTrain.Add(Location.frankfurt);
            strasbourg.ByTrain.Add(Location.zurich);
            tempMap[(int)Location.strasbourg] = strasbourg;

            cologne.Name = "Cologne";
            cologne.Abbreviation = "COL";
            cologne.Type = LocationType.City;
            cologne.IsEastern = false;
            cologne.ByRoad.Add(Location.brussels);
            cologne.ByRoad.Add(Location.amsterdam);
            cologne.ByRoad.Add(Location.hamburg);
            cologne.ByRoad.Add(Location.leipzig);
            cologne.ByRoad.Add(Location.frankfurt);
            cologne.ByRoad.Add(Location.strasbourg);
            cologne.ByTrain.Add(Location.brussels);
            cologne.ByTrain.Add(Location.frankfurt);
            tempMap[(int)Location.cologne] = cologne;

            hamburg.Name = "Hamburg";
            hamburg.Abbreviation = "HAM";
            hamburg.Type = LocationType.City;
            hamburg.IsEastern = false;
            hamburg.ByRoad.Add(Location.cologne);
            hamburg.ByRoad.Add(Location.berlin);
            hamburg.ByRoad.Add(Location.leipzig);
            hamburg.ByTrain.Add(Location.berlin);
            hamburg.BySea.Add(Location.northsea);
            tempMap[(int)Location.hamburg] = hamburg;

            frankfurt.Name = "Frankfurt";
            frankfurt.Abbreviation = "FRA";
            frankfurt.Type = LocationType.Town;
            frankfurt.IsEastern = false;
            frankfurt.ByRoad.Add(Location.strasbourg);
            frankfurt.ByRoad.Add(Location.cologne);
            frankfurt.ByRoad.Add(Location.leipzig);
            frankfurt.ByRoad.Add(Location.nuremburg);
            frankfurt.ByTrain.Add(Location.strasbourg);
            frankfurt.ByTrain.Add(Location.cologne);
            frankfurt.ByTrain.Add(Location.leipzig);
            tempMap[(int)Location.frankfurt] = frankfurt;

            nuremburg.Name = "Nuremburg";
            nuremburg.Abbreviation = "NUR";
            nuremburg.Type = LocationType.Town;
            nuremburg.IsEastern = false;
            nuremburg.ByRoad.Add(Location.strasbourg);
            nuremburg.ByRoad.Add(Location.frankfurt);
            nuremburg.ByRoad.Add(Location.leipzig);
            nuremburg.ByRoad.Add(Location.prague);
            nuremburg.ByRoad.Add(Location.munich);
            nuremburg.ByTrain.Add(Location.leipzig);
            nuremburg.ByTrain.Add(Location.munich);
            tempMap[(int)Location.nuremburg] = nuremburg;

            leipzig.Name = "Leipzig";
            leipzig.Abbreviation = "LEI";
            leipzig.Type = LocationType.City;
            leipzig.IsEastern = false;
            leipzig.ByRoad.Add(Location.cologne);
            leipzig.ByRoad.Add(Location.hamburg);
            leipzig.ByRoad.Add(Location.berlin);
            leipzig.ByRoad.Add(Location.nuremburg);
            leipzig.ByRoad.Add(Location.frankfurt);
            leipzig.ByTrain.Add(Location.frankfurt);
            leipzig.ByTrain.Add(Location.berlin);
            leipzig.ByTrain.Add(Location.nuremburg);
            tempMap[(int)Location.leipzig] = leipzig;

            berlin.Name = "Berlin";
            berlin.Abbreviation = "BER";
            berlin.Type = LocationType.City;
            berlin.IsEastern = false;
            berlin.ByRoad.Add(Location.hamburg);
            berlin.ByRoad.Add(Location.prague);
            berlin.ByRoad.Add(Location.leipzig);
            berlin.ByTrain.Add(Location.hamburg);
            berlin.ByTrain.Add(Location.leipzig);
            berlin.ByTrain.Add(Location.prague);
            tempMap[(int)Location.berlin] = berlin;

            prague.Name = "Prague";
            prague.Abbreviation = "PRA";
            prague.Type = LocationType.City;
            prague.IsEastern = true;
            prague.ByRoad.Add(Location.berlin);
            prague.ByRoad.Add(Location.vienna);
            prague.ByRoad.Add(Location.nuremburg);
            prague.ByTrain.Add(Location.berlin);
            prague.ByTrain.Add(Location.vienna);
            tempMap[(int)Location.prague] = prague;

            castledracula.Name = "Castle Dracula";
            castledracula.Abbreviation = "CAS";
            castledracula.Type = LocationType.Castle;
            castledracula.IsEastern = true;
            castledracula.ByRoad.Add(Location.klausenburg);
            castledracula.ByRoad.Add(Location.galatz);
            tempMap[(int)Location.castledracula] = castledracula;

            santander.Name = "Santander";
            santander.Abbreviation = "SAN";
            santander.Type = LocationType.Town;
            santander.IsEastern = false;
            santander.ByRoad.Add(Location.lisbon);
            santander.ByRoad.Add(Location.madrid);
            santander.ByRoad.Add(Location.saragossa);
            santander.ByTrain.Add(Location.madrid);
            santander.BySea.Add(Location.bayofbiscay);
            tempMap[(int)Location.santander] = santander;

            saragossa.Name = "Saragossa";
            saragossa.Abbreviation = "SAG";
            saragossa.Type = LocationType.Town;
            saragossa.IsEastern = false;
            saragossa.ByRoad.Add(Location.madrid);
            saragossa.ByRoad.Add(Location.santander);
            saragossa.ByRoad.Add(Location.bordeaux);
            saragossa.ByRoad.Add(Location.toulouse);
            saragossa.ByRoad.Add(Location.barcelona);
            saragossa.ByRoad.Add(Location.alicante);
            saragossa.ByTrain.Add(Location.madrid);
            saragossa.ByTrain.Add(Location.bordeaux);
            saragossa.ByTrain.Add(Location.barcelona);
            tempMap[(int)Location.saragossa] = saragossa;

            bordeaux.Name = "Bordeaux";
            bordeaux.Abbreviation = "BOR";
            bordeaux.Type = LocationType.City;
            bordeaux.IsEastern = false;
            bordeaux.ByRoad.Add(Location.saragossa);
            bordeaux.ByRoad.Add(Location.nantes);
            bordeaux.ByRoad.Add(Location.clermontferrand);
            bordeaux.ByRoad.Add(Location.toulouse);
            bordeaux.ByTrain.Add(Location.paris);
            bordeaux.ByTrain.Add(Location.saragossa);
            bordeaux.BySea.Add(Location.bayofbiscay);
            tempMap[(int)Location.bordeaux] = bordeaux;

            toulouse.Name = "Toulouse";
            toulouse.Abbreviation = "TOU";
            toulouse.Type = LocationType.Town;
            toulouse.IsEastern = false;
            toulouse.ByRoad.Add(Location.saragossa);
            toulouse.ByRoad.Add(Location.bordeaux);
            toulouse.ByRoad.Add(Location.clermontferrand);
            toulouse.ByRoad.Add(Location.marseilles);
            toulouse.ByRoad.Add(Location.barcelona);
            tempMap[(int)Location.toulouse] = toulouse;

            barcelona.Name = "Barcelona";
            barcelona.Abbreviation = "BAC";
            barcelona.Type = LocationType.City;
            barcelona.IsEastern = false;
            barcelona.ByRoad.Add(Location.saragossa);
            barcelona.ByRoad.Add(Location.toulouse);
            barcelona.ByTrain.Add(Location.saragossa);
            barcelona.ByTrain.Add(Location.alicante);
            barcelona.BySea.Add(Location.mediterraneansea);
            tempMap[(int)Location.barcelona] = barcelona;

            clermontferrand.Name = "Clermont Ferrand";
            clermontferrand.Abbreviation = "CLE";
            clermontferrand.Type = LocationType.Town;
            clermontferrand.IsEastern = false;
            clermontferrand.ByRoad.Add(Location.bordeaux);
            clermontferrand.ByRoad.Add(Location.nantes);
            clermontferrand.ByRoad.Add(Location.paris);
            clermontferrand.ByRoad.Add(Location.geneva);
            clermontferrand.ByRoad.Add(Location.marseilles);
            clermontferrand.ByRoad.Add(Location.toulouse);
            tempMap[(int)Location.clermontferrand] = clermontferrand;

            marseilles.Name = "Marseilles";
            marseilles.Abbreviation = "MAR";
            marseilles.Type = LocationType.City;
            marseilles.IsEastern = false;
            marseilles.ByRoad.Add(Location.toulouse);
            marseilles.ByRoad.Add(Location.clermontferrand);
            marseilles.ByRoad.Add(Location.geneva);
            marseilles.ByRoad.Add(Location.zurich);
            marseilles.ByRoad.Add(Location.milan);
            marseilles.ByRoad.Add(Location.genoa);
            marseilles.ByTrain.Add(Location.paris);
            marseilles.BySea.Add(Location.mediterraneansea);
            tempMap[(int)Location.marseilles] = marseilles);

            geneva.Name = "Geneva";
            geneva.Abbreviation = "GEV";
            geneva.Type = LocationType.Town;
            geneva.IsEastern = false;
            geneva.ByRoad.Add(Location.marseilles);
            geneva.ByRoad.Add(Location.clermontferrand);
            geneva.ByRoad.Add(Location.paris);
            geneva.ByRoad.Add(Location.strasbourg);
            geneva.ByRoad.Add(Location.zurich);
            geneva.ByTrain.Add(Location.milan);
            tempMap[(int)Location.geneva] = geneva;

            genoa.Name = "Genoa";
            genoa.Abbreviation = "GEO";
            genoa.Type = LocationType.City;
            genoa.IsEastern = true;
            genoa.ByRoad.Add(Location.marseilles);
            genoa.ByRoad.Add(Location.milan);
            genoa.ByRoad.Add(Location.venice);
            genoa.ByRoad.Add(Location.florence);
            genoa.ByTrain.Add(Location.milan);
            genoa.BySea.Add(Location.tyrrheniansea);
            tempMap[(int)Location.genoa = genoa;

            milan.Name = "Milan";
            milan.Abbreviation = "MIL";
            milan.Type = LocationType.City;
            milan.IsEastern = true;
            milan.ByRoad.Add(Location.marseilles);
            milan.ByRoad.Add(Location.zurich);
            milan.ByRoad.Add(Location.munich);
            milan.ByRoad.Add(Location.venice);
            milan.ByRoad.Add(Location.genoa);
            milan.ByTrain.Add(Location.geneva);
            milan.ByTrain.Add(Location.zurich);
            milan.ByTrain.Add(Location.florence);
            milan.ByTrain.Add(Location.genoa);
            tempMap[(int)Location.milan] = milan;

            zurich.Name = "Zurich";
            zurich.Abbreviation = "ZUR";
            zurich.Type = LocationType.Town;
            zurich.IsEastern = false;
            zurich.ByRoad.Add(Location.marseilles);
            zurich.ByRoad.Add(Location.geneva);
            zurich.ByRoad.Add(Location.strasbourg);
            zurich.ByRoad.Add(Location.munich);
            zurich.ByRoad.Add(Location.milan);
            zurich.ByTrain.Add(Location.strasbourg);
            zurich.ByTrain.Add(Location.milan);
            tempMap[(int)Location.zurich] = zurich;

            florence.Name = "Florence";
            florence.Abbreviation = "FLO";
            florence.Type = LocationType.Town;
            florence.IsEastern = true;
            florence.ByRoad.Add(Location.genoa);
            florence.ByRoad.Add(Location.venice);
            florence.ByRoad.Add(Location.rome);
            florence.ByTrain.Add(Location.milan);
            florence.ByTrain.Add(Location.rome);
            tempMap[(int)Location.florence] = florence;

            venice.Name = "Venice";
            venice.Abbreviation = "VEN";
            venice.Type = LocationType.Town;
            venice.IsEastern = true;
            venice.ByRoad.Add(Location.florence);
            venice.ByRoad.Add(Location.genoa);
            venice.ByRoad.Add(Location.milan);
            venice.ByRoad.Add(Location.munich);
            venice.ByTrain.Add(Location.vienna);
            venice.BySea.Add(Location.adriaticsea);
            tempMap[(int)Location.venice] = venice;

            munich.Name = "Munich";
            munich.Abbreviation = "MUN";
            munich.Type = LocationType.City;
            munich.IsEastern = false;
            munich.ByRoad.Add(Location.milan);
            munich.ByRoad.Add(Location.zurich);
            munich.ByRoad.Add(Location.strasbourg);
            munich.ByRoad.Add(Location.nuremburg);
            munich.ByRoad.Add(Location.vienna);
            munich.ByRoad.Add(Location.zagreb);
            munich.ByRoad.Add(Location.venice);
            munich.ByTrain.Add(Location.nuremburg);
            tempMap[(int)Location.munich] = munich;

            zagreb.Name = "Zagreb";
            zagreb.Abbreviation = "ZAG";
            zagreb.Type = LocationType.Town;
            zagreb.IsEastern = true;
            zagreb.ByRoad.Add(Location.munich);
            zagreb.ByRoad.Add(Location.vienna);
            zagreb.ByRoad.Add(Location.budapest);
            zagreb.ByRoad.Add(Location.szeged);
            zagreb.ByRoad.Add(Location.stjosephandstmary);
            zagreb.ByRoad.Add(Location.sarajevo);
            tempMap[(int)Location.zagreb] = zagreb;

            vienna.Name = "Vienna";
            vienna.Abbreviation = "VIE";
            vienna.Type = LocationType.City;
            vienna.IsEastern = true;
            vienna.ByRoad.Add(Location.munich);
            vienna.ByRoad.Add(Location.prague);
            vienna.ByRoad.Add(Location.budapest);
            vienna.ByRoad.Add(Location.zagreb);
            vienna.ByTrain.Add(Location.venice);
            vienna.ByTrain.Add(Location.prague);
            vienna.ByTrain.Add(Location.budapest);
            tempMap[(int)Location.vienna] = vienna;

            stjosephandstmary.Name = "St. Joseph & St. Mary";
            stjosephandstmary.Abbreviation = "STJ";
            stjosephandstmary.Type = LocationType.Hospital;
            stjosephandstmary.IsEastern = true;
            stjosephandstmary.ByRoad.Add(Location.zagreb);
            stjosephandstmary.ByRoad.Add(Location.szeged);
            stjosephandstmary.ByRoad.Add(Location.belgrade);
            stjosephandstmary.ByRoad.Add(Location.sarajevo);
            tempMap[(int)Location.stjosephandstmary] = stjosephandstmary;

            sarajevo.Name = "Sarajevo";
            sarajevo.Abbreviation = "SAJ";
            sarajevo.Type = LocationType.Town;
            sarajevo.IsEastern = true;
            sarajevo.ByRoad.Add(Location.zagreb);
            sarajevo.ByRoad.Add(Location.stjosephandstmary);
            sarajevo.ByRoad.Add(Location.belgrade);
            sarajevo.ByRoad.Add(Location.sofia);
            sarajevo.ByRoad.Add(Location.valona);
            tempMap[(int)Location.sarajevo] = sarajevo;

            szeged.Name = "Szeged";
            szeged.Abbreviation = "SZE";
            szeged.Type = LocationType.Town;
            szeged.IsEastern = true;
            szeged.ByRoad.Add(Location.zagreb);
            szeged.ByRoad.Add(Location.budapest);
            szeged.ByRoad.Add(Location.klausenburg);
            szeged.ByRoad.Add(Location.belgrade);
            szeged.ByRoad.Add(Location.stjosephandstmary);
            szeged.ByTrain.Add(Location.budapest);
            szeged.ByTrain.Add(Location.bucharest);
            szeged.ByTrain.Add(Location.belgrade);
            tempMap[(int)Location.szeged] = szeged;

            budapest.Name = "Budapest";
            budapest.Abbreviation = "BUD";
            budapest.Type = LocationType.City;
            budapest.IsEastern = true;
            budapest.ByRoad.Add(Location.vienna);
            budapest.ByRoad.Add(Location.klausenburg);
            budapest.ByRoad.Add(Location.szeged);
            budapest.ByRoad.Add(Location.zagreb);
            budapest.ByTrain.Add(Location.vienna);
            budapest.ByTrain.Add(Location.szeged);
            tempMap[(int)Location.budapest] = budapest;

            belgrade.Name = "Belgrade";
            belgrade.Abbreviation = "BEL";
            belgrade.Type = LocationType.Town;
            belgrade.IsEastern = true;
            belgrade.ByRoad.Add(Location.stjosephandstmary);
            belgrade.ByRoad.Add(Location.szeged);
            belgrade.ByRoad.Add(Location.klausenburg);
            belgrade.ByRoad.Add(Location.bucharest);
            belgrade.ByRoad.Add(Location.sofia);
            belgrade.ByRoad.Add(Location.sarajevo);
            belgrade.ByTrain.Add(Location.szeged);
            belgrade.ByTrain.Add(Location.sofia);
            tempMap[(int)Location.belgrade] = belgrade;

            klausenburg.Name = "Klausenburg";
            klausenburg.Abbreviation = "KLA";
            klausenburg.Type = LocationType.Town;
            klausenburg.IsEastern = true;
            klausenburg.ByRoad.Add(Location.budapest);
            klausenburg.ByRoad.Add(Location.castledracula);
            klausenburg.ByRoad.Add(Location.galatz);
            klausenburg.ByRoad.Add(Location.bucharest);
            klausenburg.ByRoad.Add(Location.belgrade);
            klausenburg.ByRoad.Add(Location.szeged);
            tempMap[(int)Location.klausenburg] = klausenburg;

            sofia.Name = "Sofia";
            sofia.Abbreviation = "SOF";
            sofia.Type = LocationType.Town;
            sofia.IsEastern = true;
            sofia.ByRoad.Add(Location.sarajevo);
            sofia.ByRoad.Add(Location.belgrade);
            sofia.ByRoad.Add(Location.bucharest);
            sofia.ByRoad.Add(Location.varna);
            sofia.ByRoad.Add(Location.salonica);
            sofia.ByRoad.Add(Location.valona);
            sofia.ByTrain.Add(Location.belgrade);
            sofia.ByTrain.Add(Location.salonica);
            tempMap[(int)Location.sofia] = sofia;

            bucharest.Name = "Bucharest";
            bucharest.Abbreviation = "BUC";
            bucharest.Type = LocationType.City;
            bucharest.IsEastern = true;
            bucharest.ByRoad.Add(Location.belgrade);
            bucharest.ByRoad.Add(Location.klausenburg);
            bucharest.ByRoad.Add(Location.galatz);
            bucharest.ByRoad.Add(Location.constanta);
            bucharest.ByRoad.Add(Location.sofia);
            bucharest.ByTrain.Add(Location.szeged);
            bucharest.ByTrain.Add(Location.galatz);
            bucharest.ByTrain.Add(Location.constanta);
            tempMap[(int)Location.bucharest] = bucharest;

            galatz.Name = "Galatz";
            galatz.Abbreviation = "GAT";
            galatz.Type = LocationType.Town;
            galatz.IsEastern = true;
            galatz.ByRoad.Add(Location.klausenburg);
            galatz.ByRoad.Add(Location.castledracula);
            galatz.ByRoad.Add(Location.constanta);
            galatz.ByRoad.Add(Location.bucharest);
            galatz.ByTrain.Add(Location.bucharest);
            tempMap[(int)Location.galatz] = galatz;

            varna.Name = "Varna";
            varna.Abbreviation = "VAR";
            varna.Type = LocationType.City;
            varna.IsEastern = true;
            varna.ByRoad.Add(Location.sofia);
            varna.ByRoad.Add(Location.constanta);
            varna.ByTrain.Add(Location.sofia);
            varna.BySea.Add(Location.blacksea);
            tempMap[(int)Location.varna] = varna;

            constanta.Name = "Constanta";
            constanta.Abbreviation = "CON";
            constanta.Type = LocationType.City;
            constanta.IsEastern = true;
            constanta.ByRoad.Add(Location.galatz);
            constanta.ByRoad.Add(Location.varna);
            constanta.ByRoad.Add(Location.bucharest);
            constanta.ByTrain.Add(Location.bucharest);
            constanta.BySea.Add(Location.blacksea);
            tempMap[(int)Location.constanta] = constanta;

            lisbon.Name = "Lisbon";
            lisbon.Abbreviation = "LIS";
            lisbon.Type = LocationType.City;
            lisbon.IsEastern = false;
            lisbon.ByRoad.Add(Location.santander);
            lisbon.ByRoad.Add(Location.madrid);
            lisbon.ByRoad.Add(Location.cadiz);
            lisbon.ByTrain.Add(Location.madrid);
            lisbon.BySea.Add(Location.atlanticocean);
            tempMap[(int)Location.lisbon] = lisbon;

            cadiz.Name = "Cadiz";
            cadiz.Abbreviation = "CAD";
            cadiz.Type = LocationType.City;
            cadiz.IsEastern = false;
            cadiz.ByRoad.Add(Location.lisbon);
            cadiz.ByRoad.Add(Location.madrid);
            cadiz.ByRoad.Add(Location.granada);
            cadiz.BySea.Add(Location.atlanticocean);
            tempMap[(int)Location.cadiz] = cadiz;

            madrid.Name = "Madrid";
            madrid.Abbreviation = "MAD";
            madrid.Type = LocationType.City;
            madrid.IsEastern = false;
            madrid.ByRoad.Add(Location.lisbon);
            madrid.ByRoad.Add(Location.santander);
            madrid.ByRoad.Add(Location.saragossa);
            madrid.ByRoad.Add(Location.alicante);
            madrid.ByRoad.Add(Location.granada);
            madrid.ByRoad.Add(Location.cadiz);
            madrid.ByTrain.Add(Location.lisbon);
            madrid.ByTrain.Add(Location.santander);
            madrid.ByTrain.Add(Location.saragossa);
            madrid.ByTrain.Add(Location.alicante);
            tempMap[(int)Location.madrid] = madrid;

            granada.Name = "Granada";
            granada.Abbreviation = "GRA";
            granada.Type = LocationType.Town;
            granada.IsEastern = false;
            granada.ByRoad.Add(Location.cadiz);
            granada.ByRoad.Add(Location.madrid);
            granada.ByRoad.Add(Location.alicante);
            tempMap[(int)Location.granada] = granada;

            alicante.Name = "Alicante";
            alicante.Abbreviation = "ALI";
            alicante.Type = LocationType.Town;
            alicante.IsEastern = false;
            alicante.ByRoad.Add(Location.granada);
            alicante.ByRoad.Add(Location.madrid);
            alicante.ByRoad.Add(Location.saragossa);
            alicante.ByTrain.Add(Location.madrid);
            alicante.ByTrain.Add(Location.barcelona);
            alicante.BySea.Add(Location.mediterraneansea);
            tempMap[(int)Location.alicante] = alicante;

            cagliari.Name = "Cagliari";
            cagliari.Abbreviation = "CAG";
            cagliari.Type = LocationType.Town;
            cagliari.IsEastern = true;
            cagliari.BySea.Add(Location.mediterraneansea);
            cagliari.BySea.Add(Location.tyrrheniansea);
            tempMap[(int)Location.cagliari] = cagliari;

            rome.Name = "Rome";
            rome.Abbreviation = "ROM";
            rome.Type = LocationType.City;
            rome.IsEastern = true;
            rome.ByRoad.Add(Location.florence);
            rome.ByRoad.Add(Location.bari);
            rome.ByRoad.Add(Location.naples);
            rome.ByTrain.Add(Location.florence);
            rome.ByTrain.Add(Location.naples);
            rome.BySea.Add(Location.tyrrheniansea);
            tempMap[(int)Location.rome] = rome;

            naples.Name = "Naples";
            naples.Abbreviation = "NAP";
            naples.Type = LocationType.City;
            naples.IsEastern = true;
            naples.ByRoad.Add(Location.rome);
            naples.ByRoad.Add(Location.bari);
            naples.ByTrain.Add(Location.rome);
            naples.ByTrain.Add(Location.bari);
            naples.BySea.Add(Location.tyrrheniansea);
            tempMap[(int)Location.naples] = naples;

            bari.Name = "Bari";
            bari.Abbreviation = "BAI";
            bari.Type = LocationType.Town;
            bari.IsEastern = true;
            bari.ByRoad.Add(Location.naples);
            bari.ByRoad.Add(Location.rome);
            bari.ByTrain.Add(Location.naples);
            bari.BySea.Add(Location.adriaticsea);
            tempMap[(int)Location.bari] = bari;

            valona.Name = "Valona";
            valona.Abbreviation = "VAL";
            valona.Type = LocationType.Town;
            valona.IsEastern = true;
            valona.ByRoad.Add(Location.sarajevo);
            valona.ByRoad.Add(Location.sofia);
            valona.ByRoad.Add(Location.salonica);
            valona.ByRoad.Add(Location.athens);
            valona.BySea.Add(Location.ioniansea);
            tempMap[(int)Location.valona] = valona;

            salonica.Name = "Salonica";
            salonica.Abbreviation = "SAL";
            salonica.Type = LocationType.Town;
            salonica.IsEastern = true;
            salonica.ByRoad.Add(Location.valona);
            salonica.ByRoad.Add(Location.sofia);
            salonica.ByTrain.Add(Location.sofia);
            salonica.BySea.Add(Location.ioniansea);
            tempMap[(int)Location.salonica] = salonica;

            athens.Name = "Athens";
            athens.Abbreviation = "ATH";
            athens.Type = LocationType.City;
            athens.IsEastern = true;
            athens.ByRoad.Add(Location.valona);
            athens.BySea.Add(Location.ioniansea);
            tempMap[(int)Location.athens] = athens;

            atlanticocean.Name = "Atlantic Ocean";
            atlanticocean.Abbreviation = "ATL";
            atlanticocean.Type = LocationType.Sea;
            atlanticocean.IsEastern = false;
            atlanticocean.BySea.Add(Location.northsea);
            atlanticocean.BySea.Add(Location.irishsea);
            atlanticocean.BySea.Add(Location.englishchannel);
            atlanticocean.BySea.Add(Location.bayofbiscay);
            atlanticocean.BySea.Add(Location.mediterraneansea);
            atlanticocean.BySea.Add(Location.galway);
            atlanticocean.BySea.Add(Location.lisbon);
            atlanticocean.BySea.Add(Location.cadiz);
            tempMap[(int)Location.atlanticocean] = atlanticocean;

            irishsea.Name = "Irish Sea";
            irishsea.Abbreviation = "IRI";
            irishsea.Type = LocationType.Sea;
            irishsea.IsEastern = false;
            irishsea.BySea.Add(Location.atlanticocean);
            irishsea.BySea.Add(Location.dublin);
            irishsea.BySea.Add(Location.liverpool);
            irishsea.BySea.Add(Location.swansea);
            tempMap[(int)Location.irishsea] = irishsea;

            englishchannel.Name = "English Channel";
            englishchannel.Abbreviation = "ENG";
            englishchannel.Type = LocationType.Sea;
            englishchannel.IsEastern = false;
            englishchannel.BySea.Add(Location.atlanticocean);
            englishchannel.BySea.Add(Location.northsea);
            englishchannel.BySea.Add(Location.plymouth);
            englishchannel.BySea.Add(Location.london);
            englishchannel.BySea.Add(Location.lehavre);
            tempMap[(int)Location.englishchannel] = englishchannel;

            northsea.Name = "North Sea";
            northsea.Abbreviation = "NOR";
            northsea.Type = LocationType.Sea;
            northsea.IsEastern = false;
            northsea.BySea.Add(Location.atlanticocean);
            northsea.BySea.Add(Location.englishchannel);
            northsea.BySea.Add(Location.edinburgh);
            northsea.BySea.Add(Location.amsterdam);
            northsea.BySea.Add(Location.hamburg);
            tempMap[(int)Location.northsea] = northsea;

            bayofbiscay.Name = "Bay of Biscay";
            bayofbiscay.Abbreviation = "BAY";
            bayofbiscay.Type = LocationType.Sea;
            bayofbiscay.IsEastern = false;
            bayofbiscay.BySea.Add(Location.atlanticocean);
            bayofbiscay.BySea.Add(Location.nantes);
            bayofbiscay.BySea.Add(Location.bordeaux);
            bayofbiscay.BySea.Add(Location.santander);
            tempMap[(int)Location.bayofbiscay] = bayofbiscay;

            mediterraneansea.Name = "Mediterranean Sea";
            mediterraneansea.Abbreviation = "MED";
            mediterraneansea.Type = LocationType.Sea;
            mediterraneansea.IsEastern = true;
            mediterraneansea.BySea.Add(Location.atlanticocean);
            mediterraneansea.BySea.Add(Location.tyrrheniansea);
            mediterraneansea.BySea.Add(Location.alicante);
            mediterraneansea.BySea.Add(Location.barcelona);
            mediterraneansea.BySea.Add(Location.marseilles);
            mediterraneansea.BySea.Add(Location.cagliari);
            tempMap[(int)Location.mediterraneansea] = mediterraneansea;

            tyrrheniansea.Name = "Tyrrhenian Sea";
            tyrrheniansea.Abbreviation = "TYR";
            tyrrheniansea.Type = LocationType.Sea;
            tyrrheniansea.IsEastern = false;
            tyrrheniansea.BySea.Add(Location.mediterraneansea);
            tyrrheniansea.BySea.Add(Location.ioniansea);
            tyrrheniansea.BySea.Add(Location.cagliari);
            tyrrheniansea.BySea.Add(Location.genoa);
            tyrrheniansea.BySea.Add(Location.rome);
            tyrrheniansea.BySea.Add(Location.naples);
            tempMap[(int)Location.tyrrheniansea] = tyrrheniansea;

            adriaticsea.Name = "Adriatic Sea";
            adriaticsea.Abbreviation = "ADR";
            adriaticsea.Type = LocationType.Sea;
            adriaticsea.IsEastern = false;
            adriaticsea.BySea.Add(Location.ioniansea);
            adriaticsea.BySea.Add(Location.bari);
            adriaticsea.BySea.Add(Location.venice);
            tempMap[(int)Location.adriaticsea] = adriaticsea;

            ioniansea.Name = "Ionian Sea";
            ioniansea.Abbreviation = "ION";
            ioniansea.Type = LocationType.Sea;
            ioniansea.IsEastern = false;
            ioniansea.BySea.Add(Location.mediterraneansea);
            ioniansea.BySea.Add(Location.adriaticsea);
            ioniansea.BySea.Add(Location.blacksea);
            ioniansea.BySea.Add(Location.valona);
            ioniansea.BySea.Add(Location.athens);
            ioniansea.BySea.Add(Location.salonica);
            tempMap[(int)Location.ioniansea] = ioniansea;

            blacksea.Name = "Black Sea";
            blacksea.Abbreviation = "BLA";
            blacksea.Type = LocationType.Sea;
            blacksea.IsEastern = false;
            blacksea.BySea.Add(Location.ioniansea);
            blacksea.BySea.Add(Location.varna);
            blacksea.BySea.Add(Location.constanta);
            tempMap[(int)Location.blacksea] = blacksea;

            return tempMap;
        }

        public LocationDetail LocationDetails(Location location)
        {
            return network[(int)location];
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
