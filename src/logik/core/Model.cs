using System;
using System.Linq;
using System.Collections.Generic;
using Logik.Core.Formula;

namespace Logik.Core {
    public delegate EvalNode ValueLookup(string name);
    public delegate float TabularLookup(string name, int row, int column);

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
        private Dictionary<string, TabularCell> tcells = new Dictionary<string, TabularCell>();
                
        public const string DefaultEvaluatorType = "default";
        public string EvaluatorType { get; private set; }

        private EvalNode Lookup(string id) => cells[id].EvalNode;
        private float TabularLookup(string id, int row, int column) => tcells[id][row, column];

        private int lastCellIndex = 1;

        private readonly EvalNodeBuilder nodeBuilder;

        public Model(string evaluatorType = null) {
            EvaluatorType = evaluatorType ?? DefaultEvaluatorType;
            nodeBuilder = new EvalNodeBuilder(Lookup, TabularLookup);
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
            GenerateEvalNode(cell);
            return cell;
        }
        
        public TabularCell CreateTable(string name = null) {
            var tcell = new TabularCell(name ?? GenerateCellName());
            tcells.Add(tcell.Name, tcell);
            return tcell;
        }

        public void DeleteCell(ICell cell) {
            var ncell = cell as NumericCell;

            ncell.FormulaChanged -= CellFormulaChanged;
            ncell.NameChanged -= ChangeCellName;
            ncell.DeleteRequested -= DeleteCell;
            cells.Remove(cell.Name);
            foreach (var other in ncell.references)
                other.referencedBy.Remove(ncell);
            UpdateReferences();
            Evaluate();
        }

        private void CellFormulaChanged(ICell cell) {
            var ncell = cell as NumericCell;
            try {
                cell.ClearError();
                GenerateEvalNode(ncell);
                UpdateReferences(ncell);
                UpdateValue(ncell);
            } catch (CircularReference e) {
                cell.SetError(e.Message);
                ClearReferences(ncell);
            } catch (Exception e) {
                cell.SetError(e.Message);
            }
            StartPropagation(ncell);
        }

        private void GenerateEvalNode(NumericCell cell) {
            cell.EvalNode = nodeBuilder.Build(cell.Formula);
        }

        private void ChangeCellName(ICell cell, string newName) {
            var oldName = cell.Name;

            if (cells.ContainsKey(newName))
                throw new LogikException("Cell with name '" + newName + "' already exists");

            cells[newName] = cells[oldName];
            cells.Remove(oldName);

            if (cell is NumericCell ncell)
                StartPropagation(ncell);
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
                cell.ClearError();
                cell.Value = cell.EvalNode.Eval();
            } catch (Exception e) {
                cell.SetError(e.Message);
            }
        }

        private void StartPropagation(NumericCell cell) {
            Propagate(cell, new ErrorPropagation(cell));
        }

        private void Propagate(NumericCell cell, ErrorPropagation ep) {
            foreach (NumericCell other in cell.referencedBy) {
                if (ep.SetError)
                    other.SetError(ep.ErrorMessage);
                else {
                    UpdateValue(other);
                }

                Propagate(other, ep.Update(other));
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
                cell.SetError("Error(s) in referenced cell(s)");
        }

        private void BuildReferences(NumericCell cell) {
            try {
                var referencedNames = cell.References();
                // TODO: if Lookup returns a cell, then it could be passed as a parameter to obtain cells from names
                var refs = new HashSet<NumericCell>(referencedNames.ConvertAll(name => cells[name]));
                cell.references = new HashSet<NumericCell>(refs);
                foreach (var other in refs)
                    other.referencedBy.Add(cell);
            } catch (Exception e) {
                cell.SetError(e.Message);
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
