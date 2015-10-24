using FuryOfDracula.GameLogic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.UnitTests
{
    [TestFixture]
    public class EventSetTests
    {
        public EventSet eventDeck;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            eventDeck = new EventSet();
        }

        [Test]
        public void GetEventsFromString_NewspaperReports_ReturnsAllNewspaperReportsEvents()
        {
            List<EventCard> newspaperEvents = eventDeck.GetEventsFromString("Newspaper");
            Assert.AreEqual(true, newspaperEvents.Contains(EventCard.NewspaperReports1));
            Assert.AreEqual(true, newspaperEvents.Contains(EventCard.NewspaperReports2));
            Assert.AreEqual(true, newspaperEvents.Contains(EventCard.NewspaperReports3));
            Assert.AreEqual(true, newspaperEvents.Contains(EventCard.NewspaperReports4));
            Assert.AreEqual(true, newspaperEvents.Contains(EventCard.NewspaperReports5));
            Assert.AreEqual(false, newspaperEvents.Contains(EventCard.NightVisit));
        }

        [Test]
        public void GetEventsFromString_MismatchingString_ReturnsEmptyList()
        {
            Assert.AreEqual(0, eventDeck.GetEventsFromString("ZZZZZ").Count());
        }

        [Test]
        public void GetEventsFromString_AmbiguousString_ReturnsEmptyList()
        {
            Assert.AreEqual(0, eventDeck.GetEventsFromString("RE").Count());
        }
    }
}
