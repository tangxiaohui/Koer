using UnityEngine;
using System.Collections;

public abstract class SingleInstance<T> where T : new()
{
    protected static T m_Instance;
    protected bool m_bInitalize = false;
    public static T Instance
    {
        get
        {
            if(m_Instance == null)
            {
                m_Instance = new T();
            }
            return m_Instance;
        }
    }

    public virtual void Initalize()
    {
        m_bInitalize = true;
    }

    public virtual void Deinitialization()
    {
        m_bInitalize = false;
    }
}
