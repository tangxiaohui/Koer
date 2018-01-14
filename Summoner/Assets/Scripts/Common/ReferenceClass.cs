using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceClass : IReference
{
    protected int count = 0;
    public int getCount()
    {
        return count;
    }

    public void unReference()
    {
        --count;
    }

    public void Reference()
    {
        ++count;
    }
}
