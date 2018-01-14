namespace UpdateSystem.Download
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class MyThread
    {
        private int _id;
        private Thread _threadIns;
        private ManualResetEvent _threadManger;
        internal string Name;

        public MyThread(int id)
        {
            this._id = id;
            this._threadManger = new ManualResetEvent(false);
        }

        public void Pause()
        {
            this._threadManger.Reset();
            this._threadManger.WaitOne();
        }

        public void Resume()
        {
            this._threadManger.Set();
        }

        public void Start(ThreadStart run)
        {
            this._threadIns = new Thread(run);
            this._threadIns.Name = "MyThread : " + this._id;
            this._threadIns.Start();
        }
    }
}

