using NUnit.Framework;
using Logik.Storage;
using Logik.Core;
using System;

namespace Logik.Tests.Storage {

    public class TestSQLiteStorage : CellTestBase {
        private SQLiteModelStorage storage;

        [SetUp]
        public new void Setup() {
            base.Setup();
            storage = new SQLiteModelStorage("dummy_db.logik");
        }

        [Test]
        public void TestSaveAndLoad() {
            WhenFormulaIs(cell, "1 + 2");
            WhenFormulaIs(cell2, "18");
            WhenFormulaIs(cell3, "100");
            WhenModelIsStored();
            WhenModelIsLoaded();
            ThenFormulaIs(cell, "1 + 2");
            ThenFormulaIs(cell2, "18");
            ThenFormulaIs(cell3, "100");

        }

        private void WhenModelIsLoaded() {
            model = storage.LoadModel();
        }

        private void WhenModelIsStored() {
            storage.StoreModel(model);
        }
    }
}