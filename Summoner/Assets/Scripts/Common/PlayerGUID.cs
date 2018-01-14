using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
public static class PlayerGUID
{
    public static string CreateGUID()
    {
        return Guid.NewGuid().ToString();
    }
}