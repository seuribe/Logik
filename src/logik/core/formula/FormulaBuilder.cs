
namespace Logik.Core.Formula {

    public class FormulaBuilder {

        public static Formula Build(string formula) {
            return new Formula(formula);
        }
    }
}