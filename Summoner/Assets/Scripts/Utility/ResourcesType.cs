
namespace Res
{
    public enum ClassType
    {
        Texture = 0,
        Sprite = 1,
        AudioClip = 2,
        GameObject = 3,
        Material = 4,
        TextAsset = 5,
        AssetBundle = 6,
        Object = 7,            //其他
    }

    public enum SceneType
    {
        Start = 0,
        Game = 1,
    }

    public enum ResLocation
    {
        Resources = 0,      
        StreamingAssets = 1,
        PersistenData = 2,
    }

    public class ResInfo
    {
        public string ResName;
        public ResLocation Location = ResLocation.Resources;
        public string ResPath;

    }

    public enum ResourceType
    {
        Texture = 0,            //
        Sprite = 1,             // 
        Audio = 2,              //
        Lua = 3,                // 
        Config = 4,             // 配置表
        UI = 5,
        Scene = 6,
        Material = 7,
        SpineJson = 8,          //spine的主要数据文件
        SpineAltasTxt = 9,      //spine的图集文件
        SpineAltasData = 10,     //spine自己的图集数据文件
        SpineBoneData = 11,      //spine骨骼数据
        Map = 12,//地图
        AssetBundle = 13,//AB
        Role3D = 14,
        ///以下类型认为需要对象池
        Role = 16,
        Fx = 17,                 //特效
        Pet = 18,
    }

    public enum IconType
    {
        Equipment = 0,            //
        Skill = 1,             // 
        Item = 2,              //
        Quality = 3,
        Quest = 4,
        GuildQuest = 5,
        Recharge = 6,
        HeadIcon = 7
    }

    public enum AltasType
    {
        Equipment = 0,            
        Skill = 1,             
        Item = 2,              
        Quality = 3,       
        Quest = 4,
        GuildQuest = 5,
        Recharge = 6,
        HeadIcon = 7,
		StoryIcon = 8,
        GuildBossIcon = 9,
        PetCards1 = 20,
        PetCards2 = 21,
        PetCards3 = 22,
        FashionCards = 23,
        HeronForm = 24,
        HeroineForm = 25,
        RankIcon = 26,
        //99以后都用于Loading图
        LoadingForm = 99,
    }
}