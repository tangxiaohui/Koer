namespace UpdateSystem.Download
{
    using UpdateSystem.Data;
    using UpdateSystem.Delegate;
    using System;

    public class ThreadMapDataPool : ThreadPool<MapFileData>
    {
        public ThreadMapDataPool(int maxThreadCount, ThreadPoolAction<MapFileData> action) : base(maxThreadCount, action)
        {
        }

        public override MapFileData PopData()
        {
            lock (base._lockObj)
            {
                MapFileData data = base.PopData();
                while ((data != null) && (data.Downloading || data.Downloaded))
                {
                    data = base.PopData();
                }
                if (data != null)
                {
                    data.Downloading = true;
                }
                return data;
            }
        }
    }
}

