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
            this.CenterToParent();
            this.ControlBox = false;
        }

        public void queen_Click(object sender, EventArgs e)
        {
            game.board[spot.x, spot.y].job = "Queen";
            game.board[spot.x, spot.y].picture = Resources.mQueen;
            game.coordinateToPictureBox(spot).Image = Resources.mQueen;
            this.Close();
        }

        public void rook_Click(object sender, EventArgs e)
        {
            game.board[spot.x, spot.y].job = "Rook";
            game.board[spot.x, spot.y].picture = Resources.mRook;
            game.coordinateToPictureBox(spot).Image = Resources.mRook;
            this.Close();
        }

        public void bishop_Click(object sender, EventArgs e)
        {
            game.board[spot.x, spot.y].job = "Bishop";
            game.board[spot.x, spot.y].picture = Resources.mBishop;
            game.coordinateToPictureBox(spot).Image = Resources.mBishop;
            this.Close();
        }

        public void knight_Click(object sender, EventArgs e)
        {
            game.board[spot.x, spot.y].job = "Knight";
            game.board[spot.x, spot.y].picture = Resources.mKnight;
            game.coordinateToPictureBox(spot).Image = Resources.mKnight;
            this.Close();
        }
    }
}
