using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SudokuWindowsFormApp
{
    internal class BaseSudokuBoard
    {
        private const int groupSize = 9;

        // One shared Random for the whole app. Creating a new Random on every call
        // can return the same sequence when calls happen within the same millisecond.
        private static readonly Random random = new Random();

        protected SudokuCell[,] sudokuCells;
        protected List<int>[] rowsOptions;
        protected List<int>[] columnsOptions;
        protected List<int>[] nonentsOptions;
        protected Control.ControlCollection formControlCollection;

        public BaseSudokuBoard(Control.ControlCollection formControlCollection)
        {
            this.formControlCollection = formControlCollection;
            InitialiseOptionCollections();
            InitialiseSudokuCells();
        }

        private void InitialiseOptionCollections()
        {
            rowsOptions = new List<int>[groupSize];
            columnsOptions = new List<int>[groupSize];
            nonentsOptions = new List<int>[groupSize];
            for (int i = 0; i < groupSize; i++)
            {
                rowsOptions[i] = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                columnsOptions[i] = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                nonentsOptions[i] = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            }
        }

        private void InitialiseSudokuCells()
        {
            sudokuCells = new SudokuCell[groupSize, groupSize];
            for (int i = 0; i < sudokuCells.GetLength(0); i++)
                for (int j = 0; j < sudokuCells.GetLength(1); j++)
                    sudokuCells[i, j] = new SudokuCell(0, i, j);
        }

        protected void AddToForm()
        {
            for (int i = 0; i < sudokuCells.GetLength(0); i++)
                for (int j = 0; j < sudokuCells.GetLength(1); j++)
                    formControlCollection.Add(sudokuCells[i, j]);
        }

        protected void ToggleValue(int value, SudokuCell sudokuCell)
        {
            if (sudokuCell.Value == value)
                UnuseValue(sudokuCell);
            else if (value >= 0 && value <= 9)
                UseValue(value, sudokuCell);
        }

        private bool ValueIsValid(int value, SudokuCell sudokuCell, out string errorExplenaion)
        {
            int rowIndex = sudokuCell.RowIndex, columnIndex = sudokuCell.ColumnIndex, nonentIndex = sudokuCell.NonentIndex;
            errorExplenaion = "";

            if (value == 0)
                return true;
            else if (value < 0 || value > 9)
                return false;
            else if (!rowsOptions[rowIndex].Contains(value))
            {
                errorExplenaion = $"The value {value} can not appear twice in row {rowIndex + 1}.";
                return false;
            }
            else if (!columnsOptions[columnIndex].Contains(value))
            {
                errorExplenaion = $"The value {value} can not appear twice in column {columnIndex + 1}.";
                return false;
            }
            else if (!nonentsOptions[nonentIndex].Contains(value))
            {
                errorExplenaion = $"The value {value} can not appear twice in nonent {nonentIndex + 1}.";
                return false;
            }
            return true;
        }

        protected void UseValue(int value, SudokuCell sudokuCell)
        {
            int rowIndex = sudokuCell.RowIndex, columnIndex = sudokuCell.ColumnIndex, nonentIndex = sudokuCell.NonentIndex;
            string errorExplenation;

            if (ValueIsValid(value, sudokuCells[rowIndex, columnIndex], out errorExplenation))
            {
                if (sudokuCells[rowIndex, columnIndex].Value > 0)
                    UnuseValue(sudokuCell);

                sudokuCells[rowIndex, columnIndex].Value = value;

                if (value > 0)
                {
                    rowsOptions[rowIndex].Remove(value);
                    columnsOptions[columnIndex].Remove(value);
                    nonentsOptions[nonentIndex].Remove(value);
                }
            }
            else
            {
                throw new Exception(errorExplenation);
            }
        }

        protected void UnuseValue(SudokuCell sudokuCell)
        {
            int rowIndex = sudokuCell.RowIndex, columnIndex = sudokuCell.ColumnIndex, nonentIndex = sudokuCell.NonentIndex;
            int value = sudokuCells[rowIndex, columnIndex].Value;

            if (value > 0)
            {
                sudokuCells[rowIndex, columnIndex].Value = 0;
                rowsOptions[rowIndex].Add(value);
                columnsOptions[columnIndex].Add(value);
                nonentsOptions[nonentIndex].Add(value);
            }
        }

        protected void DisableAllSudokuCells()
        {
            for (int i = 0; i < sudokuCells.GetLength(0); i++)
                for (int j = 0; j < sudokuCells.GetLength(1); j++)
                    sudokuCells[i, j].Enabled = false;
        }

        public virtual void RemoveFromForm()
        {
            for (int i = 0; i < sudokuCells.GetLength(0); i++)
                for (int j = 0; j < sudokuCells.GetLength(1); j++)
                    formControlCollection.Remove(sudokuCells[i, j]);
        }

        private int[] GetCurrentPossibilities(int rowIndex, int columnIndex, int nonentIndex)
        {
            int[] possibilities = rowsOptions[rowIndex]
                .Intersect(columnsOptions[columnIndex])
                .Intersect(nonentsOptions[nonentIndex])
                .ToArray();
            return possibilities;
        }

        /// <summary>
        /// Returns the possible values of a cell in a random order (Fisher-Yates shuffle).
        /// </summary>
        private int[] GetCurrentShuffelledPossibilities(int rowIndex, int columnIndex, int nonentIndex)
        {
            int[] possibilities = GetCurrentPossibilities(rowIndex, columnIndex, nonentIndex);
            for (int i = possibilities.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int temp = possibilities[i];
                possibilities[i] = possibilities[j];
                possibilities[j] = temp;
            }
            return possibilities;
        }

        /// <summary>
        /// Recursive backtracking solver. The shuffle flag is passed down the recursion,
        /// so every cell (not only the first one) gets its candidates in random order.
        /// </summary>
        private bool Solve(int rowIndex, int columnIndex, bool shuffle = false)
        {
            if (sudokuCells[rowIndex, columnIndex].Value > 0)
            {
                if (rowIndex == groupSize - 1 && columnIndex == groupSize - 1)
                    return true;
                else
                    return Solve(rowIndex + (columnIndex / (groupSize - 1)), (columnIndex + 1) % groupSize, shuffle);
            }
            else
            {
                int nonentIndex = sudokuCells[rowIndex, columnIndex].NonentIndex;
                int[] possibleNumbers = shuffle
                    ? GetCurrentShuffelledPossibilities(rowIndex, columnIndex, nonentIndex)
                    : GetCurrentPossibilities(rowIndex, columnIndex, nonentIndex);

                foreach (int number in possibleNumbers)
                {
                    UseValue(number, sudokuCells[rowIndex, columnIndex]);

                    if (rowIndex == 8 && columnIndex == 8)
                        return true;
                    else if (Solve(rowIndex + (columnIndex / (groupSize - 1)), (columnIndex + 1) % groupSize, shuffle))
                        return true;
                    else
                        UnuseValue(sudokuCells[rowIndex, columnIndex]);
                }
                return false;
            }
        }

        public void Solve(bool shuffle, bool showMessage = false)
        {
            DisableAllSudokuCells();
            bool success = Solve(0, 0, shuffle);
            if (showMessage)
            {
                if (success)
                    MessageBox.Show("The sudoku board was solved successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Invalid sudoku board", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}