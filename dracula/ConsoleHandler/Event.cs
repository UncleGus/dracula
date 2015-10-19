using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EventHandler
{
    public class EventDetail
    {
        public string name { get; set; }
        public bool isDraculaCard { get; set; }
        public EventType type { get; set; }

        public EventDetail(string newName, bool newIsDraculaCard, EventType eventType)
        {
            name = newName;
            isDraculaCard = newIsDraculaCard;
            type = eventType;
        }

    }

    public enum EventType
    {
        None,
        Ally,
        Keep,
        PlayImmediately
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
        DraculasBrides,
        EscapeRoute1,
        EscapeRoute2,
        Evasion,
        ExcellentWeathe1r,
        ExcellentWeather2,
        FalseTipoff1,
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
        QuinceyPMorris,
        Rage,
        ReEquip1,
        ReEquip2,
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
}
