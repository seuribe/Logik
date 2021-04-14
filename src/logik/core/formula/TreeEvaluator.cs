using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {
    public class TreeEvaluator : IEvaluator {

        public static string EvaluatorType = "default";

        private Dictionary<string, NumericCell> cells = new Dictionary<string, NumericCell>();
        private Dictionary<NumericCell, EvalNode> trees = new Dictionary<NumericCell, EvalNode>();

        public string Type => EvaluatorType;


        private float Lookup(string id) {
            return cells[id].Value;
        }

        public void Define(NumericCell cell) {
            cells[cell.Name] = cell;
            var tokens = new Tokenizer(cell.Formula).Tokens;
            var postfix = new FormulaParser(tokens).Output;
            var tree = new EvalTreeBuilder(postfix, Lookup).Root;
            trees[cell] = tree;
        }

        public float Evaluate(NumericCell cell) {
            return trees[cell].Eval();
        }

        public List<string> References(NumericCell cell) {
            var referenceNodes = trees[cell].Collect( node => node is ExternalReferenceNode);
            return referenceNodes.Select( node => (node as ExternalReferenceNode).Name).ToList();
        }

        public void Undefine(NumericCell cell) {
            cells.Remove(cell.Name);
            trees.Remove(cell);
        }

        public void Rename(NumericCell cell, string newName) {
            cells[newName] = cells[cell.Name];
            cells.Remove(cell.Name);
        }
    }
}