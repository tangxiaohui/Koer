using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;
using UpdateSystem.Log;
using UpdateSystem.Delegate;


namespace UpdateSystem.Trans
{
    /// <summary>
    /// UILoginFormScript的逻辑处理类
    /// </summary>
    public class TransResource
    {
        //@"H:\Qinji2\Client\Target\test1.apk";
        protected static string _resourcePath;
        //@"H:\Qinji2\Client\Target";//Application.streamingAssetsPath;
        protected static string _outPath;

        protected int nReadCount = 0;
        protected int nWriteCount = 0;

        //转移资源成功
        protected bool _success = false;

        protected TransResourceFinishCallback _finishCallback;

        public void SetUnzipPath(string sourcePath, string outPath, TransResourceFinishCallback callback)
        {
            _resourcePath = sourcePath;
            _outPath = outPath;
            _finishCallback = callback;
        }

        public virtual void StartUnzipByThread()
        {
            BeginTransRes();
        }

        public virtual void BeginTransRes()
        {
        }


        public int GetCurrentProgress()
        {
            return nWriteCount;
        }

        public float GetCurrentProgressValue()
        {
            if (nReadCount == 0)
            {
                return 0;
            }
            return (float)nWriteCount / (float)nReadCount;
        }

        public int GetTotalValue()
        {
            return nReadCount;
        }

        //转移资源结果
        public bool GetTransReslult()
        {
            return _success;
        }

        public virtual void CallFinish(bool success)
        {
            if (_finishCallback != null)
            {
                _finishCallback(success);
            }
        }
    }
}