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
}