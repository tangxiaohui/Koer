using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoad
{
    public static GameObject Load(string path)
    {
        GameObject pre = Resources.Load<GameObject>(path);
        if(pre == null)
        {
            Debug.LogError("艹，这个资源为空？？？" + path);
            return null;
        }
        GameObject go = GameObject.Instantiate(pre);
        return go;
    }
}
