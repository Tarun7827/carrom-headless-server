using System;
using System.Collections.Generic;
using UnityEngine;
using Carrom.Headless.DTO;
namespace Carrom.Headless.Simulation
{
    public sealed class CarromBoard:MonoBehaviour
    {
        public int BoardId{get;private set;} public bool IsAvailable{get;private set;}
        Carrom.Headless.Core.BoardAvailability _availability;
        readonly Dictionary<string,CarromPiece> _tokens=new Dictionary<string,CarromPiece>(StringComparer.Ordinal);
        readonly List<CarromPiece> _tokenPool=new List<CarromPiece>(); CarromPiece _striker;
        public IReadOnlyDictionary<string,CarromPiece> Tokens=>_tokens; public CarromPiece Striker=>_striker;
        public void Build(int id){BoardId=id;IsAvailable=true;_availability=gameObject.GetComponent<Carrom.Headless.Core.BoardAvailability>()??gameObject.AddComponent<Carrom.Headless.Core.BoardAvailability>();_availability.Set(true);name=$"CarromBoard_{id:00}";CreateEdges();CreatePockets();_striker=CreatePiece("Striker","striker",true);_striker.SetState(new Vector2(0,-3.5f),Vector2.zero);}
        GameObject Child(string name){var go=new GameObject(name);go.transform.SetParent(transform,false);return go;}
        void CreateEdges(){var go=Child("Edges");go.tag=CarromConstants.EdgeTag;var ec=go.AddComponent<EdgeCollider2D>();ec.points=new[]{new Vector2(-CarromConstants.PlayingHalfSize,-CarromConstants.PlayingHalfSize),new Vector2(CarromConstants.PlayingHalfSize,-CarromConstants.PlayingHalfSize),new Vector2(CarromConstants.PlayingHalfSize,CarromConstants.PlayingHalfSize),new Vector2(-CarromConstants.PlayingHalfSize,CarromConstants.PlayingHalfSize),new Vector2(-CarromConstants.PlayingHalfSize,-CarromConstants.PlayingHalfSize)};}
        void CreatePockets(){float p=CarromConstants.BoardHalfSize-0.35f;var index=0;foreach(var pos in new[]{new Vector2(-p,-p),new Vector2(p,-p),new Vector2(p,p),new Vector2(-p,p)}){var go=Child($"Pocket_{++index}");go.tag=CarromConstants.PocketTag;go.transform.localPosition=pos;var c=go.AddComponent<CircleCollider2D>();c.radius=CarromConstants.PocketRadius;c.isTrigger=true;var pocket=go.AddComponent<Pocket>();pocket.Board=this;pocket.PocketId=index;}}
        CarromPiece CreatePiece(string id,string type,bool striker){var go=Child(striker?"Striker":$"Token_{id}");var rb=go.AddComponent<Rigidbody2D>();rb.bodyType=RigidbodyType2D.Dynamic;rb.gravityScale=0f;rb.linearDamping=0.32f;rb.angularDamping=1f;rb.collisionDetectionMode=CollisionDetectionMode2D.Continuous;var c=go.AddComponent<CircleCollider2D>();c.radius=striker?CarromConstants.StrikerRadius:CarromConstants.TokenRadius;var p=go.AddComponent<CarromPiece>();p.Configure(id,type,striker);return p;}
        CarromPiece GetToken(string id,string type){if(_tokenPool.Count>_tokens.Count){var p=_tokenPool[_tokens.Count];p.Configure(id,type,false);return p;}var created=CreatePiece(id,type,false);_tokenPool.Add(created);return created;}
        public void BeginRequest(SimulationRequest request){IsAvailable=false;_availability.Set(false);ResetPieces(request);}
        void ResetPieces(SimulationRequest request){_tokens.Clear();for(var i=0;i<_tokenPool.Count;i++)_tokenPool[i].Deactivate();if(request.tokens!=null)foreach(var input in request.tokens){var piece=GetToken(input.id,input.type);_tokens[input.id]=piece;piece.SetState(new Vector2(input.position.x,input.position.y),new Vector2(input.velocity?.x??0,input.velocity?.y??0));}var s=request.striker;_striker.SetState(new Vector2(s.position.x,s.position.y),new Vector2(s.velocity?.x??0,s.velocity?.y??0));foreach(var t in _tokens.Values)t.Wake();_striker.Wake();}
        public void Pocket(CarromPiece piece){piece.MarkPocketed();}
        public void Release(){IsAvailable=true;_availability.Set(true);foreach(var t in _tokenPool)t.Deactivate();_tokens.Clear();_striker.SetState(new Vector2(0,-3.5f),Vector2.zero);}
    }
}
