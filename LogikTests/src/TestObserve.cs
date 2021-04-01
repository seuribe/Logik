using Logik.Core;
using NUnit.Framework;

namespace Logik.Tests.Core {
    public class TestObserve : CellTestBase {

        [Test]
        public void ChangeWhenObservedChanges() {
            WhenFormulaIs(cell, "1");
            WhenFormulaIs(cell2, $"(+ ({cell.Id}) 2)");
            ThenValueIs(cell2, "3");

            WhenFormulaIs(cell, "2");
            ThenValueIs(cell2, "4");
        }

        [Test]
        public void IndirectChangeTriggersUpdate() {
            WhenFormulaIs(cell, "1");
            WhenFormulaIs(cell2, $"(+ ({cell.Id}) 2)");
            WhenFormulaIs(cell3, $"(* ({cell2.Id}) 3)");
            ThenValueIs(cell3, "9");

            WhenFormulaIs(cell, "2");
            ThenValueIs(cell2, "4");
            ThenValueIs(cell3, "12");
        }

        [Test]
        public void ErrorsPropagate() {
            WhenOneCellReferencesAnother(cell2, cell);
            WhenOneCellReferencesAnother(cell3, cell2);
            WhenFormulaIs(cell, InvalidFormulaString);
            ThenCellHasError(cell);
            ThenCellHasError(cell2);
            ThenCellHasError(cell3);
        }
    }
}