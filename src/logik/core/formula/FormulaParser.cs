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

        private readonly IEnumerator<string> tokens;
        private readonly Stack<string> opstack = new Stack<string>();
        private readonly Stack<int> arity = new Stack<int>();
        public List<string> Output { get; private set; }  = new List<string>();

        private string current = null;
        private string previous = null;

        private bool CurrentIsOpenParens() => current == ParensOpenToken;
        private bool CurrentIsCloseParens() => current == ParensCloseToken;
        private bool CurrentIsUnary() => CurrentIsUnaryToken() &&
            (CurrentIsStartOfExpression() || StackTopIsSemicolon() || OperatorLibrary.IsOperator(previous));
        private bool CurrentIsStartOfExpression() => previous == null || previous == ParensOpenToken || previous == SemicolonToken;
        private bool CurrentIsUnaryToken() => IsUnaryToken(current);
        private bool CurrentIsOperator() => OperatorLibrary.IsOperator(current);
        private bool CurrentIsFunction() => FunctionLibrary.IsFunction(current);
        private bool CurrentIsSemicolon() => current == SemicolonToken;

        private bool StackTopIsSemicolon() => (HasStackedTokens() && TopOfStack == SemicolonToken);
        private bool HasStackedTokens() => opstack.Count > 0;

        private string TopOfStack {
            get => opstack.Peek();
        }

        public bool HasPrecedenceOverCurrent(string token) =>
            OperatorLibrary.Operators[token].HasPrecedenceOver(OperatorLibrary.Operators[current]);

        public FormulaParser(List<string> tokens) {
            this.tokens = tokens.GetEnumerator();
            ToPostfix();
        }

        private void ToPostfix() {

            while (ReadNextToken()) {
                if (CurrentIsOpenParens()) {
                    PushCurrent();
                } else if (CurrentIsCloseParens()) {
                    OutputSubExpression();
                } else if (CurrentIsOperator()) {
                    PushOperator();
                } else if (CurrentIsFunction()) {
                    PushFunction();
                } else if (CurrentIsSemicolon()) {
                    PushFunctionParameter();
                } else {
                    OutputCurrent();
                }
            }
            EnqueueAllOperators();
        }

        private void PushOperator() {
            if (CurrentIsUnary())
                MakeCurrentUnary();

            OutputStackedWithHigherPrecedence();

            PushCurrent();
        }
        
        private void PushFunction() {
            PushNewArity();
            PushCurrent();
        }
        
        private void PushFunctionParameter() {
            IncreaseArity();
            OutputOperatorsUntilOpenParens();
            PushOpenParens();
        }

        private void PushNewArity() {
            arity.Push(1);
        }

        private void IncreaseArity() {
            arity.Push(arity.Pop() + 1);
        }

        private void PushOpenParens() {
            opstack.Push(ParensOpenToken);
        }

        private void PushCurrent() {
            opstack.Push(current);
        }

        private void OutputStackedWithHigherPrecedence() {
            while (HasStackedTokens() && OperatorLibrary.IsOperator(TopOfStack) && HasPrecedenceOverCurrent(TopOfStack)) {
                OutputTopOfStack();
            }
        }

        private void MakeCurrentUnary() {
            current = UnaryTokens[current];
        }

        private bool ReadNextToken() {
            previous = current;
            if (!tokens.MoveNext())
                return false;

            current = tokens.Current;
            return true;
        }
        
        private void OutputTopOfStack() {
            Output.Add(opstack.Pop());
        }

        private void DiscardTopOfStack() {
            opstack.Pop();
        }

        private void OutputCurrent() {
            Output.Add(current);
        }

        private void OutputSubExpression() {
            OutputOperatorsUntilOpenParens();
            if (!HasStackedTokens())
                return;

            if (FunctionLibrary.IsFunction(TopOfStack)) {
                OutputTopOfStack();
                Output.Add(arity.Pop().ToString());
            }
        }

        private void EnqueueAllOperators() {
            while (HasStackedTokens()) {
                OutputTopOfStack();
            }
        }

        private void OutputOperatorsUntilOpenParens() {
            while (TopOfStack != ParensOpenToken) {
                OutputTopOfStack();
            }
            DiscardTopOfStack();
        }
    }
}