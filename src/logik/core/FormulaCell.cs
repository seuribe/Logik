using Logik.Core.Formula;

namespace Logik.Core {
    public class FormulaCell : BaseCell {
        public override event CellEvent ValueChanged;
        public override event CellEvent ContentChanged;

        public string Formula { get => formula.Text; }

        public Value Value { get; private set; }

        protected Formula.Formula formula = new Formula.Formula();

        public void SetFormula(Formula.Formula formula) {
            this.formula = formula;
            ContentChanged?.Invoke(this);
        }

        public FormulaCell(string name) {
            Name = name;
        }

        public override void ReEvaluate(EvalContext context) {
            Value = formula.Eval(context);
            ValueChanged?.Invoke(this);
        }
    }
}
