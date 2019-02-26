using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using InMemoryCache.Caching;
using NUnit.Framework;

namespace InMemoryCacheTest.CachingTest
{
    [TestFixture]
    public class InMemoryCacheTest
    {
        private InMemoryCache.Caching.InMemoryCache _memory;
        [SetUp]
        public void SetUp()
        {
            _memory = new InMemoryCache.Caching.InMemoryCache();
        }

        [Test]
        public void Set_Entry()
        {
            Assert.IsTrue(_memory.Set("mytest", "isWork"));
        }

        [Test]
        public void Get_Entry()
        {
            Assert.IsTrue(_memory.Set("getEntry", "isGetTest"));
            Assert.AreEqual("isGetTest", _memory.Get("getEntry"));
        }

        [Test]
        public void Set_Entry_With_Expire()
        {
            var setResult = _memory.Set("entryWithExpire", "myAnotherTest", TimeSpan.FromSeconds(2));
            Assert.IsTrue(setResult);
            Assert.AreEqual("myAnotherTest", _memory.Get("entryWithExpire"));
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Assert.IsNull(_memory.Get("entryWithExpire"));
        }

        [Test]
        public void Del_Entry()
        {
            _memory.Set("myRemovedItem", "");
            Assert.IsTrue(_memory.Del("myRemovedItem"));
            Assert.IsNull(_memory.Get("myRemovedItem"));
        }


        [Test]
        public void DbSize()
        {
            _memory = new InMemoryCache.Caching.InMemoryCache();
            Enumerable.Range(1, 10).ToList().ForEach(i => _memory.Set($"myKey{i}", i));
            Assert.AreEqual(10, _memory.DbSize());
        }


        [Test]
        public void DbSize_Removing_Entry()
        {
            _memory = new InMemoryCache.Caching.InMemoryCache();
            Enumerable.Range(1, 10).ToList().ForEach(i => _memory.Set($"myKey{i}", i));
            _memory.Del("myKey1");
            Assert.AreEqual(9, _memory.DbSize());
        }

        [Test]
        public void Increment_Entry()
        {
            _memory.Set("myIncrementTest", 1);
            Assert.AreEqual(2, _memory.Incr("myIncrementTest"));
        }

        [Test]
        public void Increment_Entry_Not_Exist()
        {
            Assert.AreEqual(1, _memory.Incr("myIncrementNotExistTest"));
        }

        [Test]
        public void Zadd_Entry()
        {
            for (int i = 0, j = 1; i < _topGames.Length; i++, j++)
                _memory.ZAdd("topGames", _topGames[i], j);

            var list = _memory.Get("topGames") as List<SortedCacheItem>;

            Assert.AreEqual(14, list.Count);
        }

        [Test]
        public void ZCard()
        {
            for (int i = 0, j = 1; i < _topGames.Length; i++, j++)
                _memory.ZAdd("topGamesZcard", _topGames[i], j);

            Assert.AreEqual(14, _memory.ZCard("topGamesZcard"));
        }

        [Test]
        public void ZRank()
        {
            _memory.ZAdd("myzset", "one", 1);
            _memory.ZAdd("myzset", "two", 1);
            _memory.ZAdd("myzset", "three", 1);
            
            Assert.AreEqual(2, _memory.ZRank("myzset", "three"));
        }

        [Test]
        public void ZRange_All_Elements()
        {
            _memory.ZAdd("myzRange", "one", 1);
            _memory.ZAdd("myzRange", "two", 1);
            _memory.ZAdd("myzRange", "three", 1);

            var ranked = _memory.ZRange<string>("myzRange", 0, -1);
            Assert.AreEqual(3,ranked.Count);
            Assert.AreEqual("one",ranked.First());
            Assert.AreEqual("three", ranked.Last());
        }

        [Test]
        public void ZRange_Get_One_Range_Element()
        {
            _memory.ZAdd("myzRange2", "one", 1);
            _memory.ZAdd("myzRange2", "two", 1);
            _memory.ZAdd("myzRange2", "three", 1);

            var ranked = _memory.ZRange<string>("myzRange2", 2, 3);
            Assert.AreEqual(1, ranked.Count);
            Assert.AreEqual("three", ranked.First());            
        }

        [Test]
        public void ZRange_Get_Elements_With_Last_Elements()
        {
            _memory.ZAdd("myzRange3", "one", 1);
            _memory.ZAdd("myzRange3", "two", 1);
            _memory.ZAdd("myzRange3", "three", 1);

            var ranked = _memory.ZRange<string>("myzRange3", -2, -1);
            Assert.AreEqual(2, ranked.Count);
            Assert.AreEqual("two", ranked.First());
            Assert.AreEqual("three", ranked.Last());
        }

        private readonly string[] _topGames = new[]
        {
            "Apex Legends",
            "Slay the Spire",
            "Resident Evil 2",
            "Rainbow Six Siege",
            "Wargroove",
            "Fortnite Battle Royale",
            "Into the Breach",
            "CS:GO",
            "Warframe",
            "Dusk",
            "Battlefield 5",
            "Hitman 2",
            "PUBG",
            "Return of the Obra Dinn"

        };

    }
}
