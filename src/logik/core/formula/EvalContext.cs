namespace Logik.Core.Formula {
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