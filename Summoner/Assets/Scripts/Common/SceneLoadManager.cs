using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoadManager
{
    public static void LoadScene(string name)
    {
        //LoadEmptyScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(name, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public static void LoadRealScene(string name)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(name, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public static void LoadEmptyScene()
    {
        SceneManager.LoadScene("Null", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public static Scene GetSceneByName(string name)
    {
       Scene tmp =  SceneManager.GetSceneByName(name);
        return tmp;
    }

    public static bool isScene(string name)
    {
        if(SceneManager.GetActiveScene().name.Equals(name))
        {
            return true;
        }
        return false;
    }

    public static void LoadScene(Res.SceneType type)
    {
        SceneManager.LoadScene((int)type, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public static AsyncOperation LoadSceneAsync(string name)
    {
        AsyncOperation  asyn =  SceneManager.LoadSceneAsync(name);
        return asyn;
    }
}