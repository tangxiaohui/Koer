using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

public static class LayerUtils
{
    public readonly static int Default  = LayerMask.NameToLayer("Default");
    public readonly static int UI = LayerMask.NameToLayer("UI");
    public readonly static int UITop = LayerMask.NameToLayer("UITop");
    public readonly static int UIWorldMap = LayerMask.NameToLayer("UIWorldMap");
    public readonly static int UIMove = LayerMask.NameToLayer("UIMove");
    public readonly static int Hero = LayerMask.NameToLayer("Hero");
    public readonly static int Map = LayerMask.NameToLayer("Map");
    public readonly static int Enemy = LayerMask.NameToLayer("Enemy");
    public readonly static int Teammate = LayerMask.NameToLayer("Teammate");
    public readonly static int UIModel = LayerMask.NameToLayer("UIModel");
    public readonly static int Model3D = LayerMask.NameToLayer("3DModel");
    public readonly static int UIBottom = LayerMask.NameToLayer("UIBottom");
    public readonly static int EffectBloom = LayerMask.NameToLayer("EffectBloom");
}