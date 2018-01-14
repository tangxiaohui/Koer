using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsUIData
{
    public GameObject gameObject = null;
    float speed = 200;
    float height = 200;
    protected bool m_bRealease = false;
    public bool bRelease
    {
        get
        {
            return m_bRealease;
        }
    }
    UIText text = null;
    public void Initalize()
    {
        m_bRealease = true;
        text = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "UIText");
        gameObject.SetActive(false);
    }

    public void Start(string str)
    {
        gameObject.SetActive(true);
        gameObject.transform.localPosition = Vector3.zero;
        text.text = str;
        m_bRealease = false;
    }

    public void Exit()
    {
        m_bRealease = true;
        gameObject.SetActive(false);
    }

    public void Deinitialization()
    {
        GameObject.Destroy(gameObject);
        m_bRealease = true;
    }
    public void Update(float dt)
    {
        if(!m_bRealease)
        {
            gameObject.transform.localPosition += new Vector3(0, speed * dt, 0);
            if(gameObject.transform.localPosition.y > height)
            {
                Exit();
            }
        }
    }
}

public class TipsUI :UIBase
{
    protected int maxCount = 6;
    protected GameObject m_Pre = null;
    protected List<TipsUIData> m_list = new List<TipsUIData>();
    void Awake()
    {
        
    }

    public override void Initalize()
    {
        base.Initalize();
        m_Pre = Utility.GameUtility.FindDeepChildGameObject(gameObject, "node/pre");
        m_Pre.SetActive(false);
        for(int i = 0; i < maxCount; ++i)
        {
            AddList();
        }
        OpenUI();
    }

    public void AddList()
    {
        GameObject go = GameObject.Instantiate(m_Pre);
        TipsUIData data = new TipsUIData();
        data.gameObject = go;
        data.Initalize();
        data.gameObject.transform.SetParent(m_Pre.transform.parent);
        Utility.GameUtility.ResetTransform(data.gameObject.transform);
        m_list.Add(data);
    }

    public override void Deinitialization()
    {
       for(int i = 0; i < m_list.Count; ++i)
        {
            m_list[i].Deinitialization();
        }
        m_list.Clear();
    }

    public void PushStr(string str)
    {
        for(int i = 0; i < m_list.Count; ++i)
        {
            if(m_list[i].bRelease)
            {
                m_list[i].Start(str);
                return;
            }
        }
        AddList();
        for (int i = 0; i < m_list.Count; ++i)
        {
            if (m_list[i].bRelease)
            {
                m_list[i].Start(str);
                return;
            }
        }
    }

    public void Update()
    {
        for(int i = 0; i < m_list.Count; ++i)
        {
            if(!m_list[i].bRelease)
            {
                m_list[i].Update(Time.deltaTime);
            }
        }
    }
    public override void OpenUI()
    {
        gameObject.SetActive(true);
    }

    public override void CloseUI()
    {
        gameObject.SetActive(false);
    }
}
