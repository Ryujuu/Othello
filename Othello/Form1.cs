using Othello.Properties;

namespace Othello
{
    public partial class Form1 : Form
    {
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

            label1.Hide();
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
            Update();
            if (IsGameOver())
                return;
            RunBots();
            label6.Text = gameBoard.EvaluatePosition(CurrentState).ToString();
        }

        private void PreformAfterMove()
        {
            HideAvailiableMoves(); // hide previously available moves and update them later after we made the new move
            DisplayBoard(); // display the new postion on the screen
            SwitchPlayer(); // change players
            DisplayScore(); // displays the number of pieces each player has on the board
            label4.Text = aI.nPositions.ToString(); // show the amount of postions that the ai went through
            if (showAvailiableMoves) // if the option to see moves is activated then display them
            {
                ShowAvailiableMoves(gameBoard.GetAvailableMoves(CurrentState));
            }
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
                SwitchPlayer();

                availiableMoves = gameBoard.GetAvailableMoves(CurrentState);
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

        private void RunBots()
        {
            if (blackBot && isPlayer1)
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
            }
            else if (whiteBot && !isPlayer1)
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
            }
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
            blackBot = BlackBot.Checked;
            if (isPlayer1)
            {
                RunBots();
            }

        }

        private void WhiteBot_CheckedChanged(object sender, EventArgs e)
        {
            whiteBot = WhiteBot.Checked;
            if (!isPlayer1)
            {
                RunBots();
            }
        }
    }
}