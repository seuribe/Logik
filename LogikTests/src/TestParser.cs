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
    }
}