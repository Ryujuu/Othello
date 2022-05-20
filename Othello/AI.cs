using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    internal class AI
    {
        public int nPositions = 0;
        static int maxDepth = 6;

        public void Run(Board gameBoard, int currentState)
        {
            //Maybe block enemy moves
            nPositions = 0;
            var move = Minimax(gameBoard, maxDepth, double.MinValue, double.MaxValue, true, currentState);
            if (move.coordinates == null) return;

            gameBoard.MakeMove(move.coordinates, currentState);
        }

        public List<Position> ManualFirstDepth(Board gamePosition, List<Position> moves, int player)
        {
            List<PostionEvaluationResult> bestToWorstMoves = new List<PostionEvaluationResult>();
            foreach (var move in moves)
            {
                Board firstGenBoard = new Board(gamePosition);
                firstGenBoard.MakeMove(move, player);
                PostionEvaluationResult postion1 = new PostionEvaluationResult();
                postion1.score = firstGenBoard.EvaluatePosition(player);
                postion1.coordinates = move;
                bestToWorstMoves.Add(postion1);
            }
            return bestToWorstMoves.OrderByDescending(x => x.score).Select(y => y.coordinates).ToList();
        }

        public PostionEvaluationResult Minimax(Board gamePosition, int depth, double alpha, double beta, bool maximizingplayer, int player)
        {
            int oppositePlayer = player == 1 ? 2 : 1;
            var moves1 = gamePosition.GetAvailableMoves(player);
            var moves2 = gamePosition.GetAvailableMoves(oppositePlayer);

            if (depth == 0 || gamePosition.moves == Board.gridSize * Board.gridSize || !moves1.Any() || !moves2.Any()) // If no valid moves exist then return position
            {
                nPositions++;
                PostionEvaluationResult result = new PostionEvaluationResult();
                result.score = gamePosition.EvaluatePosition(player);
                return result;
            }

            PostionEvaluationResult evaluation = new PostionEvaluationResult();
            if (maximizingplayer)
            {

                if (depth == maxDepth)
                {
                    var newMoves = ManualFirstDepth(gamePosition, moves1, player);
                    moves1.Clear();
                    moves1 = newMoves;
                }
                double maxEval = double.MinValue;
                foreach (var move in moves1)
                {
                    // Make a copy of the board
                    Board generationBoard = new Board(gamePosition); // This will copy the board data
                    // Make the "move" on the board

                    generationBoard.MakeMove(move, player);

                    PostionEvaluationResult eval;
                    var availiableMoves = generationBoard.GetAvailableMoves(oppositePlayer); // Does this maybe exist on row 262?
                    if (availiableMoves.Any())
                    {
                        eval = Minimax(generationBoard, depth - 1, alpha, beta, false, player);
                    }
                    else
                    {
                        eval = Minimax(generationBoard, depth - 1, alpha, beta, true, player);
                    }
                    if (maxEval < eval.score)
                    {
                        evaluation.coordinates = move;
                        maxEval = eval.score;
                    }
                    alpha = Math.Max(alpha, eval.score);
                    if (beta <= alpha)
                        break;
                }

                evaluation.score = maxEval;
                return evaluation;
            }
            else
            {
                double minEval = double.MaxValue;


                if (depth == maxDepth)
                {
                    var newMoves = ManualFirstDepth(gamePosition, moves2, oppositePlayer);
                    moves2.Clear();
                    moves2 = newMoves;
                }
                foreach (var move in moves2)
                {
                    Board generationBoard = new Board(gamePosition); // This will copy the board data
                                                                     // Make the "move" on the board

                    generationBoard.MakeMove(move, oppositePlayer);

                    PostionEvaluationResult eval;
                    var availiableMoves = generationBoard.GetAvailableMoves(player);
                    if (availiableMoves.Any())
                    {
                        eval = Minimax(generationBoard, depth - 1, alpha, beta, true, player);
                    }
                    else
                    {
                        eval = Minimax(generationBoard, depth - 1, alpha, beta, false, player);
                    }

                    if (minEval > eval.score)
                    {
                        evaluation.coordinates = move;
                        minEval = eval.score;
                    }
                    beta = Math.Min(beta, eval.score);
                    if (beta <= alpha)
                        break;
                }
                evaluation.score = minEval;
                return evaluation;
            }
        }
    }
}
