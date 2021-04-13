using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {
    public class TreeEvaluator : IEvaluator {

        public static string EvaluatorType = "default";

        private Dictionary<string, Cell> cells = new Dictionary<string, Cell>();
        private Dictionary<Cell, EvalNode> trees = new Dictionary<Cell, EvalNode>();
        private readonly EvalTreeBuilder builder;

        public string Type => EvaluatorType;

        public TreeEvaluator() {
            builder = new EvalTreeBuilder(Lookup);
        }

        private float Lookup(string id) {
            return float.Parse(cells[id].Value);
        }

        public void Define(Cell cell) {
            cells[cell.Name] = cell;
            var tokens = new Tokenizer(cell.Formula).Tokens;
            var parser = new FormulaParser(tokens);
            var tree = builder.BuildTree(parser.Output);
            trees[cell] = tree;
        }

        public string Evaluate(Cell cell) {
            return trees[cell].Eval().ToString();
        }

        public List<string> References(Cell cell) {
            var referenceNodes = trees[cell].Collect( node => node is ExternalReferenceNode);
            return referenceNodes.Select( node => (node as ExternalReferenceNode).Name).ToList();
        }

        public void Undefine(Cell cell) {
            cells.Remove(cell.Name);
            trees.Remove(cell);
        }

        public void Rename(Cell cell, string newName) {
            cells[newName] = cells[cell.Name];
            cells.Remove(cell.Name);
        }
    }
}