using System;
using System.Linq;
using Assets.Classes.EntryProviders.OnlineDatabase;
using Assets.Classes.GraphElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArachneeTests
{
    [TestClass]
    public class Tests_OnlineDatabase
    {
        [TestMethod]
        public void Test_TryGetEntry()
        {
            var onlineDb = new OnlineDatabase();
            Entry e;
            var res = onlineDb.TryGetEntry("Movie-218", out e);

            Assert.IsTrue(res);
            Assert.IsNotNull(e);
            Assert.AreEqual("Movie-218", e.Id);

            var movie = e as Movie;
            Assert.IsNotNull(movie);
            Assert.AreEqual("The Terminator", movie.Title);
        }

        [TestMethod]
        public void Test_Search()
        {
            var onlineDb = new OnlineDatabase();
            var results = onlineDb.GetSearchResults<Movie>("the terminator");
            
            Assert.IsTrue(results.Any());

            var bestResult = results.Dequeue();

            Assert.AreEqual("The Terminator", bestResult.Title);
            Assert.AreEqual("Movie-218", bestResult.Id);

            results = onlineDb.GetSearchResults<Movie>(string.Empty);
            Assert.IsFalse(results.Any());
        }
    }
}
