using System;
using System.Collections.Generic;

namespace Logik.Core {
    public abstract class BaseCell : ICell {
        public string Name { get; protected set; }
        public bool Error { get; private set; }
        public string ErrorMessage { get; private set; }
        public HashSet<ICell> References { get; set; } = new HashSet<ICell>();
        public HashSet<ICell> DeepReferences { get; set; } = new HashSet<ICell>();
        public HashSet<ICell> ReferencedBy { get; set; } = new HashSet<ICell>();

        public abstract event CellEvent ValueChanged;

        public event CellEvent ErrorStateChanged;
        public event CellEvent DeleteRequested;
        public event CellNameEvent NameChanged;

        public bool TryNameChange(string newName) {
            try {
                NameChanged?.Invoke(this, newName);
                Name = newName;
                return true;
            } catch (Exception e) {
                SetError(e.Message);
                return false;
            }
        }

        public void Delete() {
            DeleteRequested?.Invoke(this);
        }

        public void ClearError() {
            Error = false;
            ErrorMessage = "";
            ErrorStateChanged?.Invoke(this);
        }
    
        public void SetError(string message) {
            Error = true;
            ErrorMessage = message;
            ErrorStateChanged?.Invoke(this);
        }

        public virtual void InternalUpdateValue() { }
    }
}
