using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    public static class Extensions
    {
        public static string Name(this Enum e)
        {
            return DescriptionAttr(e);
        }

        public static string DescriptionAttr<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                var description = Regex.Replace(source.ToString(), "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
                description = Regex.Replace(description, "([0-9])", "", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
                return description;
            }
        }
    }
    
    public enum EventType
    {
        None,
        Ally,
        Keep,
        PlayImmediately
    }

    public enum ConnectionType
    {
        None,
        Road,
        Rail,
        Sea,
        SenseOfEmergency
    }

    public enum Event
    {
        None,
        AdvancePlanning1,
        AdvancePlanning2,
        AdvancePlanning3,
        BloodTransfusion,
        CharteredCarriage1,
        CharteredCarriage2,
        CharteredCarriage3,
        ConsecratedGround,
        ControlStorms,
        CustomsSearch,
        DevilishPower1,
        DevilishPower2,
        [Description("Dracula's Brides")]
        DraculasBrides,
        EscapeRoute1,
        EscapeRoute2,
        Evasion,
        ExcellentWeather1,
        ExcellentWeather2,
        [Description("False Tip-off")]
        FalseTipoff1,
        [Description("False Tip-off")]
        FalseTipoff2,
        Forewarned1,
        Forewarned2,
        Forewarned3,
        GoodLuck1,
        GoodLuck2,
        GreatStrength,
        HeroicLeap,
        HiredScouts1,
        HiredScouts2,
        HiredScouts3,
        Hypnosis1,
        Hypnosis2,
        ImmanuelHildesheim,
        JonathanHarker,
        LongDay1,
        LongDay2,
        MoneyTrail,
        MysticResearch1,
        MysticResearch2,
        NewspaperReports1,
        NewspaperReports2,
        NewspaperReports3,
        NewspaperReports4,
        NewspaperReports5,
        NightVisit,
        [Description("Quincey P. Morris")]
        QuinceyPMorris,
        Rage,
        [Description("Re-Equip")]
        ReEquip1,
        [Description("Re-Equip")]
        ReEquip2,
        [Description("Re-Equip")]
        ReEquip3,
        RelentlessMinion1,
        RelentlessMinion2,
        Roadblock,
        RufusSmith,
        SecretWeapon1,
        SecretWeapon2,
        Seduction,
        SensationalistPress,
        SenseofEmergency1,
        SenseofEmergency2,
        SisterAgatha,
        StormySeas,
        SurprisingReturn1,
        SurprisingReturn2,
        TelegraphAhead1,
        TelegraphAhead2,
        TimeRunsShort,
        Trap1,
        Trap2,
        Trap3,
        UnearthlySwiftness,
        VampireLair,
        VampiricInfluence1,
        VampiricInfluence2,
        WildHorses
    }

    public enum Item
    {
        None,
        Crucifix1,
        Crucifix2,
        Crucifix3,
        Dogs1,
        Dogs2,
        FastHorse1,
        FastHorse2,
        FastHorse3,
        Garlic1,
        Garlic2,
        Garlic3,
        Garlic4,
        HeavenlyHost1,
        HeavenlyHost2,
        HolyWater1,
        HolyWater2,
        HolyWater3,
        Knife1,
        Knife2,
        Knife3,
        Knife4,
        Knife5,
        LocalRumors1,
        LocalRumors2,
        Pistol1,
        Pistol2,
        Pistol3,
        Pistol4,
        Pistol5,
        Rifle1,
        Rifle2,
        Rifle3,
        Rifle4,
        SacredBullets1,
        SacredBullets2,
        SacredBullets3,
        Stake1,
        Stake2,
        Stake3,
        Stake4,
        Punch,
        Dodge,
        Escape
    }

    public enum LocationType
    {
        None,
        SmallCity,
        LargeCity,
        Sea,
        Castle,
        Hospital
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
        [Description("Clermont-Ferrand")]
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
        [Description("St. Joseph and St. Mary")]
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

    public enum Encounter
    {
        None,
        Ambush1,
        Ambush2,
        Ambush3,
        Assassin,
        Bats1,
        Bats2,
        Bats3,
        DesecratedSoil1,
        DesecratedSoil2,
        DesecratedSoil3,
        Fog1,
        Fog2,
        Fog3,
        Fog4,
        MinionWithKnife1,
        MinionWithKnife2,
        MinionWithKnife3,
        MinionWithKnifeAndPistol1,
        MinionWithKnifeAndPistol2,
        MinionWithKnifeAndRifle1,
        MinionWithKnifeAndRifle2,
        Hoax1,
        Hoax2,
        Lightning1,
        Lightning2,
        Peasants1,
        Peasants2,
        Plague,
        Rats1,
        Rats2,
        Saboteur1,
        Saboteur2,
        Spy1,
        Spy2,
        Thief1,
        Thief2,
        NewVampire1,
        NewVampire2,
        NewVampire3,
        NewVampire4,
        NewVampire5,
        NewVampire6,
        Wolves1,
        Wolves2,
        Wolves3
    }

    public enum Hunter
    {
        Nobody,
        LordGodalming,
        [Description("Dr. Seward")]
        DrSeward,
        VanHelsing,
        MinaHarker
    }

    public enum EnemyCombatCards
    {
        None,
        Punch,
        Knife,
        Rifle,
        Dodge,
        Claws,
        [Description("Escape (Man)")]
        EscapeMan,
        [Description("Escape (Bat)")]
        EscapeBat,
        [Description("Escape (Mist)")]
        EscapeMist,
        Fangs,
        Mesmerize,
        Strength
    }
}
