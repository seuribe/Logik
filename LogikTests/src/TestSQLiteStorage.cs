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
            WhenModelIsReset();
            WhenModelIsLoaded();
            WhenCellsAreRestoredFromModel();
            ThenFormulaIs(cell, "1 + 2");
            ThenFormulaIs(cell2, "18");
            ThenFormulaIs(cell3, "100");
        }

        [Test]
        public void SaveTabularData() {
            WhenResized(tcell, 2, 2);
            WhenValueIs(tcell, 0, 0, 5);
            WhenValueIs(tcell, 0, 1, 6);
            WhenValueIs(tcell, 1, 0, 7);
            WhenValueIs(tcell, 1, 1, 8);
            WhenModelIsStored();
            WhenModelIsReset();
            WhenModelIsLoaded();
            WhenCellsAreRestoredFromModel();
            ThenValueIs(tcell, 0, 0, 5);
            ThenValueIs(tcell, 0, 1, 6);
            ThenValueIs(tcell, 1, 0, 7);
            ThenValueIs(tcell, 1, 1, 8);
        }


        private void WhenModelIsLoaded() {
            model = storage.LoadModel();
        }

        private void WhenModelIsStored() {
            storage.StoreModel(model);
        }
    }
}