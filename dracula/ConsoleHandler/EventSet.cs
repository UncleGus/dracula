using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraculaSimulator
{
    public class EventSet
    {
        private EventDetail[] EventDeck;

        public EventSet()
        {
            EventDeck = CreateEventDeck();
        }

        private EventDetail[] CreateEventDeck()
        {
            EventDetail[] tempEventDeck = new EventDetail[76];
            tempEventDeck[(int)Event.None] = new EventDetail("None", false, EventType.None);
            tempEventDeck[(int)Event.AdvancePlanning1] = new EventDetail("Advance Planning", false, EventType.Keep);
            tempEventDeck[(int)Event.AdvancePlanning2] = new EventDetail("Advance Planning", false, EventType.Keep);
            tempEventDeck[(int)Event.AdvancePlanning3] = new EventDetail("Advance Planning", false, EventType.Keep);
            tempEventDeck[(int)Event.BloodTransfusion] = new EventDetail("Blood Transfusion", false, EventType.Keep);
            tempEventDeck[(int)Event.CharteredCarriage1] = new EventDetail("Chartered Carriage", false, EventType.Keep);
            tempEventDeck[(int)Event.CharteredCarriage2] = new EventDetail("Chartered Carriage", false, EventType.Keep);
            tempEventDeck[(int)Event.CharteredCarriage3] = new EventDetail("Chartered Carriage", false, EventType.Keep);
            tempEventDeck[(int)Event.ConsecratedGround] = new EventDetail("Consecrated Ground", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.ControlStorms] = new EventDetail("Control Storms", true, EventType.Keep);
            tempEventDeck[(int)Event.CustomsSearch] = new EventDetail("Customs Search", true, EventType.Keep);
            tempEventDeck[(int)Event.DevilishPower1] = new EventDetail("Devilish Power", true, EventType.Keep);
            tempEventDeck[(int)Event.DevilishPower2] = new EventDetail("Devilish Power", true, EventType.Keep);
            tempEventDeck[(int)Event.DraculasBrides] = new EventDetail("Dracula's Brides", true, EventType.Ally);
            tempEventDeck[(int)Event.EscapeRoute1] = new EventDetail("Escape Route", false, EventType.Keep);
            tempEventDeck[(int)Event.EscapeRoute2] = new EventDetail("Escape Route", false, EventType.Keep);
            tempEventDeck[(int)Event.Evasion] = new EventDetail("Evasion", true, EventType.PlayImmediately);
            tempEventDeck[(int)Event.ExcellentWeathe1r] = new EventDetail("Excellent Weather", false, EventType.Keep);
            tempEventDeck[(int)Event.ExcellentWeather2] = new EventDetail("Excellent Weather", false, EventType.Keep);
            tempEventDeck[(int)Event.FalseTipoff1] = new EventDetail("False Tip-Off", true, EventType.Keep);
            tempEventDeck[(int)Event.FalseTipoff2] = new EventDetail("False Tip-Off", true, EventType.Keep);
            tempEventDeck[(int)Event.Forewarned1] = new EventDetail("Forewarned", false, EventType.Keep);
            tempEventDeck[(int)Event.Forewarned2] = new EventDetail("Forewarned", false, EventType.Keep);
            tempEventDeck[(int)Event.Forewarned3] = new EventDetail("Forewarned", false, EventType.Keep);
            tempEventDeck[(int)Event.GoodLuck1] = new EventDetail("Good Luck", false, EventType.Keep);
            tempEventDeck[(int)Event.GoodLuck2] = new EventDetail("Good Luck", false, EventType.Keep);
            tempEventDeck[(int)Event.GreatStrength] = new EventDetail("Great Strength", false, EventType.Keep);
            tempEventDeck[(int)Event.HeroicLeap] = new EventDetail("Heroic Leap", false, EventType.Keep);
            tempEventDeck[(int)Event.HiredScouts1] = new EventDetail("Hired Scouts", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.HiredScouts2] = new EventDetail("Hired Scouts", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.HiredScouts3] = new EventDetail("Hired Scouts", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.Hypnosis1] = new EventDetail("Hypnosis", false, EventType.Keep);
            tempEventDeck[(int)Event.Hypnosis2] = new EventDetail("Hypnosis", false, EventType.Keep);
            tempEventDeck[(int)Event.ImmanuelHildesheim] = new EventDetail("Immanuel Hildesheim", true, EventType.Ally);
            tempEventDeck[(int)Event.JonathanHarker] = new EventDetail("Jonathan Harker", false, EventType.Ally);
            tempEventDeck[(int)Event.LongDay1] = new EventDetail("Long Day", false, EventType.Keep);
            tempEventDeck[(int)Event.LongDay2] = new EventDetail("Long Day", false, EventType.Keep);
            tempEventDeck[(int)Event.MoneyTrail] = new EventDetail("Money Trail", false, EventType.Keep);
            tempEventDeck[(int)Event.MysticResearch1] = new EventDetail("Mystic Research", false, EventType.Keep);
            tempEventDeck[(int)Event.MysticResearch2] = new EventDetail("Mystic Research", false, EventType.Keep);
            tempEventDeck[(int)Event.NewspaperReports1] = new EventDetail("Newspaper Reports", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.NewspaperReports2] = new EventDetail("Newspaper Reports", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.NewspaperReports3] = new EventDetail("Newspaper Reports", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.NewspaperReports4] = new EventDetail("Newspaper Reports", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.NewspaperReports5] = new EventDetail("Newspaper Reports", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.NightVisit] = new EventDetail("Night Visit", true, EventType.PlayImmediately);
            tempEventDeck[(int)Event.QuinceyPMorris] = new EventDetail("Quincey P. Morris", true, EventType.Ally);
            tempEventDeck[(int)Event.Rage] = new EventDetail("Rage", true, EventType.Keep);
            tempEventDeck[(int)Event.ReEquip1] = new EventDetail("Re-Equip", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.ReEquip2] = new EventDetail("Re-Equip", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.ReEquip3] = new EventDetail("Re-Equip", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.RelentlessMinion1] = new EventDetail("Relentless Minion", true, EventType.Keep);
            tempEventDeck[(int)Event.RelentlessMinion2] = new EventDetail("Relentless Minion", true, EventType.Keep);
            tempEventDeck[(int)Event.Roadblock] = new EventDetail("Roadblock", true, EventType.Keep);
            tempEventDeck[(int)Event.RufusSmith] = new EventDetail("Rufus Smith", false, EventType.Ally);
            tempEventDeck[(int)Event.SecretWeapon1] = new EventDetail("Secret Weapon", false, EventType.Keep);
            tempEventDeck[(int)Event.SecretWeapon2] = new EventDetail("Secret Weapon", false, EventType.Keep);
            tempEventDeck[(int)Event.Seduction] = new EventDetail("Seduction", true, EventType.Keep);
            tempEventDeck[(int)Event.SensationalistPress] = new EventDetail("Sensationalist Press", true, EventType.Keep);
            tempEventDeck[(int)Event.SenseofEmergency1] = new EventDetail("Sense of Emergency", false, EventType.Keep);
            tempEventDeck[(int)Event.SenseofEmergency2] = new EventDetail("Sense of Emergency", false, EventType.Keep);
            tempEventDeck[(int)Event.SisterAgatha] = new EventDetail("Sister Agatha", false, EventType.Ally);
            tempEventDeck[(int)Event.StormySeas] = new EventDetail("Stormy Seas", false, EventType.Keep);
            tempEventDeck[(int)Event.SurprisingReturn1] = new EventDetail("Surprising Return", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.SurprisingReturn2] = new EventDetail("Surprising Return", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.TelegraphAhead1] = new EventDetail("Telegraph Ahead", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.TelegraphAhead2] = new EventDetail("Telegraph Ahead", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.TimeRunsShort] = new EventDetail("Time Runs Short", true, EventType.Keep);
            tempEventDeck[(int)Event.Trap1] = new EventDetail("Trap", true, EventType.Keep);
            tempEventDeck[(int)Event.Trap2] = new EventDetail("Trap", true, EventType.Keep);
            tempEventDeck[(int)Event.Trap3] = new EventDetail("Trap", true, EventType.Keep);
            tempEventDeck[(int)Event.UnearthlySwiftness] = new EventDetail("Unearthly Swiftness", true, EventType.Keep);
            tempEventDeck[(int)Event.VampireLair] = new EventDetail("Vampir Lair", false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.VampiricInfluence1] = new EventDetail("Vampiric Influence", true, EventType.PlayImmediately);
            tempEventDeck[(int)Event.VampiricInfluence2] = new EventDetail("Vampiric Influence", true, EventType.PlayImmediately);
            tempEventDeck[(int)Event.WildHorses] = new EventDetail("Wild Horses", true, EventType.Keep);
            return tempEventDeck;
        }
    }
}
