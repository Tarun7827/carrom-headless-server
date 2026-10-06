using UnityEngine;

namespace CarromHeadless.Configuration
{
    [CreateAssetMenu(menuName = "Carrom/Server Config")]
    public sealed class ServerConfig : ScriptableObject
    {
        [Min(1)] public int boardCount = 10;
        [Min(0.0001f)] public float fixedDeltaTime = 1f / 120f;
        [Min(1)] public int maxSimulationSteps = 4000;
        [Min(0.01f)] public float sleepVelocityThreshold = 0.05f;
        [Min(1)] public int maxQueuedRequests = 1000;
    }
}
