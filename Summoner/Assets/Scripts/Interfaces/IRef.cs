using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReference
{
    /// <summary>
    /// 增加引用计数
    /// </summary>
    void Reference();
    /// <summary>
    /// 减少引用计数
    /// </summary>
    void unReference();
    /// <summary>
    /// 获取引用计数
    /// </summary>
    /// <returns></returns>
    int getCount();
}
