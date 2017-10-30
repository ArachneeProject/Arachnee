using System;
using Assets.Classes.Core.Models;
using UnityEngine;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    public class EntryView : MonoBehaviour
    {
        public delegate void EntryViewClickedDelegate(EntryView v);

        public Entry Entry { get; set; }

        public Rigidbody Rigidbody { get; private set; }

        public event EntryViewClickedDelegate OnClicked;

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
            if (Rigidbody == null)
                throw new Exception("No Rigibody component found on EntryView GameObject.");
        }

        void OnMouseUpAsButton()
        {
            OnClicked?.Invoke(this);
        }
    }
}