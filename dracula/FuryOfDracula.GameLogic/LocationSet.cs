using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    public class LocationSet
    {
        private LocationDetail[] map;

        public LocationSet()
        {
            map = CreateMap();
        }

        private LocationDetail[] CreateMap()
        {
            LocationDetail[] tempMap = new LocationDetail[72];
            LocationDetail nowhere = new LocationDetail(Location.Nowhere);
            LocationDetail galway = new LocationDetail(Location.Galway);
            LocationDetail dublin = new LocationDetail(Location.Dublin);
            LocationDetail liverpool = new LocationDetail(Location.Liverpool);
            LocationDetail edinburgh = new LocationDetail(Location.Edinburgh);
            LocationDetail manchester = new LocationDetail(Location.Manchester);
            LocationDetail swansea = new LocationDetail(Location.Swansea);
            LocationDetail plymouth = new LocationDetail(Location.Plymouth);
            LocationDetail nantes = new LocationDetail(Location.Nantes);
            LocationDetail lehavre = new LocationDetail(Location.LeHavre);
            LocationDetail london = new LocationDetail(Location.London);
            LocationDetail paris = new LocationDetail(Location.Paris);
            LocationDetail brussels = new LocationDetail(Location.Brussels);
            LocationDetail amsterdam = new LocationDetail(Location.Amsterdam);
            LocationDetail strasbourg = new LocationDetail(Location.Strasbourg);
            LocationDetail cologne = new LocationDetail(Location.Cologne);
            LocationDetail hamburg = new LocationDetail(Location.Hamburg);
            LocationDetail frankfurt = new LocationDetail(Location.Frankfurt);
            LocationDetail nuremburg = new LocationDetail(Location.Nuremburg);
            LocationDetail leipzig = new LocationDetail(Location.Leipzig);
            LocationDetail berlin = new LocationDetail(Location.Berlin);
            LocationDetail prague = new LocationDetail(Location.Prague);
            LocationDetail castledracula = new LocationDetail(Location.CastleDracula);
            LocationDetail santander = new LocationDetail(Location.Santander);
            LocationDetail saragossa = new LocationDetail(Location.Saragossa);
            LocationDetail bordeaux = new LocationDetail(Location.Bordeaux);
            LocationDetail toulouse = new LocationDetail(Location.Toulouse);
            LocationDetail barcelona = new LocationDetail(Location.Barcelona);
            LocationDetail clermontferrand = new LocationDetail(Location.ClermontFerrand);
            LocationDetail marseilles = new LocationDetail(Location.Marseilles);
            LocationDetail geneva = new LocationDetail(Location.Geneva);
            LocationDetail genoa = new LocationDetail(Location.Genoa);
            LocationDetail milan = new LocationDetail(Location.Milan);
            LocationDetail zurich = new LocationDetail(Location.Zurich);
            LocationDetail florence = new LocationDetail(Location.Florence);
            LocationDetail venice = new LocationDetail(Location.Venice);
            LocationDetail munich = new LocationDetail(Location.Munich);
            LocationDetail zagreb = new LocationDetail(Location.Zagreb);
            LocationDetail vienna = new LocationDetail(Location.Vienna);
            LocationDetail stjosephandstmary = new LocationDetail(Location.StJosephAndStMary);
            LocationDetail sarajevo = new LocationDetail(Location.Sarajevo);
            LocationDetail szeged = new LocationDetail(Location.Szeged);
            LocationDetail budapest = new LocationDetail(Location.Budapest);
            LocationDetail belgrade = new LocationDetail(Location.Belgrade);
            LocationDetail klausenburg = new LocationDetail(Location.Klausenburg);
            LocationDetail sofia = new LocationDetail(Location.Sofia);
            LocationDetail bucharest = new LocationDetail(Location.Bucharest);
            LocationDetail galatz = new LocationDetail(Location.Galatz);
            LocationDetail varna = new LocationDetail(Location.Varna);
            LocationDetail constanta = new LocationDetail(Location.Constanta);
            LocationDetail lisbon = new LocationDetail(Location.Lisbon);
            LocationDetail cadiz = new LocationDetail(Location.Cadiz);
            LocationDetail madrid = new LocationDetail(Location.Madrid);
            LocationDetail granada = new LocationDetail(Location.Granada);
            LocationDetail alicante = new LocationDetail(Location.Alicante);
            LocationDetail cagliari = new LocationDetail(Location.Cagliari);
            LocationDetail rome = new LocationDetail(Location.Rome);
            LocationDetail naples = new LocationDetail(Location.Naples);
            LocationDetail bari = new LocationDetail(Location.Bari);
            LocationDetail valona = new LocationDetail(Location.Valona);
            LocationDetail salonica = new LocationDetail(Location.Salonica);
            LocationDetail athens = new LocationDetail(Location.Athens);
            LocationDetail atlanticocean = new LocationDetail(Location.AtlanticOcean);
            LocationDetail irishsea = new LocationDetail(Location.IrishSea);
            LocationDetail englishchannel = new LocationDetail(Location.EnglishChannel);
            LocationDetail northsea = new LocationDetail(Location.NorthSea);
            LocationDetail bayofbiscay = new LocationDetail(Location.BayOfBiscay);
            LocationDetail mediterraneansea = new LocationDetail(Location.MediterraneanSea);
            LocationDetail tyrrheniansea = new LocationDetail(Location.TyrrhenianSea);
            LocationDetail adriaticsea = new LocationDetail(Location.AdriaticSea);
            LocationDetail ioniansea = new LocationDetail(Location.IonianSea);
            LocationDetail blacksea = new LocationDetail(Location.BlackSea);

            tempMap[(int)Location.Nowhere] = nowhere;

            galway.LocationType = LocationType.SmallCity;
            galway.IsEastern = false;
            galway.ByRoad.Add(Location.Dublin);
            galway.BySea.Add(Location.AtlanticOcean);
            tempMap[(int)Location.Galway] = galway;

            dublin.LocationType = LocationType.SmallCity;
            dublin.IsEastern = false;
            dublin.ByRoad.Add(Location.Galway);
            dublin.BySea.Add(Location.IrishSea);
            tempMap[(int)Location.Dublin] = dublin;

            liverpool.LocationType = LocationType.LargeCity;
            liverpool.IsEastern = false;
            liverpool.ByRoad.Add(Location.Manchester);
            liverpool.ByRoad.Add(Location.Swansea);
            liverpool.ByTrain.Add(Location.Manchester);
            liverpool.BySea.Add(Location.IrishSea);
            tempMap[(int)Location.Liverpool] = liverpool;

            edinburgh.LocationType = LocationType.LargeCity;
            edinburgh.IsEastern = false;
            edinburgh.ByRoad.Add(Location.Manchester);
            edinburgh.ByTrain.Add(Location.Manchester);
            edinburgh.BySea.Add(Location.NorthSea);
            tempMap[(int)Location.Edinburgh] = edinburgh;

            manchester.LocationType = LocationType.LargeCity;
            manchester.IsEastern = false;
            manchester.ByRoad.Add(Location.Edinburgh);
            manchester.ByRoad.Add(Location.Liverpool);
            manchester.ByRoad.Add(Location.London);
            manchester.ByTrain.Add(Location.Edinburgh);
            manchester.ByTrain.Add(Location.Liverpool);
            manchester.ByTrain.Add(Location.London);
            tempMap[(int)Location.Manchester] = manchester;

            swansea.LocationType = LocationType.SmallCity;
            swansea.IsEastern = false;
            swansea.ByRoad.Add(Location.Liverpool);
            swansea.ByRoad.Add(Location.London);
            swansea.ByTrain.Add(Location.London);
            swansea.BySea.Add(Location.IrishSea);
            tempMap[(int)Location.Swansea] = swansea;

            plymouth.LocationType = LocationType.SmallCity;
            plymouth.IsEastern = false;
            plymouth.ByRoad.Add(Location.London);
            plymouth.BySea.Add(Location.EnglishChannel);
            tempMap[(int)Location.Plymouth] = plymouth;

            nantes.LocationType = LocationType.LargeCity;
            nantes.IsEastern = false;
            nantes.ByRoad.Add(Location.LeHavre);
            nantes.ByRoad.Add(Location.Paris);
            nantes.ByRoad.Add(Location.ClermontFerrand);
            nantes.ByRoad.Add(Location.Bordeaux);
            nantes.BySea.Add(Location.BayOfBiscay);
            tempMap[(int)Location.Nantes] = nantes;

            lehavre.LocationType = LocationType.SmallCity;
            lehavre.IsEastern = false;
            lehavre.ByRoad.Add(Location.Nantes);
            lehavre.ByRoad.Add(Location.Paris);
            lehavre.ByRoad.Add(Location.Brussels);
            lehavre.ByTrain.Add(Location.Paris);
            lehavre.BySea.Add(Location.EnglishChannel);
            tempMap[(int)Location.LeHavre] = lehavre;

            london.LocationType = LocationType.LargeCity;
            london.IsEastern = false;
            london.ByRoad.Add(Location.Manchester);
            london.ByRoad.Add(Location.Swansea);
            london.ByRoad.Add(Location.Plymouth);
            london.ByTrain.Add(Location.Manchester);
            london.ByTrain.Add(Location.Swansea);
            london.BySea.Add(Location.EnglishChannel);
            tempMap[(int)Location.London] = london;

            paris.LocationType = LocationType.LargeCity;
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

            brussels.LocationType = LocationType.LargeCity;
            brussels.IsEastern = false;
            brussels.ByRoad.Add(Location.LeHavre);
            brussels.ByRoad.Add(Location.Amsterdam);
            brussels.ByRoad.Add(Location.Cologne);
            brussels.ByRoad.Add(Location.Strasbourg);
            brussels.ByRoad.Add(Location.Paris);
            brussels.ByTrain.Add(Location.Cologne);
            brussels.ByTrain.Add(Location.Paris);
            tempMap[(int)Location.Brussels] = brussels;

            amsterdam.LocationType = LocationType.LargeCity;
            amsterdam.IsEastern = false;
            amsterdam.ByRoad.Add(Location.Brussels);
            amsterdam.ByRoad.Add(Location.Cologne);
            amsterdam.BySea.Add(Location.NorthSea);
            tempMap[(int)Location.Amsterdam] = amsterdam;

            strasbourg.LocationType = LocationType.SmallCity;
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

            cologne.LocationType = LocationType.LargeCity;
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

            hamburg.LocationType = LocationType.LargeCity;
            hamburg.IsEastern = false;
            hamburg.ByRoad.Add(Location.Cologne);
            hamburg.ByRoad.Add(Location.Berlin);
            hamburg.ByRoad.Add(Location.Leipzig);
            hamburg.ByTrain.Add(Location.Berlin);
            hamburg.BySea.Add(Location.NorthSea);
            tempMap[(int)Location.Hamburg] = hamburg;

            frankfurt.LocationType = LocationType.SmallCity;
            frankfurt.IsEastern = false;
            frankfurt.ByRoad.Add(Location.Strasbourg);
            frankfurt.ByRoad.Add(Location.Cologne);
            frankfurt.ByRoad.Add(Location.Leipzig);
            frankfurt.ByRoad.Add(Location.Nuremburg);
            frankfurt.ByTrain.Add(Location.Strasbourg);
            frankfurt.ByTrain.Add(Location.Cologne);
            frankfurt.ByTrain.Add(Location.Leipzig);
            tempMap[(int)Location.Frankfurt] = frankfurt;

            nuremburg.LocationType = LocationType.SmallCity;
            nuremburg.IsEastern = false;
            nuremburg.ByRoad.Add(Location.Strasbourg);
            nuremburg.ByRoad.Add(Location.Frankfurt);
            nuremburg.ByRoad.Add(Location.Leipzig);
            nuremburg.ByRoad.Add(Location.Prague);
            nuremburg.ByRoad.Add(Location.Munich);
            nuremburg.ByTrain.Add(Location.Leipzig);
            nuremburg.ByTrain.Add(Location.Munich);
            tempMap[(int)Location.Nuremburg] = nuremburg;

            leipzig.LocationType = LocationType.LargeCity;
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

            berlin.LocationType = LocationType.LargeCity;
            berlin.IsEastern = false;
            berlin.ByRoad.Add(Location.Hamburg);
            berlin.ByRoad.Add(Location.Prague);
            berlin.ByRoad.Add(Location.Leipzig);
            berlin.ByTrain.Add(Location.Hamburg);
            berlin.ByTrain.Add(Location.Leipzig);
            berlin.ByTrain.Add(Location.Prague);
            tempMap[(int)Location.Berlin] = berlin;

            prague.LocationType = LocationType.LargeCity;
            prague.IsEastern = true;
            prague.ByRoad.Add(Location.Berlin);
            prague.ByRoad.Add(Location.Vienna);
            prague.ByRoad.Add(Location.Nuremburg);
            prague.ByTrain.Add(Location.Berlin);
            prague.ByTrain.Add(Location.Vienna);
            tempMap[(int)Location.Prague] = prague;

            castledracula.LocationType = LocationType.Castle;
            castledracula.IsEastern = true;
            castledracula.ByRoad.Add(Location.Klausenburg);
            castledracula.ByRoad.Add(Location.Galatz);
            tempMap[(int)Location.CastleDracula] = castledracula;

            santander.LocationType = LocationType.SmallCity;
            santander.IsEastern = false;
            santander.ByRoad.Add(Location.Lisbon);
            santander.ByRoad.Add(Location.Madrid);
            santander.ByRoad.Add(Location.Saragossa);
            santander.ByTrain.Add(Location.Madrid);
            santander.BySea.Add(Location.BayOfBiscay);
            tempMap[(int)Location.Santander] = santander;

            saragossa.LocationType = LocationType.SmallCity;
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

            bordeaux.LocationType = LocationType.LargeCity;
            bordeaux.IsEastern = false;
            bordeaux.ByRoad.Add(Location.Saragossa);
            bordeaux.ByRoad.Add(Location.Nantes);
            bordeaux.ByRoad.Add(Location.ClermontFerrand);
            bordeaux.ByRoad.Add(Location.Toulouse);
            bordeaux.ByTrain.Add(Location.Paris);
            bordeaux.ByTrain.Add(Location.Saragossa);
            bordeaux.BySea.Add(Location.BayOfBiscay);
            tempMap[(int)Location.Bordeaux] = bordeaux;

            toulouse.LocationType = LocationType.SmallCity;
            toulouse.IsEastern = false;
            toulouse.ByRoad.Add(Location.Saragossa);
            toulouse.ByRoad.Add(Location.Bordeaux);
            toulouse.ByRoad.Add(Location.ClermontFerrand);
            toulouse.ByRoad.Add(Location.Marseilles);
            toulouse.ByRoad.Add(Location.Barcelona);
            tempMap[(int)Location.Toulouse] = toulouse;

            barcelona.LocationType = LocationType.LargeCity;
            barcelona.IsEastern = false;
            barcelona.ByRoad.Add(Location.Saragossa);
            barcelona.ByRoad.Add(Location.Toulouse);
            barcelona.ByTrain.Add(Location.Saragossa);
            barcelona.ByTrain.Add(Location.Alicante);
            barcelona.BySea.Add(Location.MediterraneanSea);
            tempMap[(int)Location.Barcelona] = barcelona;

            clermontferrand.LocationType = LocationType.SmallCity;
            clermontferrand.IsEastern = false;
            clermontferrand.ByRoad.Add(Location.Bordeaux);
            clermontferrand.ByRoad.Add(Location.Nantes);
            clermontferrand.ByRoad.Add(Location.Paris);
            clermontferrand.ByRoad.Add(Location.Geneva);
            clermontferrand.ByRoad.Add(Location.Marseilles);
            clermontferrand.ByRoad.Add(Location.Toulouse);
            tempMap[(int)Location.ClermontFerrand] = clermontferrand;

            marseilles.LocationType = LocationType.LargeCity;
            marseilles.IsEastern = false;
            marseilles.ByRoad.Add(Location.Toulouse);
            marseilles.ByRoad.Add(Location.ClermontFerrand);
            marseilles.ByRoad.Add(Location.Geneva);
            marseilles.ByRoad.Add(Location.Zurich);
            marseilles.ByRoad.Add(Location.Milan);
            marseilles.ByRoad.Add(Location.Genoa);
            marseilles.ByTrain.Add(Location.Paris);
            marseilles.BySea.Add(Location.MediterraneanSea);
            tempMap[(int)Location.Marseilles] = marseilles;

            geneva.LocationType = LocationType.SmallCity;
            geneva.IsEastern = false;
            geneva.ByRoad.Add(Location.Marseilles);
            geneva.ByRoad.Add(Location.ClermontFerrand);
            geneva.ByRoad.Add(Location.Paris);
            geneva.ByRoad.Add(Location.Strasbourg);
            geneva.ByRoad.Add(Location.Zurich);
            geneva.ByTrain.Add(Location.Milan);
            tempMap[(int)Location.Geneva] = geneva;

            genoa.LocationType = LocationType.LargeCity;
            genoa.IsEastern = true;
            genoa.ByRoad.Add(Location.Marseilles);
            genoa.ByRoad.Add(Location.Milan);
            genoa.ByRoad.Add(Location.Venice);
            genoa.ByRoad.Add(Location.Florence);
            genoa.ByTrain.Add(Location.Milan);
            genoa.BySea.Add(Location.TyrrhenianSea);
            tempMap[(int)Location.Genoa] = genoa;

            milan.LocationType = LocationType.LargeCity;
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

            zurich.LocationType = LocationType.SmallCity;
            zurich.IsEastern = false;
            zurich.ByRoad.Add(Location.Marseilles);
            zurich.ByRoad.Add(Location.Geneva);
            zurich.ByRoad.Add(Location.Strasbourg);
            zurich.ByRoad.Add(Location.Munich);
            zurich.ByRoad.Add(Location.Milan);
            zurich.ByTrain.Add(Location.Strasbourg);
            zurich.ByTrain.Add(Location.Milan);
            tempMap[(int)Location.Zurich] = zurich;

            florence.LocationType = LocationType.SmallCity;
            florence.IsEastern = true;
            florence.ByRoad.Add(Location.Genoa);
            florence.ByRoad.Add(Location.Venice);
            florence.ByRoad.Add(Location.Rome);
            florence.ByTrain.Add(Location.Milan);
            florence.ByTrain.Add(Location.Rome);
            tempMap[(int)Location.Florence] = florence;

            venice.LocationType = LocationType.SmallCity;
            venice.IsEastern = true;
            venice.ByRoad.Add(Location.Florence);
            venice.ByRoad.Add(Location.Genoa);
            venice.ByRoad.Add(Location.Milan);
            venice.ByRoad.Add(Location.Munich);
            venice.ByTrain.Add(Location.Vienna);
            venice.BySea.Add(Location.AdriaticSea);
            tempMap[(int)Location.Venice] = venice;

            munich.LocationType = LocationType.LargeCity;
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

            zagreb.LocationType = LocationType.SmallCity;
            zagreb.IsEastern = true;
            zagreb.ByRoad.Add(Location.Munich);
            zagreb.ByRoad.Add(Location.Vienna);
            zagreb.ByRoad.Add(Location.Budapest);
            zagreb.ByRoad.Add(Location.Szeged);
            zagreb.ByRoad.Add(Location.StJosephAndStMary);
            zagreb.ByRoad.Add(Location.Sarajevo);
            tempMap[(int)Location.Zagreb] = zagreb;

            vienna.LocationType = LocationType.LargeCity;
            vienna.IsEastern = true;
            vienna.ByRoad.Add(Location.Munich);
            vienna.ByRoad.Add(Location.Prague);
            vienna.ByRoad.Add(Location.Budapest);
            vienna.ByRoad.Add(Location.Zagreb);
            vienna.ByTrain.Add(Location.Venice);
            vienna.ByTrain.Add(Location.Prague);
            vienna.ByTrain.Add(Location.Budapest);
            tempMap[(int)Location.Vienna] = vienna;

            stjosephandstmary.LocationType = LocationType.Hospital;
            stjosephandstmary.IsEastern = true;
            stjosephandstmary.ByRoad.Add(Location.Zagreb);
            stjosephandstmary.ByRoad.Add(Location.Szeged);
            stjosephandstmary.ByRoad.Add(Location.Belgrade);
            stjosephandstmary.ByRoad.Add(Location.Sarajevo);
            tempMap[(int)Location.StJosephAndStMary] = stjosephandstmary;

            sarajevo.LocationType = LocationType.SmallCity;
            sarajevo.IsEastern = true;
            sarajevo.ByRoad.Add(Location.Zagreb);
            sarajevo.ByRoad.Add(Location.StJosephAndStMary);
            sarajevo.ByRoad.Add(Location.Belgrade);
            sarajevo.ByRoad.Add(Location.Sofia);
            sarajevo.ByRoad.Add(Location.Valona);
            tempMap[(int)Location.Sarajevo] = sarajevo;

            szeged.LocationType = LocationType.SmallCity;
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

            budapest.LocationType = LocationType.LargeCity;
            budapest.IsEastern = true;
            budapest.ByRoad.Add(Location.Vienna);
            budapest.ByRoad.Add(Location.Klausenburg);
            budapest.ByRoad.Add(Location.Szeged);
            budapest.ByRoad.Add(Location.Zagreb);
            budapest.ByTrain.Add(Location.Vienna);
            budapest.ByTrain.Add(Location.Szeged);
            tempMap[(int)Location.Budapest] = budapest;

            belgrade.LocationType = LocationType.SmallCity;
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

            klausenburg.LocationType = LocationType.SmallCity;
            klausenburg.IsEastern = true;
            klausenburg.ByRoad.Add(Location.Budapest);
            klausenburg.ByRoad.Add(Location.CastleDracula);
            klausenburg.ByRoad.Add(Location.Galatz);
            klausenburg.ByRoad.Add(Location.Bucharest);
            klausenburg.ByRoad.Add(Location.Belgrade);
            klausenburg.ByRoad.Add(Location.Szeged);
            tempMap[(int)Location.Klausenburg] = klausenburg;

            sofia.LocationType = LocationType.SmallCity;
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

            bucharest.LocationType = LocationType.LargeCity;
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

            galatz.LocationType = LocationType.SmallCity;
            galatz.IsEastern = true;
            galatz.ByRoad.Add(Location.Klausenburg);
            galatz.ByRoad.Add(Location.CastleDracula);
            galatz.ByRoad.Add(Location.Constanta);
            galatz.ByRoad.Add(Location.Bucharest);
            galatz.ByTrain.Add(Location.Bucharest);
            tempMap[(int)Location.Galatz] = galatz;

            varna.LocationType = LocationType.LargeCity;
            varna.IsEastern = true;
            varna.ByRoad.Add(Location.Sofia);
            varna.ByRoad.Add(Location.Constanta);
            varna.ByTrain.Add(Location.Sofia);
            varna.BySea.Add(Location.BlackSea);
            tempMap[(int)Location.Varna] = varna;

            constanta.LocationType = LocationType.LargeCity;
            constanta.IsEastern = true;
            constanta.ByRoad.Add(Location.Galatz);
            constanta.ByRoad.Add(Location.Varna);
            constanta.ByRoad.Add(Location.Bucharest);
            constanta.ByTrain.Add(Location.Bucharest);
            constanta.BySea.Add(Location.BlackSea);
            tempMap[(int)Location.Constanta] = constanta;

            lisbon.LocationType = LocationType.LargeCity;
            lisbon.IsEastern = false;
            lisbon.ByRoad.Add(Location.Santander);
            lisbon.ByRoad.Add(Location.Madrid);
            lisbon.ByRoad.Add(Location.Cadiz);
            lisbon.ByTrain.Add(Location.Madrid);
            lisbon.BySea.Add(Location.AtlanticOcean);
            tempMap[(int)Location.Lisbon] = lisbon;

            cadiz.LocationType = LocationType.LargeCity;
            cadiz.IsEastern = false;
            cadiz.ByRoad.Add(Location.Lisbon);
            cadiz.ByRoad.Add(Location.Madrid);
            cadiz.ByRoad.Add(Location.Granada);
            cadiz.BySea.Add(Location.AtlanticOcean);
            tempMap[(int)Location.Cadiz] = cadiz;

            madrid.LocationType = LocationType.LargeCity;
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

            granada.LocationType = LocationType.SmallCity;
            granada.IsEastern = false;
            granada.ByRoad.Add(Location.Cadiz);
            granada.ByRoad.Add(Location.Madrid);
            granada.ByRoad.Add(Location.Alicante);
            tempMap[(int)Location.Granada] = granada;

            alicante.LocationType = LocationType.SmallCity;
            alicante.IsEastern = false;
            alicante.ByRoad.Add(Location.Granada);
            alicante.ByRoad.Add(Location.Madrid);
            alicante.ByRoad.Add(Location.Saragossa);
            alicante.ByTrain.Add(Location.Madrid);
            alicante.ByTrain.Add(Location.Barcelona);
            alicante.BySea.Add(Location.MediterraneanSea);
            tempMap[(int)Location.Alicante] = alicante;

            cagliari.LocationType = LocationType.SmallCity;
            cagliari.IsEastern = true;
            cagliari.BySea.Add(Location.MediterraneanSea);
            cagliari.BySea.Add(Location.TyrrhenianSea);
            tempMap[(int)Location.Cagliari] = cagliari;

            rome.LocationType = LocationType.LargeCity;
            rome.IsEastern = true;
            rome.ByRoad.Add(Location.Florence);
            rome.ByRoad.Add(Location.Bari);
            rome.ByRoad.Add(Location.Naples);
            rome.ByTrain.Add(Location.Florence);
            rome.ByTrain.Add(Location.Naples);
            rome.BySea.Add(Location.TyrrhenianSea);
            tempMap[(int)Location.Rome] = rome;

            naples.LocationType = LocationType.LargeCity;
            naples.IsEastern = true;
            naples.ByRoad.Add(Location.Rome);
            naples.ByRoad.Add(Location.Bari);
            naples.ByTrain.Add(Location.Rome);
            naples.ByTrain.Add(Location.Bari);
            naples.BySea.Add(Location.TyrrhenianSea);
            tempMap[(int)Location.Naples] = naples;

            bari.LocationType = LocationType.SmallCity;
            bari.IsEastern = true;
            bari.ByRoad.Add(Location.Naples);
            bari.ByRoad.Add(Location.Rome);
            bari.ByTrain.Add(Location.Naples);
            bari.BySea.Add(Location.AdriaticSea);
            tempMap[(int)Location.Bari] = bari;

            valona.LocationType = LocationType.SmallCity;
            valona.IsEastern = true;
            valona.ByRoad.Add(Location.Sarajevo);
            valona.ByRoad.Add(Location.Sofia);
            valona.ByRoad.Add(Location.Salonica);
            valona.ByRoad.Add(Location.Athens);
            valona.BySea.Add(Location.IonianSea);
            tempMap[(int)Location.Valona] = valona;

            salonica.LocationType = LocationType.SmallCity;
            salonica.IsEastern = true;
            salonica.ByRoad.Add(Location.Valona);
            salonica.ByRoad.Add(Location.Sofia);
            salonica.ByTrain.Add(Location.Sofia);
            salonica.BySea.Add(Location.IonianSea);
            tempMap[(int)Location.Salonica] = salonica;

            athens.LocationType = LocationType.LargeCity;
            athens.IsEastern = true;
            athens.ByRoad.Add(Location.Valona);
            athens.BySea.Add(Location.IonianSea);
            tempMap[(int)Location.Athens] = athens;

            atlanticocean.LocationType = LocationType.Sea;
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

            irishsea.LocationType = LocationType.Sea;
            irishsea.IsEastern = false;
            irishsea.BySea.Add(Location.AtlanticOcean);
            irishsea.BySea.Add(Location.Dublin);
            irishsea.BySea.Add(Location.Liverpool);
            irishsea.BySea.Add(Location.Swansea);
            tempMap[(int)Location.IrishSea] = irishsea;

            englishchannel.LocationType = LocationType.Sea;
            englishchannel.IsEastern = false;
            englishchannel.BySea.Add(Location.AtlanticOcean);
            englishchannel.BySea.Add(Location.NorthSea);
            englishchannel.BySea.Add(Location.Plymouth);
            englishchannel.BySea.Add(Location.London);
            englishchannel.BySea.Add(Location.LeHavre);
            tempMap[(int)Location.EnglishChannel] = englishchannel;

            northsea.LocationType = LocationType.Sea;
            northsea.IsEastern = false;
            northsea.BySea.Add(Location.AtlanticOcean);
            northsea.BySea.Add(Location.EnglishChannel);
            northsea.BySea.Add(Location.Edinburgh);
            northsea.BySea.Add(Location.Amsterdam);
            northsea.BySea.Add(Location.Hamburg);
            tempMap[(int)Location.NorthSea] = northsea;

            bayofbiscay.LocationType = LocationType.Sea;
            bayofbiscay.IsEastern = false;
            bayofbiscay.BySea.Add(Location.AtlanticOcean);
            bayofbiscay.BySea.Add(Location.Nantes);
            bayofbiscay.BySea.Add(Location.Bordeaux);
            bayofbiscay.BySea.Add(Location.Santander);
            tempMap[(int)Location.BayOfBiscay] = bayofbiscay;

            mediterraneansea.LocationType = LocationType.Sea;
            mediterraneansea.IsEastern = true;
            mediterraneansea.BySea.Add(Location.AtlanticOcean);
            mediterraneansea.BySea.Add(Location.TyrrhenianSea);
            mediterraneansea.BySea.Add(Location.Alicante);
            mediterraneansea.BySea.Add(Location.Barcelona);
            mediterraneansea.BySea.Add(Location.Marseilles);
            mediterraneansea.BySea.Add(Location.Cagliari);
            tempMap[(int)Location.MediterraneanSea] = mediterraneansea;

            tyrrheniansea.LocationType = LocationType.Sea;
            tyrrheniansea.IsEastern = false;
            tyrrheniansea.BySea.Add(Location.MediterraneanSea);
            tyrrheniansea.BySea.Add(Location.IonianSea);
            tyrrheniansea.BySea.Add(Location.Cagliari);
            tyrrheniansea.BySea.Add(Location.Genoa);
            tyrrheniansea.BySea.Add(Location.Rome);
            tyrrheniansea.BySea.Add(Location.Naples);
            tempMap[(int)Location.TyrrhenianSea] = tyrrheniansea;

            adriaticsea.LocationType = LocationType.Sea;
            adriaticsea.IsEastern = false;
            adriaticsea.BySea.Add(Location.IonianSea);
            adriaticsea.BySea.Add(Location.Bari);
            adriaticsea.BySea.Add(Location.Venice);
            tempMap[(int)Location.AdriaticSea] = adriaticsea;

            ioniansea.LocationType = LocationType.Sea;
            ioniansea.IsEastern = false;
            ioniansea.BySea.Add(Location.MediterraneanSea);
            ioniansea.BySea.Add(Location.AdriaticSea);
            ioniansea.BySea.Add(Location.BlackSea);
            ioniansea.BySea.Add(Location.Valona);
            ioniansea.BySea.Add(Location.Athens);
            ioniansea.BySea.Add(Location.Salonica);
            tempMap[(int)Location.IonianSea] = ioniansea;

            blacksea.LocationType = LocationType.Sea;
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
            foreach (LocationDetail loc in map)
            {
                if (loc.Location.Name().ToLower().StartsWith(line.ToLower()))
                {
                    countOfMatches++;
                    if (tempLocation == Location.Nowhere)
                    {
                        tempLocation = loc.Location;
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

        public LocationType TypeOfLocation(Location location)
        {
            return map[(int)location].LocationType;
        }

        public bool IsEastern(Location location)
        {
            return map[(int)location].IsEastern;
        }

        public List<Location> LocationsConnectedByRoadOrSeaTo(Location location)
        {
            List<Location> tempList = new List<Location>();
            tempList.AddRange(map[(int)location].ByRoad);
            tempList.AddRange(map[(int)location].BySea);
            return tempList;
        }
    }
}
