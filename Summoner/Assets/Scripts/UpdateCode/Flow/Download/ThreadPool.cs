namespace UpdateSystem.Download
{
    using UpdateSystem.Delegate;
    using UpdateSystem.Log;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class ThreadPool<T> where T: new()
    {
        private ThreadPoolAction<T> _action;
        private DataPool<T> _dataPool;
        private MyThread _highPriorityThread;
        protected object _lockObj;
        private bool _pause;
        private bool _stop;
        private ManualResetEvent _threadManger;
        private List<MyThread> _threadQueue;

        public ThreadPool(int maxThreadCount, ThreadPoolAction<T> action)
        {
            _threadQueue = new List<MyThread>();
            _dataPool = new DataPool<T>();
            _lockObj = new object();
            _stop = false;
            _action = action;
            _threadManger = new ManualResetEvent(false);
            for (int i = 0; i < maxThreadCount; i++)
            {
                MyThread item = new MyThread(i);
                item.Name = "download thread: " + i;
                if (_highPriorityThread == null)
                {
                    _highPriorityThread = item;
                    _highPriorityThread.Start(new ThreadStart(HighPriorityThreadFunc));
                }
                else
                {
                    item.Start(new ThreadStart(ThreadFunc));
                }
                _threadQueue.Add(item);
            }
            UpdateLog.WARN_LOG("Init ThreadPool");
        }

        internal void AddData(T data, DataLevel level, bool fail = false)
        {
            _dataPool.AddData(data, level, fail);
            if (!_pause)
            {
                _threadManger.Set();
            }
            if (level == DataLevel.High)
            {
                _highPriorityThread.Resume();
            }
        }

        internal void AddFailData(T data)
        {
            _dataPool.AddFailData(data);
        }

        internal void ClearData(DataLevel level)
        {
            _dataPool.ClearQueue(level);
        }

        private void Dowork(T fileData)
        {
            if (_action != null)
            {
                _action(fileData);
            }
        }

        private T GetTask()
        {
            return PopData();
        }

        private void Join()
        {
            for (int i = 0; i < _threadQueue.Count; i++)
            {
            }
            _dataPool.Clear();
            _threadQueue.Clear();
        }

        /// <summary>
        /// 暂停普通下载队列，除了高优先级队列，其他队列都暂停
        /// </summary>
        public void Pause()
        {
            _pause = true;
            _threadManger.Reset();
        }

        /// <summary>
        /// 暂停指定下载队列
        /// </summary>
        /// <param name="level"></param>
        internal void PauseLevel(DataLevel level)
        {
            _dataPool.PauseLevel(level);
        }

        /// <summary>
        /// 恢复指定下载队列
        /// </summary>
        /// <param name="level"></param>
        internal void ResumeLevel(DataLevel level)
        {
            _dataPool.ResumeLevel(level);
        }

        public virtual T PopData()
        {
            return _dataPool.PopData();
        }

        /// <summary>
        /// 恢复所有下载队列
        /// </summary>
        public void Resume()
        {
            _pause = false;
            _threadManger.Set();
            _highPriorityThread.Resume();
        }

        /// <summary>
        /// 是否暂停中，除了高优先级队列，其他队列都暂停
        /// </summary>
        /// <returns></returns>
        public bool IsPaused()
        {
            return _pause;
        }

        /// <summary>
        /// 是否静默更新暂停中
        /// </summary>
        /// <returns></returns>
        internal bool IsBaseResPaused()
        {
            return _dataPool.IsBaseResPaused();
        }

        /// <summary>
        /// 是否当前场景暂停中
        /// </summary>
        /// <returns></returns>
        internal bool IsCurScenePaused()
        {
            return _dataPool.IsCurScenePaused();
        }

        /// <summary>
        /// 获取对应下载队列中数据个数
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        internal int GetDataCount(DataLevel level)
        {
            return _dataPool.GetDataCount(level);
        }

        private void Stop()
        {
            _stop = true;
            Resume();
        }

        /// <summary>
        /// 高优先级下载
        /// </summary>
        private void HighPriorityThreadFunc()
        {
            while (!_stop)
            {
                if (_dataPool.GetHighPriorityQueueCount() == 0)
                {
                    _highPriorityThread.Pause();
                }
                T task = GetTask();
                if (task == null)
                {
                    if (!_stop)
                    {
                        _highPriorityThread.Pause();
                    }
                }
                else
                {
                    Dowork(task);
                }
            }
            UpdateLog.DEBUG_LOG("Thread abort: " + Thread.CurrentThread.Name);
        }

        /// <summary>
        /// 普通资源下载
        /// </summary>
        private void ThreadFunc()
        {
            while (!_stop)
            {
                if (_pause)
                {
                    UpdateLog.DEBUG_LOG("Thread pause : " + Thread.CurrentThread.Name);
                    _threadManger.Reset();
                    _threadManger.WaitOne();
                }
                T task = GetTask();
                if (task == null)
                {
                    if (!_stop)
                    {
                        _threadManger.Reset();
                        _threadManger.WaitOne();
                    }
                }
                else
                {
                    Dowork(task);
                }
            }
            UpdateLog.DEBUG_LOG("Thread abort: " + Thread.CurrentThread.Name);
        }

        public void WaitForFinish()
        {
            Stop();
            Join();
        }

        public object Locker
        {
            get
            {
                return _lockObj;
            }
        }
    }
}

