using System.Collections.Generic;

namespace Logik.Core {
    public interface IEvaluator {
        string Type { get; }
        void Define(Cell cell);
        void Undefine(Cell cell);
        string Evaluate(Cell cell);
        List<string> References(Cell cell);
        void Rename(Cell cell, string newName);
    }
}
