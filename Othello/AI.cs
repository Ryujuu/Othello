using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Othello
{
    internal class AI
    {
        public int nPositions = 0;
        public static int maxDepth = 6;
        public int positionsSearched = 0;
        public int movesMade = 0;
        Stopwatch moveStopWatch = new Stopwatch();
        Stopwatch allMovesStopWatch = new Stopwatch();

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

        public void Run(Board gameBoard, int currentState)
        {
            nPositions = 0;
            movesMade++;

            var availableMoves = gameBoard.GetAvailableMoves(currentState);

            allMovesStopWatch.Start();
            moveStopWatch.Start();
            var move = Minimax(gameBoard, 0, double.MinValue, double.MaxValue, true, currentState, availableMoves);
            allMovesStopWatch.Stop();
            moveStopWatch.Stop();

            if (move.coordinates == null) return;

            gameBoard.MakeMove(move.coordinates, currentState);

            positionsSearched += nPositions;

            System.Diagnostics.Debug.WriteLine($"On move {movesMade}, {nPositions} where searched taking {moveStopWatch.ElapsedMilliseconds}ms");
            System.Diagnostics.Debug.WriteLine($"The avg number of positions searched per move is {positionsSearched / movesMade} so far at a depth of {move.depth} taking an avg of {allMovesStopWatch.ElapsedMilliseconds / movesMade}ms");
            System.Diagnostics.Debug.WriteLine($"The final eval {move.depth} moves deep is {move.score}");

            moveStopWatch.Reset();
        }

        private PositionEvaluationResult ManualFirstDepth(Board gamePosition, List<Position> moves, int player, int curPlayersNumberOfMoves, int depth)
        {
            List<PositionEvaluationResult> bestToWorstMoves = new List<PositionEvaluationResult>();
            foreach (var move in moves)
            {
                Board firstGenBoard = new Board(gamePosition);
                firstGenBoard.MakeMove(move, player);
                PositionEvaluationResult postion1 = new PositionEvaluationResult();
                postion1.score = firstGenBoard.EvaluatePosition(player, curPlayersNumberOfMoves);
                postion1.coordinates = move;
                bestToWorstMoves.Add(postion1);
            }
            var moveToSearch = bestToWorstMoves.OrderByDescending(x => x.score).Select(y => y.coordinates).ToList();

            var evaluation = RunParallel(gamePosition, player, moveToSearch, depth);

            return evaluation;
        }

        public PositionEvaluationResult RunParallel(Board gameBoard, int currentState, List<Position> availableMoves, int depth)
        {
            var tasks = new List<Task<PositionEvaluationResult>>();

            foreach (var move in availableMoves)
            {
                var task = Task.Run(() =>
                {
                    Board newBoard = new Board(gameBoard);
                    newBoard.MakeMove(move, currentState);
                    return Minimax(newBoard, depth + 1, double.MinValue, double.MaxValue, false, currentState, newBoard.GetAvailableMoves(currentState));
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            return tasks.Select(t => t.Result).OrderByDescending(r => r.score).FirstOrDefault();
        }

        private PositionEvaluationResult Minimax(Board gamePosition, int depth, double alpha, double beta, bool maximizingplayer, int player, List<Position> availableMoves)
        {
            int oppositePlayer = player == 1 ? 2 : 1;

            var availableMovesOppPlayer = gamePosition.GetAvailableMoves(oppositePlayer);
            int curPlayerMoveCount = availableMoves.Count;

            if (depth == maxDepth || gamePosition.moves == Board.gridSize * Board.gridSize || gamePosition.StateHasWon(player) || gamePosition.StateHasWon(oppositePlayer))      // if the position is won AKA all pieces on one side has been captured return
            {
                nPositions++;
                PositionEvaluationResult result = new()
                {
                    score = gamePosition.EvaluatePosition(player, curPlayerMoveCount),
                    depth = depth
                };
                

                return result;
            }

            PositionEvaluationResult evaluation;
            if (depth == 0)
            {
                evaluation = ManualFirstDepth(gamePosition, availableMoves, player, curPlayerMoveCount, depth);
            }
            else
            {
                if (maximizingplayer)
                {
                    return MaximizingPlayer(gamePosition, depth, alpha, beta, player, oppositePlayer, availableMoves);
                }
                else
                {
                    return MinimizingPlayer(gamePosition, depth, alpha, beta, player, oppositePlayer, availableMoves);
                }
            }
            return evaluation;

        }

        private PositionEvaluationResult MaximizingPlayer(Board gamePosition, int depth, double alpha, double beta, int player, int oppositePlayer, List<Position> availableMoves)
        {
            double maxEval = double.MinValue;
            var evaluation = new PositionEvaluationResult();

            foreach (var move in availableMoves)
            {
                // Make a copy of the board
                Board generationBoard = new Board(gamePosition); // This will copy the board data
                                                                 // Make the "move" on the board

                generationBoard.MakeMove(move, player);

                PositionEvaluationResult eval;
                var availiableMoves = generationBoard.GetAvailableMoves(oppositePlayer); // check if there are any moves that opponent can make
                if (availiableMoves.Any()) // if there are moves ==>
                {
                    availableMoves = OptimizeMoveOrder(availiableMoves);
                    eval = Minimax(generationBoard, depth + 1, alpha, beta, false, player, availiableMoves); // play as minimizing player during next move
                }
                else // opponent has no moves and turn goes back
                {
                    availableMoves = generationBoard.GetAvailableMoves(player);
                    availableMoves = OptimizeMoveOrder(availiableMoves);
                    eval = Minimax(generationBoard, depth + 1, alpha, beta, true, player, availableMoves); // play again as maximizing player
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

        private PositionEvaluationResult MinimizingPlayer(Board gamePosition, int depth, double alpha, double beta, int player, int oppositePlayer, List<Position> availableMoves)
        {
            double minEval = double.MaxValue;
            var evaluation = new PositionEvaluationResult();

            foreach (var move in availableMoves)
            {
                Board generationBoard = new Board(gamePosition);    // This will copy the board data
                generationBoard.MakeMove(move, oppositePlayer);             // Make the "move" on the board

                PositionEvaluationResult eval;
                var availiableMoves = generationBoard.GetAvailableMoves(player); // check if there are any moves that opponent can make
                if (availiableMoves.Any()) // if there are moves ==>
                {
                    availableMoves = OptimizeMoveOrder(availiableMoves);
                    eval = Minimax(generationBoard, depth + 1, alpha, beta, true, player, availiableMoves); // play as maximizing player during next move
                }
                else // opponent has no moves and turn goes back
                {
                    availableMoves = generationBoard.GetAvailableMoves(oppositePlayer);
                    availableMoves = OptimizeMoveOrder(availiableMoves);
                    eval = Minimax(generationBoard, depth + 1, alpha, beta, false, player, availableMoves); // play again as minimizing player
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

        private List<Position> OptimizeMoveOrder(List<Position> unsortedMoves)
        {
            // Use LINQ to sort the moves based on their score retrieved from boardWeight
            var sortedMoves = unsortedMoves.OrderByDescending(move => boardWeight[move.x, move.y]).ToList();

            return sortedMoves;
        }
    }
}
