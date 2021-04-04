using System.Collections.Generic;

namespace Logik.Core.Formula {
    // http://www.wcipeg.com/wiki/Shunting_yard_algorithm
    // http://math.oxford.emory.edu/site/cs171/shuntingYardAlgorithm/

    public class FormulaParser : Constants {


        public static List<string> ToPostfix(List<string> tokens) {
            Stack<string> opstack = new Stack<string>();
            Queue<string> postfix = new Queue<string>();

            foreach (var token in tokens) {
                if (token == ParensOpenToken) {
                    opstack.Push(token);
                } else if (token == ParensCloseToken) {
                    var op = opstack.Pop();
                    while (op != ParensOpenToken) {
                        postfix.Enqueue(op);
                        op = opstack.Pop();
                    }
                } else if (IsOperator(token)) {
                    if (opstack.Count == 0) {
                        opstack.Push(token);
                    } else {
                        var prev = opstack.Peek();
                        if (IsOperator(prev) && HigherPrecedence(prev, token)) {
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
}