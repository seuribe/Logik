using Logik.Core;
using Logik.Core.Formula;
using NUnit.Framework;

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

        protected virtual IEvaluator GetEvaluator() {
            return new TreeEvaluator();
        }
        
        public void ThenCellHasNoError(Cell cell) {
            Assert.IsFalse(cell.Error, "Cell has no error");
        }

        public void ThenCellHasError(Cell cell) {
            Assert.IsTrue(cell.Error);
        }

        public void ThenCellHasNoReferences(Cell cell) {
            CollectionAssert.IsEmpty(model.GetReferences(cell), "Cell does not have references");
        }

        public void WhenOneCellReferencesAnother(Cell cell, Cell referenced) {
            cell.Formula = $"({referenced.Id})";
        }
        
        public void WhenFormulaIs(Cell cell, string formula) {
            cell.Formula = formula;
        }

        public void ThenValueIs(Cell cell, string expected) {
            Assert.AreEqual(expected, cell.Value);
        }

        public void ThenCellIsReferencingAnother(Cell cell, Cell referenced) {
            Assert.IsTrue(model.GetReferences(cell).Contains(referenced));
        }
    }
}