using Assets.Classes.CoreVisualization.ModelViews;

namespace Assets.Classes.CoreVisualization.ModelViewManagement.Builders.Tailors
{
    /// <summary>
    /// Simplest implementation of ITailor. Does nothing.
    /// </summary>
    public class RawTailor : ITailor
    {
        public void DressUp(EntryView entryView)
        {
        }

        public void DressUp(ConnectionView connectionView)
        {
        }

        public void DressUp(SearchResultView searchResultView)
        {
        }
    }
}