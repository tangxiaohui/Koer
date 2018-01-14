using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

public class TextManager : SingleInstance<TextManager>
{

    public static readonly string DEFALUT_LANG = "TEXT.lang";
    public static readonly string[] TEXT_SEPARATOR = { "@$@" };

    private static string[] m_textContents = null;
	private static bool m_isInit = false;

    private Dictionary<string, string> m_dicTectContents = null;
    public override void Initalize()
    {
        LoadText();
    }

    public override void Deinitialization()
    {
        m_dicTectContents.Clear();
        m_textContents = null;
        m_isInit = false;
        
    }

    private void LoadText()
	{
        if(!m_isInit)
        {
            if (m_textContents == null)
            {
                m_textContents = new string[TEXTS.TEXTS_TOTAL_NUM];
            }
            string textFile = DEFALUT_LANG;
           var textFilePath = Common.StringUtils.CombineString(PathUtils.PERSISTENT_DATA_PATH, "Texts/", textFile);
            if (!FileUtils.Exist(textFilePath))
            {
                textFilePath = Common.StringUtils.CombineString(PathUtils.STREAMING_ASSET_PATH, "Texts/", textFile);
            }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_IOS
            if (DevelopSetting.IsUsePersistent)
                textFilePath = textFilePath.Replace("file:///", "");
            else
                textFilePath = textFilePath.Replace("file://", "");
#elif UNITY_ANDROID
            if (DevelopSetting.IsUsePersistent)
                textFilePath = textFilePath.Replace("file:///", "");
#endif
            var filelength = 0L;
            var data = FileUtils.OpenFile (textFilePath, out filelength);
		    if (data != null) 
		    {
                m_dicTectContents = new Dictionary<string, string>(TEXTS.TEXTS_TOTAL_NUM + 1);
                string textContents = System.Text.Encoding.UTF8.GetString (data).Trim ();
                string[] text = textContents.Split(TEXT_SEPARATOR, System.StringSplitOptions.RemoveEmptyEntries);
			    if (text.Length == TEXTS.TEXTS_TOTAL_NUM * 2) 
                {
				    for (int i = 0; i < TEXTS.TEXTS_TOTAL_NUM; ++i) 
                    {
					    m_textContents [i] = text [i * 2 + 1];
                        if (!m_dicTectContents.ContainsKey(text[i * 2]))
                        {
                            m_dicTectContents.Add(text[i * 2], m_textContents[i]);
                        }
#if _DEBUG_INFO
                        else
                        {
                            Common.UDebug.LogError("Text has the same key : " + text[i * 2]);
                        }
#endif
				    }
				    m_isInit = true;
                } else {
                    Common.UDebug.Assert(false, " text resources error ,please check the text files: " + textFile);
			    }
                text = null;
                textContents = null;
            }
		}
	}
	
	public string GetString(int index)
	{	
		if (!m_isInit) 
        {
            Common.UDebug.LogError("TextManger not init!");
			return string.Empty;
		}
		if (index < 0 || index >= TEXTS.TEXTS_TOTAL_NUM) {
            Common.UDebug.Assert(false, " GetString index out range : " + TEXTS.TEXTS_TOTAL_NUM);
			return string.Empty;
		}
		return m_textContents [index];
	}

    public string GetStringByName(string index)
    {
        if (!m_isInit)
        {
            Common.UDebug.LogError("TextManger not init!");
            return string.Empty;
        }
        string value = string.Empty;
        if (m_dicTectContents.TryGetValue(index, out value))
        {
            return value;
        }
        return string.Empty;
    }

    //fontCacheTextLoading
    #region
    public string GetUsedText()
    {
	    string m_gameString = "";
        var textFilePath = "Texts/texts_loading.lang";
        textFilePath = ResourcesUtils.GetAssetRealPath(textFilePath);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_IOS
        if (DevelopSetting.IsUsePersistent)
            textFilePath = textFilePath.Replace("file:///", "");
        else
            textFilePath = textFilePath.Replace("file://", "");
#elif UNITY_ANDROID
        if (DevelopSetting.IsUsePersistent)
            textFilePath = textFilePath.Replace("file:///", "");
#endif
        var filelength = 0L;
        var data = FileUtils.OpenFile (textFilePath, out filelength);
        if (data != null && filelength != 0L)
        {
               string textContents = System.Text.Encoding.UTF8.GetString(data);
               m_gameString = textContents;
         }
        return m_gameString;
    }
    #endregion

    //public string GetStringByName(string name)
	//{
	//	return GetStringByName(name);
	//}
}
