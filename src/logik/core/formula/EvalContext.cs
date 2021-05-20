namespace Logik.Core.Formula {

    /// <summary>
    /// An EvalContext provides the context needed for evaluating formulas which reference other cells,
    /// by using a ValueLookup and TabularLookup.
    /// </summary>
    public class EvalContext {
        public ValueLookup Lookup { get; }
        public TabularLookup TabularLookup { get; }

        public EvalContext(ValueLookup lookup, TabularLookup tabularLookup) {
            Lookup = lookup;
            TabularLookup = tabularLookup;
        }

        public static EvalContext EmptyContext = new EvalContext( name => new ValueNode(0), (name, row, column) => 0);
    }
}