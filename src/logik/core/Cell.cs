
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

        public string Id { get; private set; }
        public string Value {
            get => value;
            private set {
                this.value = value;
                NotifyObservers();
            }
        }

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
                evaluator.DefineCell(this, formula);
                UpdateReferences();
                UpdateValue();
            }
        }

        internal void Observe(Cell cell) {
            cell.ValueChanged += CellChanged;
        }

        internal void Ignore(Cell cell) {
            cell.ValueChanged -= CellChanged;
        }

        private void UpdateValue() {
            Value = evaluator.EvaluateCell(this);
        }

        private void UpdateReferences() {
            foreach (var other in referenced) {
                other.ValueChanged -= CellChanged;
            }
            referenced = evaluator.GetReferencedCells(this).ConvertAll( (name) => cellIndex.GetCell(name) );
            foreach (var other in referenced) {
                other.ValueChanged += CellChanged;
            }
        }

        private void CellChanged(Cell other) {
            UpdateValue();
        }
    }
}
