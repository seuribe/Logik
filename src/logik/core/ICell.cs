namespace Logik.Core {

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
        event CellEvent ValueChanged;
        event CellEvent DeleteRequested;
    }
}
