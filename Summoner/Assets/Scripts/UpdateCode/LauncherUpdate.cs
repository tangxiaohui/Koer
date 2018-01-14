using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Update.Platform;

namespace UpdateSystem
{
    public static class LauncherUpdate
    {
        private static UpdateCenter _center;
        public static MyRuntimePlatform RuntimePlatform;
        public static void Start()
        {
            GameObject go = GameObject.Find("[UpdateRoot]");
            if (go != null)
            {
                if (_center == null)
                {
                    _center = go.AddComponent<UpdateCenter>();
                }
            }
        }

        /// <summary>
        /// 网络是否可用
        /// </summary>
        /// <returns></returns>
        public static bool IsNetworkEnable()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        /// <summary>
        /// 是否4G流量
        /// </summary>
        /// <returns></returns>
        public static bool Is4G()
        {
            return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
        }
    }
}