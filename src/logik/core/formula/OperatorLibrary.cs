using System.Collections.Generic;

namespace Logik.Core.Formula {

    public class Operator : Constants {
        public string Token { get; private set; }
        public Precedence Precedence { get; private set; }
        public int Arguments { get; private set; }
        public bool LeftAssociative { get; private set; }
        public OpFunction Function { get; private set; }

        public Operator(string token, OpFunction function, int arguments, Precedence precedence, bool leftAssociative = true) {
            Token = token;
            Function = function;
            Arguments = arguments;
            Precedence = precedence;
            LeftAssociative = leftAssociative;
        }

        public bool HasPrecedenceOver(Operator op) {
            return Precedence > op.Precedence || (LeftAssociative && Precedence == op.Precedence);
        }
    }

    public enum Precedence {
        Comparison = 1,
        Addition = 2,
        Multiplication = 3,
        Unary = 4
    }

    public class OperatorLibrary : Constants {

        public static Dictionary<string, Operator> Operators = new Dictionary<string, Operator>();

        static OperatorLibrary() {
            Add(new Operator(LessThanToken, (c, ctx) => c[0].Eval(ctx).AsFloat < c[1].Eval(ctx).AsFloat, 2, Precedence.Comparison));
            Add(new Operator(LessOrEqualToken, (c, ctx) => c[0].Eval(ctx).AsFloat <= c[1].Eval(ctx).AsFloat, 2, Precedence.Comparison));
            Add(new Operator(GreaterThanToken, (c, ctx) => c[0].Eval(ctx).AsFloat > c[1].Eval(ctx).AsFloat, 2, Precedence.Comparison));
            Add(new Operator(GreaterOrEqualToken, (c, ctx) => c[0].Eval(ctx).AsFloat >= c[1].Eval(ctx).AsFloat, 2, Precedence.Comparison));
            Add(new Operator(EqualToken, (c, ctx) => c[0].Eval(ctx).AsFloat == c[1].Eval(ctx).AsFloat, 2, Precedence.Comparison));
            Add(new Operator(NotEqualToken, (c, ctx) => c[0].Eval(ctx).AsFloat != c[1].Eval(ctx).AsFloat, 2, Precedence.Comparison));

            Add(new Operator(NotToken, (c, ctx) => !c[0].Eval(ctx).AsBool, 1, Precedence.Unary, false));

            Add(new Operator(UnaryMinusToken, (c, ctx) => -c[0].Eval(ctx).AsFloat, 1, Precedence.Unary, false));
            Add(new Operator(UnaryPlusToken, (c, ctx) => c[0].Eval(ctx).AsFloat, 1, Precedence.Unary, false));

            Add(new Operator(PlusToken, (c, ctx) => c[0].Eval(ctx).AsFloat + c[1].Eval(ctx).AsFloat, 2, Precedence.Addition));
            Add(new Operator(MinusToken, (c, ctx) => c[0].Eval(ctx).AsFloat - c[1].Eval(ctx).AsFloat, 2, Precedence.Addition));
            Add(new Operator(MultiplicationToken, (c, ctx) => c[0].Eval(ctx).AsFloat * c[1].Eval(ctx).AsFloat, 2, Precedence.Multiplication));
            Add(new Operator(DivisionToken, (c, ctx) => c[0].Eval(ctx).AsFloat / c[1].Eval(ctx).AsFloat, 2, Precedence.Multiplication));
        }

        static void Add(Operator op) {
            Operators.Add(op.Token, op);
        }

        public static bool IsOperator(string token) {
            return Operators.ContainsKey(token);
        }

    }
}