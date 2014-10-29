using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            game.medMode = false;
            game.hardMode = false;

            if(onePlayer.Checked == true)
            {
                game.onePlayer = true;

                if(medium.Checked == true)
                {
                    game.medMode = true;
                }

                else if(hard.Checked == true)
                {
                    game.hardMode = true;
                }
            }
            game.createGrid();
            game.setImages();
            game.firstGame = true;
            game.clearToAndFrom();
            game.clearSelectedOrPossible();
            game.movablePieceSelected = false;
            game.offensiveTeam = "light";
            this.Close();
        }
    }
}
