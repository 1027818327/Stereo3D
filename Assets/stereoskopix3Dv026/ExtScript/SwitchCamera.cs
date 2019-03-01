using UnityEngine;

namespace Stereo3D
{
    public class SwitchCamera : MonoBehaviour
    {
        public KeyCode toggleDoubleCameraKey = KeyCode.X;
        public bool toggleDoubleCamera = true;

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

        private void SwitchSingleCamera()
        {
            Camera tempCamera = GetComponent<Camera>();
            if (tempCamera != null)
            {
                tempCamera.cullingMask = -1;
                tempCamera.backgroundColor = new Color(0, 0, 0, 255);
                tempCamera.clearFlags = CameraClearFlags.Skybox;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform tempTrans = transform.GetChild(i);
                Camera tempC = tempTrans.GetComponent<Camera>();
                if (tempC != null)
                {
                    tempTrans.gameObject.SetActive(false);
                }
            }
        }

        private void SwitchDoubleCamera()
        {
            Camera tempCamera = GetComponent<Camera>();
            if (tempCamera != null)
            {
                tempCamera.cullingMask = 0;
                tempCamera.backgroundColor = new Color(0, 0, 0, 0);
                tempCamera.clearFlags = CameraClearFlags.Nothing;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform tempTrans = transform.GetChild(i);
                Camera tempC = tempTrans.GetComponent<Camera>();
                if (tempC != null)
                {
                    tempTrans.gameObject.SetActive(true);
                }
            }
        }
    }
}
