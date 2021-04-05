using Logik.Core;
using NUnit.Framework;

namespace Logik.Tests.SchemeEvaluator {

    public class TestSchemeEvaluation : CellTestBase {

        protected override IEvaluator GetEvaluator() {
            return new Logik.Core.SchemeEvaluator();
        }

        [Test]
        public void EvalNumericValue() {
            WhenFormulaIs(cell, NumericValueOne);
            ThenValueIs(cell, NumericValueOne);
        }

        [Test]
        public void EvalAdditionFunction() {
            WhenFormulaIs(cell, "(+ 1 2)");
            ThenValueIs(cell, "3");
        }

        [Test]
        public void EvalReferencingValueCell() {
            WhenFormulaIs(cell, "1");
            WhenFormulaIs(cell2, $"(+ ({cell.Name}) 2)");
            ThenValueIs(cell2, "3");
        }
        
        [Test]
        public void EvalReferencingFormulaCell() {
            WhenFormulaIs(cell, "(+ 1 2)");
            WhenFormulaIs(cell2, $"(+ ({cell.Name}) 3)");
            ThenValueIs(cell2, "6");
        }

        [Test]
        public void EvalNestedFormulas() {
            WhenFormulaIs(cell, "(+ (* 3 (abs 4.0)) 2)");
            ThenValueIs(cell, "14");
        }
    }
}