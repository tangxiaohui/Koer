using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储滑动列表中单项的子物体，防止每次都要查找
/// </summary>
public class ItemNodeFinder : MonoBehaviour {

    public Dictionary<string, Transform> _nodeDic = new Dictionary<string, Transform>();

    Transform _transform;

    public Transform Find(string name)
    {
        if (_nodeDic.ContainsKey(name))
        {
            return _nodeDic[name];
        }
        else
        {
            _nodeDic[name] = transform.Find(name);
            return _nodeDic[name];
        }
    }
}
