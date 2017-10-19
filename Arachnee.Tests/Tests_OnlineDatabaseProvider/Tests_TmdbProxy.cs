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
        public void TryToBuild_InvalidMovieId_ReturnsFalseAndDefaultEntry()
        {
            var tmdbProxy = new TmdbProxy();

            Entry entry;
            var success = tmdbProxy.TryToBuild("Movie-invalid", out entry);

            Assert.IsFalse(success);
            Assert.AreEqual(DefaultEntry.Instance, entry);
        }

        [TestMethod]
        public void TryToBuild_InvalidArtistId_ReturnsFalseAndDefaultEntry()
        {
            var tmdbProxy = new TmdbProxy();

            Entry entry;
            var success = tmdbProxy.TryToBuild("Artist-invalid", out entry);

            Assert.IsFalse(success);
            Assert.AreEqual(DefaultEntry.Instance, entry);
        }

        [TestMethod]
        public void TryToBuild_InvalidEntryType_ReturnsFalseAndDefaultEntry()
        {
            var tmdbProxy = new TmdbProxy();

            Entry entry;
            var success = tmdbProxy.TryToBuild("Invalid-1100", out entry);

            Assert.IsFalse(success);
            Assert.AreEqual(DefaultEntry.Instance, entry);
        }
    }
}