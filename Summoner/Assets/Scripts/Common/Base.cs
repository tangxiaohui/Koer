using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Base : MonoBehaviour
{
    
    protected virtual void OnEnable()
    {
        if(GetComponent<Mask>() != null)
        {
            MaskUtilities.NotifyStencilStateChanged(this);
        }
    }
    protected virtual void OnDisable()
    {
        if (GetComponent<Mask>() != null)
        {
            MaskUtilities.NotifyStencilStateChanged(this);
        }
    }
}
