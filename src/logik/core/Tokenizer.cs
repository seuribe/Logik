using System;
using System.Collections.Generic;
using System.Text;

namespace Logik.Core {

    // http://www.wcipeg.com/wiki/Shunting_yard_algorithm
    // http://math.oxford.emory.edu/site/cs171/shuntingYardAlgorithm/

    public class FormulaParser {

        private static string Operators = "+-/*";
        private static int[] Precedence = {2, 2, 3, 3};

        private static bool HigherPrecedence(string op1, string op2) {
            return Precedence[Operators.IndexOf(op1)] > Precedence[Operators.IndexOf(op2)];
        }

        private static bool IsOperator(string token) {
            return Operators.Contains(token);
        }

        public static List<string> ToPostfix(List<string> tokens) {
            Stack<string> opstack = new Stack<string>();
            Queue<string> postfix = new Queue<string>();

            foreach (var token in tokens) {
                if (IsOperator(token)) {
                    if (opstack.Count == 0) {
                        opstack.Push(token);
                    } else {
                        var prev = opstack.Peek();
                        if (HigherPrecedence(prev, token)) {
                            opstack.Pop();
                            postfix.Enqueue(prev);
                        }
                        opstack.Push(token);
                    }
                } else {
                    postfix.Enqueue(token);
                }
            }
            while (opstack.Count > 0) {
                postfix.Enqueue(opstack.Pop());
            }

            return new List<string>(postfix);   
        }
    }


    public class Tokenizer {
        public const char ParensOpen = '(';
        public const char ParensClose = ')';

        public const char Space = ' ';
        public const char Tab = '\t';

        public const char Plus = '+';
        public const char Minus = '-';
        public const char Multiplication = '*';
        public const char Division = '/';

        public static readonly int[] WhitespaceChars = new int[] { Space, Tab };
        public static readonly int[] NumberEndChars = new int[] { Space, Tab, ParensClose, ParensOpen, Plus, Minus, Multiplication, Division };
        public static readonly int[] Operators = new int[] { Plus, Minus, Multiplication, Division, ParensOpen, ParensClose };

        public static bool IsNumberEnd(int ch) => Array.IndexOf(NumberEndChars, ch) != -1;

        public static bool IsWhitespace(int ch) => Array.IndexOf(WhitespaceChars, ch) != -1;

        public static bool IsOperator(int ch) => Array.IndexOf(Operators, ch) != -1;

        public static List<string> ProcessInput(string input) {
            var tokens = new List<string>();
            var reader = new SimpleStringReader(input);

            while (reader.Current != -1) {
                if (IsWhitespace(reader.Current)) {
                    DiscardWhile(reader, IsWhitespace);
                    continue;
                }
                if (IsOperator(reader.Current)) {
                    tokens.Add(ReadOperator(reader));
                    continue;
                }
                tokens.Add(ReadAtom(reader));
            }
            return tokens;
        }

        static void DiscardWhile(SimpleStringReader reader, Func<int, bool> predicate) {
            while (reader.Available && predicate(reader.Current))
                reader.Advance();
        }

        static string ReadOperator(SimpleStringReader reader) {
            string op = "" + Convert.ToChar(reader.Current);
            reader.Advance();
            return op;
        }

        static string ReadAtom(SimpleStringReader reader) {
            var buffer = new StringBuilder();

            while (reader.Available && !IsNumberEnd(reader.Current)) {
                buffer.Append(Convert.ToChar(reader.Current));
                reader.Advance();
            }

            return buffer.ToString();
        }
     }
}