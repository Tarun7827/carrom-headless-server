using UnityEngine;
namespace Carrom.Headless.Simulation
{
    public sealed class Pocket:MonoBehaviour
    {
        public CarromBoard Board{get;set;}public int PocketId{get;set;}
        void OnTriggerEnter2D(Collider2D other){var piece=other.GetComponent<CarromPiece>();if(piece!=null)Board?.Pocket(piece);}
    }
}
