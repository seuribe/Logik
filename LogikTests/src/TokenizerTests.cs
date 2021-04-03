using Logik.Core;
using NUnit.Framework;

namespace Logik.Tests.Core {

    public class ParsingTest {
        
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

        public const string SimpleParens = "(1 + 2) * 3";
        public static string[] SimpleParensExpected = {"(", "1", "+", "2", ")", "*", "3"};

        public const string NestedParens = "((1 + 2) * 3)";
        public static string[] NestedParensExpected = {"(", "(", "1", "+", "2", ")", "*", "3",")"};

        public const string LotsOfParens = "( ((1 + (2)) ) * 3)";
        public static string[] LotsOfParensExpected = {"(", "(", "(", "1", "+", "(", "2", ")",")",")", "*", "3",")"};

        public const string UnaryMinus = "-(1 + -2) * 3";
        public static string[] UnaryMinusExpected = {"-","(","1","+","-","2",")","*","3"};

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
    }

    public class TestParser : ParsingTest {
        private FormulaParser parser;

        [SetUp]
        public void Setup() {
            parser = new FormulaParser();
        }

        [Test]
        public void TestSimpleParser() {
            var tokenizer = new Tokenizer(OnePlusTwo);
            var postfix = parser.ToPostfix(tokenizer.Tokens);
            CollectionAssert.AreEqual(OnePlusTwoPostfix, postfix);
        }
    }

    public class TokenTestCase {
        public readonly string input;
        public readonly string[] expected;

        public TokenTestCase(string input, string[] expected) {
            this.input = input;
            this.expected = expected;
        }
    }

    public class TestTokenizer : ParsingTest {
        private Tokenizer tokenizer;
        private string input;

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

        private void RunMultipleTests(TokenTestCase[] tests) {
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