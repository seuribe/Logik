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
            WhenBuildingTree("min(1; 2; 10; -6; 3)");
            ThenTreeEvalsTo(-6);
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

        [Test]
        public void Substring() {
            const string a = "lala1234567890lele";
            WhenBuildingTree($"substring(\"{a}\")");
            ThenTreeEvalsTo(a);
            WhenBuildingTree($"substring(\"{a}\"; 1)");
            ThenTreeEvalsTo(a.Substring(1));
            WhenBuildingTree($"substring(\"{a}\"; 1; 9)");
            ThenTreeEvalsTo(a.Substring(1, 9));
        }

        [Test]
        public void IndexOf() {
            const string a = "lala1234567890lele";
            const string b = "lala";
            WhenBuildingTree($"indexof(\"{a}\"; \"{b}\")");
            ThenTreeEvalsTo(0);
            const string c = "1";
            WhenBuildingTree($"indexof(\"{a}\"; \"{c}\")");
            ThenTreeEvalsTo(4);
        }

        [Test]
        public void TestIf() {
            WhenBuildingTree("if(true; 18; 5)");
            ThenTreeEvalsTo(18);
            WhenBuildingTree("if(false; 18; 5)");
            ThenTreeEvalsTo(5);
        }

    }
}