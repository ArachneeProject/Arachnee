using System;
using Assets.Classes.GraphElements;
using UnityEngine;

namespace Assets.Classes.PhysicsEngine
{
    public class Vertex : MonoBehaviour
    {
        public delegate void VertexClickedDelegate(Vertex v);

        public Entry Entry { get; set; }

        public Rigidbody Rigidbody { get; private set; }

        public event VertexClickedDelegate OnClicked;

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
            if (Rigidbody == null)
            {
                throw new Exception("No Rigibody component found on Vertex GameObject.");
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