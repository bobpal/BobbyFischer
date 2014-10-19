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

//When a player's pawn reaches the other side of the board,
//the player gets to turn that piece into any other piece except a king

namespace BobbyFischer
{
    public partial class PawnTransformation : Form
    {
        Chess.coordinate spot;
        Chess game;

        public PawnTransformation(Chess.coordinate point, Chess chess)
        {
            InitializeComponent();
            this.spot = point;
            this.game = chess;
        }

        public void queen_Click(object sender, EventArgs e)
        {
            if(game.offensiveTeam == "light")
            {
                game.coordinateToPictureBox(spot).Image = game.lQueen;
            }
            else
            {
                game.coordinateToPictureBox(spot).Image = game.dQueen;
            }
            game.board[spot.x, spot.y].job = "Queen";
            this.Close();
        }

        public void rook_Click(object sender, EventArgs e)
        {
            if (game.offensiveTeam == "light")
            {
                game.coordinateToPictureBox(spot).Image = game.lRook;
            }
            else
            {
                game.coordinateToPictureBox(spot).Image = game.dRook;
            }
            game.board[spot.x, spot.y].job = "Rook";
            this.Close();
        }

        public void bishop_Click(object sender, EventArgs e)
        {
            if (game.offensiveTeam == "light")
            {
                game.coordinateToPictureBox(spot).Image = game.lBishop;
            }
            else
            {
                game.coordinateToPictureBox(spot).Image = game.dBishop;
            }
            game.board[spot.x, spot.y].job = "Bishop";
            this.Close();
        }

        public void knight_Click(object sender, EventArgs e)
        {
            if (game.offensiveTeam == "light")
            {
                game.coordinateToPictureBox(spot).Image = game.lKnight;
            }
            else
            {
                game.coordinateToPictureBox(spot).Image = game.dKnight;
            }
            game.board[spot.x, spot.y].job = "Knight";
            this.Close();
        }
    }
}
