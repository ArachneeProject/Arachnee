using Assets.Classes.GraphElements;
using UnityEngine;

namespace Assets.Classes.PhysicsEngine
{
    public class Vertex : MonoBehaviour
    {
        public Entry Entry { get; set; }

        public Rigidbody Rigidbody { get; private set; }

        void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
    }
}