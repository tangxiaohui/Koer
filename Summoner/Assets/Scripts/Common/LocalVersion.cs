using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalVersion
{
    protected string LocalVersionXML = "Config/LocalVersion.xml";
    protected string m_ip = string.Empty;
    protected string m_iport = string.Empty;
    protected string m_version = string.Empty;
    protected string m_PhotoAdress = string.Empty;
    string _chargeAddress = string.Empty;
    public void Initalize()
    {
        string xmlPath = Common.StringUtils.CombineString(Common.PathUtils.PERSISTENT_DATA_PATH, LocalVersionXML);
        if (!Common.FileUtils.Exist(xmlPath))
        {
            xmlPath = Common.StringUtils.CombineString(Common.PathUtils.STREAMING_ASSET_PATH, LocalVersionXML);
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
            m_ip = Mono.Xml.MonoXmlUtils.Parse(dom, "local_info/server_adress");
            m_iport = Mono.Xml.MonoXmlUtils.Parse(dom, "local_info/server_iport");
            m_version = Mono.Xml.MonoXmlUtils.Parse(dom, "local_info/local_app_version");
            m_PhotoAdress = Mono.Xml.MonoXmlUtils.Parse(dom, "local_info/photo_adress");
            _chargeAddress = Mono.Xml.MonoXmlUtils.Parse(dom,"local_info/charge_adress");
        }
    }

    public string ip
    {
        get
        {
            return m_ip;
        }
    }

    public string iport
    {
        get
        {
            return m_iport;
        }
    }


    public string version
    {
        get
        {
            return m_version;
        }
    }
    public string stringPhotoAdress
    {
        get
        {
            return m_PhotoAdress;
        }
    }

    public string ChargeAddress
    {
        get {
            return _chargeAddress;
        }
    }


    public void Deinitialization()
    {

    }
}
