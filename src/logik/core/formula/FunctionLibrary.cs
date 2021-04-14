using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {

    public class FunctionLibrary : Constants {

        public static Dictionary<string, OpFunction> Functions = new Dictionary<string, OpFunction> {
            { "max", Max },
            { "min", Min },
            { "average", Average },
        };

        public static float Max(List<EvalNode> parameters) {
            var values = parameters.Select( c => c.Eval() );
            return values.Aggregate(System.Math.Max);
        }

        public static float Min(List<EvalNode> parameters) {
            var values = parameters.Select( c => c.Eval() );
            return values.Aggregate(System.Math.Min);
        }

        public static float Average(List<EvalNode> parameters) {
            var values = parameters.Select( c => c.Eval() );
            return values.Aggregate( (a, b) => a + b) / values.Count();
        }

        public static bool IsFunction(string token) {
            return Functions.ContainsKey(token);
        }

    }
}