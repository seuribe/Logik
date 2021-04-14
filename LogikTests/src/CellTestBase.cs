using Logik.Core;
using Logik.Core.Formula;
using NUnit.Framework;

namespace Logik.Tests {
    public class CellTestBase {
        public const string NumericValueOne = "1";
        public const string StringValueHello = "\"Hello!\"";
        public const string InvalidFormulaString = "blabla";

        protected Model model;
        protected NumericCell cell;
        protected NumericCell cell2;
        protected NumericCell cell3;
        protected NumericCell cell4;

        [SetUp]
        public void Setup() {
            model = new Model(GetEvaluator());
            cell = model.CreateCell();
            cell2 = model.CreateCell();
            cell3 = model.CreateCell();
            cell4 = model.CreateCell();
        }

        #region When Methods
        protected virtual IEvaluator GetEvaluator() {
            return new TreeEvaluator();
        }
        
        public void WhenOneCellReferencesAnother(NumericCell cell, NumericCell referenced) {
            cell.Formula = $"({referenced.Name})";
        }
        
        public void WhenFormulaIs(NumericCell cell, string formula) {
            cell.Formula = formula;
        }

        public void WhenModelIsReset() {
            model = new Model(GetEvaluator());
        }
        public void WhenCellsAreRestoredFromModel() {
            cell = model.GetCell(cell.Name);
            cell2 = model.GetCell(cell2.Name);
            cell3 = model.GetCell(cell3.Name);
            cell4 = model.GetCell(cell4.Name);
        }

        protected void CanChangeName(NumericCell cell, string newName) {
            Assert.IsTrue(cell.TryNameChange(newName));
        }
        
        protected void CannotChangeName(NumericCell cell, string newName) {
            Assert.IsFalse(cell.TryNameChange(newName));
        }

        #endregion

        #region Then Methods
        public void ThenCellHasNoError(NumericCell cell) {
            Assert.IsFalse(cell.Error, "Cell has no error");
        }

        public void ThenCellHasError(NumericCell cell) {
            Assert.IsTrue(cell.Error);
        }

        public void ThenCellHasNoReferences(NumericCell cell) {
            CollectionAssert.IsEmpty(cell.references, "Cell does not have references");
        }

        public void ThenFormulaIs(NumericCell cell, string formula) {
            Assert.AreEqual(formula, cell.Formula);
        }

        public void ThenValueIs(NumericCell cell, string expected) {
            Assert.AreEqual(float.Parse(expected), cell.Value);
        }

        public void ThenValueIs(NumericCell cell, float expected) {
            Assert.AreEqual(expected, cell.Value);
        }

        public void ThenCellIsReferencingAnother(NumericCell cell, NumericCell referenced) {
            Assert.IsTrue(cell.references.Contains(referenced));
        }

        protected void ThenNameIs(NumericCell cell, string name) {
            Assert.AreEqual(name, cell.Name);
        }

        protected void ThenNameIsNot(NumericCell cell, string name) {
            Assert.AreNotEqual(name, cell.Name);
        }      
         #endregion
    }
}