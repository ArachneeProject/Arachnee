using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViews;
using Assets.Classes.Logging;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Assets.Classes.CoreVisualization.ModelViewManagement
{
    public class ModelViewBuilder
    {
        public Dictionary<Type, EntryView> EntryViewPrefabs { get; } = new Dictionary<Type, EntryView>();

        public ConnectionView ConnectionViewPrefab { get; set; }
        
        [CanBeNull]
        public EntryView BuildEntryView(Entry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            if (entry == DefaultEntry.Instance)
            {
                Logger.LogError("The given entry was the Default entry.");
                return null;
            }

            EntryView entryViewPrefab;
            if (!this.EntryViewPrefabs.TryGetValue(entry.GetType(), out entryViewPrefab))
            {
                Logger.LogWarning($"Cannot create view for type \"{entry.GetType().Name}\" because no prefab is set for it.");
                return null;
            }
            
            // instantiate EntryView GameObject
            var entryView = Object.Instantiate(entryViewPrefab);
            entryView.Entry = entry;
            entryView.gameObject.name = entry.ToString();
            
            return entryView;
        }
        
        [CanBeNull]
        public ConnectionView BuildConnectionView(EntryView leftEntryView, EntryView rightEntryView)
        {
            if (leftEntryView == null || rightEntryView == null)
            {
                Logger.LogError("Unable to build ConnectionView because at least one EntryView was null.");
                return null;
            }
            
            var leftToRight = leftEntryView.Entry.Connections.Where(c => c.ConnectedId == rightEntryView.Entry.Id);
            var rightToLeft = rightEntryView.Entry.Connections.Where(c => c.ConnectedId == leftEntryView.Entry.Id);

            var connectionTypes = leftToRight.Select(c => c.Type).Union(rightToLeft.Select(c => c.Type)).Distinct().ToList();

            // instantiate ConnectionView GameObjects
            var connectionView = Object.Instantiate(this.ConnectionViewPrefab);
            connectionView.ConnectionTypes = connectionTypes;
            connectionView.Left = leftEntryView;
            connectionView.Right = rightEntryView;
            connectionView.gameObject.name = connectionView.Id;
            
            return connectionView;
        }
    }
}