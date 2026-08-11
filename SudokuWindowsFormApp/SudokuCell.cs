using System;
using System.Drawing;
using System.Windows.Forms;

namespace SudokuWindowsFormApp
{
    internal class SudokuCell : Button
    {
        private char[] userNotes;
        private int value, rowIndex, columnIndex, nonentIndex;
        public const int SideLength = 75;
        private readonly Font valueFont = new Font("impact", 32);
        private readonly Font noteFont = new Font("david", 14);

        public SudokuCell(int value, int rowIndex, int columnIndex)
            : this(new string(' ', 9), value, rowIndex, columnIndex, true) { }

        public SudokuCell(string noteString, int value, int rowIndex, int columnIndex, bool isEnabled)
        {
            userNotes = new char[9];
            for (int i = 0; i < 9; i++)
                userNotes[i] = noteString[i];

            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            nonentIndex = rowIndex - rowIndex % 3 + columnIndex / 3;

            Value = value; // אם 0 - יוצגו הפתקים, אחרת - הערך

            Location = new Point(SideLength * columnIndex, SideLength * rowIndex);
            Size = new Size(SideLength, SideLength);
            Font = valueFont;
            BackColor = (nonentIndex % 2 == 0) ? Color.White : Color.Wheat;
            Enabled = isEnabled;
        }

        public int Value
        {
            get { return value; }
            set
            {
                Font = valueFont;
                if (value >= 1 && value <= 9)
                {
                    this.value = value;
                    base.Text = value.ToString();
                }
                else if (value == 0)
                {
                    this.value = value;
                    DisplayNotes();
                }
                else
                {
                    throw new Exception("Sudoku cell's value must be between 0 and 9.");
                }
            }
        }

        public int RowIndex { get { return rowIndex; } }
        public int ColumnIndex { get { return columnIndex; } }
        public int NonentIndex { get { return nonentIndex; } }

        public void ToggleNote(int note)
        {
            if (note > 0 && note < 10)
            {
                int noteIndex = note - 1;
                if (userNotes[noteIndex] == ' ')
                    AddNote(note);
                else
                    RemoveNote(note);
                DisplayNotes();
            }
        }

        private void AddNote(int note)
        {
            int noteIndex = note - 1;
            userNotes[noteIndex] = (char)(note + '0');
        }

        private void RemoveNote(int note)
        {
            int noteIndex = note - 1;
            userNotes[noteIndex] = ' ';
        }

        private void DisplayNotes()
        {
            Font = noteFont;
            Text = "";
            for (int i = 0; i < userNotes.Length; i++)
            {
                Text += userNotes[i];
                if (i % 3 == 2 && i < 8)
                    Text += '\n';
                else
                    Text += ' ';
            }
        }

        public string GetNotesAsString()
        {
            string notesString = "";
            foreach (char note in userNotes)
                notesString += note;
            return notesString;
        }

        public void SetNotesFromString(string notesString)
        {
            if (notesString.Length == 9)
            {
                for (int i = 0; i < userNotes.Length; i++)
                {
                    if (notesString[i] == ' ' || notesString[i] == (char)((i + 1) + '0'))
                        userNotes[i] = notesString[i];
                    else
                        throw new Exception("Invalid notes");
                }
            }
            else
            {
                throw new Exception("Invalid notes");
            }
        }
    }
}