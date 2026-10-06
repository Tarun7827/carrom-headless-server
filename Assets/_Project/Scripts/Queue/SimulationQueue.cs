using System.Collections.Concurrent;
using CarromHeadless.Requests;

namespace CarromHeadless.Queue
{
    public sealed class SimulationQueue
    {
        private readonly ConcurrentQueue<SimulationRequest> queue = new();
        private readonly int capacity;
        public SimulationQueue(int capacity) { this.capacity = capacity; }
        public int Count => queue.Count;
        public bool TryEnqueue(SimulationRequest request)
        {
            if (queue.Count >= capacity) return false;
            queue.Enqueue(request);
            return true;
        }
        public bool TryDequeue(out SimulationRequest request) => queue.TryDequeue(out request);
    }
}
