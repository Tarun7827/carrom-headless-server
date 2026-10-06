using System.Collections.Generic;
using CarromHeadless.Core;
using CarromHeadless.Requests;
using UnityEngine;

namespace CarromHeadless.Board
{
    public sealed class BoardInstance : MonoBehaviour
    {
        [SerializeField] private int boardId;
        [SerializeField] private BoardLifecycle lifecycle = BoardLifecycle.Available;
        [SerializeField] private Rigidbody2D striker;
        [SerializeField] private List<Rigidbody2D> tokens = new();

        private readonly Dictionary<string, Rigidbody2D> tokenById = new();

        public int Id => boardId;
        public BoardLifecycle Lifecycle => lifecycle;

        public void Initialize(int id)
        {
            boardId = id;
            lifecycle = BoardLifecycle.Available;
            tokenById.Clear();
            foreach (var body in tokens)
            {
                if (body != null)
                    tokenById[body.name] = body;
            }
        }

        public bool TryReserve()
        {
            if (lifecycle != BoardLifecycle.Available)
                return false;

            lifecycle = BoardLifecycle.Reserved;
            return true;
        }

        public void BeginSimulation()
        {
            lifecycle = BoardLifecycle.Simulating;
        }

        public void BeginReset()
        {
            lifecycle = BoardLifecycle.Resetting;
        }

        public void Release()
        {
            lifecycle = BoardLifecycle.Available;
        }

        public void Fault()
        {
            lifecycle = BoardLifecycle.Faulted;
        }

        public void Restore(SimulationRequest request)
        {
            striker.position = request.striker.position;
            striker.velocity = request.striker.velocity;
            striker.angularVelocity = 0f;

            foreach (var token in request.tokens)
            {
                if (!tokenById.TryGetValue(token.id, out var body))
                    continue;

                body.position = token.position;
                body.velocity = Vector2.zero;
                body.angularVelocity = 0f;
            }
        }

        public Rigidbody2D Striker => striker;
        public IReadOnlyDictionary<string, Rigidbody2D> Tokens => tokenById;
    }
}
