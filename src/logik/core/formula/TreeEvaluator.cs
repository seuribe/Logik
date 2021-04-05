using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {
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
            cells[cell.Name] = cell;
            var tokens = Tokenizer.ProcessInput(cell.Formula);
            var parser = new FormulaParser(tokens);
            var tree = builder.BuildTree(parser.Output);
            trees[cell.Name] = tree;
        }

        public string Evaluate(Cell cell) {
            return trees[cell.Name].Eval().ToString();
        }

        public List<string> References(Cell cell) {
            var referenceNodes = trees[cell.Name].Collect( node => node is ExternalReferenceNode);
            return referenceNodes.Select( node => (node as ExternalReferenceNode).Name).ToList();
        }

        public void Undefine(Cell cell) {
            cells.Remove(cell.Name);
            trees.Remove(cell.Name);
        }

        public void Rename(Cell cell, string newName) {
            cells[newName] = cells[cell.Name];
            cells.Remove(cell.Name);
            trees[newName] = trees[cell.Name];
            trees.Remove(cell.Name);
        }
    }
}