using Logik.Core.Formula;
using System.Collections.Generic;

namespace Logik.Core {

    public delegate void CellEvent(ICell cell);
    public delegate void CellNameEvent(ICell cell, string name);

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
