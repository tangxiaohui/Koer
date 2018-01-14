using UnityEngine;
using System.Collections;
using Utility;
using UnityEngine.Profiling;

public class YSProfiler
{
    public static void Regester()
    {
        Utility_Profiler.On_Profiler_Begin += Utility_Profiler_On_Profiler_Begin;
        Utility_Profiler.On_Profiler_End += Utility_Profiler_On_Profiler_End;
    }

    private static void Utility_Profiler_On_Profiler_End()
    {
        Profiler.EndSample();
    }

    private static void Utility_Profiler_On_Profiler_Begin(string obj)
    {
        Profiler.BeginSample(obj);
    }
}
