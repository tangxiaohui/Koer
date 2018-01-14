namespace UpdateSystem.Enum
{
    /// <summary>
    /// 用来定义转换函数
    /// </summary>
    public enum ConvertFuncEnum
    {
        TransFunc,              //onTransResourceFinishCallback
        DownloadClientFunc,     //onClientDownloadFinishCallback
        NoticeFunc,             //onDownloadNoticeCall
        FinishFunc,             //onFinishCallback
        ActionCallFunc,         //onActionCall
        CheckResFunc,           //onMyActionCallback
    }
}
