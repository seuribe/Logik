using Logik.Core;
using NUnit.Framework;

namespace Logik.Tests.Core {
    public class CellName : CellTestBase {
        [Test]
        public void CellNameChangesWhenNoConflict() {
            ThenValueIs(cell, "0");
            const string newName = "lala";
            ThenNameIsNot(cell, newName);
            CanChangeName(cell, newName);
            ThenNameIs(cell, newName);
            ThenValueIs(cell, "0");
        }

        [Test]
        public void CannotChangeNameWhenConflict() {
            string oldName = cell.Name;
            ThenNameIs(cell, oldName);
            CannotChangeName(cell, cell2.Name);
            ThenNameIs(cell, oldName);
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