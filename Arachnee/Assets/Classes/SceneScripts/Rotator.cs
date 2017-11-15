using UnityEngine;

namespace Assets.Classes.SceneScripts
{
    public class Rotator : MonoBehaviour
    {
        public float rotationSpeed = 1;
        public Vector3 axis = Vector3.one;
        
        void Update()
        {
            this.transform.Rotate(axis, rotationSpeed);
        }
    }
}