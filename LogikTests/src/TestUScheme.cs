using NUnit.Framework;

namespace Logik.Tests.SchemeEvaluator {
    public class TestUScheme : CellTestBase {

        [Test]
        public void EvalBrokenExpressions() {
            WhenFormulaIs(cell, "1");
            ThenCellHasNoError(cell2);
            WhenFormulaIs(cell2, $"{cell.Id})");
            ThenCellHasError(cell2);
        }
    }
}