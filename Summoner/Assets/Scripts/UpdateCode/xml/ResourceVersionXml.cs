using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using UpdateSystem.Xml;

namespace UpdateSystem.Xml
{
    /// <summary>
    /// 服务器端配置的xml，数据类
    /// </summary>
    public partial class ResourceVersionXml
    {
        //正式流程数据
        internal DataModel NormalFollow { get; set;}
        //测试流程
        internal DataModel TestFollow { get; set;}
        //白名单：mac地址、或机器码
        public List<string> WhiteCode { get; set;}
        //白名单：用户名或版本号
        public List<string> WhiteUsers { get; set;}
        //白名单：ip地址
        public List<string> WhiteIp { get; set; }

        public ResourceVersionXml()
        {
            NormalFollow = new DataModel();
            TestFollow = new DataModel();
            WhiteCode = new List<string>();
            WhiteUsers = new List<string>();
            WhiteIp = new List<string>();
        }
        
    }
}
