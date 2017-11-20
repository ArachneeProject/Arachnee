using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Classes.Core.Graph;
using Assets.Classes.Core.Models;
using Assets.Classes.Logging;
using Newtonsoft.Json;

namespace Assets.Classes.Core.Serialization
{
    public class GraphDeserializer
    {
        private IEnumerable<CompactEdge> UnwrapEdges(string source, string serializedTargets)
        {
            if (string.IsNullOrEmpty(serializedTargets))
            {
                // no edge for this source
                yield break;
            }

            // $serializedTargets should looks like "A44C_0;AA98_0;AA99_0;..."
            var edgesStr = serializedTargets.Split(';');

            foreach (var edgeStr in edgesStr)
            {
                // looking at "A44C_0" chunk
                var edgeData = edgeStr.Split('_');

                var target = edgeData[0];

                ConnectionType connectionType;
                Enum.TryParse(edgeData[1], out connectionType);

                yield return new CompactEdge(source, target, connectionType);
            }
        }

        public CompactGraph LoadFrom(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"File not found at \"{filePath}\"");
            }

            var graph = new CompactGraph();

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    try
                    {
                        var row = JsonConvert.DeserializeObject<SerializedGraphRow>(reader.ReadLine());

                        var edges = UnwrapEdges(row.I, row.C);
                        graph.AddVerticesAndEdgeRange(edges);
                    }
                    catch (Exception e)
                    {
                        Logger.LogException(e);
                    }
                }
            }

            return graph;
        }
    }
}