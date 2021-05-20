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
            input = Preprocess(input);
            reader = new SimpleStringReader(input);

            while (HasInput()) {
                if (DiscardWhiteSpace())
                    continue;

                if (IsCurrentStringStart())
                    ReadString();
                else if (IsCurrentSingleCharSymbol())
                    ReadSingleChar();
                else
                    ReadAtom();
            }
        }

        private string Preprocess(string input) {
            foreach (var replacePair in InputReplace)
                input = input.Replace(replacePair.Key, replacePair.Value);

            return input;
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

        private bool IsCurrentStringStart() {
            return IsStringStart(reader.Current);
        }

        private bool IsCurrentSingleCharSymbol() {
            return IsSingleCharSymbol(reader.Current);
        }

        private string ReadUntil(Predicate<int> stopCondition) {
            var buffer = new StringBuilder();

            while (reader.Available && !stopCondition(reader.Current)) {
                buffer.Append(Convert.ToChar(reader.Current));
                reader.Advance();
            }

            return buffer.ToString();
        }

        private void ReadString() {
            reader.Advance(); // discard opening "
            var str = ReadUntil(IsStringStart);
            reader.Advance(); // discard closing "
            Tokens.Add(QuoteToken + str + QuoteToken);
        }

        private void ReadSingleChar() {
            string op = "" + Convert.ToChar(reader.Current);
            reader.Advance();
            Tokens.Add(op);
        }

        private void ReadAtom() {
            Tokens.Add(ReadUntil(IsAtomEnd));
        }
     }
}