
namespace Logik.Core.Formula {

    public class EvalNodeBuilder {
        private readonly ValueLookup valueLookup;
        private readonly TabularLookup tabularLookup;

        public EvalNodeBuilder(ValueLookup valueLookup, TabularLookup tabularLookup) {
            this.valueLookup = valueLookup;
            this.tabularLookup = tabularLookup;
        }

        public EvalNode Build(string formula) {
            var tokens = new Tokenizer(formula).Tokens;
            var postfix = new FormulaParser(tokens).Output;
            return new EvalTreeBuilder(postfix, valueLookup, tabularLookup).Root;
        }
    }
}