using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : SingleInstance<BattleManager>
{
    public Dictionary<int, BattleCard> MyPlayerCardDic = new Dictionary<int, BattleCard>();
    public Dictionary<int, BattleCard> EnemyCardDic = new Dictionary<int, BattleCard>();
    public int MyFightCardID = 0;
    public int EnemyFightCardID = 0;
    public Action OnFightOpComplete = null;

    public enum ATTRIBUTED
    {
        Metal,
        Wood,
        Water,
        Fire,
        Earth,
        Chaos
    };

    public void InitBattleData()
    {
        foreach(int i in MyPlayer.Instance.data.BattleCardList)
        {
            BattleCard battleCard = new BattleCard();
            battleCard.Id = i;
            battleCard.BaseData = Cards.Get(i);
            battleCard.Hp = battleCard.BaseData.Hp;
            battleCard.CurHp = battleCard.Hp;

            MyPlayerCardDic.Add(i, battleCard);
        }

        foreach(int i in EnemyPlayer.Instance.data.BattleCardList)
        {
            BattleCard battleCard = new BattleCard();
            battleCard.Id = i;
            battleCard.BaseData = Cards.Get(i);
            battleCard.Hp = battleCard.BaseData.Hp;
            battleCard.CurHp = battleCard.Hp;

            EnemyCardDic.Add(i, battleCard);
        }
    }

    public void FightOp(int enemyCardId)
    {
        EnemyFightCardID = enemyCardId;
        BattleCard myplayer = MyPlayerCardDic[MyFightCardID];
        BattleCard enemyplayer = EnemyCardDic[EnemyFightCardID];
        float Attributed = 1f;
        //if (myplayer.BaseData.Attributed == (int)ATTRIBUTED.Metal && enemyplayer.BaseData.Attributed == (int)ATTRIBUTED.Wood
        //    || )
        //    Attributed = 1.2f;
        EnemyCardDic[enemyCardId].Hurt(MyPlayerCardDic[MyFightCardID].Attack());
        MyPlayerCardDic[MyFightCardID].Hurt(EnemyCardDic[enemyCardId].Attack());

        if (OnFightOpComplete != null)
            OnFightOpComplete();
    }

    public BattleCard GetCurrentEnemyFightCard()
    {
        return EnemyCardDic[EnemyFightCardID];
    }

    public BattleCard GetCurrentMyFightCard()
    {
        return MyPlayerCardDic[MyFightCardID];
    }
}


