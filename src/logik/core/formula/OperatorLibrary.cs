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
            { PlusToken, new Operator(PlusToken,
                (children, context) => children[0].Eval(context).AsFloat + children[1].Eval(context).AsFloat,
                2, 2) },
            { MinusToken, new Operator(MinusToken,
                (children, context) => children[0].Eval(context).AsFloat - children[1].Eval(context).AsFloat,
                2, 2) },
            { MultiplicationToken, new Operator(MultiplicationToken,
                (children, context) => children[0].Eval(context).AsFloat * children[1].Eval(context).AsFloat,
                2, 3) },
            { DivisionToken, new Operator(DivisionToken,
                (children, context) => children[0].Eval(context).AsFloat / children[1].Eval(context).AsFloat,
                2, 3) },
            { UnaryMinusToken, new Operator(MinusToken,
                (children, context) => -children[0].Eval(context).AsFloat, 1, 4, false) },
        };

        public static bool IsOperator(string token) {
            return Operators.ContainsKey(token);
        }

    }
}