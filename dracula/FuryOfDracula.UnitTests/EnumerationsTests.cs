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
    public class EnumerationsTests
    {
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
    }
}
