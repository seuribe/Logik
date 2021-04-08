using Logik.Core;
using Logik.Core.Formula;
using Logik.Storage;
using NUnit.Framework;
using System.IO;

namespace Logik.Tests.Storage {

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

        [Test]
        public void CellRenamingIsPreserved() {
            CanChangeName(cell, "celda");
            WhenModelIsSavedIn("rename_test.logik");
            WhenModelIsReset();
            WhenModelIsRestoredFrom("rename_test.logik");
            WhenCellsAreRestoredFromModel();
            ThenNameIs(cell, "celda");
        }

        [Test]
        public void CannotReadUnknownEvaluator() {
            string strModel = @"{""evaluator"":""udfsakfuashdlaksidshaladjl"",""cells"":[{""name"":""C1"",""formula"":""10""}]}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(strModel);
            MemoryStream ms = new MemoryStream(bytes);
            using (JsonModelReader reader = new JsonModelReader(ms)) {
                TestDelegate evalCall = () => reader.ReadModel();
                Assert.Throws(Is.InstanceOf<System.Exception>(), evalCall);
            }
        }

        private void WhenModelIsRestoredFrom(string filename) {
            model = storage.Load(filename);
        }

        private void WhenModelIsSavedIn(string filename) {
            storage.Save(model, filename);
        }
    }
}