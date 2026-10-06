using System;
using CarromHeadless.Board;
using CarromHeadless.Requests;
using UnityEngine;

namespace CarromHeadless.Physics
{
    public sealed class SimulationRunner
    {
        public SimulationResponse Run(BoardInstance board, SimulationRequest request, float fixedDeltaTime, int maxSteps, float sleepVelocityThreshold)
        {
            board.BeginSimulation();
            var started = DateTime.UtcNow;
            board.Restore(request);

            for (var step = 0; step < maxSteps; step++)
            {
                Physics2D.Simulate(fixedDeltaTime);
                if (AllBodiesStopped(board, sleepVelocityThreshold))
                    break;
            }

            var response = new SimulationResponse
            {
                requestId = request.requestId,
                success = true,
                boardId = board.Id,
                simulationTimeMs = (DateTime.UtcNow - started).TotalMilliseconds,
                striker = new StrikerState { finalPosition = board.Striker.position, finalVelocity = board.Striker.velocity }
            };

            foreach (var token in request.tokens)
            {
                if (board.Tokens.TryGetValue(token.id, out var body))
                    response.tokens.Add(new TokenState { id = token.id, position = body.position });
            }

            return response;
        }

        private static bool AllBodiesStopped(BoardInstance board, float threshold)
        {
            var limit = threshold * threshold;
            if (board.Striker.velocity.sqrMagnitude > limit) return false;
            foreach (var body in board.Tokens.Values)
                if (body.velocity.sqrMagnitude > limit) return false;
            return true;
        }
    }
}
