using NUnit.Framework;

namespace Logik.Tests.Core {
    public class TestTreeBuilder : ParsingTest {
        [Test]
        public void EvalSimpleSum() {
            WhenBuildingTree(OnePlusTwo);
            ThenTreeEvalsTo(3);
        }

        [Test]
        public void EvalPrecedence() {
            WhenBuildingTree(OnePlusTwoTimesThree);
            ThenTreeEvalsTo(7);
            WhenBuildingTree(OneTimesTwoPlusThree);
            ThenTreeEvalsTo(5);
        }

        [Test]
        public void EvalDivision() {
            WhenBuildingTree("4 / 2");
            ThenTreeEvalsTo(2);

            WhenBuildingTree("5 / 3");
            ThenTreeEvalsTo(5/3f);
        }
    }
}