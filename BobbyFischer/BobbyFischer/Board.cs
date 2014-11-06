using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BobbyFischer.Properties;

//an 8x8 checkered grid where the game is played

namespace BobbyFischer
{
    public partial class Board : Form
    {
        private Chess game;

        public Board()
        {
            InitializeComponent();
            game = new Chess(this);
            game.tryDlls();
            this.Show();
            game.newGame();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {
            
        }

        private void changeThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.themeForm();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.undo();
        }

        private void showLastMoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(showLastMoveToolStripMenuItem.Checked == false)
            {
                game.clearToAndFrom();
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.newGame();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if(game.firstGame == true)
            {
                PictureBox picBox = (sender as PictureBox);
                int row = 7 - gridPanel.GetRow(picBox);
                int col = gridPanel.GetColumn(picBox);

                if (picBox.BackColor == System.Drawing.Color.LawnGreen)
                {
                    picBox.Cursor = System.Windows.Forms.Cursors.Hand;
                }

                else if (game.board[col, row].color == game.offensiveTeam)
                {
                    picBox.Cursor = System.Windows.Forms.Cursors.Hand;
                }

                else
                {
                    picBox.Cursor = System.Windows.Forms.Cursors.Default;
                }
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox picBox = (sender as PictureBox);
            int row = 7 - gridPanel.GetRow(picBox);
            int col = gridPanel.GetColumn(picBox);
            game.clicker(new Chess.coordinate(col, row));
        }

        private void Close_Game(object sender, FormClosingEventArgs e)
        {
            game.saveState();
        }
    }
}
