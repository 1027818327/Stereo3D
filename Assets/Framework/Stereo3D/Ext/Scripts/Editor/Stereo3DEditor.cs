
#region 版权信息
/*
 * -----------------------------------------------------------
 *  Copyright (c) KeJun All rights reserved.
 * -----------------------------------------------------------
 *		描述: 
 *      创建者：陈伟超
 *      创建时间: 20190226
 *  
 */
#endregion


using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.EventSystems;

namespace Stereo3D
{
    public class Stereo3DEditor
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Unity Messages
        //    void Awake()
        //    {
        //
        //    }
        //    void OnEnable()
        //    {
        //
        //    }
        //
        //    void Start() 
        //    {
        //    
        //    }
        //    
        //    void Update() 
        //    {
        //    
        //    }
        //
        //    void OnDisable()
        //    {
        //
        //    }
        //
        //    void OnDestroy()
        //    {
        //
        //    }

        #endregion

        #region Private Methods
        [MenuItem("Tools/Stereo3D/Polarize/Into Current Scene")]
        static void AddPolarizeEffect()
        {
            Camera tempCamera = Camera.main;
            if (tempCamera != null)
            {
                AddPolarizeEffect(tempCamera.gameObject);
                ConfigMouse(tempCamera);

                Scene tempScene = EditorSceneManager.GetActiveScene();
                EditorSceneManager.SaveScene(tempScene, tempScene.path);

                Debug.Log("AddPolarizeEffect Success");
            }
        }

        [MenuItem("Tools/Stereo3D/Mix3D/Into Current Scene")]
        static void AddMix3DEffect()
        {
            Camera tempCamera = Camera.main;
            if (tempCamera != null)
            {
                AddMix3DEffect(tempCamera.gameObject);
                ConfigMouse(tempCamera);

                Scene tempScene = EditorSceneManager.GetActiveScene();
                EditorSceneManager.SaveScene(tempScene, tempScene.path);

                Debug.Log("AddMix3DEffect Success");
            }
        }

        [MenuItem("Tools/Stereo3D/Polarize/Into Build Scene")]
        static void AddPolarizeEffectToBuildScene()
        {
            List<Scene> tempLoadSceneList = null;
            List<Camera> tempGoList = GetBuildSceneObjs(out tempLoadSceneList);
            if (tempGoList != null && tempGoList.Count >= 0)
            {
                foreach (Camera tempCamera in tempGoList)
                {
                    AddPolarizeEffect(tempCamera.gameObject);
                    ConfigMouse(tempCamera);
                }
                EditorSceneManager.MarkAllScenesDirty();
                EditorSceneManager.SaveOpenScenes();
                foreach (Scene tempScene in tempLoadSceneList)
                {
                    EditorSceneManager.UnloadSceneAsync(tempScene);
                }
            }
        }

        [MenuItem("Tools/Stereo3D/Mix3D/Into Build Scene")]
        static void AddMix3DEffectToBuildScene()
        {
            List<Scene> tempLoadSceneList = null;
            List<Camera> tempGoList = GetBuildSceneObjs(out tempLoadSceneList);
            if (tempGoList != null && tempGoList.Count >= 0)
            {
                foreach (Camera tempCamera in tempGoList)
                {
                    AddMix3DEffect(tempCamera.gameObject);
                    ConfigMouse(tempCamera);
                }

                EditorSceneManager.MarkAllScenesDirty();
                EditorSceneManager.SaveOpenScenes();
                foreach (Scene tempScene in tempLoadSceneList)
                {
                    EditorSceneManager.UnloadSceneAsync(tempScene);
                }
            }
        }

        [MenuItem("Tools/Stereo3D/Remove 3D/From Current Scene")]
        static void Remove3D()
        {
            Camera tempCamera = Camera.main;
            if (tempCamera != null)
            {
                Remove3DEffect(tempCamera.gameObject);
                RemoveMouse(tempCamera);

                Scene tempScene = EditorSceneManager.GetActiveScene();
                EditorSceneManager.SaveScene(tempScene, tempScene.path);

                Debug.Log("Remove3D Success");
            }
        }


        [MenuItem("Tools/Stereo3D/Remove 3D/From Build Scene")]
        static void Remove3DFromBuildScene()
        {
            List<Scene> tempLoadSceneList = null;
            List<Camera> tempGoList = GetBuildSceneObjs(out tempLoadSceneList);
            if (tempGoList != null && tempGoList.Count >= 0)
            {
                foreach (Camera tempCamera in tempGoList)
                {
                    Remove3DEffect(tempCamera.gameObject);
                    RemoveMouse(tempCamera);
                }

                EditorSceneManager.MarkAllScenesDirty();
                EditorSceneManager.SaveOpenScenes();
                foreach (Scene tempScene in tempLoadSceneList)
                {
                    EditorSceneManager.UnloadSceneAsync(tempScene);
                }
            }
        }


