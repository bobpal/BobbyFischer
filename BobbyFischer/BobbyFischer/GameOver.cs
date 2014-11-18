using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

//dialog displays when checkmate is reached by either side

namespace BobbyFischer
{
    public partial class GameOver : Form
    {
        private Chess game;

        public GameOver(Chess chess, string losingTeam)
        {
            InitializeComponent();
            this.game = chess;
            string winningTeam;

            if (game.onePlayer == false)
            {
                if (losingTeam == "light")
                {
                    winningTeam = "dark";
                }

                else
                {
                    winningTeam = "light";
                }

                label1.Text = "The " + winningTeam + " army has slain the " + losingTeam + " army's king in battle";
            }

            else
            {
                if (losingTeam == game.compTeam)
                {
                    label1.Text = "Congratulations!\n\nYou have slain the evil king\n and saved the princess!";
                }

                else
                {
                    label1.Text = "Sorry\n\nYou gave a valiant effort,\nbut you have been bested in battle by the enemy army\n";
                }
            }
        }

        private void play_Click(object sender, EventArgs e)
        {
            this.Close();
            game.newGame();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            game.gameOverExit = true;
            Application.Exit();
        }
    }
}
