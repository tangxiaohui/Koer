using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UpdateSystem.Xml;
using UpdateSystem.Log;
using UpdateSystem.Trans;
using UpdateSystem.Delegate;

namespace UpdateSystem.Flow
{
    /// <summary>
    /// 10.收尾流程，判断成功或者失败
    /// </summary>
    public class FlowFinish : BaseFlow
    {
        //更新流程完成后调用
        FinishCallback _finishCallback;

        public void SetExternalData(FinishCallback callback)
        {
            _finishCallback = callback;
        }

        public override void OnEnter(BaseFlow oldFlow)
        {
            base.OnEnter(oldFlow);
            UseDownload = false;
        }

        public override int Work()
        {
            UpdateLog.DEBUG_LOG("更新流程结束++++");
            int ret = LastFlowResult;
            if (ret >= CodeDefine.RET_SUCCESS)
            {
                UpdateLog.DEBUG_LOG("更新流程正常结束");
            }
            else
                UpdateLog.DEBUG_LOG("更新失败 ret=" + ret);

            UpdateLog.DEBUG_LOG("更新流程结束----");

            callFinish(ret);

            return ret;
        }

        public override void GetCurDownInfo(out string url, out int total, out int downloaded)
        {
            //base.GetCurDownInfo(out url, out total, out downloaded);
            url = "";
            total = 0;
            downloaded = 0;
        }

        public override void Uninitialize()
        {

        }

        public void FinishWithError(int code)
        {
            UpdateLog.ERROR_LOG("FinishWithError : + " + code);
            LastFlowResult = code;
            Work();
        }

        private void callFinish(int ret)
        {
            if (_finishCallback != null)
            {
                _finishCallback(ret >= CodeDefine.RET_SUCCESS, ret);
            }
            else
                UpdateLog.DEBUG_LOG("没有找到FinishCallback 回调函数");
        }
    }
}
