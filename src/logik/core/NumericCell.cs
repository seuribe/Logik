using System;
using System.Collections.Generic;

namespace Logik.Core {

    public delegate void CellEvent(NumericCell cell);
    public delegate void CellNameEvent(NumericCell cell, string name);

    public enum ErrorState {
        None = 0,
        Definition,
        Evaluation,
        CircularReference,
        Carried
    }

    public class NumericCell {
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

        public event CellNameEvent NameChanged;
        public event CellEvent ValueChanged;
        public event CellEvent FormulaChanged;
        public event CellEvent DeleteRequested;

        public HashSet<NumericCell> references = new HashSet<NumericCell>();
        public HashSet<NumericCell> deepReferences = new HashSet<NumericCell>();
        public HashSet<NumericCell> referencedBy = new HashSet<NumericCell>();

        public void SetError(string message) {
            Error = true;
            ErrorMessage = message;
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
        }

        public NumericCell(string name) {
            Name = name;
        }
    }}
