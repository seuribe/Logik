using Godot;
using Logik.Core;

public class GridCell : NameEdit {
	public event GridCellEvent ContentChanged;

	public int Row { get; set; }
	public int Column { get; set; }

	public override void _Ready() {
		Connect("text_entered", this, nameof(OnTextEntered));
		Connect("text_changed", this, nameof(OnTextEntered));
		Connect("focus_entered", this, nameof(OnFocusEntered));
		Connect("focus_exited", this, nameof(OnFocusExited));
	}

	private void OnFocusExited() {
		Deselect();
    }

	private void OnFocusEntered() {
		SelectAll();
    }

	private void OnTextEntered(string newText) {
		ContentChanged?.Invoke(Row, Column);
	}
}
