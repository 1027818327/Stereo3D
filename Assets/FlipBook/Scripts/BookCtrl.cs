
#region 版权信息
/*
 * -----------------------------------------------------------
 *  Copyright (c) KeJun All rights reserved.
 * -----------------------------------------------------------
 *		描述: 
 *      创建者：陈伟超
 *      创建时间: 2018/12/12
 *  
 */
#endregion


using Framework.Unity.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UI;
using UTJ.Alembic;

namespace FlipBook
{
    public class BookCtrl : MonoBehaviour
    {
        #region Fields
        private AlembicStreamPlayer mAsp;
        private EventSystem mEventSystem;
        private Text mTempText;

        /// <summary>
        /// 上一页
        /// </summary>
        public PlayableDirector mFrontPd;
        /// <summary>
        /// 下一页
        /// </summary>
        public PlayableDirector mNextPd;

        /// <summary>
        /// 打开书本
        /// </summary>
        public PlayableDirector mOpenPd;
        /// <summary>
        /// 关闭书本
        /// </summary>
        public PlayableDirector mClosePd;

        /// <summary>
        /// 当前第几页，[0, Max + 1]
        /// </summary>
        private int mCurPage = -1;
        public Book mBook;

        public Text mOpenText1;
        public Text mOpenText2;
        public Text mOpenPageText1;
        public Text mOpenPageText2;

        public Text mNextText1;
        public Text mNextText2;
        public Text mNextPageText1;
        public Text mNextPageText2;

        public Text mPreText1;
        public Text mPreText2;
        public Text mPrePageText1;
        public Text mPrePageText2;

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
        //void Start()
        //{

        //}
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            var tempScript = GameObject.FindObjectOfType<EventSystem>();
            if (tempScript == null)
            {
                var tempObj = new GameObject("EventSystem");
                tempObj.AddComponent<EventSystem>();
                tempObj.AddComponent<StandaloneInputModule>();
            }
        }
#endif

        #endregion

        #region Private Methods

        private void RestoreBook()
        {
            OpenEvent();
            RestoreOpenText();
        }

        private void RestoreOpenText()
        {
            RefreshLeftPage();
            RefreshRightPage();
        }

        private void CloseEvent()
        {
            mEventSystem = EventSystem.current;
            if (mEventSystem != null)
            {
                mEventSystem.enabled = false;
            }
        }

        private void OpenEvent()
        {
            if (mEventSystem != null)
            {
                mEventSystem.enabled = true;
            }
        }

        private void ResetLeftPageAnim()
        {
            mFrontPd.gameObject.SetActive(false);
            /*
            var tempScript = mFrontPd.GetComponent<AlembicStreamPlayer>();
            if (tempScript != null)
            {
                tempScript.currentTime = 0f;
            }
            */
        }

        private void ResetRightPageAnim()
        {
            mNextPd.gameObject.SetActive(false);
            /*
            var tempScript = mNextPd.GetComponent<AlembicStreamPlayer>();
            if (tempScript != null)
            {
                tempScript.currentTime = 0f;
            }
            */
        }

        private void RefreshLeftPage()
        {
            mOpenText1.text = mBook.GetPageContent(mCurPage);
            mOpenPageText1.text = (mCurPage + 1).ToString();
        }

        private void RefreshRightPage()
        {
            mOpenText2.text = mBook.GetPageContent(mCurPage + 1);
            mOpenPageText2.text = (mCurPage + 2).ToString();
        }

        #endregion

        #region Protected & Public Methods
        [ContextMenu("播放打开书本动画")]
        public void PlayOpenBook()
        {
            if (mOpenPd != null)
            {
                mOpenPd.Play();

                CloseEvent();
                Invoke("OpenEvent", (float)(mOpenPd.duration - mOpenPd.initialTime));
            }

            mCurPage = 0;
            /// 显示左页和右页内容
            RestoreOpenText();
        }

