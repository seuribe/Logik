using System;
using System.Linq;
using System.Collections.Generic;
using Logik.Core.Formula;

namespace Logik.Core {


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
                references[cell].Clear();
                deepReferences[cell].Clear();
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
            BuildReferences(cell);
            CheckSelfReference(cell);

            BuildDeepReferences(cell);
            CheckCircularReference(cell);

            CheckCarriedErrors(cell);
        }

        private void CheckCarriedErrors(Cell cell) {
            if (deepReferences[cell].Any(c => c.Error))
                cell.SetError(ErrorState.Carried, "Error(s) in referenced cell(s)");
        }

        private void BuildReferences(Cell cell) {
            var refs = new HashSet<Cell>(evaluator.References(cell).ConvertAll((name) => GetCell(name)));
            references[cell] = new HashSet<Cell>(refs);
        }

        private void BuildDeepReferences(Cell cell) {
            deepReferences[cell] = new HashSet<Cell>(references[cell]);
            foreach (var other in references[cell]) {
                deepReferences[cell].UnionWith(deepReferences[other]);
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
                if (otherKV.Value.Contains(cell) && otherKV.Key != cell) {
                    yield return otherKV.Key;
                }
            }
        }

        public Cell GetCell(string id) {
            return cells[id];
        }
    }
}
