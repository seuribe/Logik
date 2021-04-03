using System;
using System.Collections.Generic;
using System.Text;

namespace Logik.Core {

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