using CarromHeadless.Queue;
using CarromHeadless.Requests;
using NUnit.Framework;

namespace CarromHeadless.Tests
{
    public sealed class SimulationQueueTests
    {
        [Test]
        public void QueueRejectsRequestsBeyondCapacity()
        {
            var queue = new SimulationQueue(1);
            Assert.That(queue.TryEnqueue(new SimulationRequest { requestId = "one" }), Is.True);
            Assert.That(queue.TryEnqueue(new SimulationRequest { requestId = "two" }), Is.False);
        }
    }
}
