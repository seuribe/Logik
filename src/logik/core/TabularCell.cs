using System;
using System.Collections.Generic;

namespace Logik.Core {

    public delegate void GridCellEvent(int row, int column);

    /// <summary>
    /// Helper class for exchanging the value of each grid element
    /// </summary>
    public class GridCellData {
        public int Row { get; set; }
        public int Column { get; set; }
        public Value Value { get; set; }
    }

    /// <summary>
    /// A cell holding several tabular/grid values
    /// </summary>
    public class TabularCell : BaseCell {
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        private Value[,] data;

        public override event CellEvent ValueChanged;
        public override event CellEvent ContentChanged;

        public TabularCell(string name = null, int rows = 1, int columns = 1) : base(name ?? "T") {
            Rows = rows;
            Columns = columns;
            data = new Value[rows, columns];
        }

        public Value this[int row, int column] {
            get {
                CheckRange(row, column);
                return data[row, column];
            }
            set {
                CheckRange(row, column);
                data[row, column] = value;
                ContentChanged?.Invoke(this);
                ValueChanged?.Invoke(this);
            }
        }

        private void CheckRange(int row, int column) {
            if (row < 0 || row >= Rows || column < 0 || column >= Columns)
                throw new LogikException($"Invalid Index. {row},{column} is outside {Name}'s size of {Rows}x{Columns}");
        }

        public void Resize(int rows, int columns) {
            EnsureSpace(rows, columns);

            Rows = rows;
            Columns = columns;
        }

        private void EnsureSpace(int rows, int columns) {
            if (rows <= data.GetLength(0) && columns <= data.GetLength(1))
                return;

            var newData = new Value[ Math.Max(rows, Rows), Math.Max(columns, Columns) ];

            for (int r = 0 ; r < Rows ; r++)
                for (int c = 0 ; c < Columns; c++)
                    newData[r,c] = data[r,c];

            data = newData;
        }

        internal void SetData(IEnumerable<GridCellData> gcds) {
            foreach (var gcd in gcds)
                data[gcd.Row, gcd.Column] = gcd.Value;
        }
    }
}
