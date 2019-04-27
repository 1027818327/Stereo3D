
#region 版权信息
/*
 * -----------------------------------------------------------
 *  Copyright (c) KeJun All rights reserved.
 * -----------------------------------------------------------
 *		描述: 
 *      创建者：陈伟超
 *      创建时间: 2019/02/14 15:49:36
 *  
 */
#endregion

using System;
using System.Collections.Generic;
using Framework.Procedure;
using Framework.Unity.MultiLanguage;
using Framework.Unity.Tools;
using Framework.Unity.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Unity
{
    public class InitProcedure : MonoBehaviour, IProcedure
    {
        #region Fields
        public MonoBehaviour[] mLoadProcedures;
        public UISimpleLoading initLoading;

        /// <summary>
        /// 初始化流程列表
        /// </summary>
        private List<ILoadProcedure> mProcedureList = new List<ILoadProcedure>();

        private int totalProcedures;
        private int curProcedures;

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
        void Start()
        {
            if (initLoading != null)
            {
                initLoading.InitOpenArgs(null);
                initLoading.ShowProgress(0f);
            }
            
            TimeManager.Instance.AddOneFrameTask(false, ProcedureBegin);
        }
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
        private void OneLoadProcedureEnd()
        {
            if (mProcedureList.Count > 0)
            {
                curProcedures++;

                if (initLoading != null)
                {
                    float tempProgress = (float)curProcedures / (float)totalProcedures;
                    if (mProcedureList[0] as LanguageManager)
                    {
                        initLoading.ShowProgress(LanguageManager.mInstance.GetValue("加载数据"), tempProgress);
                    }
                    else
                    {
                        initLoading.ShowProgress(tempProgress);
                    }
                }
                
                mProcedureList.RemoveAt(0);
                if (mProcedureList.Count > 0)
                {
                    mProcedureList[0].ProcedureBegin();
                }
                else
                {
                    ProcedureEnd();
                }
            }
        }

        #endregion

        #region Protected & Public Methods

        #endregion
        public void ProcedureBegin()
        {
            int tempCount = transform.childCount;
            for (int i = 0; i < mLoadProcedures.Length; i++)
            {
                var tempProcedure = mLoadProcedures[i] as ILoadProcedure;
                if (tempProcedure != null)
                {
                    mProcedureList.Add(tempProcedure);
                }
            }

            totalProcedures = mProcedureList.Count;
            curProcedures = 0;

            if (mProcedureList.Count > 0)
            {
                for (int i = 0; i < mProcedureList.Count; i++)
                {
                    mProcedureList[i].RegisterEndCallback(OneLoadProcedureEnd);
                }
                mProcedureList[0].ProcedureBegin();
            }
        }

        public void ProcedureEnd()
        {
            if (initLoading != null)
            {
                initLoading.gameObject.SetActive(false);
            }
            
            Scene tempS = SceneManager.GetActiveScene();
            UIManager.GetInstance().LoadScene(tempS.buildIndex + 1);
        }

        public void RegisterEndCallback(Action action)
        {
        }

        public void UnRegisterEndCallback(Action action)
        {
        }
    }
}