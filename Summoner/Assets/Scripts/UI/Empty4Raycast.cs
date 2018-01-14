using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// 用于空白的点击接收
/// </summary>
public class Empty4Raycast : MaskableGraphic
{
    protected Empty4Raycast()
    {
        useLegacyMeshGeneration = false;
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();
    }
}

