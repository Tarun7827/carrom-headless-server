using System;
namespace Carrom.Headless.DTO
{
    [Serializable] public sealed class Vector2Dto{public float x;public float y;public Vector2Dto(){}public Vector2Dto(float x,float y){this.x=x;this.y=y;}}
    [Serializable] public sealed class TokenInput{public string id;public string type;public Vector2Dto position;public Vector2Dto velocity;}
    [Serializable] public sealed class StrikerInput{public Vector2Dto position;public Vector2Dto velocity;}
    [Serializable] public sealed class SimulationRequest{public string requestId;public StrikerInput striker;public TokenInput[] tokens;public float maxSimulationSeconds=8f;}
    [Serializable] public sealed class TokenOutput{public string id;public string type;public Vector2Dto finalPosition;public Vector2Dto finalVelocity;public bool pocketed;}
    [Serializable] public sealed class SimulationResponse
    {
        public string requestId;public bool success;public string error;public int boardId;public float simulationTimeMs;public float simulatedSeconds;
        public Vector2Dto strikerFinalPosition;public Vector2Dto strikerFinalVelocity;public bool strikerPocketed;public TokenOutput[] tokens;
        public static SimulationResponse Failure(string requestId,string error,int boardId=0)=>new SimulationResponse{requestId=requestId,success=false,error=error,boardId=boardId,tokens=Array.Empty<TokenOutput>()};
    }
}
