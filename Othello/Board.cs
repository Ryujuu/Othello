namespace Othello
{
    public class Board
    {
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
        double weightCount = 1;
        double weightMobility = 5;
        double weightCorners = 25;
        double weightEdges = 3;
        double weightFrontierDiscs = -1;
        double weightDiaCornerDiscs = -10;
        double weightBesideCornerDiscs = -5;


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
        public double EvaluatePosition(int curPlayer, List<Position> curPlayerAvailableMoves)
        {
            if (IsGameOver())
            {
                int winner = DetermineWinner();
                if (winner == 0)
                {
                    return 0;
                }
                else
                {
                    return winner == curPlayer ? 1000 : -1000;
                }
            }

            int oppPlayer = curPlayer == 1 ? 2 : 1;
            var oppPlayerAvailableMoves = GetAvailableMoves(oppPlayer);
            double eval = 0;

            int count = CountDiscs(curPlayer) - CountDiscs(oppPlayer);
            int mobility = CountMoves(curPlayerAvailableMoves) - CountMoves(oppPlayerAvailableMoves);
            int corners = CountCorners(curPlayer) - CountCorners(oppPlayer);
            int edges = CountEdges(curPlayer) - CountEdges(oppPlayer);
            int frontier = CountFrontierDiscs(curPlayer) - CountFrontierDiscs(oppPlayer);
            int diaCorners = CountDiaCorners(curPlayer) - CountDiaCorners(oppPlayer);
            int besideCorners = CountBesideCorners(curPlayer) - CountBesideCorners(oppPlayer);
            double discSquareValue = CalculateDiscSquareValue(curPlayer, oppPlayer);

            eval = weightCount * count +
                   weightMobility * mobility +
                   weightCorners * corners +
                   weightEdges * edges +
                   weightFrontierDiscs * frontier +
                   weightDiaCornerDiscs * diaCorners +
                   weightBesideCornerDiscs * besideCorners +
                   discSquareValue;

            return eval;
        }


        public bool IsGameOver()
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

        int DetermineWinner()
        {
            int countPlayer1 = CountDiscs(1);
            int countPlayer2 = CountDiscs(2);

            if (countPlayer1 > countPlayer2) return 1;
            else if (countPlayer2 > countPlayer1) return 2;
            else return 0; // Draw
        }

        public int CountDiscs(int player)
        {
            int tiles = 0;
            foreach (var tile in position)
            {
                if (tile == player)
                    tiles++;
            }
            return tiles;
        }

        private int CountMoves(List<Position> availableMoves)
        {
            return availableMoves.Count;
        }
        private int CountCorners(int player)
        {
            int count = 0;
            int[] corners = { 0, gridSize - 1 };
            foreach (int x in corners)
            {
                foreach (int y in corners)
                {
                    if (position[x, y] == player)
                        count++;
                }
            }
            return count;
        }

        private int CountEdges(int player)
        {
            int count = 0;
            for (int i = 1; i < gridSize - 1; i++)
            {
                foreach (int edge in new[] { 0, gridSize - 1 })
                {
                    if (position[i, edge] == player)
                        count++;
                    if (position[edge, i] == player)
                        count++;
                }
            }
            return count;
        }
        private int CountFrontierDiscs(int player)
        {
            int frontierDiscs = 0;
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    // If the current disc belongs to the player, check if it's a frontier disc
                    if (position[x, y] == player)
                    {
                        // Check all adjacent squares
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                int newX = x + dx;
                                int newY = y + dy;
                                // Check if the adjacent square is within the board and is empty
                                if (IsOnBoard(newX, newY) && position[newX, newY] == 0)
                                {
                                    frontierDiscs++;
                                    break; // Move to the next disc once a frontier is found
                                }
                            }
                        }
                    }
                }
            }
            return frontierDiscs;
        }

        private int CountDiaCorners(int player)
        {
            int count = 0;
            count += position[1, 1] == player ? 1 : 0;
            count += position[gridSize - 1, 1] == player ? 1 : 0;
            count += position[1, gridSize - 1] == player ? 1 : 0;
            count += position[gridSize - 1, gridSize - 1] == player ? 1 : 0;

            return count;
        }

        private int CountBesideCorners(int player)
        {
            int count = 0;
            count += position[0, 1] == player ? 1 : 0;
            count += position[1, 0] == player ? 1 : 0;
            count += position[0, gridSize - 2] == player ? 1 : 0;
            count += position[1, gridSize - 1] == player ? 1 : 0;
            count += position[gridSize - 2, 0] == player ? 1 : 0;
            count += position[gridSize - 1, 1] == player ? 1 : 0;
            count += position[gridSize - 2, gridSize - 1] == player ? 1 : 0;
            count += position[gridSize - 1, gridSize - 2] == player ? 1 : 0;
            return count;
        }

        private double CalculateDiscSquareValue(int player, int oppPlayer)
        {
            double value = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (position[x, y] == player)
                    {
                        value += squareWeightTable[x, y];
                    }
                    else if (position[x, y] == oppPlayer)
                    {
                        value -= squareWeightTable[x, y];
                    }
                }
            }
            value *= 5;
            return value;
        }

        public bool IsOnBoard(int x, int y)
        {
            return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
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
