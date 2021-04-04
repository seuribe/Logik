using System.Collections.Generic;

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
    }

    public class OperatorNode : EvalNode {
        protected List<EvalNode> children = new List<EvalNode>();
        OpFunction opFunction;
        public int NumArguments { get ; private set; }

        public OperatorNode(OpFunction opFunction, int numArguments) {
            this.opFunction = opFunction;
            NumArguments = numArguments;
        }
        public void AddChild(EvalNode child) {
            children.Add(child);
        }
        public override float Eval() {
            return opFunction(children);
        }
    }

    public class EvalTreeBuilder : Constants {
        private static float PlusFunction(List<EvalNode> children) {
            return children[1].Eval() + children[0].Eval();
        }
        private static float MinusFunction(List<EvalNode> children) {
            return children[1].Eval() - children[0].Eval();
        }
        private static float MultiplyFunction(List<EvalNode> children) {
            return children[1].Eval() * children[0].Eval();
        }
        private static float DivideFunction(List<EvalNode> children) {
            return children[1].Eval() / children[0].Eval();
        }

        private static OperatorNode BuildOpNode(string op) {
            if (op == PlusToken)
                return new OperatorNode(PlusFunction, NumArguments(PlusToken));
            if (op == MinusToken)
                return new OperatorNode(MinusFunction, NumArguments(MinusToken));
            if (op == MultiplicationToken)
                return new OperatorNode(MultiplyFunction, NumArguments(MultiplicationToken));
            if (op == DivisionToken)
                return new OperatorNode(DivideFunction, NumArguments(DivisionToken));

            throw new System.Exception("Unknown operator " + op);
        }

        public static EvalNode BuildTree(List<string> postfix) {
            Stack<EvalNode> treeNodes = new Stack<EvalNode>();

            foreach (var token in postfix) {
                if (IsOperator(token)) {
                    var opNode = BuildOpNode(token);
                    for (int i = 0 ; i < opNode.NumArguments ; i++) {
                        opNode.AddChild(treeNodes.Pop());
                    }
                    treeNodes.Push(opNode);
                } else {
                    treeNodes.Push(new ValueNode(token));
                }
            }
            return treeNodes.Pop();
        }
    }
}