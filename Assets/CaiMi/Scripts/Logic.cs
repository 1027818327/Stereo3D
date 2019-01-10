
#region 版权信息
/*
 * -----------------------------------------------------------
 *  Copyright (c) KeJun All rights reserved.
 * -----------------------------------------------------------
 *		描述: 
 *      创建者：DESKTOP-1050N1H\luoyikun
 *      创建时间: 2019/01/02 11:59:07
 *  
 */
#endregion


using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

namespace CaiMi
{
    public class Logic : MonoBehaviour
    {
        #region Fields
        public VideoPlayer mVideoPlayer;
        /// <summary>
        /// 电视机
        /// </summary>
        public GameObject mDianShiJi;
        /// <summary>
        ///  蜜蜂
        /// </summary>
        public GameObject mBee;
        /// <summary>
        /// 花
        /// </summary>
        public GameObject mFlower;

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
            mVideoPlayer.prepareCompleted += PrepareCompleted;
            mVideoPlayer.Play();
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
        private void PrepareCompleted(VideoPlayer source)
        {
            var tempRender = mVideoPlayer.targetMaterialRenderer;
            var tempMats = tempRender.materials;
            foreach (Material tempMat in tempMats)
            {
                tempMat.color = Color.white;
            }
            tempRender.materials = tempMats;

            PlayableDirector tempPd = GetComponent<PlayableDirector>();
            if (tempPd != null)
            {
                tempPd.Play();
            }
        }
        #endregion

        #region Protected & Public Methods
        public void Restart()
        {
            mVideoPlayer.Stop();
            mVideoPlayer.Play();

            PlayableDirector tempPd = GetComponent<PlayableDirector>();
            if (tempPd != null)
            {
                tempPd.Stop();
                tempPd.initialTime = 0f;
            }
        }
        #endregion
    }
}