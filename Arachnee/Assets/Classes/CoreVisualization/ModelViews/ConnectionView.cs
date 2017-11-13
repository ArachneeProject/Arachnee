using System;
using UnityEngine;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    public class ConnectionView : MonoBehaviour
    {
        public EntryView Left { get; set; }

        public EntryView Right { get; set; }
        
        public string Id => GetIdentifier(this.Left.Entry.Id, this.Right.Entry.Id);

        public static string GetIdentifier(string fromEntryId, string toEntryId)
        {
            return string.Compare(fromEntryId, toEntryId, StringComparison.OrdinalIgnoreCase) < 0
                ? $"{fromEntryId}::::{toEntryId}"
                : $"{toEntryId}::::{fromEntryId}";
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
            
        }
    }
}