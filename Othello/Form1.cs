using Othello.Properties;

namespace Othello
{
    public partial class Form1 : Form
    {
        Board gameBoard = new Board();

        public int nPositions = 0;

        PictureBox[,] visualBoard = new PictureBox[gridSize, gridSize];

        // size of the squares sides
        const int tileSize = 40;
        // number of rows and collums
        static int gridSize = Board.gridSize;

        // space between squares
        const int pad = 1;

        // Maximum amount of moves that the engine will look ahead
        static int maxDepth = 4; // Problem with depth 4 ATM

        // board side size
        static int sides = tileSize * gridSize + gridSize * pad * 2;

        // keep track of who's turn it is
        bool isPlayer1 = true;

        bool bot_p1 = false;
        bool bot_p2 = false;
        int bot1Gen = 3;
        int bot2Gen = 3;

        RandomBot randomBot;
        BiasedBot biasedBot;


        public bool showAvailiableMoves = true;

        public int CurrentState { get { return isPlayer1 ? 1 : 2; } }
        public int OppositeState { get { return isPlayer1 ? 2 : 1; } }

        public Form1()
        {
            InitializeComponent();

            label1.Hide();
            pictureBox1.Image = Resources.Black;
            flowLayoutPanel1.Size = new Size(sides, sides);
            flowLayoutPanel1.BackColor = Color.Black;

            AddPicuresToForm();

            gameBoard.PlaceStartingMoves();

            DisplayBoard(gameBoard.GetAvailableMoves(CurrentState));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void AddPicuresToForm()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    int y = i;
                    int x = j;
                    var piece = new PictureBox()
                    {
                        Margin = new Padding(pad),
                        Name = "pic" + i,
                        Height = tileSize,
                        Width = tileSize,
                        MinimumSize = new Size(tileSize, tileSize),
                        BorderStyle = System.Windows.Forms.BorderStyle.None,
                        Text = "",
                        BackColor = Color.DarkGreen,
                        Tag = 0,
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };
                    visualBoard[j, i] = piece;
                    flowLayoutPanel1.Controls.Add(visualBoard[j, i]);
                    visualBoard[j, i].Click += delegate
                    {
                        Click(x, y);
                    };
                }
            }
        }

        public void Click(int x, int y)
        {
            if (gameBoard.position[x, y] != 0) // if tile is already placed here then do not proceed
                return;

            var tilesToChange = gameBoard.MakeMove(new Position(x, y), CurrentState); // Maybe check if move is legal before making it

            if (tilesToChange.Any())
            {
                HideAvailiableMoves();
                SwitchPlayer();
                DisplayScore();
                
                label4.Text = nPositions.ToString();
            }
            else
            {
                ErrorFlash(x, y);
            }

            var availiableMoves = gameBoard.GetAvailableMoves(CurrentState);
            if (!availiableMoves.Any())
            {
                SwitchPlayer();
                RunBots();

                availiableMoves = gameBoard.GetAvailableMoves(CurrentState);
                if (!availiableMoves.Any())
                {
                    DisplayVictory();
                    return;
                }
            }
            else if (gameBoard.IsBoardFull())
            {
                DisplayVictory();
            }
            
            DisplayBoard(availiableMoves);
            RunBots();
        }


        public bool CanClickSpot(int x, int y)
        {
            if (gameBoard.GetAffectedDiscs(x, y, CurrentState).Any())
                return true;
            else
                return false;
        }


        public void DisplayBoard(List<Position> availableMoves)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    switch (gameBoard.position[x, y])
                    {
                        case 1:
                            visualBoard[x, y].Image = Resources.Black;
                            break;
                        case 2:
                            visualBoard[x, y].Image = Resources.White;
                            break;
                        default:
                            break;
                    }
                    visualBoard[x, y].Tag = gameBoard.position[x, y];
                }
            }
            if (showAvailiableMoves)
            {
                ShowAvailiableMoves(availableMoves); // This updates the GUI too
            }
            else
            {
                HideAvailiableMoves();
            }
        }

        private void RunBots()
        {
            if (isPlayer1 && bot_p1)
            {
                switch (bot1Gen)
                {
                    case 1:
                        BotEngine01();
                        break;
                    case 2:
                        BotEngine02();
                        break;
                    case 3:
                        BotEngine03();
                        break;
                }
            }
            else if (!isPlayer1 && bot_p2)
            {
                switch (bot2Gen)
                {
                    case 1:
                        BotEngine01();
                        break;
                    case 2:
                        BotEngine02();
                        break;
                    case 3:
                        BotEngine03();
                        break;
                }
            }
        }

        private void ErrorFlash(int x, int y)
        {
            Task.Run(async () =>
            {
                visualBoard[x, y].BackColor = Color.Red;
                await Task.Delay(300);
                visualBoard[x, y].BackColor = Color.DarkGreen;
            });
        }

        public void SwitchPlayer()
        {
            isPlayer1 = !isPlayer1;
            pictureBox1.Image = isPlayer1 ? Resources.Black : Resources.White;
        }

        public void HideAvailiableMoves()
        {
            foreach (PictureBox tile in visualBoard)
            {
                if ((int)tile.Tag == 0)
                {
                    tile.Image = null;
                }
            }
        }

        public void ShowAvailiableMoves(List<Position> availiableMoves)
        {
            foreach (var move in availiableMoves)
            {
                visualBoard[move.x, move.y].Image = Resources.Moves;
            }
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

            if (depth == 0 || gamePosition.moves == gridSize * gridSize || !moves1.Any() || !moves2.Any()) // If no valid moves exist then return position
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

        public void BotEngine03()
        {
            Task.Run(async () =>
            {
                //Maybe block enemy moves
                nPositions = 0;
                var move = Minimax(gameBoard, maxDepth, double.MinValue, double.MaxValue, true, CurrentState);
                if (move.coordinates == null) return;

                await Task.Delay(1);
                label1.Invoke(() =>
                {
                    Click(move.coordinates.x, move.coordinates.y);
                });
            });
        }


        public void BotEngine02()
        {

            Task.Run(async () =>
            {
                //Maybe block enemy moves



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
                await Task.Delay(1);
                label1.Invoke(() =>
                {
                    Click(move.x, move.y);
                });
            });
        }
        public void BotEngine01()
        {
            Task.Run(async () =>
            {
                var moves = gameBoard.GetAvailableMoves(CurrentState);
                var move = moves.OrderByDescending(x => Guid.NewGuid()).FirstOrDefault();

                if (move == null) return;
                await Task.Delay(1);
                label1.Invoke(() =>
                {
                    Click(move.x, move.y);
                });
            });
        }

        public void DisplayScore()
        {
            label2.Text = "White Tiles: " + gameBoard.CountScore(2);
            label3.Text = "Black Tiles: " + gameBoard.CountScore(1);
        }
        public void DisplayVictory()
        {
            int countWhite = gameBoard.CountScore(2);
            int countBlack = gameBoard.CountScore(1);

            if (countWhite > countBlack)
            {
                label1.Text = "White has won!";
                label1.Show();
            }
            else if (countWhite == countBlack)
            {
                label1.Text = "It's a draw!";
                label1.Show();
            }
            else
            {
                label1.Text = "Black has won!";
                label1.Show();
            }
        }

        public void ChangeState(int x, int y, int state)
        {
            gameBoard.position[x, y] = state;
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bot_p2 = checkBox1.Checked;
            RunBots();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            bot_p1 = checkBox2.Checked;
            RunBots();
        }

        private void checkBox3_CheckedChanged_1(object sender, EventArgs e)
        {
            showAvailiableMoves = checkBox3.Checked;
            if (showAvailiableMoves)
            {
                var availiableMoves = gameBoard.GetAvailableMoves(CurrentState);
                ShowAvailiableMoves(availiableMoves);
            }
            else
                HideAvailiableMoves();
        }
    }
}