namespace WebApplication
{
    public class ThreadPoolStats
    {
        public int WorkerThreads { get; set; }
        public int CompletionPortThreads { get; set; }
        public int MaxWorkerThreads { get; set; }
        public int MaxCompletionPortThreads { get; set; }
        public int MinWorkerThreads { get; set; }
        public int MinCompletionPortThreads { get; set; }
    }
}