using Logik.Core.Formula;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logik.Core {

    /// <summary>
    /// A formula-based cell, that can use the values of others to calculate a result and in turn
    /// be referenced by others.
    /// </summary>
    public class FormulaCell : BaseCell {
        private Formula.Formula formula = new Formula.Formula();

        public string Formula {
            get => formula.Text;
            set {
                formula = new Formula.Formula(value);
                ContentChanged?.Invoke(this);
            }
        }

        public IEvaluable Evaluable {
            get {
                if (Error)
                    throw new LogikException("Cell has error, value unavailable");

                return formula;
            }
        }

        /// <summary>
        /// Cache for the result of the formula. Needs to be recomputed when the value of a
        /// referenced cell changes.
        /// </summary>
        private Value value = 0;
        public Value Value {
            get {
                if (Error)
                    throw new LogikException("Cell has error, value unavailable");
                return value;
            }
            set {
                this.value = value;
                ValueChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Invoked when the output value changes
        /// </summary>
        public override event CellEvent ValueChanged;
        /// <summary>
        /// Invoked when the intrinsic internal value (formula) changes
        /// </summary>
        public override event CellEvent ContentChanged;

        public FormulaCell(string name) : base(name) { }

        /// <summary>
        /// To be called by the model when the value of referenced cells change
        /// </summary>
        public override void ReEvaluate(EvalContext context) {
            Value = formula.Eval(context);
        }

        /// <summary>
        /// Returns a list of names from other cells referenced by this. This will be used to
        /// calculate the references from the model.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetNamesReferencedInContent() {
            return formula.GetReferencedNames();
        }
    }
}
