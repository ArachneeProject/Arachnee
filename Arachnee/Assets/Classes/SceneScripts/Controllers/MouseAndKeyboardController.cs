using UnityEngine;

namespace Assets.Classes.SceneScripts.Controllers
{
    public class MouseAndKeyboardController : ControllerBase 
    {
        // keyboard
        public bool canMove = true;
        public float keyboardSensitivity = 0.8F;

        // mouse
        public bool canLookAround = true;

        public float sensitivityX = 2F;
        public float sensitivityY = -2F;
        
        public float verticalDeadAngle = 30;
        
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
                float rotationX = Input.GetAxis("Mouse X") * sensitivityX;
                var rotationY = Input.GetAxis("Mouse Y") * sensitivityY;
                
                transform.Rotate(Vector3.up, rotationX, Space.World);
                transform.Rotate(Vector3.right, rotationY, Space.Self);
                
                if (transform.localEulerAngles.x > 90 - verticalDeadAngle
                    && transform.localEulerAngles.x < 270 + verticalDeadAngle)
                {
                    transform.Rotate(Vector3.right, -rotationY, Space.Self);
                }
            }
        }
    }
}
