using NUnit.Framework;

namespace Logik.Tests.Core {
    public class TestFunctionLibrary : ParsingTest {
                
        [Test]
        public void Average() {
            WhenBuildingTree("average(1; 2; 10; 6; 3)");
            ThenTreeEvalsTo(4.4f);
        }
                
        [Test]
        public void Min() {
            WhenBuildingTree("min(1; 2; 10; 6; 3)");
            ThenTreeEvalsTo(1);
        }
                
        [Test]
        public void Max() {
            WhenBuildingTree("max(1; 2; 10; 6; 3)");
            ThenTreeEvalsTo(10);
        }

        [Test]
        public void Concat() {
            const string a = "\"lala\"";
            const string b = "\"lele\"";
            const string expected = "lalalele";
            WhenBuildingTree($"concat({a};{b})");
            ThenTreeEvalsTo(expected);
        }
    }
}