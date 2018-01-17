using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerData
{
    public int score = 0;
    public int win = 0;
    public int fail = 0;
    public List<int> CardList = new List<int>();
    public List<int> BattleCardList = new List<int>();

    public PlayerData(List<int> list)
    {
        CardList = list;
    }

    public PlayerData(List<int> list, List<int> battleList)
    {
        CardList = list;
        BattleCardList = battleList;
    }
}
