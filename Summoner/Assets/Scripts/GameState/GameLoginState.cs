using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoginState : gxGameState
{
    protected const string Login = "LoginState";
    protected const string m_GameSceneName = "Start";
    private static WaitForEndOfFrame defaultWaitForEndOfFrame = new WaitForEndOfFrame();
    public override int Initalize()
    {
        if(ClientProxy.isBackLogin)
        {
            SceneLoadManager.LoadRealScene(m_GameSceneName);
            Common.Root.coro.StartCoroutine(StartLoad());

            return base.Initalize();
        }

        Common.Root.coro.StartCoroutine(StartLoad());
        ClientProxy.isBackLogin = true;
        return base.Initalize();
    }

    IEnumerator StartLoad()
    {
        yield return defaultWaitForEndOfFrame;
        yield return defaultWaitForEndOfFrame;
        SoundManager.PlayBGMusic(Common.StringUtils.CombineString("loginbg", System.IO.Path.DirectorySeparatorChar.ToString(), "loginbg"));
        yield return defaultWaitForEndOfFrame;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update(float dt)
    {
        
    }

    public override int Deinitialization()
    {
        UIManager.Instance.ClearAllUI();
        SoundManager.Instance.Deinitialization();
        // Resources.UnloadUnusedAssets();
        Res.ResourcesManager.Instance.UnloadAll();
        return base.Deinitialization();
    }
}
