using Logik.Core;
using Logik.Core.Formula;
using Logik.Storage;
using NUnit.Framework;
using System.IO;

namespace Logik.Tests.Storage {

    public class TestJsonRepresentation : CellTestBase {
        [Test]
        public void CannotReadUnknownEvaluator() {
            /*
            string strModel = @"{""evaluator"":""udfsakfuashdlaksidshaladjl"",""cells"":[{""name"":""C1"",""formula"":""10""}]}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(strModel);
            MemoryStream ms = new MemoryStream(bytes);
            using (JsonModelReader reader = new JsonModelReader(ms)) {
                TestDelegate evalCall = () => reader.ReadModel();
                Assert.Throws(Is.InstanceOf<System.Exception>(), evalCall);
            }
            */
        }
    }

    public class TestStorage : CellTestBase {
        JsonModelStorage storage;

        [SetUp]
        public new void Setup() {
            base.Setup();
            storage = new JsonModelStorage();
        }

        [Test]
        public void CellInformationPreservedInFile() {
            WhenFormulaIs(cell, "10");
            WhenFormulaIs(cell2, "65");
            WhenFormulaIs(cell3, "C1 * 5");
            WhenFormulaIs(cell4, "C2 + C3 + C1");
            WhenModelIsSavedAndReloaded();
            ThenFormulaIs(cell, "10");
            ThenFormulaIs(cell2, "65");
            ThenFormulaIs(cell3, "C1 * 5");
            ThenFormulaIs(cell4, "C2 + C3 + C1");
        }

        [Test]
        public void CellRenamingIsPreserved() {
            CanChangeName(cell, "celda");
            WhenModelIsSavedAndReloaded();
            ThenNameIs(cell, "celda");
        }

        [Test]
        public void CellValueCalculatedAfterLoad() {
            WhenFormulaIs(cell, "10");
            WhenFormulaIs(cell2, $"{cell.Name} + 2");
            ThenValueIs(cell2, 12);
            WhenModelIsSavedAndReloaded();
            ThenValueIs(cell2, 12);
        }


        private void WhenModelIsSavedAndReloaded(string filename = null) {
            filename ??= Path.GetTempFileName();
            WhenModelIsSavedIn(filename);
            WhenModelIsReset();
            WhenModelIsRestoredFrom(filename);
            WhenCellsAreRestoredFromModel();
        }

        private void WhenModelIsRestoredFrom(string filename) {
            model = JsonModelStorage.Load(filename);
        }

        private void WhenModelIsSavedIn(string filename) {
            JsonModelStorage.Save(model, filename);
        }
    }
}