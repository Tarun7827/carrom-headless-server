using CarromHeadless.Board;
using CarromHeadless.Configuration;
using CarromHeadless.Physics;
using CarromHeadless.Queue;
using CarromHeadless.Requests;
namespace CarromHeadless.Core
{
    public sealed class SimulationService
    {
        private readonly BoardPool boardPool;
        private readonly SimulationQueue queue;
        private readonly ServerConfig config;
        private readonly SimulationRunner runner = new();
        public SimulationService(BoardPool boardPool, ServerConfig config)
        {
            this.boardPool = boardPool;
            this.config = config;
            queue = new SimulationQueue(config.maxQueuedRequests);
        }
        public bool TrySubmit(SimulationRequest request) => queue.TryEnqueue(request);
        public bool TryProcessNext(out SimulationResponse response)
        {
            response = null;
            if (!queue.TryDequeue(out var request)) return false;
            if (!boardPool.TryAcquire(out var board)) return false;
            try
            {
                response = runner.Run(board, request, config.fixedDeltaTime, config.maxSimulationSteps, config.sleepVelocityThreshold);
                return true;
            }
            finally { boardPool.Release(board); }
        }
    }
}
