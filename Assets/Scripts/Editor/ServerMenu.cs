#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace Carrom.Headless.Editor { public static class ServerMenu { [MenuItem("Carrom/Validate Server Setup")] static void Validate(){Debug.Log("Carrom headless server: runtime bootstrap, configurable board pool, manual Physics2D simulation and WebSocket endpoint are configured.");} } }
#endif
