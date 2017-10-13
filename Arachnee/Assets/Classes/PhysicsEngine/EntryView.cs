using System;
using Assets.Classes.GraphElements;
using UnityEngine;

namespace Assets.Classes.PhysicsEngine
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
            {
                throw new Exception("No Rigibody component found on EntryView GameObject.");
            }
        }

        private void OnMouseUpAsButton()
        {
            if (OnClicked != null)
            {
                OnClicked(this);
            }

        }
    }
}