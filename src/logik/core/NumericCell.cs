using Logik.Core.Formula;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logik.Core {

    public class NumericCell : BaseCell {
        private static readonly string DefaultCellFormula = "0";

        private string formula = DefaultCellFormula;
        public string Formula {
            get => formula;
            set {
                formula = value;
                ContentChanged?.Invoke(this);
            }
        }

        private float value = 0;
        public float Value {
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

        private EvalNode evalNode;
        public EvalNode EvalNode {
            get {
                if (Error)
                    throw new LogikException("Cell has error, value unavailable");
                return evalNode;
            }
            internal set {
                evalNode = value;
            }
        }

        public override event CellEvent ValueChanged;
        public override event CellEvent ContentChanged;

        public NumericCell(string name) {
            Name = name;
        }

        public override void InternalUpdateValue() {
            Value = EvalNode.Eval();
        }

        public override void PrepareValueCalculation(EvalNodeBuilder nodeBuilder) {
            EvalNode = nodeBuilder.Build(Formula);
        }
    }
}
