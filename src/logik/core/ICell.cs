using Logik.Core.Formula;
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

    public interface IValue {
        float AsFloat { get; }
        int AsInt { get; }
        string AsString { get; }
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
