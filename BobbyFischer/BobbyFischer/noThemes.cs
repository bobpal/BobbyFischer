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
    public partial class NoThemes : Form
    {
        public NoThemes()
        {
            InitializeComponent();
        }

        private void retry_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }
    }
}