        /*
        [MenuItem("Tools/Stereo3D/Update Canvas")]
        static void UpdateCanvas()
        {
            var tempArray = Selection.gameObjects;
            if (tempArray == null || tempArray.Length == 0)
            {
                return;
            }
            GameObject tempTemplateObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Framework/Stereo3D/Ext/Prefabs/CanvasMouse.prefab");
            RectTransform tempTemplateRt = tempTemplateObj.GetComponent<RectTransform>();
            Canvas tempTemplateCanvas = tempTemplateObj.GetComponent<Canvas>();

            foreach (GameObject tempObj in tempArray)
            {
                Canvas tempCanvas = tempObj.GetComponent<Canvas>();
                if (tempCanvas != null)
                {
                    tempCanvas.renderMode = tempTemplateCanvas.renderMode;

                    RectTransform tempRt = tempObj.GetComponent<RectTransform>();
                    tempRt.anchoredPosition3D = tempTemplateRt.anchoredPosition3D;
                    tempRt.sizeDelta = tempTemplateRt.sizeDelta;

                    tempRt.anchorMin = tempTemplateRt.anchorMin;
                    tempRt.anchorMax = tempTemplateRt.anchorMax;
                    tempRt.localRotation = tempTemplateRt.localRotation;
                    tempRt.localScale = tempTemplateRt.localScale;
                }
            }
        }
        */

        #endregion

        #region Protected & Public Methods
        static List<Camera> GetBuildSceneObjs(out List<Scene> varLoadSceneList)
        {
            List<string> tempScenes = new List<string>();
            varLoadSceneList = new List<Scene>();

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene tempEbss = EditorBuildSettings.scenes[i];
                if (tempEbss.enabled)
                {
                    tempScenes.Add(tempEbss.path);
                }
            }

            List<Camera> tempGoList = new List<Camera>();
            foreach (string tempScenePath in tempScenes)
            {
                Scene tempScene = EditorSceneManager.GetSceneByPath(tempScenePath);
                if (tempScene.isLoaded == false)
                {
                    tempScene = EditorSceneManager.OpenScene(tempScenePath, OpenSceneMode.Additive);
                    varLoadSceneList.Add(tempScene);
                }
                GameObject[] tempObjArray = tempScene.GetRootGameObjects();
                foreach (GameObject tempObj in tempObjArray)
                {
                    Camera[] tempCameraArray = tempObj.GetComponentsInChildren<Camera>(true);
                    if (tempCameraArray == null || tempCameraArray.Length == 0) continue;
                    foreach (Camera tempC in tempCameraArray)
                    {
                        if (tempC.CompareTag("MainCamera"))
                        {
                            tempGoList.Add(tempC);
                        }
                    }
                }
            }
            return tempGoList;
        }

        static void AddPolarizeEffect(GameObject varObj)
        {
            var tempS3 = varObj.GetComponent<Stereoskopix3D>();
            if (tempS3 != null)
            {
                Object.DestroyImmediate(tempS3);
            }

            Polarize tempP = varObj.GetComponent<Polarize>();
            if (tempP == null)
            {
                tempP = varObj.AddComponent<Polarize>();
                tempP.stereoMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Framework/Stereo3D/Polarize/Material/Polarize.mat");
            }
            tempP.GuiVisible = false;

            SwitchCamera tempSc = varObj.GetComponent<SwitchCamera>();
            if (tempSc == null)
            {
                tempSc = varObj.AddComponent<SwitchCamera>();
            }
        }

        static void AddMix3DEffect(GameObject varObj)
        {
            var tempP = varObj.GetComponent<Polarize>();
            if (tempP != null)
            {
                Object.DestroyImmediate(tempP);
            }

            Stereoskopix3D tempS3 = varObj.GetComponent<Stereoskopix3D>();
            if (tempS3 == null)
            {
                tempS3 = varObj.AddComponent<Stereoskopix3D>();
                tempS3.stereoMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/stereoskopix3Dv026/Material/stereo3DMat.mat");
            }
            tempS3.GuiVisible = false;

            SwitchCamera tempSc = varObj.GetComponent<SwitchCamera>();
            if (tempSc == null)
            {
                tempSc = varObj.AddComponent<SwitchCamera>();
            }
        }

        static void Remove3DEffect(GameObject varObj)
        {
            var tempP = varObj.GetComponent<Polarize>();
            if (tempP != null)
            {
                Object.DestroyImmediate(tempP);
            }

            Stereoskopix3D tempS3 = varObj.GetComponent<Stereoskopix3D>();
            if (tempS3 != null)
            {
                Object.DestroyImmediate(tempS3);
            }

            SwitchCamera tempSc = varObj.GetComponent<SwitchCamera>();
            if (tempSc != null)
            {
                Object.DestroyImmediate(tempSc);
            }

            PhysicsRaycaster tempPr = varObj.GetComponent<PhysicsRaycaster>();
            if (tempPr == null)
            {
                tempPr = varObj.AddComponent<PhysicsRaycaster>();
            }
            tempPr.eventMask = ~(1 << LayerMask.NameToLayer("UI"));  // 渲染除去层x的所有层
        }

        static void ConfigMouse(Camera varC)
        {
            Transform tempTrans = varC.transform;
            if (tempTrans != null)
            {
                var tempScript = tempTrans.GetComponentInChildren<MouseCtrl>();
                if (tempScript == null)
                {
                    GameObject tempObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Framework/Stereo3D/Ext/Prefabs/CanvasMouse.prefab");
                    GameObject tempClone = GameObject.Instantiate(tempObj, tempTrans);
                    tempClone.name = tempObj.name;

                }
            }
        }

        static void RemoveMouse(Camera varC)
        {
            Transform tempTrans = varC.transform;
            if (tempTrans != null)
            {
                var tempScript = tempTrans.GetComponentInChildren<MouseCtrl>();
                if (tempScript != null)
                {
                    Object.DestroyImmediate(tempScript.gameObject);
                }
            }
        }

        #endregion
    }
}