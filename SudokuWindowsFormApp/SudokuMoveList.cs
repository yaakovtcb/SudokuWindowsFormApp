using System;
using System.Collections.Generic;

namespace SudokuWindowsFormApp
{
    internal class SudokuMoveList
    {
        private List<SudokuCellStateChange> moves;
        private int currentIndex;

        public SudokuMoveList()
        {
            moves = new List<SudokuCellStateChange>();
            currentIndex = -1;
        }

        public int MoveIndex
        {
            get { return currentIndex; }
            set
            {
                if (value > -2 && value < moves.Count)
                    currentIndex = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }

        public int Length { get { return moves.Count; } }

        public void AddMove(SudokuCellStateChange newMove)
        {
            currentIndex++;
            while (currentIndex < moves.Count)
                moves.RemoveAt(moves.Count - 1);
            moves.Add(newMove);
        }

        public SudokuCellStateHolder getPreviousState(out int rowIndex, out int columnIndex)
        {
            if (currentIndex > -1)
            {
                rowIndex = moves[currentIndex].RowIndex;
                columnIndex = moves[currentIndex].ColumnIndex;
                SudokuCellStateChange move = moves[currentIndex];
                currentIndex--;
                return move.Before;
            }
            else
            {
                throw new IndexOutOfRangeException("Index out of range.");
            }
        }

        public SudokuCellStateHolder getNextState(out int rowIndex, out int columnIndex)
        {
            if (currentIndex < moves.Count - 1)
            {
                currentIndex++;
                rowIndex = moves[currentIndex].RowIndex;
                columnIndex = moves[currentIndex].ColumnIndex;
                SudokuCellStateChange move = moves[currentIndex];
                return move.After;
            }
            else
            {
                throw new IndexOutOfRangeException("Index out of range.");
            }
        }

        public SudokuCellStateChange getMove(int index)
        {
            if (index >= 0 && index < moves.Count)
                return moves[index];
            else
                throw new IndexOutOfRangeException("Index out of range.");
        }
    }
}