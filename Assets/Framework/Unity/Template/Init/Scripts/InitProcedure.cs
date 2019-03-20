
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
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Unity
{
    public class InitProcedure : MonoBehaviour, IProcedure
    {
        #region Fields
        public MonoBehaviour[] mLoadProcedures;
        /// <summary>
        /// 初始化流程列表
        /// </summary>
        private List<ILoadProcedure> mProcedureList = new List<ILoadProcedure>();
        private float mLoadTime;
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
            mLoadTime = Time.realtimeSinceStartup;
            Invoke("ProcedureBegin", 0.2f);
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
                mProcedureList.RemoveAt(0);
                if (mProcedureList.Count > 0)
                {
                    mProcedureList[0].ProcedureBegin();
                }
                else
                {
                    if (Time.realtimeSinceStartup - mLoadTime < 1f)
                    {
                        Invoke("ProcedureEnd", 1f - (Time.realtimeSinceStartup - mLoadTime));
                    }
                    else
                    {
                        ProcedureEnd();
                    }
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
            Scene tempS = SceneManager.GetActiveScene();
            SceneManager.LoadScene(tempS.buildIndex + 1);
        }

        public void RegisterEndCallback(Action action)
        {
        }

        public void UnRegisterEndCallback(Action action)
        {
        }
    }
}