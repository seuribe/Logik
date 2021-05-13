namespace Logik.Core.Formula {
    public class Formula {

        public readonly string Text;

        private readonly EvalNode evalNode;

        public Formula() {
            Text = "0";
            evalNode = new ValueNode(0);
        }

        public Formula(string formula, ValueLookup valueLookup, TabularLookup tabularLookup) {
            Text = formula;
            var tokens = new Tokenizer(formula).Tokens;
            var postfix = new FormulaParser(tokens).Output;
            evalNode = new EvalTreeBuilder(postfix, valueLookup, tabularLookup).Root;
        }

        public Value Eval() => evalNode.Eval();

    }
}