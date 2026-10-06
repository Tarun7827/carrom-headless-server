using UnityEngine;
namespace Carrom.Headless.Simulation
{
    public sealed class CarromPiece:MonoBehaviour
    {
        public string PieceId{get;private set;}public string PieceType{get;private set;}public bool Pocketed{get;private set;}Rigidbody2D _body;
        public void Configure(string id,string type,bool striker){PieceId=id;PieceType=type;Pocketed=false;_body=GetComponent<Rigidbody2D>();_body.simulated=true;_body.velocity=Vector2.zero;_body.angularVelocity=0f;gameObject.tag=striker?CarromConstants.StrikerTag:CarromConstants.TokenTag;gameObject.name=striker?"Striker":$"Token_{id}";}
        public void SetState(Vector2 position,Vector2 velocity){transform.position=position;_body.position=position;_body.velocity=velocity;_body.angularVelocity=0f;Pocketed=false;gameObject.SetActive(true);}
        public Vector2 Position=>_body.position;public Vector2 Velocity=>_body.velocity;
        public void MarkPocketed(){Pocketed=true;_body.velocity=Vector2.zero;_body.angularVelocity=0f;_body.simulated=false;gameObject.SetActive(false);}
        public void Deactivate(){Pocketed=true;_body.velocity=Vector2.zero;_body.angularVelocity=0f;_body.simulated=false;gameObject.SetActive(false);}
        public void Wake(){if(!Pocketed){_body.simulated=true;_body.WakeUp();}}
    }
}
