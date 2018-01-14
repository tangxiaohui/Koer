using System;
using System.Collections.Generic;
using System.Text;
using UpdateSystem.Xml;
using UpdateSystem.Log;

namespace UpdateSystem.Flow
{
    /// <summary>
    /// 2. 解析本地LocalVersion.xml的流程
    /// </summary>
    public class Flow2LocalXml : BaseFlow
    {

        public override int Work()
        {
            if (!CheckLastFlowResult()) return LastFlowResult;
            string localXmlPath = _localXmlPath;
            return parseLocalVersion(localXmlPath);
        }

        public override void Uninitialize()
        {
            LocalXml = null;
        }

        //解析localversion
        public int parseLocalVersion(string localXmlPath)
        {
            int ret = CodeDefine.RET_SUCCESS;
            LocalXml = new LocalVersionXml();
            ret = LocalXml.parseLocalVersionXml(localXmlPath);
            if (ret >= CodeDefine.RET_SUCCESS)
            {
                LocalBaseResVersion = LocalXml.BaseResVersion;
            }
            return ret;
        }
    }
}
