using System;
using System.Collections.Generic;
using Assets.Classes.Core.Graph;
using Assets.Classes.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arachnee.Tests.Test_CompactGraph
{
    [TestClass]
    public class Tests_UndirectedUnweightedGraph
    {
        [TestInitialize]
        public void SetUp()
        {
            Logger.Mode = LogMode.SystemConsole;
        }

        [TestMethod]
        public void AddVerticesAndEdge_ValidEdge_ReturnsTrue()
        {
            var graph = new UndirectedUnweightedGraph<string>();

            var edge = new Tuple<string, string>("Arnold Schwarzenegger", "The Terminator");

            var added = graph.AddVerticesAndEdgeRange(new[] { edge });

            Assert.IsTrue(added);
        }

        [TestMethod]
        public void AddVerticesAndEdge_SelfEdge_ReturnsFalse()
        {
            var graph = new UndirectedUnweightedGraph<string>();

            var edge = new Tuple<string, string>("Arnold Schwarzenegger", "Arnold Schwarzenegger");

            var added = graph.AddVerticesAndEdgeRange(new[] { edge });

            Assert.IsFalse(added);
        }

        [TestMethod]
        public void AddVerticesAndEdge_TwiceTheSameEdgeInstance_ReturnsFalse()
        {
            var graph = new UndirectedUnweightedGraph<string>();

            var edge = new Tuple<string, string>("Arnold Schwarzenegger", "The Terminator");

            graph.AddVerticesAndEdgeRange(new[] { edge });
            var added = graph.AddVerticesAndEdgeRange(new[] { edge });

            Assert.IsFalse(added);
        }

        [TestMethod]
        public void AddVerticesAndEdge_TwoEdgesWithSameData_ReturnsFalse()
        {
            var graph = new UndirectedUnweightedGraph<string>();

            var edge1 = new Tuple<string, string>("Arnold Schwarzenegger", "The Terminator");
            var edge2 = new Tuple<string, string>("Arnold Schwarzenegger", "The Terminator");

            graph.AddVerticesAndEdgeRange(new[] { edge1 });
            var added = graph.AddVerticesAndEdgeRange(new[] { edge2 });

            Assert.IsFalse(added);
        }


        [TestMethod]
        public void GetShortestPath_ShortestPathExist_CorrectPath()
        {
            var graph = new UndirectedUnweightedGraph<int>();
            graph.AddVerticesAndEdgeRange(new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1, 2),
                new Tuple<int, int>(2, 3),
                new Tuple<int, int>(3, 4)
            });

            var res = graph.GetShortestPath(1, 4);

            Assert.AreEqual(3, res.Count);
            Assert.AreEqual(2, res[0]);
            Assert.AreEqual(3, res[1]);
            Assert.AreEqual(4, res[2]);
        }

        [TestMethod]
        public void GetShortestPath_ShortestPathBetweenOneSingleVertex_EmptyPath()
        {
            var graph = new UndirectedUnweightedGraph<int>();
            graph.AddVertex(0);

            var res = graph.GetShortestPath(0, 0);

            Assert.AreEqual(0, res.Count);
        }

        [TestMethod]
        public void GetShortestPath_ShortestPathDoesntExist_EmptyPath()
        {
            var graph = new UndirectedUnweightedGraph<int>();
            graph.AddVertex(0);
            graph.AddVertex(1);

            var res = graph.GetShortestPath(0, 1);

            Assert.AreEqual(0, res.Count);
        }

        [TestMethod]
        public void GetShortestPath_VerticesDontExist_EmptyPath()
        {
            var graph = new UndirectedUnweightedGraph<int>();
            graph.AddVertex(0);
            graph.AddVertex(1);

            var res = graph.GetShortestPath(2, 4);

            Assert.AreEqual(0, res.Count);
        }
    }
}