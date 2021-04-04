using System.Collections.Generic;

namespace Logik.Core.Formula {
    // http://www.wcipeg.com/wiki/Shunting_yard_algorithm
    // http://math.oxford.emory.edu/site/cs171/shuntingYardAlgorithm/

    public class FormulaParser : Constants {

        private IEnumerator<string> tokens;
        private Stack<string> opstack = new Stack<string>();
        public List<string> Output { get; private set; }  = new List<string>();
        private string current = "";
        private string previous = "";

        private bool CurrentIsOpenParens() => current == ParensOpenToken;
        private bool CurrentIsCloseParens() => current == ParensCloseToken;
        private bool CurrentIsUnaryMinus() => 
            (current == MinusToken && (opstack.Count == 0 || previous == ParensOpenToken || IsOperator(previous)));

        private bool CurrentIsOperator() => IsOperator(current);

        public static bool ShouldStackBefore(string op1, string op2) {
            int idx1 = OperatorsString.IndexOf(op1);
            int idx2 = OperatorsString.IndexOf(op2);
            int prec1 = OperatorPrecedence[idx1];
            int prec2 = OperatorPrecedence[idx2];
            return prec2 > prec1 || (LeftAssociative[idx1] && prec1 == prec2);
        }

        public FormulaParser(List<string> tokens) {
            this.tokens = tokens.GetEnumerator();
            ToPostfix();
        }

        private void ToPostfix() {

            while (ReadNextToken()) {
                if (CurrentIsOpenParens()) {
                    PushCurrent();
                } else if (CurrentIsCloseParens()) {
                    EnqueueOperatorsUntilOpenParens();
                } else if (CurrentIsOperator()) {
                    if (CurrentIsUnaryMinus())
                        MakeCurrentUnaryMinus();

                    if (HasStackedOperators())
                        OutputStackedWithHigherPrecedence();

                    PushCurrent();
                } else {
                    OutputCurrent();
                }
            }
            EnqueueAllOperators();
        }

        private void OutputStackedWithHigherPrecedence() {
            var prev = opstack.Peek();

            while (opstack.Count > 0 && IsOperator(prev) && ShouldStackBefore(current, prev)) {
                Output.Add(opstack.Pop());
                if (opstack.Count > 0)
                    prev = opstack.Peek();
            }
        }

        private bool HasStackedOperators() {
            return opstack.Count > 0;
        }

        private void MakeCurrentUnaryMinus() {
            current = UnaryMinusToken;
        }

        private void OutputCurrent() {
            Output.Add(current);
        }

        private bool ReadNextToken() {
            previous = current;
            if (!tokens.MoveNext())
                return false;

            current = tokens.Current;
            return true;
        }

        private void PushCurrent() {
            opstack.Push(current);
        }

        private void EnqueueAllOperators() {
            while (opstack.Count > 0) {
                Output.Add(opstack.Pop());
            }
        }

        private void EnqueueOperatorsUntilOpenParens() {
            var op = opstack.Pop();
            while (op != ParensOpenToken) {
                Output.Add(op);
                op = opstack.Pop();
            }
        }
    }
}