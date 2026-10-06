using System.Collections.Generic;
using UnityEngine;

namespace CarromHeadless.Physics
{
    public sealed class PocketDetector : MonoBehaviour
    {
        private readonly List<PocketEvent> events = new();
        public IReadOnlyList<PocketEvent> Events => events;
        public void Clear() => events.Clear();
        public void RegisterPocket(string tokenId, int pocketId) => events.Add(new PocketEvent(tokenId, pocketId));
    }
}
