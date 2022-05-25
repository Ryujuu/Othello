using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    internal class BiasedBot
    {
        public void Run(Board gameBoard, int CurrentState)
        {
            Random random = new Random();
            var moves = gameBoard.GetAvailableMoves(CurrentState);
            var move = moves.OrderByDescending(x =>
            gameBoard.GetAffectedDiscs(x.x, x.y, CurrentState).Count *
            (x.x == 0 || x.x == 7 ? 3 : 1) *
            (x.y == 0 || x.y == 7 ? 3 : 1) *
            (x.x == 1 || x.x == 6 ? 0.3 : 1) *
            (x.y == 1 || x.y == 6 ? 0.3 : 1) *
            (x.x > 1 || x.x < 6 ? 1.5 : 1) *
            (x.y > 1 || x.y < 6 ? 1.5 : 1)).FirstOrDefault();

            if (move == null) return;

            gameBoard.MakeMove(move, CurrentState);
        }
    }
}
