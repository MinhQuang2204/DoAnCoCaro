using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoCaro
{
    public partial class FormGame : Form
    {
        #region Properties
        ChessBoardMaganer ChessBoard;
        #endregion

        #region Methods
        public FormGame(string playerOne, string playerTwo, int numberOfPlayers, int level)
        {
            InitializeComponent();

            ChessBoard = new ChessBoardMaganer(pnlChessBoard, txbPlayerName, picbMark, playerOne, playerTwo, numberOfPlayers, level);
            
            ChessBoard.EndedGame += ChessBoard_EndedGame;

            NewGame();
        }
        void EndGame()
        {
            pnlChessBoard.Enabled = false;
            undoToolStripMenuItem.Enabled = false;
            MessageBox.Show("Kết thúc game!");
        }

        void NewGame()
        {
            undoToolStripMenuItem.Enabled = true;
            ChessBoard.drawChessBoard();
        }
        void Quit()
        {
            Application.Exit();
        }
        void Undo()
        {
            ChessBoard.Undo();
        }

        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                e.Cancel = true;
        }
        #endregion

        private void btnPlayMusic_Click(object sender, EventArgs e)
        {
            FormMusic formMusic = new FormMusic();
            formMusic.Show();
        }
    }
}
