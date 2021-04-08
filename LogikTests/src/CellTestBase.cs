using Logik.Core;
using Logik.Core.Formula;
using Logik.Storage;
using NUnit.Framework;
using System;

namespace Logik.Tests {
    public class CellTestBase {
        public const string NumericValueOne = "1";
        public const string StringValueHello = "\"Hello!\"";
        public const string InvalidFormulaString = "blabla";

        protected Model model;
        protected Cell cell;
        protected Cell cell2;
        protected Cell cell3;
        protected Cell cell4;

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
        
        public void WhenOneCellReferencesAnother(Cell cell, Cell referenced) {
            cell.Formula = $"({referenced.Name})";
        }
        
        public void WhenFormulaIs(Cell cell, string formula) {
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
        
        #endregion

        #region Then Methods
        public void ThenCellHasNoError(Cell cell) {
            Assert.IsFalse(cell.Error, "Cell has no error");
        }

        public void ThenCellHasError(Cell cell) {
            Assert.IsTrue(cell.Error);
        }

        public void ThenCellHasNoReferences(Cell cell) {
            CollectionAssert.IsEmpty(cell.references, "Cell does not have references");
        }

        public void ThenFormulaIs(Cell cell, string formula) {
            Assert.AreEqual(formula, cell.Formula);
        }

        public void ThenValueIs(Cell cell, string expected) {
            Assert.AreEqual(expected, cell.Value);
        }

        public void ThenValueIs(Cell cell, float expected) {
            Assert.AreEqual(expected.ToString(), cell.Value);
        }

        public void ThenCellIsReferencingAnother(Cell cell, Cell referenced) {
            Assert.IsTrue(cell.references.Contains(referenced));
        }
        #endregion
    }
}