using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public interface ISkillCharater
    {
        GameObject gameObject { get; }

        Transform transform { get; }

        Transform FindTransform(string name);

        void PlayAnimation(string animation, bool isLoop, float speed, bool isReset);

        void PlayAnimationNoCross(string animation, bool isloop, float speed, bool isReset);

        float GetAnimationTime(string name);

        uint Rage { get; }

        int Team { get; }

        bool flipX { get; }
        string PlayerID { get; }
        Vector3 position{ get; set;}

        void SetPreSkill(int preSkillID);

        Vector3 GetDir();

        int RoleID { get; }

        bool FXflipX { get; }
    }
}
