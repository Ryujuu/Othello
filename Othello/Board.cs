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
            int oppositePlayer = player == 1 ? 2 : 1;
            double Eval = CountScore(player) - CountScore(oppositePlayer);

            if (moves < (64-AI.maxDepth))
            {
                if (position[0, 0] == player || position[gridSize - 1, gridSize - 1] == player)
                    Eval += 7;
                if (position[0, gridSize - 1] == player || position[gridSize - 1, 0] == player)
                    Eval += 7;

                if (position[0, 0] == oppositePlayer || position[gridSize - 1, gridSize - 1] == oppositePlayer)
                    Eval -= 12;
                if (position[0, gridSize - 1] == oppositePlayer || position[gridSize - 1, 0] == oppositePlayer)
                    Eval -= 12;

                var curMoves = GetAvailableMoves(player).Count;
                var oppMoves = GetAvailableMoves(oppositePlayer).Count;
                Eval += curMoves * 2.2;
                Eval -= oppMoves * 2.5;

                if (curMoves == 0)
                    Eval -= 20;

                if (oppMoves == 0)
                    Eval += 20;

                int playerTilesOnSides = 0;
                int oppositeplayerTilesOnSides = 0;
                for (int x = 0; x < gridSize; x++)
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
                for (int y = 0; y < gridSize; y++)
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

                Eval += playerTilesOnSides * 1.5;
                Eval -= oppositeplayerTilesOnSides * 1.5;
            }
            return Eval;
        }


        public List<Position> GetAvailableMoves(int tag)
        {
            var availableMoves = new List<Position>();

            foreach (var position in possiblePositions)
            {
                if (GetAffectedDiscs(position.x, position.y, tag).Any())
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

        public List<Position> GetAffectedDiscs(int x, int y, int nextTag)
        {
            var allAffectedDiscs = new List<Position>();

            allAffectedDiscs.AddRange(CheckNorth(x, y, nextTag));
            allAffectedDiscs.AddRange(CheckNortheast(x, y, nextTag));
            allAffectedDiscs.AddRange(CheckNorthwest(x, y, nextTag));
            allAffectedDiscs.AddRange(CheckSouth(x, y, nextTag));
            allAffectedDiscs.AddRange(CheckSoutheast(x, y, nextTag));
            allAffectedDiscs.AddRange(CheckSouthwest(x, y, nextTag));
            allAffectedDiscs.AddRange(CheckEast(x, y, nextTag));
            allAffectedDiscs.AddRange(CheckWest(x, y, nextTag));

            return allAffectedDiscs;
        }

        public List<Position> CheckNorth(int x, int y, int tag)
        {
            var list = new List<Position>();

            if (y != 0)
            {
                for (int i = y - 1; i >= 0; i--) // Go upwards from current postion
                {
                    var nTag = position[x, i];

                    if (nTag == 0)
                    {
                        return new List<Position>();
                    }
                    if (nTag == tag)
                    {
                        return list;
                    }
                    list.Add(new Position(x, i));
                }
            }
            return new List<Position>();
        }
        public List<Position> CheckNortheast(int x, int y, int tag)
        {
            var list = new List<Position>();

            if (x != 7 && y != 0)
            {
                for (int i = 1; i <= Math.Min(gridSize - x - 1, y); i++)
                {
                    var nTag = position[x + i, y - i];

                    if (nTag == 0)
                    {
                        return new List<Position>();
                    }
                    if (nTag == tag)
                    {
                        return list;
                    }
                    list.Add(new Position(x + i, y - i));
                }
            }
            return new List<Position>();
        }
        public List<Position> CheckNorthwest(int x, int y, int tag)
        {
            var list = new List<Position>();

            if (x != 0 && y != 0)
            {
                for (int i = 1; i <= Math.Min(x, y); i++)
                {
                    var nTag = position[x - i, y - i];

                    if (nTag == 0)
                    {
                        return new List<Position>();
                    }
                    if (nTag == tag)
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

        public List<Position> CheckSouth(int x, int y, int tag)
        {
            var list = new List<Position>();

            if (y != 7)
            {
                for (int i = y + 1; i < gridSize; i++) // Go downwards from current postion
                {
                    var nTag = position[x, i]; // Tag of the tile being checked

                    if (nTag == 0) // If the position is empty
                    {
                        return new List<Position>(); // Return empty list to signify that no change should occur
                    }
                    if (nTag == tag) // If the tag is the same as the tag to check against
                    {
                        return list;
                    }
                    list.Add(new Position(x, i)); // Add the current position to the list of tiles to change
                }
            }
            return new List<Position>();
        }
        public List<Position> CheckSoutheast(int x, int y, int tag)
        {
            var list = new List<Position>();

            if (x != 7 && y != 7)
            {
                for (int i = 1; i <= Math.Min(gridSize - x - 1, gridSize - y - 1); i++)
                {
                    var nTag = position[x + i, y + i];

                    if (nTag == 0)
                    {
                        return new List<Position>();
                    }
                    if (nTag == tag)
                    {
                        return list;
                    }
                    list.Add(new Position(x + i, y + i));
                }
            }
            return new List<Position>();
        }
        public List<Position> CheckSouthwest(int x, int y, int tag)
        {
            var list = new List<Position>();

            if (x != 0 && y != 7)
            {
                for (int i = 1; i <= Math.Min(x, gridSize - y - 1); i++) // Change the others aswell
                {
                    var nTag = position[x - i, y + i];

                    if (nTag == 0)
                    {
                        return new List<Position>();
                    }
                    if (nTag == tag)
                    {
                        return list;
                    }
                    list.Add(new Position(x - i, y + i));
                }
            }
            return new List<Position>();
        }

        public List<Position> CheckEast(int x, int y, int tag)
        {
            var list = new List<Position>();

            if (x != 7)
            {
                for (int i = x + 1; i < gridSize; i++) // Go to the right from current postion
                {
                    var nTag = position[i, y];

                    if (nTag == 0)
                    {
                        return new List<Position>();
                    }
                    if (nTag == tag)
                    {
                        return list;
                    }
                    list.Add(new Position(i, y));
                }
            }
            return new List<Position>();
        }

        public List<Position> CheckWest(int x, int y, int tag)
        {
            var list = new List<Position>();

            if (x != 0)
            {
                for (int i = x - 1; i >= 0; i--) // Go to the right from current postion
                {
                    var nTag = position[i, y];

                    if (nTag == 0)
                    {
                        return new List<Position>();
                    }
                    if (nTag == tag)
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
