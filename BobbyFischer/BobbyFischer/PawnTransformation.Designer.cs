namespace BobbyFischer
{
    partial class PawnTransformation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.question = new System.Windows.Forms.Label();
            this.queen = new System.Windows.Forms.Button();
            this.rook = new System.Windows.Forms.Button();
            this.bishop = new System.Windows.Forms.Button();
            this.knight = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.question, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.queen, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.rook, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.bishop, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.knight, 0, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(285, 265);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // question
            // 
            this.question.AutoSize = true;
            this.question.Dock = System.Windows.Forms.DockStyle.Fill;
            this.question.Location = new System.Drawing.Point(3, 0);
            this.question.Name = "question";
            this.question.Size = new System.Drawing.Size(279, 53);
            this.question.TabIndex = 0;
            this.question.Text = "Choose which piece to turn your pawn into";
            this.question.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // queen
            // 
            this.queen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queen.Location = new System.Drawing.Point(3, 56);
            this.queen.Name = "queen";
            this.queen.Size = new System.Drawing.Size(279, 47);
            this.queen.TabIndex = 1;
            this.queen.Text = "Queen";
            this.queen.UseVisualStyleBackColor = true;
            this.queen.Click += new System.EventHandler(this.queen_Click);
            // 
            // rook
            // 
            this.rook.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rook.Location = new System.Drawing.Point(3, 109);
            this.rook.Name = "rook";
            this.rook.Size = new System.Drawing.Size(279, 47);
            this.rook.TabIndex = 2;
            this.rook.Text = "Rook";
            this.rook.UseVisualStyleBackColor = true;
            this.rook.Click += new System.EventHandler(this.rook_Click);
            // 
            // bishop
            // 
            this.bishop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bishop.Location = new System.Drawing.Point(3, 162);
            this.bishop.Name = "bishop";
            this.bishop.Size = new System.Drawing.Size(279, 47);
            this.bishop.TabIndex = 3;
            this.bishop.Text = "Bishop";
            this.bishop.UseVisualStyleBackColor = true;
            this.bishop.Click += new System.EventHandler(this.bishop_Click);
            // 
            // knight
            // 
            this.knight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.knight.Location = new System.Drawing.Point(3, 215);
            this.knight.Name = "knight";
            this.knight.Size = new System.Drawing.Size(279, 47);
            this.knight.TabIndex = 4;
            this.knight.Text = "Knight";
            this.knight.UseVisualStyleBackColor = true;
            this.knight.Click += new System.EventHandler(this.knight_Click);
            // 
            // PawnTransformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PawnTransformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PawnTransformation";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label question;
        private System.Windows.Forms.Button queen;
        private System.Windows.Forms.Button rook;
        private System.Windows.Forms.Button bishop;
        private System.Windows.Forms.Button knight;

    }
}