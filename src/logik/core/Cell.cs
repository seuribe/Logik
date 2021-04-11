using System;
using System.Collections.Generic;

namespace Logik.Core {

    public delegate void CellEvent(Cell cell);
    public delegate void CellNameEvent(Cell cell, string name);

    public enum ErrorState {
        None = 0,
        Definition,
        Evaluation,
        CircularReference,
        Carried
    }

    public class Cell {
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

        public event CellNameEvent NameChanged;
        public event CellEvent ValueChanged;
        public event CellEvent FormulaChanged;
        public event CellEvent DeleteRequested;

        public HashSet<Cell> references = new HashSet<Cell>();
        public HashSet<Cell> deepReferences = new HashSet<Cell>();
        public HashSet<Cell> referencedBy = new HashSet<Cell>();

        public void SetError(ErrorState newState, string message) {
            ErrorState = newState;
            Value = message;
        }

        public void Delete() {
            DeleteRequested?.Invoke(this);
        }

        public bool TryNameChange(string newName) {
            try {
                NameChanged?.Invoke(this, newName);
                Name = newName;
                return true;
            } catch (Exception e) {
                return false;
            }
        }

        public void ClearError() {
            ErrorState = ErrorState.None;
        }

        public Cell(string name) {
            Name = name;
        }
    }}
