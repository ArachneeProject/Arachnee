using System;
using System.Linq;
using System.Security.Cryptography;
using Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb;
using Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arachnee.Tests.Tests_OnlineDatabaseProvider.Tests_Tmdb
{
    [TestClass]
    public class Tests_TmdbClient
    {
        #region Movie

        [TestMethod]
        public void GetMovie_ValidId_ReturnsCorrectMovie()
        {
            var client = new TmdbClient();

            var tmdbMovie = client.GetMovie(218);

            Assert.IsNotNull(tmdbMovie);

            Assert.IsFalse(tmdbMovie.Adult);
            Assert.AreEqual("/6yFoLNQgFdVbA8TZMdfgVpszOla.jpg", tmdbMovie.BackdropPath);
            Assert.IsNotNull(tmdbMovie.BelongsToCollection);
            Assert.AreEqual(6400000, tmdbMovie.Budget);
            Assert.IsNotNull(tmdbMovie.Credits);
            Assert.IsTrue(tmdbMovie.Genres.Count > 0);
            Assert.AreEqual("http://www.mgm.com/#/our-titles/1970/The-Terminator/", tmdbMovie.Homepage);
            Assert.AreEqual(218UL, tmdbMovie.Id);
            Assert.IsTrue(tmdbMovie.Images.Posters.Any());
            Assert.IsTrue(tmdbMovie.Images.Backdrops.Any());
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
            Assert.ThrowsException<TmdbRequestFailedException>(() => client.GetMovie(0));
        }
        
        #endregion Movie

        #region Person

        [TestMethod]
        public void GetPerson_ValidId_ReturnsCorrectPerson()
        {
            var client = new TmdbClient();

            var tmdbPerson = client.GetPerson(1100);

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
            Assert.AreEqual(1100UL, tmdbPerson.Id);
            Assert.IsTrue(tmdbPerson.Images.Profiles.Any());
            Assert.AreEqual("nm0000216", tmdbPerson.ImdbId);
            Assert.AreEqual("Arnold Schwarzenegger", tmdbPerson.Name);
            Assert.AreEqual("Thal, Styria, Austria", tmdbPerson.PlaceOfBirth);
            Assert.IsTrue(tmdbPerson.Popularity > 0);
            Assert.IsFalse(string.IsNullOrEmpty(tmdbPerson.ProfilePath));
        }
        
        [TestMethod]
        public void GetPerson_InvalidId_ThrowsInvalidTmdbRequestException()
        {
            var client = new TmdbClient();
            Assert.ThrowsException<TmdbRequestFailedException>(() => client.GetPerson(0));
        }

        #endregion Person

        #region CombinedSearch

        [TestMethod]
        public void GetCombinedSearchResults_ValidQuery_ReturnsCorrectResults()
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

            Assert.AreEqual(9404UL, movieResult.Id);
            Assert.AreEqual(18897UL, personResult.Id);
            Assert.AreEqual(240UL, tvResult.Id);

            Assert.IsFalse(string.IsNullOrEmpty(movieResult.PosterPath));
            Assert.IsFalse(string.IsNullOrEmpty(personResult.ProfilePath));
            Assert.IsFalse(string.IsNullOrEmpty(tvResult.PosterPath));
        }

        [TestMethod]
        public void GetCombinedSearchResults_EmptyQuery_ReturnsEmptyResult()
        {
            var client = new TmdbClient();
            var results = client.GetCombinedSearchResults("");

            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void GetCombinedSearchResults_Null_ReturnsEmptyResult()
        {
            var client = new TmdbClient();
            var results = client.GetCombinedSearchResults(null);

            Assert.AreEqual(0, results.Count);
        }

        #endregion CombinedSearch

        [TestMethod,
         Description("Official Tmdb job list may increase over time, but will still contains at least 400 items.")]
        public void GetAllJobs_ReturnsJobList()
        {
            var client = new TmdbClient();
            var jobs = client.GetAllJobs().OrderBy(t => t).ToList();

            Assert.IsTrue(jobs.Count > 400);
        }

        [TestMethod]
        public void GetConfiguration_ReturnsValidConfiguration()
        {
            var client = new TmdbClient();
            var configuration = client.GetConfiguration();

            Assert.IsNotNull(configuration);
            Assert.IsTrue(configuration.ChangeKeys.Any());
            Assert.AreEqual("http://image.tmdb.org/t/p/", configuration.Images.BaseUrl);
            Assert.AreEqual("https://image.tmdb.org/t/p/", configuration.Images.SecureBaseUrl);

            Assert.IsTrue(configuration.Images.BackdropSizes.Any());
            Assert.IsTrue(configuration.Images.BackdropSizes.Contains("w300"));
            Assert.IsTrue(configuration.Images.BackdropSizes.Contains("w780"));
            Assert.IsTrue(configuration.Images.BackdropSizes.Contains("w1280"));
            Assert.IsTrue(configuration.Images.BackdropSizes.Contains("original"));

            Assert.IsTrue(configuration.Images.LogoSizes.Any());
            Assert.IsTrue(configuration.Images.LogoSizes.Contains("w45"));
            Assert.IsTrue(configuration.Images.LogoSizes.Contains("w185"));
            Assert.IsTrue(configuration.Images.LogoSizes.Contains("w500"));
            Assert.IsTrue(configuration.Images.LogoSizes.Contains("original"));

            Assert.IsTrue(configuration.Images.PosterSizes.Any());
            Assert.IsTrue(configuration.Images.PosterSizes.Contains("w92"));
            Assert.IsTrue(configuration.Images.PosterSizes.Contains("w185"));
            Assert.IsTrue(configuration.Images.PosterSizes.Contains("w780"));
            Assert.IsTrue(configuration.Images.PosterSizes.Contains("original"));

            Assert.IsTrue(configuration.Images.ProfileSizes.Any());
            Assert.IsTrue(configuration.Images.ProfileSizes.Contains("w45"));
            Assert.IsTrue(configuration.Images.ProfileSizes.Contains("w185"));
            Assert.IsTrue(configuration.Images.ProfileSizes.Contains("h632"));
            Assert.IsTrue(configuration.Images.ProfileSizes.Contains("original"));

            Assert.IsTrue(configuration.Images.StillSizes.Any());
            Assert.IsTrue(configuration.Images.StillSizes.Contains("w92"));
            Assert.IsTrue(configuration.Images.StillSizes.Contains("w185"));
            Assert.IsTrue(configuration.Images.StillSizes.Contains("w300"));
            Assert.IsTrue(configuration.Images.StillSizes.Contains("original"));
        }

        #region Image

        [TestMethod]
        public void GetImage_NullFileSize_ThrowsArgumentNullException()
        {
            var client = new TmdbClient();
            Assert.ThrowsException<ArgumentNullException>(() => client.GetImage(null, "/wOMD2jDmY6wU2oScXOEgq9hqeNN.png"));
        }

        [TestMethod]
        public void GetImage_EmptyFileSize_ThrowsArgumentNullException()
        {
            var client = new TmdbClient();
            Assert.ThrowsException<ArgumentNullException>(() => client.GetImage("", "/wOMD2jDmY6wU2oScXOEgq9hqeNN.png"));
        }

        [TestMethod]
        public void GetImage_InvalidFileSize_ThrowsTmdbRequestFailedException()
        {
            var client = new TmdbClient();
            Assert.ThrowsException<TmdbRequestFailedException>(() => client.GetImage("#invalid#", "/wOMD2jDmY6wU2oScXOEgq9hqeNN.png"));
        }

        [TestMethod]
        public void GetImage_NullFilePath_ThrowsArgumentNullException()
        {
            var client = new TmdbClient();
            Assert.ThrowsException<ArgumentNullException>(() => client.GetImage("w45", null));
        }

        [TestMethod]
        public void GetImage_EmptyFilePath_ThrowsArgumentNullException()
        {
            var client = new TmdbClient();
            Assert.ThrowsException<ArgumentNullException>(() => client.GetImage("w45", ""));
        }

        [TestMethod]
        public void GetImage_InvalidFilePath_ThrowsTmdbRequestFailedException()
        {
            var client = new TmdbClient();
            Assert.ThrowsException<TmdbRequestFailedException>(() => client.GetImage("w45", "#invalid#"));
        }
        
        [TestMethod]
        public void GetImage_ValidParameters_ReturnsValidByteArray()
        {
            var client = new TmdbClient();
            var bytes = client.GetImage("w45", "/wOMD2jDmY6wU2oScXOEgq9hqeNN.png");
            
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(bytes);
            }

            Assert.IsTrue(bytes.Length > 10);
            Assert.AreEqual((byte)'P', bytes[1]);
            Assert.AreEqual((byte)'N', bytes[2]);
            Assert.AreEqual((byte)'G', bytes[3]);
        }

        #endregion Image
    }
}