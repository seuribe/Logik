using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {

    public class FunctionLibrary : Constants {

        public static Dictionary<string, OpFunction> Functions = new Dictionary<string, OpFunction> {
            { "max", Max },
            { "min", Min },
            { "average", Average },
            { "concat", Concat }
        };

        public static Value Max(List<EvalNode> parameters) {
            var values = parameters.Select( c => c.Eval() );
            return values.Aggregate(float.MinValue, (a, b) => System.Math.Max(a, b.AsFloat));
        }

        public static Value Min(List<EvalNode> parameters) {
            var values = parameters.Select( c => c.Eval() );
            return values.Aggregate(float.MaxValue, (a, b) => System.Math.Min(a, b));
        }

        public static Value Average(List<EvalNode> parameters) {
            var values = parameters.Select( c => c.Eval() );
            return values.Aggregate(0f, (a, b) => a + b) / values.Count();
        }

        public static Value Concat(List<EvalNode> parameters) {
            var strings = parameters.Select( node => node.Eval().AsString);
            return string.Concat(strings);
        }

        public static bool IsFunction(string token) {
            return Functions.ContainsKey(token);
        }

    }
}