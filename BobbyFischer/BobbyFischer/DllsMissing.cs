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
    public partial class DllsMissing : Form
    {
        public DllsMissing(bool ntfs)
        {
            InitializeComponent();
            
            if(ntfs == true)
            {
                this.label1.Text = "Required dll not found.\n\nPlace 'Trinet.Core.IO.Ntfs.dll'\nin directory containing 'BobbyFischer.exe'";
            }
            else
	        {
                this.label1.Text = "No themes found.\n\nPlace theme dll in directory containing 'BobbyFischer.exe'";
	        }
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
