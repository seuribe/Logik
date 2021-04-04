﻿using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {
    
    public delegate float OpFunction(List<EvalNode> children);

    public class TreeEvaluator : IEvaluator {
        private Dictionary<string, Cell> cells = new Dictionary<string, Cell>();
        private Dictionary<string, EvalNode> trees = new Dictionary<string, EvalNode>();
        private readonly EvalTreeBuilder builder;

        public TreeEvaluator() {
            builder = new EvalTreeBuilder(Lookup);
        }

        private float Lookup(string id) {
            return float.Parse(cells[id].Value);
        }

        public void Define(Cell cell) {
            cells[cell.Id] = cell;
            var tokens = Tokenizer.ProcessInput(cell.Formula);
            var parser = new FormulaParser(tokens);
            var tree = builder.BuildTree(parser.Output);
            trees[cell.Id] = tree;
        }

        public string Evaluate(Cell cell) {
            return trees[cell.Id].Eval().ToString();
        }

        public List<string> References(Cell cell) {
            var referenceNodes = trees[cell.Id].Collect( node => node is ExternalReferenceNode);
            return referenceNodes.Select( node => (node as ExternalReferenceNode).Name).ToList();
        }

        public void Undefine(Cell cell) {
            cells.Remove(cell.Id);
            trees.Remove(cell.Id);
        }
    }

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
        public readonly string Name;
        private readonly ValueLookup lookupFunction;

        public ExternalReferenceNode(string name, ValueLookup lookupFunction) {
            this.Name = name;
            this.lookupFunction = lookupFunction;
        }

        public override float Eval() {
            return lookupFunction(Name);
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