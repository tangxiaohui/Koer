using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIProcess : UIImage
{
    private float m_process = 0.0f;
    private float m_initWidth = 0.0f;
    private float m_initHeight = 0.0f;
    protected override void Awake()
    {
        base.Awake();
        m_initWidth = this.rectTransform.sizeDelta.x;
        m_initHeight = this.rectTransform.sizeDelta.y;
    }
    public float process
    {
        get
        {
            return m_process;
        }
        set
        {
            m_process = value;
            if(m_process > 1.0f)
            {
                m_process = 1.0f;
            }

            if (m_process < 0.0f)
            {
                m_process = 0.0f;
            }
            this.rectTransform.sizeDelta = new Vector2(m_initWidth * m_process, m_initHeight);
        }
    }
}
