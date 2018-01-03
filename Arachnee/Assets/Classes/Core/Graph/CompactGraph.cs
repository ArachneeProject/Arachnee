using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Assets.Classes.Core.Models;
using Assets.Classes.Logging;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Assets.Classes.Core.Graph
{
    public class CompactGraph : BidirectionalGraph<string, CompactEdge>
    {
        public static CompactGraph InitializeFrom(string serializedGraphFilePath, ICollection<ConnectionType> acceptedConnectionTypes)
        {
            if (!File.Exists(serializedGraphFilePath))
            {
                throw new FileNotFoundException($"File not found at\"{serializedGraphFilePath}\".");
            }
            
            var graph = new CompactGraph();
            using (var reader = new StreamReader(serializedGraphFilePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        // empty line
                        continue;
                    }

                    // example of valid line
                    // a14B1B2:0_m5B343,sEF2F;1_mDA;

                    // process "a14B1B2:0_m5B343,sEF2F;1_mDA;"
                    var split1 = line.Split(':');
                    if (split1.Length != 2 || split1.Any(string.IsNullOrEmpty))
                    {
                        Logger.LogWarning("Ignored invalid line: " + line);
                        continue;
                    }

                    // process "a14B1B2"
                    string vertexId = split1[0];

                    // process "0_m5B343,sEF2F;1_mDA;"
                    var split2 = split1[1].Split(';').ToList();
                    split2.RemoveAll(string.IsNullOrEmpty);
                    if (split2.Count == 0)
                    {
                        Logger.LogWarning("Ignored line with no edge: " + line);
                        continue;
                    }

                    var edges = new List<CompactEdge>();

                    foreach (var serializedEdges in split2)
                    {
                        // process "0_m5B343,sEF2F"
                        var split3 = serializedEdges.Split('_');
                        if (split3.Length != 2 || split3.Any(string.IsNullOrWhiteSpace))
                        {
                            Logger.LogWarning($"Ignored invalid edges: {serializedEdges}. Other edges are still processed (full line is \"{line}\").");
                            continue;
                        }

                        // process "0"
                        int intType;
                        if (!int.TryParse(split3[0], out intType))
                        {
                            Logger.LogWarning($"Ignored edges with invalid type: {split3[0]}. Edges were {serializedEdges}. Other edges are still processed (full line is \"{line}\").");
                            continue;
                        }

                        var type = (ConnectionType) intType;
                        if (!Enum.IsDefined(typeof(ConnectionType), type))
                        {
                            Logger.LogWarning($"Ignored edges with undefined type: {split3[0]}. Edges were {serializedEdges}. Other edges are still processed (full line is \"{line}\").");
                            continue;
                        }

                        if (!acceptedConnectionTypes.Contains(type))
                        {
                            continue;
                        }

                        // process "m5B343,sEF2F"
                        var split4 = split3[1].Split(',').ToList();
                        split4.RemoveAll(string.IsNullOrWhiteSpace);
                        if (split4.Count == 0)
                        {
                            Logger.LogWarning($"Ignored edges with type {type} because there was no associated entry. " +
                                              $"Edges were {serializedEdges}. Other edges are still processed (full line is \"{line}\").");
                            continue;
                        }

                        var edgeWithSameType = new List<CompactEdge>();
                        foreach (var connectedVertexId in split4)
                        {
                            // process "m5B343"
                            edgeWithSameType.Add(new CompactEdge(vertexId, connectedVertexId));
                            edgeWithSameType.Add(new CompactEdge(connectedVertexId, vertexId));
                        }

                        if (edgeWithSameType.Count == 0)
                        {
                            Logger.LogWarning($"Ignored edges because no valid vertex id could be processed: {serializedEdges}. " +
                                              $"Other edges are still processed (full line is \"{line}\").");
                            continue;
                        }

                        edges.AddRange(edgeWithSameType);
                    }

                    if (edges.Count == 0)
                    {
                        continue;
                    }
                    
                    graph.AddVerticesAndEdgeRange(edges);
                }

                Logger.LogInfo($"Loaded graph with {graph.VertexCount} vertices and {graph.EdgeCount} edges.");
            }

            return graph;
        }

        public List<string> GetShortestPath(string sourceId, string targetId)
        {
            var res = GetShortestPaths(sourceId, new List<string> {targetId});
            return res.Any() 
                ? res.First() 
                : new List<string>();
        }
        
        public List<List<string>> GetShortestPaths(string sourceId, ICollection<string> targetIds)
        {
            var sourceVertex = GetVertexFrom(sourceId);
            
            var results = new List<List<string>>();
            if (!this.ContainsVertex(sourceVertex))
            {
                return results;
            }

            var computeShortestPathFunc = this.ShortestPathsAStar(e => 1, v => 1, sourceVertex);

            foreach (var targetId in targetIds)
            {
                var targetVertex = GetVertexFrom(targetId);

                if (!this.ContainsVertex(targetVertex)
                    || targetVertex == sourceVertex)
                {
                    continue;
                }

                IEnumerable<CompactEdge> path;
                computeShortestPathFunc(targetVertex, out path);

                var result = path.Select(edge => GetEntryId(edge.Source)).ToList();
                result.Add(targetId);

                results.Add(result);
            }
            
            return results;
        }

        public override bool AddEdge(CompactEdge e)
        {
            if (this.ContainsEdge(e))
            {
                return false;
            }

            return base.AddEdge(e);
        }

        public override bool AddVerticesAndEdge(CompactEdge e)
        {
            if (this.ContainsEdge(e))
            {
                return false;
            }

            return base.AddVerticesAndEdge(e);
        }
        
        private string GetVertexFrom(string entryId)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException("EntryId was null.");
            }

            var split = entryId.Split('-');

            if (split.Length != 2)
            {
                throw new ArgumentException($"\"{entryId}\" is not a valid id.");
            }

            var entryType = split[0];
            var tmdbId = split[1];

            if (string.IsNullOrEmpty(entryType) || string.IsNullOrEmpty(tmdbId))
            {
                throw new ArgumentException($"\"{entryId}\" is not a valid id.");
            }

            uint id;
            if (!uint.TryParse(tmdbId, out id))
            {
                throw new ArgumentException($"\"{entryId}\" is not a valid id.");
            }

            switch (entryType)
            {
                case nameof(Movie):
                    return "m" + id.ToString("X");

                case nameof(Artist):
                    return "a" + id.ToString("X");

                default:
                    throw new ArgumentException($"Chunk \"{entryType}\" of \"{entryId}\" is not handled.");
            }
        }

        private string GetEntryId(string vertexId)
        {
            if (string.IsNullOrEmpty(vertexId) || vertexId.Length < 2)
            {
                throw new ArgumentException("The given vertexId was null.", nameof(vertexId));
            }

            var compressedEntryType = vertexId[0];
            var compressedEntryId = vertexId.Substring(1);

            switch (compressedEntryType)
            {
                case 'm':
                    return nameof(Movie) + '-' + uint.Parse(compressedEntryId, NumberStyles.HexNumber);

                case 'a':
                    return nameof(Artist) + '-' + uint.Parse(compressedEntryId, NumberStyles.HexNumber);
                default:
                    throw new ArgumentException($"Chunk \"{compressedEntryType}\" of \"{vertexId}\" is not handled.");
            }
        }
    }
}