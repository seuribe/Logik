using NUnit.Framework;

namespace Logik.Tests.Core {

    public class TestParser : ParsingTest {
        [Test]
        public void PostfixSimpleSum() {
            WhenTokenizing(OnePlusTwo);
            WhenParsingTokens();
            ThenPostfixIs(OnePlusTwoPostfix);
        }

        [Test]
        public void PostfixOperatorPrecedence() {
            WhenTokenizing(OnePlusTwoTimesThree);
            WhenParsingTokens();
            ThenPostfixIs(OnePlusTwoTimesThreePostFix);

            WhenTokenizing(OneTimesTwoPlusThree);
            WhenParsingTokens();
            ThenPostfixIs(OneTimesTwoPlusThreePostFix);
        }

        [Test]
        public void ParenthesesSimple() {
            WhenParsingTokens(SimpleParens);
            ThenPostfixIs(SimpleParensPostfix);
            
            WhenParsingTokens(SimpleParensTwo);
            ThenPostfixIs(SimpleParensTwoPostfix);
        }

        [Test]
        public void ParseTabularLookup() {
            WhenParsingTokens(Brackets);
            ThenPostfixIs(BracketsPostfix);
        }
    }
}