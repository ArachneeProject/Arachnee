using System;
using Assets.Classes.EntryProviders.OnlineDatabase;
using Assets.Classes.GraphElements;
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
            Assert.Fail();
        }

        [TestMethod]
        public void GetEntry_ValidArtistId_ReturnsValidArtist()
        {
            var tmdbProxy = new TmdbProxy();
            var artist = tmdbProxy.GetEntry("Artist-1100") as Artist;

            Assert.IsNotNull(artist);
            Assert.Fail();
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
    }
}