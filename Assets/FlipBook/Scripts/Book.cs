
#region 版权信息
/*
 * -----------------------------------------------------------
 *  Copyright (c) KeJun All rights reserved.
 * -----------------------------------------------------------
 *		描述: 
 *      创建者：陈伟超
 *      创建时间: 2018/12/21 14:10:57
 *  
 */
#endregion


using System.Collections.Generic;

namespace FlipBook
{
    public class Book
    {
        #region Fields
        private List<string> mPageContent;
        private int mPageNum;
        #endregion

        #region Properties
        public int PageNum
        {
            get { return mPageNum; }
            set
            {
                if (mPageNum != value)
                {
                    Init(value);
                }
            }
        }
        #endregion

        #region Private Methods
        private void Init(int pageNum)
        {
            mPageNum = pageNum;
            mPageContent = new List<string>();
            for (int i = 0; i < pageNum; i++)
            {
                mPageContent.Add("");
            }
        }
        #endregion

        #region Protected & Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pages">页码, 必须大于0</param>
        public Book(int pageNum)
        {
            Init(pageNum);
        }

        /// <summary>
        /// 设置页面内容
        /// </summary>
        /// <param name="pageCode"></param>
        /// <param name="text"></param>
        public void SetPageContent(int pageCode, string text)
        {
            if (pageCode >= 0 && mPageContent.Count > pageCode)
            {
                mPageContent[pageCode] = text;
            }
        }

        /// <summary>
        /// 获取页面内容
        /// </summary>
        /// <param name="pageCode"></param>
        /// <returns></returns>
        public string GetPageContent(int pageCode)
        {
            if (pageCode < 0 || mPageContent.Count <= pageCode)
            {
                //throw new System.Exception("页码越界异常");
                return null;
            }
            return mPageContent[pageCode];
        }
        #endregion
    }
}