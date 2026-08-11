namespace SudokuWindowsFormApp
{
    internal class SudokuCellStateChange
    {
        private int rowIndex, columnIndex;
        private SudokuCellStateHolder before, after;

        public SudokuCellStateChange(int rowIndex, int columnIndex, SudokuCellStateHolder before, SudokuCellStateHolder after)
        {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.before = before;
            this.after = after;
        }

        public int RowIndex { get { return rowIndex; } }
        public int ColumnIndex { get { return columnIndex; } }
        public SudokuCellStateHolder Before { get { return before; } }
        public SudokuCellStateHolder After { get { return after; } }
    }
}