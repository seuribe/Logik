using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {
    public abstract class EvalNode : IEvaluable {
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

}