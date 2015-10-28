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
    public static class Enumerations
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

        public static List<Power> GetAvailablePowers(TimeOfDay timeOfDay)
        {
            List<Power> tempListOfPowers = new List<Power>() { Power.DoubleBack, Power.Hide };

            if ((int)timeOfDay > 3) {
                tempListOfPowers.Add(Power.DarkCall);
                tempListOfPowers.Add(Power.Feed);
                tempListOfPowers.Add(Power.WolfForm);
            }
            return tempListOfPowers;
        }

        public static Item GetItemFromString(string name)
        {
            for (int i = 0; i < 16; i++)
            {
                if (((Item)i).Name().ToLower().StartsWith(name.ToLower()))
                {
                    return (Item)i;
                }
            }
            return Item.None;
        }

        public static Event GetEventFromString(string name)
        {
            for (int i = 0; i < 46; i++)
            {
                if (((Event)i).Name().ToLower().StartsWith(name.ToLower()))
                {
                    return (Event)i;
                }
            }
            return Event.None;
        }

        public static Location GetLocationFromString(string name)
        {
            Location tempLocation = Location.Nowhere;
            int countOfMatches = 0;
            for (int i = 0; i < 72; i ++)
            {
                if (((Location)i).Name().ToLower().StartsWith(name.ToLower()))
                {
                    countOfMatches++;
                    if (tempLocation == Location.Nowhere)
                    {
                        tempLocation = (Location)i;
                    }
                }
            }
            if (countOfMatches == 1)
            {
                return tempLocation;
            }
            else
            {
                return Location.Nowhere;
            }
        }

        public static List<Location> GetAllLocations()
        {
            List<Location> tempListOfLocations = new List<Location>();
            for (int i = 1; i < 72; i++) {
                tempListOfLocations.Add((Location)i);
            }
            return tempListOfLocations;
        }

        public static ConnectionType GetConnectionTypeFromString(string input)
        {
            for (int i = 0; i < 5; i++)
            {
                if (((ConnectionType)i).Name().ToLower().StartsWith(input.ToLower()))
                {
                    return (ConnectionType)i;
                }
            }
            return ConnectionType.None;
        }

        public static ResolveAbility GetResolveAbilityFromString(string input)
        {
            for (int i = 1; i < 4; i++)
            {
                if (((ResolveAbility)i).Name().ToLower().StartsWith(input.ToLower()))
                {
                    return (ResolveAbility)i;
                }
            }
            return ResolveAbility.None;
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
        AdvancePlanning,
        BloodTransfusion,
        CharteredCarriage,
        ConsecratedGround,
        ControlStorms,
        CustomsSearch,
        DevilishPower,
        [Description("Dracula's Brides")]
        DraculasBrides,
        EscapeRoute,
        Evasion,
        ExcellentWeather,
        [Description("False Tip-off")]
        FalseTipoff,
        Forewarned,
        GoodLuck,
        GreatStrength,
        HeroicLeap,
        HiredScouts,
        Hypnosis,
        ImmanuelHildesheim,
        JonathanHarker,
        LongDay,
        MoneyTrail,
        MysticResearch,
        NewspaperReports,
        NightVisit,
        [Description("Quincey P. Morris")]
        QuinceyPMorris,
        Rage,
        [Description("Re-Equip")]
        ReEquip,
        RelentlessMinion,
        Roadblock,
        RufusSmith,
        SecretWeapon,
        Seduction,
        SensationalistPress,
        SenseofEmergency,
        SisterAgatha,
        StormySeas,
        SurprisingReturn,
        TelegraphAhead,
        TimeRunsShort,
        Trap,
        UnearthlySwiftness,
        VampireLair,
        VampiricInfluence,
        WildHorses
    }

    public enum Item
    {
        None,
        Crucifix,
        Dogs,
        FastHorse,
        Garlic,
        HeavenlyHost,
        HolyWater,
        Knife,
        LocalRumors,
        Pistol,
        Rifle,
        SacredBullets,
        Stake,
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
        Ambush,
        Assassin,
        Bats,
        DesecratedSoil,
        Fog,
        MinionWithKnife,
        MinionWithKnifeAndPistol,
        MinionWithKnifeAndRifle,
        Hoax,
        Lightning,
        Peasants,
        Plague,
        Rats,
        Saboteur,
        Spy,
        Thief,
        NewVampire,
        Wolves
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

    public enum EnemyCombatCard
    {
        None,
        Punch,
        Knife,
        Pistol,
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

    public enum Power
    {
        None,
        Hide,
        DarkCall,
        Feed,
        WolfForm,
        DoubleBack
    }

    public enum TimeOfDay
    {
        None,
        Dawn,
        Noon,
        Dusk,
        Twilight,
        Midnight,
        SmallHours
    }

    public enum Opponent
    {
        None,
        MinionWithKnife,
        MinionWithKnifeAndPistol,
        MinionWithKnifeAndRifle,
        Assassin,
        Dracula,
        NewVampire
    }

    public enum CardType
    {
        None,
        Item,
        Event
    }

    public enum DevilishPowerTarget
    {
        None,
        HeavenlyHost1,
        HeavenlyHost2,
        HunterAlly
    }

    public enum ResolveAbility
    {
        None,
        NewspaperReports,
        InnerStrength,
        SenseOfEmergency
    }
}
