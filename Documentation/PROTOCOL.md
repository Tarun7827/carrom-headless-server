# Protocol

## Request

Required: requestId, striker position, striker velocity, token ids and token positions.

## Response

The server returns requestId, success or error, boardId, simulation wall-clock measurement, final striker state, final token positions, and pocket events.

The client never submits a claimed result for the server to accept.
