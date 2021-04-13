using Logik.Core.Formula;
using NUnit.Framework;
using System.Collections.Generic;

namespace Logik.Tests.Core {
    public class ParsingTest {
        protected List<string> tokens;
        protected List<string> postfix;
        protected EvalNode evalTree;

        public const string NumericValueOne = "1";

        public const string OnePlusTwo = "1 + 2";
        public const string OnePlusTwoNoSpaces = "1+2";
        public const string OnePlusTwoSpaceBeforeOp = "1 +2";
        public const string OnePlusTwoSpaceAfterOp = "1+ 2";
        public const string OnePlusTwoUntrimmed = " 1 + 2 ";
        public const string OnePlusTwoManySpaces = "  1    +   2    ";

        public static string[] OnePlusTwoExpected = {"1", "+", "2" };
        public static string[] OnePlusTwoPostfix = {"1", "2", "+" };

        public const string MultipleOperations = "1 + 2 * 3 / 4 - 5";
        public static string[] MultipleOperationsExpected = {"1", "+", "2", "*", "3", "/", "4", "-", "5" };

        public static string OnePlusTwoTimesThree = "1 + 2 * 3";
        public static string[] OnePlusTwoTimesThreePostFix = {"1", "2", "3", "*", "+" };
        public static string OneTimesTwoPlusThree = "1 * 2 + 3";
        public static string[] OneTimesTwoPlusThreePostFix = {"1", "2", "*", "3", "+" };


        public const string SimpleParens = "(1 + 2) * 3";
        public static string[] SimpleParensExpected = {"(", "1", "+", "2", ")", "*", "3"};
        public static string[] SimpleParensPostfix = {"1", "2", "+", "3", "*"};

        public const string SimpleParensTwo = "1 + (2 * 3)";
        public static string[] SimpleParensTwoPostfix = {"1", "2", "3", "*", "+"};

        public const string NestedParens = "((1 + 2) * 3)";
        public static string[] NestedParensExpected = {"(", "(", "1", "+", "2", ")", "*", "3",")"};

        public const string LotsOfParens = "( ((1 + (2)) ) * 3)";
        public static string[] LotsOfParensExpected = {"(", "(", "(", "1", "+", "(", "2", ")",")",")", "*", "3",")"};

        public const string UnaryMinus = "-(1 + -2) * 3";
        public static string[] UnaryMinusExpected = {"-","(","1","+","-","2",")","*","3"};
        
        public class TokenTestCase {
            public readonly string input;
            public readonly string[] expected;

            public TokenTestCase(string input, string[] expected) {
                this.input = input;
                this.expected = expected;
            }
        }

        public TokenTestCase[] testsWithSpaces = {
            new TokenTestCase(OnePlusTwo, OnePlusTwoExpected),
            new TokenTestCase(OnePlusTwoNoSpaces, OnePlusTwoExpected),
            new TokenTestCase(OnePlusTwoSpaceBeforeOp, OnePlusTwoExpected),
            new TokenTestCase(OnePlusTwoSpaceAfterOp, OnePlusTwoExpected),
            new TokenTestCase(OnePlusTwoUntrimmed, OnePlusTwoExpected),
            new TokenTestCase(OnePlusTwoManySpaces, OnePlusTwoExpected),
        };

        public TokenTestCase[] testsWithParens = {
            new TokenTestCase(SimpleParens, SimpleParensExpected),
            new TokenTestCase(NestedParens, NestedParensExpected),
            new TokenTestCase(LotsOfParens, LotsOfParensExpected),
        };

        protected void WhenTokenizing(string input) {
            tokens = new Tokenizer(input).Tokens;
        }

        protected void WhenParsingTokens(string input = null) {
            if (input != null)
                WhenTokenizing(input);

            postfix = new FormulaParser(tokens).Output;
        }

        protected void WhenBuildingTree(string input = null, ValueLookup lookupFunction = null) {
            if (input != null)
                WhenParsingTokens(input);

            evalTree = new EvalTreeBuilder( lookupFunction ).BuildTree(postfix);
        }

        protected void ThenTokensAre(IEnumerable<string> expected) {
            CollectionAssert.AreEqual(expected, tokens);
        }

        public void ThenFirstTokenIs(string expected) {
            Assert.AreEqual(expected, tokens[0]);
        }

        protected void ThenPostfixIs(IEnumerable<string> expected) {
            CollectionAssert.AreEqual(expected, postfix);
        }

        protected void ThenTreeEvalsTo(float expected) {
            Assert.AreEqual(expected, evalTree.Eval());
        }
    }
}