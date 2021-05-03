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

        event CellEvent ErrorStateChanged;
    }
}
