using System;

namespace Logik.Core {
    public class LogikException : Exception {
        public LogikException(string message) : base(message) { }
    }
    
    public class CircularReference : LogikException {
        public CircularReference(string message) : base(message) { }
    }
}
