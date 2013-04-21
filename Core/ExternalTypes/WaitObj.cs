using System.Threading;

namespace Core.ExternalTypes
{
    public class WaitObj
    {
        public int Count;
        public AutoResetEvent WaitEvent;

        public WaitObj(int count)
        {
            Count = count;
            WaitEvent = new AutoResetEvent(false);
        }
    }

    class SyncWaitObj : WaitObj
    {
        public SyncWaitObj(int count) : base(count) { }

        public double MultidownloadRate
        {
            get { return _multidownloadRate; }
            set
            {
                lock (sync)
                {
                    _multidownloadRate = value;
                }
            }
        }
        double _multidownloadRate;

        static object sync = new object();
    }
}
