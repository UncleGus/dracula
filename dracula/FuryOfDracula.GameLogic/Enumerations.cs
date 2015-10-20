using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    public static class Extensions
    {
        public static string Name(this Event e)
        {
            return DescriptionAttr(e);
        }

        public static string Name(this EventType v)
        {
            return DescriptionAttr(v);
        }

        public static string Name(this Item i)
        {
            return DescriptionAttr(i);
        }

        public static string Name(this Location l)
        {
            return DescriptionAttr(l);
        }

        public static string Name(this LocationType t)
        {
            return DescriptionAttr(t);
        }

        public static string DescriptionAttr<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }
    }
    
    public enum EventType
    {
        [Description("None")]
        None,
        [Description("Ally")]
        Ally,
        [Description("Keep")]
        Keep,
        [Description("Play Immediately")]
        PlayImmediately
    }

    public enum Event
    {
        [Description("None")]
        None,
        [Description("Advance Planning")]
        AdvancePlanning1,
        [Description("Advance Planning")]
        AdvancePlanning2,
        [Description("Advance Planning")]
        AdvancePlanning3,
        [Description("Blood Transfusion")]
        BloodTransfusion,
        [Description("Chartered Carriage")]
        CharteredCarriage1,
        [Description("Chartered Carriage")]
        CharteredCarriage2,
        [Description("Chartered Carriage")]
        CharteredCarriage3,
        [Description("Consecrated Ground")]
        ConsecratedGround,
        [Description("Control Storms")]
        ControlStorms,
        [Description("Customs Search")]
        CustomsSearch,
        [Description("Devilish Power")]
        DevilishPower1,
        [Description("Devilish Power")]
        DevilishPower2,
        [Description("Dracula's Brides")]
        DraculasBrides,
        [Description("Escape Route")]
        EscapeRoute1,
        [Description("Escape Route")]
        EscapeRoute2,
        [Description("Evasion")]
        Evasion,
        [Description("Excellent Weather")]
        ExcellentWeather1,
        [Description("Excellent Weather")]
        ExcellentWeather2,
        [Description("False Tip-off")]
        FalseTipoff1,
        [Description("False Tip-off")]
        FalseTipoff2,
        [Description("Forewarned")]
        Forewarned1,
        [Description("Forewarned")]
        Forewarned2,
        [Description("Forewarned")]
        Forewarned3,
        [Description("Good Luck")]
        GoodLuck1,
        [Description("Good Luck")]
        GoodLuck2,
        [Description("Great Strength")]
        GreatStrength,
        [Description("Heroic Leap")]
        HeroicLeap,
        [Description("Hired Scouts")]
        HiredScouts1,
        [Description("Hired Scouts")]
        HiredScouts2,
        [Description("Hired Scouts")]
        HiredScouts3,
        [Description("Hypnosis")]
        Hypnosis1,
        [Description("Hypnosis")]
        Hypnosis2,
        [Description("Immanuel Hildesheim")]
        ImmanuelHildesheim,
        [Description("Jonathan Harker")]
        JonathanHarker,
        [Description("Long Day")]
        LongDay1,
        [Description("Long Day")]
        LongDay2,
        [Description("Money Trail")]
        MoneyTrail,
        [Description("Mystic Research")]
        MysticResearch1,
        [Description("Mystic Research")]
        MysticResearch2,
        [Description("Newspaper Reports")]
        NewspaperReports1,
        [Description("Newspaper Reports")]
        NewspaperReports2,
        [Description("Newspaper Reports")]
        NewspaperReports3,
        [Description("Newspaper Reports")]
        NewspaperReports4,
        [Description("Newspaper Reports")]
        NewspaperReports5,
        [Description("Night Visit")]
        NightVisit,
        [Description("Quincey P. Morris")]
        QuinceyPMorris,
        [Description("Rage")]
        Rage,
        [Description("Re-Equip")]
        ReEquip1,
        [Description("Re-Equip")]
        ReEquip2,
        [Description("Re-Equip")]
        ReEquip3,
        [Description("Relentless Minion")]
        RelentlessMinion1,
        [Description("Relentless Minion")]
        RelentlessMinion2,
        [Description("Roadblock")]
        Roadblock,
        [Description("Rufus Smith")]
        RufusSmith,
        [Description("Secret Weapon")]
        SecretWeapon1,
        [Description("Secret Weapon")]
        SecretWeapon2,
        [Description("Seduction")]
        Seduction,
        [Description("Sensationalist Press")]
        SensationalistPress,
        [Description("Sense of Emergency")]
        SenseofEmergency1,
        [Description("Sense of Emergency")]
        SenseofEmergency2,
        [Description("Sister Agatha")]
        SisterAgatha,
        [Description("Stormy Seas")]
        StormySeas,
        [Description("Surprising Return")]
        SurprisingReturn1,
        [Description("Surprising Return")]
        SurprisingReturn2,
        [Description("Telegraph Ahead")]
        TelegraphAhead1,
        [Description("Telegraph Ahead")]
        TelegraphAhead2,
        [Description("Time Runs Short")]
        TimeRunsShort,
        [Description("Trap")]
        Trap1,
        [Description("Trap")]
        Trap2,
        [Description("Trap")]
        Trap3,
        [Description("Unearthly Swiftness")]
        UnearthlySwiftness,
        [Description("Vampire Lair")]
        VampireLair,
        [Description("Vampiric Influence")]
        VampiricInfluence1,
        [Description("Vampiric Influence")]
        VampiricInfluence2,
        [Description("Wild Horses")]
        WildHorses
    }

    public enum Item
    {
        [Description("No item")]
        None,
        [Description("Crucifix")]
        Crucifix1,
        [Description("Crucifix")]
        Crucifix2,
        [Description("Crucifix")]
        Crucifix3,
        [Description("Dogs")]
        Dogs1,
        [Description("Dogs")]
        Dogs2,
        [Description("Fast Horse")]
        FastHorse1,
        [Description("Fast Horse")]
        FastHorse2,
        [Description("Fast Horse")]
        FastHorse3,
        [Description("Garlic")]
        Garlic1,
        [Description("Garlic")]
        Garlic2,
        [Description("Garlic")]
        Garlic3,
        [Description("Garlic")]
        Garlic4,
        [Description("Heavenly Host")]
        HeavenlyHost1,
        [Description("Heavenly Host")]
        HeavenlyHost2,
        [Description("Holy Water")]
        HolyWater1,
        [Description("Holy Water")]
        HolyWater2,
        [Description("Holy Water")]
        HolyWater3,
        [Description("Knife")]
        Knife1,
        [Description("Knife")]
        Knife2,
        [Description("Knife")]
        Knife3,
        [Description("Knife")]
        Knife4,
        [Description("Knife")]
        Knife5,
        [Description("Local Rumors")]
        LocalRumors1,
        [Description("Local Rumors")]
        LocalRumors2,
        [Description("Pistol")]
        Pistol1,
        [Description("Pistol")]
        Pistol2,
        [Description("Pistol")]
        Pistol3,
        [Description("Pistol")]
        Pistol4,
        [Description("Pistol")]
        Pistol5,
        [Description("Rifle")]
        Rifle1,
        [Description("Rifle")]
        Rifle2,
        [Description("Rifle")]
        Rifle3,
        [Description("Rifle")]
        Rifle4,
        [Description("Sacred Bullets")]
        SacredBullets1,
        [Description("Sacred Bullets")]
        SacredBullets2,
        [Description("Sacred Bullets")]
        SacredBullets3,
        [Description("Stake")]
        Stake1,
        [Description("Stake")]
        Stake2,
        [Description("Stake")]
        Stake3,
        [Description("Stake")]
        Stake4
    }

    public enum LocationType
    {
        [Description("None")]
        None,
        [Description("Small City")]
        SmallCity,
        [Description("Large City")]
        LargeCity,
        [Description("Sea")]
        Sea,
        [Description("Castle")]
        Castle,
        [Description("Hospital")]
        Hospital
    }

    public enum Location
    {
        [Description("Nowhere")]
        Nowhere,
        [Description("Adriatic Sea")]
        AdriaticSea,
        [Description("Alicante")]
        Alicante,
        [Description("Amsterdam")]
        Amsterdam,
        [Description("Athens")]
        Athens,
        [Description("Atlantic Ocean")]
        AtlanticOcean,
        [Description("Barcelona")]
        Barcelona,
        [Description("Bari")]
        Bari,
        [Description("Bay Of Biscay")]
        BayOfBiscay,
        [Description("Belgrade")]
        Belgrade,
        [Description("Berlin")]
        Berlin,
        [Description("Black Sea")]
        BlackSea,
        [Description("Bordeaux")]
        Bordeaux,
        [Description("Brussels")]
        Brussels,
        [Description("Bucharest")]
        Bucharest,
        [Description("Budapest")]
        Budapest,
        [Description("Cadiz")]
        Cadiz,
        [Description("Cagliari")]
        Cagliari,
        [Description("Castle Dracula")]
        CastleDracula,
        [Description("Clermont-Ferrand")]
        ClermontFerrand,
        [Description("Cologne")]
        Cologne,
        [Description("Constanta")]
        Constanta,
        [Description("Dublin")]
        Dublin,
        [Description("Edinburgh")]
        Edinburgh,
        [Description("English Channel")]
        EnglishChannel,
        [Description("Florence")]
        Florence,
        [Description("Frankfurt")]
        Frankfurt,
        [Description("Galatz")]
        Galatz,
        [Description("Galway")]
        Galway,
        [Description("Geneva")]
        Geneva,
        [Description("Genoa")]
        Genoa,
        [Description("Granada")]
        Granada,
        [Description("Hamburg")]
        Hamburg,
        [Description("Ionian Sea")]
        IonianSea,
        [Description("Irish Sea")]
        IrishSea,
        [Description("Klausenburg")]
        Klausenburg,
        [Description("Le Havre")]
        LeHavre,
        [Description("Leipzig")]
        Leipzig,
        [Description("Lisbon")]
        Lisbon,
        [Description("Liverpool")]
        Liverpool,
        [Description("London")]
        London,
        [Description("Madrid")]
        Madrid,
        [Description("Manchester")]
        Manchester,
        [Description("Marseilles")]
        Marseilles,
        [Description("Mediterranean Sea")]
        MediterraneanSea,
        [Description("Milan")]
        Milan,
        [Description("Munich")]
        Munich,
        [Description("Nantes")]
        Nantes,
        [Description("Naples")]
        Naples,
        [Description("North Sea")]
        NorthSea,
        [Description("Nuremburg")]
        Nuremburg,
        [Description("Paris")]
        Paris,
        [Description("Plymouth")]
        Plymouth,
        [Description("Prague")]
        Prague,
        [Description("Rome")]
        Rome,
        [Description("Salonica")]
        Salonica,
        [Description("Santander")]
        Santander,
        [Description("Saragossa")]
        Saragossa,
        [Description("Sarajevo")]
        Sarajevo,
        [Description("Sofia")]
        Sofia,
        [Description("St. Joseph and St. Mary")]
        StJosephAndStMary,
        [Description("Strasbourg")]
        Strasbourg,
        [Description("Swansea")]
        Swansea,
        [Description("Szeged")]
        Szeged,
        [Description("Toulouse")]
        Toulouse,
        [Description("Tyrrhenian Sea")]
        TyrrhenianSea,
        [Description("Valona")]
        Valona,
        [Description("Varna")]
        Varna,
        [Description("Venice")]
        Venice,
        [Description("Vienna")]
        Vienna,
        [Description("Zagreb")]
        Zagreb,
        [Description("Zurich")]
        Zurich
    }
}
