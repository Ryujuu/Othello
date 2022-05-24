namespace Othello
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.Winner = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.WhiteScore = new System.Windows.Forms.Label();
            this.Positions = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ShowMoves = new System.Windows.Forms.CheckBox();
            this.BlackBot = new System.Windows.Forms.CheckBox();
            this.BlackScore = new System.Windows.Forms.Label();
            this.WhiteBot = new System.Windows.Forms.CheckBox();
            this.Eval = new System.Windows.Forms.Label();
            this.Depth = new System.Windows.Forms.Label();
            this.Moves = new System.Windows.Forms.Label();
            this.RandomMove = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.flowLayoutPanel1.ForeColor = System.Drawing.Color.Black;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(496, 496);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // Winner
            // 
            this.Winner.BackColor = System.Drawing.Color.Transparent;
            this.Winner.Font = new System.Drawing.Font("Snap ITC", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.Winner.ForeColor = System.Drawing.Color.White;
            this.Winner.Location = new System.Drawing.Point(514, 178);
            this.Winner.Name = "Winner";
            this.Winner.Size = new System.Drawing.Size(250, 40);
            this.Winner.TabIndex = 4;
            this.Winner.Text = "Winner";
            this.Winner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Location = new System.Drawing.Point(657, 221);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 40);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // WhiteScore
            // 
            this.WhiteScore.BackColor = System.Drawing.Color.Transparent;
            this.WhiteScore.Font = new System.Drawing.Font("Snap ITC", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.WhiteScore.ForeColor = System.Drawing.Color.White;
            this.WhiteScore.Location = new System.Drawing.Point(514, 12);
            this.WhiteScore.Name = "WhiteScore";
            this.WhiteScore.Size = new System.Drawing.Size(168, 23);
            this.WhiteScore.TabIndex = 6;
            this.WhiteScore.Text = "White Tiles: 2";
            // 
            // Positions
            // 
            this.Positions.BackColor = System.Drawing.Color.Transparent;
            this.Positions.Font = new System.Drawing.Font("Snap ITC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Positions.ForeColor = System.Drawing.Color.White;
            this.Positions.Location = new System.Drawing.Point(518, 325);
            this.Positions.Name = "Positions";
            this.Positions.Size = new System.Drawing.Size(246, 23);
            this.Positions.TabIndex = 11;
            this.Positions.Text = "Positions reached: ";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Snap ITC", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(514, 231);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(146, 20);
            this.label5.TabIndex = 12;
            this.label5.Text = "Current Color:";
            // 
            // ShowMoves
            // 
            this.ShowMoves.AutoSize = true;
            this.ShowMoves.BackColor = System.Drawing.Color.Transparent;
            this.ShowMoves.Checked = true;
            this.ShowMoves.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowMoves.Font = new System.Drawing.Font("Snap ITC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ShowMoves.ForeColor = System.Drawing.Color.White;
            this.ShowMoves.Location = new System.Drawing.Point(518, 264);
            this.ShowMoves.Name = "ShowMoves";
            this.ShowMoves.Size = new System.Drawing.Size(113, 21);
            this.ShowMoves.TabIndex = 13;
            this.ShowMoves.Text = "Show Moves";
            this.ShowMoves.UseVisualStyleBackColor = false;
            this.ShowMoves.CheckedChanged += new System.EventHandler(this.ShowMoves_CheckedChanged);
            // 
            // BlackBot
            // 
            this.BlackBot.AutoSize = true;
            this.BlackBot.BackColor = System.Drawing.Color.Transparent;
            this.BlackBot.Font = new System.Drawing.Font("Snap ITC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BlackBot.ForeColor = System.Drawing.Color.White;
            this.BlackBot.Location = new System.Drawing.Point(518, 461);
            this.BlackBot.Name = "BlackBot";
            this.BlackBot.Size = new System.Drawing.Size(92, 21);
            this.BlackBot.TabIndex = 14;
            this.BlackBot.Text = "Black Bot";
            this.BlackBot.UseVisualStyleBackColor = false;
            this.BlackBot.CheckedChanged += new System.EventHandler(this.BlackBot_CheckedChanged);
            // 
            // BlackScore
            // 
            this.BlackScore.BackColor = System.Drawing.Color.Transparent;
            this.BlackScore.Font = new System.Drawing.Font("Snap ITC", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BlackScore.ForeColor = System.Drawing.Color.White;
            this.BlackScore.Location = new System.Drawing.Point(514, 485);
            this.BlackScore.Name = "BlackScore";
            this.BlackScore.Size = new System.Drawing.Size(168, 23);
            this.BlackScore.TabIndex = 15;
            this.BlackScore.Text = "Black Tiles: 2";
            // 
            // WhiteBot
            // 
            this.WhiteBot.AutoSize = true;
            this.WhiteBot.BackColor = System.Drawing.Color.Transparent;
            this.WhiteBot.Font = new System.Drawing.Font("Snap ITC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.WhiteBot.ForeColor = System.Drawing.Color.White;
            this.WhiteBot.Location = new System.Drawing.Point(518, 38);
            this.WhiteBot.Name = "WhiteBot";
            this.WhiteBot.Size = new System.Drawing.Size(96, 21);
            this.WhiteBot.TabIndex = 16;
            this.WhiteBot.Text = "White Bot";
            this.WhiteBot.UseVisualStyleBackColor = false;
            this.WhiteBot.CheckedChanged += new System.EventHandler(this.WhiteBot_CheckedChanged);
            // 
            // Eval
            // 
            this.Eval.Font = new System.Drawing.Font("Snap ITC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Eval.ForeColor = System.Drawing.Color.White;
            this.Eval.Location = new System.Drawing.Point(518, 348);
            this.Eval.Name = "Eval";
            this.Eval.Size = new System.Drawing.Size(246, 23);
            this.Eval.TabIndex = 17;
            this.Eval.Text = "Evaluation: ";
            // 
            // Depth
            // 
            this.Depth.Font = new System.Drawing.Font("Snap ITC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Depth.ForeColor = System.Drawing.Color.White;
            this.Depth.Location = new System.Drawing.Point(518, 302);
            this.Depth.Name = "Depth";
            this.Depth.Size = new System.Drawing.Size(246, 23);
            this.Depth.TabIndex = 18;
            this.Depth.Text = "Depth: ";
            // 
            // Moves
            // 
            this.Moves.Font = new System.Drawing.Font("Snap ITC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Moves.ForeColor = System.Drawing.Color.White;
            this.Moves.Location = new System.Drawing.Point(518, 99);
            this.Moves.Name = "Moves";
            this.Moves.Size = new System.Drawing.Size(246, 23);
            this.Moves.TabIndex = 19;
            this.Moves.Text = "Pieces on the board: ";
            // 
            // RandomMove
            // 
            this.RandomMove.Location = new System.Drawing.Point(518, 397);
            this.RandomMove.Name = "RandomMove";
            this.RandomMove.Size = new System.Drawing.Size(125, 23);
            this.RandomMove.TabIndex = 20;
            this.RandomMove.Text = "Make random move";
            this.RandomMove.UseVisualStyleBackColor = true;
            this.RandomMove.Click += new System.EventHandler(this.RandomMove_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(776, 521);
            this.Controls.Add(this.RandomMove);
            this.Controls.Add(this.Moves);
            this.Controls.Add(this.Depth);
            this.Controls.Add(this.Eval);
            this.Controls.Add(this.WhiteBot);
            this.Controls.Add(this.BlackScore);
            this.Controls.Add(this.BlackBot);
            this.Controls.Add(this.ShowMoves);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Positions);
            this.Controls.Add(this.WhiteScore);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Winner);
            this.Controls.Add(this.flowLayoutPanel1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "Form1";
            this.Text = "Othello";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Label Winner;
        private PictureBox pictureBox1;
        private Label WhiteScore;
        private Label Positions;
        private Label label5;
        private CheckBox ShowMoves;
        private CheckBox BlackBot;
        private Label BlackScore;
        private CheckBox WhiteBot;
        private Label Eval;
        private Label Depth;
        private Label Moves;
        private Button RandomMove;
    }
}