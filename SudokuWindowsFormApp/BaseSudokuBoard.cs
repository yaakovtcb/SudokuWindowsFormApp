using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SudokuWindowsFormApp
{
    internal class BaseSudokuBoard
    {
        private const int groupSize = 9;

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
                errorExplenaion = $"The value {value} can not appear twice in row {rowIndex}.";
                return false;
            }
            else if (!columnsOptions[columnIndex].Contains(value))
            {
                errorExplenaion = $"The value {value} can not appear twice in column {columnIndex}.";
                return false;
            }
            else if (!nonentsOptions[nonentIndex].Contains(value))
            {
                errorExplenaion = $"The value {value} can not appear twice in nonent {nonentIndex}.";
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

        private int[] GetCurrentShuffelledPossibilities(int rowIndex, int columnIndex, int nonentIndex)
        {
            Random random = new Random();
            int[] possibilities = GetCurrentPossibilities(rowIndex, columnIndex, nonentIndex);
            int[] shuffeledPossibilities = new int[possibilities.Length];
            for (int i = 0; i < shuffeledPossibilities.Length; i++)
                shuffeledPossibilities[i] = 0;

            int index;
            foreach (int value in possibilities)
            {
                do
                {
                    index = random.Next(shuffeledPossibilities.Length);
                } while (shuffeledPossibilities[index] > 0);
                shuffeledPossibilities[index] = value;
            }
            return shuffeledPossibilities;
        }

        private bool Solve(int rowIndex, int columnIndex, bool shuffle = false)
        {
            if (sudokuCells[rowIndex, columnIndex].Value > 0)
            {
                if (rowIndex == groupSize - 1 && columnIndex == groupSize - 1)
                    return true;
                else
                    return Solve(rowIndex + (columnIndex / (groupSize - 1)), (columnIndex + 1) % groupSize);
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
                    else if (Solve(rowIndex + (columnIndex / (groupSize - 1)), (columnIndex + 1) % groupSize))
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