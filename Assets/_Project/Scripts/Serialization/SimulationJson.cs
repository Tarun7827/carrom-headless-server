using UnityEngine;
using CarromHeadless.Requests;
namespace CarromHeadless.Serialization
{
    public static class SimulationJson
    {
        public static bool TryDeserialize(string payload, out SimulationRequest request)
        {
            request = null;
            if (string.IsNullOrWhiteSpace(payload)) return false;
            try
            {
                request = JsonUtility.FromJson<SimulationRequest>(payload);
                return request != null && !string.IsNullOrWhiteSpace(request.requestId) && request.striker != null;
            }
            catch { return false; }
        }
        public static string Serialize(SimulationResponse response) => JsonUtility.ToJson(response);
    }
}
