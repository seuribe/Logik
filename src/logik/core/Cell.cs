using System;
using System.Collections.Generic;

namespace Logik.Core {

    public delegate void CellEvent(Cell cell);

    public enum ErrorState {
        None = 0,
        Definition,
        Evaluation,
        CircularReference,
        Carried
    }

    public class Cell {
        private static readonly string DefaultCellFormula = "0";

        public string Id { get; }

        private string formula = DefaultCellFormula;
        public string Formula {
            get => formula;
            set {
                formula = value;
                FormulaChanged?.Invoke(this);
            }
        }

        private string value = "0";
        public string Value {
            get => value;
            set {
                this.value = value;
                ValueChanged?.Invoke(this);
            }
        }

        public bool Error { get => ErrorState != ErrorState.None; }

        public ErrorState ErrorState { get; private set; }

        public event CellEvent ValueChanged;
        public event CellEvent FormulaChanged;

        public void SetError(ErrorState newState, string message) {
            ErrorState = newState;
            Value = message;
        }

        public void ClearError() {
            ErrorState = ErrorState.None;
        }

        public Cell(string id) {
            Id = id;
        }
    }}
