using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoCaro
{
    public partial class FormStart : Form
    {
        public FormStart()
        {
            InitializeComponent();
        }

        private void btnOnePlayer_Click(object sender, EventArgs e)
        {
            Form1Player form1Player = new Form1Player();
            form1Player.ShowDialog();
        }

        private void btnTwoPlayer_Click(object sender, EventArgs e)
        {
            Form2Player form2Player = new Form2Player();
            form2Player.ShowDialog();
        }
    }
}
