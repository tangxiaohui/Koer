using System.Collections;
using System.Collections.Generic;

public class BattleCard
{
    public int Id;
    public int Hp;
    public int CurHp;
    public Cards BaseData;

    public int Attack()
    {
        return BaseData.Atk;
    }

    public void Hurt(int hurt)
    {
        CurHp -= hurt;
    }

    public void Dead()
    {

    }

    public float HpPercent()
    {
        return CurHp * 1f / Hp;
    }
}
