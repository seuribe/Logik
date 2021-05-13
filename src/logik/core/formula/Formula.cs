using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {
    public class Formula {

        public readonly string Text;

        private readonly EvalNode evalNode;

        public Formula() {
            Text = "0";
            evalNode = new ValueNode(0);
        }

        public Formula(string formula) {
            Text = formula;
            var tokens = new Tokenizer(formula).Tokens;
            var postfix = new FormulaParser(tokens).Output;
            evalNode = new EvalNodeBuilder(postfix).Root;
        }

        public Value Eval(EvalContext context) => evalNode.Eval(context);
    }
}