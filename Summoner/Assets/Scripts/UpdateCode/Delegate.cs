using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;

namespace UpdateSystem.Delegate
{
    /// <summary>
    /// 中断操作回调
    /// </summary>
    /// <param name="success"></param>
    public delegate void AbortFinishCallback(bool success);
    /// <summary>
    /// 资源转移回调
    /// </summary>
    /// <param name="success"></param>
    public delegate void TransResourceFinishCallback(bool success);
    /// <summary>
    /// 客户端下载完成
    /// </summary>
    /// <param name="success"></param>
    public delegate void ClientDownloadFinishCallback(bool success);
    /// <summary>
    /// 线程池操作
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="arg1"></param>
    public delegate void ThreadPoolAction<T1>(T1 arg1);
    public delegate void UpdateAction<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// 下载提示回调,在回调中做UI处理，弹出提示框等
    /// </summary>
    /// <param name="size">byte为单位</param>
    public delegate void DownloadNoticeCall(int size);
    /// <summary>
    /// 自定义下载客户端的方法
    /// </summary>
    /// <param name="url"></param>
    /// <param name="storeDir"></param>
    /// <returns></returns>
    public delegate int CustomDownClientFunc(string url, string storeDir);
    /// <summary>
    /// 更新流程结束调用
    /// </summary>
    /// <param name="success"></param>
    /// <param name="ret"></param>
    public delegate void FinishCallback(bool success, int ret);
    /// <summary>
    /// 一些操作触发调用
    /// </summary>
    public delegate void ActionCall(object obj);

    /// <summary>
    /// 默认的action
    /// </summary>
    public delegate void ActionV();
}