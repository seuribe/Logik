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

        public ErrorPropagation(ICell cell) {
            SetError = cell.Error;
            ErrorMessage = cell.ErrorMessage;
        }

        public ErrorPropagation Update(ICell cell) {
            return SetError ? this : new ErrorPropagation(cell);
        }
    }

    public class Model {

        private readonly Dictionary<string, BaseCell> cells = new Dictionary<string, BaseCell>();
                
        public const string DefaultEvaluatorType = "default";
        public string EvaluatorType { get; private set; }

        private EvalNode Lookup(string id) => (cells[id] as NumericCell).EvalNode;
        private float TabularLookup(string id, int row, int column) => (cells[id] as TabularCell)[row, column];

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
            cells.Add(cell.Name, cell);

            cell.Formula = formula ?? "0";
            GenerateEvalNode(cell);

            AddListeners(cell);

            return cell;
        }
        
        public TabularCell CreateTable(string name = null) {
            var tcell = new TabularCell(name ?? GenerateCellName());
            cells.Add(tcell.Name, tcell);

            AddListeners(tcell);

            return tcell;
        }

        private void AddListeners(ICell cell) {
            cell.ContentChanged += CellContentChanged;
            cell.NameChanged += ChangeCellName;
            cell.DeleteRequested += DeleteCell;
        }

        public void DeleteCell(ICell cell) {
            RemoveListeners(cell);
            cells.Remove(cell.Name);
            foreach (var other in cell.References)
                other.ReferencedBy.Remove(cell);

            UpdateReferences();
            Evaluate();
        }

        private void RemoveListeners(ICell cell) {
            cell.NameChanged -= ChangeCellName;
            cell.DeleteRequested -= DeleteCell;
            cell.ContentChanged -= CellContentChanged;
        }

        private void CellContentChanged(ICell cell) {
            try {
                cell.ClearError();
                GenerateEvalNode(cell);
                UpdateReferences(cell);
                UpdateValue(cell);
            } catch (CircularReference e) {
                cell.SetError(e.Message);
                ClearReferences(cell);
            } catch (Exception e) {
                cell.SetError(e.Message);
            }
            StartPropagation(cell);
        }

        private void GenerateEvalNode(ICell cell) {
            cell.PrepareValueCalculation(nodeBuilder);
        }

        private void ChangeCellName(ICell cell, string newName) {
            var oldName = cell.Name;

            if (NameExists(newName))
                throw new LogikException("Cell with name '" + newName + "' already exists");

            cells[newName] = cells[oldName];
            cells.Remove(oldName);

            StartPropagation(cell);
        }

        private bool NameExists(string name) => cells.ContainsKey(name);

        private void ClearReferences(ICell cell) {
            foreach (var other in cell.References) {
                other.ReferencedBy.Remove(cell);
            }

            cell.References.Clear();
            cell.DeepReferences.Clear();
        }
        
        public void Evaluate() {
            var toEvaluate = BuildEvaluationOrder();
            foreach (var cell in toEvaluate) {
                UpdateValue(cell);
            }
        }

        public IEnumerable<ICell> BuildEvaluationOrder() {
            var toEvaluate = new List<ICell>();
            var allCells = new List<ICell>(cells.Values);
            var references = new Dictionary<ICell, HashSet<ICell>>();
            allCells.ForEach( cell => references[cell] = new HashSet<ICell>(cell.References));

            var toRemove = new HashSet<ICell>();
            while (allCells.Count > 0) {
                foreach (var cell in allCells) {
                    if (references[cell].Count == 0) {
                        toEvaluate.Add(cell);
                        toRemove.Add(cell);
                        foreach (var other in cell.ReferencedBy)
                            references[other].Remove(cell);
                    }
                }
                allCells.RemoveAll(cell => toRemove.Contains(cell));
                toRemove.Clear();
            }

            return toEvaluate;
        }

        private void UpdateValue(ICell cell) {
            try {
                cell.ClearError();
                cell.InternalUpdateValue();
            } catch (Exception e) {
                cell.SetError(e.Message);
            }
        }

        private void StartPropagation(ICell cell) {
            Propagate(cell, new ErrorPropagation(cell));
        }

        private void Propagate(ICell cell, ErrorPropagation ep) {
            foreach (ICell other in cell.ReferencedBy) {
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

        private void UpdateReferences(ICell cell) {
            BuildReferences(cell);
            CheckSelfReference(cell);

            BuildDeepReferences(cell);
            CheckCircularReference(cell);

            CheckCarriedErrors(cell);
        }

        private void CheckCarriedErrors(ICell cell) {
            if (cell.DeepReferences.Any(c => c.Error))
                cell.SetError("Error(s) in referenced cell(s)");
        }

        private void BuildReferences(ICell cell) {
            try {
                var referencedNames = cell.GetNamesReferencedInContent().ToList();
                cell.References = new HashSet<ICell>(referencedNames.ConvertAll(GetCell));
                foreach (var other in cell.References)
                    other.ReferencedBy.Add(cell);

            } catch (Exception e) {
                cell.SetError(e.Message);
                ClearReferences(cell);
            }
        }

        private void BuildDeepReferences(ICell cell) {
            cell.DeepReferences = new HashSet<ICell>(cell.References);
            foreach (var other in cell.References) {
                cell.DeepReferences.UnionWith(other.DeepReferences);
            }
        }

        private void CheckCircularReference(ICell cell) {
            if (cell.DeepReferences.Contains(cell))
                throw new CircularReference("Circular reference found including Cell " + cell.Name);
        }

        private void CheckSelfReference(ICell cell) {
            if (cell.References.Contains(cell))
                throw new CircularReference("Self reference in Cell " + cell.Name);
        }
        public ICell GetCell(string name) {
            if (cells.TryGetValue(name, out BaseCell cell))
                return cell;

            throw new LogikException($"Cell {name} does not exist");
        }

        public IEnumerable<NumericCell> GetCells() {
            return cells.Values.Where( cell => cell is NumericCell).Select( cell => cell as NumericCell);
        }
    }
}
