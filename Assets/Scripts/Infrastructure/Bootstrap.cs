using UnityEngine;
using Carrom.Headless.Core;
namespace Carrom.Headless.Infrastructure
{
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize(){var go=new GameObject("CarromSimulationServer");Object.DontDestroyOnLoad(go);go.AddComponent<SimulationServer>();}
    }
}
