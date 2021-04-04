using System.Collections.Generic;

namespace Logik.Core {
    public interface IEvaluator {
        void Define(Cell cell);
        void Undefine(Cell cell);
        string Evaluate(Cell cell);
        List<string> References(Cell cell);
    }
}
