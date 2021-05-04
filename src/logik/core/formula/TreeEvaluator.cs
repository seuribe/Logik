using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {

    public class EvalNodeBuilder {
        public EvalNode Build(string formula, ValueLookup valueLookup, TabularLookup tabularLookup) {
            var tokens = new Tokenizer(formula).Tokens;
            var postfix = new FormulaParser(tokens).Output;
            return new EvalTreeBuilder(postfix, valueLookup, tabularLookup).Root;
        }
    }

    public class TreeEvaluator : IEvaluator {

        public static string EvaluatorType = "default";

        public string Type => EvaluatorType;

        public void Define(TabularCell tcell) {
        }

        public void Define(NumericCell cell) {
        }

        public float Evaluate(NumericCell cell) {
            return 0;
        }

        public List<string> References(NumericCell cell) {
            var referenceNodes = cell.EvalNode.Collect(node => node is ExternalReferenceNode);
            return referenceNodes.Select(node => (node as ExternalReferenceNode).Name).ToList();
        }

        public void Undefine(NumericCell cell) {
        }

        public void Rename(NumericCell cell, string newName) {
        }
    }
}