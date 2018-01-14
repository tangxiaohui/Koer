using Mono.Xml;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading;
using UpdateSystem.Flow;
using UpdateSystem.Log;

public class ServerNoticeXml
{
    protected string ServerNoticeXmlPath = "Config/ServerNotice.xml";
    protected string m_notice_title = string.Empty;
    protected string m_notice_content = string.Empty;
    protected string m_account_notice_title = string.Empty;
    protected string m_account_notice_content = string.Empty;
    public string notice_title
    {
        get
        {
            return m_notice_title;
        }
        set
        {
            m_notice_title = value;
        }
    }

    public string notice_content
    {
        get
        {
            return m_notice_content;
        }
        set
        {
            m_notice_content = value;
        }
    }

    public string account_notice_title
    {
        get
        {
            return m_account_notice_title;
        }
        set
        {
            m_account_notice_title = value;
        }
    }

    public string account_notice_content
    {
        get
        {
            return m_account_notice_content;
        }
        set
        {
            m_account_notice_content = value;
        }
    }


    string _TAG = "ServerNoticeXml.cs ";
    SecurityElement dom = null;
    string _localVersionXml;

    public void Initalize()
    {
        string xmlPath = Common.StringUtils.CombineString(Common.PathUtils.PERSISTENT_DATA_PATH, ServerNoticeXmlPath);
        if (!Common.FileUtils.Exist(xmlPath))
        {
            xmlPath = Common.StringUtils.CombineString(Common.PathUtils.STREAMING_ASSET_PATH, ServerNoticeXmlPath);
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
            parse(xml.ToXml());
        }
    }

    private string parse(SecurityElement dom, string domPath)
    {
        if (dom == null || string.IsNullOrEmpty(domPath))
        {
            return string.Empty;
        }
        SecurityElement node = dom;
        var subpaths = domPath.Split('/');
        for (int i = 1; node != null && i < subpaths.Length; ++i)
        {
            var tag = subpaths[i];
            node = node.SearchForChildByTag(tag);
        }
        return node != null ? node.Text : string.Empty;
    }

    private void parse(SecurityElement dom)
    {
        m_notice_title = parse(dom, "local_info/game_notice_title");
        m_notice_content = parse(dom, "local_info/game_notice_content");
        m_account_notice_title = parse(dom, "local_info/account_notice_title");
        m_account_notice_content = parse(dom, "local_info/account_notice_content");
    }

    private bool set(SecurityElement dom, string domPath, string value)
    {
        if (dom == null || string.IsNullOrEmpty(domPath))
        {
            return false;
        }
        SecurityElement node = dom;
        var subpaths = domPath.Split('/');
        for (int i = 1; node != null && i < subpaths.Length; ++i)
        {
            var tag = subpaths[i];
            node = node.SearchForChildByTag(tag);
        }
        if (node != null)
        {
            node.Text = value;
            return true;
        }
        return false;
    }
}