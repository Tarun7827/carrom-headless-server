using System;
using System.Collections.Generic;
using UnityEngine;

namespace CarromHeadless.Requests
{
    [Serializable]
    public sealed class SimulationRequest
    {
        public string requestId;
        public StrikerInput striker;
        public List<TokenState> tokens = new();
    }

    [Serializable]
    public sealed class StrikerInput
    {
        public Vector2 position;
        public Vector2 velocity;
    }

    [Serializable]
    public sealed class TokenState
    {
        public string id;
        public Vector2 position;
    }

    [Serializable]
    public sealed class PocketedToken
    {
        public string id;
        public int pocketId;
    }

    [Serializable]
    public sealed class SimulationResponse
    {
        public string requestId;
        public bool success;
        public int boardId;
        public double simulationTimeMs;
        public StrikerState striker;
        public List<PocketedToken> pocketedTokens = new();
        public List<TokenState> tokens = new();
        public string error;
    }

    [Serializable]
    public sealed class StrikerState
    {
        public Vector2 finalPosition;
        public Vector2 finalVelocity;
    }
}
