using Logik.Core;
using NUnit.Framework;
using System;

namespace Logik.Tests.Core {

    public class TestSchemeEvaluation : CellTestBase {

        protected override IEvaluator GetEvaluator() {
            return new SchemeEvaluator();
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
            WhenFormulaIs(cell2, $"(+ ({cell.Id}) 2)");
            ThenValueIs(cell2, "3");
        }
        
        [Test]
        public void EvalReferencingFormulaCell() {
            WhenFormulaIs(cell, "(+ 1 2)");
            WhenFormulaIs(cell2, $"(+ ({cell.Id}) 3)");
            ThenValueIs(cell2, "6");
        }

        [Test]
        public void EvalNestedFormulas() {
            WhenFormulaIs(cell, "(+ (* 3 (abs 4.0)) 2)");
            ThenValueIs(cell, "14");
        }
    }
}