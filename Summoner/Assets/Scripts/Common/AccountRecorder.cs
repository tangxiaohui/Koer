using Common;
using System.Security;
using UnityEngine;
using Mono.Xml;
using System.Collections.Generic;

public class AccountRecorder : MonoBehaviour {

    static AccountRecorder _instance;

    public static AccountRecorder Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("[AccountRecorder]");
                _instance = go.AddComponent<AccountRecorder>();
            }
            return _instance;
        }
    }

    public List<string> GetAllAccount()
    {
        List<string> result = new List<string>();

        SecurityElement se;
        if (FileUtils.Exist(PathUtils.AccountInfo) == false)
        {
            se = MonoXmlUtils.Create();
        }
        else
        {
            se = MonoXmlUtils.LoadXmlSE(PathUtils.AccountInfo);
        }

        if (se.Children != null)
        {
            SecurityElement item;
            string pwd;
            for (int i = 0, count = se.Children.Count; i < count; ++i)
            {
                item = se.Children[i] as SecurityElement;
                pwd = item.Attributes["pwd"] as string;
                if (string.IsNullOrEmpty(item.Text) == false && string.IsNullOrEmpty(pwd) == false)
                    result.Add(item.Text);
            }
        }
        
        return result;
    }

    public List<string> GetAllPassward()
    {
        List<string> result = new List<string>();

        SecurityElement se;
        if (FileUtils.Exist(PathUtils.AccountInfo) == false)
        {
            se = MonoXmlUtils.Create();
        }
        else
        {
            se = MonoXmlUtils.LoadXmlSE(PathUtils.AccountInfo);
        }

        if (se.Children != null)
        {
            SecurityElement item;
            string pwd;
            for (int i = 0, count = se.Children.Count; i < count; ++i)
            {
                item = se.Children[i] as SecurityElement;
                pwd = item.Attributes["pwd"] as string;
                if(string.IsNullOrEmpty(item.Text) == false && string.IsNullOrEmpty(pwd) == false)
                    result.Add(pwd);
            }
        }

        return result;
    }

    public void Add(string account,string password)
    {
        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            return;
        SecurityElement se;
        if (FileUtils.Exist(PathUtils.AccountInfo) == false)
        {
            se = MonoXmlUtils.Create();
        }
        else
        {
            se = MonoXmlUtils.LoadXmlSE(PathUtils.AccountInfo);
        }

        SecurityElement item;
        bool found = false;
        if (se.Children != null)
        {
            int index = 0;
            for (int i = 0, count = se.Children.Count; i < count; ++i)
            {
                item = se.Children[i] as SecurityElement;
                if (string.IsNullOrEmpty(item.Text) == false && item.Text.CompareTo(account) == 0)
                {
                    found = true;
                    index = i;
                    item.SetAttribute("pwd", password);
                }
            }

            if (index != 0)
            {
                ///将本次登录的账号置于第一位，下次登录默认就是这个账号了
                string median = (se.Children[0] as SecurityElement).Text;
                (se.Children[0] as SecurityElement).Text = (se.Children[index] as SecurityElement).Text;
                (se.Children[index] as SecurityElement).Text = median;
                median = (se.Children[0] as SecurityElement).Attributes["pwd"] as string;
                (se.Children[0] as SecurityElement).SetAttribute("pwd",  (se.Children[index] as SecurityElement).Attributes["pwd"] as string);
                (se.Children[index] as SecurityElement).SetAttribute("pwd", median);
            }
        }

        if (found == false)
        {
            MonoXmlUtils.AddAttr(se, account, password);       
        }
        MonoXmlUtils.SaveXml(PathUtils.AccountInfo, se);
    }

    public bool Remove(string account)
    {
        
        if (FileUtils.Exist(PathUtils.AccountInfo) == false)
        {
            return false;
        }

        SecurityElement se = MonoXmlUtils.LoadXmlSE(PathUtils.AccountInfo);

        if (MonoXmlUtils.RemoveByValue(se, account))
        {
            MonoXmlUtils.SaveXml(PathUtils.AccountInfo, se);
            return true;
        }
        else
        {
            return false;
        }
    }
}
