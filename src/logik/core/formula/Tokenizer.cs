using System;
using System.Collections.Generic;
using System.Text;

namespace Logik.Core.Formula {

    public class Tokenizer : Constants {

        public static List<string> ProcessInput(string input) {
            var tokens = new List<string>();
            var reader = new SimpleStringReader(input);

            while (reader.Current != -1) {
                if (IsWhitespace(reader.Current)) {
                    DiscardWhile(reader, IsWhitespace);
                    continue;
                }
                if (IsFormulaSymbol(reader.Current)) {
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