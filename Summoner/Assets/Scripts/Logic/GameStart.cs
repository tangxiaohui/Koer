#define _DEBUG_INFO

using UnityEngine;
using System;
using System.Diagnostics;
using Utility;

public class GameStart 
{
    public static ClientProxy clientProxy = null;
    public static void Start()
    {
#if UNITY_EDITOR
        YSProfiler.Regester();
#endif
        //Common.UDebug.enabled = true;
        //Common.UDebug.LogError("GameStart Start");
        Utility_Profiler.BeginSample(" Common.Root.Initialize()");
        Common.Root.Initialize();
        Utility_Profiler.EndSample();

        Utility_Profiler.BeginSample("   UIManager.Instance.Initalize()");
        UIManager.Instance.Initalize();
        Utility_Profiler.EndSample();

        NetMgr.srvConn.proto = new ProtocolBytes();
        NetMgr.srvConn.Connect("127.0.0.1", 1234);

		if (clientProxy == null) 
		{
			GameObject go = new GameObject ("[ClientProxy]");
			clientProxy = go.GetOrAddComponent<ClientProxy> ();
		} 
		else if(UpdateSystem.UpdateCenter.bRestart)
		{
			UpdateSystem.UpdateCenter.bRestart = false;
			clientProxy.ChangeState(GameStateEnum.GameStateEnum_Resert);
		}
    }
}
