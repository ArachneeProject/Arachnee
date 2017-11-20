using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Classes.Core.Models;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.ShortestPath;

namespace Assets.Classes.Core.Graph
{
    public class CompactGraph : BidirectionalGraph<string, CompactEdge>
    {
        private string Compress(string entryId)
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
            {
                throw new ArgumentException("The given compressedId was null.", nameof(compressedId));
            }

            var compressedEntryType = compressedId[0];
            var compressedEntryId = compressedId.Substring(1);

            switch (compressedEntryType)
            {
                case 'M':
                    return nameof(Movie) + '-' + uint.Parse(compressedEntryId, NumberStyles.HexNumber);

                case 'A':
                    return nameof(Artist) + '-' + uint.Parse(compressedEntryId, NumberStyles.HexNumber);
                default:
                    throw new ArgumentException($"Chunk \"{compressedEntryType}\" of \"{compressedId}\" is not handled.");
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
    }
}