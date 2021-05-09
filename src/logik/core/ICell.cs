﻿using Logik.Core.Formula;
using System.Collections.Generic;

namespace Logik.Core {

    public delegate void CellEvent(ICell cell);
    public delegate void CellNameEvent(ICell cell, string name);

    public enum ErrorState {
        None = 0,
        Definition,
        Evaluation,
        CircularReference,
        Carried
    }

    public struct Value {
        private readonly string data;
        private Value(string data) {
            this.data = data;
        }

        public float AsFloat { get => float.Parse(data); }
        public int AsInt { get  => int.Parse(data); }
        public string AsString { get => data; }

        public static implicit operator Value(float value) => new Value(value.ToString());
        public static implicit operator Value(int value) => new Value(value.ToString());
        public static implicit operator Value(string value) => new Value(value);

        public static implicit operator float(Value value) => value.AsFloat;
        public static implicit operator int(Value value) => value.AsInt;
        public static implicit operator string(Value value) => value.data;

    }

    public interface ICell {
        string Name { get; }
        bool Error { get; }
        string ErrorMessage { get; }

        void SetError(string errorMessage);
        void ClearError();

        void InternalUpdateValue();
        void PrepareValueCalculation(EvalNodeBuilder nodeBuilder);
        IEnumerable<string> GetNamesReferencedInContent();

        event CellEvent ErrorStateChanged;
        event CellNameEvent NameChanged;
        event CellEvent ValueChanged;
        event CellEvent ContentChanged;
        event CellEvent DeleteRequested;

        HashSet<ICell> References { get; set; }
        HashSet<ICell> DeepReferences { get; set; }
        HashSet<ICell> ReferencedBy { get; set; }
    }
}
