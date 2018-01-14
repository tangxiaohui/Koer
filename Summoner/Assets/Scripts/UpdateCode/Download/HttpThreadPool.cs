using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using System.IO;
using UpdateSystem.Log;
using UpdateSystem.Delegate;

namespace UpdateSystem.Download
{
    /// <summary>
    /// 线程池
    /// </summary>
    public class HttpThreadPool<T>
    {

        int _count;

        bool _stop;
        bool _waitWhileWorking;

        ThreadPoolAction<T> _action;

        List<Thread> _threadQueue;
        Queue<T> _taskList;

        public HttpThreadPool(int maxThreadCount, ThreadPoolAction<T> action)
        {
            _action = action;

            _count = maxThreadCount;
            _stop = false;
            _waitWhileWorking = false;

            _threadQueue = new List<Thread>();
            _taskList = new Queue<T>();

            for (int i = 0; i < _count; i++)
            {
                Thread thread = new Thread(threadFunc);
                thread.Name = "download thread: " + i;
                thread.Start();
                
                _threadQueue.Add(thread);
                
            }
        }

        public void waitWhileWorking()
        {
            _waitWhileWorking = true;

            join();
        }

        public void stop()
        {
            _stop = true;
            //Thread.Sleep(100);
            //join();
        }

        //尽量不用这个方法， ios的IL2CPP调用thread的abort的时候会崩溃
        public void Abort()
        {
            for (int i = 0; i < _threadQueue.Count; ++i)
            {
                if (_threadQueue[i] != null && _threadQueue[i].IsAlive)
                {
                    _threadQueue[i].Abort();
                }
            }
        }

        private void join()
        {
            for (int i = 0; i < _threadQueue.Count; i++)
            {
                _threadQueue[i].Join();
            }

            _taskList.Clear();
            _threadQueue.Clear();
        }

        public void addTask(T data)
        {
            lock (_taskList)
            {
                _taskList.Enqueue(data);
            }
        }

        private T getTask()
        {
            lock (_taskList)
            {
                if (_taskList.Count > 0)
                {
                    return _taskList.Dequeue();
                }

                return default(T);
            }
            
        }

        public void threadFunc()
        {
            while (_stop == false)
            {
                T data = getTask();
                if (data == null)
                {
                    Console.WriteLine(Thread.CurrentThread.Name + " is sleeping");
                    Thread.Sleep(20);
                    if (_waitWhileWorking)
                    {
                        lock (_taskList)
                        {
                            if (_taskList.Count == 0)
                            {
                                return;
                            }
                        }
                    }
                }
                else
                {
                    dowork(data);
                }
            }

            UpdateLog.DEBUG_LOG("Thread abort: " + Thread.CurrentThread.Name);
        }

        private void dowork(T fileData)
        {
            //这里lock的话，就变成单线程下载了，无意义
            //lock (_httpDownloadInstance)
            {
                if (_action != null)
                {
                    _action(fileData);
                }
            }
        }
    }
}
