using System.Collections.Generic;
using CarromHeadless.Core;
using UnityEngine;

namespace CarromHeadless.Board
{
    public sealed class BoardPool : MonoBehaviour
    {
        [SerializeField] private BoardInstance boardPrefab;
        [SerializeField] private int boardCount = 10;

        private readonly List<BoardInstance> boards = new();

        public IReadOnlyList<BoardInstance> Boards => boards;

        public void Initialize()
        {
            if (boards.Count > 0)
                return;

            for (var i = 0; i < boardCount; i++)
            {
                var board = Instantiate(boardPrefab, transform);
                board.name = $"Board_{i:00}";
                board.Initialize(i);
                boards.Add(board);
            }
        }

        public bool TryAcquire(out BoardInstance board)
        {
            foreach (var candidate in boards)
            {
                if (candidate.TryReserve())
                {
                    board = candidate;
                    return true;
                }
            }

            board = null;
            return false;
        }

        public void Release(BoardInstance board)
        {
            if (board == null)
                return;

            board.BeginReset();
            ResetBoard(board);
            board.Release();
        }

        private static void ResetBoard(BoardInstance board)
        {
            board.Striker.velocity = Vector2.zero;
            board.Striker.angularVelocity = 0f;
            foreach (var body in board.Tokens.Values)
            {
                body.velocity = Vector2.zero;
                body.angularVelocity = 0f;
            }
        }
    }
}
