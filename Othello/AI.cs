using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;

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
            var move = Minimax(gameBoard, 0, double.MinValue, double.MaxValue, MinMaxMode.Max, currentState, availableMoves);
            allMovesStopWatch.Stop();
            moveStopWatch.Stop();

            if (move.coordinates == null) return;

            gameBoard.MakeMove(move.coordinates, currentState);

            positionsSearched += nPositions;

            System.Diagnostics.Debug.WriteLine($"On move {movesMade} the bot tried to play ({move.coordinates.x}, {move.coordinates.y}) and {nPositions} where searched taking {moveStopWatch.ElapsedMilliseconds}ms");
            System.Diagnostics.Debug.WriteLine($"The avg number of positions searched per move is {positionsSearched / movesMade} so far at a depth of {move.depth} taking an avg of {allMovesStopWatch.ElapsedMilliseconds / movesMade}ms");
            System.Diagnostics.Debug.WriteLine($"The final eval {move.depth} moves deep is {move.score}");
            // Print eval results to a json file
            var json = JsonConvert.SerializeObject(evalResults);
            File.WriteAllText("ai_dbg.json", json);
            evalResults.Clear();

            moveStopWatch.Reset();
        }

        private PositionEvaluationResult ManualFirstDepth(Board gamePosition, int depth, int player, int curPlayersNumberOfMoves, List<Position> moves)
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

            var evaluation = RunParallel(gamePosition, depth, player, moveToSearch);

            return evaluation;
        }

        public PositionEvaluationResult RunParallel(Board gameBoard, int depth, int currentPlayer, List<Position> availableMoves)
        {
            var tasks = new List<Task<PositionEvaluationResult>>();

            foreach (var move in availableMoves)
            {
                var task = Task.Run(() =>
                {
                    Board newBoard = new Board(gameBoard);
                    newBoard.MakeMove(move, currentPlayer);
                    int oppositePlayer = currentPlayer == 1 ? 2 : 1;
                    var eval = Minimax(newBoard, depth + 1, double.MinValue, double.MaxValue, MinMaxMode.Min, currentPlayer);
                    return new PositionEvaluationResult
                    {
                        coordinates = move,
                        score = eval.score,
                        depth = eval.depth
                    };
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            return tasks.Select(t => t.Result).OrderByDescending(r => r.score).FirstOrDefault();
        }

        public struct EvalDebuggingResult
        {
            public PositionEvaluationResult eval;
            public Board gameBoard;
        }

        public List<EvalDebuggingResult> evalResults = new List<EvalDebuggingResult>();

        private PositionEvaluationResult Minimax(Board gamePosition, int depth, double alpha, double beta, MinMaxMode minMaxMode, int currentPlayer, List<Position> availableMoves = null)
        {
            if (availableMoves == null)
                availableMoves = gamePosition.GetAvailableMoves(currentPlayer); // find all positions where we can make a move
            availableMoves = OptimizeMoveOrder(availableMoves); // sort the moves based on their score retrieved from boardWeight to try to optimize it a little bit
            int oppositePlayer = currentPlayer == 1 ? 2 : 1;

            var maximizingPlayer = minMaxMode == MinMaxMode.Max ? currentPlayer : oppositePlayer;

            if (depth == maxDepth || gamePosition.moves == Board.gridSize * Board.gridSize || gamePosition.StateHasWon(currentPlayer) || gamePosition.StateHasWon(oppositePlayer))      // if the position is won AKA all pieces on one side has been captured return
            {
                // We have reached the end of our search, evaluate the position
                nPositions++; // Increment the total position search counter
                var evalResult = new PositionEvaluationResult()
                {
                    score = gamePosition.EvaluatePosition(maximizingPlayer, availableMoves.Count),
                    depth = depth
                };
                evalResults.Add(new EvalDebuggingResult
                {
                    eval = evalResult,
                    gameBoard = gamePosition
                });
                return evalResult;
            }

            // If we are at the first step, evaluate the first step to allow for more efficient \alpha/\beta pruning
            PositionEvaluationResult evaluation;
            if (depth == 0)
            {
                evaluation = ManualFirstDepth(gamePosition, depth, currentPlayer, availableMoves.Count, availableMoves);
            }
            else
            {
                if (minMaxMode == MinMaxMode.Max)
                {
                    return MaximizingPlayer(gamePosition, depth, alpha, beta, currentPlayer, availableMoves);
                }
                else
                {
                    return MinimizingPlayer(gamePosition, depth, alpha, beta, currentPlayer, availableMoves);
                }
            }
            return evaluation;

        }

        private PositionEvaluationResult MaximizingPlayer(Board gamePosition, int depth, double alpha, double beta, int maximizingPlayer, List<Position> availableMoves)
        {
            var minimizingPlayer = maximizingPlayer == 1 ? 2 : 1;
            var evaluation = new PositionEvaluationResult
            {
                score = double.MinValue
            };
            foreach (var move in availableMoves)
            {
                // Make a copy of the board
                Board generationBoard = new Board(gamePosition); // This will copy the board data
                generationBoard.MakeMove(move, maximizingPlayer); // Make the "move" on the board

                PositionEvaluationResult currentEval;
                var opponentMoves = generationBoard.GetAvailableMoves(minimizingPlayer); // check if there are any moves that opponent can make
                if (opponentMoves.Any()) // if there are moves ==>
                {
                    currentEval = Minimax(generationBoard, depth + 1, alpha, beta, MinMaxMode.Min, minimizingPlayer, opponentMoves); // play as minimizing player during next move
                }
                else // opponent has no moves and turn goes back
                {
                    currentEval = Minimax(generationBoard, depth + 1, alpha, beta, MinMaxMode.Max, maximizingPlayer); // play again as maximizing player
                }
                if (evaluation.score < currentEval.score) // if the previously highest score is less then the positions score
                {
                    evaluation.coordinates = move; // save cordinates of the move
                    evaluation.depth = currentEval.depth;
                    evaluation.score = currentEval.score; // save the new score as the highest score
                }
                alpha = Math.Max(alpha, currentEval.score);
                if (beta <= alpha)
                    break;
            }

            return evaluation;
        }

        private PositionEvaluationResult MinimizingPlayer(Board gamePosition, int depth, double alpha, double beta, int minimizingPlayer, List<Position> availableMoves)
        {
            var maximizingPlayer = minimizingPlayer == 1 ? 2 : 1;
            var evaluation = new PositionEvaluationResult
            {
                score = double.MaxValue
            };

            foreach (var move in availableMoves)
            {
                Board generationBoard = new Board(gamePosition);    // This will copy the board data
                generationBoard.MakeMove(move, minimizingPlayer);             // Make the "move" on the board

                PositionEvaluationResult currentEval;
                var opponentMoves = generationBoard.GetAvailableMoves(maximizingPlayer); // check if there are any moves that opponent can make

                if (opponentMoves.Any()) // if there are moves ==>
                {
                    currentEval = Minimax(generationBoard, depth + 1, alpha, beta, MinMaxMode.Max, maximizingPlayer, opponentMoves); // play as minimizing player during next move
                }
                else // opponent has no moves and turn goes back
                {
                    currentEval = Minimax(generationBoard, depth + 1, alpha, beta, MinMaxMode.Min, minimizingPlayer); // play again as minimizing player
                }
                if (evaluation.score > currentEval.score)
                {
                    evaluation.coordinates = move; // save cordinates of the move
                    evaluation.depth = currentEval.depth;
                    evaluation.score = currentEval.score; // save the new score as the highest score
                }

                beta = Math.Min(beta, currentEval.score);
                if (beta <= alpha)
                    break;
            }
            return evaluation;
        }

        private List<Position> OptimizeMoveOrder(List<Position> unsortedMoves)
        {
            // Use LINQ to sort the moves based on their score retrieved from boardWeight
            var sortedMoves = unsortedMoves.OrderByDescending(move => boardWeight[move.x, move.y]).ToList();

            return sortedMoves;
        }
    }

    enum MinMaxMode
    {
        Min,
        Max
    }
}
