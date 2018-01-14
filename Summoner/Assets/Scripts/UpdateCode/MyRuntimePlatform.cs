namespace Update.Platform
{
    public enum MyRuntimePlatform
    {
        None,               //初始值，如果是初始值，则表示不是通过Launcher启动的
        AndroidEditor,      //编辑器下，安卓环境
        IOSEditor,          //编辑器下，IOS环境
        StandaloneEditor,   //编辑器下，windows环境
        Android,            //移动平台，安卓环境
        IOS,                //移动平台，ios环境
        Standalone          //桌面平台，Windows环境
    }
}
