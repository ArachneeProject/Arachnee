using Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients.Builders.FromMedia;
using Assets.Classes.GraphElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArachneeTests
{
    [TestClass]
    public class Tests_MovieBuilder
    {
        [TestMethod]
        public void TryToBuild_ValidId_ValidMovie()
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

        [TestMethod]
        public void TryToBuild_InvalidId_DefaultEntry()
        {
            var builder = new MovieBuilder();

            var success = builder.TryToBuild("invalid", out var e);

            Assert.IsFalse(success);
            Assert.AreEqual(DefaultEntry.Instance, e);
        }
    }
}