using Godot;
using System;

public class NameEdit : LineEdit {

	public event Action<string> TextChanged;

	public bool WorkMode { get; set; }

	public override void _Ready() {
		Connect("mouse_entered", this, "OnEnter");
		Connect("mouse_exited", this, "OnExit");
		Connect("focus_entered", this, "OnEnter");
		Connect("focus_exited", this, "OnExit");
		Connect("text_entered", this, "OnTextEntered");
	}

	private void OnEnter() {
		if (!WorkMode)
			Set("editable", true);
	}

	private void OnExit() {
		if (!HasFocus())
			Set("editable", false);
		TextChanged?.Invoke(Text);
	}

	private void OnTextEntered(string text) {
		TextChanged?.Invoke(text);
	}
}
