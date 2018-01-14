using UnityEngine;

namespace UpdateForm.Hardware
{
#if !UNITY_IPHONE
    /// <summary>
    /// android播放器
    /// </summary>
    public interface IAndroidPlayer
    {
        //初始化
        void Initialize();
        //卸载
        void Uninitialize();

        //获取当前UnityPlayer播放器
        AndroidJavaClass GetPlayer();

        //获取当前活动Activity
        AndroidJavaObject GetActivity();
       
        //获取类
        AndroidJavaObject Get(string className);
     
        //执行对象方法 --- 没有返回值
        void CallAction(AndroidJavaObject obj, string actionName, params object[] args);

        //执行对象方法 --- 有返回值
        T CallFunc<T>(AndroidJavaObject obj, string actionName, params object[] args);


        //执行静态方法 --- 没有返回值
        void CallActionStatic(string className, string actionName, params object[] args);

        //执行静态方法 --- 有返回值
        T CallFuncStatic<T>(string className, string actionName, params object[] args);

        //执行静态方法 --- 没有返回值
        void CallActionStatic(AndroidJavaObject obj, string actionName, params object[] args);

        //执行静态方法 --- 有返回值
        T CallFuncStatic<T>(AndroidJavaObject obj, string actionName, params object[] args);

    }
#endif
}
