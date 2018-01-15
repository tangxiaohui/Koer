using System;
public partial class HandleConnMsg
{
    public void MsgHeatBeat(Conn conn, ProtocolBase protoBase)
    {
        conn.lastTickTime = Sys.GetTimeStamp();
        Console.WriteLine("更新心跳时间" + conn.GetAdress());
    }

    public void MsgRegister(Conn conn, ProtocolBase protoBase)
    {
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        string id = protocol.GetString(start, ref start);
        string pw = protocol.GetString(start, ref start);
        string strFormat = "收到注册协议" + conn.GetAdress();
        Console.WriteLine(strFormat + "用户名：" + id + "密码：" + pw);
        protocol = new ProtocolBytes();
        protocol.AddString("Register");
        if(DataMgr.instance.Register(id, pw))
        {
            protocol.AddInt(0);
            DataMgr.instance.CreatePlayer(id);
        }
        else
        {
            protocol.AddInt(-1);
        }
        conn.Send(protocol);
    }

    public void MsgLogin(Conn conn, ProtocolBase protoBase)
    {
        int start = 0;
        ProtocolBytes protocol = new ProtocolBytes();
        string protoName = protocol.GetString(start, ref start);
        string id = protocol.GetString(start, ref start);
        string pw = protocol.GetString(start, ref start);
        string strFormat = "收到登录协议" + conn.GetAdress();
        Console.WriteLine(strFormat + "用户名：" + id + "密码：" + pw);
        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("Login");
        if(!DataMgr.instance.CheckPassWord(id, pw))
        {
            protocolRet.AddInt(-1);
            conn.Send(protocolRet);
            return;
        }

        ProtocolBytes protocolLogout = new ProtocolBytes();
        protocolLogout.AddString("Logout");
        if(!Player.KickOff(id, protocolLogout))
        {
            protocolRet.AddInt(-1);
            conn.Send(protocolRet);
            return;
        }

        PlayerData palyerData = DataMgr.instance.GetPlayerData(id);
        if(palyerData == null)
        {
            protocolRet.AddInt(-1);
            conn.Send(protocolRet);
            return;
        }
        conn.player = new Player(id, conn);
        conn.player.data = palyerData;
        ServNet.instance.handlePlayerEvent.OnLogin(conn.player);
        protocolRet.AddInt(0);
        conn.Send(protocolRet);
        return;
    }

    public void MsgLogout(Conn conn, ProtocolBase protoBase)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Logout");
        protocol.AddInt(0);
        if(conn.player == null)
        {
            conn.Send(protocol);
            conn.Close();
        }
        else
        {
            conn.Send(protocol);
            conn.player.Logout();
        }
    }
}
