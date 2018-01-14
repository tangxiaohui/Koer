using System;
public partial class HandlePlayerMsg
{
    public void MsgGetScore(Player player, ProtocolBase protoBase)
    {
        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("GetScore");
        protocolRet.AddInt(player.data.score);
        player.Send(protocolRet);
        Console.WriteLine("MsgGetScore " + player.id + ":" + player.data.score);
    }

    public void MsgAddScore(Player player, ProtocolBase protoBase)
    {
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        player.data.score += 1;
        Console.WriteLine("MsgAddScore " + player.id + ":" + player.data.score.ToString());
    }
}
