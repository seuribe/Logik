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

        event CellEvent ErrorStateChanged;
        event CellNameEvent NameChanged;
        event CellEvent OutputChanged;
        event CellEvent DeleteRequested;
    }
}
