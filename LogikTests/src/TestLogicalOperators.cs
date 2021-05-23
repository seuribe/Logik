using NUnit.Framework;

namespace Logik.Tests.Core {
    public class TestLogicalOperators : ParsingTest {
        [Test]
        public void And() {
            WhenBuildingTree("true && true");
            ThenTreeEvalsTo(true);

            WhenBuildingTree("(5 > -5) && (4 + 3 = 7)");
            ThenTreeEvalsTo(true);

            WhenBuildingTree("(5 > -5) && (4 - 3 = 7)");
            ThenTreeEvalsTo(false);
        }

        [Test]
        public void Or() {
            WhenBuildingTree("true || false");
            ThenTreeEvalsTo(true);

            WhenBuildingTree("1 = 2 || false");
            ThenTreeEvalsTo(false);
        }
    }
}