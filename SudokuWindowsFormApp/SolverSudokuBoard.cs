using System;
using System.Drawing;
using System.Windows.Forms;

namespace SudokuWindowsFormApp
{
    internal class SolverSudokuBoard : BaseSudokuBoard
    {
        private Button buttonSolve;

        public SolverSudokuBoard(Control.ControlCollection formControlCollection) : base(formControlCollection)
        {
            buttonSolve = new Button();
            buttonSolve.BackColor = Color.LightGray;
            buttonSolve.Text = "Solve";
            buttonSolve.Location = new Point(sudokuCells[8, 3].Left, sudokuCells[8, 3].Bottom);
            buttonSolve.Size = new Size(SudokuCell.SideLength * 3, SudokuCell.SideLength);
            buttonSolve.Font = new Font("impact", 16);
            buttonSolve.Click += new EventHandler((sender, e) =>
            {
                buttonSolve.Enabled = false;
                Solve(false, true);
            });

            for (int i = 0; i < sudokuCells.GetLength(0); i++)
                for (int j = 0; j < sudokuCells.GetLength(1); j++)
                    sudokuCells[i, j].KeyPress += new KeyPressEventHandler(SudokuCellKeyPressed);

            AddToForm();
        }

        new private void AddToForm()
        {
            base.AddToForm();
            formControlCollection.Add(buttonSolve);
        }

        public override void RemoveFromForm()
        {
            base.RemoveFromForm();
            formControlCollection.Remove(buttonSolve);
        }

        private void SudokuCellKeyPressed(object sender, KeyPressEventArgs e)
        {
            try
            {
                ToggleValue(e.KeyChar - '0', sender as SudokuCell);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}