namespace Logik.Core {
    public struct Value {
        private readonly string data;

        private Value(string data) {
            this.data = data;
        }

        public float AsFloat { get => (data == null) ? 0f : float.Parse(data); }
        public int AsInt { get  => (data == null) ? 0 : int.Parse(data); }
        public string AsString { get => (data == null) ? "" : data; }
        public bool AsBool { get => (data != null) && (data != "") && (data != "0"); }

        public static implicit operator Value(float value) => new Value(value.ToString());
        public static implicit operator Value(int value) => new Value(value.ToString());
        public static implicit operator Value(string value) => new Value(value);
        public static implicit operator Value(bool value) => new Value(value ? "1" : "0");

        public static implicit operator float(Value value) => value.AsFloat;
        public static implicit operator int(Value value) => value.AsInt;
        public static implicit operator string(Value value) => value.data;
        public static implicit operator bool(Value value) => value.AsBool;

        public override string ToString() {
            return AsString;
        }
    }
}
