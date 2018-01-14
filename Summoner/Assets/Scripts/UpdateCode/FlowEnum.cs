namespace UpdateSystem.Enum
{
    /// <summary>
    /// 流程定义
    /// </summary>
    public enum FlowEnum
    {
        /// <summary>
        /// 资源转移
        /// </summary>
        Flow1TransResource,
        /// <summary>
        /// 解析LocalXml
        /// </summary>
        Flow2LocalXml,
        /// <summary>
        /// 下载RemoteXml
        /// </summary>
        Flow3RemoteXml,
        /// <summary>
        /// 下载客户端
        /// </summary>
        Flow4DownloadClient,
        /// <summary>
        /// 下载分段资源
        /// </summary>
        Flow5DownloadBaseRes,
        /// <summary>
        /// 释放分段资源
        /// </summary>
        Flow6ReleaseBaseRes,
        /// <summary>
        /// 下载分段描述文件
        /// </summary>
        Flow7DownloadMapFile,
        Flow7ExDownloadMapFile,
        /// <summary>
        /// 资源检查
        /// </summary>
        Flow8CheckResource,
        Flow8ExCheckResource,
        /// <summary>
        /// 资源修复
        /// </summary>
        Flow9RepairResource,
        /// <summary>
        /// 流程结束
        /// </summary>
        FlowFinish,
    }
}
