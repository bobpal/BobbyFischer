using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//dialog displays when checkmate is reached by either side

namespace BobbyFischer
{
    public partial class GameOver : Form
    {
        Chess game;
        string losingTeam;

        public GameOver(Chess chess, string team)
        {
            InitializeComponent();
            this.game = chess;
            this.losingTeam = team;
            this.ControlBox = false;

            if (losingTeam == "onyx")
            {
                label1.Text = "Congratulations!\n\nYou have slain the evil king\n and saved the princess!";
            }

            else
            {
                label1.Text = "Sorry\n\nYou gave a valiant effort,\nbut you have been bested in battle by the onyx army\n";
            }
        }

        private void play_Click(object sender, EventArgs e)
        {
            this.Close();
            game.newGame();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
