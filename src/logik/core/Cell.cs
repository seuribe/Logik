
using System;
using System.Collections.Generic;

namespace Logik.Core {
    public class Cell {

        private string formula;
        private string value;
        private readonly Evaluator evaluator;
        private List<Cell> referenced = new List<Cell>();
        private readonly CellIndex cellIndex;

        public List<Cell> Referenced { get => referenced; }

        public bool Error { get; private set; } = false;

        public string Id { get; private set; }
        public string Value { get; private set; }

        private void NotifyObservers() {
            if (ValueChanged != null)
                ValueChanged(this);
        }

        public delegate void CellEvent(Cell cell);
        public event CellEvent ValueChanged;

        public Cell(string id, CellIndex cellIndex, Evaluator evaluator) {
            Id = id;
            this.cellIndex = cellIndex;
            this.evaluator = evaluator;
        }

        public string Formula {
            get => formula;
            set {
                formula = value;
                EvaluateSelf();
            }
        }

        private void EvaluateSelf() {
            try {
                evaluator.DefineCell(this, formula);
                UpdateReferences();
                Value = evaluator.EvaluateCell(this);
                Error = false;
            } catch (Exception e) {
                Value = "Error: " + e.Message;
                Error = true;
            }
            NotifyObservers();
        }

        internal void Observe(Cell cell) {
            cell.ValueChanged += ReferencedCellChanged;
        }

        internal void Ignore(Cell cell) {
            cell.ValueChanged -= ReferencedCellChanged;
        }

        private void UpdateReferences() {
            foreach (var other in referenced) {
                other.ValueChanged -= ReferencedCellChanged;
            }
            referenced = evaluator.GetReferencedCells(this).ConvertAll( (name) => cellIndex.GetCell(name) );
            foreach (var other in referenced) {
                other.ValueChanged += ReferencedCellChanged;
                Error |= other.Error;
            }
        }

        private void ReferencedCellChanged(Cell other) {
            EvaluateSelf();
        }
    }
}
