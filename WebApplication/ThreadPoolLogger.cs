using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication
{
    /// <summary>
    /// Demo code. never do this in 
    /// </summary>
    public class ThreadPoolLogger : IDisposable
    {
        private bool _disposed;
        private CancellationTokenSource _cancellationTokenSource;
        private Thread _workerThread;

        public event ThreadStats OnNewStats;

        public delegate Task ThreadStats(ThreadPoolStats threadPoolStats);

        public ThreadPoolLogger()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _workerThread = new Thread(async source =>
           {
               var token = (CancellationToken)source;
               while (token.IsCancellationRequested == false)
               {
                   ThreadPool.GetAvailableThreads(out int workerThreads, out int completionPortThreads);
                   ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);
                   ThreadPool.GetMinThreads(out int minWorkerThreads, out int minCompletionPortThreads);

                   var stats = new ThreadPoolStats
                   {
                       WorkerThreads = workerThreads,
                       CompletionPortThreads = completionPortThreads,
                       MaxWorkerThreads = maxWorkerThreads,
                       MaxCompletionPortThreads = maxCompletionPortThreads,
                       MinWorkerThreads = minWorkerThreads,
                       MinCompletionPortThreads = minCompletionPortThreads
                   };

                   if (OnNewStats != null)
                   {
                       await OnNewStats(stats);
                   }

                   await Task.Delay(1, token);
               }
           })
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };

        }

        public void Start()
        {
            if (_workerThread.ThreadState == ThreadState.Unstarted)
            {
                _workerThread.Start(_cancellationTokenSource.Token);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool dispose)
        {
            if (dispose && _disposed == false)
            {
                _disposed = true;
                _cancellationTokenSource.Cancel();
                _workerThread.Abort();
            }
        }
    }
}