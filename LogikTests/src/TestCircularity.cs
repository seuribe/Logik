using NUnit.Framework;

namespace Logik.Tests.Core {
    public class TestCircularity : CellTestBase {
        [Test]
        public void SelfReferenceIsDetected() {
            WhenOneCellReferencesAnother(cell, cell);
            ThenCellHasError(cell);
            ThenCellHasNoReferences(cell);
        }

        [Test]
        public void BrokenSelfReference() {
            WhenOneCellReferencesAnother(cell, cell);
            ThenCellHasError(cell);
        }

        [Test]
        public void MutualReferenceIsDetected() {
            WhenOneCellReferencesAnother(cell, cell2);
            ThenCellHasNoError(cell);

            WhenOneCellReferencesAnother(cell2, cell);
            ThenCellHasError(cell2);
            ThenCellHasNoReferences(cell2);
            ThenCellHasError(cell);
        }

    }
}