using System;
using System.Collections.Generic;

namespace Logik.Core {

    public class LogikException : Exception {
        public LogikException(string message) : base(message) { }
    }

    public delegate void CellEvent(Cell cell);

    public class Cell {
        private static readonly string DefaultCellFormula = "0";
        private Model model;

        public string Id { get; }

        private string formula = DefaultCellFormula;
        public string Formula {
            get => formula;
            set {
                formula = value;
                model?.CellFormulaChanged(this);
            }
        }

        private string value;
        public string Value {
            get => value;
            set {
                this.value = value;
                ContentChanged?.Invoke(this);
            }
        }

        public bool Error { get; private set; }

        public event CellEvent ContentChanged;

        public void SetError(string message) {
            Error = true;
            Value = message;
        }

        public void ClearError() {
            Error = false;
        }

        public Cell(string id, Model model) {
            this.model = model;
            Id = id;
        }
    }

    public class Model {

        private Dictionary<string, Cell> cells = new Dictionary<string, Cell>();
        private Dictionary<Cell, HashSet<Cell>> references = new Dictionary<Cell, HashSet<Cell>>();
        private Dictionary<Cell, HashSet<Cell>> deepReferences = new Dictionary<Cell, HashSet<Cell>>();

        private readonly Evaluator evaluator = new Evaluator();

        private int lastCellIndex = 1;

        private string GenerateCellName() {
            return "C" + (lastCellIndex++);
        }

        public Cell CreateCell() {
            var cell = new Cell(GenerateCellName(), this);
            cells.Add(cell.Id, cell);
            evaluator.DefineCell(cell, cell.Formula);
            references[cell] = new HashSet<Cell>();
            deepReferences[cell] = new HashSet<Cell>();
            return cell;
        }

        public List<Cell> GetReferences(Cell cell) {
            return new List<Cell>(references[cell]);
        }

        public void CellFormulaChanged(Cell cell) {
            try {
                cell.ClearError();
                evaluator.DefineCell(cell, cell.Formula);
                UpdateReferences(cell);
                DeepUpdateValue(cell);
            } catch (Exception e) {
                ClearReferences(cell);
                DeepPropagateError(cell, cell.Id + ": " + e.Message);
            }
        }

        private void DeepPropagateError(Cell cell, string message) {
            cell.SetError(message);
            var cells = GetCellsReferencing(cell);
            foreach (Cell other in cells) {
                DeepPropagateError(other, message);
            }
        }

        private void ClearReferences(Cell cell) {
            references[cell].Clear();
            deepReferences[cell].Clear();
        }

        private void DeepUpdateValue(Cell cell) {
            cell.Value = evaluator.EvaluateCell(cell);

            var cells = GetCellsReferencing(cell);
            foreach (Cell other in cells) {
                DeepUpdateValue(other);
            }
        }

        public void UpdateReferences(Cell cell) {
            var refs = new HashSet<Cell>(evaluator.GetReferencedIds(cell).ConvertAll( (name) => GetCell(name) ));
            references[cell] = new HashSet<Cell>(refs);
            deepReferences[cell] = new HashSet<Cell>(refs);

            if (refs.Contains(cell))
                throw new LogikException("Self reference in Cell " + cell.Id);

            foreach (var other in refs) {
                deepReferences[cell].UnionWith(deepReferences[other]);
            }
            if (deepReferences[cell].Contains(cell))
                throw new LogikException("Circular reference found including Cell " + cell.Id);
        }

        public IEnumerable<Cell> GetCellsReferencing(Cell cell) {
            foreach (var otherKV in references) {
                if (otherKV.Value.Contains(cell)) {
                    yield return otherKV.Key;
                }
            }
        }

        public void RemoveCell(Cell cell) {

            foreach (var referencer in GetCellsReferencing(cell)) {
                referencer.SetError("Reference to invalid Cell " + cell.Id);
                RemoveReference(referencer, cell);
            }

            cells.Remove(cell.Id);
        }

        private void RemoveReference(Cell referencer, Cell referenced) {
            references[referencer].Remove(referenced);
            UpdateReferences(referencer);
        }

        public Cell GetCell(string id) {
            return cells[id];
        }

        public void Clear() {
            cells.Clear();
            references.Clear();
            deepReferences.Clear();
            lastCellIndex = 1;
        }
    }
}
