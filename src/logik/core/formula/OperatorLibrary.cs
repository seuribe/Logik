using System;
using System.Collections.Generic;

namespace Logik.Core.Formula {

    public class Operator {
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

    public delegate Value BinaryFunction(Value first, Value second);
    public delegate Value UnaryFunction(Value val);

    public class BinaryOperator : Operator {
        public BinaryOperator(string token, BinaryFunction function, Precedence precedence, bool leftAssociative = true) :
            base(token, (c, ctx) => function(c[0].Eval(ctx), c[1].Eval(ctx)), 2, precedence, leftAssociative) {
        }
    }

    public class UnaryOperator : Operator {
        public UnaryOperator(string token, UnaryFunction function, Precedence precedence = Precedence.Unary, bool leftAssociative = false) :
            base(token, (c, ctx) => function(c[0].Eval(ctx)), 1, precedence, leftAssociative) { }
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
            Add(new BinaryOperator(LessThanToken, (f, s) => f.AsFloat < s.AsFloat, Precedence.Comparison));
            Add(new BinaryOperator(LessOrEqualToken, (f, s) => f.AsFloat <= s.AsFloat, Precedence.Comparison));
            Add(new BinaryOperator(GreaterThanToken, (f, s) => f.AsFloat > s.AsFloat, Precedence.Comparison));
            Add(new BinaryOperator(GreaterOrEqualToken, (f, s) => f.AsFloat >= s.AsFloat, Precedence.Comparison));
            Add(new BinaryOperator(EqualToken, (f, s) => f.AsFloat == s.AsFloat, Precedence.Comparison));
            Add(new BinaryOperator(NotEqualToken, (f, s) => f.AsFloat != s.AsFloat, Precedence.Comparison));

            Add(new UnaryOperator(NotToken, val => !(val.AsBool)));
            Add(new UnaryOperator(UnaryMinusToken, val => -(val.AsFloat)));
            Add(new UnaryOperator(UnaryPlusToken, val => val));

            Add(new BinaryOperator(PlusToken, (f, s) => f.AsFloat + s.AsFloat, Precedence.Addition));
            Add(new BinaryOperator(MinusToken, (f, s) => f.AsFloat - s.AsFloat, Precedence.Addition));
            Add(new BinaryOperator(MultiplicationToken, (f, s) => f.AsFloat * s.AsFloat, Precedence.Multiplication));
            Add(new BinaryOperator(DivisionToken, (f, s) => f.AsFloat / s.AsFloat, Precedence.Multiplication));
        }

        static void Add(Operator op) {
            Operators.Add(op.Token, op);
        }

        public static bool IsOperator(string token) {
            return Operators.ContainsKey(token);
        }

    }
}