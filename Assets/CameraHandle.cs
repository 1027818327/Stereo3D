
#region 版权信息
/*
 * -----------------------------------------------------------
 *  Copyright (c) KeJun All rights reserved.
 * -----------------------------------------------------------
 *		描述: 
 *      创建者：陈伟超
 *      创建时间: 2019/04/26 23:31:37
 *  
 */
#endregion


using UnityEngine;

namespace Assets
{
    public class CameraHandle : MonoBehaviour
    {
        #region Fields
        private Camera mCamera;
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
            mCamera = GetComponent<Camera>();
        }
        //    
        void Update()
        {
            //if (Input.GetMouseButtonDown(0))
            {
                //判断是否是点击事件
                Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
                

                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    //Debug.Log(hitInfo.transform);
                    Debug.DrawRay(ray.origin, hitInfo.point, Color.red);//划出射线，在scene视图中能看到由摄像机发射出的射线
                }

                
            }
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

        #endregion

        #region Protected & Public Methods

        #endregion
    }
}