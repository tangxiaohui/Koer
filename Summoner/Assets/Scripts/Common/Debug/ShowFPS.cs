using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine.Profiling;



public class ShowFPS : UIBase
{
    // public variables
    float updateInterval = 0.1f;  // fps更新间隔

    public Text uiLabel;
    public Text luaLabel;
    public Text MemeryLabel;
    // private variables
    private float accum = 0.0f;           // fps累积
    private int frames = 0;             // 帧数累积
    private float timeLeft = 0.0f;        // 当前更新间隔的计时
                                            /////////////////////////////////////////////////////////////////////
    public float fps = 30;

    private float max = -1f;
    private float min = 9999f;
    private float average = 0f;
    private int frameCount = 0;
    private float totalFrame = 0;
    private float caluteTime;
    private float caluteProfilerTime = 0;
    private float caluteConstTime = 60f;
    /// <summary>
    /// 获取FPS 
    /// </summary>
    /// <param name="minFPS"></param>
    /// <param name="maxFPS"></param>
    /// <param name="averageFPS"></param>
    /// <returns></returns>
    public bool GetFPS(ref int minFPS, ref int maxFPS, ref int averageFPS)
    {
        if (max < 0)
        {
            minFPS = 0;
            maxFPS = 0;
            average = 0;
            return false;
        }
        else
        {
            minFPS = (int)max;
            maxFPS = (int)min;
            averageFPS = (int)(totalFrame / frameCount);
            return true;
        }
    }
    public void SetCaluteTime(float time)
    {
        caluteConstTime = time;
        caluteTime = Time.time + caluteConstTime;
        caluteProfilerTime = Time.time + caluteConstTime;
    }

    // private functions
    private void Start()
    {

    }

    public override void Initalize()
    {
        base.Initalize();
        uiLabel = Utility.GameUtility.FindDeepChild<Text>(gameObject, "UIText");
        uiLabel.color = Color.red;

        luaLabel = Utility.GameUtility.FindDeepChild<Text>(gameObject, "LuaMe");
        luaLabel.color = Color.red;

        MemeryLabel = Utility.GameUtility.FindDeepChild<Text>(gameObject, "Memery");
        MemeryLabel.color = Color.red;
        caluteTime = Time.time + caluteConstTime;
        caluteProfilerTime = Time.time + caluteConstTime;
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        timeLeft = updateInterval;

        OpenUI();
    }

    #region 打印内存
    private string sUserMemory;
    private string s;
    public bool OnMemoryGUI;
    private uint MonoUsedM;
    private uint AllMemory;
    [Range(0, 100)]
    public int MaxMonoUsedM = 50;
    [Range(0, 400)]
    public int MaxAllMemory = 200;
    void UpdateUsed()
    {
        sUserMemory = "";
        MonoUsedM = Profiler.GetMonoUsedSize() / 1000000;
        AllMemory = Profiler.GetTotalAllocatedMemory() / 1000000;

        sUserMemory += "MonoUsed:" + MonoUsedM + "M" + "\n";
        sUserMemory += "AllMemory:" + AllMemory + "M" + "\n";
        sUserMemory += "UnUsedReserved:" + Profiler.GetTotalUnusedReservedMemory() / 1000000 + "M" + "\n";
        s = string.Empty;
        s += "MonoHeap:" + Profiler.GetMonoHeapSize() / 1000 + "k\n";
        s += "MonoUsed:" + Profiler.GetMonoUsedSize() / 1000 + "k\n";
        s += "Allocated:" + Profiler.GetTotalAllocatedMemory() / 1000 + "k\n";
        s += "Reserved:" + Profiler.GetTotalReservedMemory() / 1000 + "k\n";
        s += "UnusedReserved:" + Profiler.GetTotalUnusedReservedMemory() / 1000 + "k\n";
        s += "UsedHeap:" + Profiler.usedHeapSize / 1000 + "k\n";
        MemeryLabel.text = s;
    }
    #endregion

    private void Update()
    {
        //
        timeLeft -= Time.deltaTime;

        //
        accum += (Time.timeScale / Time.deltaTime);

        //
        ++frames;

        //
        if (timeLeft <= 0.0f) // 到达更新间隔
        {
            fps = accum / frames; // 计算fps
            uiLabel.text = Common.StringUtils.CombineString("FPS: ", fps.ToString("f2"));
            //luaLabel.text = "LuaMemory:" + LuaClient.Instance.GetluaCollect();
           // UpdateUsed();
            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
            if (caluteTime < Time.time)
            {
                frameCount++;
                if (fps > max) max = fps;
                if (fps < min) min = fps;
                totalFrame += fps;
                if (caluteProfilerTime < Time.time)
                {
                    caluteProfilerTime = Time.time + caluteConstTime;
                    var av = (int)(totalFrame / frameCount);
                    max = 0;
                    min = 0;
                    totalFrame = 0;
                    frameCount = 0;
                   // Utility.Event.EventDispatcher.Dispatch(EventType.CaluteFPS, av);
                }
            }
        }
    }
}