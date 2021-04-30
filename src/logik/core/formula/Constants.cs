using System;

namespace Logik.Core.Formula {
    public abstract class Constants {
        public const char ParensOpen = '(';
        public const char ParensClose = ')';

        public const char BracketsOpen = '[';
        public const char BracketsClose = ']';

        public const char Space = ' ';
        public const char Tab = '\t';
        public const char Semicolon = ';';
        public const char Colon = ':';

        public const char Plus = '+';
        public const char Minus = '-';
        public const char Multiplication = '*';
        public const char Division = '/';
        public const char UnaryMinus = '_';

        public static readonly string PlusToken = "" + Plus;
        public static readonly string MinusToken = "" + Minus;
        public static readonly string MultiplicationToken = "" + Multiplication;
        public static readonly string DivisionToken = "" + Division;
        public static readonly string UnaryMinusToken = "" + UnaryMinus;

        public static readonly string ParensOpenToken = "" + ParensOpen;
        public static readonly string ParensCloseToken = "" + ParensClose;
        public static readonly string BracketsOpenToken = "" + BracketsOpen;
        public static readonly string BracketsCloseToken = "" + BracketsClose;
        public static readonly string SemicolonToken = "" + Semicolon;
        public static readonly string ColonToken = "" + Colon;

        public static readonly int[] WhitespaceChars = new int[] { Space, Tab };
        public static readonly int[] NumberEndChars = new int[]
            { Space, Tab,
              Plus, Minus, Multiplication, Division,
              ParensClose, ParensOpen,
              BracketsOpen, BracketsClose,
              Semicolon,
              Colon };

        public static readonly int[] FormulaSymbols = new int[]
            { Plus, Minus, Multiplication, Division,
              ParensOpen, ParensClose,
              BracketsOpen, BracketsClose,
              Semicolon,
              Colon };
        
        public static bool IsAtomEnd(int ch) => Array.IndexOf(NumberEndChars, ch) != -1;

        public static bool IsWhitespace(int ch) => Array.IndexOf(WhitespaceChars, ch) != -1;

        public static bool IsFormulaSymbol(int ch) => Array.IndexOf(FormulaSymbols, ch) != -1;

    }
}