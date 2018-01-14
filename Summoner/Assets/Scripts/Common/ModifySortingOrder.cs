using UnityEngine;
using System.Collections;

/// <summary>
/// 设置Renderer的SortingOrder
/// </summary>
[ExecuteInEditMode]
public class ModifySortingOrder : MonoBehaviour
{
    public string sortingLayerName = "Default";
    public int sortingOrder = 0;

    // Use this for initialization
    void Start()
    {
        Renderer render = GetComponent<Renderer>();
        if (render != null)
        {
            render.sortingLayerName = sortingLayerName;
            render.sortingOrder = sortingOrder;
        }
    }

#if UNITY_EDITOR
    void FixedUpdate()
    {
        Renderer render = GetComponent<Renderer>();
        if (render != null)
        {
            render.sortingLayerName = sortingLayerName;
            render.sortingOrder = sortingOrder;
        }
    }
#endif
}
