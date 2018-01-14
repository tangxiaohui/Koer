using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticalDepth : MonoBehaviour
{
    public int order;
    void OnEnable()
    {
        Renderer[] renders = GetComponentsInChildren<Renderer>();
        int count = renders.Length;
        for (int i = 0; i < count; i++)
        {
            Renderer render = renders[i];
            render.sortingOrder = order;
        }
    }
}