using Logik.Core;
using Logik.Core.Formula;
using NUnit.Framework;

namespace Logik.Tests {
    public class CellTestBase {
        public const string NumericValueOne = "1";
        public const string StringValueHello = "\"Hello!\"";
        public const string InvalidFormulaString = "blabla";

        protected Model model;
        protected FormulaCell cell;
        protected FormulaCell cell2;
        protected FormulaCell cell3;
        protected FormulaCell cell4;
        protected TabularCell tcell;

        [SetUp]
        public void Setup() {
            WhenModelIsReset();
        }

        #region When Methods
        public void WhenOneCellReferencesAnother(FormulaCell cell, ICell referenced) {
            cell.Formula = $"({referenced.Name})";
        }
        
        public void WhenFormulaIs(FormulaCell cell, string formula) {
            cell.Formula = formula;
        }

        public void WhenModelIsReset() {
            model = new Model();
            cell = model.CreateCell();
            cell2 = model.CreateCell();
            cell3 = model.CreateCell();
            cell4 = model.CreateCell();
            tcell = model.CreateTable();
        }

        public void WhenCellsAreRestoredFromModel() {
            cell = model.GetCell(cell.Name) as FormulaCell;
            cell2 = model.GetCell(cell2.Name) as FormulaCell;
            cell3 = model.GetCell(cell3.Name) as FormulaCell;
            cell4 = model.GetCell(cell4.Name) as FormulaCell;
            tcell = model.GetCell(tcell.Name) as TabularCell;
        }

        protected void CanChangeName(BaseCell cell, string newName) {
            Assert.IsTrue(cell.TryNameChange(newName));
        }
        
        protected void CannotChangeName(BaseCell cell, string newName) {
            Assert.IsFalse(cell.TryNameChange(newName));
        }

        protected void WhenValueIs(TabularCell tcell, int row, int column, float value) {
            tcell[row, column] = value;
        }

        protected void WhenResized(TabularCell tcell, int newRows, int newColumns) {
            tcell.Resize(newRows, newColumns);
        }
        
        protected void WhenOneCellIsRemoved(ICell cell) {
            model.DeleteCell(cell);
        }
        #endregion

        #region Then Methods
        public void ThenCellHasNoError(ICell cell) {
            Assert.IsFalse(cell.Error, "Cell has no error");
        }

        public void ThenCellHasError(ICell cell) {
            Assert.IsTrue(cell.Error);
        }

        public void ThenCellHasNoReferences(ICell cell) {
            CollectionAssert.IsEmpty(cell.References, "Cell does not have references");
        }

        public void ThenFormulaIs(FormulaCell cell, string formula) {
            Assert.AreEqual(formula, cell.Formula);
        }

        public void ThenValueIs(FormulaCell cell, string expected) {
            Assert.AreEqual(expected,(string) cell.Value);
        }

        public void ThenValueIs(FormulaCell cell, float expected) {
            Assert.AreEqual(expected, (float)cell.Value);
        }

        public void ThenCellIsReferencingAnother(ICell cell, ICell referenced) {
            Assert.IsTrue(cell.References.Contains(referenced));
        }

        public void ThenCellIsReferencedBy(ICell cell, ICell referencedBy) {
            Assert.IsTrue(cell.ReferencedBy.Contains(referencedBy));
        }
        public void ThenCellIsReferencedByNone(ICell cell) {
            Assert.AreEqual(0, cell.ReferencedBy.Count);
        }

        protected void ThenNameIs(ICell cell, string name) {
            Assert.AreEqual(name, cell.Name);
        }

        protected void ThenNameIsNot(ICell cell, string name) {
            Assert.AreNotEqual(name, cell.Name);
        }      
        protected void ThenValueIs(TabularCell tcell, int row, int col, float value) {
            Assert.AreEqual(value, tcell[row, col].AsFloat);
        }

        protected void ThenSizeIs(TabularCell tcell, int rows, int columns) {
            Assert.AreEqual(rows, tcell.Rows);
            Assert.AreEqual(columns, tcell.Columns);
        }

        protected void ThenColumnCountIs(TabularCell tcell, int c) {
            Assert.AreEqual(c, tcell.Columns);
        }

        protected void ThenRowCountIs(TabularCell tcell, int r) {
            Assert.AreEqual(r, tcell.Rows);
        }
 
        #endregion
    }
}