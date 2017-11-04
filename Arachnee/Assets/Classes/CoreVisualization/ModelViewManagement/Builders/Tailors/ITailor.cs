using Assets.Classes.CoreVisualization.ModelViews;

namespace Assets.Classes.CoreVisualization.ModelViewManagement.Builders.Tailors
{
    public interface ITailor
    {
        /// <summary>
        /// Adds some aesthetic components and sets them up.
        /// </summary>
        void DressUp(EntryView behaviour);
    }
}