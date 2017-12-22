using System;
using Assets.Classes.CoreVisualization.ModelViews;
using JetBrains.Annotations;
using Logger = Assets.Classes.Logging.Logger;
using Object = UnityEngine.Object;

namespace Assets.Classes.CoreVisualization.ModelViewManagement.Builders
{
    /// <summary>
    /// Class in charge of creating a ConnectionView.
    /// </summary>
    public class ConnectionViewBuilder
    {
        public ConnectionView ConnectionViewPrefab { get; set; }

        /// <summary>
        /// Fired when an <see cref="ConnectionView"/> is built.
        /// </summary>
        public Action<ConnectionView> OnBuilt;

        [CanBeNull]
        public ConnectionView BuildConnectionView(EntryView leftEntryView, EntryView rightEntryView)
        {
            if (leftEntryView == null || rightEntryView == null)
            {
                Logger.LogError("Unable to build ConnectionView because at least one EntryView was null.");
                return null;
            }
            
            // instantiate ConnectionView GameObjects
            var connectionView = Object.Instantiate(this.ConnectionViewPrefab);
            connectionView.Left = leftEntryView;
            connectionView.Right = rightEntryView;
            connectionView.gameObject.name = connectionView.Id;
            OnBuilt?.Invoke(connectionView);

            return connectionView;
        }
    }
}