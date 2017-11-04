using Assets.Classes.CoreVisualization.ModelViews;

namespace Assets.Classes.CoreVisualization.ModelViewManagement.Builders.Tailors
{
    public interface ITailor
    {
        /// <summary>
        /// Adds some aesthetic components on the given EntryView.
        /// </summary>
        void DressUp(EntryView entryView);

        /// <summary>
        /// Adds some aesthetic components on the given ConnectionView.
        /// </summary>
        void DressUp(ConnectionView connectionView);

        /// <summary>
        /// Adds some aesthetic components on the given SearchResultView.
        /// </summary>
        void DressUp(SearchResultView searchResultView);
    }
}