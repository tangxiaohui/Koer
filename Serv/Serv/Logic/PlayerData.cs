using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
	public int score = 0;
	public int win = 0;
	public int fail = 0;
    public List<int> CardList = new List<int>();

	public PlayerData()
	{
		score = 100;
        CardList.Add(1);
        CardList.Add(2);
        CardList.Add(3);
	}
}