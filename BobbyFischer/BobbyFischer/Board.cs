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
        public Chess game;

        public Board()
        {
            InitializeComponent();
            this.CenterToScreen();
            game = new Chess(this);
            game.setLetterTheme();
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            
        }

        public void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {
            
        }

        private void lettersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.setLetterTheme();

            if(game.board != null)
            {
                game.changeTheme();
            }
        }

        private void fFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.setFFTheme();

            if (game.board != null)
            {
                game.changeTheme();
            }
        }

        private void soMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.setSOMTheme();

            if (game.board != null)
            {
                game.changeTheme();
            }
        }

        private void lttPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.setLttPTheme();

            if (game.board != null)
            {
                game.changeTheme();
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

        private void pictureBox57_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(0, 0));
        }

        private void pictureBox58_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(1, 0));
        }
        
        private void pictureBox59_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(2, 0));
        }

        private void pictureBox60_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(3, 0));
        }

        private void pictureBox61_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(4, 0));
        }

        private void pictureBox62_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(5, 0));
        }

        private void pictureBox63_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(6, 0));
        }

        private void pictureBox64_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(7, 0));
        }

        private void pictureBox49_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(0, 1));
        }

        private void pictureBox50_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(1, 1));
        }

        private void pictureBox51_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(2, 1));
        }

        private void pictureBox52_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(3, 1));
        }

        private void pictureBox53_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(4, 1));
        }

        private void pictureBox54_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(5, 1));
        }

        private void pictureBox55_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(6, 1));
        }

        private void pictureBox56_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(7, 1));
        }

        private void pictureBox41_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(0, 2));
        }

        private void pictureBox42_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(1, 2));
        }

        private void pictureBox43_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(2, 2));
        }

        private void pictureBox44_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(3, 2));
        }

        private void pictureBox45_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(4, 2));
        }

        private void pictureBox46_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(5, 2));
        }

        private void pictureBox47_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(6, 2));
        }

        private void pictureBox48_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(7, 2));
        }

        private void pictureBox33_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(0, 3));
        }

        private void pictureBox34_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(1, 3));
        }

        private void pictureBox35_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(2, 3));
        }

        private void pictureBox36_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(3, 3));
        }

        private void pictureBox37_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(4, 3));
        }

        private void pictureBox38_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(5, 3));
        }

        private void pictureBox39_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(6, 3));
        }

        private void pictureBox40_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(7, 3));
        }

        private void pictureBox25_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(0, 4));
        }

        private void pictureBox26_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(1, 4));
        }

        private void pictureBox27_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(2, 4));
        }

        private void pictureBox28_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(3, 4));
        }

        private void pictureBox29_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(4, 4));
        }

        private void pictureBox30_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(5, 4));
        }

        private void pictureBox31_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(6, 4));
        }

        private void pictureBox32_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(7, 4));
        }

        private void pictureBox17_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(0, 5));
        }

        private void pictureBox18_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(1, 5));
        }

        private void pictureBox19_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(2, 5));
        }

        private void pictureBox20_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(3, 5));
        }

        private void pictureBox21_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(4, 5));
        }

        private void pictureBox22_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(5, 5));
        }

        private void pictureBox23_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(6, 5));
        }

        private void pictureBox24_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(7, 5));
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(0, 6));
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(1, 6));
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(2, 6));
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(3, 6));
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(4, 6));
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(5, 6));
        }

        private void pictureBox15_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(6, 6));
        }

        private void pictureBox16_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(7, 6));
        }

        public void pictureBox1_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(0, 7));
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(1, 7));
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(2, 7));
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(3, 7));
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(4, 7));
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(5, 7));
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(6, 7));
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            game.clicker(new Chess.coordinate(7, 7));
        }
    }
}
