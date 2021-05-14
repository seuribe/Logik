using System;
using System.Collections.Generic;
using System.Text;

namespace Logik.Core.Formula {

    /// <summary>
    /// Turn a string into a list of tokens
    /// </summary>
    public class Tokenizer : Constants {

        public List<string> Tokens { get; private set; } = new List<string>();
        private SimpleStringReader reader;

        public Tokenizer(string input) {
            reader = new SimpleStringReader(input);

            while (HasInput()) {
                if (DiscardWhiteSpace())
                    continue;

                if (IsCurrentSingleCharSymbol())
                    ReadSingleChar();
                else
                    ReadAtom();
            }
        }

        private bool HasInput() {
            return reader.Current != -1;
        }

        private bool DiscardWhiteSpace() {
            if (!IsWhitespace(reader.Current))
                return false;

            while (reader.Available && IsWhitespace(reader.Current))
                reader.Advance();

            return true;
        }

        private bool IsCurrentSingleCharSymbol() {
            return IsSingleCharSymbol(reader.Current);
        }

        private void ReadSingleChar() {
            string op = "" + Convert.ToChar(reader.Current);
            reader.Advance();
            Tokens.Add(op);
        }

        private void ReadAtom() {
            var buffer = new StringBuilder();

            while (reader.Available && !IsAtomEnd(reader.Current)) {
                buffer.Append(Convert.ToChar(reader.Current));
                reader.Advance();
            }

            Tokens.Add(buffer.ToString());
        }
     }
}