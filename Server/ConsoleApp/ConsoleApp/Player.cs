using System;
public class Player
{
    public string id;
    public Conn conn;
    public PlayerData data;
    public PlayerTempData tempData;

    public Player()
    {

    }

    public Player(string id, Conn conn)
    {
        this.id = id;
        this.conn = conn;
        tempData = new PlayerTempData();
    }

    public void Send(ProtocolBase protocol)
    {
        if (conn == null)
            return;
        ServNet.instance.Send(conn, protocol);
    }

    public static bool KickOff(string id, ProtocolBase protocol)
    {
        Conn[] conns = ServNet.instance.conns;
        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null)
                continue;
            if (!conns[i].isUse)
                continue;
            if (conns[i].player == null)
                continue;
            if(conns[i].player.id == id)
            {
                lock(conns[i].player)
                {
                    if (protocol != null)
                        conns[i].player.Send(protocol);
                    return conns[i].player.Logout();
                }
            }
        }
        return true;
    }

    public bool Logout()
    {
        //ServNet.instance.handlePlayerEvent.OnLogout(this);
        if (!DataMgr.instance.SavePlayer(this))
            return false;
        conn.player = null;
        conn.Close();
        return true;
    }
}
