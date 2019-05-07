using UnityEngine;

namespace Stereo3D
{
    public class SwitchCamera : MonoBehaviour
    {
        public KeyCode toggleDoubleCameraKey = KeyCode.X;
        public static bool toggleDoubleCamera = false;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(toggleDoubleCameraKey))
            {
                toggleDoubleCamera = !toggleDoubleCamera;
                if (toggleDoubleCamera)
                {
                    gameObject.SendMessage("SwitchDoubleCamera");
                }
                else
                {
                    gameObject.SendMessage("SwitchSingleCamera");
                }
            }
        }
    }
}
