using Framework.Debugger;
using Framework.Runtime;
using Framework.Unity.Tools;
using Framework.Unity.UI;
using UnityEngine;

namespace Framework.Unity
{
    // 主程序启动类
    public class AppMain : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            GameObject tempSingleObj = new GameObject(typeof(AppMain).Name);
            //tempSingleObj.hideFlags = HideFlags.HideInHierarchy;
            AppMain tempScript = tempSingleObj.AddComponent<AppMain>();
            GameObject.DontDestroyOnLoad(tempSingleObj);


#if DEVELOPMENT_BUILD || UNITY_EDITOR
            tempScript.OpenLog();
#else
            tempScript.CloseLog();
#endif
        }

        private void Awake()
        {
            // 其他需要初始化的功能都放在这个函数
            {
                var tempInstance = MonoHelper.Instance;
            }
        }

        private void OpenLog()
        {
            Debuger.EnableLog = true;
#if UNITY_2017_1_OR_NEWER
            Debug.unityLogger.logEnabled = true;
#else
            Debug.logger.logEnabled = true;
#endif
            gameObject.AddComponent<DebuggerComponent>();
        }

        private void CloseLog()
        {
            Debuger.EnableLog = false;
#if UNITY_2017_1_OR_NEWER
            Debug.unityLogger.logEnabled = false;
#else
            Debug.logger.logEnabled = false;
#endif
        }
    }
}