using Othello.Properties;

namespace Othello
{
    public partial class Form1 : Form
    {
        List<Board> gamePositions = new List<Board>();

        Board gameBoard = new Board();

        PictureBox[,] visualBoard = new PictureBox[gridSize, gridSize];

        // size of the squares sides
        const int tileSize = 60;
        // number of rows and collums
        static int gridSize = Board.gridSize;

        // space between squares
        const int pad = 1;

        // board side size
        static int sides = tileSize * gridSize + gridSize * pad * 2;

        // keep track of who's turn it is
        bool isPlayer1 = true;

        bool blackBot = false;
        bool whiteBot = false;

        int genBlack = 3;
        int genWhite = 3;

        RandomBot randomBot = new RandomBot();
        BiasedBot biasedBot = new BiasedBot();
        AI aI = new AI();


        public bool showAvailiableMoves = true;

        public int CurrentState { get { return isPlayer1 ? 1 : 2; } }
        public int OppositeState { get { return isPlayer1 ? 2 : 1; } }

        public Form1()
        {
            InitializeComponent();

            Winner.Hide();
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = Resources.Black;
            flowLayoutPanel1.Size = new Size(sides, sides);
            flowLayoutPanel1.BackColor = Color.Black;

            AddPicuresToForm();

            gameBoard.PlaceStartingMoves();

            DisplayBoard();
            if (showAvailiableMoves) // if the option to see moves is activated then display them
            {
                ShowAvailiableMoves(gameBoard.GetAvailableMoves(CurrentState));
            }
            Positions.Text = "Positions reached: " + 0;
            Depth.Text = "Depth: " + AI.maxDepth;
            Eval.Text = "Evaluation: " + 0;
            Moves.Text = "Pieces on the board: " + gameBoard.moves.ToString();
            Board tempPosition = new Board(gameBoard);
            gamePositions.Add(tempPosition);
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
                        if ((blackBot && !isPlayer1) || (whiteBot && isPlayer1) || (!whiteBot && !blackBot)) // Click still registers somehow?! Fix this later!! Player can still make a move on the bots turn
                            Click(x, y);
                    };
                }
            }
        }

        private void Click(int x, int y)
        {
            var affectedDiscs = gameBoard.GetAffectedDiscs(x, y, CurrentState); // Get the affected discs to see if move is legal

            if (gameBoard.position[x, y] != 0) // if tile is already placed here then do not proceed
                return;
            if (!affectedDiscs.Any()) // if there are no discs that get turned then give indication and jump out of method
            {
                ErrorFlash(x, y);
                return;
            }
            gameBoard.MakeMove(new Position(x, y), CurrentState); // makes the move on the gameboard
            PreformAfterMove();
            SwitchPlayer(); // change players
            Update();
            if (IsGameOver())
                return;
            if (blackBot && isPlayer1)
                RunBlackBot();
            else if (whiteBot && !isPlayer1)
                RunWhiteBot();
        }

        private void PreformAfterMove()
        {
            Board tempPosition = new Board(gameBoard);
            gamePositions.Add(tempPosition);
            HideAvailiableMoves(); // hide previously available moves and update them later after we made the new move
            DisplayBoard(); // display the new postion on the screen
            DisplayScore(); // displays the number of pieces each player has on the board
            Depth.Text = "Depth: " + AI.maxDepth.ToString();
            Positions.Text = "Positions reached: " + aI.nPositions.ToString(); // show the amount of postions that the ai went through
            if (showAvailiableMoves) // if the option to see the next moves is activated then display then
            {
                ShowAvailiableMoves(gameBoard.GetAvailableMoves(OppositeState)); // show the next players moves
            }
            float eval = (float)gameBoard.EvaluatePosition(OppositeState);
            Eval.Text = "Evaluation: " + eval.ToString();
            Moves.Text = "Pieces on the board: " + gameBoard.moves.ToString();
        }
        private bool IsGameOver()
        {
            if (gameBoard.moves == 64) // this would indicate that the board is full and therefore someone has won
            {
                DisplayVictory();
                return true;
            }
            var availiableMoves = gameBoard.GetAvailableMoves(CurrentState); // get the available moves for the new position
            if (!availiableMoves.Any()) // if there are no moves for the player
            {
                availiableMoves = gameBoard.GetAvailableMoves(OppositeState);
                if (!availiableMoves.Any()) // if there are still no possible moves then someone has won
                {
                    DisplayVictory();
                    return true;
                }
            }
            return false;
        }

        private void DisplayBoard()
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

        public void DisplayScore()
        {
            WhiteScore.Text = "White Tiles: " + gameBoard.CountScore(2);
            BlackScore.Text = "Black Tiles: " + gameBoard.CountScore(1);
        }
        public void DisplayVictory()
        {
            int countWhite = gameBoard.CountScore(2);
            int countBlack = gameBoard.CountScore(1);

            if (countWhite > countBlack)
            {
                Winner.Text = "White has won!";
                Winner.Show();
            }
            else if (countWhite == countBlack)
            {
                Winner.Text = "It's a draw!";
                Winner.Show();
            }
            else
            {
                Winner.Text = "Black has won!";
                Winner.Show();
            }
        }

        private void RunBlackBot()
        {
            switch (genBlack)
            {
                case 1:
                    randomBot.Run(gameBoard, CurrentState);
                    break;
                case 2:
                    biasedBot.Run(gameBoard, CurrentState);
                    break;
                case 3:
                    aI.Run(gameBoard, CurrentState);
                    break;
            }
            PreformAfterMove();
            if (IsGameOver())
                return;

            if (gameBoard.GetAvailableMoves(OppositeState).Count == 0)
            {
                RunBlackBot();
            }
            else
                SwitchPlayer();
            // else if (whiteBot)             // this has a high probability to crash the program so dont implement just yet
            // {
            //     RunWhiteBot();
            // }
        }
        private void RunWhiteBot()
        {
            switch (genWhite)
            {
                case 1:
                    randomBot.Run(gameBoard, CurrentState);
                    break;
                case 2:
                    biasedBot.Run(gameBoard, CurrentState);
                    break;
                case 3:
                    aI.Run(gameBoard, CurrentState);
                    break;
            }
            PreformAfterMove();
            if (IsGameOver())
                return;

            if (gameBoard.GetAvailableMoves(OppositeState).Count == 0)
            {
                RunWhiteBot();
            }
            else
                SwitchPlayer();
            // else if (blackBot)                 // this has a high probability to crash the program so dont implement just yet
            // {
            //     RunBlackBot();
            // }
        }


        private void ShowMoves_CheckedChanged(object sender, EventArgs e)
        {
            showAvailiableMoves = ShowMoves.Checked;
            if (showAvailiableMoves)
            {
                ShowAvailiableMoves(gameBoard.GetAvailableMoves(CurrentState));
            }
            else
                HideAvailiableMoves();
        }

        private void BlackBot_CheckedChanged(object sender, EventArgs e)
        {
            Update();
            blackBot = BlackBot.Checked;
            if (isPlayer1)
            {
                RunBlackBot();
            }

        }

        private void WhiteBot_CheckedChanged(object sender, EventArgs e)
        {
            Update();
            whiteBot = WhiteBot.Checked;
            if (!isPlayer1)
            {
                RunWhiteBot();
            }
        }

        private void RandomMove_Click(object sender, EventArgs e)
        {
            randomBot.Run(gameBoard, CurrentState);
            PreformAfterMove();
            if (IsGameOver())
                return;
            SwitchPlayer();
        }
    }
}