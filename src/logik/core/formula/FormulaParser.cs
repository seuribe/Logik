using System;
using System.Collections.Generic;

namespace Logik.Core.Formula {

    /// <summary>
    /// Parse mathematical formulas using the Shunting Yard algorithm
    /// 
    /// http://www.wcipeg.com/wiki/Shunting_yard_algorithm
    /// http://math.oxford.emory.edu/site/cs171/shuntingYardAlgorithm/
    /// 
    /// Supports parenthesis, unary minus and (variadic) functions.
    /// 
    /// Also supports a special function for tabular access
    /// 
    /// </summary>

    public class FormulaParser : Constants {

        private IEnumerator<string> tokens;
        private Stack<string> opstack = new Stack<string>();
        private Stack<int> arity = new Stack<int>();
        public List<string> Output { get; private set; }  = new List<string>();

        private string current = null;
        private string previous = null;

        private bool CurrentIsOpenParens() => current == ParensOpenToken;
        private bool CurrentIsCloseParens() => current == ParensCloseToken;
        private bool CurrentIsUnaryMinus() => (current == MinusToken &&
            (previous == null || previous == ParensOpenToken || OperatorLibrary.IsOperator(previous)));
        private bool CurrentIsOperator() => OperatorLibrary.IsOperator(current);
        private bool CurrentIsFunction() => FunctionLibrary.IsFunction(current);
        private bool CurrentIsSemicolon() => current == SemicolonToken;
        private bool CurrentIsTableAccess() => IsTableAccess(current);

        public static bool ShouldStackBefore(string token1, string token2) {
            var op1 = OperatorLibrary.Operators[token1];
            var op2 = OperatorLibrary.Operators[token2];
            return op2.Precedence > op1.Precedence|| (op1.LeftAssociative && op1.Precedence == op2.Precedence);
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
                    FinishSubExpression();
                } else if (CurrentIsOperator()) {
                    if (CurrentIsUnaryMinus())
                        MakeCurrentUnaryMinus();

                    if (HasStackedOperators())
                        OutputStackedWithHigherPrecedence();

                    PushCurrent();
                } else if (CurrentIsFunction()) {
                    PushFunction();
                } else if (CurrentIsTableAccess()) {
                    PushTableAccess();
                } else if (CurrentIsSemicolon()) {
                    PushFunctionParameter();
                } else {
                    OutputCurrent();
                }
            }
            EnqueueAllOperators();
        }

        private void OutputStackedWithHigherPrecedence() {
            var prev = opstack.Peek();

            while (opstack.Count > 0 && OperatorLibrary.IsOperator(prev) && ShouldStackBefore(current, prev)) {
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
        
        private void FinishSubExpression() {
            EnqueueOperatorsUntilOpenParens();
            if (opstack.Count == 0)
                return;

            var top = opstack.Peek();
            if (!FunctionLibrary.IsFunction(top) && !IsTableAccess(top))
                return;

            Output.Add(top);
            Output.Add(arity.Pop().ToString());
            opstack.Pop();
        }

        private void PushFunction() {
            arity.Push(1);
            PushCurrent();
        }

        private void PushTableAccess() {
            arity.Push(1);
            PushCurrent();
        }

        private void PushFunctionParameter() {
            arity.Push(arity.Pop()+1);
            EnqueueOperatorsUntilOpenParens();
            PushOpenParens();
        }

        private void PushOpenParens() {
            opstack.Push(ParensOpenToken);
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