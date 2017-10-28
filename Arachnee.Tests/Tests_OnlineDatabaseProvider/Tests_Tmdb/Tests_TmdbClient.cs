using System.Linq;
using Assets.Classes.EntryProviders.OnlineDatabase.Tmdb;
using Assets.Classes.EntryProviders.OnlineDatabase.Tmdb.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arachnee.Tests.Tests_OnlineDatabaseProvider.Tests_Tmdb
{
    [TestClass]
    public class Tests_TmdbClient
    {
        #region Movie

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
            Assert.ThrowsException<InvalidTmdbRequestException>(() => client.GetMovie("-10"));
        }

        [TestMethod]
        public void GetMovie_NotAnId_ThrowsInvalidTmdbRequestException()
        {
            var client = new TmdbClient();
            Assert.ThrowsException<InvalidTmdbRequestException>(() => client.GetMovie("#invalid#"));
        }
        
        [TestMethod]
        public void GetMovie_EmptyId_ThrowsInvalidTmdbRequestException()
        {
            var client = new TmdbClient();
            Assert.ThrowsException<InvalidTmdbRequestException>(() => client.GetMovie(""));
        }

        [TestMethod]
        public void GetMovie_Null_ThrowsInvalidTmdbRequestException()
        {
            var client = new TmdbClient();
            Assert.ThrowsException<InvalidTmdbRequestException>(() => client.GetMovie(null));
        }

        #endregion Movie

        #region Person

        [TestMethod]
        public void GetPerson_ValidId_CorrectPerson()
        {
            var client = new TmdbClient();

            var tmdbPerson = client.GetPerson("1100");

            Assert.IsNotNull(tmdbPerson);

            Assert.IsFalse(tmdbPerson.Adult);
            Assert.IsTrue(tmdbPerson.AlsoKnownAs.Count > 0);
            Assert.IsTrue(tmdbPerson.Biography.StartsWith(
                "Arnold Alois Schwarzenegger (born July 30, 1947) is an Austrian-American former professional bodybuilder, " +
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

            Assert.ThrowsException<InvalidTmdbRequestException>(() => client.GetPerson("-10"));
        }

        [TestMethod]
        public void GetPerson_NotAnId_ThrowsInvalidTmdbRequestException()
        {
            var client = new TmdbClient();

            Assert.ThrowsException<InvalidTmdbRequestException>(() => client.GetPerson("#invalid#"));
        }
        
        [TestMethod]
        public void GetPerson_EmptyId_ThrowsInvalidTmdbRequestException()
        {
            var client = new TmdbClient();
            Assert.ThrowsException<InvalidTmdbRequestException>(() => client.GetPerson(""));
        }

        [TestMethod]
        public void GetPerson_Null_ThrowsInvalidTmdbRequestException()
        {
            var client = new TmdbClient();
            Assert.ThrowsException<InvalidTmdbRequestException>(() => client.GetPerson(null));
        }

        #endregion Person

        #region CombinedSearch

        [TestMethod]
        public void GetCombinedSearchResults_ValidQuery_CorrectResults()
        {
            var client = new TmdbClient();
            var results = client.GetCombinedSearchResults("Jackie Chan");
            var movieResult = results.FirstOrDefault(r => r.Title == "First Strike");
            var personResult = results.FirstOrDefault(r => r.Name == "Jackie Chan");
            var tvResult = results.FirstOrDefault(r => r.Name == "Jackie Chan Adventures");

            Assert.IsTrue(results.Count >= 20);

            Assert.IsNotNull(movieResult);
            Assert.IsNotNull(personResult);
            Assert.IsNotNull(tvResult);

            Assert.AreEqual("movie", movieResult.MediaType);
            Assert.AreEqual("person", personResult.MediaType);
            Assert.AreEqual("tv", tvResult.MediaType);

            Assert.AreEqual(9404, movieResult.Id);
            Assert.AreEqual(9502, personResult.Id);
            Assert.AreEqual(240, tvResult.Id);

            Assert.AreEqual("/9i6bhYbxe2g02e3GhljtktuyDMj.jpg", movieResult.PosterPath);
            Assert.AreEqual("/pmKJ4sGvPQ3imzXaFnjW4Vk5Gyc.jpg", personResult.ProfilePath);
            Assert.AreEqual("/6bsg03VVkB41Vzs6w1NvpFvq2yH.jpg", tvResult.PosterPath);
        }

        [TestMethod]
        public void GetCombinedSearchResults_EmptyQuery_EmptyResult()
        {
            var client = new TmdbClient();
            var results = client.GetCombinedSearchResults("");

            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void GetCombinedSearchResults_Null_EmptyResult()
        {
            var client = new TmdbClient();
            var results = client.GetCombinedSearchResults(null);

            Assert.AreEqual(0, results.Count);
        }

        #endregion CombinedSearch

        [TestMethod,
         Description("Official Tmdb job list can increase over time, but will still contains at least 400 items.")]
        public void GetAllJobs_ListHasMoreThan400Items()
        {
            var client = new TmdbClient();
            var jobs = client.GetAllJobs().OrderBy(t => t).ToList();

            Assert.IsTrue(jobs.Count > 400);
        }

    }
}