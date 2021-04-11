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

        private void WhenOneCellIsRemoved(Cell cell) {
            model.DeleteCell(cell);
        }
    }
}