# Performance

## Simulation speed

The simulation uses explicit fixed-step physics so wall-clock execution is separated from game time. A high Unity time scale can accelerate the perceived simulation timeline, but it does not by itself guarantee millisecond wall-clock execution.

Measure at least:

- queue wait time
- board acquisition time
- physics step count
- simulation wall-clock time
- serialization time
- end-to-end request latency

## Capacity

With ten isolated boards, up to ten simulations can own a board concurrently inside a worker. Additional requests remain queued until a board is released.

## Optimization priorities

1. Keep board state isolated and reusable.
2. Avoid Instantiate/Destroy during a shot.
3. Reuse token and response objects where profiling justifies it.
4. Keep collision geometry simple.
5. Pin physics settings for reproducibility.
6. Profile actual builds rather than extrapolating from Editor timings.
