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

//lets users change themes

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
            index = game.themeIndex;
            populate();
        }

        private void populate()
        {
            previewBox.Image = game.lKing;

            for (int i = 0; i < game.themeList.Count(); i++)
            {
                themeBox.Items.Add(game.themeList[i].GetName().Name);

                if(i == index)
                {
                    themeBox.Text = game.themeList[i].GetName().Name;
                }
            }
        }

        private void ok_Click(object sender, EventArgs e)
        {
            game.themeIndex = index;
            game.setTheme();

            if (game.board != null)
            {
                game.changeTheme();
            }
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void themeBox_SelectionMade(object sender, EventArgs e)
        {
            index = themeBox.SelectedIndex;
            System.IO.Stream streamFile = game.themeList[index].GetManifestResourceStream(game.themeList[index].GetName().Name + ".lKing.png");
            previewBox.Image = Image.FromStream(streamFile);
        }
    }
}
