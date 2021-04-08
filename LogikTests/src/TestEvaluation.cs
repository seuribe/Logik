using Logik.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logik.Tests.Core {
    public class TestEvaluation : CellTestBase {
        private List<Cell> evaluationOrder;

        [Test]
        public void TestEvaluationOrderStrict() {
            WhenOneCellReferencesAnother(cell2, cell);
            WhenOneCellReferencesAnother(cell3, cell2);
            WhenOneCellReferencesAnother(cell4, cell3);
            WhenBuildEvaluationOrder();
            ThenEvaluationOrderIs(new List<Cell>{cell, cell2, cell3, cell4});
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

        private void ThenCellIsEvaluatedBefore(Cell before, Cell after) {
            Assert.Less(evaluationOrder.IndexOf(before), evaluationOrder.IndexOf(after));
        }

        private void ThenEvaluationOrderIs(IEnumerable<Cell> expected) {
            CollectionAssert.AreEqual(expected, evaluationOrder);
        }

        private void WhenBuildEvaluationOrder() {
            evaluationOrder = model.BuildEvaluationOrder().ToList();
        }
    }
}