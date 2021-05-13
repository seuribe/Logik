using Logik.Core.Formula;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logik.Core {

    /// <summary>
    /// A formula-based cell, that can use the values of others to calculate a result and in turn
    /// be referenced by others.
    /// </summary>
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

        /// <summary>
        /// Cache for the result of the formula. Needs to be recomputed when the value of a
        /// referenced cell changes.
        /// </summary>
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

        /// <summary>
        /// cache for the evaluation tree/node generated from the formula, which when computed
        /// provides the value of the cell. Needs to be recomputed when the formula changes.
        /// </summary>
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

        /// <summary>
        /// Invoked when the output value changes
        /// </summary>
        public override event CellEvent ValueChanged;
        /// <summary>
        /// Invoked when the intrinsic internal value (formula) changes
        /// </summary>
        public override event CellEvent ContentChanged;

        public NumericCell(string name) {
            Name = name;
        }

        /// <summary>
        /// To be called by the model when the value of referenced cells change
        /// </summary>
        public override void InternalUpdateValue(EvalContext context) {
            Value = EvalNode.Eval(context);
        }

        /// <summary>
        /// To be called by the model when the formula changed, to recalculate the eval node
        /// </summary>
        /// <param name="nodeBuilder"></param>
        public override void PrepareValueCalculation(EvalNodeBuilder nodeBuilder) {
            EvalNode = nodeBuilder.Build(Formula);
        }

        /// <summary>
        /// Returns a list of names from other cells referenced by this. This will be used to
        /// calculate the references from the model.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetNamesReferencedInContent() {
            var referenceNodes = EvalNode.Collect(node => node is ExternalReferenceNode);
            return referenceNodes.Select(node => (node as ExternalReferenceNode).Name).ToList();
        }
    }
}
