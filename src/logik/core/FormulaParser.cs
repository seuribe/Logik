using System.Collections.Generic;

namespace Logik.Core {
    // http://www.wcipeg.com/wiki/Shunting_yard_algorithm
    // http://math.oxford.emory.edu/site/cs171/shuntingYardAlgorithm/

    public class FormulaParser {

        private static string Operators = "+-/*";
        private static int[] Precedence = {2, 2, 3, 3};
        private static int[] Arguments = {2, 2, 2, 2 };

        private static bool HigherPrecedence(string op1, string op2) {
            return Precedence[Operators.IndexOf(op1)] > Precedence[Operators.IndexOf(op2)];
        }

        public static int NumArguments(string token) {
            return Arguments[Operators.IndexOf(token)];
        }

        public static bool IsOperator(string token) {
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
}