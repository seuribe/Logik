using System;
using System.Linq;
using System.Collections.Generic;
using Logik.Core.Formula;

namespace Logik.Core {

    public class Model {

        private Dictionary<string, Cell> cells = new Dictionary<string, Cell>();

        private readonly IEvaluator evaluator;

        private int lastCellIndex = 1;

        public IEvaluator Evaluator { get => evaluator; }

        public Model(IEvaluator evaluator) {
            this.evaluator = evaluator;
        }

        private string GenerateCellName() {
            return "C" + (lastCellIndex++);
        }

        public Cell CreateCell(string name = null, string formula = null) {
            var cell = new Cell(name ?? GenerateCellName());
            if (formula != null)
                cell.Formula = formula;
            cell.FormulaChanged += CellFormulaChanged;
            cell.NameChanged += ChangeCellName;
            cells.Add(cell.Name, cell);
            evaluator.Define(cell);
            return cell;
        }

        private void CellFormulaChanged(Cell cell) {
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
            Propagate(cell);
        }

        private void ChangeCellName(Cell cell, string newName) {
            var oldName = cell.Name;

            if (cells.ContainsKey(newName))
                throw new LogikException("Cell with name '" + newName + "' already exists");

            evaluator.Rename(cell, newName);
            cells.Remove(oldName);
            cells[newName] = cell;
            Propagate(cell);
        }

        private void ClearReferences(Cell cell) {
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

        public IEnumerable<Cell> BuildEvaluationOrder() {
            var toEvaluate = new List<Cell>();
            var allCells = new List<Cell>(cells.Values);
            var references = new Dictionary<Cell, HashSet<Cell>>();
            allCells.ForEach( cell => references[cell] = new HashSet<Cell>(cell.references));

            var toRemove = new HashSet<Cell>();
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

            foreach (Cell other in cell.referencedBy) {
                if (cell.Error) {
                    other.SetError(ErrorState.Carried, cell.Value);
                } else {
                    UpdateValue(other);
                }
                Propagate(other);
            }
        }

        public void UpdateReferences() {
            foreach (var cell in BuildEvaluationOrder())
                UpdateReferences(cell);
        }

        private void UpdateReferences(Cell cell) {
            BuildReferences(cell);
            CheckSelfReference(cell);

            BuildDeepReferences(cell);
            CheckCircularReference(cell);

            CheckCarriedErrors(cell);
        }

        private void CheckCarriedErrors(Cell cell) {
            if (cell.deepReferences.Any(c => c.Error))
                cell.SetError(ErrorState.Carried, "Error(s) in referenced cell(s)");
        }

        private void BuildReferences(Cell cell) {
            var refs = new HashSet<Cell>(evaluator.References(cell).ConvertAll((name) => cells[name]));
            cell.references = new HashSet<Cell>(refs);
            foreach (var other in refs)
                other.referencedBy.Add(cell);
        }

        private void BuildDeepReferences(Cell cell) {
            cell.deepReferences = new HashSet<Cell>(cell.references);
            foreach (var other in cell.references) {
                cell.deepReferences.UnionWith(other.deepReferences);
            }
        }

        private void CheckCircularReference(Cell cell) {
            if (cell.deepReferences.Contains(cell))
                throw new CircularReference("Circular reference found including Cell " + cell.Name);
        }

        private void CheckSelfReference(Cell cell) {
            if (cell.references.Contains(cell))
                throw new CircularReference("Self reference in Cell " + cell.Name);
        }
        public Cell GetCell(string name) {
            return cells[name];
        }

        public IEnumerable<Cell> GetCells() {
            return cells.Values;
        }
    }
}
