using FuryOfDracula.GameLogic;
using NUnit.Framework;

namespace FuryOfDracula.UnitTests
{
    [TestFixture]
    public class EnumerationsTests
    {
        [Test]
        public void GetEventFromString_GOO_ReturnsEventGoodLuck()
        {
            Assert.AreEqual(Event.GoodLuck, Enumerations.GetEventFromString("GOO"));
        }

        [Test]
        public void GetItemFromString_PIS_ReturnsItemPistol()
        {
            Assert.AreEqual(Item.Pistol, Enumerations.GetItemFromString("PIS"));
        }

        [Test]
        public void Name_CamelCasedLocationWithoutDescription_ReturnsEnumValueWithSpaces()
        {
            Assert.AreEqual("Adriatic Sea", Location.AdriaticSea.Name());
        }

        [Test]
        public void Name_EnumValueWithNumberWithoutDescription_ReturnsEnumValueWithoutNumber()
        {
            Assert.AreEqual("Advance Planning", Event.AdvancePlanning.Name());
        }

        [Test]
        public void Name_LocationWithDescription_ReturnsDescription()
        {
            Assert.AreEqual("Clermont-Ferrand", Location.ClermontFerrand.Name());
        }

        [Test]
        public void Name_SingleWordLocationWithoutDescription_ReturnsEnumValue()
        {
            Assert.AreEqual("Paris", Location.Paris.Name());
        }
    }
}