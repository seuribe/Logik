using System;
using System.Linq;
using System.Collections.Generic;
using Logik.Core.Formula;

namespace Logik.Core {
    // Delegate for lookup function for obtaining the value of a cell while calculating the result of a formula
    public delegate EvalNode ValueLookup(string name);
    // Delegate for lookup function for obtaining a value inside a table
    public delegate Value TabularLookup(string name, int row, int column);

    /// <summary>
    ///      Helper class for carrying over the state error when propagating the results of a change in a cell.
    /// Once a cell with an error is reached, the state of an instance of ErrorPropagation will be set to
    /// error, and cannot be cleared back to no error. This is in line with the fact that once a cell has an
    /// error, all other cells depending on that, either directly or indirectly, will also be in error state.
    /// </summary>
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

    /// <summary>
    /// Model is the main class to hold the representation of a problem that wants to be solved. It is the topmost
    /// unit and contains all cells and additional information.
    /// 
    /// A cell does not do much computing on its own, and is barely more than a container for information. The
    /// model is in charge of making sure that all cell's values are updated. This keeps the algorithms simpler
    /// and easier to maintain, as they are not distributed among several cells.
    /// </summary>
    public class Model {
        private readonly Dictionary<string, BaseCell> cells = new Dictionary<string, BaseCell>();
        // Remmanent from earlier versions, where it was possible to use different types of
        // formula evaluators.
        public const string DefaultEvaluatorType = "default";
        public string EvaluatorType { get; private set; }

        private EvalNode Lookup(string id) => (cells[id] as NumericCell).EvalNode;
        private Value TabularLookup(string id, int row, int column) => (cells[id] as TabularCell)[row, column];

        private int lastCellIndex = 1;
        public string NextCellName {
            get {
                string next;
                // This makes sure that cell names are not repeated
                do {
                    next = "C" + (lastCellIndex++);
                } while (cells.ContainsKey(next));
                return next;
            }
        }

        private readonly EvalContext context;

        public Model(string evaluatorType = null) {
            EvaluatorType = evaluatorType ?? DefaultEvaluatorType;
            context = new EvalContext(Lookup, TabularLookup);
        }

        /// <summary>
        /// Creates a cell *within* the model. Can optionally receive a formula.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        public NumericCell CreateCell(string name = null, string formula = null) {
            var cell = new NumericCell(name ?? NextCellName);
            cells.Add(cell.Name, cell);

            cell.Formula = formula ?? "0";
            GenerateEvalNode(cell);

            AddListeners(cell);

            return cell;
        }

        /// <summary>
        /// Creates a table *within* the model. Can optionally receive the data for its cells.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public TabularCell CreateTable(string name = null, int rows = 1, int columns = 1, IEnumerable<GridCellData> data = null) {
            var tcell = new TabularCell(name ?? NextCellName, rows, columns);
            if (data != null)
                tcell.SetData(data);

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

        /// <summary>
        /// This is called whenever the internal content of a cell changed because of a user action
        /// (e.g. entering a new value). It will recalculate the output value and propagate the
        /// calculation through other cells
        /// </summary>
        /// <param name="cell"></param>
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
            cell.PrepareValueCalculation();
        }

        /// <summary>
        /// This method should throw an exception if it is not possible to rename the cell
        /// (i.e., if another cell with the same name exists, or if it is an invalid name)
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="newName"></param>
        private void ChangeCellName(ICell cell, string newName) {
            CheckValidName(newName);
            CheckNameUnique(newName);

            DoRename(cell.Name, newName);

            StartPropagation(cell);
        }

        private void CheckValidName(string name) {
            if (float.TryParse(name, out _))
                throw new LogikException($"Invalid name '{name}' for cell");
        }

        private void CheckNameUnique(string name) {
            if (cells.ContainsKey(name))
                throw new LogikException($"Cell with name '{name}' already exists");
        }

        private void DoRename(string oldName, string newName) {
            cells[newName] = cells[oldName];
            cells.Remove(oldName);
        }

        /// <summary>
        /// Removes all references from or to this cell.
        /// </summary>
        /// <param name="cell"></param>
        private void ClearReferences(ICell cell) {
            foreach (var other in cell.References) {
                other.ReferencedBy.Remove(cell);
            }

            cell.References.Clear();
            cell.DeepReferences.Clear();
        }
        
        /// <summary>
        /// Re-evaluates the whole model
        /// </summary>
        public void Evaluate() {
            var toEvaluate = BuildEvaluationOrder();
            foreach (var cell in toEvaluate) {
                UpdateValue(cell);
            }
        }

        /// <summary>
        /// Creates an optimal evaluation order for cells, so that no cell needs to be evaluated twice
        /// because of references among them
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Attempts to re-evaluate the value of a cell from its formula/references. Clears existing
        /// error state if successful.
        /// </summary>
        /// <param name="cell"></param>
        private void UpdateValue(ICell cell) {
            try {
                cell.ClearError();
                cell.InternalUpdateValue(context);
            } catch (Exception e) {
                cell.SetError(e.Message);
            }
        }

        /// <summary>
        /// Begins the propagation of a cell's result
        /// </summary>
        /// <param name="cell"></param>
        private void StartPropagation(ICell cell) {
            Propagate(cell, new ErrorPropagation(cell));
        }

        /// <summary>
        /// Helper function, recursively called to propagate the new values of cells
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="ep"></param>
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

        /// <summary>
        /// Re-builds the references in all cells
        /// </summary>
        public void UpdateReferences() {
            foreach (var cell in cells.Values)
                UpdateReferences(cell);
        }

        /// <summary>
        /// Re-builds the dependencies from and to a cell and checks for referencing errors
        /// (e.g. circular references)
        /// </summary>
        /// <param name="cell"></param>
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

        /// <summary>
        /// Builds the list of cells referenced/referencing a cell from its formula
        /// </summary>
        /// <param name="cell"></param>
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

        /// <summary>
        /// Builds the deep references, that is, all cells that are recursively referenced by this
        /// </summary>
        /// <param name="cell"></param>
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

        public IEnumerable<ICell> GetCells() {
            return cells.Values;
        }
    }
}
