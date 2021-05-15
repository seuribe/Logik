using System.Collections.Generic;
using System.Linq;

namespace Logik.Core.Formula {

    public class FunctionLibrary : Constants {

        public static Dictionary<string, OpFunction> Functions = new Dictionary<string, OpFunction> {
            { "max", Max },
            { "min", Min },
            { "average", Average },
            { "concat", Concat },
            { "substring", Substring },
            { "indexof", IndexOf },
            { "if", If },
        };

        public static Value Max(List<EvalNode> parameters, EvalContext context) {
            var values = parameters.Select( c => c.Eval(context) );
            return values.Aggregate(float.MinValue, (a, b) => System.Math.Max(a, b.AsFloat));
        }

        public static Value Min(List<EvalNode> parameters, EvalContext context) {
            var values = parameters.Select( c => c.Eval(context) );
            return values.Aggregate(float.MaxValue, (a, b) => System.Math.Min(a, b));
        }

        public static Value Average(List<EvalNode> parameters, EvalContext context) {
            var values = parameters.Select( c => c.Eval(context) );
            return values.Aggregate(0f, (a, b) => a + b) / values.Count();
        }

        public static Value Concat(List<EvalNode> parameters, EvalContext context) {
            var strings = parameters.Select( node => node.Eval(context).AsString);
            return string.Concat(strings);
        }

        public static Value Substring(List<EvalNode> parameters, EvalContext context) {
            var str = parameters[0].Eval(context).AsString;
            int start = (parameters.Count > 1) ? parameters[1].Eval(context).AsInt : 0;
            int length = (parameters.Count > 2) ? parameters[2].Eval(context).AsInt : (str.Length - start);
            
            return str.Substring(start, length);
        }

        public static Value IndexOf(List<EvalNode> parameters, EvalContext context) {
            var str = parameters[0].Eval(context).AsString;
            var substr = parameters[1].Eval(context).AsString;
            
            return str.IndexOf(substr);
        }

        public static Value If(List<EvalNode> parameters, EvalContext context) {
            if (parameters[0].Eval(context).AsBool)
                return parameters[1].Eval(context);
            else
                return parameters[2].Eval(context);
        }

        public static bool IsFunction(string token) {
            return Functions.ContainsKey(token);
        }

    }
}