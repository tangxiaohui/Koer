using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UpdateForm.Hardware
{
#if !UNITY_IPHONE
    /// <summary>
    /// android调用的处理
    /// </summary>
    public class AndroidPlayer : IAndroidPlayer
    {
        //播放器类的路径
        private const string CN_UNITY_PLAYER_CLASS_NAME = "com.unity3d.player.UnityPlayer";
        //当前活动页的静态字段名
        private const string CN_CURRENT_ACTIVITY_FIELD_NAME = "currentActivity";

        //静态对象
        private static IAndroidPlayer _shareInstance = null;
        //Android的实例
        public static IAndroidPlayer ShareInstance
        {
            get
            {
                if (_shareInstance == null)
                {
                    _shareInstance = new AndroidPlayer();
                    _shareInstance.Initialize();
                }
                return _shareInstance;
            }
 
        }

        //Unity在Android下的播放器
        private AndroidJavaClass _player;
        //播放器的Activity
        private AndroidJavaObject _activity;
        //类的存储器
        private Dictionary<string, AndroidJavaObject> _classStore = new Dictionary<string, AndroidJavaObject>();

        //初始化处理
        public void Initialize()
        {
            if (_player==null)
                _player = new AndroidJavaClass(CN_UNITY_PLAYER_CLASS_NAME);

            if(_activity == null && _player != null)
                _activity = _player.GetStatic<AndroidJavaObject>(CN_CURRENT_ACTIVITY_FIELD_NAME);
        }

        //卸载
        public void Uninitialize()
        {
            if (_classStore.Values.Count > 0)
            {
                foreach (AndroidJavaObject jo in _classStore.Values)
                {
                    jo.Dispose();
                }

                _classStore.Clear();
            }

            if (_activity != null)
            {
                _activity.Dispose();
            }

            if (_player != null)
            {
                _player.Dispose();
            }
        }

        //获取UnityPlayer
        public AndroidJavaClass GetPlayer()
        {
            if (_player == null)
            {
                _player = new AndroidJavaClass(CN_UNITY_PLAYER_CLASS_NAME);
            }
            return _player;
        }

        //获取当前Activeity
        public AndroidJavaObject GetActivity()
        {
            if (_activity == null)
            {
                _activity = GetPlayer().GetStatic<AndroidJavaObject>(CN_CURRENT_ACTIVITY_FIELD_NAME);
            }
            return _activity;
        }       

        //通过类名获取对象
        public AndroidJavaObject Get(string className = null)
        {
            if (string.IsNullOrEmpty(className))
            {
                return GetActivity();
            }
            else
            {
                AndroidJavaObject jo;
                if (!_classStore.TryGetValue(className, out jo))
                {
                    jo = new AndroidJavaClass(className);
                    _classStore[className] = jo;
                }
                return jo;
            }
        }

        //调用静态方法 -- 不返回值
        public void CallActionStatic(string className, string actionName, params object[] args)
        {
            CallActionStatic(Get(className), actionName, args);
        }

        //调用静态方法 -- 返回值
        public T CallFuncStatic<T>(string className, string actionName, params object[] args)
        {
            UnityEngine.Debug.Log("CallFuncStatic: " + className + " func: " + actionName);
            return CallFuncStatic<T>(Get(className), actionName, args);
        }

        //调用对象方法 -- 不返回值
        public void CallAction(AndroidJavaObject obj, string actionName, params object[] args)
        {
            try
            {
                obj.Call(actionName, args);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        //调用对象方法 -- 返回值
        public T CallFunc<T>(AndroidJavaObject obj, string actionName, params object[] args)
        {
            try
            {
                return obj.Call<T>(actionName, args);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return default(T);
        }

        //调用静态方法 -- 不返回值
        public void CallActionStatic(AndroidJavaObject obj, string actionName, params object[] args)
        {
            try
            {
                obj.CallStatic(actionName, args);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        //调用静态方法 -- 返回值
        public T CallFuncStatic<T>(AndroidJavaObject obj, string actionName, params object[] args)
        {
            try
            {
                UnityEngine.Debug.Log("CallFuncStatic: " + obj + " func: " + actionName);
                return obj.CallStatic<T>(actionName, args);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("CallFuncStatic: error " + obj + " func: " + actionName);
                UnityEngine.Debug.LogError(ex);
                Debug.LogException(ex);
            }
            return default(T);
        }

       
    }
#endif
}
