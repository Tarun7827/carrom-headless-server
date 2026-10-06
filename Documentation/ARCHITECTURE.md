# Architecture

## Runtime boundaries

1. Transport accepts untrusted messages.
2. Serialization and validation convert them into a typed request.
3. SimulationQueue provides back-pressure.
4. BoardPool provides exclusive physics worlds.
5. SimulationRunner owns the explicit Physics2D stepping loop.
6. Result serialization returns only server-observed state.

## Concurrency rule

Never use an isAvailable tag as a lock. Board lifecycle state is authoritative. A board is acquired exactly once before simulation and released only after reset.

## Failure handling

Malformed requests are rejected at the boundary. A simulation exception should fault the board, emit an operational error, and prevent that board from returning to the available pool until reset and health checks succeed.

## Scaling

A worker owns N isolated boards. Add workers horizontally behind a gateway when throughput requirements exceed one Unity process.
