using System;
using System.Linq;
using System.Collections.Generic;
using Logik.Core.Formula;

namespace Logik.Core {

    public class LogikException : Exception {
        public LogikException(string message) : base(message) { }
    }

    public class CircularReference : LogikException {
        public CircularReference(string message) : base(message) { }
    }

    public delegate void CellEvent(Cell cell);

    public enum ErrorState {
        None = 0,
        Definition,
        Evaluation,
        CircularReference,
        Carried
    }

    public class Cell {
        private static readonly string DefaultCellFormula = "0";

        public string Id { get; }

        private string formula = DefaultCellFormula;
        public string Formula {
            get => formula;
            set {
                formula = value;
                FormulaChanged?.Invoke(this);
            }
        }

        private string value = "0";
        public string Value {
            get => value;
            set {
                this.value = value;
                ValueChanged?.Invoke(this);
            }
        }

        public bool Error { get => ErrorState != ErrorState.None; }

        public ErrorState ErrorState { get; private set; }

        public event CellEvent ValueChanged;
        public event CellEvent FormulaChanged;

        public void SetError(ErrorState newState, string message) {
            ErrorState = newState;
            Value = message;
        }

        public void ClearError() {
            ErrorState = ErrorState.None;
        }

        public Cell(string id) {
            Id = id;
        }
    }

    public class Model {

        private Dictionary<string, Cell> cells = new Dictionary<string, Cell>();
        private Dictionary<Cell, HashSet<Cell>> references = new Dictionary<Cell, HashSet<Cell>>();
        private Dictionary<Cell, HashSet<Cell>> deepReferences = new Dictionary<Cell, HashSet<Cell>>();

        private readonly IEvaluator evaluator;

        private int lastCellIndex = 1;

        public Model(IEvaluator evaluator) {
            this.evaluator = evaluator;
        }

        private string GenerateCellName() {
            return "C" + (lastCellIndex++);
        }

        public Cell CreateCell() {
            var cell = new Cell(GenerateCellName());
            cell.FormulaChanged += CellFormulaChanged;
            cells.Add(cell.Id, cell);
            evaluator.Define(cell);
            references[cell] = new HashSet<Cell>();
            deepReferences[cell] = new HashSet<Cell>();
            return cell;
        }

        public List<Cell> GetReferences(Cell cell) {
            return new List<Cell>(references[cell]);
        }

        public void CellFormulaChanged(Cell cell) {
            try {
                evaluator.Define(cell);
                UpdateReferences(cell);
                cell.ClearError();
                UpdateValue(cell);
            } catch (CircularReference e) {
                cell.SetError(ErrorState.CircularReference, e.Message);
            } catch (Exception e) {
                evaluator.Undefine(cell);
                cell.SetError(ErrorState.Definition, e.Message);
            }
            Propagate(cell);
        }

        private void UpdateValue(Cell cell) {
            try {
                cell.Value = evaluator.Evaluate(cell);
                cell.ClearError();
            } catch (Exception e) {
                cell.SetError(ErrorState.Evaluation, e.Message);
            }

        }

        private void Propagate(Cell cell) {
            try {
                if (!cell.Error)
                    cell.Value = evaluator.Evaluate(cell);
            } catch (Exception e) {
                cell.SetError(ErrorState.Evaluation, e.Message);
            }

            foreach (Cell other in GetCellsReferencing(cell)) {
                if (cell.Error) {
                    other.SetError(ErrorState.Carried, cell.Value);
                } else {
                    if (other.ErrorState == ErrorState.Carried)
                        other.ClearError();

                    UpdateValue(other);
                }
                Propagate(other);
            }
        }

        public void UpdateReferences(Cell cell) {
            var refs = new HashSet<Cell>(evaluator.References(cell).ConvertAll((name) => GetCell(name)));
            references[cell] = new HashSet<Cell>(refs);

            CheckSelfReference(cell);

            deepReferences[cell] = new HashSet<Cell>(refs);
            foreach (var other in refs) {
                deepReferences[cell].UnionWith(deepReferences[other]);
            }

            CheckCircularReference(cell);

            var carriedErrors = deepReferences[cell].Select(c => c.Error);
            if (carriedErrors.Count() > 0) {
                var errorMessage = string.Join(", ", carriedErrors.ToArray());
                cell.SetError(ErrorState.Carried, "Error(s) in referenced cell(s) " + errorMessage);
            }
        }

        private void CheckCircularReference(Cell cell) {
            if (deepReferences[cell].Contains(cell))
                throw new CircularReference("Circular reference found including Cell " + cell.Id);
        }

        private void CheckSelfReference(Cell cell) {
            if (references[cell].Contains(cell))
                throw new CircularReference("Self reference in Cell " + cell.Id);
        }

        public IEnumerable<Cell> GetCellsReferencing(Cell cell) {
            foreach (var otherKV in references) {
                if (otherKV.Value.Contains(cell)) {
                    yield return otherKV.Key;
                }
            }
        }

        public Cell GetCell(string id) {
            return cells[id];
        }
    }
}
