
namespace Logik.Core.Formula {

    public class FormulaBuilder {
        private readonly ValueLookup valueLookup;
        private readonly TabularLookup tabularLookup;

        public FormulaBuilder(ValueLookup valueLookup, TabularLookup tabularLookup) {
            this.valueLookup = valueLookup;
            this.tabularLookup = tabularLookup;
        }

        public Formula Build(string formula) {
            return new Formula(formula, valueLookup, tabularLookup);
        }
    }
}