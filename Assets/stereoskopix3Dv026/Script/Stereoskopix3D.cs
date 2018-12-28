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
    [UnityEngine.AddComponentMenu("Stereoskopix/Stereoskopix3D")]
    public class Stereoskopix3D : MonoBehaviour
    {
        private RenderTexture leftCamRT;
        private RenderTexture rightCamRT;
        private GameObject leftCam;
        private GameObject rightCam;
        public Material stereoMaterial;
        private string[] modeStrings = new string[] { "Anaglyph", "Side By Side", "Over Under", "Interlace", "Checkerboard" };
        public mode3D format3D = mode3D.Anaglyph;
        private string[] anaStrings = new string[] { "Gray", "Half Color", "Color", "Optimized", "Magenta" };
        public anaType anaglyphOptions = anaType.HalfColor;
        private string[] sbsStrings = new string[] { "Squeezed", "Unsqueezed" };
        public modeSBS sideBySideOptions = modeSBS.Squeezed;
        public int interlaceRows = 1080;
        public int checkerboardColumns = 1920;
        public int checkerboardRows = 1080;
        public float interaxial = 0.25f;
        public float zeroParallax = 6.0f;
        private float toParallax = 6.0f;
        public float fieldOfView = 60.0f;
        public bool GuiVisible = true;
        public KeyCode ToggleGuiKey = KeyCode.Tab;
        public KeyCode ToggleToeInKey = KeyCode.P;
        public KeyCode LeftRightKey = KeyCode.Y;
        public KeyCode LeftOnlyKey = KeyCode.U;
        public KeyCode RightOnlyKey = KeyCode.I;
        public KeyCode RightLeftKey = KeyCode.O;
        public KeyCode trackObjKey = KeyCode.T;
        private string[] methodStrings = new string[] { "Parallel", "Toed In" };
        public method3D cameraMethod = method3D.Parallel;
        private string[] camStrings;
        public cams3D cameraSelect = cams3D.LeftRight;
        public float cameraAspect = 1.33f;
        public bool saveCustomAspect = false;
        private Rect windowRect = new Rect(20, 20, 600, 300);
        private MouseLookButton mouseLookScript; // find MouseLookButton script
        private bool dummy = false; // dummy button to get focus off text fields
        private bool toggleTrackObj = false;
        private GameObject trackObject;

        public void Start()
        {
            camStrings = new string[] 
            {
                "Left/Right ["+LeftRightKey+"]",
                "LeftOnly [" +LeftOnlyKey+"]",
                "RightOnly [" +RightOnlyKey+"]",
                "Right/Left [" +RightLeftKey+"]"
            };

            if (!stereoMaterial)
            {
                Debug.LogError("No Stereo Material Found. Please drag 'stereoMat' into the Stereo Material Field");
                this.enabled = false;
                return;
            }
            leftCam = new GameObject("leftCam", typeof(Camera));
            rightCam = new GameObject("rightCam", typeof(Camera));

            Camera tempCamera = GetComponent<Camera>();
            Camera tempLeftCamera = leftCam.GetComponent<Camera>();
            Camera tempRightCamera = rightCam.GetComponent<Camera>();

            tempLeftCamera.CopyFrom(tempCamera);
            tempRightCamera.CopyFrom(tempCamera);

            tempLeftCamera.renderingPath = tempCamera.renderingPath;
            tempRightCamera.renderingPath = tempCamera.renderingPath;

            fieldOfView = tempCamera.fieldOfView;
            if (saveCustomAspect)
            {
                tempCamera.aspect = cameraAspect;
            }
            else
            {
                cameraAspect = tempCamera.aspect;
            }

            //leftCam.AddComponent<GUILayer>();
            //rightCam.AddComponent<GUILayer>();

            leftCamRT = new RenderTexture(Screen.width, Screen.height, 24);
            rightCamRT = new RenderTexture(Screen.width, Screen.height, 24);

            tempLeftCamera.targetTexture = leftCamRT;
            tempRightCamera.targetTexture = rightCamRT;

            stereoMaterial.SetTexture("_LeftTex", leftCamRT);
            stereoMaterial.SetTexture("_RightTex", rightCamRT);

            tempLeftCamera.depth = tempCamera.depth - 2;
            tempRightCamera.depth = tempCamera.depth - 1;

            UpdateView();

            leftCam.transform.parent = transform;
            rightCam.transform.parent = transform;

            tempCamera.cullingMask = 0;
            tempCamera.backgroundColor = new Color(0, 0, 0, 0);
            tempCamera.clearFlags = CameraClearFlags.Nothing;

            mouseLookScript = tempCamera.GetComponent<MouseLookButton>(); // deactivate MouseLookButton script (if it exists) when GUI visible	
        }

        public void Update()
        {
            if (Input.GetKeyUp(ToggleGuiKey))
            {
                GuiVisible = !GuiVisible;
            }
            else if (Input.GetKeyUp(trackObjKey))
            {
                toggleTrackObj = !toggleTrackObj;
            }
            else if (Input.GetKeyUp(LeftRightKey))
            {
                cameraSelect = cams3D.LeftRight;
            }
            else if (Input.GetKeyUp(LeftOnlyKey))
            {
                cameraSelect = cams3D.LeftOnly;
            }
            else if (Input.GetKeyUp(RightOnlyKey))
            {
                cameraSelect = cams3D.RightOnly;
            }
            else if (Input.GetKeyUp(RightLeftKey))
            {
                cameraSelect = cams3D.RightLeft;
            }
            else if (Input.GetKeyUp(ToggleToeInKey))
            {
                if (cameraMethod == method3D.ToedIn)
                {
                    cameraMethod = method3D.Parallel;
                }
                else
                {
                    cameraMethod = method3D.ToedIn;
                }
            }
            else if (Input.GetKey("-"))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    interaxial -= 0.01f;
                }
                else
                {
                    interaxial -= 0.001f;
                }
                interaxial = Mathf.Max(interaxial, 0);
            }
            else if (Input.GetKey("="))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    interaxial += 0.01f;
                }
                else
                {
                    interaxial += 0.001f;
                }
            }
            else if (Input.GetKey("["))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    zeroParallax -= 0.1f;
                }
                else
                {
                    zeroParallax -= 0.01f;
                }
                zeroParallax = Mathf.Max(zeroParallax, 1);
            }
            else if (Input.GetKey("]"))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    zeroParallax += 0.1f;
                }
                else
                {
                    zeroParallax += 0.01f;
                }
            }
            if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
            {
                toParallax = convergeOnObject();
                StartCoroutine(LerpZero(zeroParallax, toParallax, 1.0f));
            }
            else if (trackObject && toggleTrackObj)
            {
                convergeTrackObject();
            }
        }

        public IEnumerator LerpZero(float start, float end, float speed)
        {
            var t = 0.0f;
            var rate = 1.0f / speed;
            while (t < 1.0f)
            {
                t += Time.deltaTime * rate;
                zeroParallax = Mathf.Lerp(start, end, t);
                yield return null;
            }
        }

        public float convergeOnObject()
        {
            RaycastHit hit;
            Ray ray = leftCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);   // converge to clicked point
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                trackObject = hit.collider.gameObject;
                //zeroParallax = Vector3.Distance(transform.position,hit.collider.gameObject.transform.position); // converge to center of object
                float newZero = hit.distance;
                return newZero;
            }
            return 0f;
        }

        public void convergeTrackObject()//Debug.Log(trackObject.name+ " is OFF CAMERA");
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            //if(vec.x>0 && vec.x<1 && vec.y>0 && vec.y<1 && vec.z>0) { // alternate to bounds - just check object center
            if (GeometryUtility.TestPlanesAABB(planes, trackObject.GetComponent< Collider > ().bounds))
            {
                //Debug.Log(trackObject.name+" is ON CAMERA");
                RaycastHit hit;
                Vector3 vec = Camera.main.WorldToViewportPoint(trackObject.transform.position);
                Ray ray = Camera.main.ViewportPointToRay(vec);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    if (hit.collider.gameObject == trackObject && hit.distance > Camera.main.nearClipPlane)
                    {
                        zeroParallax = hit.distance;
                    }
                    else
                    {
                        //Debug.Log(trackObject.name+" is ON BUT HIDDEN");
                    }
                }
            }
            else
            {
                //Debug.Log(trackObject.name+ " is OFF CAMERA");
            }
        }

        public void LateUpdate()
        {
            UpdateView();
        }

        public void UpdateView()
        {
            switch (cameraSelect)
            {
                case cams3D.LeftRight:
                    leftCam.transform.position = transform.position + transform.TransformDirection(-interaxial / 2, 0, 0);
                    rightCam.transform.position = transform.position + transform.TransformDirection(interaxial / 2, 0, 0);
                    break;
                case cams3D.LeftOnly:
                    leftCam.transform.position = transform.position + transform.TransformDirection(-interaxial / 2, 0, 0);
                    rightCam.transform.position = transform.position + transform.TransformDirection(-interaxial / 2, 0, 0);
                    break;
                case cams3D.RightOnly:
                    leftCam.transform.position = transform.position + transform.TransformDirection(interaxial / 2, 0, 0);
                    rightCam.transform.position = transform.position + transform.TransformDirection(interaxial / 2, 0, 0);
                    break;
                case cams3D.RightLeft:
                    leftCam.transform.position = transform.position + transform.TransformDirection(interaxial / 2, 0, 0);
                    rightCam.transform.position = transform.position + transform.TransformDirection(-interaxial / 2, 0, 0);
                    break;
            }
            if (cameraMethod == method3D.ToedIn)
            {
                Camera tempCamera = GetComponent<Camera>();
                leftCam.GetComponent<Camera>().projectionMatrix = tempCamera.projectionMatrix;
                rightCam.GetComponent<Camera>().projectionMatrix = tempCamera.projectionMatrix;
                leftCam.transform.LookAt(transform.position + (transform.TransformDirection(Vector3.forward) * zeroParallax));
                rightCam.transform.LookAt(transform.position + (transform.TransformDirection(Vector3.forward) * zeroParallax));
            }
            else
            {
                leftCam.transform.rotation = transform.rotation;
                rightCam.transform.rotation = transform.rotation;
                switch (cameraSelect)
                {
                    case cams3D.LeftRight:
                        leftCam.GetComponent<Camera>().projectionMatrix = projectionMatrix(true);
                        rightCam.GetComponent<Camera>().projectionMatrix = projectionMatrix(false);
                        break;
                    case cams3D.LeftOnly:
                        leftCam.GetComponent<Camera>().projectionMatrix = projectionMatrix(true);
                        rightCam.GetComponent<Camera>().projectionMatrix = projectionMatrix(true);
                        break;
                    case cams3D.RightOnly:
                        leftCam.GetComponent<Camera>().projectionMatrix = projectionMatrix(false);
                        rightCam.GetComponent<Camera>().projectionMatrix = projectionMatrix(false);
                        break;
                    case cams3D.RightLeft:
                        leftCam.GetComponent<Camera>().projectionMatrix = projectionMatrix(false);
                        rightCam.GetComponent<Camera>().projectionMatrix = projectionMatrix(true);
                        break;
                }
            }
        }

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            RenderTexture.active = destination;
            GL.PushMatrix();
            GL.LoadOrtho();
            switch (format3D)
            {
                case mode3D.Anaglyph:
                    stereoMaterial.SetPass(0);
                    DrawQuad(0);
                    break;
                case mode3D.SideBySide:
                case mode3D.OverUnder:
                    for (int i = 1; i <= 2; i++) {
                        stereoMaterial.SetPass(i);
                        DrawQuad(i);
                    }
                    break;
                case mode3D.Interlace:
                case mode3D.Checkerboard:
                    stereoMaterial.SetPass(3);
                    DrawQuad(3);
                    break;
                default:
                    break;
            }
            GL.PopMatrix();
        }

        public void OnGUI()
        {
            if (GuiVisible)
            {
                windowRect = GUILayout.Window(0, windowRect, DoWindow, "Stereoskopix 3D Controls");
                if (mouseLookScript) MouseLookButton.suppress = true;
            }
            else
            {
                if (mouseLookScript) MouseLookButton.suppress = false;
            }
        }

        public void DoWindow(int windowID)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Mode");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            format3D = (mode3D)GUILayout.SelectionGrid((int)format3D, modeStrings, 1, GUILayout.MaxWidth(100));
            if (GUI.changed)
            {
                if (format3D == mode3D.Interlace)
                {
                    SetWeave(0);
                }
                else if (format3D == mode3D.Checkerboard)
                {
                    SetWeave(1);
                }
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Options");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            anaglyphOptions = (anaType)GUILayout.Toolbar((int)anaglyphOptions, anaStrings, GUILayout.MaxWidth(500));
            if (GUI.changed)
            {
                SetAnaglyphType();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            sideBySideOptions = (modeSBS)GUILayout.Toolbar((int)sideBySideOptions, sbsStrings, GUILayout.MaxWidth(200));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("[Alt-Click on Object to Converge]");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("Rows", GUILayout.MinWidth(60));
            string interlaceString = System.Convert.ToString(interlaceRows);
            interlaceString = GUILayout.TextField(interlaceString, 4, GUILayout.MaxWidth(50));
            interlaceRows = System.Convert.ToInt32(interlaceString);
            if (GUI.changed)
            {
                if (format3D == mode3D.Interlace)
                {
                    SetWeave(0);
                }
            }
            if (GUILayout.Button("-"))
            {
                interlaceRows -= 1;
                SetWeave(0);
            }
            if (GUILayout.Button("+"))
            {
                interlaceRows += 1;
                SetWeave(0);
            }
            GUILayout.FlexibleSpace();
            toggleTrackObj = GUILayout.Toggle(toggleTrackObj, "Track Object [T]");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("Columns", GUILayout.MinWidth(60));
            string checkerString = System.Convert.ToString(checkerboardColumns);
            checkerString = GUILayout.TextField(checkerString, 4, GUILayout.MaxWidth(50));
            checkerboardColumns = System.Convert.ToInt32(checkerString);
            GUILayout.Label("Rows");
            checkerString = System.Convert.ToString(checkerboardRows);
            checkerString = GUILayout.TextField(checkerString, 4, GUILayout.MaxWidth(50));
            checkerboardRows = System.Convert.ToInt32(checkerString);
            if (GUI.changed)
            {
                if (format3D == mode3D.Checkerboard)
                {
                    SetWeave(1);
                }
            }
            if (GUILayout.Button("enter"))
            {
                GUI.FocusControl("focus");
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label("  [" + ToggleGuiKey + " toggles controls]");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera Select", GUILayout.MinWidth(120));
            GUILayout.Space(15);
            cameraSelect = (cams3D)GUILayout.Toolbar((int)cameraSelect, camStrings, GUILayout.MaxWidth(400));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera Method [P]", GUILayout.MinWidth(120));
            GUILayout.Space(15);
            cameraMethod = (method3D)GUILayout.Toolbar((int)cameraMethod, methodStrings, GUILayout.MaxWidth(200));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Interaxial  - +", GUILayout.MinWidth(120));
            interaxial = GUILayout.HorizontalSlider(interaxial, 0.0f, 5.0f, GUILayout.MaxWidth(300));
            GUILayout.Label(" " + interaxial);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Zero Parallax  [ ] ", GUILayout.MinWidth(120));
            zeroParallax = GUILayout.HorizontalSlider(zeroParallax, 1.0f, 100.0f, GUILayout.MaxWidth(300));
            GUILayout.Label(" " + zeroParallax);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Field of View", GUILayout.MinWidth(120));
            fieldOfView = GUILayout.HorizontalSlider(fieldOfView, 1.0f, 180.0f, GUILayout.MaxWidth(300));

            Camera tempCamera = GetComponent<Camera>();
            tempCamera.fieldOfView = fieldOfView;
            GUILayout.Label(" " + fieldOfView);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Aspect Ratio", GUILayout.MinWidth(120));
            if (GUILayout.Button("Reset"))
            {
                tempCamera.ResetAspect();
                cameraAspect = tempCamera.aspect;
            }
            cameraAspect = GUILayout.HorizontalSlider(cameraAspect, 0.1f, 4.0f, GUILayout.MaxWidth(250));
            tempCamera.aspect = cameraAspect;
            GUILayout.Label(" " + cameraAspect);
            GUILayout.FlexibleSpace();
            GUI.SetNextControlName("focus");
            dummy = GUILayout.Toggle(dummy, "");

            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        private void SetWeave(int xy)
        {
            if (System.Convert.ToBoolean(xy))
            {
                stereoMaterial.SetFloat("_Weave_X", checkerboardColumns);
                stereoMaterial.SetFloat("_Weave_Y", checkerboardRows);
            }
            else
            {
                stereoMaterial.SetFloat("_Weave_X", 1);
                stereoMaterial.SetFloat("_Weave_Y", interlaceRows);
            }
        }

        private void SetAnaglyphType()
        {
            anaType anaType = this.anaglyphOptions;
            if (anaType == anaType.Monochrome)
            {
                this.stereoMaterial.SetVector("_Balance_Left_R", new Vector4(0.299f, 0.587f, 0.114f, (float)0));
                this.stereoMaterial.SetVector("_Balance_Left_G", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Left_B", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_R", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_G", new Vector4(0.299f, 0.587f, 0.114f, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_B", new Vector4(0.299f, 0.587f, 0.114f, (float)0));
            }
            else if (anaType == anaType.HalfColor)
            {
                this.stereoMaterial.SetVector("_Balance_Left_R", new Vector4(0.299f, 0.587f, 0.114f, (float)0));
                this.stereoMaterial.SetVector("_Balance_Left_G", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Left_B", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_R", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_G", new Vector4((float)0, (float)1, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_B", new Vector4((float)0, (float)0, (float)1, (float)0));
            }
            else if (anaType == anaType.FullColor)
            {
                this.stereoMaterial.SetVector("_Balance_Left_R", new Vector4((float)1, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Left_G", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Left_B", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_R", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_G", new Vector4((float)0, (float)1, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_B", new Vector4((float)0, (float)0, (float)1, (float)0));
            }
            else if (anaType == anaType.Optimized)
            {
                this.stereoMaterial.SetVector("_Balance_Left_R", new Vector4((float)0, 0.7f, 0.3f, (float)0));
                this.stereoMaterial.SetVector("_Balance_Left_G", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Left_B", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_R", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_G", new Vector4((float)0, (float)1, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_B", new Vector4((float)0, (float)0, (float)1, (float)0));
            }
            else if (anaType == anaType.Purple)
            {
                this.stereoMaterial.SetVector("_Balance_Left_R", new Vector4(0.299f, 0.587f, 0.114f, (float)0));
                this.stereoMaterial.SetVector("_Balance_Left_G", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Left_B", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_R", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_G", new Vector4((float)0, (float)0, (float)0, (float)0));
                this.stereoMaterial.SetVector("_Balance_Right_B", new Vector4(0.299f, 0.587f, 0.114f, (float)0));
            }
        }

        private void DrawQuad(int cam)
        {
            if (this.format3D == mode3D.Anaglyph)
            {
                GL.Begin(7);
                GL.TexCoord2((float)0, (float)0);
                GL.Vertex3((float)0, (float)0, 0.1f);
                GL.TexCoord2(1f, (float)0);
                GL.Vertex3((float)1, (float)0, 0.1f);
                GL.TexCoord2(1f, 1f);
                GL.Vertex3((float)1, 1f, 0.1f);
                GL.TexCoord2((float)0, 1f);
                GL.Vertex3((float)0, 1f, 0.1f);
                GL.End();
            }
            else if (this.format3D == mode3D.SideBySide)
            {
                if (cam == 1)
                {
                    GL.Begin(7);
                    GL.TexCoord2((float)0, (float)0);
                    GL.Vertex3((float)0, (float)0, 0.1f);
                    GL.TexCoord2(1f, (float)0);
                    GL.Vertex3(0.5f, (float)0, 0.1f);
                    GL.TexCoord2(1f, 1f);
                    GL.Vertex3(0.5f, 1f, 0.1f);
                    GL.TexCoord2((float)0, 1f);
                    GL.Vertex3((float)0, 1f, 0.1f);
                    GL.End();
                }
                else
                {
                    GL.Begin(7);
                    GL.TexCoord2((float)0, (float)0);
                    GL.Vertex3(0.5f, (float)0, 0.1f);
                    GL.TexCoord2(1f, (float)0);
                    GL.Vertex3(1f, (float)0, 0.1f);
                    GL.TexCoord2(1f, 1f);
                    GL.Vertex3(1f, 1f, 0.1f);
                    GL.TexCoord2((float)0, 1f);
                    GL.Vertex3(0.5f, 1f, 0.1f);
                    GL.End();
                }
            }
            else if (this.format3D == mode3D.OverUnder)
            {
                if (cam == 1)
                {
                    GL.Begin(7);
                    GL.TexCoord2((float)0, (float)0);
                    GL.Vertex3((float)0, 0.5f, 0.1f);
                    GL.TexCoord2(1f, (float)0);
                    GL.Vertex3(1f, 0.5f, 0.1f);
                    GL.TexCoord2(1f, 1f);
                    GL.Vertex3(1f, 1f, 0.1f);
                    GL.TexCoord2((float)0, 1f);
                    GL.Vertex3((float)0, 1f, 0.1f);
                    GL.End();
                }
                else
                {
                    GL.Begin(7);
                    GL.TexCoord2((float)0, (float)0);
                    GL.Vertex3((float)0, (float)0, 0.1f);
                    GL.TexCoord2(1f, (float)0);
                    GL.Vertex3(1f, (float)0, 0.1f);
                    GL.TexCoord2(1f, 1f);
                    GL.Vertex3(1f, 0.5f, 0.1f);
                    GL.TexCoord2((float)0, 1f);
                    GL.Vertex3((float)0, 0.5f, 0.1f);
                    GL.End();
                }
            }
            else if (this.format3D == mode3D.Interlace || this.format3D == mode3D.Checkerboard)
            {
                GL.Begin(7);
                GL.TexCoord2((float)0, (float)0);
                GL.Vertex3((float)0, (float)0, 0.1f);
                GL.TexCoord2(1f, (float)0);
                GL.Vertex3((float)1, (float)0, 0.1f);
                GL.TexCoord2(1f, 1f);
                GL.Vertex3((float)1, 1f, 0.1f);
                GL.TexCoord2((float)0, 1f);
                GL.Vertex3((float)0, 1f, 0.1f);
                GL.End();
            }
        }

        public Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            var x = (2.0f * near) / (right - left);
            var y = (2.0f * near) / (top - bottom);
            var a = (right + left) / (right - left);
            var b = (top + bottom) / (top - bottom);
            var c = -(far + near) / (far - near);
            var d = -(2.0f * far * near) / (far - near);
            var e = -1.0f;

            Matrix4x4 m = default(Matrix4x4);
            m[0, 0] = x;
            m[0, 1] = 0;
            m[0, 2] = a;
            m[0, 3] = 0;
            m[1, 0] = 0;
            m[1, 1] = y;
            m[1, 2] = b;
            m[1, 3] = 0;
            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = c;
            m[2, 3] = d;
            m[3, 0] = 0;
            m[3, 1] = 0;
            m[3, 2] = e;
            m[3, 3] = 0;
            return m;
        }

        public Matrix4x4 projectionMatrix(bool isLeftCam)
        {
            float left;
            float right;
            float a;
            float b;
            float FOVrad;
            Camera tempCamera = GetComponent<Camera>();
            float aspect = tempCamera.aspect;
            float tempAspect;
            if (sideBySideOptions == modeSBS.Unsqueezed && format3D == mode3D.SideBySide)
            {
                FOVrad = tempCamera.fieldOfView / 90.0f * Mathf.PI;
                tempAspect = aspect / 2;
            }
            else
            {
                FOVrad = tempCamera.fieldOfView / 180.0f * Mathf.PI;
                tempAspect = aspect;
            }

            a = tempCamera.nearClipPlane * Mathf.Tan(FOVrad * 0.5f);
            b = tempCamera.nearClipPlane / (zeroParallax + tempCamera.nearClipPlane);

            if (isLeftCam)
            {
                left = -tempAspect * a + (interaxial / 2) * b;
                right = tempAspect * a + (interaxial / 2) * b;
            }
            else
            {
                left = -tempAspect * a - (interaxial / 2) * b;
                right = tempAspect * a - (interaxial / 2) * b;
            }

            return PerspectiveOffCenter(left, right, -a, a, tempCamera.nearClipPlane, tempCamera.farClipPlane);
        }
    }
}