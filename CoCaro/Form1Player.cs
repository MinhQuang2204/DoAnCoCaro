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
    public partial class Form1Player : Form
    {
        public Form1Player()
        {
            InitializeComponent();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (txtPlayer.Text == "")
            {
                errorMessage("Bạn chưa nhập tên!");
                return;
            }
            if (txtPlayer.Text == "Máy tính")
            {
                errorMessage("Bạn không được đặt tên này!");
                return;
            }
            if (cbbLevel.SelectedItem == null)
            {
                errorMessage("Bạn chưa chọn cấp độ!");
                return;
            }
            int level;
            if (cbbLevel.SelectedItem.ToString() == "Level 1")
                level = 1;
            else if (cbbLevel.SelectedItem.ToString() == "Level 2")
                level = 2;
            else if (cbbLevel.SelectedItem.ToString() == "Level 3")
                level = 3;
            else if (cbbLevel.SelectedItem.ToString() == "Level 4")
                level = 4;
            else
                level = 5;
            FormGame formGame = new FormGame("Máy tính", txtPlayer.Text, 1, level);

            formGame.ShowDialog();
            this.Close();
        }
        private void errorMessage(string text)
        {
            MessageBox.Show(text, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