        [ContextMenu("播放关闭书本动画")]
        public void PlayCloseBook()
        {
            if (mCurPage < 0)
            {
                Debuger.LogError("当前书本已经关闭");
                return;
            }

            if (mClosePd != null)
            {
                mClosePd.Play();
                CloseEvent();
                Invoke("OpenEvent", (float)(mClosePd.duration - mClosePd.initialTime));
            }

            if (mCurPage >= 0)
            {
                RestoreOpenText();
            }

            mCurPage = -1;
            /// 文本清空
            mNextText1.text = null;
            mNextText2.text = null;
            mPreText1.text = null;
            mPreText2.text = null;
        }


        [ContextMenu("播放上一页动画")]
        public void PlayFront()
        {
            if (mCurPage < 0)
            {
                Debuger.LogError("请先打开书，才能翻一页");
                return;
            }

            if (mCurPage > 0)
            {
                float tempIntervalTime = (float)(mFrontPd.duration - mFrontPd.initialTime);
                Invoke("RestoreBook", tempIntervalTime);
                CloseEvent();

                /// 将要翻动的左页显示
                mFrontPd.gameObject.SetActive(true);
                mFrontPd.Play();
                Invoke("ResetLeftPageAnim", tempIntervalTime);

                RestoreOpenText();
                /// 要翻动的左页文字刷新
                mPreText1.text = mBook.GetPageContent(mCurPage - 1);
                mPreText2.text = mBook.GetPageContent(mCurPage);
                mPrePageText1.text = (mCurPage).ToString();
                mPrePageText2.text = (mCurPage + 1).ToString();
                Invoke("RefreshLeftPage", 0.15f);
                Invoke("RefreshRightPage", tempIntervalTime - 0.15f);

                mCurPage -= 2;
                if (mCurPage < 0)
                {
                    mCurPage = 0;
                }
            }
        }

        [ContextMenu("播放下一页动画")]
        public void PlayNext()
        {
            if (mCurPage < 0)
            {
                Debuger.LogError("请先打开书，才能翻一页");
                return;
            }

            if (mCurPage >= mBook.PageNum - 2)
            {
                return;
            }

            float tempIntervalTime = (float)(mNextPd.duration - mNextPd.initialTime);
            Invoke("RestoreBook", tempIntervalTime);
            CloseEvent();

            /// 将要翻动的右页显示
            mNextPd.gameObject.SetActive(true);
            mNextPd.Play();
            Invoke("ResetRightPageAnim", tempIntervalTime);

            RestoreOpenText();

            /// 要翻动的右页文字刷新
            mNextText1.text = mBook.GetPageContent(mCurPage + 1);
            mNextText2.text = mBook.GetPageContent(mCurPage + 2);
            mNextPageText1.text = (mCurPage + 2).ToString();
            mNextPageText2.text = (mCurPage + 3).ToString();
            Invoke("RefreshRightPage", 0.15f);
            Invoke("RefreshLeftPage", tempIntervalTime - 0.15f);

            mCurPage += 2;
            bool tempIsLast = mCurPage >= mBook.PageNum;
            if (tempIsLast)
            {
                mCurPage = mBook.PageNum;
            }
        }

        public List<string> GetGontent(Text text, string content)
        {
            List<string> tempList = new List<string>();
            while (content != null && content.Length != 0)
            {
                text.text = content;
                text.cachedTextGenerator.Populate(text.text, text.GetGenerationSettings(text.rectTransform.rect.size));
                int tempCount = text.cachedTextGenerator.vertexCount / 4 - 1;
                string tempPageContent = content.Substring(0, tempCount);
                tempList.Add(tempPageContent);

                content = content.Substring(tempCount);
            }
            text.text = null;
            return tempList;
        }

        public void SetBookContent(string text)
        {
            if (mTempText == null)
            {
                mTempText = GameObjectUtils.FindComponent<Text>(gameObject, "TempCanvas/Text");
            }

            List<string> tempList = GetGontent(mTempText, text);
            mBook = new Book(tempList.Count);
            for (int i = 0; i < tempList.Count; i++)
            {
                mBook.SetPageContent(i, tempList[i]);
            }
        }
        #endregion
    }
}