using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {

    public delegate Value OpFunction(List<EvalNode> children, EvalContext context);
    public delegate bool NodePredicate(EvalNode node);

    public class EvalContext {
        public ValueLookup Lookup { get; }
        public TabularLookup TabularLookup { get; }

        public EvalContext(ValueLookup lookup, TabularLookup tabularLookup) {
            Lookup = lookup;
            TabularLookup = tabularLookup;
        }

        public static EvalContext EmptyContext = new EvalContext( name => new ValueNode(0), (name, row, column) => 0);
    }

    public interface Evaluable {
        Value Eval(EvalContext context);
    }

    public abstract class EvalNode : Evaluable {
        public abstract Value Eval(EvalContext context);

        public virtual IEnumerable<EvalNode> Collect(NodePredicate predicate) {
            if (predicate(this))
                return new List<EvalNode>() { this };
            else
                return Enumerable.Empty<EvalNode>();
        }
    }

    public class ValueNode : EvalNode {
        private readonly Value value;

        public ValueNode(float value) {
            this.value = value;
        }

        public ValueNode(string value) {
            this.value = float.Parse(value);
        }

        public override Value Eval(EvalContext context) {
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

        public CellReferenceNode(string name) {
            this.Name = name;
        }

        public override Value Eval(EvalContext context) {
            return context.Lookup(Name).Eval(context);
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

        public override Value Eval(EvalContext context) {
            return Function(children, context);
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

        public override Value Eval(EvalContext context) {
            return op.Function(children, context);
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

        public TabularReferenceNode(string name, EvalNode row, EvalNode column) {
            this.Name = name;
            this.Row = row;
            this.Column = column;
        }

        public override Value Eval(EvalContext context) {
            return context.TabularLookup(Name, (int)Row.Eval(context), (int)Column.Eval(context));
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

        public EvalTreeBuilder(List<string> postfix) {
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
                    PushTableAccess();
                } else if (IsValue(token)) {
                    PushValue(token);
                } else {
                    PushCellReferenceNode(token);
                }
            }
            Root = treeNodes.Pop();
        }

        private void PushTableAccess() {
            var column = treeNodes.Pop();
            var row = treeNodes.Pop();
            var cellName = (treeNodes.Pop() as ExternalReferenceNode).Name;
            var lookupNode = new TabularReferenceNode(cellName, row, column);

            treeNodes.Push(lookupNode);
        }

        private void PushCellReferenceNode(string token) {
            treeNodes.Push(new CellReferenceNode(token));
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