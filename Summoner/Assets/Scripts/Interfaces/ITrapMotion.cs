using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrapMotion
{
    void Initalize(string arg);
    void Start(Transform transform,Vector3 dir);
    void Update(float dt);
    void Exit();
    void Deinitialization();
}
