using UnityEngine;
using System.Collections;

/*
.-------------------------------------------------------------------
|  Unity Stereoskopix 3D v027
|-------------------------------------------------------------------
|  This all started when TheLorax began this thread:
|  http://forum.unity3d.com/threads/11775 
|-------------------------------------------------------------------
|  There were numerous contributions to the thread from 
|  aNTeNNa trEE, InfiniteAlec, Jonathan Czeck, monark and others.
|-------------------------------------------------------------------
|  checco77 of Esimple Studios wrapped the whole thing up
|  in a script & packaged it with a shader, materials, etc. 
|  http://forum.unity3d.com/threads/60961 
|  Esimple included a copyright & license:
|  Copyright (c) 2010, Esimple Studios All Rights Reserved.
|  License: Distributed under the GNU GENERAL PUBLIC LICENSE (GPL) 
| ------------------------------------------------------------------
|  I tweaked everything, added options for Side-by-Side, Over-Under,
|  Swap Left/Right, etc, along with a GUI interface: 
|  http://forum.unity3d.com/threads/63874 
|-------------------------------------------------------------------
|  Wolfram then pointed me to shaders for interlaced/checkerboard display.
|-------------------------------------------------------------------
|  In this version (v026), I added Wolfram's additional display modes,
|  moved Esimple's anaglyph options into the script (so that only one
|  material is needed), and reorganized the GUI.
|-------------------------------------------------------------------
|  The package consists of
|  1) this script ('stereoskopix3D.js')
|  2) a shader ('stereo3DViewMethods.shader') 
|  3) a material ('stereo3DMat')
|  4) a demo scene ('demoScene3D.scene') - WASD or arrow keys travel, 
|     L button grab objects, L button lookaround when GUI hidden.
|-------------------------------------------------------------------
|  Instructions: (NOTE: REQUIRES UNITY PRO) 
|  1. Drag this script onto your camera.
|  2. Drag 'stereoMat' into the 'Stereo Materials' field.
|  3. Hit 'Play'. 
|  4. Adjust parameters with the GUI controls, press the tab key to toggle.
|  5. To save settings from the GUI, copy them down, hit 'Stop',
|     and enter the new settings in the camera inspector.
'-------------------------------------------------------------------
|  Perry Hoberman <hoberman (at) bway.net
|-------------------------------------------------------------------
*/

namespace Stereo3D
{
    [System.Serializable]
    [UnityEngine.RequireComponent(typeof(Camera))]
    [UnityEngine.AddComponentMenu("stereoskopix/stereoskopix3D")]
    public class Stereoskopix3D : MonoBehaviour
    {
        private RenderTexture leftCamRT;
        private RenderTexture rightCamRT;
        private GameObject leftCam;
        private GameObject rightCam;
        public Material stereoMaterial;
        private string[] modeStrings;
        public mode3D format3D;
        private string[] anaStrings;
        public anaType anaglyphOptions;
        private string[] sbsStrings;
        public modeSBS sideBySideOptions;
        public int interlaceRows;
        public int checkerboardColumns;
        public int checkerboardRows;
        public float interaxial;
        public float zeroParallax;
        private float toParallax;
        public float fieldOfView;
        public bool GuiVisible;
        public KeyCode ToggleGuiKey;
        public KeyCode ToggleToeInKey;
        public KeyCode LeftRightKey;
        public KeyCode LeftOnlyKey;
        public KeyCode RightOnlyKey;
        public KeyCode RightLeftKey;
        public KeyCode trackObjKey;
        private string[] methodStrings;
        public method3D cameraMethod;
        private string[] camStrings;
        public cams3D cameraSelect;
        public float cameraAspect;
        public bool saveCustomAspect;
        private Rect windowRect;
        private MouseLookButton mouseLookScript; // find MouseLookButton script
        private bool dummy; // dummy button to get focus off text fields
        private bool toggleTrackObj;
        private GameObject trackObject;

        public void Start()
        {
            
        }

        public void Update()
        {
            
        }

        public IEnumerator LerpZero(float start, float end, float speed)
        {
            yield return null;
        }

        public float convergeOnObject()
        {
            return 0f;
        }

        public void convergeTrackObject()//Debug.Log(trackObject.name+ " is OFF CAMERA");
        {

        }

        public void LateUpdate()
        {
        }

        public void UpdateView()
        {

        }

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {

        }

        public void OnGUI()
        {

        }

        public void DoWindow(int windowID)
        {

        }

        private void SetWeave(object xy)
        {

        }

        private void SetAnaglyphType()
        {

        }

        private void DrawQuad(int cam)
        {

        }

        public Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            return default(Matrix4x4);
        }

        public Matrix4x4 projectionMatrix(bool isLeftCam)
        {
            return default(Matrix4x4);
        }
    }
}