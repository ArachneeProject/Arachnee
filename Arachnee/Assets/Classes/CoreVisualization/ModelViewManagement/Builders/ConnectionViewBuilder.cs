using System.Linq;
using Assets.Classes.CoreVisualization.ModelViews;
using JetBrains.Annotations;
using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization.ModelViewManagement.Builders
{
    public class ConnectionViewBuilder
    {
        public ConnectionView ConnectionViewPrefab { get; set; }

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

            return connectionView;
        }
    }
}