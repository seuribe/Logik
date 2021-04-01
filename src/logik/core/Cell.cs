
using System;
using System.Collections.Generic;

namespace Logik.Core {
/*
    public class Cell {

        private static readonly string DefaultCellFormula = "0";

        private string formula;
        private readonly Evaluator evaluator;
        private readonly Model cellIndex;

        public bool Error { get; private set; } = false;

        public string Id { get; private set; }
        public string Value { get; private set; }

        private void NotifyObservers() {
            ValueChanged?.Invoke(this);
        }

        public delegate void CellEvent(Cell cell);
        public event CellEvent ValueChanged;

        public Cell(string id, Model cellIndex, Evaluator evaluator) {
            Id = id;
            this.cellIndex = cellIndex;
            this.evaluator = evaluator;
            this.Formula = DefaultCellFormula;
        }

        public string Formula {
            get => formula;
            set {
                formula = value;
                cellIndex.CellFormulaChanged(this);
                EvaluateSelf();
            }
        }

        public void SetError(string message) {
            Error = true;
            Value = message;
        }

        private void EvaluateSelf() {
            try {
                evaluator.DefineCell(this, formula);
                UpdateReferences();
                CheckForCircularity();
                Value = evaluator.EvaluateCell(this);
                Error = false;
            } catch (Exception e) {
                Value = "Error: " + e.Message;
                Error = true;
            }
            NotifyObservers();
        }

        internal void Observe(Cell cell) {
            if (cell == this)
                throw new Exception("Self-reference in cell " + cell.Id);
            
            cell.ValueChanged += ReferencedCellChanged;
        }

        private void CheckForCircularity() {
            HashSet<Cell> cells = new HashSet<Cell>(referenced);

            while (cells.Count > 0) {
                Cell cell = cells.G;
                cells.RemoveAt(0);
                if (cell == this) {
//                    ValueChanged = null;
                    referenced.Clear();
                    throw new Exception ("Circular reference found in " + this.Id);
                }

                cells.Add
                foreach (var referenced in cell.Referenced)
                    cells.Push(referenced);
            }
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
            if (other.Error) {
                CarryError(other);
            } else {
                EvaluateSelf();
            }
        }

        private void CarryError(Cell other) {
            this.Error = other.Error;
            this.Value = other.Value;
            NotifyObservers();
        }
    }
*/
}
