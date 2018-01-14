using Common;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnimCurvesConfig
{
    private static Dictionary<string, AnimCurvesConfig> m_DicDatas = null;
    public static Dictionary<string, AnimCurvesConfig> DicDatas
    {
        get
        {
            Load();
            return m_DicDatas;
        }
    }

    private static List<AnimCurvesConfig> m_Datas = null;
    public static List<AnimCurvesConfig> Datas
    {
        get
        {
            Load();
            return m_Datas;
        }
    }

    public static int Count
    {
        get
        {
            Load();
            return m_Datas.Count;
        }
    }

    private static string DefaultFolderAnimCurvesData = "AnimCurvesData/";
    public static Stream OpenDataAnimCurves(string fileName)
    {
        var filePath = DefaultFolderAnimCurvesData + fileName;
        filePath = ResourcesUtils.GetAssetRealPath(filePath);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_IOS
        if (DevelopSetting.IsUsePersistent)
            filePath = filePath.Replace("file:///", "");
        else
            filePath = filePath.Replace("file://", "");
#elif UNITY_ANDROID
            if (DevelopSetting.IsUsePersistent)
                filePath = filePath.Replace("file:///", "");
#endif
        return Common.FileUtils.OpenFileStream(filePath);
    }

    public static AnimCurvesConfig Get(string key)
    {
        Load();
        AnimCurvesConfig AnimCurvesConfigdata = null;
        m_DicDatas.TryGetValue(key, out AnimCurvesConfigdata);
        return AnimCurvesConfigdata;
    }

    public void SetAnimationCurve(AnimationCurve _data)
    {
        m_AnimationCurve = _data;
    }

    public void SetBaseValue(float baseValue)
    {
        m_baseValue = baseValue;
    }

    public void SetDurationTime(float DurationTim)
    {
        m_durationTime = DurationTim;
    }

    public  void Load(BinaryReader pStream)
    {
        m_AnimationCurveName = Common.BinarySerializer.Read_String(pStream);
        m_durationTime = Common.BinarySerializer.Read_Single(pStream);
        m_baseValue = Common.BinarySerializer.Read_Single(pStream);
        m_AnimationCurve = BinarySerializer_ReadAnimationCurve(pStream);
    }

    private static AnimationCurve BinarySerializer_ReadAnimationCurve(BinaryReader reader)
    {
        var keyFrameLength = Common.BinarySerializer.Read_Int32(reader);
        var keyFrames = new Keyframe[keyFrameLength];
        for (int i = 0; i < keyFrameLength; ++i)
        {
            keyFrames[i].time = Common.BinarySerializer.Read_Single(reader);
            keyFrames[i].value = Common.BinarySerializer.Read_Single(reader);
            keyFrames[i].inTangent = Common.BinarySerializer.Read_Single(reader);
            keyFrames[i].outTangent = Common.BinarySerializer.Read_Single(reader);
            keyFrames[i].tangentMode = Common.BinarySerializer.Read_Int32(reader);
        }
        return new AnimationCurve(keyFrames);
    }

    public static void ReLoad()
    {
        if(m_DicDatas != null)
        {
            m_DicDatas.Clear();
        }

        if(m_Datas != null)
        {
            m_Datas.Clear();
        }
        m_DicDatas = null;
        m_Datas = null;
        Load();
    }

    public static void Load()
    {
        if (m_DicDatas == null || m_Datas == null)
        {
            Stream fs = OpenDataAnimCurves("AnimCurves.bytes");
            if (fs != null)
            {
                BinaryReader br = new BinaryReader(fs);
                int dataNum = br.ReadInt32();
                m_DicDatas = new Dictionary<string, AnimCurvesConfig>(dataNum + 1);
                m_Datas = new List<AnimCurvesConfig>(dataNum + 1);
                for (int i = 0; i < dataNum; ++i)
                {
                    AnimCurvesConfig data = new AnimCurvesConfig();
                    data.Load(br);

                    if (m_DicDatas.ContainsKey(data.AnimationCurveName))
                    {
                        Debug.LogError("fuck you mate, ID:" + data.AnimationCurveName + " already exists in SkillConfig!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        continue;
                    }

                    m_DicDatas.Add(data.AnimationCurveName, data);
                    m_Datas.Add(data);
                }
                br.Close();
                br = null;
                fs.Close();
                fs = null;
            }
        }
    }

    private string m_AnimationCurveName = string.Empty;
    public string AnimationCurveName
    {
        get
        {
            return m_AnimationCurveName;
        }
    }

    private float m_durationTime = 0.0f;
    public float durationTime
    {
        get
        {
            return m_durationTime;
        }
    }

    private float m_baseValue = 0.0f;
    public float baseValue
    {
        get
        {
            return m_baseValue;
        }
    }

    private AnimationCurve m_AnimationCurve = null;
    public AnimationCurve AnimationCurve
    {
        get
        {
            return m_AnimationCurve;
        }
    }
}
