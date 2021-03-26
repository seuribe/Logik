using Logik.Core;
using NUnit.Framework;

namespace Logik.Tests.Core {
    public class TestCell {
        private CellIndex cellIndex = new CellIndex();
        private Cell cell;
        private const string NumericValueOne = "1";
        private const string StringValueHello = "\"Hello!\"";

        [SetUp]
        public void Setup() {
            cellIndex.Clear();
            cell = cellIndex.CreateCell();
        }

        [Test]
        public void WhenFormulaIsNumericValueThenValueIsNumeric() {
            WhenFormulaIs(NumericValueOne);
            ThenValueIs(NumericValueOne);
        }

        [Test]
        public void WhenFormulaIsStringValueThenValueIsStringValue() {
            WhenFormulaIs(StringValueHello);
            ThenValueIs(StringValueHello);
        }

        public void WhenFormulaIs(string formula) {
            cell.Formula = formula;
        }

        public void ThenValueIs(string expected) {
            Assert.AreEqual(cell.Value, expected);
        }

    }
}