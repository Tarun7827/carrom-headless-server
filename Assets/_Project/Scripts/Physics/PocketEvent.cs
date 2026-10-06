using System;
namespace CarromHeadless.Physics
{
    public readonly struct PocketEvent
    {
        public PocketEvent(string tokenId, int pocketId) { TokenId = tokenId; PocketId = pocketId; }
        public string TokenId { get; }
        public int PocketId { get; }
    }
}
