using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;

namespace UpdateSystem.Data
{
    /// <summary>
    /// 打包的资源，二进制文件内部文件结构
    /// 一个文件包含信息有： 路径长度， 文件名， md5长度， 文件大小，路径， md5值
    /// </summary>
    public class ResourceFileData
    {
        //路径长度
        public int DirLen { get; set; }
        //文件名
        public int NameLen { get; set; }
        //md5长度，32位
        public int Md5Len { get; set; }
        //文件大小
        public long FileSize { get; set; }
        //路径
        public string Dir { get; set; }
        //文件名
        public string Name { get; set; }
        //md5值
        public string Md5 { get; set; }
    }
}
