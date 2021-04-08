using Logik.Core.Formula;
using System;

namespace Logik.Core {
    public class EvaluatorProvider {

        public static readonly string DefaultEvaluatorType = TreeEvaluator.EvaluatorType;

        public static IEvaluator GetEvaluator(string type = null) {
            if (type == null || type == DefaultEvaluatorType)
                return new TreeEvaluator();

            throw new Exception($"Cannot instance unknown evaluator '{type}'");
        }
    }
}
