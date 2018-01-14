using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DevelopSetting
{
    /// <summary>
    /// 是否在开发
    /// </summary>
    public static bool isDevelop = false;
    /// <summary>
    /// 新手引导
    /// </summary>
    public static bool isGuide = false;
    public static bool ShowFPS = false;
    /// <summary>
    /// 是否读取Lua AB包
    /// </summary>
    public static bool isBeZip = false;
    /// <summary>
    /// 是否在技能编译器环境中
    /// </summary>
    public static bool isSkillEditor
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
               return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SkillEditor";
            }
#endif
            return false;
        }
    }
    /// <summary>
    /// 是否开启热更新
    /// </summary>
    public static bool HotFix = false;

    /// <summary>
    /// 是否限定帧数
    /// </summary>
    public static bool bFPS = false;

    /// <summary>
    /// 是否将资源拷贝到Persistent里面
    /// </summary>
    public static bool IsUsePersistent = false;

    /// <summary>
    /// 解锁所有功能
    /// </summary>
    public static bool UnlockAllFunction = false;
    /// <summary>
    /// 剧情
    /// </summary>
    public static bool IsShowStroyPush = false;
    /// <summary>
    /// 资源AB包
    /// </summary>
    public static bool IsLoadAB = false;
}
