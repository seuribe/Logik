using NUnit.Framework;
using Logik.Core;
using System;

namespace Logik.Tests.Core {
    public class TestDeletion : CellTestBase {
        [Test]
        public void WhenReferenceIsDeletedCellHasError() {
            WhenOneCellReferencesAnother(cell, cell2);
            WhenOneCellIsRemoved(cell2);
            ThenCellHasError(cell);
        }


        [Test]
        public void ReEvaluationWorksAfterDeletion() {
            WhenFormulaIs(cell, $"{cell2.Name} + {cell3.Name}");
            WhenFormulaIs(cell2, $"{cell3.Name} + 10");
            WhenFormulaIs(cell3, "1");
            ThenValueIs(cell3, 1);
            ThenValueIs(cell2, 11);
            ThenValueIs(cell, 12);

            WhenOneCellIsRemoved(cell);
            ThenValueIs(cell2, 11);
            ThenValueIs(cell, 12);
        }

        private void WhenOneCellIsRemoved(NumericCell cell) {
            model.DeleteCell(cell);
        }


    }
}