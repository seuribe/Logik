using Logik.Core;
using NUnit.Framework;

namespace Logik.Tests.Core {

    public class TestObserve {
        private CellIndex cellIndex = new CellIndex();
        private Cell cell;
        private Cell cell2;
        private Cell cell3;

        [SetUp]
        public void Setup() {
            cellIndex.Clear();
            cell = cellIndex.CreateCell();
            cell2 = cellIndex.CreateCell();
            cell3 = cellIndex.CreateCell();
        }

        [Test]
        public void ChangeWhenObservedChanges() {
            cell.Formula = "1";
            cell2.Formula = $"(+ ({cell.Id}) 2)";
            Assert.AreEqual("3", cell2.Value);
            cell.Formula = "2";
            Assert.AreEqual("4", cell2.Value);
        }

        [Test]
        public void IndirectChangeTriggersUpdate() {
            cell.Formula = "1";
            cell2.Formula = $"(+ ({cell.Id}) 2)";
            Assert.AreEqual("3", cell2.Value);
            cell3.Formula = $"(* ({cell2.Id}) 3)";
            Assert.AreEqual("9", cell3.Value);
            cell.Formula = "2";
            Assert.AreEqual("4", cell2.Value);
            Assert.AreEqual("12", cell3.Value);
        }
    }

    public class TestEvaluation {
        private CellIndex cellIndex = new CellIndex();
        private Cell cell;
        private Cell cell2;

        [SetUp]
        public void Setup() {
            cellIndex.Clear();
            cell = cellIndex.CreateCell();
            cell2 = cellIndex.CreateCell();
        }

        [Test]
        public void EvalNumericValue() {
            cell.Formula = "1";
            Assert.AreEqual("1", cell.Value);
        }

        [Test]
        public void EvalAdditionFunction() {
            cell.Formula = "(+ 1 2)";
            Assert.AreEqual("3", cell.Value);
        }

        [Test]
        public void EvalReferencingValueCell() {
            cell.Formula = "1";
            cell2.Formula = $"(+ ({cell.Id}) 2)";
            Assert.AreEqual("3", cell2.Value);
        }
        
        [Test]
        public void EvalReferencingFormulaCell() {
            cell.Formula = "(+ 1 2)";
            cell2.Formula = $"(+ ({cell.Id}) 3)";
            Assert.AreEqual("6", cell2.Value);
        }

        [Test]
        public void EvalNestedFormulas() {
            cell.Formula = "(+ (* 3 (abs 4.0)) 2)";
            Assert.AreEqual("14", cell.Value);
        }
    }
}