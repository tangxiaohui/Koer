using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
	public int score = 0;
	public int win = 0;
	public int fail = 0;
    public List<int> CardList = new List<int>();
    public List<int> BattleCardList = new List<int>();

    public PlayerData()
	{
		
	}
}