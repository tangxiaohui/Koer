using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimChange : MonoBehaviour
{
    private static AnimChange m_instance = null;
    protected AnimCurvesConfig m_AnimCurvesConfig = null;
    protected AnimationCurve m_AnimationCurve = null;
    protected float m_baseValue = 1.0f;
    protected float m_duration = 1.0f;
    public float duration
    {
        get
        {
            return m_duration;
        }
        set
        {
            m_duration = value;
            if (m_AnimCurvesConfig != null)
            {
                m_AnimCurvesConfig.SetDurationTime(m_duration);
            }
        }
    }

    public float baseValue
    {
        get
        {
            return m_baseValue;
        }
        set
        {
            m_baseValue = value;
            if (m_AnimCurvesConfig != null)
            {
                m_AnimCurvesConfig.SetBaseValue(m_baseValue);
            }
        }
    }


    public AnimationCurve AnimationCurveSetting
    {
        get
        {
            return m_AnimationCurve;
        }
        set
        {
            m_AnimationCurve = value;
            if(m_AnimCurvesConfig != null)
            {
                m_AnimCurvesConfig.SetAnimationCurve(m_AnimationCurve);
                Utility.Event.EventDispatcher.Dispatch(Utility.Event.EventType.ResetAnimationCurve, null);
            }
        }
    }

    protected string m_AnimChangeName = string.Empty;
    public string AnimChangeName
    {
        get
        {
            return m_AnimChangeName;
        }
        set
        {
            m_AnimChangeName = value;
            m_AnimCurvesConfig =  AnimCurvesConfig.Get(m_AnimChangeName);
            if(m_AnimCurvesConfig != null)
            {
                m_AnimationCurve = m_AnimCurvesConfig.AnimationCurve;
                m_baseValue = m_AnimCurvesConfig.baseValue;
                m_duration = m_AnimCurvesConfig.durationTime;
            }
        }
    }

    public static AnimChange Instance
    {
        get
        {
            if (m_instance == null)
            {
                var gameConfigObj = new GameObject("[AnimChangeSetting]");
                gameConfigObj.transform.SetParent(Common.Root.root);
                m_instance = gameConfigObj.AddComponent<AnimChange>();
            }
            return m_instance;
        }
    }

    public void Initalize()
    {

    }
}
