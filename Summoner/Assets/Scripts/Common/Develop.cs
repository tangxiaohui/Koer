using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Develop : MonoBehaviour
{
#if UNITY_EDITOR
    public bool 读取LuaAB包 = false;
#else
    public bool 读取LuaAB包 = true;
#endif
    public bool 开发者模式 = false;
    public bool 热更新 = false;
    public bool 限定帧率 = false;
    public bool 显示FPS = true;
    public bool 使用Persistent = false;
    public bool 解锁所有功能 = false;
    public bool 新手引导 = false;
    public bool 剧情 = false;
    public bool 是否使用资源AB包 = false;
    public void Awake()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        DevelopSetting.isBeZip = 读取LuaAB包;
        DevelopSetting.isDevelop = 开发者模式;
        DevelopSetting.isGuide = 新手引导;
        DevelopSetting.HotFix = 热更新;
        DevelopSetting.bFPS = 限定帧率;
        DevelopSetting.ShowFPS = 显示FPS;
        DevelopSetting.IsUsePersistent = 使用Persistent;
        DevelopSetting.UnlockAllFunction = 解锁所有功能;
        DevelopSetting.IsShowStroyPush = 剧情;
        DevelopSetting.IsLoadAB = 是否使用资源AB包;
    }
}
