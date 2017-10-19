using System.Linq;
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

        [TestMethod]
        public void GetPerson_ValidId_CorrectPerson()
        {
            var client = new TmdbClient();

            var tmdbPerson = client.GetPerson("1100");

            Assert.IsNotNull(tmdbPerson);

            Assert.IsFalse(tmdbPerson.Adult);
            Assert.IsTrue(tmdbPerson.AlsoKnownAs.Count > 0);
            Assert.IsTrue(tmdbPerson.Biography.StartsWith("Arnold Alois Schwarzenegger (born July 30, 1947) is an Austrian-American former professional bodybuilder, " +
                                                      "actor, model, businessman and politician who served as the 38th Governor of California (2003–2011)."));
            Assert.AreEqual("1947-07-30", tmdbPerson.Birthday);
            Assert.IsNotNull(tmdbPerson.CombinedCredits);
            Assert.IsNull(tmdbPerson.Deathday);
            Assert.AreEqual(2, tmdbPerson.Gender);
            Assert.AreEqual("http://www.schwarzenegger.com/", tmdbPerson.Homepage);
            Assert.AreEqual(1100, tmdbPerson.Id);
            Assert.AreEqual("nm0000216", tmdbPerson.ImdbId);
            Assert.AreEqual("Arnold Schwarzenegger", tmdbPerson.Name);
            Assert.AreEqual("Thal, Styria, Austria", tmdbPerson.PlaceOfBirth);
            Assert.IsTrue(tmdbPerson.Popularity > 0);
            Assert.AreEqual("/sOkCXc9xuSr6v7mdAq9LwEBje68.jpg", tmdbPerson.ProfilePath);
        }

        [TestMethod]
        public void GetPerson_InvalidId_ThrowsInvalidTmdbRequestException()
        {
            var client = new TmdbClient();

            Assert.ThrowsException<InvalidTmdbRequestException>(() => client.GetPerson("invalid"));
        }

        [TestMethod]
        public void GetAllJobs_ListIsNotEmpty()
        {
            var client = new TmdbClient();
            var jobs = client.GetAllJobs().OrderBy(t => t).ToList();

            Assert.IsTrue(jobs.Count > 400);
        }
    }
}