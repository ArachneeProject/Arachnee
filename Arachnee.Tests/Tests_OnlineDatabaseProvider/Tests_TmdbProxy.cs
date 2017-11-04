using System;
using System.IO;
using System.Linq;
using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb;
using Assets.Classes.Core.Models;
using Assets.Classes.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arachnee.Tests.Tests_OnlineDatabaseProvider
{
    [TestClass]
    public class Tests_TmdbProxy
    {
        [TestInitialize]
        public void SetUp()
        {
            Logger.Mode = LogMode.SystemConsole;
        }

        [TestMethod]
        public void GetEntry_ValidMovieId_ReturnsValidMovie()
        {
            var tmdbProxy = new TmdbProxy();
            var movie = tmdbProxy.GetEntry("Movie-218") as Movie;

            Assert.IsNotNull(movie);
            
            Assert.AreEqual("Movie-218", movie.Id);
            Assert.AreEqual("The Terminator", movie.Title);
            
            Assert.AreEqual("/6yFoLNQgFdVbA8TZMdfgVpszOla.jpg", movie.BackdropPath);
            Assert.AreEqual(6400000, movie.Budget);
            Assert.AreEqual(3, movie.Tags.Count);
            Assert.AreEqual("http://www.mgm.com/#/our-titles/1970/The-Terminator/", movie.Homepage);
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
            Assert.IsTrue(movie.VoteAverage > 0);
            Assert.IsTrue(movie.VoteCount > 0);
        }

        [TestMethod]
        public void GetEntry_ValidMovieId_ReturnsMovieWithValidConnections()
        {
            var tmdbProxy = new TmdbProxy();
            var movie = (Movie)tmdbProxy.GetEntry("Movie-218");

            var arnoldConnection = movie.Connections.FirstOrDefault(c =>
                c.ConnectedId == "Artist-1100" && c.Type == ConnectionType.Actor);

            var cameronConnection = movie.Connections.FirstOrDefault(c =>
                c.ConnectedId == "Artist-2710" && c.Type == ConnectionType.Director);

            Assert.IsNotNull(arnoldConnection);
            Assert.AreEqual("The Terminator", arnoldConnection.Label);
            
            Assert.IsNotNull(cameronConnection);
            Assert.AreEqual("Director", cameronConnection.Label);
        }

        [TestMethod]
        public void GetEntry_ValidArtistId_ReturnsValidArtist()
        {
            var tmdbProxy = new TmdbProxy();
            var artist = tmdbProxy.GetEntry("Artist-1100") as Artist;

            Assert.IsNotNull(artist);

            Assert.AreEqual("Artist-1100", artist.Id);
            Assert.AreEqual("Arnold Schwarzenegger", artist.Name);

            Assert.IsTrue(artist.Biography.StartsWith(
                "Arnold Alois Schwarzenegger (born July 30, 1947) is an Austrian-American former professional bodybuilder, " +
                "actor, model, businessman and politician who served as the 38th Governor of California (2003–2011)."));
            Assert.AreEqual("1947-07-30", artist.Birthday);
            Assert.IsNull(artist.Deathday);
            Assert.AreEqual(2, artist.Gender);
            Assert.AreEqual("http://www.schwarzenegger.com/", artist.Homepage);
            Assert.AreEqual("nm0000216", artist.ImdbId);
            Assert.IsTrue(artist.NickNames.Count > 3);
            Assert.AreEqual("Thal, Styria, Austria", artist.PlaceOfBirth);
            Assert.IsTrue(artist.Popularity > 0);
            Assert.AreEqual("/sOkCXc9xuSr6v7mdAq9LwEBje68.jpg", artist.ProfilePath);
        }

        [TestMethod]
        public void GetEntry_ValidArtistId_ReturnsArtistWithValidConnections()
        {
            var tmdbProxy = new TmdbProxy();
            var artist = (Artist) tmdbProxy.GetEntry("Artist-1100");

            var terminatorConnection = artist.Connections.FirstOrDefault(c =>
                c.ConnectedId == "Movie-218" && c.Type == ConnectionType.Actor);

            Assert.IsNotNull(terminatorConnection);
            Assert.AreEqual("The Terminator", terminatorConnection.Label);
        }

        [TestMethod]
        public void GetEntry_InvalidEntryTypeSegment_ThrowsArgumentException()
        {
            var tmdbProxy = new TmdbProxy();

            Assert.ThrowsException<ArgumentException>(() => tmdbProxy.GetEntry("#Invalid#-1100"));
        }

        [TestMethod]
        public void GetEntry_EmptyEntryTypeSegment_ThrowsArgumentException()
        {
            var tmdbProxy = new TmdbProxy();

            Assert.ThrowsException<ArgumentException>(() => tmdbProxy.GetEntry("-1100"));
        }

        [TestMethod]
        public void GetEntry_InvalidIdSegment_ThrowsArgumentException()
        {
            var tmdbProxy = new TmdbProxy();

            Assert.ThrowsException<ArgumentException>(() => tmdbProxy.GetEntry("Movie-#invalid#"));
        }

        [TestMethod]
        public void GetEntry_EmptyIdSegment_ThrowsArgumentException()
        {
            var tmdbProxy = new TmdbProxy();

            Assert.ThrowsException<ArgumentException>(() => tmdbProxy.GetEntry("Movie-"));
        }

        [TestMethod]
        public void GetEntry_TooManySegments_ThrowsArgumentException()
        {
            var tmdbProxy = new TmdbProxy();

            Assert.ThrowsException<ArgumentException>(() => tmdbProxy.GetEntry("Movie-1100-1"));
        }

        [TestMethod]
        public void GetEntry_InvalidId_ThrowsArgumentException()
        {
            var tmdbProxy = new TmdbProxy();

            Assert.ThrowsException<ArgumentException>(() => tmdbProxy.GetEntry("#invalid#"));
        }

        [TestMethod]
        public void GetEntry_Null_ThrowsArgumentException()
        {
            var tmdbProxy = new TmdbProxy();

            Assert.ThrowsException<ArgumentException>(() => tmdbProxy.GetEntry(null));
        }

        [TestMethod]
        public void GetSearchResults_ValidQuery_ReturnsValidResults()
        {
            var tmdbProxy = new TmdbProxy();
            var results = tmdbProxy.GetSearchResults("Jackie Chan");

            Assert.AreEqual(19, results.Count);

            var movieResult = results.FirstOrDefault(r => r.Name == "First Strike");
            var personResult = results.FirstOrDefault(r => r.Name == "Jackie Chan");
            var tvResult = results.FirstOrDefault(r => r.Name == "Jackie Chan Adventures");

            Assert.IsNotNull(movieResult);
            Assert.IsNotNull(personResult);
            Assert.IsNotNull(tvResult);

            Assert.AreEqual("Movie-9404", movieResult.EntryId);
            Assert.AreEqual("Artist-18897", personResult.EntryId);
            Assert.AreEqual("Serie-240", tvResult.EntryId);
            
            Assert.AreEqual("/9i6bhYbxe2g02e3GhljtktuyDMj.jpg", movieResult.ImagePath);
            Assert.AreEqual("/pmKJ4sGvPQ3imzXaFnjW4Vk5Gyc.jpg", personResult.ImagePath);
            Assert.AreEqual("/6bsg03VVkB41Vzs6w1NvpFvq2yH.jpg", tvResult.ImagePath);
        }
        
        [TestMethod]
        public void GetSearchResults_EmptyQuery_ReturnsEmptyResults()
        {
            var tmdbProxy = new TmdbProxy();
            var results = tmdbProxy.GetSearchResults("");

            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void GetSearchResults_Null_ReturnsEmptyResults()
        {
            var tmdbProxy = new TmdbProxy();
            var results = tmdbProxy.GetSearchResults(null);

            Assert.AreEqual(0, results.Count);
        }

    }
}