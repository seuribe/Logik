using Logik.Core.Formula;
using System.Collections.Generic;

namespace Logik.Core {

    public delegate void CellEvent(ICell cell);
    public delegate void CellNameEvent(ICell cell, string name);

    public struct Value {
        private readonly string data;

        private Value(string data) {
            this.data = data;
        }

        public float AsFloat { get => (data == null) ? 0f : float.Parse(data); }
        public int AsInt { get  => (data == null) ? 0 : int.Parse(data); }
        public string AsString { get => (data == null) ? "" : data; }

        public static implicit operator Value(float value) => new Value(value.ToString());
        public static implicit operator Value(int value) => new Value(value.ToString());
        public static implicit operator Value(string value) => new Value(value);

        public static implicit operator float(Value value) => value.AsFloat;
        public static implicit operator int(Value value) => value.AsInt;
        public static implicit operator string(Value value) => value.data;

        public override string ToString() {
            return AsString;
        }
    }

    /// <summary>
    /// Interface for all cells in a model.
    /// Core functionalities are name, error state and references, and the listeners required
    /// to work with them.
    /// </summary>
    public interface ICell {
        string Name { get; }
        bool Error { get; }
        string ErrorMessage { get; }

        void SetError(string errorMessage);
        void ClearError();

        void ReEvaluate(EvalContext context);
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
