using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SudokuWindowsFormApp
{
    internal enum EMPTY_CELLS_FOR_DIFFICULTY
    {
        EASY_DIFFICULTY_AMOUNT = 31,
        MEDIUM_DIFFICULTY_AMOUNT = 46,
        HARD_DIFFICULTY_AMOUNT = 56
    }

    internal class UserSudokuBoard : BaseSudokuBoard
    {
        private readonly Font controlFont = new Font("impact", 16);
        private Button buttonUndo, buttonRedo;
        private CheckBox markModeCheckBox;
        private SudokuMoveList moveList;

        private UserSudokuBoard(Control.ControlCollection formControlCollection) : base(formControlCollection)
        {
            InitializeCheckBox();
            InitializeButtons();
            moveList = new SudokuMoveList();

            for (int i = 0; i < sudokuCells.GetLength(0); i++)
                for (int j = 0; j < sudokuCells.GetLength(1); j++)
                    sudokuCells[i, j].KeyPress += new KeyPressEventHandler(SudokuCellKeyPressed);
        }

        public UserSudokuBoard(Control.ControlCollection formControlCollection, string difficulty)
            : this(formControlCollection)
        {
            ShuffleBoard(difficulty);
            AddToForm();
        }

        private void InitializeCheckBox()
        {
            markModeCheckBox = new CheckBox();
            markModeCheckBox.Checked = false;
            markModeCheckBox.Text = "Note✏";
            markModeCheckBox.Padding = new Padding(SudokuCell.SideLength / 2, SudokuCell.SideLength / 10, 0, 0);
            markModeCheckBox.Location = new Point(SudokuCell.SideLength * 3, SudokuCell.SideLength * 9);
            markModeCheckBox.Size = new Size(SudokuCell.SideLength * 3, SudokuCell.SideLength);
            markModeCheckBox.Font = controlFont;
        }

        private void InitializeButtons()
        {
            buttonUndo = new Button();
            buttonRedo = new Button();
            buttonUndo.Size = buttonRedo.Size = new Size(SudokuCell.SideLength * 3, SudokuCell.SideLength);
            buttonUndo.BackColor = buttonRedo.BackColor = Color.LightGray;
            buttonUndo.Location = new Point(0, SudokuCell.SideLength * 9);
            buttonRedo.Location = new Point(SudokuCell.SideLength * 6, SudokuCell.SideLength * 9);
            buttonUndo.Text = "Undo ↺";
            buttonRedo.Text = "Redo ↻";
            buttonUndo.Click += new EventHandler(UndoMove);
            buttonRedo.Click += new EventHandler(RedoMove);
            buttonUndo.Font = buttonRedo.Font = controlFont;
            buttonUndo.Enabled = buttonRedo.Enabled = false;
        }

        private void SudokuCellKeyPressed(object sender, KeyPressEventArgs e)
        {
            try
            {
                SudokuCell sudokuCell = (SudokuCell)sender;
                bool keyIsValid = (e.KeyChar >= '0' && e.KeyChar <= '9');
                bool noEmptyMove = (sudokuCell.Value > 0 || e.KeyChar > '0');

                if (keyIsValid && noEmptyMove)
                {
                    sudokuCell.ForeColor = Color.Blue;
                    SudokuCellStateHolder before, after;
                    before = new SudokuCellStateHolder(sudokuCell.GetNotesAsString(), sudokuCell.Value);

                    if (markModeCheckBox.Checked)
                    {
                        sudokuCell.ToggleNote(e.KeyChar - '0');
                    }
                    else
                    {
                        ToggleValue(e.KeyChar - '0', sudokuCell);
                        if (WinCheck())
                        {
                            DisableAllSudokuCells();
                            MessageBox.Show("You win!");
                        }
                    }

                    after = new SudokuCellStateHolder(sudokuCell.GetNotesAsString(), sudokuCell.Value);
                    moveList.AddMove(new SudokuCellStateChange(sudokuCell.RowIndex, sudokuCell.ColumnIndex, before, after));
                    buttonUndo.Enabled = true;
                    buttonRedo.Enabled = false;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShuffleBoard(string difficulty)
        {
            Solve(true);

            Random random = new Random();
            int amountOfCellsToEmpty, rowIndex, columnIndex, nonentIndex;

            if (difficulty == "easy")
                amountOfCellsToEmpty = (int)EMPTY_CELLS_FOR_DIFFICULTY.EASY_DIFFICULTY_AMOUNT;
            else if (difficulty == "medium")
                amountOfCellsToEmpty = (int)EMPTY_CELLS_FOR_DIFFICULTY.MEDIUM_DIFFICULTY_AMOUNT;
            else if (difficulty == "hard")
                amountOfCellsToEmpty = (int)EMPTY_CELLS_FOR_DIFFICULTY.HARD_DIFFICULTY_AMOUNT;
            else
                throw new ArgumentException("Invalid difficulty level.");

            for (int i = 0; i < amountOfCellsToEmpty; i++)
            {
                do
                {
                    rowIndex = random.Next(9);
                    columnIndex = random.Next(9);
                    nonentIndex = sudokuCells[rowIndex, columnIndex].NonentIndex;
                } while (sudokuCells[rowIndex, columnIndex].Value == 0);

                rowsOptions[rowIndex].Add(sudokuCells[rowIndex, columnIndex].Value);
                columnsOptions[columnIndex].Add(sudokuCells[rowIndex, columnIndex].Value);
                nonentsOptions[nonentIndex].Add(sudokuCells[rowIndex, columnIndex].Value);
                sudokuCells[rowIndex, columnIndex].Value = 0;
                sudokuCells[rowIndex, columnIndex].Enabled = true;
            }
        }

        private void UndoMove(object sender, EventArgs e)
        {
            if (moveList.MoveIndex > -1)
            {
                int rowIndex, columnIndex;
                SudokuCellStateHolder stateHolder = moveList.getPreviousState(out rowIndex, out columnIndex);
                sudokuCells[rowIndex, columnIndex].SetNotesFromString(stateHolder.NotesString);
                UnuseValue(sudokuCells[rowIndex, columnIndex]);
                UseValue(stateHolder.Value, sudokuCells[rowIndex, columnIndex]);

                buttonRedo.Enabled = true;
                if (moveList.MoveIndex == -1)
                    buttonUndo.Enabled = false;
            }
        }

        private void RedoMove(object sender, EventArgs e)
        {
            if (moveList.MoveIndex < moveList.Length - 1)
            {
                int rowIndex, columnIndex;
                SudokuCellStateHolder stateHolder = moveList.getNextState(out rowIndex, out columnIndex);
                sudokuCells[rowIndex, columnIndex].SetNotesFromString(stateHolder.NotesString);
                UnuseValue(sudokuCells[rowIndex, columnIndex]);
                UseValue(stateHolder.Value, sudokuCells[rowIndex, columnIndex]);

                buttonUndo.Enabled = true;
                if (moveList.MoveIndex == moveList.Length - 1)
                    buttonRedo.Enabled = false;
            }
        }

        public bool WinCheck()
        {
            for (int i = 0; i < 9; i++)
            {
                if (rowsOptions[i].Count > 0 || columnsOptions[i].Count > 0 || nonentsOptions[i].Count > 0)
                    return false;
            }
            return true;
        }

        public bool ToFile(string fileName)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
                {
                    for (int i = 0; i < sudokuCells.GetLength(0); i++)
                    {
                        for (int j = 0; j < sudokuCells.GetLength(1); j++)
                        {
                            writer.Write(sudokuCells[i, j].GetNotesAsString());
                            writer.Write(sudokuCells[i, j].Value);
                            writer.Write(sudokuCells[i, j].Enabled);
                        }
                    }

                    writer.Write(moveList.MoveIndex);
                    for (int i = 0; i < moveList.Length; i++)
                    {
                        SudokuCellStateChange move = moveList.getMove(i);
                        writer.Write(move.RowIndex);
                        writer.Write(move.ColumnIndex);
                        writer.Write(move.Before.NotesString);
                        writer.Write(move.Before.Value);
                        writer.Write(move.After.NotesString);
                        writer.Write(move.After.Value);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static UserSudokuBoard FromFile(string fileName, Control.ControlCollection formControlCollection)
        {
            UserSudokuBoard userSudokuBoard = new UserSudokuBoard(formControlCollection);
            userSudokuBoard.AddToForm();

            try
            {
                using (BinaryReader reader = new BinaryReader(new FileStream(fileName, FileMode.Open)))
                {
                    for (int i = 0; i < userSudokuBoard.sudokuCells.GetLength(0); i++)
                    {
                        for (int j = 0; j < userSudokuBoard.sudokuCells.GetLength(1); j++)
                        {
                            userSudokuBoard.sudokuCells[i, j].SetNotesFromString(reader.ReadString());
                            userSudokuBoard.UseValue(reader.ReadInt32(), userSudokuBoard.sudokuCells[i, j]);
                            userSudokuBoard.sudokuCells[i, j].Enabled = reader.ReadBoolean();
                            if (userSudokuBoard.sudokuCells[i, j].Enabled)
                                userSudokuBoard.sudokuCells[i, j].ForeColor = Color.Blue;
                        }
                    }

                    int currentMoveIndex = reader.ReadInt32();
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        int rowIndex = reader.ReadInt32();
                        int columnIndex = reader.ReadInt32();
                        string notesBefore = reader.ReadString();
                        int valueBefore = reader.ReadInt32();
                        string notesAfter = reader.ReadString();
                        int valueAfter = reader.ReadInt32();

                        SudokuCellStateHolder before = new SudokuCellStateHolder(notesBefore, valueBefore);
                        SudokuCellStateHolder after = new SudokuCellStateHolder(notesAfter, valueAfter);
                        userSudokuBoard.moveList.AddMove(
                            new SudokuCellStateChange(rowIndex, columnIndex, before, after));
                    }

                    userSudokuBoard.moveList.MoveIndex = currentMoveIndex;
                    if (currentMoveIndex > -1)
                        userSudokuBoard.buttonUndo.Enabled = true;
                    if (currentMoveIndex < userSudokuBoard.moveList.Length - 1)
                        userSudokuBoard.buttonRedo.Enabled = true;
                }
                return userSudokuBoard;
            }
            catch (FileNotFoundException)
            {
                userSudokuBoard.RemoveFromForm();
                MessageBox.Show("could not find Saved version, please select another difficulty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            catch (Exception)
            {
                userSudokuBoard.RemoveFromForm();
                MessageBox.Show("could not open Saved version, please select another difficulty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        new private void AddToForm()
        {
            base.AddToForm();
            formControlCollection.Add(markModeCheckBox);
            formControlCollection.Add(buttonUndo);
            formControlCollection.Add(buttonRedo);
        }

        public override void RemoveFromForm()
        {
            base.RemoveFromForm();
            formControlCollection.Remove(markModeCheckBox);
            formControlCollection.Remove(buttonUndo);
            formControlCollection.Remove(buttonRedo);
        }
    }
}