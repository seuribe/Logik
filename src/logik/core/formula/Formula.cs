using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {
    public class Formula : IEvaluable {

        public readonly string Text;

        private readonly EvalNode evalNode;

        public Formula() {
            Text = "0";
            evalNode = new ValueNode(0);
        }

        public Formula(string formula) {
            Text = formula;
            evalNode = EvalNodeBuilder.Build(formula);
        }

        public Value Eval(EvalContext context) => evalNode.Eval(context);

        public IEnumerable<string> GetReferencedNames() {
            var referenceNodes = evalNode.Collect(node => node is ExternalReferenceNode);
            return referenceNodes.Select(node => (node as ExternalReferenceNode).Name).ToList();
        }
    }
}