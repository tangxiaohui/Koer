using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterAction
{
    public interface  IGameAction
    {
        void Start();

        void ChangeObjs(object[] objs);

        void SetArgs(object[] objs);

        void Update(float dt);

        void Exit();
    }
}
