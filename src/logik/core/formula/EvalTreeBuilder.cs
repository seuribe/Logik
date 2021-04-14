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
        public EvalNode Root { get; private set; }

        private readonly Stack<EvalNode> treeNodes = new Stack<EvalNode>();

        public EvalTreeBuilder(List<string> postfix, ValueLookup lookupFunction) {
            var tokens = new Queue<string>(postfix);
            while (tokens.Count > 0) {
                var token = tokens.Dequeue();

                if (OperatorLibrary.IsOperator(token)) {
                    PushOperator(token);
                } else if (IsFunction(token)) {
                    var arity = int.Parse(tokens.Dequeue());
                    PushFunction(token, arity);
                } else if (IsValue(token)) {
                    PushValue(token);
                } else {
                    PushExternalReference(token, lookupFunction);
                }
            }
            Root = treeNodes.Pop();
        }

        private void PushExternalReference(string token, ValueLookup lookupFunction) {
            treeNodes.Push(new ExternalReferenceNode(token, lookupFunction));
        }

        private void PushValue(string token) {
            treeNodes.Push(new ValueNode(token));
        }

        private void PushOperator(string opToken) {
            if (OperatorLibrary.Operators.TryGetValue(opToken, out Operator op)) {
                var opNode = new OperatorNode(op);
                for (int i = 0 ; i < op.Arguments ; i++)
                    opNode.AddChild(treeNodes.Pop());

                treeNodes.Push(opNode);
            } else {
                throw new System.Exception("Unknown operator " + opToken);
            }
        }
        
        private void PushFunction(string funcToken, int arity) {
            if (FunctionLibrary.Functions.TryGetValue(funcToken, out OpFunction function)) {
                var funcNode = new FunctionNode(function);
                for (int i = 0 ; i < arity ; i++)
                    funcNode.AddChild(treeNodes.Pop());

                treeNodes.Push(funcNode);
            } else {
                throw new System.Exception("Unknown function " + funcToken);
            }
        }

        private static bool IsValue(string token) {
            return float.TryParse(token, out _);
        }

        private static bool IsFunction(string token) {
            return FunctionLibrary.Functions.Keys.Contains(token);
        }

    }
}