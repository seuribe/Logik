using Logik.Core.Formula;
using NUnit.Framework;
using System.Collections.Generic;

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
        public void EvalEqualPrecedence() {
            WhenBuildingTree("10 / 5 * 3");
            ThenTreeEvalsTo(6);
        }

        [Test]
        public void EvalDivision() {
            WhenBuildingTree("4 / 2");
            ThenTreeEvalsTo(2);

            WhenBuildingTree("5 / 3");
            ThenTreeEvalsTo(5/3f);
        }

        [Test]
        public void EvalParens() {
            WhenBuildingTree(SimpleParens);
            ThenTreeEvalsTo(9);
            
            WhenBuildingTree(SimpleParensTwo);
            ThenTreeEvalsTo(7);
        }

        [Test]
        public void EvalUnaryMinus() {
            WhenBuildingTree(UnaryMinus);
            ThenTreeEvalsTo(3);
        }

        [Test]
        public void UnaryMinusPrecedence() {
            WhenBuildingTree("10/-1*-2");
            ThenTreeEvalsTo(20);
        }

        [Test]
        public void EvaluateLookupFunction() {
            WhenBuildingTree("a + 10", name => 7);
            ThenTreeEvalsTo(17);
        }

        [Test]
        public void EvaluateFunction() {
            WhenBuildingTree("max(1; 2)");
            ThenTreeEvalsTo(2);
        }

        [Test]
        public void EvaluateNestedFunction() {
            WhenBuildingTree("18 * ( 4 / max(1; 2))");
            ThenTreeEvalsTo(36);
        }
        
        [Test]
        public void EvaluateVariadicFunction() {
            WhenBuildingTree("max(1; 2; 10; 6; 3)");
            ThenTreeEvalsTo(10);
            WhenBuildingTree("min(1; 2; 10; 6; 3)");
            ThenTreeEvalsTo(1);
        }

        [Test]
        public void EvaluateVariables() {
            WhenBuildingTree("a * b", name => {
                if (name == "a")
                    return 5;
                if (name == "b")
                    return 9;
                throw new System.Exception("Unknown variable " + name);
            });
            ThenTreeEvalsTo(5*9);
        }

        [Test]
        public void ThrowOnUnknownVariable() {
            WhenBuildingTree("a * c", name => {
                if (name == "a")
                    return 5;
                if (name == "b")
                    return 9;
                throw new System.Exception("Unknown variable " + name);
            });
            TestDelegate evalCall = () => ThenTreeEvalsTo(5*9);

            Assert.Throws<System.Exception>(evalCall);
        }

        [Test]
        public void ThrowWhenVariableIsUndefined() {
            Dictionary<string, float> variables = new Dictionary<string, float> {
                {"a", 5 },
                {"b", 3 },
                {"c", 8 }
            };
            ValueLookup valueLookup = name => variables[name];

            WhenBuildingTree("a + b * c", valueLookup);
            ThenTreeEvalsTo(5 + 3 * 8);

            TestDelegate evalCall = () => ThenTreeEvalsTo(5 + 3 + 8);
            variables.Remove("a");

            Assert.Throws(Is.InstanceOf<System.Exception>(), evalCall);
        }

        [Test]
        public void SimpleTabularAccess() {
            TabularLookup lookup = (name, row, column) => 17;

            WhenBuildingTree("cell(C1; 0; 0)", null, lookup);
            ThenTreeEvalsTo(17);
        }

        [Test]
        public void TabularAccessFunction() {
            var tables = new Dictionary<string, float[,]> {
                { "one", new float[2,3] {{1, 2, 3 }, {4, 5, 6} } },
                { "two", new float[1,4] {{7, 8, 9, 0 } } }
            };

            TabularLookup lookup = (name, row, column) => tables[name][row, column];

            WhenBuildingTree("cell(one; 1; 1)", null, lookup);
            ThenTreeEvalsTo(5);

            WhenBuildingTree("cell(two; 0; 0)", null, lookup);
            ThenTreeEvalsTo(7);
        }

        [Test]
        public void NestedTabularAccess() {
            var tables = new Dictionary<string, float[,]> {
                { "one", new float[2,3] {{1, 2, 3 }, {4, 5, 6} } },
                { "two", new float[1,4] {{7, 8, 9, 0 } } }
            };

            TabularLookup lookup = (name, row, column) => tables[name][row, column];

            WhenBuildingTree("cell(one; 0; 0)", null, lookup);
            ThenTreeEvalsTo(1);

            WhenBuildingTree("cell(two; 0; cell(one; 0; 0))", null, lookup);
            ThenTreeEvalsTo(8);
        }

    }
}