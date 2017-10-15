using Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients.Builders.FromMedia;
using Assets.Classes.GraphElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArachneeTests
{
    [TestClass]
    public class Tests_ArtistBuilder
    {
        [TestMethod]
        public void TryToBuild_ValidId_ValidArtist()
        {
            var builder = new ArtistBuilder();
            Entry e;

            var success = builder.TryToBuild("1100", out e);
            var artist = e as Artist;

            Assert.IsTrue(success);
            Assert.IsNotNull(artist);

            Assert.IsTrue(artist.Biography.StartsWith("Arnold Alois Schwarzenegger (born July 30, 1947) is an Austrian-American former professional bodybuilder, " +
                                                      "actor, model, businessman and politician who served as the 38th Governor of California (2003–2011)."));
            Assert.AreEqual("1947-07-30", artist.Birthday);
            Assert.AreEqual(null, artist.Deathday);
            Assert.AreEqual(2, artist.Gender);
            Assert.AreEqual("http://www.schwarzenegger.com/", artist.Homepage);
            Assert.AreEqual("Artist-1100", artist.Id);
            Assert.AreEqual("nm0000216", artist.ImdbId);
            Assert.AreEqual("Arnold Schwarzenegger", artist.Name);
            Assert.AreEqual("Thal, Styria, Austria", artist.PlaceOfBirth);
            Assert.IsTrue(artist.Popularity > 0);
            Assert.AreEqual("/sOkCXc9xuSr6v7mdAq9LwEBje68.jpg", artist.ProfilePath);
        }

        [TestMethod]
        public void TryToBuild_InvalidId_DefaultEntry()
        {
            var builder = new ArtistBuilder();

            var success = builder.TryToBuild("invalid", out var e);
            
            Assert.IsFalse(success);
            Assert.AreEqual(DefaultEntry.Instance, e);
        }
    }
}