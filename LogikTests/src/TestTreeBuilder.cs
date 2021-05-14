using Logik.Core;
using Logik.Core.Formula;
using NUnit.Framework;
using System.Collections.Generic;

namespace Logik.Tests.Core {

    public class TestTreeBuilder : ParsingTest {

        static Dictionary<string, float[,]> tables = new Dictionary<string, float[,]> {
            { "one", new float[2,3] {{1, 2, 3 }, {4, 5, 6} } },
            { "two", new float[1,4] {{7, 8, 9, 0 } } }
        };

        static  Value LookupTable(string name, int row, int column) => tables[name][row, column];
        EvalContext ctx = new EvalContext(null, LookupTable);

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
            static EvalNode lookup(string name) => new ValueNode("7");
            WhenBuildingTree("a + 10");
            var localContext = new EvalContext(lookup, null);

            ThenTreeEvalsTo(17, localContext);
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
            ValueLookup lookup = name => {
                if (name == "a")
                    return new ValueNode(5);
                if (name == "b")
                    return new ValueNode(9);
                throw new System.Exception("Unknown variable " + name);
            };
            var localContext = new EvalContext(lookup, null);

            WhenBuildingTree("a * b");
            ThenTreeEvalsTo(5*9, localContext);
        }

        [Test]
        public void ThrowOnUnknownVariable() {
            ValueLookup lookup = name => {
                if (name == "a")
                    return new ValueNode(5);
                if (name == "b")
                    return new ValueNode(9);
                throw new System.Exception("Unknown variable " + name);
            };
            var localContext = new EvalContext(lookup, null);

            WhenBuildingTree("a * c");
            TestDelegate evalCall = () => ThenTreeEvalsTo(5*9, localContext);

            Assert.Throws<System.Exception>(evalCall);
        }

        [Test]
        public void ThrowWhenVariableIsUndefined() {
            Dictionary<string, EvalNode> variables = new Dictionary<string, EvalNode> {
                {"a", new ValueNode(5) },
                {"b", new ValueNode(3) },
                {"c", new ValueNode(8) }
            };
            ValueLookup valueLookup = name => variables[name];
            var testContext = new EvalContext(valueLookup, null);

            WhenBuildingTree("a + b * c");
            ThenTreeEvalsTo(5 + 3 * 8, testContext);

            TestDelegate evalCall = () => ThenTreeEvalsTo(5 + 3 + 8, testContext);
            variables.Remove("a");

            Assert.Throws(Is.InstanceOf<System.Exception>(), evalCall);
        }

        [Test]
        public void SimpleTabularAccess() {
            TabularLookup lookup = (name, row, column) => 17;

            WhenBuildingTree("cell(C1; 0; 0)");
            ThenTreeEvalsTo(17, new EvalContext(null, lookup));
        }

        [Test]
        public void TabularAccessFunction() {
            WhenBuildingTree("cell(one; 1; 1)");
            ThenTreeEvalsTo(5, ctx);

            WhenBuildingTree("cell(two; 0; 0)");
            ThenTreeEvalsTo(7, ctx);
        }

        [Test]
        public void NestedTabularAccess() {
            WhenBuildingTree("cell(one; 0; 0)");
            ThenTreeEvalsTo(1, ctx);

            WhenBuildingTree("cell(two; 0; cell(one; 0; 0))");
            ThenTreeEvalsTo(8, ctx);
        }

        [Test]
        public void CollectTableReferenceCells() {
            WhenBuildingTree("cell(two; 0; cell(one; 0; 0))");
            var nodes = evalTree.Collect( node => (node is TabularReferenceNode) );
            foreach (var node in nodes)
                Assert.IsTrue(node is TabularReferenceNode);
            
            nodes = evalTree.Collect( node => (node is ValueNode) && node.Eval(ctx) == 0 );
            foreach (var node in nodes)
                Assert.AreEqual(0, node.Eval(ctx).AsInt);
        }

        [Test]
        public void StringsCorrectlyParsed() {
            const string test = "\"hello + hello!\"";
            WhenBuildingTree(test);
            ThenTreeEvalsTo(test);
        }
    }
}