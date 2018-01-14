using System;
using UnityEngine;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
namespace Launcher
{
    public class Launcher : MonoBehaviour
    {
        public static void Launch()
        {
            UpdateSystem.UpdateCenter.Close();
            GameStart();
        }


        private static bool GameStart()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; ++i)
            {
                var gameStart = assemblies[i].GetType("GameStart");
                if (gameStart != null)
                {
                    gameStart.InvokeMember("Start", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, null);
                    return true;
                }
            }
            return false;
        }
    }
}
