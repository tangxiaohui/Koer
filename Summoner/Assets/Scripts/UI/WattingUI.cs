using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WattingUI : UIBase
{
    // protected Text m_Text = null;
    protected Image m_icon = null;
    protected float m_WaittingTime = 0.0f;
    protected float m_fEndTime = 0;
    protected Vector3 m_speedDir = new Vector3(0, 0,-360);
    protected System.Action m_AutoFailCloseFun = null;
    protected GameObject m_waitNode = null;
    protected bool bShow = false;
    public override void Initalize()
    {
        base.Initalize();
        // m_Text = Utility.GameUtility.FindDeepChild<Text>(gameObject, "Text");
        m_waitNode = Utility.GameUtility.FindDeepChildGameObject(gameObject, "waitNode");
        m_icon = Utility.GameUtility.FindDeepChild<Image>(gameObject, "waitNode/icon");
        CloseUI();
    }

    public void SetAutoFailClose(System.Action call)
    {
        m_AutoFailCloseFun = call;
    }

    public void Update()
    {
        if(bShow)
        {
            if(Time.time > m_WaittingTime)
            {
                if(!m_waitNode.activeSelf)
                {
                    m_waitNode.SetActive(true);
                }

                if (Time.time > m_fEndTime)
                {
                    if (m_AutoFailCloseFun != null)
                    {
                        m_AutoFailCloseFun();
                        m_AutoFailCloseFun = null;
                    }
                    CloseUI();
                }
                m_icon.transform.Rotate(m_speedDir * Time.deltaTime);
            }
        }
    }

    public override void OpenUI()
    {
        m_fEndTime = Time.time + 5.0f;
        m_WaittingTime = Time.time + 0.5f;
        bShow = true;
        gameObject.SetActive(true);
    }

    public override void CloseUI()
    {
        m_AutoFailCloseFun = null;
        bShow = false;
        gameObject.SetActive(false);
        m_waitNode.SetActive(false);
    }

}
