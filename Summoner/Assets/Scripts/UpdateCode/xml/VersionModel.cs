using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;

namespace UpdateSystem.Xml
{
    /// <summary>
    /// 版本数据，包含从xx升级到xx的版本配置，资源下载地址，md5，filesize等
    /// </summary>
    public class VersionModel :IComparable
    {
        //低版本
        public string FromVersion { get; set; }
        //高版本
        public string ToVersion { get; set; }
        //资源地址
        public string ResourceUrl { get; set; }
        //资源md5
        public string Md5 { get; set; }
        //资源大小
        public string FileSize { get; set; }
        //map文件的md5
        public string Map_md5 { get; set; }
        //map文件的下载地址
        public string Map_url { get; set; }
        //map文件大小
        public string Map_size { get; set; }
        //资源状态，-1为未下载，0为已安装，1为已下载但是未安装
        public int State { get; set; }
        //已经下载大小
        public long DownloadedSize { get; set; }
        //第几段资源
        public string Stage { get; set; }
        //是否安装好了
        public bool Installed { get; set; }

        public int CompareTo(object obj)
        {
            int result = 0;
            try
            {
                VersionModel model = obj as VersionModel;
                if (this.ToVersion.CompareTo(model.ToVersion) < 0)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

            return result;
        }
    }
}
