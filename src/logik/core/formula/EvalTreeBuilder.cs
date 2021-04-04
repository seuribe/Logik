﻿using System.Collections.Generic;

namespace Logik.Core.Formula {
    
    public delegate float OpFunction(List<EvalNode> children);

    public abstract class EvalNode {
        public abstract float Eval();
    }

    public class ValueNode : EvalNode {
        private readonly float value;

        public ValueNode(string value) {
            this.value = float.Parse(value);
        }

        public override float Eval() {
            return value;
        }
        public override string ToString() {
            return value.ToString();
        }
    }

    public delegate float ValueLookup(string name);

    public class ExternalReferenceNode : EvalNode {
        private readonly string name;
        private readonly ValueLookup lookupFunction;

        public ExternalReferenceNode(string name, ValueLookup lookupFunction) {
            this.name = name;
            this.lookupFunction = lookupFunction;
        }

        public override float Eval() {
            return lookupFunction(name);
        }
    }

    public class OperatorNode : EvalNode {
        protected List<EvalNode> children = new List<EvalNode>();
        OpFunction opFunction;
        public int NumArguments { get ; private set; }
        public string OpToken { get; private set; }

        public OperatorNode(string opToken, OpFunction opFunction, int numArguments) {
            this.OpToken = opToken;
            this.opFunction = opFunction;
            NumArguments = numArguments;
        }
        public void AddChild(EvalNode child) {
            children.Insert(0, child);
        }
        public override float Eval() {
            return opFunction(children);
        }
        public override string ToString() {
            if (NumArguments == 1) {
                return "(" + OpToken + " " + children[0] + ")";
            } else if (NumArguments == 2)
                return "(" + children[0] + " " + OpToken + " " + children[1] + ")";
            else
                return "(" + OpToken + string.Join(" ", children.ConvertAll( c => c.Eval().ToString()).ToArray()) + ")";
        }
    }

    public class EvalTreeBuilder : Constants {

        private static Dictionary<string, OpFunction> functions = new Dictionary<string, OpFunction> {
            { PlusToken, children => children[0].Eval() + children[1].Eval() },
            { MinusToken, children => children[0].Eval() - children[1].Eval() },
            { MultiplicationToken, children => children[0].Eval() * children[1].Eval() },
            { DivisionToken, children => children[0].Eval() / children[1].Eval() },
            { UnaryMinusToken, children => -children[0].Eval() },
        };

        private readonly ValueLookup lookupFunction;

        private static OperatorNode BuildOpNode(string opToken) {
            if (functions.TryGetValue(opToken, out OpFunction function)) {
                return new OperatorNode(opToken, function, NumArguments(opToken));
            }

            throw new System.Exception("Unknown operator " + opToken);
        }

        private static bool IsValue(string token) {
            return float.TryParse(token, out _);
        }

        public EvalTreeBuilder(ValueLookup lookupFunction) {
            this.lookupFunction = lookupFunction;
        }

        public EvalNode BuildTree(List<string> postfix) {
            Stack<EvalNode> treeNodes = new Stack<EvalNode>();

            foreach (var token in postfix) {
                if (IsOperator(token)) {
                    var opNode = BuildOpNode(token);
                    for (int i = 0 ; i < opNode.NumArguments ; i++) {
                        opNode.AddChild(treeNodes.Pop());
                    }
                    treeNodes.Push(opNode);
                } else if (IsValue(token)) {
                    treeNodes.Push(new ValueNode(token));
                } else {
                    treeNodes.Push(new ExternalReferenceNode(token, lookupFunction));
                }
            }
            return treeNodes.Pop();
        }
    }
}