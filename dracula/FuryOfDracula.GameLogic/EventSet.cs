using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    public class EventSet
    {
        private EventDetail[] eventDeck;

        public EventSet()
        {
            eventDeck = CreateEventDeck();
        }

        private EventDetail[] CreateEventDeck()
        {
            EventDetail[] tempEventDeck = new EventDetail[76];
            tempEventDeck[(int)Event.None] = new EventDetail(Event.None, false, EventType.None);
            tempEventDeck[(int)Event.AdvancePlanning1] = new EventDetail(Event.AdvancePlanning1, false, EventType.Keep);
            tempEventDeck[(int)Event.AdvancePlanning2] = new EventDetail(Event.AdvancePlanning2, false, EventType.Keep);
            tempEventDeck[(int)Event.AdvancePlanning3] = new EventDetail(Event.AdvancePlanning3, false, EventType.Keep);
            tempEventDeck[(int)Event.BloodTransfusion] = new EventDetail(Event.BloodTransfusion, false, EventType.Keep);
            tempEventDeck[(int)Event.CharteredCarriage1] = new EventDetail(Event.CharteredCarriage1, false, EventType.Keep);
            tempEventDeck[(int)Event.CharteredCarriage2] = new EventDetail(Event.CharteredCarriage2, false, EventType.Keep);
            tempEventDeck[(int)Event.CharteredCarriage3] = new EventDetail(Event.CharteredCarriage3, false, EventType.Keep);
            tempEventDeck[(int)Event.ConsecratedGround] = new EventDetail(Event.ConsecratedGround, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.ControlStorms] = new EventDetail(Event.ControlStorms, true, EventType.Keep);
            tempEventDeck[(int)Event.CustomsSearch] = new EventDetail(Event.CustomsSearch, true, EventType.Keep);
            tempEventDeck[(int)Event.DevilishPower1] = new EventDetail(Event.DevilishPower1, true, EventType.Keep);
            tempEventDeck[(int)Event.DevilishPower2] = new EventDetail(Event.DevilishPower2, true, EventType.Keep);
            tempEventDeck[(int)Event.DraculasBrides] = new EventDetail(Event.DraculasBrides, true, EventType.Ally);
            tempEventDeck[(int)Event.EscapeRoute1] = new EventDetail(Event.EscapeRoute1, false, EventType.Keep);
            tempEventDeck[(int)Event.EscapeRoute2] = new EventDetail(Event.EscapeRoute2, false, EventType.Keep);
            tempEventDeck[(int)Event.Evasion] = new EventDetail(Event.Evasion, true, EventType.PlayImmediately);
            tempEventDeck[(int)Event.ExcellentWeather1] = new EventDetail(Event.ExcellentWeather1, false, EventType.Keep);
            tempEventDeck[(int)Event.ExcellentWeather2] = new EventDetail(Event.ExcellentWeather2, false, EventType.Keep);
            tempEventDeck[(int)Event.FalseTipoff1] = new EventDetail(Event.FalseTipoff1, true, EventType.Keep);
            tempEventDeck[(int)Event.FalseTipoff2] = new EventDetail(Event.FalseTipoff2, true, EventType.Keep);
            tempEventDeck[(int)Event.Forewarned1] = new EventDetail(Event.Forewarned1, false, EventType.Keep);
            tempEventDeck[(int)Event.Forewarned2] = new EventDetail(Event.Forewarned2, false, EventType.Keep);
            tempEventDeck[(int)Event.Forewarned3] = new EventDetail(Event.Forewarned3, false, EventType.Keep);
            tempEventDeck[(int)Event.GoodLuck1] = new EventDetail(Event.GoodLuck1, false, EventType.Keep);
            tempEventDeck[(int)Event.GoodLuck2] = new EventDetail(Event.GoodLuck2, false, EventType.Keep);
            tempEventDeck[(int)Event.GreatStrength] = new EventDetail(Event.GreatStrength, false, EventType.Keep);
            tempEventDeck[(int)Event.HeroicLeap] = new EventDetail(Event.HeroicLeap, false, EventType.Keep);
            tempEventDeck[(int)Event.HiredScouts1] = new EventDetail(Event.HiredScouts1, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.HiredScouts2] = new EventDetail(Event.HiredScouts2, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.HiredScouts3] = new EventDetail(Event.HiredScouts3, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.Hypnosis1] = new EventDetail(Event.Hypnosis1, false, EventType.Keep);
            tempEventDeck[(int)Event.Hypnosis2] = new EventDetail(Event.Hypnosis2, false, EventType.Keep);
            tempEventDeck[(int)Event.ImmanuelHildesheim] = new EventDetail(Event.ImmanuelHildesheim, true, EventType.Ally);
            tempEventDeck[(int)Event.JonathanHarker] = new EventDetail(Event.JonathanHarker, false, EventType.Ally);
            tempEventDeck[(int)Event.LongDay1] = new EventDetail(Event.LongDay1, false, EventType.Keep);
            tempEventDeck[(int)Event.LongDay2] = new EventDetail(Event.LongDay2, false, EventType.Keep);
            tempEventDeck[(int)Event.MoneyTrail] = new EventDetail(Event.MoneyTrail, false, EventType.Keep);
            tempEventDeck[(int)Event.MysticResearch1] = new EventDetail(Event.MysticResearch1, false, EventType.Keep);
            tempEventDeck[(int)Event.MysticResearch2] = new EventDetail(Event.MysticResearch2, false, EventType.Keep);
            tempEventDeck[(int)Event.NewspaperReports1] = new EventDetail(Event.NewspaperReports1, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.NewspaperReports2] = new EventDetail(Event.NewspaperReports2, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.NewspaperReports3] = new EventDetail(Event.NewspaperReports3, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.NewspaperReports4] = new EventDetail(Event.NewspaperReports4, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.NewspaperReports5] = new EventDetail(Event.NewspaperReports5, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.NightVisit] = new EventDetail(Event.NightVisit, true, EventType.PlayImmediately);
            tempEventDeck[(int)Event.QuinceyPMorris] = new EventDetail(Event.QuinceyPMorris, true, EventType.Ally);
            tempEventDeck[(int)Event.Rage] = new EventDetail(Event.Rage, true, EventType.Keep);
            tempEventDeck[(int)Event.ReEquip1] = new EventDetail(Event.ReEquip1, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.ReEquip2] = new EventDetail(Event.ReEquip2, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.ReEquip3] = new EventDetail(Event.ReEquip3, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.RelentlessMinion1] = new EventDetail(Event.RelentlessMinion1, true, EventType.Keep);
            tempEventDeck[(int)Event.RelentlessMinion2] = new EventDetail(Event.RelentlessMinion2, true, EventType.Keep);
            tempEventDeck[(int)Event.Roadblock] = new EventDetail(Event.Roadblock, true, EventType.Keep);
            tempEventDeck[(int)Event.RufusSmith] = new EventDetail(Event.RufusSmith, false, EventType.Ally);
            tempEventDeck[(int)Event.SecretWeapon1] = new EventDetail(Event.SecretWeapon1, false, EventType.Keep);
            tempEventDeck[(int)Event.SecretWeapon2] = new EventDetail(Event.SecretWeapon2, false, EventType.Keep);
            tempEventDeck[(int)Event.Seduction] = new EventDetail(Event.Seduction, true, EventType.Keep);
            tempEventDeck[(int)Event.SensationalistPress] = new EventDetail(Event.SensationalistPress, true, EventType.Keep);
            tempEventDeck[(int)Event.SenseofEmergency1] = new EventDetail(Event.SenseofEmergency1, false, EventType.Keep);
            tempEventDeck[(int)Event.SenseofEmergency2] = new EventDetail(Event.SenseofEmergency2, false, EventType.Keep);
            tempEventDeck[(int)Event.SisterAgatha] = new EventDetail(Event.SisterAgatha, false, EventType.Ally);
            tempEventDeck[(int)Event.StormySeas] = new EventDetail(Event.StormySeas, false, EventType.Keep);
            tempEventDeck[(int)Event.SurprisingReturn1] = new EventDetail(Event.SurprisingReturn1, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.SurprisingReturn2] = new EventDetail(Event.SurprisingReturn2, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.TelegraphAhead1] = new EventDetail(Event.TelegraphAhead1, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.TelegraphAhead2] = new EventDetail(Event.TelegraphAhead2, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.TimeRunsShort] = new EventDetail(Event.TimeRunsShort, true, EventType.Keep);
            tempEventDeck[(int)Event.Trap1] = new EventDetail(Event.Trap1, true, EventType.Keep);
            tempEventDeck[(int)Event.Trap2] = new EventDetail(Event.Trap2, true, EventType.Keep);
            tempEventDeck[(int)Event.Trap3] = new EventDetail(Event.Trap3, true, EventType.Keep);
            tempEventDeck[(int)Event.UnearthlySwiftness] = new EventDetail(Event.UnearthlySwiftness, true, EventType.Keep);
            tempEventDeck[(int)Event.VampireLair] = new EventDetail(Event.VampireLair, false, EventType.PlayImmediately);
            tempEventDeck[(int)Event.VampiricInfluence1] = new EventDetail(Event.VampiricInfluence1, true, EventType.PlayImmediately);
            tempEventDeck[(int)Event.VampiricInfluence2] = new EventDetail(Event.VampiricInfluence2, true, EventType.PlayImmediately);
            tempEventDeck[(int)Event.WildHorses] = new EventDetail(Event.WildHorses, true, EventType.Keep);
            return tempEventDeck;
        }

        public List<Event> GetEventsFromString(string line)
        {
            List<Event> tempEventList = new List<Event>();
            foreach (EventDetail e in eventDeck)
            {
                if (e.Event.Name().ToLower().StartsWith(line.ToLower()))
                {
                    tempEventList.Add(e.Event);
                }
            }
            foreach (Event e in tempEventList)
            {
                if (e.Name() != tempEventList.First().Name())
                {
                    return new List<Event>();
                }
            }
            return tempEventList;
        }

    }
}
