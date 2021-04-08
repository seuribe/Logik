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
    }
}