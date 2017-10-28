using System.Linq;
using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arachnee.Tests.Tests_OnlineDatabaseProvider
{
    [TestClass]
    public class Tests_OnlineDatabase
    {
        [TestMethod]
        public void TryGetEntry_ValidMovieId_ReturnsTrueAndValidEntry()
        {
            var onlineDb = new OnlineDatabase();

            Entry e;
            var success = onlineDb.TryGetEntry("Movie-218", out e);
            var movie = e as Movie;

            Assert.IsTrue(success);
            Assert.IsNotNull(movie);
            Assert.AreEqual("Movie-218", movie.Id);
            Assert.AreEqual("The Terminator", movie.Title);
            Assert.AreEqual("/q8ffBuxQlYOHrvPniLgCbmKK4Lv.jpg", movie.PosterPath);
        }

        [TestMethod]
        public void TryGetEntry_ValidArtistId_ReturnsTrueAndValidEntry()
        {
            var onlineDb = new OnlineDatabase();

            Entry e;
            var success = onlineDb.TryGetEntry("Artist-1100", out e);
            var artist = e as Artist;

            Assert.IsTrue(success);
            Assert.IsNotNull(artist);
            Assert.AreEqual("Artist-1100", artist.Id);
            Assert.AreEqual("Arnold Schwarzenegger", artist.Name);
            Assert.AreEqual("/sOkCXc9xuSr6v7mdAq9LwEBje68.jpg", artist.ProfilePath);
        }

        [TestMethod]
        public void GetSearchResults_ValidQuery_ReturnsCorrectBestResult()
        {
            var onlineDb = new OnlineDatabase();
            var results = onlineDb.GetSearchResults("Jackie Chan");
            var bestResult = results.Dequeue();

            Assert.IsTrue(results.Any());
            Assert.AreEqual("Jackie Chan Adventures", bestResult.Name);
            Assert.AreEqual("Serie-240", bestResult.EntryId);
            Assert.AreEqual("/6bsg03VVkB41Vzs6w1NvpFvq2yH.jpg", bestResult.ImagePath);
        }

        [TestMethod]
        public void GetSearchResults_EmptyQuery_EmptyResults()
        {
            var onlineDb = new OnlineDatabase();
            var results = onlineDb.GetSearchResults(string.Empty);

            Assert.IsFalse(results.Any());
        }

        [TestMethod]
        public void GetSearchResults_Null_EmptyResults()
        {
            var onlineDb = new OnlineDatabase();
            var results = onlineDb.GetSearchResults(null);

            Assert.IsFalse(results.Any());
        }
    }
}
