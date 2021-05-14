using System;

namespace Logik.Core.Formula {
    public abstract class Constants {

        public static readonly string TableAccessToken = "cell";

        public const char ParensOpen = '(';
        public const char ParensClose = ')';

        public const char Space = ' ';
        public const char Tab = '\t';
        public const char Semicolon = ';';

        public const char Plus = '+';
        public const char Minus = '-';
        public const char Multiplication = '*';
        public const char Division = '/';
        public const char UnaryMinus = '_';
        public const char Quote = '"';

        public static readonly string PlusToken = Plus.ToString();
        public static readonly string MinusToken = Minus.ToString();
        public static readonly string MultiplicationToken = Multiplication.ToString();
        public static readonly string DivisionToken = Division.ToString();
        public static readonly string UnaryMinusToken = UnaryMinus.ToString();
        public static readonly string ParensOpenToken = ParensOpen.ToString();
        public static readonly string ParensCloseToken = ParensClose.ToString();
        public static readonly string SemicolonToken = Semicolon.ToString();
        public static readonly string QuoteToken = Quote.ToString();

        public static readonly int[] WhitespaceChars = new int[] {
            Space, Tab
        };

        public static readonly int[] SingleCharSymbols = new int[] {
            Plus, Minus, Multiplication, Division,
            ParensOpen, ParensClose,
            Semicolon,
            Quote
        };

        public static bool IsSingleCharSymbol(int ch) => Array.IndexOf(SingleCharSymbols, ch) != -1;
        public static bool IsWhitespace(int ch) => Array.IndexOf(WhitespaceChars, ch) != -1;

        public static bool IsAtomEnd(int ch) => IsSingleCharSymbol(ch) || IsWhitespace(ch);

        public static bool IsTableAccess(string token) => token == TableAccessToken;
    }
}