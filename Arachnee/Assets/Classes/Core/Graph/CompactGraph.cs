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
        public void InitializeFrom(string serializedGraphFilePath)
        {
            if (!File.Exists(serializedGraphFilePath))
            {
                throw new FileNotFoundException($"File not found at\"{serializedGraphFilePath}\".");
            }

            this.Clear();

            using (var reader = new StreamReader(serializedGraphFilePath))
            {
                while (!reader.EndOfStream)
                {
                    // example of valid line
                    // MA0:3_A74C;1_A74C,A74B;

                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    // process "MA0:3_A74C;1_A74C,A74B;"
                    var split1 = line.Split(':');
                    if (split1.Length != 2 || split1.Any(string.IsNullOrEmpty))
                    {
                        Logger.LogWarning("Ignored invalid line: " + line);
                        continue;
                    }

                    // process "MA0"
                    string vertexId = split1[0];
                    
                    // process "3_A74C;1_A74C,A74B;"
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
                        // process "1_A74C,A74B"
                        var split3 = serializedEdges.Split('_');
                        if (split3.Length != 2 || split3.Any(string.IsNullOrWhiteSpace))
                        {
                            Logger.LogWarning($"Ignored invalid edges: {serializedEdges}. Other edges are still processed (full line is \"{line}\").");
                            continue;
                        }

                        // process "1"
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

                        // process "A74C,A74B"
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
                            // process "A74C"
                            edgeWithSameType.Add(new CompactEdge(vertexId, connectedVertexId, type));
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

                    Logger.LogInfo("adding " + string.Join("\n", edges.Select(e => e.Source + " :: " + e.Type + " :: " + e.Target)));
                    this.AddVerticesAndEdgeRange(edges);
                }

                Logger.LogInfo($"Loaded graph with {this.VertexCount} and {this.EdgeCount} edges.");
            }
        }

        public List<string> GetShortestPath(string sourceId, string targetId)
        {
            IEnumerable<CompactEdge> path;

            var computeShortestPathFunc = this.ShortestPathsDijkstra(e => 1, Compress(sourceId));
            computeShortestPathFunc(Compress(targetId), out path);

            var result = path.Select(edge => Decompress(edge.Source)).ToList();
            result.Add(targetId);

            return result;
        }

        public override bool AddEdge(CompactEdge e)
        {
            if (this.Edges.Contains(e))
            {
                return false;
            }

            return base.AddEdge(e);
        }

        public override bool AddVerticesAndEdge(CompactEdge e)
        {
            if (this.Edges.Contains(e))
            {
                Logger.LogInfo($"{e.Source}::{e.Type}::{e.Target} is already present.");
                return false;
            }

            return base.AddVerticesAndEdge(e);
        }
        
        private string Compress(string entryId)
        {
            if (string.IsNullOrEmpty(entryId))
                throw new ArgumentException("EntryId was null.");

            var split = entryId.Split('-');

            if (split.Length != 2)
                throw new ArgumentException($"\"{entryId}\" is not a valid id.");

            var entryType = split[0];
            var tmdbId = split[1];

            if (string.IsNullOrEmpty(entryType) || string.IsNullOrEmpty(tmdbId))
                throw new ArgumentException($"\"{entryId}\" is not a valid id.");

            uint id;
            if (!uint.TryParse(tmdbId, out id))
                throw new ArgumentException($"\"{entryId}\" is not a valid id.");

            switch (entryType)
            {
                case nameof(Movie):
                    return "M" + id.ToString("X");

                case nameof(Artist):
                    return "A" + id.ToString("X");

                default:
                    throw new ArgumentException($"Chunk \"{entryType}\" of \"{entryId}\" is not handled.");
            }
        }

        private string Decompress(string compressedId)
        {
            if (string.IsNullOrEmpty(compressedId) || compressedId.Length < 2)
                throw new ArgumentException("The given compressedId was null.", nameof(compressedId));

            var compressedEntryType = compressedId[0];
            var compressedEntryId = compressedId.Substring(1);

            switch (compressedEntryType)
            {
                case 'M':
                    return nameof(Movie) + '-' + uint.Parse(compressedEntryId, NumberStyles.HexNumber);

                case 'A':
                    return nameof(Artist) + '-' + uint.Parse(compressedEntryId, NumberStyles.HexNumber);
                default:
                    throw new ArgumentException(
                        $"Chunk \"{compressedEntryType}\" of \"{compressedId}\" is not handled.");
            }
        }
    }
}