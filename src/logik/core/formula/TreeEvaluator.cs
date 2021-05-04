using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {
    public class TreeEvaluator : IEvaluator {

        public static string EvaluatorType = "default";

        private Dictionary<string, NumericCell> cells = new Dictionary<string, NumericCell>();
        private Dictionary<string, TabularCell> tcells = new Dictionary<string, TabularCell>();

        public string Type => EvaluatorType;

        private float Lookup(string id) => cells[id].Value;

        private float TabularLookup(string id, int row, int column) => tcells[id][row, column];

        public void Define(TabularCell tcell) {
            tcells[tcell.Name] = tcell;
        }

        public void Define(NumericCell cell) {
            cells[cell.Name] = cell;
            var tokens = new Tokenizer(cell.Formula).Tokens;
            var postfix = new FormulaParser(tokens).Output;
            var tree = new EvalTreeBuilder(postfix, Lookup, TabularLookup).Root;
            cell.EvalNode = tree;
        }

        public float Evaluate(NumericCell cell) {
            return cell.EvalNode.Eval();
        }

        public List<string> References(NumericCell cell) {
            var referenceNodes = cell.EvalNode.Collect(node => node is ExternalReferenceNode);
            return referenceNodes.Select(node => (node as ExternalReferenceNode).Name).ToList();
        }

        public void Undefine(NumericCell cell) {
            cells.Remove(cell.Name);
        }

        public void Rename(NumericCell cell, string newName) {
            cells[newName] = cells[cell.Name];
            cells.Remove(cell.Name);
        }
    }
}