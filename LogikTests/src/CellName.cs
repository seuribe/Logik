using Logik.Core;
using NUnit.Framework;

namespace Logik.Tests.Core {
    public class CellName : CellTestBase {
        [Test]
        public void CellNameChangesWhenNoConflict() {
            const string newName = "lala";
            ThenNameIsNot(cell, newName);
            CanChangeName(cell, newName);
            ThenNameIs(cell, newName);
        }

        [Test]
        public void CannotChangeNameWhenConflict() {
            string oldName = cell.Name;
            ThenNameIs(cell, oldName);
            CannotChangeName(cell, cell2.Name);
            ThenNameIs(cell, oldName);
        }

        [Test]
        public void ReferencesWorkAfterRename() {
            const string newName = "lala";
            WhenFormulaIs(cell, "1");
            WhenFormulaIs(cell2, $"{cell.Name} + 3");
            ThenCellIsReferencingAnother(cell2, cell);
            ThenValueIs(cell2, "4");

            CanChangeName(cell, newName);
            ThenCellIsReferencingAnother(cell2, cell);
            ThenValueIs(cell2, "4");

            WhenFormulaIs(cell3, $"{cell.Name} * 7");
            ThenValueIs(cell3, "7");
        }

        protected void CanChangeName(Cell cell, string newName) {
            Assert.IsTrue(cell.TryNameChange(newName));
        }
        
        protected void CannotChangeName(Cell cell, string newName) {
            Assert.IsFalse(cell.TryNameChange(newName));
        }

        protected void ThenNameIs(Cell cell, string name) {
            Assert.AreEqual(name, cell.Name);
        }

        protected void ThenNameIsNot(Cell cell, string name) {
            Assert.AreNotEqual(name, cell.Name);
        }

    }
}