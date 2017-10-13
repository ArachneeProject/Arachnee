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

        [TestMethod]
        public void TryDeserializeMovie_ValidId_ValidMovie()
        {
            var builder = new MovieBuilder();
            Entry e;

            var success = builder.TryToBuild("218", out e);
            var movie = e as Movie;

            Assert.IsTrue(success);
            Assert.IsNotNull(movie);

            Assert.AreEqual("/6yFoLNQgFdVbA8TZMdfgVpszOla.jpg", movie.BackdropPath);
            Assert.AreEqual(6400000, movie.Budget);
            Assert.AreEqual("http://www.mgm.com/#/our-titles/1970/The-Terminator/", movie.Homepage);
            Assert.AreEqual("http://www.mgm.com/#/our-titles/1970/The-Terminator/", movie.Homepage);
            Assert.AreEqual("Movie-218", movie.Id);
            Assert.AreEqual("tt0088247", movie.ImdbId);
            Assert.AreEqual("en", movie.OriginalLanguage);
            Assert.AreEqual("The Terminator", movie.OriginalTitle);
            Assert.AreEqual("In the post-apocalyptic future, reigning tyrannical supercomputers teleport " +
                            "a cyborg assassin known as the \"Terminator\" back to 1984 to kill Sarah Connor, " +
                            "whose unborn son is destined to lead insurgents against 21st century mechanical hegemony. " +
                            "Meanwhile, the human-resistance movement dispatches a lone warrior to safeguard Sarah. " +
                            "Can he stop the virtually indestructible killing machine?", movie.Overview);
            Assert.IsTrue(movie.Popularity > 0);
            Assert.AreEqual("/q8ffBuxQlYOHrvPniLgCbmKK4Lv.jpg", movie.PosterPath);
            Assert.AreEqual("1984-10-26", movie.ReleaseDate);
            Assert.AreEqual(78371200, movie.Revenue);
            Assert.AreEqual(108, movie.Runtime);
            Assert.AreEqual("Released", movie.Status);
            Assert.AreEqual("Your future is in his hands.", movie.Tagline);
            Assert.AreEqual("The Terminator", movie.Title);
            Assert.IsTrue(movie.VoteAverage > 0);
            Assert.IsTrue(movie.VoteCount > 0);
        }
    }
}
