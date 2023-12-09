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
        public int positionsSearched = 0;
        public int movesMade = 4;
        Stopwatch moveStopWatch = new Stopwatch();
        Stopwatch allMovesStopWatch = new Stopwatch();
        int timeLimit = 500;
        int combindDepth = 0;

        private double[,] squareWeightTable = {
            {  1.00, -0.36,  0.53, -0.03, -0.03,  0.53, -0.36,  1.00 },
            { -0.35, -0.69, -0.22, -0.10, -0.10, -0.22, -0.69, -0.36 },
            {  0.53, -0.22,  0.08,  0.01,  0.01,  0.08, -0.22,  0.53 },
            { -0.03, -0.10,  0.01,  0.00,  0.00,  0.01, -0.10, -0.03 },
            { -0.03, -0.10,  0.01,  0.00,  0.00,  0.01, -0.10, -0.03 },
            {  0.53, -0.22,  0.08,  0.01,  0.01,  0.08, -0.22,  0.53 },
            { -0.36, -0.69, -0.22, -0.10, -0.10, -0.22, -0.69, -0.36 },
            {  1.00, -0.36,  0.53, -0.03, -0.03,  0.53, -0.36,  1.00 }
        };

        public void Run(Board gameBoard, int currentState)
        {
            nPositions = 0;
            movesMade = gameBoard.moves;
            var availableMoves = gameBoard.GetAvailableMoves(currentState);

            // Check for no available moves
            if (availableMoves.Count == 0)
                return;

            allMovesStopWatch.Start();
            moveStopWatch.Start();

            int allowedDepth = 1;
            var move = new PositionEvaluationResult();
            Dictionary<int, PositionEvaluationResult> bestMoveAfterCertainDepth = new Dictionary<int, PositionEvaluationResult>();

            while (moveStopWatch.ElapsedMilliseconds < timeLimit && (64 - movesMade) > allowedDepth)
            {
                // Prioritize the best move from the last completed depth search
                if (bestMoveAfterCertainDepth.ContainsKey(allowedDepth - 1))
                {
                    var previousBestMove = bestMoveAfterCertainDepth[allowedDepth - 1];
                    if (availableMoves.Contains(previousBestMove.coordinates))
                    {
                        availableMoves.Remove(previousBestMove.coordinates);
                        availableMoves.Insert(0, previousBestMove.coordinates);
                    }
                }

                move = Minimax(gameBoard, 0, allowedDepth, double.MinValue, double.MaxValue, MinMaxMode.Max, currentState, availableMoves);

                if (moveStopWatch.ElapsedMilliseconds < timeLimit && (64 - movesMade) >= allowedDepth)
                {
                    bestMoveAfterCertainDepth[allowedDepth] = move;
                    System.Diagnostics.Debug.WriteLine($"Depth {allowedDepth}: Best move at ({move.coordinates.x + 1}, {move.coordinates.y + 1})");
                }
                allowedDepth += 1;
            }

            allMovesStopWatch.Stop();
            moveStopWatch.Stop();

            // Select the best move from the deepest completed search
            int highestCompletedDepth = bestMoveAfterCertainDepth.Keys.Max();
            move = bestMoveAfterCertainDepth[highestCompletedDepth];

            combindDepth += highestCompletedDepth;
            // Check for a valid move
            if (move.coordinates == null) return;

            // Execute the move
            gameBoard.MakeMove(move.coordinates, currentState);
            positionsSearched += nPositions;
            movesMade++;

            System.Diagnostics.Debug.WriteLine($"Move {movesMade + 1}: Played ({move.coordinates.x + 1}, {move.coordinates.y + 1}). Searched {nPositions} positions in {moveStopWatch.ElapsedMilliseconds}ms.");
            System.Diagnostics.Debug.WriteLine($"Average positions searched per move: {positionsSearched / movesMade}. Avg depth: {combindDepth/ (movesMade - 4)}, Avg time: {allMovesStopWatch.ElapsedMilliseconds / (movesMade - 4)}ms.");
            System.Diagnostics.Debug.WriteLine($"Final evaluation at depth {move.depth}: Score = {move.score}");

            moveStopWatch.Reset();

            // Print eval results to a json file
            //var json = JsonConvert.SerializeObject(evalResults);
            //File.WriteAllText("ai_dbg.json", json);
            //evalResults.Clear();
        }

        public PositionEvaluationResult RunParallel(Board gameBoard, int currentDepth, int allowedDepth, int currentPlayer, List<Position> availableMoves)
        {
            var tasks = new List<Task<PositionEvaluationResult>>();

            foreach (var move in availableMoves)
            {
                var task = Task.Run(() =>
                {
                    Board newBoard = new Board(gameBoard);
                    newBoard.MakeMove(move, currentPlayer);
                    int oppositePlayer = currentPlayer == 1 ? 2 : 1;
                    var eval = Minimax(newBoard, currentDepth + 1, allowedDepth, double.MinValue, double.MaxValue, MinMaxMode.Min, oppositePlayer);
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

        private PositionEvaluationResult Minimax(Board gamePosition, int currentDepth, int allowedDepth, double alpha, double beta, MinMaxMode minMaxMode, int currentPlayer, List<Position> availableMoves = null)
        {
            if (availableMoves == null)
                availableMoves = gamePosition.GetAvailableMoves(currentPlayer); // find all positions where we can make a move
            availableMoves = OptimizeMoveOrder(availableMoves); // sort the moves based on their score retrieved from boardWeight to try to optimize it a little bit
            int oppositePlayer = currentPlayer == 1 ? 2 : 1;

            var maximizingPlayer = minMaxMode == MinMaxMode.Max ? currentPlayer : oppositePlayer;

            if (currentDepth == allowedDepth || moveStopWatch.ElapsedMilliseconds >= timeLimit || gamePosition.moves == Board.gridSize * Board.gridSize || gamePosition.StateHasWon(currentPlayer) || gamePosition.StateHasWon(oppositePlayer))      // if the position is won AKA all pieces on one side has been captured return
            {
                // We have reached the end of our search, evaluate the position
                nPositions++; // Increment the total position search counter
                var evalResult = new PositionEvaluationResult()
                {
                    score = gamePosition.EvaluatePosition(maximizingPlayer, availableMoves),
                    depth = currentDepth
                };
                //evalResults.Add(new EvalDebuggingResult
                //{
                //    eval = evalResult,
                //    gameBoard = gamePosition
                //});
                return evalResult;
            }

            // If we are at the first step, evaluate the first step to allow for more efficient \alpha/\beta pruning
            PositionEvaluationResult evaluation;
            if (currentDepth == 0)
            {
                evaluation = RunParallel(gamePosition, currentDepth, allowedDepth, currentPlayer, availableMoves);
            }
            else
            {
                if (minMaxMode == MinMaxMode.Max)
                {
                    return Maximizing(gamePosition, currentDepth, allowedDepth, alpha, beta, currentPlayer, availableMoves);
                }
                else
                {
                    return Minimizing(gamePosition, currentDepth, allowedDepth, alpha, beta, currentPlayer, availableMoves);
                }
            }
            return evaluation;

        }

        private PositionEvaluationResult Maximizing(Board gamePosition, int depth, int allowedDepth, double alpha, double beta, int maximizingPlayer, List<Position> availableMoves)
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
                    currentEval = Minimax(generationBoard, depth + 1, allowedDepth, alpha, beta, MinMaxMode.Min, minimizingPlayer, opponentMoves); // play as minimizing player during next move
                }
                else // opponent has no moves and turn goes back
                {
                    currentEval = Minimax(generationBoard, depth + 1, allowedDepth, alpha, beta, MinMaxMode.Max, maximizingPlayer); // play again as maximizing player
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

        private PositionEvaluationResult Minimizing(Board gamePosition, int depth, int allowedDepth, double alpha, double beta, int minimizingPlayer, List<Position> availableMoves)
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
                    currentEval = Minimax(generationBoard, depth + 1, allowedDepth, alpha, beta, MinMaxMode.Max, maximizingPlayer, opponentMoves); // play as minimizing player during next move
                }
                else // opponent has no moves and turn goes back
                {
                    currentEval = Minimax(generationBoard, depth + 1, allowedDepth, alpha, beta, MinMaxMode.Min, minimizingPlayer); // play again as minimizing player
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
            var sortedMoves = unsortedMoves.OrderByDescending(move => squareWeightTable[move.x, move.y]).ToList();

            return sortedMoves;
        }
    }

    enum MinMaxMode
    {
        Min,
        Max
    }
}
