using Logik.Core;
using Logik.Core.Formula;
using Logik.Storage;
using NUnit.Framework;

namespace Logik.Tests {

    public class TestStorage : CellTestBase {
        ModelStorage storage;

        [SetUp]
        public new void Setup() {
            base.Setup();
            storage = new ModelStorage();
        }

        [Test]
        public void CellInformationPreservedInFile() {
            WhenFormulaIs(cell, "10");
            WhenFormulaIs(cell2, "65");
            WhenFormulaIs(cell3, "C1 * 5");
            WhenFormulaIs(cell4, "C2 + C3 + C1");
            WhenModelIsSavedIn("testfile.logik");

            WhenModelIsReset();

            WhenModelIsRestoredFrom("testfile.logik");
            WhenCellsAreRestoredFromModel();

            ThenFormulaIs(cell, "10");
            ThenFormulaIs(cell2, "65");
            ThenFormulaIs(cell3, "C1 * 5");
            ThenFormulaIs(cell4, "C2 + C3 + C1");
        }

        private void WhenModelIsRestoredFrom(string filename) {
            model = storage.Load(filename);
        }

        private void WhenModelIsSavedIn(string filename) {
            storage.Save(model, filename);
        }
    }
}