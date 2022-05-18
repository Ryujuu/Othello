using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    internal class RandomBot
    {
        public void Run(Board gameBoard, int CurrentState)
        {
            var moves = gameBoard.GetAvailableMoves(CurrentState);
            var move = moves.OrderByDescending(x => Guid.NewGuid()).FirstOrDefault();

            if (move == null) return;
            gameBoard.MakeMove(move, CurrentState);
        }
    }
}
