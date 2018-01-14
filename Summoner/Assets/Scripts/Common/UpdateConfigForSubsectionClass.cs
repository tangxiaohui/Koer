using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security;
using Mono.Xml;

public class UpdateConfigForSubsectionClass
{
    protected string UpdateConfigForSubsectionXML = "Config/UpdateConfigForSubsection.xml";
	protected Dictionary<int, List<string>> m_sectionDic = new Dictionary<int, List<string>>();

    public void Initalize()
    {
		string xmlPath = Common.StringUtils.CombineString(Common.PathUtils.PERSISTENT_DATA_PATH, UpdateConfigForSubsectionXML);
        if (!Common.FileUtils.Exist(xmlPath))
        {
            xmlPath = Common.StringUtils.CombineString(Common.PathUtils.STREAMING_ASSET_PATH, UpdateConfigForSubsectionXML);
        }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_IOS
        if (DevelopSetting.IsUsePersistent)
            xmlPath = xmlPath.Replace("file:///", "");
        else
            xmlPath = xmlPath.Replace("file://", "");
#elif UNITY_ANDROID
        if (DevelopSetting.IsUsePersistent)
            xmlPath = xmlPath.Replace("file:///", "");
#endif
        Mono.Xml.SecurityParser xml = Mono.Xml.MonoXmlUtils.LoadXml(xmlPath);
        if (xml != null && xml.ToXml() != null)
        {
            var dom = xml.ToXml();
			foreach(SecurityElement child in  dom.Children)
			{
				int ID = int.Parse (child.Text);
				if (m_sectionDic.ContainsKey (ID))
					m_sectionDic [ID].Add (child.Tag);
				else
					m_sectionDic.Add (ID, new List<string> (){ child.Tag });
			}
        }
    }

	public Dictionary<int, List<string>> SectionDic
    {
        get
        {
			return m_sectionDic;
        }
    }
		
    public void Deinitialization()
    {

    }
}
