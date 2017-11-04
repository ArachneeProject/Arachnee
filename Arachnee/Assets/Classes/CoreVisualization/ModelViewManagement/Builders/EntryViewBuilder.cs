using System;
using System.Collections.Generic;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViews;
using Assets.Classes.Logging;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Assets.Classes.CoreVisualization.ModelViewManagement.Builders
{
    public class EntryViewBuilder
    {
        public Dictionary<Type, EntryView> EntryViewPrefabs { get; } = new Dictionary<Type, EntryView>();
        
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
    }
}