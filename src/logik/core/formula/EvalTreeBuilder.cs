using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {

    public delegate float OpFunction(List<EvalNode> children);

    public delegate bool NodePredicate(EvalNode node);

    public abstract class EvalNode {
        public abstract float Eval();
        public virtual IEnumerable<EvalNode> Collect(NodePredicate predicate) {
            if (predicate(this))
                return new List<EvalNode>() { this };
            else
                return Enumerable.Empty<EvalNode>();
        }
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
        public string Name { get; set; }
        private readonly ValueLookup lookupFunction;

        public ExternalReferenceNode(string name, ValueLookup lookupFunction) {
            this.Name = name;
            this.lookupFunction = lookupFunction;
        }

        public override float Eval() {
            return lookupFunction(Name);
        }
    }


    public class FunctionNode : EvalNode {
        protected List<EvalNode> children = new List<EvalNode>();
        private readonly OpFunction Function;

        public FunctionNode(OpFunction function) {
            Function = function;
        }
        
        public void AddChild(EvalNode child) {
            children.Insert(0, child);
        }

        public override float Eval() {
            return Function(children);
        }
        
        public override IEnumerable<EvalNode> Collect(NodePredicate predicate) {
            List<EvalNode> ret = new List<EvalNode>();
            if (predicate(this))
                ret.Add(this);

            foreach (var child in children)
                ret.AddRange(child.Collect(predicate));

            return ret;
        }
    }

    public class OperatorNode : EvalNode {
        protected List<EvalNode> children = new List<EvalNode>();
        private readonly Operator op;

        public OperatorNode(Operator op) {
            this.op = op;
        }

        public void AddChild(EvalNode child) {
            children.Insert(0, child);
        }

        public override float Eval() {
            return op.Function(children);
        }

        public override IEnumerable<EvalNode> Collect(NodePredicate predicate) {
            List<EvalNode> ret = new List<EvalNode>();
            if (predicate(this))
                ret.Add(this);

            foreach (var child in children)
                ret.AddRange(child.Collect(predicate));

            return ret;
        }
    }

    public class EvalTreeBuilder : Constants {
        private readonly ValueLookup lookupFunction;

        private static void BuildOpNode(string opToken, Stack<EvalNode> treeNodes) {
            if (OperatorLibrary.Operators.TryGetValue(opToken, out Operator op)) {
                var opNode = new OperatorNode(op);
                for (int i = 0 ; i < op.Arguments ; i++)
                    opNode.AddChild(treeNodes.Pop());

                treeNodes.Push(opNode);
            } else {
                throw new System.Exception("Unknown operator " + opToken);
            }
        }
        
        private static FunctionNode BuildFunctionNode(string funcToken) {
            if (FunctionLibrary.Functions.TryGetValue(funcToken, out OpFunction function)) {
                return new FunctionNode(function);
            }

            throw new System.Exception("Unknown function " + funcToken);
        }

        private static bool IsValue(string token) {
            return float.TryParse(token, out _);
        }

        private static bool IsFunction(string token) {
            return FunctionLibrary.Functions.Keys.Contains(token);
        }

        public EvalTreeBuilder(ValueLookup lookupFunction) {
            this.lookupFunction = lookupFunction;
        }

        public EvalNode BuildTree(List<string> postfix) {
            Stack<EvalNode> treeNodes = new Stack<EvalNode>();
            var tokens = new Queue<string>(postfix);
            while (tokens.Count > 0) {
                var token = tokens.Dequeue();

                if (OperatorLibrary.IsOperator(token)) {
                    BuildOpNode(token, treeNodes);
                } else if (IsFunction(token)) {
                    var arity = int.Parse(tokens.Dequeue());
                    var funcNode = BuildFunctionNode(token);
                    for (int i = 0 ; i < arity ; i++) {
                        funcNode.AddChild(treeNodes.Pop());
                    }
                    treeNodes.Push(funcNode);
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