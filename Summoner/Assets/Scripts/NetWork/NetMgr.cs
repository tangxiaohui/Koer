using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMgr
{
    public static Connection srvConn = new Connection();
    //public static Connection platformConn = new Connection();
    public static void Update()
    {
        srvConn.Update();
        //platformConn.Update();
    }

    public static ProtocolBase GetHeatBeatProtocol()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("HeatBeat");
        return protocol;
    }
}
