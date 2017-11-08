using Assets.Classes.CoreVisualization.ModelViews;

namespace Assets.Classes.CoreVisualization.ModelViewManagement.Builders.ComponentInitializers
{
    public interface IComponentInitializer
    {
        /// <summary>
        /// Initialize the components of the given EntryView.
        /// </summary>
        void InitializeComponents(EntryView entryView);

        /// <summary>
        /// Initialize the components of the given ConnectionView.
        /// </summary>
        void InitializeComponents(ConnectionView connectionView);

        /// <summary>
        /// Initialize the components of the given SearchResultView.
        /// </summary>
        void InitializeComponents(SearchResultView searchResultView);
    }
}