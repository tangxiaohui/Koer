using Common;
using System.Security;
using UnityEngine;
using Mono.Xml;
using System.Collections.Generic;

public class RechargeOrder : MonoBehaviour {

    static RechargeOrder _instance;

    public static RechargeOrder Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("[RechargeOrderRecorder]");
                _instance = go.AddComponent<RechargeOrder>();
            }
            return _instance;
        }
    }

    public List<string> GetAllOrder()
    {
        List<string> result = new List<string>();

        SecurityElement se;
        if (FileUtils.Exist(PathUtils.RechargeOrderPath) == false)
        {
            se = MonoXmlUtils.Create();
        }
        else
        {
            se = MonoXmlUtils.LoadXmlSE(PathUtils.RechargeOrderPath);
        }

        if (se.Children != null)
        {
            SecurityElement item;
            for (int i = 0, count = se.Children.Count; i < count; ++i)
            {
                item = se.Children[i] as SecurityElement;
                result.Add(item.Text);
            }
        }
        
        return result;
    }

    public void Add(string order)
    {
        SecurityElement se;
        if (FileUtils.Exist(PathUtils.RechargeOrderPath) == false)
        {
            se = MonoXmlUtils.Create();
        }
        else
        {
            se = MonoXmlUtils.LoadXmlSE(PathUtils.RechargeOrderPath);
        }

        SecurityElement item;
        bool found = false;
        if (se.Children != null)
        {
            for (int i = 0, count = se.Children.Count; i < count; ++i)
            {
                item = se.Children[i] as SecurityElement;
                if (item.Text.CompareTo(order) == 0)
                {
                    found = true;
                }
            }
        }

        if (found == false)
        {
            MonoXmlUtils.Add(se, "order", order);

            MonoXmlUtils.SaveXml(PathUtils.RechargeOrderPath, se);
        }
    }

    public bool Remove(string order)
    {
        
        if (FileUtils.Exist(PathUtils.RechargeOrderPath) == false)
        {
            return false;
        }

        SecurityElement se = MonoXmlUtils.LoadXmlSE(PathUtils.RechargeOrderPath);

        if (MonoXmlUtils.RemoveByValue(se, order))
        {
            MonoXmlUtils.SaveXml(PathUtils.RechargeOrderPath, se);
            return true;
        }
        else
        {
            return false;
        }
    }
}
