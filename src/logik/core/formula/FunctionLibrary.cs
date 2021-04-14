using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {
    public class FunctionLibrary {

        public static float Max(List<EvalNode> parameters) {
            var values = parameters.Select( c => c.Eval() );
            return values.Aggregate(System.Math.Max);
        }

        public static float Min(List<EvalNode> parameters) {
            var values = parameters.Select( c => c.Eval() );
            return values.Aggregate(System.Math.Min);
        }
    }
}