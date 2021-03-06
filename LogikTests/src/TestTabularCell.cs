using Logik.Core;
using NUnit.Framework;

namespace Logik.Tests.Core {
    public class TestTabularCell : CellTestBase {

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

        [Test]
        public void CellsAreObservable() {
            bool notified = false;
            WhenResized(tcell, 1, 1);
            tcell.ValueChanged += (cell) => {
                notified = true;
            };
            WhenValueIs(tcell, 0, 0, 5);
            Assert.IsTrue(notified);
        }

    }
}