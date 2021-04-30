using Logik.Core;
using Logik.Core.Formula;
using NUnit.Framework;
using System;

namespace Logik.Tests.Core {
    public class TestTabularCell {
        protected Model model;
        
        protected TabularCell tcell;

        [SetUp]
        public void Setup() {
            model = new Model(GetEvaluator());
            tcell = new TabularCell();
        }

        protected virtual IEvaluator GetEvaluator() {
            return new TreeEvaluator();
        }
        

        [Test]
        public void StartsWithOneCell() {
            ThenSizeIs(tcell, 1, 1);
            ThenValueIs(tcell, 0, 0, 0);
        }

        [Test]
        public void Resize() {
            WhenResized(tcell, 2, 1);
            ThenSizeIs(tcell, 2, 1);
        }

        [Test]
        public void ValuesPreservedAfterShrinkAndExpand() {
            WhenResized(tcell, 2, 2);
            WhenValueIs(tcell, 1, 1, 5);
            ThenValueIs(tcell, 1, 1, 5);

            WhenResized(tcell, 1, 1);
            WhenResized(tcell, 2, 2);
            ThenValueIs(tcell, 1, 1, 5);
        }

        [Test]
        public void CannotAccessValuesOutsideOfRange() {
            ThenSizeIs(tcell, 1, 1);

            TestDelegate evalCall = () => ThenValueIs(tcell, 2, 2, 0);

            Assert.Throws<LogikException>(evalCall);
        }

        private void WhenValueIs(TabularCell tcell, int row, int column, float value) {
            tcell[row, column] = value;
        }

        private void WhenResized(TabularCell tcell, int newRows, int newColumns) {
            tcell.Resize(newRows, newColumns);
        }

        private void ThenValueIs(TabularCell tcell, int row, int col, float value) {
            Assert.AreEqual(value, tcell[row, col]);
        }

        private void ThenSizeIs(TabularCell tcell, int rows, int columns) {
            Assert.AreEqual(rows, tcell.Rows);
            Assert.AreEqual(columns, tcell.Columns);
        }

        private void ThenColumnCountIs(TabularCell tcell, int c) {
            Assert.AreEqual(c, tcell.Columns);
        }

        private void ThenRowCountIs(TabularCell tcell, int r) {
            Assert.AreEqual(r, tcell.Rows);
        }
    }
}