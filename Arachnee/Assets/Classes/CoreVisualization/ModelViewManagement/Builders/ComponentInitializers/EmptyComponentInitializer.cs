using Assets.Classes.CoreVisualization.ModelViews;

namespace Assets.Classes.CoreVisualization.ModelViewManagement.Builders.ComponentInitializers
{
    /// <summary>
    /// Simplest implementation of IComponentInitializer. Does nothing.
    /// </summary>
    public class EmptyComponentInitializer : IComponentInitializer
    {
        public void InitializeComponents(EntryView entryView)
        {
        }

        public void InitializeComponents(ConnectionView connectionView)
        {
        }

        public void InitializeComponents(SearchResultView searchResultView)
        {
        }
    }
}