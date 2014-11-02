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
using System.Reflection;

namespace BobbyFischer
{
    public partial class ChangeTheme : Form
    {
        private Chess game;
        private int index;

        public ChangeTheme(Chess chess)
        {
            InitializeComponent();
            this.game = chess;
            previewBox.Image = game.lKing;
        }

        private void ok_Click(object sender, EventArgs e)
        {
            game.themeIndex = index;
            game.setTheme();
            game.changeTheme();
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
