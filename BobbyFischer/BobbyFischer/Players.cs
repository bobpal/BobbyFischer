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
    public partial class Players : Form
    {
        Chess game;

        public Players(Chess chess)
        {
            InitializeComponent();
            this.game = chess;
            this.ControlBox = false;
        }

        private void computer_Click(object sender, EventArgs e)
        {
            game.onePlayer = true;
            this.Close();
        }

        private void human_Click(object sender, EventArgs e)
        {
            game.onePlayer = false;
            this.Close();
        }
    }
}
