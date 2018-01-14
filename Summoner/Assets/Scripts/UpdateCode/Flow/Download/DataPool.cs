namespace UpdateSystem.Download
{
    using UpdateSystem.Data;
    using System;

    public class DataPool<T> where T: new()
    {
        //高优先级下载队列
        private SyncQueue<T> _highPriorityQueue;
        //下载失败的数据，在高优先级队列前自动下载
        private SyncQueue<T> _highPriorityFailQueue;
        //当前场景的下载队列
        private SyncQueue<T> _curSceneQueue;
        //下载失败的数据，在当前场景下载前自动下载
        private SyncQueue<T> _curSceneFailQueue;
        //下一场景的预下载队列
        private SyncQueue<T> _nextSceneQueue;
        //下载失败的数据，在预下载队列前自动下载
        private SyncQueue<T> _nextSceneFailQueue;
        //低优先级，静默下载队列
        private SyncQueue<T> _lowQueue;
        //下载失败的数据，在静默下载队列前下载
        private SyncQueue<T> _lowFailQueue;

        //下载失败的数据，非中断操作和切换网络操作造成的
        //在合适的时间，自动做下载（比如所有资源下载完成后）
        private SyncQueue<T> _allFailQueue;

        private object _lockObj;

        //暂停低优先级队列，即总资源队列
        private bool _pauseLowQueue;
        //暂停当前场景
        private bool _pauseCurSceneQueue;
        //暂停下一个场景队列
        private bool _pauseNextSceneQueue;
        //暂停高优先级队列
        private bool _pauseHighPriorityQueue;

        public DataPool()
        {
            _lockObj = new object();

            _highPriorityQueue = new SyncQueue<T>();
            _highPriorityFailQueue = new SyncQueue<T>();

            _curSceneQueue = new SyncQueue<T>();
            _curSceneFailQueue = new SyncQueue<T>();

            _nextSceneQueue = new SyncQueue<T>();
            _nextSceneFailQueue = new SyncQueue<T>();

            _lowQueue = new SyncQueue<T>();
            _lowFailQueue = new SyncQueue<T>();

            _allFailQueue = new SyncQueue<T>();
        }

        /// <summary>
        /// 添加数据 level：数据类型   fail：是否下载失败的数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="level"></param>
        /// <param name="fail"></param>
        internal void AddData(T data, DataLevel level, bool fail = false)
        {
            lock (this._lockObj)
            {
                switch (level)
                {
                    case DataLevel.High:
                        if (fail) 
                            _highPriorityFailQueue.Enqueue(data);
                        else
                            _highPriorityQueue.Enqueue(data);
                        break;

                    case DataLevel.CurScene:
                        if (fail)
                            _curSceneFailQueue.Enqueue(data);
                        else
                            _curSceneQueue.Enqueue(data);
                        break;

                    case DataLevel.NextScene:
                        if (fail)
                            _nextSceneFailQueue.Enqueue(data);
                        else
                            _nextSceneQueue.Enqueue(data);
                        break;
                    case DataLevel.Low:
                        if (fail)
                            _lowFailQueue.Enqueue(data);
                        else
                            _lowQueue.Enqueue(data);
                        break;
                    default:
                        if (fail)
                            _lowFailQueue.Enqueue(data);
                        else
                            _lowQueue.Enqueue(data);
                        break;
                }
            }
        }

        internal void AddFailData(T data)
        {
            _allFailQueue.Enqueue(data);
        }

        public void Clear()
        {
            this._curSceneQueue.Clear();
            this._highPriorityQueue.Clear();
            this._nextSceneQueue.Clear();
            this._lowQueue.Clear();
            _curSceneFailQueue.Clear();
            _highPriorityFailQueue.Clear();
            _nextSceneFailQueue.Clear();
            _lowFailQueue.Clear();
            _allFailQueue.Clear();
        }

        internal void ClearQueue(DataLevel level)
        {
            lock (this._lockObj)
            {
                switch (level)
                {
                    case DataLevel.High:
                        this._highPriorityQueue.Clear();
                        _highPriorityFailQueue.Clear();
                        break;

                    case DataLevel.CurScene:
                        this._curSceneQueue.Clear();
                        _curSceneFailQueue.Clear();
                        break;

                    case DataLevel.NextScene:
                        this._nextSceneQueue.Clear();
                        _nextSceneFailQueue.Clear();
                        break;
                    case DataLevel.Low:
                        this._lowQueue.Clear();
                        _lowFailQueue.Clear();
                        break;
                }
            }
        }

        internal int GetHighPriorityQueueCount()
        {
            lock (this._lockObj)
            {
                int count = GetDataCount(DataLevel.High);
                return count;
            }
        }

        internal int GetDataCount(DataLevel level)
        {
            lock (_lockObj)
            {
                int count = 0;
                switch (level)
                {
                    case DataLevel.Low:
                        count = _lowQueue.Count + _lowFailQueue.Count;
                        break;
                    case DataLevel.NextScene:
                        count = _nextSceneQueue.Count + _nextSceneFailQueue.Count;
                        break;
                    case DataLevel.CurScene:
                        count = _curSceneQueue.Count + _curSceneFailQueue.Count;
                        break;
                    case DataLevel.High:
                        count = _highPriorityQueue.Count + _highPriorityFailQueue.Count;
                        break;
                }

                return count;
            }
        }

        internal bool IsBaseResPaused()
        {
            return _pauseLowQueue;
        }

        internal bool IsCurScenePaused()
        {
            return _pauseCurSceneQueue;
        }

        internal void PauseLevel(DataLevel level)
        {
            SetLevelPause(level, true);
        }

        internal void ResumeLevel(DataLevel level)
        {
            SetLevelPause(level, false);
        }

        private void SetLevelPause(DataLevel level, bool pause)
        {
            switch (level)
            {
                case DataLevel.Low:
                    _pauseLowQueue = pause;
                    break;
                case DataLevel.NextScene:
                    _pauseNextSceneQueue = pause;
                    break;
                case DataLevel.CurScene:
                    _pauseCurSceneQueue = pause;
                    break;
                case DataLevel.High:
                    _pauseHighPriorityQueue = pause;
                    break;
                case DataLevel.All:
                    _pauseHighPriorityQueue = pause;
                    _pauseLowQueue = pause;
                    _pauseNextSceneQueue = pause;
                    _pauseCurSceneQueue = pause;
                    break;
            }
        }

        public T PopData()
        {
            lock (this._lockObj)
            {
                T data = default(T);
                if (GetHighPriorityData(out data))
                    return data;
                if (GetCurSceneData(out data))
                    return data;
                if (GetNextSceneData(out data))
                    return data;
                if (GetLowPriorityData(out data))
                    return data;

                return data;
            }
        }

        private bool GetHighPriorityData(out T data)
        {
            bool ret = false;
            data = default(T);

            if (_pauseHighPriorityQueue)
            {
                return ret;
            }

            if (_highPriorityFailQueue.Count > 0)
            {
                data = _highPriorityFailQueue.Dequeue();
                ret = true;
            }
            else if (_highPriorityQueue.Count > 0)
            {
                data = _highPriorityQueue.Dequeue();
                ret = true;
            }

            return ret;
        }

        private bool GetCurSceneData(out T data)
        {
            bool ret = false;
            data = default(T);

            if (_pauseCurSceneQueue)
            {
                return ret;
            }

            if (_curSceneFailQueue.Count > 0)
            {
                data = _curSceneFailQueue.Dequeue();
                ret = true;
            }
            else if (_curSceneQueue.Count > 0)
            {
                data = _curSceneQueue.Dequeue();
                ret = true;
            }

            return ret;
        }

        private bool GetNextSceneData(out T data)
        {
            bool ret = false;
            data = default(T);

            if (_pauseNextSceneQueue)
            {
                return ret;
            }

            if (_nextSceneFailQueue.Count > 0)
            {
                data = _nextSceneFailQueue.Dequeue();
                ret = true;
            }
            else if (_nextSceneQueue.Count > 0)
            {
                data = _nextSceneQueue.Dequeue();
                ret = true;
            }

            return ret;
        }

        private bool GetLowPriorityData(out T data)
        {
            bool ret = false;
            data = default(T);

            if (_pauseLowQueue)
            {
                return ret;
            }

            if (_lowFailQueue.Count > 0)
            {
                data = _lowFailQueue.Dequeue();
                ret = true;
            }
            else if (_lowQueue.Count > 0)
            {
                data = _lowQueue.Dequeue();
                ret = true;
            }

            return ret;
        }
    }
}

