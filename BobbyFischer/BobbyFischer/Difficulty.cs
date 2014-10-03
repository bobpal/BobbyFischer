using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//determines the difficulty of the AI

namespace BobbyFischer
{
    public partial class Difficulty : Form
    {
        Chess game;

        public Difficulty(Chess chess)
        {
            InitializeComponent();
            this.game = chess;
            this.CenterToScreen();
            this.ControlBox = false;
        }

        private void easy_Click(object sender, EventArgs e)
        {
            game.medMode = false;
            game.hardMode = false;
            this.Close();
        }

        private void medium_Click(object sender, EventArgs e)
        {
            game.medMode = true;
            game.hardMode = false;
            this.Close();
        }

        private void hard_Click(object sender, EventArgs e)
        {
            game.medMode = false;
            game.hardMode = true;
            this.Close();
        }
    }
}
