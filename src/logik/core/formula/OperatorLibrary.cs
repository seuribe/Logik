using System.Collections.Generic;

namespace Logik.Core.Formula {

    public class Operator : Constants {
        public string Token { get; private set; }
        public int Precedence { get; private set; }
        public int Arguments { get; private set; }
        public bool LeftAssociative { get; private set; }
        public OpFunction Function { get; private set; }

        public Operator(string token, OpFunction function, int arguments, int precedence, bool leftAssociative = true) {
            Token = token;
            Function = function;
            Arguments = arguments;
            Precedence = precedence;
            LeftAssociative = leftAssociative;
        }
    }

    public class OperatorLibrary : Constants {
        public static Dictionary<string, Operator> Operators = new Dictionary<string, Operator> {
            { PlusToken, new Operator(PlusToken, children => children[0].Eval() + children[1].Eval(), 2, 2) },
            { MinusToken, new Operator(MinusToken, children => children[0].Eval() - children[1].Eval(), 2, 2) },
            { MultiplicationToken, new Operator(PlusToken, children => children[0].Eval() * children[1].Eval(), 2, 3) },
            { DivisionToken, new Operator(PlusToken, children => children[0].Eval() / children[1].Eval(), 2, 3) },
            { UnaryMinusToken, new Operator(PlusToken, children => -children[0].Eval(), 1, 4, false) }
        };

        public static bool IsOperator(string token) {
            return Operators.ContainsKey(token);
        }

    }
}