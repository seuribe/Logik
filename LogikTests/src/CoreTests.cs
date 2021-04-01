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
        public void WhenFormulaIsStringValueThenValueIsStringValue() {
            WhenFormulaIs(cell, StringValueHello);
            ThenValueIs(cell, StringValueHello);
        }

    }
}