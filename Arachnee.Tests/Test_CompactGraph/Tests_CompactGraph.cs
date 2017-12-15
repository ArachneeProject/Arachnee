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
        public void AddVerticesAndEdge_ValidEdge_ReturnsTrue()
        {
            var compactGraph = new CompactGraph();

            var edge = new CompactEdge("Arnold Schwarzenegger", "The Terminator", ConnectionType.Actor);

            var added = compactGraph.AddVerticesAndEdge(edge);

            Assert.IsTrue(added);
        }

        [TestMethod]
        public void AddVerticesAndEdge_TwiceTheSameEdgeInstance_ReturnsFalse()
        {
            var compactGraph = new CompactGraph();

            var edge = new CompactEdge("Arnold Schwarzenegger", "The Terminator", ConnectionType.Actor);

            compactGraph.AddVerticesAndEdge(edge);
            var added = compactGraph.AddVerticesAndEdge(edge);

            Assert.IsFalse(added);
        }

        [TestMethod]
        public void AddVerticesAndEdge_TwoEdgesWithSameData_ReturnsFalse()
        {
            var compactGraph = new CompactGraph();

            var edge1 = new CompactEdge("Arnold Schwarzenegger", "The Terminator", ConnectionType.Actor);
            var edge2 = new CompactEdge("Arnold Schwarzenegger", "The Terminator", ConnectionType.Actor);

            compactGraph.AddVerticesAndEdge(edge1);
            var added = compactGraph.AddVerticesAndEdge(edge2);

            Assert.IsFalse(added);
        }

        [TestMethod]
        public void AddVerticesAndEdge_TwoEdgesWithDifferentType_ReturnsTrue()
        {
            var compactGraph = new CompactGraph();

            var edge1 = new CompactEdge("Luc Besson", "Le Grand Bleu", ConnectionType.Director);
            var edge2 = new CompactEdge("Luc Besson", "Le Grand Bleu", ConnectionType.Actor);

            compactGraph.AddVerticesAndEdge(edge1);
            var added = compactGraph.AddVerticesAndEdge(edge2);

            Assert.IsTrue(added);
        }

        [TestMethod]
        public void Initialize_ValidSerializedGraph_ValidCompactedGraph()
        {
            var compactGraph = new CompactGraph();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "temp.txt");
            File.WriteAllLines(filePath, _validSerializedGraph);

            compactGraph.InitializeFrom(filePath);
            
            Assert.AreEqual(4, compactGraph.VertexCount);
            Assert.AreEqual(6, compactGraph.EdgeCount);

            Assert.IsTrue(compactGraph.ContainsEdge("a14B1B2", "m5B343"));
            Assert.IsTrue(compactGraph.ContainsEdge("sEF2F", "a14B1B2"));

            File.Delete(filePath);
        }

        /*
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
        */
    }
}