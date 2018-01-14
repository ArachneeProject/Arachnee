using System;
using System.IO;
using Assets.Classes.Core.Graph;
using Assets.Classes.Core.Models;
using Assets.Classes.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arachnee.Tests.Test_CompactGraph
{
    [TestClass]
    public class Tests_CompactGraph
    {
        private readonly string[] _validSerializedGraph =
        {
            "a14B1B2:0_m5B343,sEF2F;1_mDA;"
        };
        
        [TestInitialize]
        public void SetUp()
        {
            Logger.Mode = LogMode.SystemConsole;
        }
        
        [TestMethod]
        public void InitializeFrom_ValidSerializedGraph_ValidCompactGraph()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "temp.txt");
            File.WriteAllLines(filePath, _validSerializedGraph);

            var compactGraph = CompactGraph.InitializeFrom(filePath, Connection.AllTypes());
            
            Assert.AreEqual(4, compactGraph.VertexCount);
            Assert.AreEqual(3, compactGraph.EdgeCount);

            Assert.IsTrue(compactGraph.ContainsEdge("a14B1B2", "m5B343"));
            Assert.IsTrue(compactGraph.ContainsEdge("sEF2F", "a14B1B2"));

            File.Delete(filePath);
        }

        [TestMethod]
        public void GetShortestPath_ShortestPathExist_CorrectPath()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "temp.txt");
            File.WriteAllLines(filePath, _validSerializedGraph);

            var compactGraph = CompactGraph.InitializeFrom(filePath, Connection.AllTypes());

            var res = compactGraph.GetShortestPath("Artist-1356210", "Movie-218");

            Assert.AreEqual(2, res.Count);
            Assert.AreEqual("Artist-1356210", res[0]);
            Assert.AreEqual("Movie-218", res[1]);

            File.Delete(filePath);
        }
    }
}