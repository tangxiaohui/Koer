using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface  IDamage
{
    void Update(float dt);
    void Deinitialization();
    bool bRelease {get; }
}
