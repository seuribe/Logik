using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {

    public delegate Value OpFunction(List<EvalNode> children, EvalContext context);
    public delegate bool NodePredicate(EvalNode node);

    public class EvalNodeBuilder : Constants {
        public EvalNode Root { get; private set; }

        private readonly Stack<EvalNode> treeNodes = new Stack<EvalNode>();

        public static EvalNode Build(string formula) {
            var tokens = new Tokenizer(formula).Tokens;
            var postfix = new FormulaParser(tokens).Output;
            return new EvalNodeBuilder(postfix).Root;
        }

        public EvalNodeBuilder(List<string> postfix) {
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
                } else if (IsBool(token)) {
                    PushBool(token);
                } else if (IsValue(token)) {
                    PushValue(token);
                } else if (IsString(token)) {
                    PushString(token);
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

        private void PushString(string token) {
            var str = token.Substring(1, token.Length - 2);
            treeNodes.Push(new ValueNode(str));
        }

        private void PushBool(string token) {
            var value = (token.ToLower() != "false");
            treeNodes.Push(new ValueNode(value));
        }

        private void PushValue(string token) {
            treeNodes.Push(new ValueNode(token));
        }

        private void PushOperator(string opToken) {
            if (OperatorLibrary.Operators.TryGetValue(opToken, out Operator op)) {
                AddChildrenAndPush(new OperatorNode(op), op.Arguments);
            } else {
                throw new System.Exception("Unknown operator " + opToken);
            }
        }
        
        private void PushFunction(string funcToken, int arity) {
            if (FunctionLibrary.Functions.TryGetValue(funcToken, out OpFunction function)) {
                AddChildrenAndPush(new FunctionNode(function), arity);
            } else {
                throw new System.Exception("Unknown function " + funcToken);
            }
        }

        private void AddChildrenAndPush(BranchNode node, int numChildren) {
            for (int i = 0 ; i < numChildren ; i++)
                node.AddChild(treeNodes.Pop());
            treeNodes.Push(node);
        }

        private static bool IsValue(string token) {
            return float.TryParse(token, out _);
        }

        private static bool IsString(string token) {
            return token.StartsWith(QuoteToken) && token.EndsWith(QuoteToken);
        }

        private static bool IsFunction(string token) {
            return FunctionLibrary.Functions.Keys.Contains(token);
        }

    }
}