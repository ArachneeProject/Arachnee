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
            "MDA:0_A44C,AA98,AA99,A71A,AA9A,AA9B,AA9C,AA9D,A805,AA9F,A8B7E,A19029,A1507A4,AA97,A19333,AD61E,A21F,A163A51;1_AA96;3_AA97,A365,AAA1,AAA3,AA96,A365,A150797,AA96,A163A51,A3DA7,AF18E;"
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

            // Serialized graph actually has 30 edges
            // However James Cameron (AA96) is Director, Writer and Script Supervisor, making him appearing twice as Crew.
            // Same for Gale Anne Hurd
            // Final graph only has 28 egdes
            Assert.AreEqual(26, compactGraph.VertexCount);
            Assert.AreEqual(28, compactGraph.EdgeCount);

            Assert.IsTrue(compactGraph.ContainsEdge("MDA", "AA96"));
            Assert.IsTrue(compactGraph.ContainsEdge("MDA", "A44C"));

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