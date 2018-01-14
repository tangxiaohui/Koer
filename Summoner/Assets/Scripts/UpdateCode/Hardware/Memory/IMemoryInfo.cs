namespace UpdateForm.Hardware
{
    /// <summary>
    /// 内存信息,单位都是MB
    /// </summary>
    public interface IMemoryInfo
    {
        //获取空闲内存,单位MB
        double GetFreeMemory();
        //获取总的内存,单位MB
        double GetSumMemory();
        //获取当前App使用的内存,单位MB
        double GetAppUsedMemory();
    }
}
