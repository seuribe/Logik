using Logik.Core.Formula;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logik.Core {

    public delegate void CellEvent(ICell cell);
    public delegate void CellNameEvent(NumericCell cell, string name);

    public enum ErrorState {
        None = 0,
        Definition,
        Evaluation,
        CircularReference,
        Carried
    }

    public class NumericCell : ICell {
        private static readonly string DefaultCellFormula = "0";

        public string Name { get; private set; }

        private string formula = DefaultCellFormula;
        public string Formula {
            get => formula;
            set {
                formula = value;
                FormulaChanged?.Invoke(this);
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

        public bool Error { get; private set; }
        public string ErrorMessage { get; private set; }
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

        public event CellNameEvent NameChanged;
        public event CellEvent ValueChanged;
        public event CellEvent FormulaChanged;
        public event CellEvent DeleteRequested;
        public event CellEvent ErrorStateChanged;

        public HashSet<NumericCell> references = new HashSet<NumericCell>();
        public HashSet<NumericCell> deepReferences = new HashSet<NumericCell>();
        public HashSet<NumericCell> referencedBy = new HashSet<NumericCell>();

        public List<string> References() {
            var referenceNodes = EvalNode.Collect(node => node is ExternalReferenceNode);
            return referenceNodes.Select(node => (node as ExternalReferenceNode).Name).ToList();
        }

        public void SetError(string message) {
            Error = true;
            ErrorMessage = message;
            ErrorStateChanged?.Invoke(this);
        }

        public void Delete() {
            DeleteRequested?.Invoke(this);
        }

        public bool TryNameChange(string newName) {
            try {
                NameChanged?.Invoke(this, newName);
                Name = newName;
                ErrorMessage = "";
                return true;
            } catch (Exception e) {
                ErrorMessage = e.Message;
                return false;
            }
        }

        public void ClearError() {
            Error = false;
            ErrorMessage = "";
            ErrorStateChanged?.Invoke(this);
        }

        public NumericCell(string name) {
            Name = name;
        }
    }
}
