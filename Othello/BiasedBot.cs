using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    internal class BiasedBot
    {
        private int[,] boardWeight = {
        { 100, -10, 11, 6, 6, 11, -10, 100 },
        { -10, -20, 1, 2, 2, 1, -20, -10},
        { 10, 1, 5, 4, 4, 5, 1, 10},
        { 6, 2, 4, 2, 2, 4, 2, 6},
        { 6, 2, 4, 2, 2, 4, 2, 6 },
        { 10, 1, 5, 4, 4, 5, 1, 10 },
        { -10, -20, 1, 2, 2, 1, -20, -10 },
        { 100, -10, 11, 6, 6, 11, -10, 100 }
        };
        public void Run(Board gameBoard, int CurrentState)
        {
            var moves = gameBoard.GetAvailableMoves(CurrentState);
            var sortedMoves = moves.OrderByDescending(move => boardWeight[move.x, move.y]).ToList();
            var move = sortedMoves.FirstOrDefault();

            if (move == null) return;

            gameBoard.MakeMove(move, CurrentState);
        }
    }
}
