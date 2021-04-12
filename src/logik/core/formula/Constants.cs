using System;
using System.Collections.Generic;

namespace Logik.Core.Formula {
    public abstract class Constants {
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

        public static readonly string PlusToken = "" + Plus;
        public static readonly string MinusToken = "" + Minus;
        public static readonly string MultiplicationToken = "" + Multiplication;
        public static readonly string DivisionToken = "" + Division;
        public static readonly string UnaryMinusToken = "" + UnaryMinus;
        public static readonly string ParensOpenToken = "" + ParensOpen;
        public static readonly string ParensCloseToken = "" + ParensClose;
        public static readonly string SemicolonToken = "" + Semicolon;
        public static readonly string MaxToken = "max";
        public static readonly string MinToken = "min";

        public static readonly int[] WhitespaceChars = new int[] { Space, Tab };
        public static readonly int[] NumberEndChars = new int[]
            { Space, Tab, ParensClose, ParensOpen, Plus, Minus, Multiplication, Division, Semicolon };
        public static readonly int[] Operators = new int[] { Plus, Minus, Multiplication, Division };
        public static readonly int[] FormulaSymbols = new int[]
            { Plus, Minus, Multiplication, Division, ParensOpen, ParensClose, Semicolon };
        protected static List<string> Functions = new List<string> {MaxToken, MinToken };
        protected static int[] FunctionArguments = {2, 2};
        
        public static bool IsNumberEnd(int ch) => Array.IndexOf(NumberEndChars, ch) != -1;

        public static bool IsWhitespace(int ch) => Array.IndexOf(WhitespaceChars, ch) != -1;

        public static bool IsFormulaSymbol(int ch) => Array.IndexOf(FormulaSymbols, ch) != -1;

        protected static string OperatorsString =  "" + Plus + Minus + Multiplication + Division + UnaryMinus;
        protected static int[] OperatorPrecedence = {2, 2, 3, 3, 4};
        protected static int[] OperatorArguments = {2, 2, 2, 2, 1 };
        protected static bool[] LeftAssociative = {true, true, true, true, false};


        public static bool HigherPrecedence(string op1, string op2) {
            return OperatorPrecedence[OperatorsString.IndexOf(op1)] > OperatorPrecedence[OperatorsString.IndexOf(op2)];
        }

        public static int NumArguments(string token) {
            return OperatorArguments[OperatorsString.IndexOf(token)];
        }
        public static int NumArgumentsFunc(string token) {
            return FunctionArguments[Functions.IndexOf(token)];
        }

        public static bool IsOperator(string token) {
            return OperatorsString.Contains(token);
        }

    }
}