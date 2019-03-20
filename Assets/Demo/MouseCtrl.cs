
#region 版权信息
/*
 * -----------------------------------------------------------
 *  Copyright (c) KeJun All rights reserved.
 * -----------------------------------------------------------
 *		描述: 
 *      创建者：DESKTOP-1050N1H\luoyikun
 *      创建时间: 2019/03/20 12:31:29
 *  
 */
#endregion


using UnityEngine;

namespace Assets.Demo
{
    public class MouseCtrl : MonoBehaviour
    {
        #region Fields
        public bool mShowCursor;
        public RectTransform mMouseTrans;
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
            if (mShowCursor)
            {
                ShowCursor();
            }
            else
            {
                HideCursor();
            }
        }
        //    
        void Update()
        {
            if (mMouseTrans == null)
            {
                return;
            }
            mMouseTrans.anchoredPosition = Input.mousePosition;
        }
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
        [ContextMenu("ShowCursor")]
        private void ShowCursor()
        {
            Cursor.visible = true;
        }

        [ContextMenu("HideCursor")]
        private void HideCursor()
        {
            Cursor.visible = false;
        }
        #endregion

        #region Protected & Public Methods

        #endregion
    }
}