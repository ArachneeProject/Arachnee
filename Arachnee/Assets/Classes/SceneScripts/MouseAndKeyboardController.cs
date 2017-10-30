using UnityEngine;

namespace Assets.Classes.SceneScripts
{
    public class MouseAndKeyboardController : MonoBehaviour 
    {
        // keyboard
        public bool canMove = true;
        public float keyboardSensitivity = 0.8F;

        // mouse
        public bool canLookAround = true;

        public float sensitivityX = 2F;
        public float sensitivityY = -2F;
        
        public float minimumY = -60F;
        public float maximumY = 60F;

        float previousYAngle = 0F;
        
        void Update()
        {
            // keyboard
            if (this.canMove)
            {
                this.transform.position += this.transform.forward * Input.GetAxis("Vertical") * keyboardSensitivity;
                this.transform.position += this.transform.right * Input.GetAxis("Horizontal") * keyboardSensitivity;
            }

            // mouse
            if (canLookAround && Input.GetMouseButton(0))
            {
                float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
                previousYAngle += Input.GetAxis("Mouse Y") * sensitivityY;
                previousYAngle = Mathf.Clamp(previousYAngle, minimumY, maximumY);
                transform.localEulerAngles = new Vector3(previousYAngle, rotationX, 0);
            }
        }
    }
}
