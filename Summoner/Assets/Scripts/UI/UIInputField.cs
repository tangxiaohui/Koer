using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInputField : InputField
{
    protected override void Awake()
    {
        base.Awake();
        onEndEdit.AddListener(OnEndEdit);
        onValueChanged.AddListener(OnChangedValue);
    }

    private void OnChangedValue(string value)
    {
        
    }

    /// <summary>
    /// 结束编辑后的回调
    /// </summary>
    /// <param name="value"></param>
    private void OnEndEdit(string value)
    {
    }


    public void RegEndEdit()
    {
        
    }

    public void RegChangedValue()
    {

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
