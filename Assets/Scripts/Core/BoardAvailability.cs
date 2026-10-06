using UnityEngine;
namespace Carrom.Headless.Core { public sealed class BoardAvailability : MonoBehaviour { public bool IsAvailable{get;private set;} public void Set(bool available){IsAvailable=available;gameObject.tag=available?"BoardAvailable":"BoardBusy";} } }
