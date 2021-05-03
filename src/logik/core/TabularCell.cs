using System;

namespace Logik.Core {

    public class TabularCell : ICell {
        public string Name { get; private set; }
        public bool Error { get; private set; }
        public string ErrorMessage { get; private set; }

        private float[,] data = {{ 0 }};
        public int Rows { get; private set; } = 1;
        public int Columns { get; private set; } = 1;

        public event CellEvent ErrorStateChanged;

        public float this[int row, int column] {
            get {
                if (row < 0 || row >= Rows || column < 0 || column >= Columns)
                    throw new LogikException($"Invalid Index. {row},{column} is outside {Name}'s size of {Rows}x{Columns}");

                return data[row, column];
            }
            set {
                if (row < 0 || row >= Rows || column < 0 || column >= Columns)
                    throw new LogikException($"Invalid Index. {row},{column} is outside {Name}'s size of {Rows}x{Columns}");
                data[row, column] = value;
            }
        }

        public void Resize(int rows, int columns) {
            EnsureSpace(rows, columns);

            Rows = rows;
            Columns = columns;
        }

        private void EnsureSpace(int rows, int columns) {
            if (rows <= data.GetLength(0) && columns <= data.GetLength(1))
                return;

            var newData = new float[
                Math.Max(rows, Rows),
                Math.Max(columns, Columns)];

            for (int r = 0 ; r < Rows ; r++)
                for (int c = 0 ; c < Columns; c++)
                    newData[r,c] = data[r,c];

            data = newData;
        }
    }
}
