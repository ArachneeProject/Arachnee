using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Assets.Classes.EntryProviders.OnlineDatabase;
using Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients.Builders.FromMedia;
using Assets.Classes.GraphElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArachneeTests
{
    [TestClass]
    public class Tests_OnlineDatabase
    {
        [TestMethod]
        public void TryGetEntry_ValidId_ValidEntry()
        {
            var onlineDb = new OnlineDatabase();
            Entry e;
            var res = onlineDb.TryGetEntry("Movie-218", out e);
            var movie = e as Movie;

            Assert.IsTrue(res);
            Assert.IsNotNull(e);
            Assert.AreEqual("Movie-218", e.Id);

            Assert.IsNotNull(movie);
            Assert.AreEqual("The Terminator", movie.Title);
        }

        [TestMethod]
        public void GetSearchResults_ValidQuery_ResultsNotEmpty()
        {
            var onlineDb = new OnlineDatabase();
            var results = onlineDb.GetSearchResults<Movie>("terminator");

            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void GetSearchResults_PreciseEntryQuery_CorrectEntryAsFirstResult()
        {
            var onlineDb = new OnlineDatabase();
            var results = onlineDb.GetSearchResults<Movie>("The Terminator");
            var bestResult = results.Dequeue();

            Assert.AreEqual("The Terminator", bestResult.Title);
            Assert.AreEqual("Movie-218", bestResult.Id);
        }

        [TestMethod]
        public void GetSearchResults_EmptyQuery_NoResult()
        {
            var onlineDb = new OnlineDatabase();
            var results = onlineDb.GetSearchResults<Movie>(string.Empty);

            Assert.IsFalse(results.Any());
        }
    }
}
