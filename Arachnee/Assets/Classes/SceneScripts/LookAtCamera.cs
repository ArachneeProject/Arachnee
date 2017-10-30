using UnityEngine;

namespace Assets.Classes.SceneScripts
{
    public class LookAtCamera : MonoBehaviour
    {
        public bool lookAway = true;

        void Update()
        {
            var positionToLookAt = lookAway
                ? 2 * this.transform.position - Camera.main.transform.position
                : Camera.main.transform.position;

            this.transform.LookAt(positionToLookAt);
        }
    }
}