using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

namespace DBServer
{
    class MqDataProcessManager
    {
        bool IsRunning = false;

        Int32 RunningThreadCount = 0;

        ConcurrentQueue<(int,byte[])> WorkQueue = new ();
        List<System.Threading.Thread> ThreadList = new ();
                
        MqDataProcess ReqProcess = new MqDataProcess();

        

        public void Init(UInt16 myServerIndex, int threadCount, Action<Int32, string, byte[], int> mqSendFunc)
        {
            ReqProcess.Init(myServerIndex, mqSendFunc);

            for (int i = 0; i < threadCount; ++i)
            {
                ThreadList.Add(new System.Threading.Thread(this.ThreadFunc));
            }
        }

        public void Start()
        {
            IsRunning = true;

            foreach (var thread in ThreadList)
            {
                ++RunningThreadCount;
                thread.Start();
            }
        }

        public void Stop()
        {
            IsRunning = false;

            while (true)
            {
                if (RunningThreadCount == 0)
                {
                    break;
                }

                System.Threading.Thread.Yield();
           }
        }

        public void AddReqData(int index, byte[] data)
        {
            WorkQueue.Enqueue((index, data));
        }


        void ThreadFunc()
        {
            while (IsRunning)
            {
                if (WorkQueue.TryDequeue(out var work))
                {
                    ReqProcess.ReqProcess(work.Item1, work.Item2);
                }
                else
                {
                    System.Threading.Thread.Sleep(1);
                }
            }

            System.Threading.Interlocked.Decrement(ref RunningThreadCount);
        }

        
    }
}
