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
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ShowMoves = new System.Windows.Forms.CheckBox();
            this.BlackBot = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.WhiteBot = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
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
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Snap ITC", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(514, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 40);
            this.label1.TabIndex = 4;
            this.label1.Text = "Winner";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Snap ITC", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(514, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 23);
            this.label2.TabIndex = 6;
            this.label2.Text = "White Tiles: 2";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(518, 278);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 23);
            this.label4.TabIndex = 11;
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
            this.ShowMoves.Location = new System.Drawing.Point(518, 254);
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
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Snap ITC", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(514, 485);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 23);
            this.label3.TabIndex = 15;
            this.label3.Text = "Black Tiles: 2";
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(523, 310);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 15);
            this.label6.TabIndex = 17;
            this.label6.Text = "label6";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(776, 521);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.WhiteBot);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BlackBot);
            this.Controls.Add(this.ShowMoves);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
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
        private Label label1;
        private PictureBox pictureBox1;
        private Label label2;
        private Label label4;
        private Label label5;
        private CheckBox ShowMoves;
        private CheckBox BlackBot;
        private Label label3;
        private CheckBox WhiteBot;
        private Label label6;
    }
}