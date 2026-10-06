using UnityEngine;
namespace CarromHeadless.Logging
{
    public static class ServerLog
    {
        public static void Info(string message) => Debug.Log($"[CarromServer] {message}");
        public static void Warn(string message) => Debug.LogWarning($"[CarromServer] {message}");
        public static void Error(string message) => Debug.LogError($"[CarromServer] {message}");
    }
}
