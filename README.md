# Carrom Headless Server

A server-authoritative Carrom simulation service built around Unity headless physics.

This project demonstrates how a real-time multiplayer game can validate a client shot on the server instead of trusting client-reported outcomes. Each request is assigned to an isolated board instance, simulated deterministically for the required physics steps, and returned as authoritative state.

## Architecture

```text
Client
  │ WebSocket
  ▼
Request Gateway
  │ validate / deserialize
  ▼
Concurrent Simulation Queue
  │
  ▼
Board Pool ──► acquire isolated board
  │
  ▼
Restore State → Apply Shot → Physics2D.Simulate
  │                              │
  │                              ├─ collision detection
  │                              └─ pocket detection
  ▼
Capture Authoritative Result
  │
  ▼
Reset Board → Release to Pool
  │
  ▼
Response Gateway
  │
  ▼
Client
```

## Why server-authoritative?

A client should submit an intent (board state + striker input), not the result of the shot. The server owns the simulation and decides:

- final striker position and velocity
- token positions
- pocketed tokens
- collision-driven state transitions
- whether the request is valid

This prevents a modified client from simply reporting a favorable outcome.

## Board pool and concurrency

The server maintains a fixed pool of isolated simulation boards. A board transitions through:

`Available → Reserved → Simulating → Resetting → Available`

A concurrent queue absorbs requests while the board pool controls exclusive ownership. Tags are useful for editor/debug visibility, but pool state is the concurrency source of truth.

The sample configuration contains 10 boards.

## Simulation loop

The headless runner advances Unity physics explicitly:

```csharp
while (!simulationFinished)
{
    Physics2D.Simulate(fixedDeltaTime);
    DetectPocketEvents();
    DetectStoppedBodies();

    if (AllBodiesStopped())
        simulationFinished = true;
}
```

`Time.timeScale` can accelerate simulated time, but it does not guarantee wall-clock latency. Actual latency depends on physics complexity, CPU, collision count, and execution model.

## Request

```json
{
  "requestId": "match_123_shot_456",
  "boardState": {
    "tokens": [
      { "id": "black_01", "position": { "x": 0.0, "y": 0.0 } }
    ]
  },
  "striker": {
    "position": { "x": -2.15, "y": -4.2 },
    "velocity": { "x": 8.4, "y": 12.7 }
  }
}
```

## Response

```json
{
  "requestId": "match_123_shot_456",
  "success": true,
  "boardId": 7,
  "simulationTimeMs": 3.84,
  "striker": {
    "finalPosition": { "x": -1.42, "y": 2.83 },
    "finalVelocity": { "x": 0.0, "y": 0.0 }
  },
  "pocketedTokens": [
    { "id": "black_01", "pocketId": 2 }
  ],
  "tokens": [
    { "id": "white_01", "position": { "x": 1.21, "y": -0.73 } }
  ]
}
```

## Unity structure

```text
Assets/_Project/
├── Prefabs/
│   ├── Board/
│   ├── Pieces/
│   └── Server/
├── Scenes/
│   ├── Bootstrap.unity
│   └── Simulation.unity
└── Scripts/
    ├── Board/
    ├── Configuration/
    ├── Core/
    ├── Logging/
    ├── Networking/
    ├── Physics/
    ├── Pieces/
    ├── Queue/
    ├── Requests/
    ├── Serialization/
    └── Utilities/
```

## Engineering goals

- isolate simulations so concurrent requests cannot share mutable physics state
- avoid allocations in hot simulation paths where practical
- validate all client-controlled values at the request boundary
- reset every board before returning it to the pool
- make simulation completion explicit instead of relying on arbitrary delays
- keep transport, queueing, simulation, and serialization independently testable

## Production considerations

For production-grade deterministic validation, pin Unity and physics versions, control fixed timestep and solver settings, avoid nondeterministic inputs, and record a simulation/version identifier with every authoritative result.

Horizontal scaling is straightforward: run multiple headless workers, each with its own board pool, behind a gateway that routes a match/session consistently.

## Status

This repository is an architecture-focused reference implementation and portfolio project. Networking is represented behind a transport abstraction so the Unity simulation core remains independently testable.

## License

MIT
