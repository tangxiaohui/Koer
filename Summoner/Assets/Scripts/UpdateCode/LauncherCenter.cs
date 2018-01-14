using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Update.Platform;
using UpdateSystem.Manager;

namespace UpdateSystem
{
    public class LauncherCenter : MonoBehaviour
    {
        private static UpdateCenter _center;
        public static MyRuntimePlatform RuntimePlatform;

        // Use this for initialization
        void Awake()
        {
            //屏幕常亮
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#if UNITY_EDITOR
            //干掉所有缓存的Bundle,
            Caching.ClearCache();
            // 删除全部键
           // PlayerPrefs.DeleteAll();
            //保存
           // PlayerPrefs.Save();
#endif
            LauncherUpdate.Start();
        }
			
    }
}