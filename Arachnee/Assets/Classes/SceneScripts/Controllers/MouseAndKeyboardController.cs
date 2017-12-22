using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

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

        private float _maxDownAngle;
        private float _maxUpAngle;

        void Start()
        {
            if (verticalDeadAngle > 90 || verticalDeadAngle < 0)
            {
                Logger.LogWarning($"{nameof(verticalDeadAngle)} has incorrect value {verticalDeadAngle} and will be reset to 0.");
                verticalDeadAngle = 0;
            }

            _maxDownAngle = 90 - verticalDeadAngle;
            _maxUpAngle = 270 + verticalDeadAngle;
        }

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
                
                if (transform.localEulerAngles.x > _maxDownAngle
                    && transform.localEulerAngles.x < _maxUpAngle)
                {
                    transform.localEulerAngles = transform.localEulerAngles.x < 180 
                        ? new Vector3(_maxDownAngle, transform.localEulerAngles.y, transform.localEulerAngles.z) 
                        : new Vector3(_maxUpAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
                }
            }
        }
    }
}
