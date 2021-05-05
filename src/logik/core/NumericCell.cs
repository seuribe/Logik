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
                DataChanged?.Invoke(this);
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
                OutputChanged?.Invoke(this);
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

        public override event CellEvent OutputChanged;
        public override event CellEvent DataChanged;

        public HashSet<NumericCell> references = new HashSet<NumericCell>();
        public HashSet<NumericCell> deepReferences = new HashSet<NumericCell>();
        public HashSet<NumericCell> referencedBy = new HashSet<NumericCell>();

        public List<string> References() {
            var referenceNodes = EvalNode.Collect(node => node is ExternalReferenceNode);
            return referenceNodes.Select(node => (node as ExternalReferenceNode).Name).ToList();
        }

        public NumericCell(string name) {
            Name = name;
        }
    }
}
