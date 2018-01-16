using System.Collections;
using System.Collections.Generic;

public class PlayerData
{
    public int score = 0;
    public int win = 0;
    public int fail = 0;
    public List<int> CardList = new List<int>();

    public PlayerData(List<int> list)
    {
        CardList.AddRange(list);
    }
}
