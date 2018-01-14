using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UpdateSystem.Enum;
using UpdateSystem.Delegate;
using UpdateSystem.Flow;
using UpdateSystem.Log;

namespace UpdateSystem.Manager
{
    /// <summary>
    /// 一些回调函数是从线程触发的，需要转为UI调用
    /// 这个类就是把函数调用转成UI调用的
    /// </summary>
    public partial class UpdateManager : SingleInstance<UpdateManager>
    {
        TransResourceFinishCallback _transFinishConvertCallback;
        ClientDownloadFinishCallback _clienDownloadFinishConvertCallback;
        DownloadNoticeCall _downloadNoticeConvertCallback;
        FinishCallback _finishConvertCallback;
        ActionCall _actionCall;
        private UpdateAction<string, bool, object> _myActionCall;

        List<KeyValuePair<ConvertFuncEnum, object>> _argList = new List<KeyValuePair<ConvertFuncEnum, object>>();
        object _locker = new object();

        //是否配置了Update函数在UI线程
        bool _useUpdate = false;

        //计数器
        int _counter = 0;

        private void convertFuncTransResourceFinishCallback(TransResourceFinishCallback callback)
        {
            _transFinishConvertCallback = callback;
        }

        private void convertFuncClientDownloadFinishCallback(ClientDownloadFinishCallback callback)
        {
            _clienDownloadFinishConvertCallback = callback;
        }

        private void convertFuncDownloadNoticeCall(DownloadNoticeCall callback)
        {
            _downloadNoticeConvertCallback = callback;
        }

        private void convertFuncFinishCallback(FinishCallback callback)
        {
            _finishConvertCallback = callback;
        }

        private void convertActionCall(ActionCall callback)
        {
            _actionCall = callback;
        }

        private UpdateAction<string, bool, object> convertMyActionCall(UpdateAction<string, bool, object> callback)
        {
            this._myActionCall = callback;
            return new UpdateAction<string, bool, object>(this.onMyActionCallback);
        }

        private void onTransResourceFinishCallback(bool success)
        {
            if (!_useUpdate)
            {
                callUIFunc(ConvertFuncEnum.TransFunc, success);
                return;
            }

            lock (_locker)
            {
                _counter++;
                KeyValuePair<ConvertFuncEnum, object> keyP = new KeyValuePair<ConvertFuncEnum, object>(ConvertFuncEnum.TransFunc, success);
                _argList.Add(keyP);
            }
        }

        private void onClientDownloadFinishCallback(bool success)
        {
            if (!_useUpdate)
            {
                callUIFunc(ConvertFuncEnum.DownloadClientFunc, success);
                return;
            }
            lock (_locker)
            {
                _counter++;
                _argList.Add(new KeyValuePair<ConvertFuncEnum, object>(ConvertFuncEnum.DownloadClientFunc, success));
            }
        }

        private void onDownloadNoticeCall(int size)
        {
            if (!_useUpdate)
            {
                callUIFunc(ConvertFuncEnum.NoticeFunc, size);
                return;
            }
            lock (_locker)
            {
                _counter++;
                _argList.Add(new KeyValuePair<ConvertFuncEnum, object>(ConvertFuncEnum.NoticeFunc, size));
            }
        }

        private void onActionCall(object obj)
        {
            if (!_useUpdate)
            {
                callUIFunc(ConvertFuncEnum.ActionCallFunc, obj);
                return;
            }

            lock (_locker)
            {
                _counter++;
                _argList.Add(new KeyValuePair<ConvertFuncEnum, object>(ConvertFuncEnum.ActionCallFunc, obj));
            }
        }

        private void onFinishCallback(bool success, int ret)
        {
            if (!_useUpdate)
            {
                object[] args = new object[] { success, ret };
                callUIFunc(ConvertFuncEnum.FinishFunc, args);
                return;
            }

            lock (_locker)
            {
                _counter++;
                object[] args = new object[] { success, ret };
                _argList.Add(new KeyValuePair<ConvertFuncEnum, object>(ConvertFuncEnum.FinishFunc, args));
            }
        }

        private void onMyActionCallback(string path, bool result, object obj)
        {
            object[] objArray;
            if (!this._useUpdate)
            {
                objArray = new object[] { path, result, obj };
                this.callUIFunc(ConvertFuncEnum.CheckResFunc, objArray);
            }
            else
            {
                lock (this._locker)
                {
                    this._counter++;
                    objArray = new object[] { path, result, obj };
                    this._argList.Add(new KeyValuePair<ConvertFuncEnum, object>(ConvertFuncEnum.CheckResFunc, objArray));
                }
            }
        }

        private void callUIFunc(ConvertFuncEnum funcType, object args)
        {
            switch (funcType)
            {
                case ConvertFuncEnum.DownloadClientFunc:
                    if (_clienDownloadFinishConvertCallback != null)
                        _clienDownloadFinishConvertCallback((bool)args);
                    break;
                case ConvertFuncEnum.FinishFunc:
                    object[] arg2 = (object[])args;
                    if (_finishConvertCallback != null)
                        _finishConvertCallback((bool)arg2[0], (int)arg2[1]);
                    break;
                case ConvertFuncEnum.NoticeFunc:
                    if (_downloadNoticeConvertCallback != null)
                        _downloadNoticeConvertCallback((int)args);
                    break;
                case ConvertFuncEnum.TransFunc:
                    if (_transFinishConvertCallback != null)
                        _transFinishConvertCallback((bool)args);
                    break;
                case ConvertFuncEnum.ActionCallFunc:
                    if (_actionCall != null)
                        _actionCall(null);
                    break;
                case ConvertFuncEnum.CheckResFunc:
                    {
                        object[] objArray2 = (object[])args;
                        if (this._myActionCall != null)
                        {
                            this._myActionCall((string)objArray2[0], (bool)objArray2[1], objArray2[2]);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 将这个Update函数放到UI线程的Update方法里面调用
        /// </summary>
        public void Update()
        {
            if (!_useUpdate)
            {
                _useUpdate = true;
            }

            if (_counter > 0)
            {
                lock (_locker)
                {
                    for (int i = 0; i < _argList.Count; ++i)
                    {
                        _counter--;
                        callUIFunc(_argList[i].Key, _argList[i].Value);
                    }

                    _argList.Clear();
                }
            }
        }

        /// <summary>
        /// 获取预加载进度信息，在UIUpdateForm使用
        /// </summary>
        /// <param name="total"></param>
        /// <param name="loaded"></param>
        /// <returns></returns>
        public bool ShowPreloadPregress()
        {
            return _showPreloadRes;
        }

        public int GetPreloadTotal()
        {
            return _totalPreloadRes;
        }

        public int GetPreloadedCount()
        {
            return _loadedRes;
        }

        public PlatformType GetPlatformType()
        {
            return _platformType;
        }
    }
}
