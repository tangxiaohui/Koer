using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIText : Text {
    protected override void Awake()
    {
        base.Awake();
        this.raycastTarget = false;
    }

    public override string text
    {
        get
        {
            return base.text;
        }

        set
        {
            if(!base.text.Equals(value))
               base.text = value;
        }
    }
}
