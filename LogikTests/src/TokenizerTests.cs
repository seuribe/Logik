using NUnit.Framework;

namespace Logik.Tests.Core {

    public class TestTokenizer : ParsingTest {

        [Test]
        public void TokenizeSimpleNumericValue() {
            WhenTokenizing(NumericValueOne);
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
            WhenTokenizing(input);
            ThenTokensAre(expected);
        }

        private void RunMultipleTests(TokenTestCase[] tests) {
            foreach (var test in tests) {
                WhenTokenizing(test.input);
                ThenTokensAre(test.expected);
            }
        }
    }
}