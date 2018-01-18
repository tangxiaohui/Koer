using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EUICanvas
{
    EUICanvas_Move = 1,
    EUICanvas_WordMap = 2,
    EUICanvas_Bottom = 3,
    EUICanvas_Normal = 4,
    EUICanvas_Top = 5,
}

public enum EUIType
{
    EUIType_None = 0,
    EUIType_Fight = 1,
    EUIType_Role = 2,
    EUIType_Home = 3,
    EUIType_Bag = 4,
    EUIType_Pet = 5,


    //--------分割
    EUIType_NoneBase = 9,
    EUIType_FightBase = 10,
    EUIType_RoleBase = 11,
    EUIType_HomeBase = 12,
    EUIType_BagBase = 13,
    EUIType_PetBase = 14,
    EUIType_SoloBase = 15,//单独的UI
    EUIType_LoginBase = 16, //LoginBase
    //其他类型----Other
    EUIType_Top = 20,
}

public static class EUIName
{
    public const string RegistUI = "RegistUI";
    public const string AccountUI = "AccountUI";
    public const string ResigsterSucceesUI = "ResigsterSucceesUI";
    public const string HomeUI = "HomeUI";
    public const string WallpaperUI = "WallpaperUI";
    public const string BattleUI = "BattleUI";
    public const string BattleCardShowUI = "BattleCardShowUI";
    public const string SearchEnemyUI = "SearchEnemyUI";
}