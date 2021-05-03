using System.Collections.Generic;

namespace Logik.Core {
    public interface IEvaluator {
        string Type { get; }
        void Define(NumericCell cell);
        void Define(TabularCell tcell);
        void Undefine(NumericCell cell);
        float Evaluate(NumericCell cell);
        List<string> References(NumericCell cell);
        void Rename(NumericCell cell, string newName);
    }
}
