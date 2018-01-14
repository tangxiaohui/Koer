using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    /// <summary>
    /// 毫秒转成秒
    /// </summary>
    private static float m_MsToS = 0.001f;
    public static float MsToS
    {
        get
        {
            return m_MsToS;
        }
    }
    /// <summary>
    /// 厘米转成米
    /// </summary>
    private static float m_CmToM = 0.01f;
    public static float CmToM
    {
        get
        {
           return m_CmToM;
        }
    }

    /// <summary>
    /// 毫米转成米
    /// </summary>
    private static float m_mmToM = 0.0001f;
    public static float mmToM
    {
        get
        {
            return m_mmToM;
        }
    }
    /// <summary>
    /// 距离误差值
    /// </summary>
    private static float m_offsetDis = 0.5f;
    public static float offsetDis
    {
        get
        {
            return m_offsetDis;
        }
    }
    /// <summary>
    /// 万分比
    /// </summary>
    private static float m_ExtremeRatio = 0.0001f;
    public static float ExtremeRatio
    {
        get
        {
            return m_ExtremeRatio;
        }

    }
    /// <summary>
    /// 百分比
    /// </summary>
    private static float m_Percentage = 0.01f;
    public static float Percentage
    {
        get
        {
            return m_Percentage;
        }

    }
}
