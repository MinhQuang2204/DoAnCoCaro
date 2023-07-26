using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoCaro
{
    public class ChessBoardMaganer
    {
        #region Properties

        private Panel chessBoard;
        public Panel ChessBoard
        {
            get
            {
                return chessBoard;
            }
            set
            {
                chessBoard = value;
            }
        }

        private List<Player> players;
        public List<Player> Players 
        { 
            get { return players; } 
            set { players = value; } 
        }

        private int currentPlayer;
        public int CurrentPlayer 
        {
            get { return currentPlayer; }
            set { currentPlayer = value; }
        }
        
        private TextBox playerName;
        public TextBox PlayerName 
        {
            get { return playerName; }
            set { playerName = value; }
        }

        private PictureBox playerMark;
        public PictureBox PlayerMark 
        {
            get { return playerMark; }
            set { playerMark = value; }
        }

        private List<List<Button>> matrix;
        public List<List<Button>> Matrix
        {
            get => matrix;
            set => matrix = value;
        }
        
        private Stack<PlayInfo> playTimeLine;
        public Stack<PlayInfo> PlayTimeLine
        {
            get => playTimeLine;
            set => playTimeLine = value;
        }

        private event EventHandler endedGame;
        public event EventHandler EndedGame
        {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }
        }

        private int level;
        public int Level { get => level; set => level = value; }
        public int NumberOfPlayers { get => numberOfPlayers; set => numberOfPlayers = value; }

        private int numberOfPlayers;

        #endregion

        #region Initialize
        public ChessBoardMaganer(Panel chessBoard, TextBox playerName, PictureBox mark, string playerOne, string playerTwo, int numberOfPlayers, int level)
        {
            this.ChessBoard = chessBoard;
            this.PlayerName = playerName;
            this.PlayerMark = mark;
            this.NumberOfPlayers = numberOfPlayers;
            this.Level = level;
            this.Players = new List<Player>()
            {
                new Player(playerOne,Image.FromFile(Application.StartupPath + "\\Resources\\iconX.png")),
                new Player(playerTwo,Image.FromFile(Application.StartupPath + "\\Resources\\iconO.png"))
            };

        }
        #endregion

        #region Methods
        public void drawChessBoard()
        {
            ChessBoard.Enabled = true;
            ChessBoard.Controls.Clear();

            PlayTimeLine = new Stack<PlayInfo>();

            CurrentPlayer = 0;

            ChangePlayer();

            Matrix = new List<List<Button>>();

            Button oldButton = new Button()
            {
                Location = new Point(0, 0),
                Width = 0
            };
            for (int i = 0; i < Const.CHESS_BOARD_LENGTH; i++)
            {
                Matrix.Add(new List<Button>());

                for (int j = 0; j < Const.CHESS_BOARD_LENGTH; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Const.CHESS_WIDTH,
                        Height = Const.CHESS_HEIGHT,
                        Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString(),
                        BackColor = Color.FromArgb(200, 200, 200)
                    };

                    btn.Click += btn_Click;
                    ChessBoard.Controls.Add(btn);

                    Matrix[i].Add(btn);
                    oldButton = btn;
                }
                oldButton.Location = new Point(0, oldButton.Location.Y + Const.CHESS_HEIGHT);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }
            setValueForArrayScore();
            if (NumberOfPlayers == 1 && Players[0].Name == "Máy tính")
            {
                Point point = getCoordinateOfComputerChess();
                Button computerChess = Matrix[point.X][point.Y];
                computerChess.BackgroundImage = Players[CurrentPlayer].Mark;
                computerChess.BackColor = Color.Aqua;
                PlayTimeLine.Push(new PlayInfo(GetChessPoint(computerChess), CurrentPlayer));
                CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
                ChangePlayer();
            }
        }
        
        void btn_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button.BackgroundImage != null)
                return;
            button.BackgroundImage = Players[CurrentPlayer].Mark;
            button.BackColor = Color.White;
            if (isEndGame(button))
            {
                endGame(CurrentPlayer);
                return;
            }
            PlayTimeLine.Push(new PlayInfo(GetChessPoint(button), CurrentPlayer));
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            ChangePlayer();
            if (NumberOfPlayers == 1)
            {
                Point point = getCoordinateOfComputerChess();
                Button computerChess = Matrix[point.X][point.Y];
                computerChess.BackgroundImage = Players[CurrentPlayer].Mark;
                computerChess.BackColor = Color.Aqua;
                if (isEndGame(computerChess))
                {
                    endGame(CurrentPlayer);
                    return;
                }
                PlayTimeLine.Push(new PlayInfo(GetChessPoint(computerChess), CurrentPlayer));
                CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
                ChangePlayer();
            }
        }
        private void endGame(int currentPlayer)
        {
            if (isDrawGame())
                MessageBox.Show("Cả 2 người chơi hòa nhau!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                EndGame();
                MessageBox.Show(Players[currentPlayer].Name + " đã thắng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public void EndGame()
        {
            if (endedGame != null)
                endedGame(this, new EventArgs());
        }

        public bool Undo()
        {
            if ((NumberOfPlayers == 1 && Players[0].Name == "Máy tính" && PlayTimeLine.Count <= 1) || PlayTimeLine.Count <= 0)
                return false;
            if (NumberOfPlayers == 1)
            {
                PlayInfo oldButton;
                Button btn;

                oldButton = PlayTimeLine.Pop();
                btn = Matrix[oldButton.Point.Y][oldButton.Point.X];
                btn.BackgroundImage = null;
                btn.BackColor = Color.FromArgb(200, 200, 200);

                oldButton = PlayTimeLine.Pop();
                btn = Matrix[oldButton.Point.Y][oldButton.Point.X];
                btn.BackgroundImage = null;
                btn.BackColor = Color.FromArgb(200, 200, 200);
            }
            else
            {
                PlayInfo oldPoint = PlayTimeLine.Pop();
                Button btn = Matrix[oldPoint.Point.Y][oldPoint.Point.X];
                btn.BackgroundImage = null;
                btn.BackColor = Color.FromArgb(200, 200, 200);
                if (PlayTimeLine.Count <= 0)
                    CurrentPlayer = 0;
                else
                    CurrentPlayer = PlayTimeLine.Peek().CurrentPlayer == 1 ? 0 : 1;
                ChangePlayer();
            }
            return true;
        }

        private bool isEndGame(Button btn)
        {
            if (isEndHorizontal(btn) || isEndVertical(btn) || isEndMain(btn) || isEndSub(btn))
                return true;
            if (isDrawGame())
                return true;
            return false;
        }

        private Point GetChessPoint(Button btn)
        {
            int vertical = Convert.ToInt32(btn.Tag);
            int horizontal = Matrix[vertical].IndexOf(btn);

            Point point = new Point(horizontal, vertical);
            return point;
        }
        private bool isEndHorizontal(Button btn)
        {
            Point point = GetChessPoint(btn);

            int countLeft = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countLeft++;
                }
                else
                    break;
            }

            int countRight = 0;
            for (int i = point.X + 1; i < Const.CHESS_BOARD_LENGTH; i++)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countRight++;
                }
                else
                    break;
            }

            return countLeft + countRight == 5;
        }
        private bool isEndVertical(Button btn)
        {
            Point point = GetChessPoint(btn);

            int countTop = 0;
            for (int i = point.Y; i >= 0; i--)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }

            int countBottom = 0;
            for (int i = point.Y + 1; i < Const.CHESS_BOARD_LENGTH; i++)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom == 5;
        }
        private bool isEndMain(Button btn)
        {
            Point point = GetChessPoint(btn);

            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.Y - i < 0 || point.X - i < 0)
                    break;

                if (Matrix[point.Y - i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }

            int countBottom = 0;
            for (int i = 1; i <= Const.CHESS_BOARD_LENGTH - point.X; i++)
            {
                if (point.Y + i >= Const.CHESS_BOARD_LENGTH || point.X + i >= Const.CHESS_BOARD_LENGTH)
                    break;

                if (Matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom == 5;
        }
        private bool isEndSub(Button btn)
        {
            Point point = GetChessPoint(btn);

            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.Y - i < 0 || point.X + i > Const.CHESS_BOARD_LENGTH)
                    break;

                if (Matrix[point.Y - i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }

            int countBottom = 0;
            for (int i = 1; i <= Const.CHESS_BOARD_LENGTH - point.X; i++)
            {
                if (point.Y + i >= Const.CHESS_BOARD_LENGTH || point.X - i < 0)
                    break;

                if (Matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom == 5;
        }
        private bool isDrawGame()
        {
            for (int i = 0; i < Const.CHESS_BOARD_LENGTH; i++) 
                for (int j = 0; j < Const.CHESS_BOARD_LENGTH; j++)
                    if (Matrix[i][j].BackgroundImage == null)
                        return false;
            return true;
        }
        

        private void ChangePlayer()
        {
            PlayerName.Text = Players[CurrentPlayer].Name;

            PlayerMark.Image = Players[CurrentPlayer].Mark;
        }
        #endregion

        #region Play with computer
        #region Set value for array score
        private int[] attackScore;
        private int[] defenseScore;

        private void setValueForArrayScore()
        {
            if (Level == 1)
            {
                attackScore = new int[7] { 0, 10, 30, 50, 200, 3000, 10000 };
                defenseScore = new int[7] { 0, 5, 10, 10, 10, 10, 10 };
            }
            else if (Level == 2)
            {
                attackScore = new int[7] { 0, 4, 13, 434, 234, 2434, 42332 };
                defenseScore = new int[7] { 0, 5, 3, 81, 42, 6561, 1221 };
            }
            else if (Level == 3)
            {
                attackScore = new int[7] { 0, 3, 24, 192, 1536, 12288, 98304 };
                defenseScore = new int[7] { 0, 1, 9, 81, 729, 6561, 59049 };
            }
            else if (Level == 4)
            {
                attackScore = new int[7] { 0, 9, 54, 162, 1458, 13112, 118008 };
                defenseScore = new int[7] { 0, 3, 27, 99, 729, 6561, 59049 };
            }
            else
            {
                attackScore = new int[7] { 0, 13, 34, 202, 1546, 12298, 98314 };
                defenseScore = new int[7] { 0, 3, 9, 81, 729, 6561, 59049 };
            }
        }

        #endregion
        
        #region Attack
        public int attackHorizonal(int currentRow, int currentCol)
        {
            int score = 0;
            int ourChess = 0;
            int enemyChess = 0;
            for (int i = 1; i < 6 && currentCol - i >= 0; i++)
            {
                if (Matrix[currentRow][currentCol - i].BackColor == Color.White)
                    ourChess++;
                else if (Matrix[currentRow][currentCol - i].BackColor == Color.Aqua)
                {
                    enemyChess++;
                    break;
                }
                else
                    break;
            }
            for (int i = 1; i < 6 && currentCol + i < Const.CHESS_BOARD_LENGTH; i++)
            {
                if (Matrix[currentRow][currentCol + i].BackColor == Color.White)
                    ourChess++;
                else if (Matrix[currentRow][currentCol + i].BackColor == Color.Aqua)
                {
                    enemyChess++;
                    break;
                }
                else
                    break;
            }
            if (enemyChess == 2)
                return 0;
            score -= defenseScore[enemyChess + 1] * 2;
            score += attackScore[ourChess];
            return score;
        }

        public int attackVertical(int currentRow, int currentCol)
        {
            int score = 0;
            int ourChess = 0;
            int enemyChess = 0;
            for (int i = 1; i < 6 && currentRow - i >= 0; i++)
            {
                if (Matrix[currentRow - i][currentCol].BackColor == Color.White)
                    ourChess++;
                else if (Matrix[currentRow - i][currentCol].BackColor == Color.Aqua)
                {
                    enemyChess++;
                    break;
                }
                else
                    break;
            }
            for (int i = 1; i < 6 && currentRow + i < Const.CHESS_BOARD_LENGTH; i++)
            {
                if (Matrix[currentRow + i][currentCol].BackColor == Color.White)
                    ourChess++;
                else if (Matrix[currentRow + i][currentCol].BackColor == Color.Aqua)
                {
                    enemyChess++;
                    break;
                }
                else
                    break;
            }
            if (enemyChess == 2)
                return 0;
            score -= defenseScore[enemyChess + 1] * 2;
            score += attackScore[ourChess];
            return score;
        }

        public int attackSubDiagonal(int currentRow, int currentCol)
        {
            int score = 0;
            int ourChess = 0;
            int enemyChess = 0;
            for (int i = 1; i < 6 && currentRow - i >= 0 && currentCol + i < Const.CHESS_BOARD_LENGTH; i++)
            {
                if (Matrix[currentRow - i][currentCol + i].BackColor == Color.White)
                    ourChess++;
                else if (Matrix[currentRow - i][currentCol + i].BackColor == Color.Aqua)
                {
                    enemyChess++;
                    break;
                }
                else
                    break;
            }
            for (int i = 1; i < 6 && currentRow + i < Const.CHESS_BOARD_LENGTH && currentCol - i >= 0; i++)
            {
                if (Matrix[currentRow + i][currentCol - i].BackColor == Color.White)
                    ourChess++;
                else if (Matrix[currentRow + i][currentCol - i].BackColor == Color.Aqua)
                {
                    enemyChess++;
                    break;
                }
                else
                    break;
            }
            if (enemyChess == 2)
                return 0;
            score -= defenseScore[enemyChess + 1] * 2;
            score += attackScore[ourChess];
            return score;
        }

        public int attackMainDiagonal(int currentRow, int currentCol)
        {
            int score = 0;
            int ourChess = 0;
            int enemyChess = 0;
            for (int i = 1; i < 6 && currentRow - i >= 0 && currentCol - i >= 0; i++)
            {
                if (Matrix[currentRow - i][currentCol - i].BackColor == Color.White)
                    ourChess++;
                else if (Matrix[currentRow - i][currentCol - i].BackColor == Color.Aqua)
                {
                    enemyChess++;
                    break;
                }
                else
                    break;
            }
            for (int i = 1; i < 6 && currentRow + i < Const.CHESS_BOARD_LENGTH && currentCol + i < Const.CHESS_BOARD_LENGTH; i++)
            {
                if (Matrix[currentRow + i][currentCol + i].BackColor == Color.White)
                    ourChess++;
                else if (Matrix[currentRow + i][currentCol + i].BackColor == Color.Aqua)
                {
                    enemyChess++;
                    break;
                }
                else
                    break;
            }
            if (enemyChess == 2)
                return 0;
            score -= defenseScore[enemyChess + 1] * 2;
            score += attackScore[ourChess];
            return score;
        }
        #endregion

        #region Defense
        public int defenseHorizonal(int currentRow, int currentCol)
        {
            int score = 0;
            int ourChess = 0;
            int enemyChess = 0;
            for (int i = 1; i < 6 && currentCol - i >= 0; i++)
            {
                if (Matrix[currentRow][currentCol - i].BackColor == Color.White)
                {
                    ourChess++;
                    break;
                }
                else if (Matrix[currentRow][currentCol - i].BackColor == Color.Aqua)
                    enemyChess++;
                else
                    break;
            }
            for (int i = 1; i < 6 && currentCol + i < Const.CHESS_BOARD_LENGTH; i++)
            {
                if (Matrix[currentRow][currentCol + i].BackColor == Color.White)
                {
                    ourChess++;
                    break;
                }
                else if (Matrix[currentRow][currentCol + i].BackColor == Color.Aqua)
                    enemyChess++;
                else
                    break;
            }
            if (ourChess == 2)
                return 0;
            score += defenseScore[enemyChess];
            return score;
        }

        public int defenseVertical(int currentRow, int currentCol)
        {
            int score = 0;
            int ourChess = 0;
            int enemyChess = 0;
            for (int i = 1; i < 6 && currentRow - i >= 0; i++)
            {
                if (Matrix[currentRow - i][currentCol].BackColor == Color.White)
                {
                    ourChess++;
                    break;
                }
                else if (Matrix[currentRow - i][currentCol].BackColor == Color.Aqua)
                    enemyChess++;
                else
                    break;
            }
            for (int i = 1; i < 6 && currentRow + i < Const.CHESS_BOARD_LENGTH; i++)
            {
                if (Matrix[currentRow + i][currentCol].BackColor == Color.White)
                {
                    ourChess++;
                    break;
                }
                else if (Matrix[currentRow + i][currentCol].BackColor == Color.Aqua)
                    enemyChess++;
                else
                    break;
            }
            if (ourChess == 2)
                return 0;
            score += defenseScore[enemyChess];
            return score;
        }

        public int defenseSubDiagonal(int currentRow, int currentCol)
        {
            int score = 0;
            int ourChess = 0;
            int enemyChess = 0;
            for (int i = 1; i < 6 && currentRow - i >= 0 && currentCol + i < Const.CHESS_BOARD_LENGTH; i++)
            {
                if (Matrix[currentRow - i][currentCol + i].BackColor == Color.White)
                {
                    ourChess++;
                    break;
                }
                else if (Matrix[currentRow - i][currentCol + i].BackColor == Color.Aqua)
                    enemyChess++;
                else
                    break;
            }
            for (int i = 1; i < 6 && currentRow + i < Const.CHESS_BOARD_LENGTH && currentCol - i >= 0; i++)
            {
                if (Matrix[currentRow + i][currentCol - i].BackColor == Color.White)
                {
                    ourChess++;
                    break;
                }
                else if (Matrix[currentRow + i][currentCol - i].BackColor == Color.Aqua)
                    enemyChess++;
                else
                    break;
            }
            if (ourChess == 2)
                return 0;
            score += defenseScore[enemyChess];
            return score;
        }

        public int defenseMainDiagonal(int currentRow, int currentCol)
        {
            int score = 0;
            int ourChess = 0;
            int enemyChess = 0;
            for (int i = 1; i < 6 && currentRow - i >= 0 && currentCol - i >= 0; i++)
            {
                if (Matrix[currentRow - i][currentCol - i].BackColor == Color.White)
                {
                    ourChess++;
                    break;
                }
                else if (Matrix[currentRow - i][currentCol - i].BackColor == Color.Aqua)
                    enemyChess++;
                else
                    break;
            }
            for (int i = 1; i < 6 && currentRow + i < Const.CHESS_BOARD_LENGTH && currentCol + i < Const.CHESS_BOARD_LENGTH; i++)
            {
                if (Matrix[currentRow + i][currentCol + i].BackColor == Color.White)
                {
                    ourChess++;
                    break;
                }
                else if (Matrix[currentRow + i][currentCol + i].BackColor == Color.Aqua)
                    enemyChess++;
                else
                    break;
            }
            if (ourChess == 2)
                return 0;
            score += defenseScore[enemyChess];
            return score;
        }
        #endregion

        #region Cắt tỉa Alpha-Beta
        bool catTiaNgang(int currentRow, int currentCol)
        {
            if (currentCol < Const.CHESS_BOARD_LENGTH - 5)
                for (int i = 1; i < 6; i++)
                    if (Matrix[currentRow][currentCol + i].BackColor != Color.FromArgb(200, 200, 200))
                        return false;
            if (currentCol > 4)
                for (int i = 1; i < 6; i++)
                    if (Matrix[currentRow][currentCol - i].BackColor != Color.FromArgb(200, 200, 200))
                        return false;
            return true;
        }

        bool catTiaDoc(int currentRow, int currentCol)
        {
            if (currentRow > 4)
                for (int i = 1; i < 6; i++)
                    if (Matrix[currentRow - i][currentCol].BackColor != Color.FromArgb(200, 200, 200))
                        return false;
            if (currentRow < Const.CHESS_BOARD_LENGTH - 5)
                for (int i = 1; i < 6; i++)
                    if (Matrix[currentRow + i][currentCol].BackColor != Color.FromArgb(200, 200, 200))
                        return false;
            return true;
        }

        bool catTiaCheoChinh(int currentRow, int currentCol)
        {
            if (currentRow > 4 && currentCol > 4)
                for (int i = 1; i < 6; i++)
                    if (Matrix[currentRow - i][currentCol - i].BackColor != Color.FromArgb(200, 200, 200))
                        return false;
            if (currentRow < Const.CHESS_BOARD_LENGTH - 5 && currentCol < Const.CHESS_BOARD_LENGTH - 5)
                for (int i = 1; i < 6; i++)
                    if (Matrix[currentRow + i][currentCol + i].BackColor != Color.FromArgb(200, 200, 200))
                        return false;
            return true;
        }

        bool catTiaCheoPhu(int currentRow, int currentCol)
        {
            if (currentRow > 4 && currentCol < Const.CHESS_BOARD_LENGTH - 5)
                for (int i = 1; i < 6; i++)
                    if (Matrix[currentRow - i][currentCol + i].BackColor != Color.FromArgb(200, 200, 200))
                        return false;
            if (currentRow < Const.CHESS_BOARD_LENGTH - 5 && currentCol > 4)
                for (int i = 1; i < 6; i++)
                    if (Matrix[currentRow + i][currentCol - i].BackColor != Color.FromArgb(200, 200, 200))
                        return false;
            return true;
        }

        bool catTia(int currentRow, int currentCol)
        {
            if (catTiaNgang(currentRow, currentCol) && catTiaDoc(currentRow, currentCol) && catTiaCheoChinh(currentRow, currentCol) && catTiaCheoPhu(currentRow, currentCol))
                return true;
            return false;
        }
        #endregion

        #region Máy đánh
        private Point getCoordinateOfComputerChess()
        {
            Point point = new Point();
            int maxScore = 0, attackScore = 0, defenseScore = 0;
            if ((Players[0].Name == "Máy tính" && PlayTimeLine.Count == 0) || (Players[1].Name == "Máy tính" && PlayTimeLine.Count == 1))
            {
                do
                {
                    Random random = new Random();
                    point.X = random.Next(Const.CHESS_BOARD_LENGTH / 2 - 3, Const.CHESS_BOARD_LENGTH / 2 + 3);
                    point.Y = random.Next(Const.CHESS_BOARD_LENGTH / 2 - 3, Const.CHESS_BOARD_LENGTH / 2 + 3);
                } while (Matrix[point.Y][point.X].BackgroundImage != null);
            }
            else
                for (int i = 0; i < Const.CHESS_BOARD_LENGTH; i++)
                    for (int j = 0; j < Const.CHESS_BOARD_LENGTH; j++)
                        if (Matrix[i][j].BackColor == Color.FromArgb(200, 200, 200) && !catTia(i, j))
                        {
                            int tempScore = 0;
                            attackScore = attackHorizonal(i, j) + attackVertical(i, j) + attackMainDiagonal(i, j) + attackSubDiagonal(i, j);
                            defenseScore = defenseHorizonal(i, j) + defenseVertical(i, j) + defenseMainDiagonal(i, j) + defenseSubDiagonal(i, j);
                            tempScore = attackScore > defenseScore ? attackScore : defenseScore;
                            if (maxScore < tempScore)
                            {
                                maxScore = tempScore;
                                point.X = i;
                                point.Y = j;
                            }
                        }
            return point;
        }
        #endregion
        #endregion
    }
}
