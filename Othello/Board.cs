using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    public class Board
    {

        public static int gridSize = 8;
        public int[,] position = new int[gridSize, gridSize];
        private List<Position> possiblePositions = new List<Position>(); // Save postitions that are outside of 3x3 around newly changed pieces
        public int moves = 0;

        public Board()
        {

        }
        public Board(Board other)
        {
            position = (int[,])other.position.Clone();
            possiblePositions = other.possiblePositions.ToList();
            moves = other.moves;
        }

        public void PlaceStartingMoves()
        {
            int L = gridSize / 2;
            int S = L - 1;
            position[S, L] = 1;
            position[L, S] = 1;
            position[S, S] = 2;
            position[L, L] = 2;

            AddNeighbours(S, L);
            AddNeighbours(L, S);
            AddNeighbours(S, S);
            AddNeighbours(L, L);

            moves += 4;
        }

        
        public bool StateHasWon(int playerState)
        {
            if (playerState == 1)
            {
                foreach (int moveState in position)
                {
                    if (moveState == 2)
                        return false;
                }
                return true;
            }
            if (playerState == 2)
            {
                foreach (int moveState in position)
                {
                    if (moveState == 1)
                        return false;
                }
                return true;
            }
            return false;
        }

        public List<Position> MakeMove(Position move, int player)
        {
            var tilesToChange = GetAffectedDiscs(move.x, move.y, player);
            if (tilesToChange.Any())
            {
                tilesToChange.Add(move);
                foreach (var tile in tilesToChange)
                {
                    position[tile.x, tile.y] = player;
                }
                AddNeighbours(move.x, move.y);
                moves++;
            }

            return tilesToChange;
        }

        private void AddNeighbours(int x, int y)
        {
            possiblePositions.RemoveAll(pos => pos.x == x && pos.y == y);
            for (int x_off = -1; x_off <= 1; x_off++)
            {
                for (int y_off = -1; y_off <= 1; y_off++)
                {
                    if (0 <= x_off + x && x_off + x < gridSize && 0 <= y_off + y && y_off + y < gridSize)
                    {
                        if (position[x_off + x, y_off + y] == 0)
                        {
                            Position coordinates = new Position(x_off + x, y_off + y);
                            if (!possiblePositions.Any(pos => pos.x == coordinates.x && pos.y == coordinates.y))
                                possiblePositions.Add(coordinates);
                        }
                    }
                }
            }
        }
        public double EvaluatePosition(int player)
        {
            double Eval = 0;
            
            if (moves < (64 - AI.maxDepth))
            {
                Eval = EarlyToMidGameEval(player, Eval);
            }
            else
            {
                Eval = EndGameEval(player, Eval);
            }
            return Eval;
        }
        
        private double EarlyToMidGameEval(int player, double Eval)
        {
            // more moves is still valuble but corners and sides are high priority

            int oppositePlayer = player == 1 ? 2 : 1;
            Eval = (CountScore(player) - CountScore(oppositePlayer)) * 0.75;

            Eval += position[0, 0] == player ? 25 : 0;
            Eval += position[gridSize - 1, gridSize - 1] == player ? 25 : 0;
            Eval += position[0, gridSize - 1] == player ? 25 : 0;
            Eval += position[gridSize - 1, 0] == player ? 25 : 0;

            Eval -= position[0, 0] == oppositePlayer ? 25 : 0;
            Eval -= position[gridSize - 1, gridSize - 1] == oppositePlayer ? 25 : 0;
            Eval -= position[0, gridSize - 1] == oppositePlayer ? 25 : 0;
            Eval -= position[gridSize - 1, 0] == oppositePlayer ? 25 : 0;

            int playerTilesOnSides = 0;
            int oppositeplayerTilesOnSides = 0;
            for (int x = 1; x < gridSize - 1; x++)
            {
                if (position[x, 0] == player)
                    playerTilesOnSides++;
                if (position[x, 7] == player)
                    playerTilesOnSides++;

                if (position[x, 0] == oppositePlayer)
                    oppositeplayerTilesOnSides++;
                if (position[x, 7] == oppositePlayer)
                    oppositeplayerTilesOnSides++;
            }
            for (int y = 1; y < gridSize - 1; y++)
            {
                if (position[0, y] == player)
                    playerTilesOnSides++;
                if (position[7, y] == player)
                    playerTilesOnSides++;

                if (position[0, y] == oppositePlayer)
                    oppositeplayerTilesOnSides++;
                if (position[7, y] == oppositePlayer)
                    oppositeplayerTilesOnSides++;
            }

            Eval += playerTilesOnSides * 2;
            Eval -= oppositeplayerTilesOnSides * 2;


            if (position[0,0] == 0)
            {
                Eval -= position[1 ,0] == player ? 5 : 0;
                Eval -= position[0, 1] == player ? 5 : 0;
                Eval -= position[1, 1] == player ? 15 : 0;
            }
            if (position[7, 0] == 0)
            {
                Eval -= position[6, 0] == player ? 5 : 0;
                Eval -= position[7, 1] == player ? 5 : 0;
                Eval -= position[6, 1] == player ? 15 : 0;
            }                                 
            if (position[0, 7] == 0)          
            {                                 
                Eval -= position[1, 7] == player ? 5 : 0;
                Eval -= position[0, 6] == player ? 5 : 0;
                Eval -= position[1, 6] == player ? 15 : 0;
            }                                 
            if (position[7, 7] == 0)          
            {                                 
                Eval -= position[6, 7] == player ? 5 : 0;
                Eval -= position[7, 6] == player ? 5 : 0;
                Eval -= position[6, 6] == player ? 15 : 0;
            }


            var curMoves = GetAvailableMoves(player).Count;
            var oppMoves = GetAvailableMoves(oppositePlayer).Count;

            Eval += curMoves * 2;
            Eval -= oppMoves * 2;

            if (StateHasWon(oppositePlayer))
                Eval -= 1000;

            if (StateHasWon(player))
                Eval += 1000;

            return Eval;
        }
        private double EndGameEval(int player, double Eval)
        {
            // when the depth reaches to the end of the game evaluate only by the score difference

            int oppositePlayer = player == 1 ? 2 : 1;
            Eval = CountScore(player) - CountScore(oppositePlayer);
            return Eval;
        }


        public List<Position> GetAvailableMoves(int state)
        {
            var availableMoves = new List<Position>();

            foreach (var position in possiblePositions)
            {
                if (GetAffectedDiscs(position.x, position.y, state).Any())
                {
                    availableMoves.Add(new Position(position.x, position.y));
                }
            }
            return availableMoves;
        }

        public int CountScore(int n)
        {
            int tiles = 0;
            foreach (var tile in position)
            {
                if (tile == n)
                    tiles++;
            }
            return tiles;
        }

        public List<Position> GetAffectedDiscs(int x, int y, int nextstate)
        {
            var allAffectedDiscs = new List<Position>();

            allAffectedDiscs.AddRange(CheckNorth(x, y, nextstate));
            allAffectedDiscs.AddRange(CheckNortheast(x, y, nextstate));
            allAffectedDiscs.AddRange(CheckNorthwest(x, y, nextstate));
            allAffectedDiscs.AddRange(CheckSouth(x, y, nextstate));
            allAffectedDiscs.AddRange(CheckSoutheast(x, y, nextstate));
            allAffectedDiscs.AddRange(CheckSouthwest(x, y, nextstate));
            allAffectedDiscs.AddRange(CheckEast(x, y, nextstate));
            allAffectedDiscs.AddRange(CheckWest(x, y, nextstate));

            return allAffectedDiscs;
        }

        public List<Position> CheckNorth(int x, int y, int state)
        {
            var list = new List<Position>();

            if (y != 0)
            {
                for (int i = y - 1; i >= 0; i--) // Go upwards from current postion
                {
                    var nState = position[x, i];

                    if (nState == 0)
                    {
                        return new List<Position>();
                    }
                    if (nState == state)
                    {
                        return list;
                    }
                    list.Add(new Position(x, i));
                }
            }
            return new List<Position>();
        }
        public List<Position> CheckNortheast(int x, int y, int state)
        {
            var list = new List<Position>();

            if (x != 7 && y != 0)
            {
                for (int i = 1; i <= Math.Min(gridSize - x - 1, y); i++)
                {
                    var nState = position[x + i, y - i];

                    if (nState == 0)
                    {
                        return new List<Position>();
                    }
                    if (nState == state)
                    {
                        return list;
                    }
                    list.Add(new Position(x + i, y - i));
                }
            }
            return new List<Position>();
        }
        public List<Position> CheckNorthwest(int x, int y, int state)
        {
            var list = new List<Position>();

            if (x != 0 && y != 0)
            {
                for (int i = 1; i <= Math.Min(x, y); i++)
                {
                    var nState = position[x - i, y - i];

                    if (nState == 0)
                    {
                        return new List<Position>();
                    }
                    if (nState == state)
                    {
                        return list;
                    }
                    list.Add(new Position(x - i, y - i));
                }
            }
            return new List<Position>();

        }

        public bool IsGameover()
        {
            if (moves == gridSize * gridSize)
                return true;
            var availiableMoves = GetAvailableMoves(1);
            if (!availiableMoves.Any())
            {
                availiableMoves = GetAvailableMoves(2);
                if (!availiableMoves.Any())
                {
                    return true;
                }
            }
            return false;
        }

        public List<Position> CheckSouth(int x, int y, int state)
        {
            var list = new List<Position>();

            if (y != 7)
            {
                for (int i = y + 1; i < gridSize; i++) // Go downwards from current postion
                {
                    var nState = position[x, i]; // state of the tile being checked

                    if (nState == 0) // If the position is empty
                    {
                        return new List<Position>(); // Return empty list to signify that no change should occur
                    }
                    if (nState == state) // If the state is the same as the state to check against
                    {
                        return list;
                    }
                    list.Add(new Position(x, i)); // Add the current position to the list of tiles to change
                }
            }
            return new List<Position>();
        }
        public List<Position> CheckSoutheast(int x, int y, int state)
        {
            var list = new List<Position>();

            if (x != 7 && y != 7)
            {
                for (int i = 1; i <= Math.Min(gridSize - x - 1, gridSize - y - 1); i++)
                {
                    var nState = position[x + i, y + i];

                    if (nState == 0)
                    {
                        return new List<Position>();
                    }
                    if (nState == state)
                    {
                        return list;
                    }
                    list.Add(new Position(x + i, y + i));
                }
            }
            return new List<Position>();
        }
        public List<Position> CheckSouthwest(int x, int y, int state)
        {
            var list = new List<Position>();

            if (x != 0 && y != 7)
            {
                for (int i = 1; i <= Math.Min(x, gridSize - y - 1); i++) // Change the others aswell
                {
                    var nState = position[x - i, y + i];

                    if (nState == 0)
                    {
                        return new List<Position>();
                    }
                    if (nState == state)
                    {
                        return list;
                    }
                    list.Add(new Position(x - i, y + i));
                }
            }
            return new List<Position>();
        }

        public List<Position> CheckEast(int x, int y, int state)
        {
            var list = new List<Position>();

            if (x != 7)
            {
                for (int i = x + 1; i < gridSize; i++) // Go to the right from current postion
                {
                    var nState = position[i, y];

                    if (nState == 0)
                    {
                        return new List<Position>();
                    }
                    if (nState == state)
                    {
                        return list;
                    }
                    list.Add(new Position(i, y));
                }
            }
            return new List<Position>();
        }

        public List<Position> CheckWest(int x, int y, int state)
        {
            var list = new List<Position>();

            if (x != 0)
            {
                for (int i = x - 1; i >= 0; i--) // Go to the right from current postion
                {
                    var nState = position[i, y];

                    if (nState == 0)
                    {
                        return new List<Position>();
                    }
                    if (nState == state)
                    {
                        return list;
                    }
                    list.Add(new Position(i, y));
                }
            }
            return new List<Position>();
        }
    }
}
