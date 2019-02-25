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
            var topGames = new[]
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
            for (int i = 0, j = 1; i < topGames.Length; i++, j++)
                _memory.ZAdd("topGames", topGames[i], j);

            var list = _memory.Get("topGames") as List<CacheItem>;

            Assert.AreEqual(14, list.Count);

        }

    }
}
