using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {

    public delegate Value OpFunction(List<EvalNode> children, EvalContext context);
    public delegate bool NodePredicate(EvalNode node);

    public class EvalTreeBuilder : Constants {
        public EvalNode Root { get; private set; }

        private readonly Stack<EvalNode> treeNodes = new Stack<EvalNode>();

        public EvalTreeBuilder(List<string> postfix) {
            var tokens = new Queue<string>(postfix);
            while (tokens.Count > 0) {
                var token = tokens.Dequeue();

                if (OperatorLibrary.IsOperator(token)) {
                    PushOperator(token);
                } else if (IsFunction(token)) {
                    var arity = int.Parse(tokens.Dequeue());
                    PushFunction(token, arity);
                } else if (IsTableAccess(token)) {
                    tokens.Dequeue(); // remove and ignore arity
                    PushTableAccess();
                } else if (IsValue(token)) {
                    PushValue(token);
                } else {
                    PushCellReferenceNode(token);
                }
            }
            Root = treeNodes.Pop();
        }

        private void PushTableAccess() {
            var column = treeNodes.Pop();
            var row = treeNodes.Pop();
            var cellName = (treeNodes.Pop() as ExternalReferenceNode).Name;
            var lookupNode = new TabularReferenceNode(cellName, row, column);

            treeNodes.Push(lookupNode);
        }

        private void PushCellReferenceNode(string token) {
            treeNodes.Push(new CellReferenceNode(token));
        }

        private void PushValue(string token) {
            treeNodes.Push(new ValueNode(token));
        }

        private void PushOperator(string opToken) {
            if (OperatorLibrary.Operators.TryGetValue(opToken, out Operator op)) {
                var opNode = new OperatorNode(op);
                for (int i = 0 ; i < op.Arguments ; i++)
                    opNode.AddChild(treeNodes.Pop());

                treeNodes.Push(opNode);
            } else {
                throw new System.Exception("Unknown operator " + opToken);
            }
        }
        
        private void PushFunction(string funcToken, int arity) {
            if (FunctionLibrary.Functions.TryGetValue(funcToken, out OpFunction function)) {
                var funcNode = new FunctionNode(function);
                for (int i = 0 ; i < arity ; i++)
                    funcNode.AddChild(treeNodes.Pop());

                treeNodes.Push(funcNode);
            } else {
                throw new System.Exception("Unknown function " + funcToken);
            }
        }

        private static bool IsValue(string token) {
            return float.TryParse(token, out _);
        }

        private static bool IsFunction(string token) {
            return FunctionLibrary.Functions.Keys.Contains(token);
        }

    }
}