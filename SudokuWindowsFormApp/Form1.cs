using System;
using System.Windows.Forms;
using System.IO;

namespace SudokuWindowsFormApp
{
    internal enum SudokuBoardType { Empty, User, Solver }

    public partial class Form1 : Form
    {
        private const string fileName = "save_file.sdku";
        BaseSudokuBoard board;
        SudokuBoardType type;

        public Form1()
        {
            InitializeComponent();
            type = SudokuBoardType.Empty;
            board = null;
            CheckSavedBoard();
        }

        private void CheckSavedBoard()
        {
            radioButtonSavedBoard.Visible = File.Exists(fileName);
        }

        private void radioButtonUser_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxSelectDifficulty.Visible = radioButtonUser.Checked;
            CheckSavedBoard();
        }

        private void buttonGenerateBoard_Click(object sender, EventArgs e)
        {
            if (board != null)
                board.RemoveFromForm();

            if (radioButtonUser.Checked)
            {
                type = SudokuBoardType.User;

                if (radioButtonSavedBoard.Checked)
                {
                    board = UserSudokuBoard.FromFile(fileName, Controls);
                    if (board == null)
                    {
                        type = SudokuBoardType.Empty;
                        radioButtonSavedBoard.Visible = false;
                        radioButtonEasyLevel.Checked = true;
                    }
                }
                else if (radioButtonEasyLevel.Checked)
                {
                    board = new UserSudokuBoard(Controls, "easy");
                }
                else if (radioButtonMediumLevel.Checked)
                {
                    board = new UserSudokuBoard(Controls, "medium");
                }
                else
                {
                    board = new UserSudokuBoard(Controls, "hard");
                }
                buttonSaveBoard.Visible = true;
            }
            else
            {
                buttonSaveBoard.Visible = false;
                board = new SolverSudokuBoard(Controls);
                type = SudokuBoardType.Solver;
            }
        }

        private void buttonSaveBoard_Click(object sender, EventArgs e)
        {
            if (type == SudokuBoardType.User)
            {
                UserSudokuBoard userSudokuBoard = board as UserSudokuBoard;
                if (userSudokuBoard.ToFile(fileName))
                {
                    radioButtonSavedBoard.Visible = true;
                    MessageBox.Show("The board was saved successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Could not save board", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    radioButtonSavedBoard.Visible = false;
                }
            }
        }
    }
}