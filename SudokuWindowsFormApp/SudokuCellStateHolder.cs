namespace SudokuWindowsFormApp
{
    internal class SudokuCellStateHolder
    {
        private string notesString;
        private int value;

        public SudokuCellStateHolder(string notesString, int value)
        {
            this.notesString = notesString;
            this.value = value;
        }

        public string NotesString { get { return notesString; } }
        public int Value { get { return value; } }
    }
}