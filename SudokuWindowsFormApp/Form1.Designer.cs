using System.Drawing;
using System.Windows.Forms;

namespace SudokuWindowsFormApp
{
    public partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private GroupBox groupBoxBoardType;
        private RadioButton radioButtonUser;
        private RadioButton radioButtonSolver;
        private GroupBox groupBoxSelectDifficulty;
        private RadioButton radioButtonEasyLevel;
        private RadioButton radioButtonMediumLevel;
        private RadioButton radioButtonHardLevel;
        private RadioButton radioButtonSavedBoard;
        private Button buttonGenerateBoard;
        private Button buttonSaveBoard;

        private void InitializeComponent()
        {
            this.groupBoxBoardType = new GroupBox();
            this.radioButtonUser = new RadioButton();
            this.radioButtonSolver = new RadioButton();
            this.groupBoxSelectDifficulty = new GroupBox();
            this.radioButtonEasyLevel = new RadioButton();
            this.radioButtonMediumLevel = new RadioButton();
            this.radioButtonHardLevel = new RadioButton();
            this.radioButtonSavedBoard = new RadioButton();
            this.buttonGenerateBoard = new Button();
            this.buttonSaveBoard = new Button();

            this.groupBoxBoardType.SuspendLayout();
            this.groupBoxSelectDifficulty.SuspendLayout();
            this.SuspendLayout();

            // buttonSaveBoard
            this.buttonSaveBoard.Location = new Point(690, 10);
            this.buttonSaveBoard.Size = new Size(200, 40);
            this.buttonSaveBoard.Text = "Save";
            this.buttonSaveBoard.UseVisualStyleBackColor = true;
            this.buttonSaveBoard.Visible = false;
            this.buttonSaveBoard.Click += new System.EventHandler(this.buttonSaveBoard_Click);

            // buttonGenerateBoard
            this.buttonGenerateBoard.Location = new Point(690, 60);
            this.buttonGenerateBoard.Size = new Size(200, 40);
            this.buttonGenerateBoard.Text = "Start game";
            this.buttonGenerateBoard.UseVisualStyleBackColor = true;
            this.buttonGenerateBoard.Click += new System.EventHandler(this.buttonGenerateBoard_Click);

            // groupBoxBoardType
            this.groupBoxBoardType.Controls.Add(this.radioButtonUser);
            this.groupBoxBoardType.Controls.Add(this.radioButtonSolver);
            this.groupBoxBoardType.Location = new Point(690, 110);
            this.groupBoxBoardType.Size = new Size(200, 90);
            this.groupBoxBoardType.Text = "Board type";

            // radioButtonUser
            this.radioButtonUser.Location = new Point(10, 20);
            this.radioButtonUser.Size = new Size(150, 24);
            this.radioButtonUser.Text = "User board";
            this.radioButtonUser.Checked = true;
            this.radioButtonUser.CheckedChanged += new System.EventHandler(this.radioButtonUser_CheckedChanged);

            // radioButtonSolver
            this.radioButtonSolver.Location = new Point(10, 50);
            this.radioButtonSolver.Size = new Size(150, 24);
            this.radioButtonSolver.Text = "Solver board";

            // groupBoxSelectDifficulty
            this.groupBoxSelectDifficulty.Controls.Add(this.radioButtonEasyLevel);
            this.groupBoxSelectDifficulty.Controls.Add(this.radioButtonMediumLevel);
            this.groupBoxSelectDifficulty.Controls.Add(this.radioButtonHardLevel);
            this.groupBoxSelectDifficulty.Controls.Add(this.radioButtonSavedBoard);
            this.groupBoxSelectDifficulty.Location = new Point(690, 210);
            this.groupBoxSelectDifficulty.Size = new Size(200, 140);
            this.groupBoxSelectDifficulty.Text = "Select difficulty";

            // radioButtonEasyLevel
            this.radioButtonEasyLevel.Location = new Point(10, 20);
            this.radioButtonEasyLevel.Size = new Size(150, 24);
            this.radioButtonEasyLevel.Text = "Easy";
            this.radioButtonEasyLevel.Checked = true;

            // radioButtonMediumLevel
            this.radioButtonMediumLevel.Location = new Point(10, 45);
            this.radioButtonMediumLevel.Size = new Size(150, 24);
            this.radioButtonMediumLevel.Text = "Medium";

            // radioButtonHardLevel
            this.radioButtonHardLevel.Location = new Point(10, 70);
            this.radioButtonHardLevel.Size = new Size(150, 24);
            this.radioButtonHardLevel.Text = "Hard";

            // radioButtonSavedBoard
            this.radioButtonSavedBoard.Location = new Point(10, 95);
            this.radioButtonSavedBoard.Size = new Size(150, 24);
            this.radioButtonSavedBoard.Text = "Saved";
            this.radioButtonSavedBoard.Visible = false;

            // Form1
            this.ClientSize = new Size(950, 760);
            this.Text = "Sudoku Game";
            this.Controls.Add(this.groupBoxBoardType);
            this.Controls.Add(this.groupBoxSelectDifficulty);
            this.Controls.Add(this.buttonGenerateBoard);
            this.Controls.Add(this.buttonSaveBoard);

            this.groupBoxBoardType.ResumeLayout(false);
            this.groupBoxSelectDifficulty.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}