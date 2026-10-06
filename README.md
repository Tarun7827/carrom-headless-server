# Carrom Headless Simulation Server

Production-oriented Unity 6 headless simulation worker for authoritative Carrom shot validation.

## What is in this repository

This repository is based only on the supplied Unity project. The server creates its simulation boards at runtime, so no visual assets are required for the headless worker.

```text
WebSocket client
      │
      ▼
RFC6455 gateway ──► request validation ──► bounded concurrent queue
                                                │
                                                ▼
                                      isolated board pool (N)
                                                │
                                                ▼
                                  restore state + apply shot
                                                │
                                                ▼
                                   Physics2D.Simulate @ 120Hz
                                                │
                                                ▼
                                  authoritative result + reset
                                                │
                                                ▼
                                      WebSocket response
```

## Runtime model

- Default pool: 10 isolated boards.
- Physics: explicit `Physics2D.Simulate` at 120 Hz.
- Networking: dependency-free RFC6455 text/ping/pong/close support.
- Queue: bounded to provide backpressure instead of unbounded memory growth.
- Unity API access: simulation is executed on Unity's main loop; networking may run concurrently.
- Board state: pool ownership is the concurrency source of truth. `BoardAvailable` / `BoardBusy` tags are diagnostic only.
- Token objects are reused per board to avoid creating an unbounded number of GameObjects across requests.
- Requests are validated for size, duplicate IDs, finite numeric values, velocity limits and maximum token count.
- Graceful shutdown stops accepting new work and closes the listener.

## Request

```json
{
  "requestId":"shot-001",
  "striker":{"position":{"x":0,"y":-3.5},"velocity":{"x":2.5,"y":12}},
  "tokens":[{"id":"black-01","type":"black","position":{"x":0,"y":0},"velocity":{"x":0,"y":0}}],
  "maxSimulationSeconds":8
}
```

## Response

```json
{
  "requestId":"shot-001",
  "success":true,
  "boardId":3,
  "simulationTimeMs":4.2,
  "simulatedSeconds":2.1,
  "strikerFinalPosition":{"x":1.1,"y":-3.2},
  "strikerFinalVelocity":{"x":0,"y":0},
  "strikerPocketed":false,
  "tokens":[{"id":"black-01","type":"black","finalPosition":{"x":0.3,"y":0.4},"finalVelocity":{"x":0,"y":0},"pocketed":true}]
}
```

## Configuration

The example configuration documents the supported runtime settings. The worker reads environment variables and command-line overrides so the same build can be deployed across environments.

| Setting | Environment | CLI | Default |
|---|---|---|---:|
| Port | `CARROM_PORT` | `-port` | 8080 |
| Boards | `CARROM_BOARD_COUNT` | `-boardCount` | 10 |
| Queue | `CARROM_QUEUE_CAPACITY` | `-queueCapacity` | 1000 |
| Time scale | `CARROM_TIME_SCALE` | `-timeScale` | 50 |
| Max simulation seconds | `CARROM_MAX_SIMULATION_SECONDS` | `-maxSimulationSeconds` | 8 |

Example:

```bash
./CarromHeadlessServer.x86_64 -batchmode -nographics -port 8080 -boardCount 10 -queueCapacity 1000
```

## Unity build

Open with **Unity 6**, select a Linux standalone target and build the project as a headless/server worker. The runtime bootstrap creates the simulation server before the first scene is loaded, so the worker does not depend on a rendered scene.

Recommended launch flags:

```bash
-batchmode -nographics -port 8080
```

The included `Assets/Scenes/Headless.unity` is retained from the supplied project; runtime bootstrap is the authoritative startup path.

## Production deployment notes

This worker is designed to be horizontally scalable: run multiple identical headless processes behind a gateway and route a match/session consistently to one worker when state affinity is required.

For production game validation, pin the exact Unity version and physics settings, keep the same simulation constants across workers, record a server/build version with authoritative results, and load-test the queue/board ratio before setting capacity limits.

The WebSocket implementation is intentionally small and dependency-free for a controlled service boundary. For internet-facing deployment, terminate TLS and apply authentication, rate limiting, connection limits and observability at the gateway/load-balancer layer.

## Local test

Install the Python WebSocket client dependency:

```bash
pip install websockets
python test_client.py
```

## License

MIT
