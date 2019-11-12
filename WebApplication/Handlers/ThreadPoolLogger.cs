using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace WebApplication
{
    /// <summary>
    /// Demo code. never do this in 
    /// </summary>
    public class ThreadPoolLogger : IDisposable
    {
        private bool _disposed;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Thread _workerThread;
        private readonly BroadcastBlock<ThreadPoolStats> _BroadCastBlock;

        public ISourceBlock<ThreadPoolStats> SourceBlock => _BroadCastBlock;

        public ThreadPoolLogger()
        {
            _BroadCastBlock = new BroadcastBlock<ThreadPoolStats>(MapFunc);
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
                       AvailableWorkerThreads = workerThreads,
                       AvailableCompletionPortThreads = completionPortThreads,
                       MaxWorkerThreads = maxWorkerThreads,
                       MaxCompletionPortThreads = maxCompletionPortThreads,
                       MinWorkerThreads = minWorkerThreads,
                       MinCompletionPortThreads = minCompletionPortThreads
                   };

                   _BroadCastBlock.SendAsync(stats).GetAwaiter().GetResult();

                   await Task.Delay(10, token);
               }
           })
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };

        }

        private ThreadPoolStats MapFunc(ThreadPoolStats arg)
        {
            return new ThreadPoolStats()
            {
                DateTime = new DateTimeOffset(year: arg.DateTime.Year, month: arg.DateTime.Month, day: arg.DateTime.Day, hour: arg.DateTime.Hour, minute: arg.DateTime.Minute, second: arg.DateTime.Second, millisecond: arg.DateTime.Millisecond, offset: new TimeSpan(days: arg.DateTime.Offset.Days, hours: arg.DateTime.Offset.Hours, minutes: arg.DateTime.Offset.Minutes, seconds: arg.DateTime.Offset.Seconds, milliseconds: arg.DateTime.Offset.Milliseconds)),
                AvailableWorkerThreads = arg.AvailableWorkerThreads,
                AvailableCompletionPortThreads = arg.AvailableCompletionPortThreads,
                MaxWorkerThreads = arg.MaxWorkerThreads,
                MaxCompletionPortThreads = arg.MaxCompletionPortThreads,
                MinWorkerThreads = arg.MinWorkerThreads,
                MinCompletionPortThreads = arg.MinCompletionPortThreads
            };
        }

        public void Start()
        {
            if ((_workerThread.ThreadState & ThreadState.Unstarted) == ThreadState.Unstarted)
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