namespace Logik.Core.Formula {
    public interface IEvaluable {
        Value Eval(EvalContext context);
    }
}