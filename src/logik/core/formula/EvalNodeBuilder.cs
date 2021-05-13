
namespace Logik.Core.Formula {

    public class EvalNodeBuilder {
        public static EvalNode Build(string formula) {
            var tokens = new Tokenizer(formula).Tokens;
            var postfix = new FormulaParser(tokens).Output;
            return new EvalTreeBuilder(postfix).Root;
        }
    }
}