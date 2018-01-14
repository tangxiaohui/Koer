namespace UpdateForm.Center
{
    /// <summary>
    /// 打包的时候动态修改这个文件，设置app版本和分段版本
    /// </summary>
    public static class BuildVersion
    {
        //TODO::Update 每次打包时修改版本，如果有自己的版本写入方式，则请把AppVer所调用的地方做替换
        /// <summary>
        /// 包内app版本
        /// </summary>
        public static string AppVer = "1.0.0";
    }
}
