using System.Diagnostics;

namespace CarromHeadless.Utilities
{
    public readonly struct StopwatchScope
    {
        private readonly Stopwatch stopwatch;

        public StopwatchScope(out StopwatchScope scope)
        {
            scope = new StopwatchScope(true);
            stopwatch = scope.stopwatch;
        }

        private StopwatchScope(bool start)
        {
            stopwatch = Stopwatch.StartNew();
        }

        public double ElapsedMilliseconds => stopwatch.Elapsed.TotalMilliseconds;
    }
}
