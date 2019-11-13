using System;

namespace WebApplication
{
    public class ThreadPoolStats
    {
        public DateTimeOffset DateTime { get; set; } = DateTimeOffset.UtcNow;
        public int AvailableWorkerThreads { get; set; } = 0;
        public int AvailableCompletionPortThreads { get; set; } = 0;
        public int MaxWorkerThreads { get; set; } = 0;
        public int MaxCompletionPortThreads { get; set; } = 0;
        public int MinWorkerThreads { get; set; } = 0;
        public int MinCompletionPortThreads { get; set; } = 0;
        public bool Error { get; set; } = false;
    }
}