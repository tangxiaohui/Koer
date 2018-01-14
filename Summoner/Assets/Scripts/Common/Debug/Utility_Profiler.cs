using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Utility_Profiler
{

    public static event System.Action<string> On_Profiler_Begin;
    public static event System.Action On_Profiler_End;
    const bool useProfiler = true;
    public static void BeginSample(string content)
    {
#if UNITY_EDITOR
        if (!useProfiler) return;
        // YSProfiler.BeginSample(content);
        if (On_Profiler_Begin != null)
            On_Profiler_Begin(content);
#endif
    }
    public static void EndSample()
    {
#if UNITY_EDITOR
        //Profiler.EndSample();
        if (!useProfiler) return;
        if (On_Profiler_End != null)
            On_Profiler_End();
#endif
        //YSProfiler.EndSample();
    }
}
