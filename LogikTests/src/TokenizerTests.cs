using Logik.Core;
using NUnit.Framework;

namespace Logik.Tests.Core {

    internal class TokenTest {
        public readonly string input;
        public readonly string[] expected;

        public TokenTest(string input, string[] expected) {
            this.input = input;
            this.expected = expected;
        }
    }

    public class TestTokenizer {
        private Tokenizer tokenizer;
        private string input;

        private const string NumericValueOne = "1";

        private const string OnePlusTwo = "1 + 2";
        private const string OnePlusTwoNoSpaces = "1+2";
        private const string OnePlusTwoSpaceBeforeOp = "1 +2";
        private const string OnePlusTwoSpaceAfterOp = "1+ 2";
        private const string OnePlusTwoUntrimmed = " 1 + 2 ";
        private const string OnePlusTwoManySpaces = "  1    +   2    ";

        private static string[] OnePlusTwoExpected = {"1", "+", "2" };

        private const string MultipleOperations = "1 + 2 * 3 / 4 - 5";
        private static string[] MultipleOperationsExpected = {"1", "+", "2", "*", "3", "/", "4", "-", "5" };

        private const string SimpleParens = "(1 + 2) * 3";
        private static string[] SimpleParensExpected = {"(", "1", "+", "2", ")", "*", "3"};

        private const string NestedParens = "((1 + 2) * 3)";
        private static string[] NestedParensExpected = {"(", "(", "1", "+", "2", ")", "*", "3",")"};

        private const string LotsOfParens = "( ((1 + (2)) ) * 3)";
        private static string[] LotsOfParensExpected = {"(", "(", "(", "1", "+", "(", "2", ")",")",")", "*", "3",")"};

        private const string UnaryMinus = "-(1 + -2) * 3";
        private static string[] UnaryMinusExpected = {"-","(","1","+","-","2",")","*","3"};

        private TokenTest[] testsWithSpaces = {
            new TokenTest(OnePlusTwo, OnePlusTwoExpected),
            new TokenTest(OnePlusTwoNoSpaces, OnePlusTwoExpected),
            new TokenTest(OnePlusTwoSpaceBeforeOp, OnePlusTwoExpected),
            new TokenTest(OnePlusTwoSpaceAfterOp, OnePlusTwoExpected),
            new TokenTest(OnePlusTwoUntrimmed, OnePlusTwoExpected),
            new TokenTest(OnePlusTwoManySpaces, OnePlusTwoExpected),
        };

        private TokenTest[] testsWithParens = {
            new TokenTest(SimpleParens, SimpleParensExpected),
            new TokenTest(NestedParens, NestedParensExpected),
            new TokenTest(LotsOfParens, LotsOfParensExpected),
        };

        [SetUp]
        public void Setup() { }

        [Test]
        public void TokenizeSimpleNumericValue() {
            WhenInputIs(NumericValueOne);
            Tokenize();
            ThenFirstTokenIs(NumericValueOne);
        }

        [Test]
        public void TokenizeAddition() {
            RunTest(OnePlusTwo, OnePlusTwoExpected);
        }

        [Test]
        public void TokenizeMultipleOperations() {
            RunTest(MultipleOperations, MultipleOperationsExpected);
        }

        [Test]
        public void TokenizeAdditionNoSpaces() {
            RunTest(OnePlusTwoNoSpaces, OnePlusTwoExpected);
        }

        [Test]
        public void TokenizeWithParens() {
            RunMultipleTests(testsWithParens);
        }

        [Test]
        public void TokenizeWithSpaces() {
            RunMultipleTests(testsWithSpaces);
        }

        [Test]
        public void TokenizeUnaryMinus() {
            RunTest(UnaryMinus, UnaryMinusExpected);
        }

        private void RunTest(string input, string[] expected) {
            WhenInputIs(input);
            Tokenize();
            ThenTokensAre(expected);
        }

        private void RunMultipleTests(TokenTest[] tests) {
            foreach (var test in tests) {
                WhenInputIs(test.input);
                Tokenize();
                ThenTokensAre(test.expected);
            }
        }

        public void Tokenize() {
            tokenizer = new Tokenizer(input);
        }

        public void WhenInputIs(string input) {
            this.input = input;
        }

        public void ThenFirstTokenIs(string expected) {
            Assert.AreEqual(expected, tokenizer.Tokens[0]);
        }

        public void ThenTokensAre(string[] expected) {
            CollectionAssert.AreEqual(expected, tokenizer.Tokens);
        }

        

    }
}