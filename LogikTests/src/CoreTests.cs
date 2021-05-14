using Logik.Core;
using NUnit.Framework;

namespace Logik.Tests.Core {
    public class TestCell : CellTestBase {

        [Test]
        public void WhenFormulaIsNumericValueThenValueIsNumeric() {
            WhenFormulaIs(cell, NumericValueOne);
            ThenValueIs(cell, NumericValueOne);
        }

        [Test]
        public void CellsWithStringValues() {
            const string a = "\"lolo\"";
            const string b = "\"lili\"";
            const string expected = a + b;

            WhenFormulaIs(cell, a);
            WhenFormulaIs(cell2, b);
            WhenFormulaIs(cell3, $"concat({cell.Name};{cell2.Name})");
            ThenValueIs(cell3, expected);
        }
    }
}