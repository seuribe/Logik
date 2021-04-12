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

    }
}