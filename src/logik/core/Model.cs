using System;
using System.Linq;
using System.Collections.Generic;
using Logik.Core.Formula;

namespace Logik.Core {

    class ErrorPropagation {
        public bool SetError { get; private set; }
        public readonly string ErrorMessage;

        public ErrorPropagation(NumericCell cell) {
            SetError = cell.Error;
            ErrorMessage = cell.ErrorMessage;
        }

        public ErrorPropagation Update(NumericCell cell) {
            return SetError ? this : new ErrorPropagation(cell);
        }
    }

    public class Model {

        private Dictionary<string, NumericCell> cells = new Dictionary<string, NumericCell>();

        private readonly IEvaluator evaluator;

        private int lastCellIndex = 1;

        public IEvaluator Evaluator { get => evaluator; }

        public Model(IEvaluator evaluator) {
            this.evaluator = evaluator;
        }

        private string GenerateCellName() {
            return "C" + (lastCellIndex++);
        }

        public NumericCell CreateCell(string name = null, string formula = null) {
            var cell = new NumericCell(name ?? GenerateCellName());
            if (formula != null)
                cell.Formula = formula;
            cell.FormulaChanged += CellFormulaChanged;
            cell.NameChanged += ChangeCellName;
            cell.DeleteRequested += DeleteCell;
            cells.Add(cell.Name, cell);
            evaluator.Define(cell);
            return cell;
        }

        public void DeleteCell(NumericCell cell) {
            cell.FormulaChanged -= CellFormulaChanged;
            cell.NameChanged -= ChangeCellName;
            cell.DeleteRequested -= DeleteCell;
            cells.Remove(cell.Name);
            evaluator.Undefine(cell);
            UpdateReferences();
            Evaluate();
        }

        private void CellFormulaChanged(NumericCell cell) {
            try {
                evaluator.Define(cell);
                UpdateReferences(cell);
                UpdateValue(cell);
            } catch (CircularReference e) {
                cell.SetError(ErrorState.CircularReference, e.Message);
                ClearReferences(cell);
            } catch (Exception e) {
                evaluator.Undefine(cell);
                cell.SetError(ErrorState.Definition, e.Message);
            }
            StartPropagation(cell);
        }

        private void ChangeCellName(NumericCell cell, string newName) {
            var oldName = cell.Name;

            if (cells.ContainsKey(newName))
                throw new LogikException("Cell with name '" + newName + "' already exists");

            evaluator.Rename(cell, newName);
            cells.Remove(oldName);
            cells[newName] = cell;
            StartPropagation(cell);
        }

        private void ClearReferences(NumericCell cell) {
            foreach (var other in cell.references)
                other.referencedBy.Remove(cell);

            cell.references.Clear();
            cell.deepReferences.Clear();
        }
        
        public void Evaluate() {
            var toEvaluate = BuildEvaluationOrder();
            foreach (var cell in toEvaluate) {
                UpdateValue(cell);
            }
        }

        public IEnumerable<NumericCell> BuildEvaluationOrder() {
            var toEvaluate = new List<NumericCell>();
            var allCells = new List<NumericCell>(cells.Values);
            var references = new Dictionary<NumericCell, HashSet<NumericCell>>();
            allCells.ForEach( cell => references[cell] = new HashSet<NumericCell>(cell.references));

            var toRemove = new HashSet<NumericCell>();
            while (allCells.Count > 0) {
                foreach (var cell in allCells) {
                    if (references[cell].Count == 0) {
                        toEvaluate.Add(cell);
                        toRemove.Add(cell);
                        foreach (var other in cell.referencedBy)
                            references[other].Remove(cell);
                    }
                }
                allCells.RemoveAll(cell => toRemove.Contains(cell));
                toRemove.Clear();
            }

            return toEvaluate;
        }

        private void UpdateValue(NumericCell cell) {
            try {
                cell.Value = evaluator.Evaluate(cell);
                cell.ClearError();
            } catch (Exception e) {
                cell.SetError(ErrorState.Evaluation, e.Message);
            }
        }

        private void StartPropagation(NumericCell cell) {
            Propagate(cell, new ErrorPropagation(cell));
        }

        private void Propagate(NumericCell cell, ErrorPropagation ep) {
            foreach (NumericCell other in cell.referencedBy) {
                if (ep.SetError)
                    other.SetError(ErrorState.Carried, ep.ErrorMessage);
                else
                    UpdateValue(other);

                Propagate(other, ep.Update(cell));
            }
        }

        public void UpdateReferences() {
            foreach (var cell in cells.Values)
                UpdateReferences(cell);
        }

        private void UpdateReferences(NumericCell cell) {
            BuildReferences(cell);
            CheckSelfReference(cell);

            BuildDeepReferences(cell);
            CheckCircularReference(cell);

            CheckCarriedErrors(cell);
        }

        private void CheckCarriedErrors(NumericCell cell) {
            if (cell.deepReferences.Any(c => c.Error))
                cell.SetError(ErrorState.Carried, "Error(s) in referenced cell(s)");
        }

        private void BuildReferences(NumericCell cell) {
            try {
                var refs = new HashSet<NumericCell>(evaluator.References(cell).ConvertAll((name) => cells[name]));
                cell.references = new HashSet<NumericCell>(refs);
                foreach (var other in refs)
                    other.referencedBy.Add(cell);
            } catch (Exception e) {
                cell.SetError(ErrorState.Definition, e.Message);
                ClearReferences(cell);
            }
        }

        private void BuildDeepReferences(NumericCell cell) {
            cell.deepReferences = new HashSet<NumericCell>(cell.references);
            foreach (var other in cell.references) {
                cell.deepReferences.UnionWith(other.deepReferences);
            }
        }

        private void CheckCircularReference(NumericCell cell) {
            if (cell.deepReferences.Contains(cell))
                throw new CircularReference("Circular reference found including Cell " + cell.Name);
        }

        private void CheckSelfReference(NumericCell cell) {
            if (cell.references.Contains(cell))
                throw new CircularReference("Self reference in Cell " + cell.Name);
        }
        public NumericCell GetCell(string name) {
            return cells[name];
        }

        public IEnumerable<NumericCell> GetCells() {
            return cells.Values;
        }
    }
}
