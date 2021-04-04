using Logik.Core;
using NUnit.Framework;

namespace Logik.Tests.Core {
    public class TestCell : CellTestBase {

        [Test]
        public void WhenFormulaIsNumericValueThenValueIsNumeric() {
            WhenFormulaIs(cell, NumericValueOne);
            ThenValueIs(cell, NumericValueOne);
        }

    }
}