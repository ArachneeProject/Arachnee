using QuickGraph;

namespace Assets.Classes.Core.Graph
{
    public class CompactEdge : EquatableEdge<string>
    {
        public CompactEdge(string source, string target) : base(source, target)
        {
        }
    }
}