using UnityEngine;

namespace Assets.Classes.PhysicsEngine
{
    public class Vertex : MonoBehaviour
    {
        public string Id { get; set; }

        public Rigidbody Rigidbody { get; private set; }

        void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
    }
}