using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Sets up a new game

namespace BobbyFischer
{
    public partial class NewGame : Form
    {
        private Chess game;

        public NewGame(Chess chess)
        {
            InitializeComponent();
            this.game = chess;
        }

        private void onePlayer_CheckedChanged(object sender, EventArgs e)
        {
            difficultyPanel.Enabled = true;
        }

        private void twoPlayer_CheckedChanged(object sender, EventArgs e)
        {
            difficultyPanel.Enabled = false;
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            if (dark.Checked == true)
            {
                game.offensiveTeam = "dark";
                game.baseOnBottom = "dark";
                game.createGrid();
                game.playAsDark();
                game.compTeam = "light";
            }
            else
            {
                game.offensiveTeam = "light";
                game.baseOnBottom = "light";
                game.createGrid();
                game.playAsLight();
                game.compTeam = "dark";
            }

            game.onePlayer = onePlayer.Checked;
            game.medMode = medium.Checked;
            game.hardMode = hard.Checked;
            game.firstGame = true;
            game.clearToAndFrom();
            game.clearSelectedOrPossible();
            game.movablePieceSelected = false;
            this.Close();
        }
    }
}
