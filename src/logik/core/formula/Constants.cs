using System.Collections.Generic;
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
        public const char UnaryPlus = '&';
        public const char Quote = '"';

        public static readonly string True = "true";
        public static readonly string False = "false";
        public static readonly string[] BoolNames = {"true","false" };

        public const char LessThan = '<';
        public const char LessOrEqual = '≤';
        public const char GreaterThan = '>';
        public const char GreaterOrEqual = '≥';
        public const char Equal = '=';       

        public static readonly string PlusToken = Plus.ToString();
        public static readonly string MinusToken = Minus.ToString();
        public static readonly string MultiplicationToken = Multiplication.ToString();
        public static readonly string DivisionToken = Division.ToString();
        public static readonly string UnaryMinusToken = UnaryMinus.ToString();
        public static readonly string UnaryPlusToken = UnaryPlus.ToString();
        public static readonly string ParensOpenToken = ParensOpen.ToString();
        public static readonly string ParensCloseToken = ParensClose.ToString();
        public static readonly string SemicolonToken = Semicolon.ToString();
        public static readonly string QuoteToken = Quote.ToString();

        public static readonly string LessThanToken = LessThan.ToString();
        public static readonly string LessOrEqualToken = LessOrEqual.ToString();
        public static readonly string GreaterThanToken = GreaterThan.ToString();
        public static readonly string GreaterOrEqualToken = GreaterOrEqual.ToString();
        public static readonly string EqualToken = Equal.ToString();

        public static readonly Dictionary<string, string> UnaryTokens = 
            new Dictionary<string, string>
                { {MinusToken, UnaryMinusToken},
                  {PlusToken, UnaryPlusToken}, };

        public static readonly int[] ComparisonChars = new int[] {
            LessThan, LessOrEqual, GreaterThan, GreaterOrEqual, Equal
        };

        public static readonly int[] WhitespaceChars = new int[] {
            Space, Tab
        };

        public static readonly int[] SingleCharSymbols = new int[] {
            Plus, Minus, Multiplication, Division,
            ParensOpen, ParensClose,
            Semicolon,
            LessThan, LessOrEqual, GreaterThan, GreaterOrEqual, Equal
        };

        public static bool IsBool(string token) => Array.IndexOf(BoolNames, token) != -1;
        public static bool IsStringStart(int ch) => ch == Quote;
        public static bool IsSingleCharSymbol(int ch) => Array.IndexOf(SingleCharSymbols, ch) != -1;
        public static bool IsWhitespace(int ch) => Array.IndexOf(WhitespaceChars, ch) != -1;
        public static bool IsComparisonChar(int ch) => Array.IndexOf(ComparisonChars, ch) != -1;

        public static bool IsAtomEnd(int ch) => IsSingleCharSymbol(ch) || IsWhitespace(ch) || IsComparisonChar(ch);

        public static bool IsTableAccess(string token) => token == TableAccessToken;

        public static bool IsUnaryToken(string token) => UnaryTokens.ContainsKey(token);

    }
}