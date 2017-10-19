using Assets.Classes.EntryProviders.OnlineDatabase.Tmdb;
using Assets.Classes.EntryProviders.OnlineDatabase.Tmdb.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arachnee.Tests.Tests_OnlineDatabaseProvider.Tests_Tmdb
{
    [TestClass]
    public class Tests_TmdbClient
    {
        [TestMethod]
        public void GetMovie_ValidId_CorrectMovie()
        {
            var client = new TmdbClient();

            var tmdbMovie = client.GetMovie("218");
            
            Assert.IsNotNull(tmdbMovie);

            Assert.IsFalse(tmdbMovie.Adult);
            Assert.AreEqual("/6yFoLNQgFdVbA8TZMdfgVpszOla.jpg", tmdbMovie.BackdropPath);
            Assert.IsNotNull(tmdbMovie.BelongsToCollection);
            Assert.AreEqual(6400000, tmdbMovie.Budget);
            Assert.IsNotNull(tmdbMovie.Credits);
            Assert.IsTrue(tmdbMovie.Genres.Count > 0);
            Assert.AreEqual("http://www.mgm.com/#/our-titles/1970/The-Terminator/", tmdbMovie.Homepage);
            Assert.AreEqual(218, tmdbMovie.Id);
            Assert.AreEqual("tt0088247", tmdbMovie.ImdbId);
            Assert.AreEqual("en", tmdbMovie.OriginalLanguage);
            Assert.AreEqual("The Terminator", tmdbMovie.OriginalTitle);
            Assert.AreEqual("In the post-apocalyptic future, reigning tyrannical supercomputers teleport " +
                            "a cyborg assassin known as the \"Terminator\" back to 1984 to kill Sarah Connor, " +
                            "whose unborn son is destined to lead insurgents against 21st century mechanical hegemony. " +
                            "Meanwhile, the human-resistance movement dispatches a lone warrior to safeguard Sarah. " +
                            "Can he stop the virtually indestructible killing machine?", tmdbMovie.Overview);
            Assert.IsTrue(tmdbMovie.Popularity > 0);
            Assert.AreEqual("/q8ffBuxQlYOHrvPniLgCbmKK4Lv.jpg", tmdbMovie.PosterPath);
            Assert.IsTrue(tmdbMovie.ProductionCompanies.Count > 0);
            Assert.IsTrue(tmdbMovie.ProductionCountries.Count > 0);
            Assert.AreEqual("1984-10-26", tmdbMovie.ReleaseDate);
            Assert.AreEqual(78371200, tmdbMovie.Revenue);
            Assert.AreEqual(108, tmdbMovie.Runtime);
            Assert.IsTrue(tmdbMovie.SpokenLanguages.Count > 0);
            Assert.AreEqual("Released", tmdbMovie.Status);
            Assert.AreEqual("Your future is in his hands.", tmdbMovie.Tagline);
            Assert.AreEqual("The Terminator", tmdbMovie.Title);
            Assert.IsFalse(tmdbMovie.Video);
            Assert.IsTrue(tmdbMovie.VoteAverage > 0);
            Assert.IsTrue(tmdbMovie.VoteCount > 0);
        }

        [TestMethod]
        public void GetMovie_InvalidId_ThrowsInvalidTmdbRequestException()
        {
            var client = new TmdbClient();

            Assert.ThrowsException<InvalidTmdbRequestException>(() => client.GetMovie("invalid"));
            
        }
    }
}