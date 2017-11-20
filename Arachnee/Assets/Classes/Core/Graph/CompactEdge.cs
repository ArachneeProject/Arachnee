using Assets.Classes.Core.Models;
using QuickGraph;

namespace Assets.Classes.Core.Graph
{
    public class CompactEdge : IEdge<string>
    {
        public string Source { get; }
        public string Target { get; }
        public ConnectionType Type { get; }

        public CompactEdge(string source, string target, ConnectionType type)
        {
            Source = source;
            Target = target;
            Type = type;
        }

        public bool Equals(CompactEdge other)
        {
            return other != null &&
                   this.Source.Equals(other.Source) &&
                   this.Target.Equals(other.Target);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as CompactEdge);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            unchecked
            {
                hash = hash * 17 ^ this.Source.GetHashCode();
                hash = hash * 17 ^ this.Target.GetHashCode();
                hash = hash * 17 ^ this.Type.GetHashCode();
            }

            return hash;
        }
    }
}