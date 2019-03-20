
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
        [MenuItem("Tools/3D/Polarize/Into Current Scene")]
        static void AddPolarizeEffect()
        {
            Camera tempCamera = Camera.main;
            if (tempCamera != null)
            {
                AddPolarizeEffect(tempCamera.gameObject);

                Scene tempScene = EditorSceneManager.GetActiveScene();
                EditorSceneManager.SaveScene(tempScene, tempScene.path);

                Debug.Log("AddPolarizeEffect Success");
            }
        }

        [MenuItem("Tools/3D/Mix3D/Into Current Scene")]
        static void AddMix3DEffect()
        {
            Camera tempCamera = Camera.main;
            if (tempCamera != null)
            {
                AddMix3DEffect(tempCamera.gameObject);

                Scene tempScene = EditorSceneManager.GetActiveScene();
                EditorSceneManager.SaveScene(tempScene, tempScene.path);

                Debug.Log("AddMix3DEffect Success");
            }
        }

        [MenuItem("Tools/3D/Polarize/Into Build Scene")]
        static void AddPolarizeEffectToBuildScene()
        {
            List<Scene> tempLoadSceneList = null;
            List<GameObject> tempGoList = GetBuildSceneObjs(out tempLoadSceneList);
            if (tempGoList != null && tempGoList.Count >= 0)
            {
                foreach (GameObject tempObj in tempGoList)
                {
                    AddPolarizeEffect(tempObj);
                }
                EditorSceneManager.MarkAllScenesDirty();
                EditorSceneManager.SaveOpenScenes();
                foreach (Scene tempScene in tempLoadSceneList)
                {
                    EditorSceneManager.UnloadSceneAsync(tempScene);
                }
            }
        }

        [MenuItem("Tools/3D/Mix3D/Into Build Scene")]
        static void AddMix3DEffectToBuildScene()
        {
            List<Scene> tempLoadSceneList = null;
            List<GameObject> tempGoList = GetBuildSceneObjs(out tempLoadSceneList);
            if (tempGoList != null && tempGoList.Count >= 0)
            {
                foreach (GameObject tempObj in tempGoList)
                {
                    AddMix3DEffect(tempObj);
                }

                EditorSceneManager.MarkAllScenesDirty();
                EditorSceneManager.SaveOpenScenes();
                foreach (Scene tempScene in tempLoadSceneList)
                {
                    EditorSceneManager.UnloadSceneAsync(tempScene);
                }
            }
        }
        #endregion

        #region Protected & Public Methods
        static List<GameObject> GetBuildSceneObjs(out List<Scene> varLoadSceneList)
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

            List<GameObject> tempGoList = new List<GameObject>();
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
                            tempGoList.Add(tempC.gameObject);
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
                tempP.stereoMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/stereoskopix3Dv026/Polarize/Material/Polarize.mat");
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
        #endregion
    }
}