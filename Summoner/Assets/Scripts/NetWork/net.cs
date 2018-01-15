using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class net : MonoBehaviour
{
    void Start()
    {
		NetMgr.srvConn.proto = new ProtocolBytes ();
        NetMgr.srvConn.Connect("127.0.0.1", 1234);
    }

    void Update()
    {
        NetMgr.Update();
    }
}
