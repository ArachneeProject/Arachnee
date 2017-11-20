using Assets.Classes.Core.Serialization;
using Assets.Classes.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arachnee.Tests.CompactGraph
{
    [TestClass]
    public class Tests_CompactGraph
    {
        [TestInitialize]
        public void SetUp()
        {
            Logger.Mode = LogMode.SystemConsole;
        }

        //TODO: improve tests

        [TestMethod]
        public void ShortestPath()
        {
            var deserializer = new GraphDeserializer();
            var graph = deserializer.LoadFrom(@"(((( dump file path ))))");

            var res = graph.GetShortestPath("Artist-1100", "Artist-2710");

            Assert.AreEqual(3, res.Count);
            Assert.AreEqual("Artist-1100", res[0]);
            Assert.AreEqual("Movie-218", res[1]);
            Assert.AreEqual("Artist-2710", res[2]);
        }
    }
}