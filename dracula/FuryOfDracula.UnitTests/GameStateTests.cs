using FuryOfDracula.GameLogic;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FuryOfDracula.UnitTests
{
    [TestFixture]
    public class GameStateTests
    {
        public GameState game;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            game = new GameState();
        }

        [Test]
        public void GameState_DraculaInitialisation()
        {
            Assert.AreEqual(4, game.Dracula.EventHandSize);
        }

        [Test]
        public void GameState_HuntersInitialisation()
        {
            Assert.AreEqual(3, game.Hunters[(int)Hunter.VanHelsing].BitesRequiredToKill);
        }

        [Test]
        public void GetHunterFromInt_1_ReturnsLordGodalming()
        {
            Assert.AreEqual(Hunter.LordGodalming, game.GetHunterFromInt(1));
        }

        [Test]
        public void GetHunterFromInt_5_ReturnsNobody()
        {
            Assert.AreEqual(Hunter.Nobody, game.GetHunterFromInt(5));
        }

        [Test]
        public void GetHunterFromString_Bob_ReturnsNobody()
        {
            Assert.AreEqual(Hunter.Nobody, game.GetHunterFromString("Bob"));
        }

        [Test]
        public void GetHunterFromString_Van_ReturnsVanHelsing()
        {
            Assert.AreEqual(Hunter.VanHelsing, game.GetHunterFromString("van"));
        }

        [Test]
        public void DistanceByRoadOrSeaBetween_MadridMadrid_Returns0()
        {
            Assert.AreEqual(0, game.DistanceByRoadOrSeaBetween(Location.Madrid, Location.Madrid, true));
        }

        [Test]
        public void DistanceByRoadOrSeaBetween_MadridSantander_Returns1()
        {
            Assert.AreEqual(1, game.DistanceByRoadOrSeaBetween(Location.Madrid, Location.Santander, true));
        }

        [Test]
        public void DistanceByRoadOrSeaBetween_MadridBerlin_Returns5()
        {
            Assert.AreEqual(5, game.DistanceByRoadOrSeaBetween(Location.Madrid, Location.Berlin, true));
        }

        [Test]
        public void DistanceByRoadBetween_MadridBerlin_Returns8()
        {
            Assert.AreEqual(8, game.DistanceByRoadBetween(Location.Madrid, Location.Berlin, true));
        }

        [Test]
        public void DistanceByRoadBetween_MunichDublin_Returns99()
        {
            Assert.AreEqual(99, game.DistanceByRoadBetween(Location.Munich, Location.Dublin, true));
        }

        [Test]
        public void DistanceByTrainBetween_MunichDublin_Returns99()
        {
            Assert.AreEqual(99, game.DistanceByTrainBetween(Location.Munich, Location.Dublin, true));
        }

        [Test]
        public void DistanceByTrainBetween_LisbonSalonica_Returns16()
        {
            Assert.AreEqual(16, game.DistanceByTrainBetween(Location.Lisbon, Location.Salonica, true));
        }

        [Test]
        public void TestingTradeStuff()
        {
            Hunter firstHunterTrading = Hunter.LordGodalming;
            Hunter secondHunterTrading = Hunter.VanHelsing;
            //game.Hunters[(int)firstHunterTrading].DrawItemCard();
            game.Hunters[(int)secondHunterTrading].DrawItemCard();
            game.Hunters[(int)secondHunterTrading].DrawItemCard();
            var knownItemCard = game.ItemDeck.Find(card => card.Item == Item.Crucifix);
            game.Hunters[(int)firstHunterTrading].ItemsPartiallyKnownToDracula.Add(knownItemCard);
            game.Hunters[(int)firstHunterTrading].PartiallyKnownItemChances.Add(0.5F);
            game.ItemDeck.Remove(knownItemCard);
            knownItemCard = game.ItemDeck.Find(card => card.Item == Item.Crucifix);
            game.Hunters[(int)secondHunterTrading].ItemsPartiallyKnownToDracula.Add(knownItemCard);
            game.Hunters[(int)secondHunterTrading].PartiallyKnownItemChances.Add(0.5F);
            game.ItemDeck.Remove(knownItemCard);
            var allKnownItems = new List<ItemCard>();
            allKnownItems.AddRange(game.Hunters[(int)firstHunterTrading].ItemsKnownToDracula);
            allKnownItems.AddRange(game.Hunters[(int)secondHunterTrading].ItemsKnownToDracula);
            var allPartiallyKnownItems = new List<ItemCard>();
            allPartiallyKnownItems.AddRange(game.Hunters[(int)firstHunterTrading].ItemsPartiallyKnownToDracula);
            allPartiallyKnownItems.AddRange(game.Hunters[(int)secondHunterTrading].ItemsPartiallyKnownToDracula);
            var allItemChances = new List<float>();
            allItemChances.AddRange(game.Hunters[(int)firstHunterTrading].PartiallyKnownItemChances);
            allItemChances.AddRange(game.Hunters[(int)secondHunterTrading].PartiallyKnownItemChances);
            var newAllPartiallyKnownItems = new List<ItemCard>();
            var newAllItemChances = new List<float>();
            int index = -1;
            foreach (var i in allPartiallyKnownItems)
            {
                index++;
                if (!newAllPartiallyKnownItems.Any(card => card.Item == i.Item))
                {
                    float newChance = 0F;
                    for (int j = index; j < allPartiallyKnownItems.Count(); j++)
                    {
                        if (i.Item == allPartiallyKnownItems[j].Item)
                        {
                            newChance += allItemChances[j];
                        }
                    }
                    newAllPartiallyKnownItems.Add(i);
                    newAllItemChances.Add(newChance);
                }
            }
            allPartiallyKnownItems = newAllPartiallyKnownItems;
            allItemChances = newAllItemChances;

            if (game.Hunters[(int)firstHunterTrading].ItemCount == 0)
            {
                game.Hunters[(int)firstHunterTrading].ItemsKnownToDracula.Clear();
                game.Hunters[(int)firstHunterTrading].ItemsPartiallyKnownToDracula.Clear();
                game.Hunters[(int)firstHunterTrading].PartiallyKnownItemChances.Clear();
                game.Hunters[(int)secondHunterTrading].ItemsKnownToDracula = allKnownItems;
                game.Hunters[(int)secondHunterTrading].ItemsPartiallyKnownToDracula = allPartiallyKnownItems;
                game.Hunters[(int)secondHunterTrading].PartiallyKnownItemChances = allItemChances;
            }
            else if (game.Hunters[(int)secondHunterTrading].ItemCount == 0)
            {
                game.Hunters[(int)secondHunterTrading].ItemsKnownToDracula.Clear();
                game.Hunters[(int)secondHunterTrading].ItemsPartiallyKnownToDracula.Clear();
                game.Hunters[(int)secondHunterTrading].PartiallyKnownItemChances.Clear();
                game.Hunters[(int)firstHunterTrading].ItemsKnownToDracula = allKnownItems;
                game.Hunters[(int)firstHunterTrading].ItemsPartiallyKnownToDracula = allPartiallyKnownItems;
                game.Hunters[(int)firstHunterTrading].PartiallyKnownItemChances = allItemChances;
            }
            else
            {
                game.Hunters[(int)firstHunterTrading].ItemsKnownToDracula.Clear();
                game.Hunters[(int)secondHunterTrading].ItemsKnownToDracula.Clear();
                game.Hunters[(int)firstHunterTrading].ItemsPartiallyKnownToDracula.Clear();
                game.Hunters[(int)secondHunterTrading].ItemsPartiallyKnownToDracula.Clear();
                game.Hunters[(int)firstHunterTrading].ItemsPartiallyKnownToDracula.AddRange(allKnownItems);
                game.Hunters[(int)secondHunterTrading].ItemsPartiallyKnownToDracula.AddRange(allKnownItems);
                foreach (var i in allKnownItems)
                {
                    game.Hunters[(int)firstHunterTrading].PartiallyKnownItemChances.Add(0.5F);
                    game.Hunters[(int)secondHunterTrading].PartiallyKnownItemChances.Add(0.5F);
                }
                foreach (var f in allItemChances)
                {
                    game.Hunters[(int)firstHunterTrading].PartiallyKnownItemChances.Add(0.5F * f);
                    game.Hunters[(int)secondHunterTrading].PartiallyKnownItemChances.Add(0.5F * f);
                }

            }
            index = -1;
            foreach (var i in game.Hunters[(int)firstHunterTrading].ItemsPartiallyKnownToDracula)
            {
                index++;
                while (game.Hunters[(int)firstHunterTrading].PartiallyKnownItemChances[index] >= 1F)
                {
                    game.Hunters[(int)firstHunterTrading].ItemsKnownToDracula.Add(i);
                    game.Hunters[(int)firstHunterTrading].PartiallyKnownItemChances[index]--;
                }
            }
            index = -1;
            foreach (var f in game.Hunters[(int)firstHunterTrading].PartiallyKnownItemChances)
            {
                index++;
                if (f == 0)
                {
                    game.Hunters[(int)firstHunterTrading].ItemsPartiallyKnownToDracula[index] = null;
                }
            }
            game.Hunters[(int)firstHunterTrading].ItemsPartiallyKnownToDracula.RemoveAll(i => i == null);
            game.Hunters[(int)firstHunterTrading].PartiallyKnownItemChances.RemoveAll(f => f == 0);
            index = -1;
            foreach (var i in game.Hunters[(int)secondHunterTrading].ItemsPartiallyKnownToDracula)
            {
                index++;
                while (game.Hunters[(int)secondHunterTrading].PartiallyKnownItemChances[index] >= 1F)
                {
                    game.Hunters[(int)secondHunterTrading].ItemsKnownToDracula.Add(i);
                    game.Hunters[(int)secondHunterTrading].PartiallyKnownItemChances[index]--;
                }
            }
            index = -1;
            foreach (var f in game.Hunters[(int)secondHunterTrading].PartiallyKnownItemChances)
            {
                index++;
                if (f == 0)
                {
                    game.Hunters[(int)secondHunterTrading].ItemsPartiallyKnownToDracula[index] = null;
                }
            }
            game.Hunters[(int)secondHunterTrading].ItemsPartiallyKnownToDracula.RemoveAll(i => i == null);
            game.Hunters[(int)secondHunterTrading].PartiallyKnownItemChances.RemoveAll(f => f == 0);
        }
    }
}