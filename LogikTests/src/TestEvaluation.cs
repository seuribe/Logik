using Logik.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logik.Tests.Core {
    public class TestEvaluation : CellTestBase {
        private List<ICell> evaluationOrder;

        [Test]
        public void TestEvaluationOrderStrict() {
            WhenOneCellReferencesAnother(cell2, cell);
            WhenOneCellReferencesAnother(cell3, cell2);
            WhenOneCellReferencesAnother(cell4, cell3);
            WhenBuildEvaluationOrder();
            ThenEvaluationOrderIs(new List<NumericCell>{cell, cell2, cell3, cell4});
        }

        [Test]
        public void TestEvaluationOrderBeforeAfter() {
            WhenOneCellReferencesAnother(cell2, cell);
            WhenOneCellReferencesAnother(cell3, cell2);
            WhenOneCellReferencesAnother(cell3, cell4);
            WhenBuildEvaluationOrder();
            ThenCellIsEvaluatedBefore(cell4, cell3);
            ThenCellIsEvaluatedBefore(cell, cell3);
            ThenCellIsEvaluatedBefore(cell2, cell3);
            ThenCellIsEvaluatedBefore(cell, cell2);
        }

        [Test]
        public void EvaluateFunctionWithReference() {
            WhenFormulaIs(cell, "11");
            WhenFormulaIs(cell2, "max(" + cell.Name + "; 8)");
            ThenValueIs(cell, 11);

            WhenFormulaIs(cell3, "max(" + cell4.Name + "; 12)");
            WhenFormulaIs(cell4, "1");
            ThenValueIs(cell3, 12);

        }

        private void ThenCellIsEvaluatedBefore(NumericCell before, NumericCell after) {
            Assert.Less(evaluationOrder.IndexOf(before), evaluationOrder.IndexOf(after));
        }

        private void ThenEvaluationOrderIs(IEnumerable<NumericCell> expected) {
            CollectionAssert.AreEqual(expected, evaluationOrder);
        }

        private void WhenBuildEvaluationOrder() {
            evaluationOrder = model.BuildEvaluationOrder().ToList();
        }
    }
}