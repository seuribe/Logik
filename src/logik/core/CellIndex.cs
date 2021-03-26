using System.Collections.Generic;

namespace Logik.Core {
    public class CellIndex {
        private Dictionary<string, Cell> cells = new Dictionary<string, Cell>();

        private readonly Evaluator evaluator = new Evaluator();

        private int lastCellIndex = 1;

        private string GenerateCellName() {
            return "C" + (lastCellIndex++);
        }

        public Cell CreateCell() {
            var cell = new Cell(GenerateCellName(), this, evaluator);
            cells.Add(cell.Id, cell);
            return cell;
        }

        public void RemoveCell(Cell cell) {
            cells.Remove(cell.Id);
            foreach (var other in cells.Values) {
                other.Ignore(cell);
                cell.Ignore(other);
            }
        }

        public Cell GetCell(string id) {
            return cells[id];
        }

        public void Clear() {
            cells.Clear();
            lastCellIndex = 1;
        }
    }
}
