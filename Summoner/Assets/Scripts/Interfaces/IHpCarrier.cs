using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHpCarrier
{
    bool isHpActive { get; set; }
    Transform hpTransform { get; }
    bool isDuangActive { get; set; }
    int type { get; }
    string PlayerID { get; }
    float HpPercent { get; }
    float DuangPercent { get; }
    bool isBoss { get; }
    string Name { get; }
    uint Level { get; }
    string RoleName { get; }
}
