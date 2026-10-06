using UnityEngine;

namespace CarromHeadless.Board
{
    public sealed class BoardPoolBootstrap : MonoBehaviour
    {
        [SerializeField] private BoardPool pool;

        private void Awake()
        {
            pool.Initialize();
        }
    }
}
