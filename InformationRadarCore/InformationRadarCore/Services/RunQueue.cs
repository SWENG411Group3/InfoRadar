
using InformationRadarCore.Models;
using System.Collections.Concurrent;

namespace InformationRadarCore.Services
{
    public class RunQueue : IRunQueue
    {
        delegate Task ProcessLighthouse(int lighthouse, bool boolVal);

        private class LighthouseManager
        {
            // When running a visitor, the bool determines whether or not the process should invoke the google spider
            // For messengers, it does nothing
            private readonly ConcurrentDictionary<int, bool> running = new();
            private int count = 0; // atomic ints needed because enumerable counts aren't thread safe

            private readonly ConcurrentQueue<int> queue = new();
            private readonly ConcurrentDictionary<int, bool> enqueued = new(); // To prevent double queuing
            private int queueCount = 0;

            private readonly int MAX_WORKERS;
            private readonly ProcessLighthouse processor;
            
            public LighthouseManager(int workers, ProcessLighthouse callback)
            {
                MAX_WORKERS = workers;
                processor = callback;
            }

            // Returns true if lighthouse is run or queued
            // Returns false if lighthouse is already in system
            public bool Add(int lighthouse, bool boolVal = false)
            {
                if (running.ContainsKey(lighthouse) || enqueued.ContainsKey(lighthouse))
                {
                    return false;
                }

                if (count < MAX_WORKERS)
                {
                    Task.Run(() => Run(lighthouse, boolVal));
                }
                else
                { 
                    Enqueue(lighthouse, boolVal);
                }

                return true;
            }

            private (int, bool)? Dequeue()
            {
                int lighthouse;
                bool boolVal;
                if (queue.TryDequeue(out lighthouse) && enqueued.TryRemove(lighthouse, out boolVal))
                {
                    Interlocked.Decrement(ref queueCount);
                    return (lighthouse, boolVal);
                }

                return null;
            }

            private void Enqueue(int lighthouse, bool boolVal)
            {
                if (enqueued.TryAdd(lighthouse, boolVal))
                {
                    queue.Enqueue(lighthouse);
                    Interlocked.Increment(ref queueCount);
                }
            }

            private void Run(int lighthouse, bool boolVal)
            {
                running.TryAdd(lighthouse, boolVal);
                Interlocked.Increment(ref count);

                processor(lighthouse, boolVal).Wait();

                bool _;
                running.TryRemove(lighthouse, out _);
                Interlocked.Decrement(ref count);

                if (count < MAX_WORKERS && queueCount > 0)
                {
                    var removed = Dequeue();
                    if (removed.HasValue)
                    {
                        var (l, b) = removed.Value;
                        Task.Run(() => Run(l, b));
                    }
                }

            }
        }

        private readonly IScrapyInterface scrapy;

        private readonly ILogger<RunQueue> logger;
        private readonly LighthouseManager visitors, messengers;

        public RunQueue(IScrapyInterface scrapy, ConfigService config, ILogger<RunQueue> logger)
        {
            this.scrapy = scrapy;
            this.logger = logger;

            var maxWorkers = config.MaxLighthouseWorkers ?? 10;
            visitors = new LighthouseManager(maxWorkers, VisitorWrapper);
            messengers = new LighthouseManager(maxWorkers, MessengerWrapper);
        }

        private async Task VisitorWrapper(int lighthouse, bool boolVal)
        {
            try
            {
                await scrapy.Visitor(lighthouse, boolVal);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in visitor process for lighthouse {0}.  Search running = {1}", lighthouse, boolVal);
            }
        }

        private async Task MessengerWrapper(int lighthouse, bool _)
        {
            try
            {
                await scrapy.Messenger(lighthouse);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in messenger process for lighthouse {0}", lighthouse);
            }
        }

        public bool StartVisitor(int lighthouse, bool runSearch)
        {
            return visitors.Add(lighthouse, runSearch);
        }

        public bool StartMessenger(int lighthouse)
        {
            return messengers.Add(lighthouse);
        }
    }
}
