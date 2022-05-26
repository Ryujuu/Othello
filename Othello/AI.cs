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
        public static int maxDepth = 6;

        // increase deapth during last 15 or so moves

        public void Run(Board gameBoard, int currentState)
        {
            //Maybe block enemy moves
            nPositions = 0;

            if (gameBoard.moves >= 44 && gameBoard.moves < 50) // When end game is reached and fewer moves exists increse the depth
                AI.maxDepth = 8;
            else if (gameBoard.moves >= 50)
                AI.maxDepth = 14;

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

            if (depth == 0 || gamePosition.moves == Board.gridSize * Board.gridSize || gamePosition.StateHasWon(1) || gamePosition.StateHasWon(2))      // if the position is won AKA all pieces on one side has been captured return
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
                    var availiableMoves = generationBoard.GetAvailableMoves(oppositePlayer); // check if there are any moves that opponent can make
                    if (availiableMoves.Any()) // if there are moves ==>
                    {
                        eval = Minimax(generationBoard, depth - 1, alpha, beta, false, player); // play as minimizing player during next move
                    }
                    else // opponent has no moves and turn goes back
                    {
                        eval = Minimax(generationBoard, depth - 1, alpha, beta, true, player); // play again as maximizing player
                    }
                    if (maxEval < eval.score) // if the previously highest score is less then the positions score
                    {
                        evaluation.coordinates = move; // save cordinates of the move
                        maxEval = eval.score; // save the new score as the highest score
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
                    var availiableMoves = generationBoard.GetAvailableMoves(player); // check if there are any moves that opponent can make
                    if (availiableMoves.Any()) // if there are moves ==>
                    {
                        eval = Minimax(generationBoard, depth - 1, alpha, beta, true, player); // play as maximizing player during next move
                    }
                    else // opponent has no moves and turn goes back
                    {
                        eval = Minimax(generationBoard, depth - 1, alpha, beta, false, player); // play again as minimizing player
                    }

                    if (minEval > eval.score) // if the previously lowest score is larger then the positions score
                    {
                        evaluation.coordinates = move; // save cordinates of the move
                        minEval = eval.score; // save the new lowest score
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
