using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;

namespace UpdateSystem.Xml
{
    /// <summary>
    /// 数据模型， 
    /// </summary>
    public class DataModel
    {
        //老凡仙用的，服务器ip，用于返回玩家本地ip
        public string ServerIp { get; set; }
        public string ServerPort { get; set; }
        //LSIP
        public string LoginIp{ get; set; }
        public string LoginPort{ get; set; }
        //执行端下载地址
        public string ClientUrl{ get; set; }
        //基础资源列表
        internal List<VersionModel> VersionModelBaseList{ get; set; }
        //补丁资源列表
        internal List<VersionModel> VersionModelPatchList{ get; set; }
        //语言
        public string Language{ get; set; }
        //最新的执行端版本
        public string AppVersion{ get; set; }
        //大版本号
        public string BigAppVersion{ get; set; }
        //小版本号
        public string SmallAppVersion{ get; set; }
        //执行端大小
        public string AppSize{ get; set; }
        //服务器列表，原来的设计，现在不用了
        public string ServerList { get; set; }
        //最新的补丁版本
        public string PatchVersion { get; set; }
        //是否使用强制更新，默认为true
        public bool EnableForceUpdate { get; set; }

        public DataModel()
        {
            EnableForceUpdate = true;
            VersionModelBaseList = new List<VersionModel>();
            VersionModelPatchList = new List<VersionModel>();
        }

    }
}
