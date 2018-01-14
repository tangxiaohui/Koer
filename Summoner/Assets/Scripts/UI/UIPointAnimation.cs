using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPointAnimation : MonoBehaviour
{
    public float m_speed = 10.0f;
    public float m_maxScale = 1.728f;
    public float m_curScale = 1.0f;
    public float m_alpha = 1.0f;
    public UIImage m_image = null;
    public void Awake()
    {
        m_image = this.gameObject.GetComponent<UIImage>();
    }

    protected void LateUpdate()
    {
        
    }

    protected void FixedUpdate()
    {
        m_curScale += m_speed * Time.fixedDeltaTime;
        m_alpha -= m_speed * Time.fixedDeltaTime; ;
        if (m_curScale > m_maxScale)
        {
            m_curScale = 1.0f;
            m_alpha = 1.0f;
        }
        m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, m_alpha);
        transform.localScale = new Vector3(m_curScale, m_curScale, m_curScale);
    }

    protected void Update()
    {

    }
}
