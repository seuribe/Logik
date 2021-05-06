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

        public ValueNode(float value) {
            this.value = value;
        }

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

    public abstract class ExternalReferenceNode : EvalNode {
        public string Name { get; set; }
    }


    public class CellReferenceNode : ExternalReferenceNode {
        private readonly ValueLookup lookupFunction;

        public CellReferenceNode(string name, ValueLookup lookupFunction) {
            this.Name = name;
            this.lookupFunction = lookupFunction;
        }

        public override float Eval() {
            return lookupFunction(Name).Eval();
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

    public class TabularReferenceNode : ExternalReferenceNode {
        public EvalNode Row { get; set; }
        public EvalNode Column { get; set; }
        private readonly TabularLookup lookupFunction;

        public TabularReferenceNode(string name, EvalNode row, EvalNode column, TabularLookup lookupFunction) {
            this.Name = name;
            this.Row = row;
            this.Column = column;
            this.lookupFunction = lookupFunction;
        }

        public override float Eval() {
            return lookupFunction(Name, (int)Row.Eval(), (int)Column.Eval());
        }

        public override IEnumerable<EvalNode> Collect(NodePredicate predicate) {
            List<EvalNode> ret = new List<EvalNode>();
            if (predicate(this))
                ret.Add(this);
            if (predicate(Row))
                ret.Add(Row);
            if (predicate(Column))
                ret.Add(Column);

            return ret;
        }

    }

    public class EvalTreeBuilder : Constants {
        public EvalNode Root { get; private set; }

        private readonly Stack<EvalNode> treeNodes = new Stack<EvalNode>();

        public EvalTreeBuilder(List<string> postfix, ValueLookup lookupFunction, TabularLookup tabularLookup) {
            var tokens = new Queue<string>(postfix);
            while (tokens.Count > 0) {
                var token = tokens.Dequeue();

                if (OperatorLibrary.IsOperator(token)) {
                    PushOperator(token);
                } else if (IsFunction(token)) {
                    var arity = int.Parse(tokens.Dequeue());
                    PushFunction(token, arity);
                } else if (IsTableAccess(token)) {
                    tokens.Dequeue(); // remove and ignore arity
                    PushTableAccess(tabularLookup);
                } else if (IsValue(token)) {
                    PushValue(token);
                } else {
                    PushCellReferenceNode(token, lookupFunction);
                }
            }
            Root = treeNodes.Pop();
        }

        private void PushTableAccess(TabularLookup tabularLookup) {
            var column = treeNodes.Pop();
            var row = treeNodes.Pop();
            var cellName = (treeNodes.Pop() as ExternalReferenceNode).Name;
            var lookupNode = new TabularReferenceNode(cellName, row, column, tabularLookup);

            treeNodes.Push(lookupNode);
        }

        private void PushCellReferenceNode(string token, ValueLookup lookupFunction) {
            treeNodes.Push(new CellReferenceNode(token, lookupFunction));
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